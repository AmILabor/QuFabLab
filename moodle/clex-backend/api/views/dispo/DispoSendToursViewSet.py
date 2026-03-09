from django.utils import timezone
from rest_framework import status
from rest_framework.exceptions import ParseError
from rest_framework.response import Response
from api.models.tour import Tour
from api.serializers.dispo.DispoTourSerializers import DispoTourSerializer
from api.services import send_tour_email

from api.views.dispo.DispoViewSet import DispoView
from utils.Exceptions import TourEditException


class DispoSendToursViewSet(DispoView):
    queryset = Tour.objects
    serializer_class = DispoTourSerializer

    def create(self, request, *args, **kwargs):
        if "tour_id" not in request.data:
            raise ParseError(detail="tour_id not provided")
        try:
            tour_id = request.data["tour_id"]
            sender_mail = request.user.mandate_user.email
            tour = self.get_queryset().get(pk=tour_id)
            tour.sent_at = timezone.now()
            tour.sent_by_id=request.user.mandate_user_id
            tour.save()
        except Exception as e:
            print(e)
            raise TourEditException()
        send_tour_email(tour,sender_mail)
        return Response(status=status.HTTP_201_CREATED)
