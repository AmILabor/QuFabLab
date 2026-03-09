from django.db.models import QuerySet
from django.shortcuts import render
from rest_framework import viewsets, response, permissions, serializers
from rest_framework.views import APIView
from rest_framework.permissions import IsAuthenticated

from api.models.order import Order
from cleximages.permissions import IsClexImagesUser
from cleximages.serializers import AuftragListSerializer, ClexImageSerializer, APITokenSerializer, \
    ClexImageSerializerPost
from rest_framework.authtoken.models import Token as AuthToken
from rest_framework.authtoken.serializers import AuthTokenSerializer as APILoginSerializer
from cleximages.selectors import select_orders_by_plate, select_orders_for_today, select_address_by_order
from cleximages.models import AuftragBild
# Create your views here.
from rest_framework import status
import api.permissions as perms


class APITokenView(APIView):
    def post(self, request, *args, **kwargs):
        """
        Returns an authorization token used to access API endpoints which require authentication
        """
        input_serializer = APILoginSerializer(data=request.data, context={'request': request})
        if input_serializer.is_valid(raise_exception=True):
            user = input_serializer.validated_data['user']
            token, _ = AuthToken.objects.get_or_create(user=user)
            output_serializer = APITokenSerializer(token)
            return response.Response(output_serializer.data, status=status.HTTP_201_CREATED)

    def delete(self, request, *args, **kwargs):
        """
        Deletes the user's authentication token and logs them out
        """
        request.user.auth_token.delete()
        return response.Response(status=status.HTTP_204_NO_CONTENT)

    def get_permissions(self):
        if self.request.method == 'POST':
            permission_classes = [perms.IsAnonymous]
        elif self.request.method in ['GET', 'DELETE']:
            permission_classes = [IsAuthenticated]
        return [permission() for permission in permission_classes]

class AuftragImageViewSet(viewsets.GenericViewSet):
    queryset = AuftragBild.objects.all()
    serializer_class = ClexImageSerializer
    serializer_class_post = ClexImageSerializerPost

    def create(self,request,*args,**kwargs):
        user_id = request.user.mandate_user.id
        order_id = request.query_params.get("order_id")
        if order_id is None:
            return response.Response("{'error':'Order id not set'}",status=status.HTTP_400_BAD_REQUEST)
        data = request.data.copy()
        data["user_id"] = user_id
        data["order_id"] = order_id
        serializer = self.serializer_class_post(data=data)
        if serializer.is_valid():
            serializer.save()
        else:
            return response.Response("{'error':'Submitted data could not be serialized'}",status=status.HTTP_400_BAD_REQUEST)
        return response.Response("", status=status.HTTP_200_OK)

    def list(self,request,*args,**kwargs):
        order_id = request.query_params.get("order_id")
        if order_id is None:
            return response.Response("",status=status.HTTP_400_BAD_REQUEST)
        qs = AuftragBild.objects.filter(order_id=order_id)
        output_serializer = ClexImageSerializer(qs, many=True)
        return response.Response(output_serializer.data)

    def delete(self,request,*args,**kwargs):
        image_id = request.query_params.get("image_id")
        if image_id is not None:
            image = AuftragBild.objects.get(pk=image_id)
            """
            TODO: Check order-State(Wich field?) here to permit deletion if order is blocked. 
            - Rechunng verschickt?
            """
            if not image:
                return response.Response("", status=status.HTTP_500_INTERNAL_SERVER_ERROR)
            image.delete()
            return response.Response("",status=status.HTTP_200_OK)



    def get_permissions(self):
        return [IsClexImagesUser()]

class AuftragViewSet(viewsets.GenericViewSet):
    queryset = Order.objects.all()
    def get_permissions(self):
        return [IsClexImagesUser()]

    def get(self,request,*args,**kwargs):
        pass

    def list(self, request, *args, **kwargs):
        license_plate = request.query_params.get("license_plate")
        expert_employee_id = request.user.mandate_user.id
        if license_plate is not None:
            qs = select_orders_by_plate(license_plate=license_plate)
        elif expert_employee_id is not None:
            qs =  select_orders_for_today(expert_employee_id=expert_employee_id)
        else:
            return response.Response("",status=status.HTTP_400_BAD_REQUEST)
        output_serializer = AuftragListSerializer(qs, many=True)
        return response.Response(output_serializer.data)
