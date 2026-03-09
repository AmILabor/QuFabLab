
from django.db import transaction
from django.utils import timezone
from rest_framework import response, status
from rest_framework.generics import get_object_or_404
from rest_framework.response import Response

from api.filters.DispoFilters import  DispoTourFilter
from api.models.order import Order
from api.models.tour import Tour
from api.serializers.dispo.DispoTourSerializers import DispoTourSerializer, \
    DispoTourCreateSerializer, DispoTourSwapSerializer
from api.services import send_revocation_mail, send_tour_swap_email, send_tour_changed_email, \
    set_order_history

from api.views.dispo.DispoViewSet import DispoView
from utils.Exceptions import MailSendException, MailSwapException, TourDeleteException, MailCreateException
from utils.OrderHistoryActions import OrderHistoryActionEnum


class DispoTourViewSet(DispoView):
    queryset = Tour.objects.all().prefetch_related('tour_stops')
    serializer_class = DispoTourSerializer
    filterset_class = DispoTourFilter

    def list(self,request,**kwargs):
        qs = self.get_queryset()
        qs = self.filter_queryset(qs)
        output_serializer = self.serializer_class(qs, many=True)
        return response.Response(output_serializer.data)

    def update(self,request,*args,**kwargs):
        serialized = DispoTourSwapSerializer(data=request.data)
        serialized.is_valid(raise_exception=True)
        try:
            with transaction.atomic():
                tours_to_send = []
                tour_1_sent_at = None
                tour_2_sent_at = None
                tour_1 = Tour.objects.get(tour_date=serialized.validated_data["tour_date"],
                                               expert_employee=serialized.validated_data["expert_employee_1"])
                tour_2 = Tour.objects.get(tour_date=serialized.validated_data["tour_date"],
                                               expert_employee=serialized.validated_data["expert_employee_2"])
                tour_1.expert_employee_id = serialized.validated_data["expert_employee_2"]
                if tour_1.sent_at is not None:
                    tour_1_sent_at = tour_1.sent_at
                    tours_to_send.append(tour_2)
                if tour_2.sent_at is not None:
                    tour_2_sent_at = tour_2.sent_at
                    tours_to_send.append(tour_1)
                tour_1.sent_at=tour_2_sent_at
                tour_1.save()
                tour_2.expert_employee_id = serialized.validated_data["expert_employee_1"]
                tour_2.sent_at = tour_1_sent_at
                tour_2.save()
                for stop in tour_1.tour_stops.all():
                    stop.expert_employee_id = serialized.validated_data["expert_employee_2"]
                    stop.save()
                    if tour_1.sent_at is not None:
                        set_order_history(order_object=stop, user=request.user.mandate_user,order_history_action=OrderHistoryActionEnum.TOUR_MAIL_SENT,referenced_object=tour_1)
                for stop in tour_2.tour_stops.all():
                    stop.expert_employee_id = serialized.validated_data["expert_employee_1"]
                    if tour_2.sent_at is not None:
                        set_order_history(order_object=stop, user=request.user.mandate_user,order_history_action=OrderHistoryActionEnum.TOUR_MAIL_SENT,referenced_object=tour_2)
                    stop.save()
                if tour_1.sent_at is not None:
                    tour_1.sent_by_id = request.user.mandate_user_id
                if tour_2.sent_at is not None:
                    tour_2.sent_by_id = request.user.mandate_user_id

        except Exception as e:
            print(e)
            raise MailSwapException()
        send_tour_swap_email(tours=tours_to_send, sender_mail=request.user.mandate_user.email)
        return response.Response(status.HTTP_200_OK)


    def destroy(self,request,pk,*args,**kwargs):
        try:
            ob = get_object_or_404(pk=pk,queryset=self.get_queryset())
            object_has_been_sent = ob.sent_at is not None
            if object_has_been_sent:
                for stop in ob.tour_stops.all():
                    set_order_history(order_object=stop, user=request.user.mandate_user,
                                      order_history_action=OrderHistoryActionEnum.TOUR_MAIL_REVOKED, referenced_object=ob)
                send_revocation_mail(tour=ob, sender_mail=request.user.mandate_user.email)
            ob.delete()
        except MailSendException as e:
            raise e
        except Exception as e:
            print(e)
            raise TourDeleteException()

        return response.Response(status.HTTP_200_OK)

    def create(self, request, *args, **kwargs):
        try:
            tour_stops = request.data.pop("tour_stops")
        except Exception as e:
            exc = AttributeError(detail="Keine Touren (tour_stops) mitgesendet")
            raise (exc)

        serialized = DispoTourCreateSerializer(data=request.data,context=self.request.user)
        serialized.is_valid(raise_exception=True)
        try:
            with transaction.atomic():
                old_tour = Tour.objects.filter(tour_date = serialized.validated_data["tour_date"], expert_employee=serialized.validated_data["expert_employee"])

                if len(old_tour)>0:
                    tour_stop_ids = [x["id"] for x in tour_stops]
                    tour_stop_overlap = len(old_tour[0].tour_stops.filter(id__in=tour_stop_ids))
                    if tour_stop_overlap == len(tour_stop_ids) == len(old_tour[0].tour_stops.all()):
                        return response.Response(status=status.HTTP_200_OK)

                tour_instance = serialized.save()

                [x.delete() for x in old_tour]

                old_tour_ids = []
                for stop in tour_stops:
                    order_object = Order.objects.get(id=stop["id"])
                    if order_object.tour_id is not None:
                        old_tour_ids.append(order_object.tour_id)
                    order_object.tour_id = tour_instance.id
                    order_object.expert_employee = serialized.validated_data["expert_employee"]
                    order_object.save()
                if len(old_tour) > 0:
                    tour_instance = Tour.objects.get(pk=tour_instance.id)
                    tour_instance.sent_at = old_tour[0].sent_at
                    if tour_instance.sent_at:
                        send_tour_changed_email(tour_instance, request.user.mandate_user.email)
                    tour_instance.save()
                old_tour_ids = set(old_tour_ids)
                old_tours = Tour.objects.filter(id__in=old_tour_ids)
                for tour in old_tours:
                    if len(tour.tour_stops.all()) == 0:
                        if tour.sent_at is not None:
                            send_revocation_mail(tour,request.user.mandate_user.email)
                            for stop in tour.tour_stops.all():
                                set_order_history(order_object=stop, user=request.user.mandate_user,
                                                  order_history_action=OrderHistoryActionEnum.TOUR_MAIL_REVOKED,
                                                  referenced_object=tour)
                        tour.delete()
                    else:
                        if tour.sent_at is not None:
                            tour_instance.sent_at = timezone.now()
                            send_tour_changed_email(tour,request.user.mandate_user.email)
                            for stop in tour.tour_stops.all():
                                set_order_history(order_object=stop, user=request.user.mandate_user,
                                                  order_history_action=OrderHistoryActionEnum.TOUR_MAIL_SENT,
                                                  referenced_object=tour)
        except MailSendException as e:
            raise e
        except Exception as e:
            print(e)
            raise MailCreateException()
        return Response(status=status.HTTP_201_CREATED)
