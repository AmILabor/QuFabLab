import json
from unittest import TestCase

from api.models import User
from cleximages.models import ClexImageUser
from cleximages.views import APITokenView, AuftragViewSet
from rest_framework.test import APIRequestFactory, force_authenticate
from rest_framework.authtoken.models import Token

class OrderTest(TestCase):
    username = "testuser"
    password = "test123test321"
    test_plate = 'HH-VE 1462'


    @staticmethod
    def login_json():
        return {"username":OrderTest.username, "password":OrderTest.password}

    def setUp(self):
        self.factory = APIRequestFactory()
        self.api_path = "/clex_images_api"
        self.view = AuftragViewSet


    @property
    def user(self):
        return User.objects.get(username=self.username)

    def test_successful_list_orders(self):
        request = self.factory.get(f"{self.api_path}/orders/")
        force_authenticate(request,user=self.user)
        view_set = self.view.as_view({'get':'list'})
        response = view_set(request)
        self.assertEqual(response.status_code, 200, "Order-Request should be successful.")
        self.assertEqual(len(response.data),2,"There should be two results for debug_date")
        for order in response.data:
            self.assertEqual(order["expert_employee_id"],self.user.mandate_user.id,"Order.expert_employee_id does not match mandate_user__id")

    def test_successful_list_order_plate(self):
        request = self.factory.get(f"{self.api_path}/orders/?license_plate={self.test_plate}")
        force_authenticate(request, user=self.user)
        view_set = self.view.as_view({'get': 'list'})
        response = view_set(request)
        self.assertEqual(response.status_code, 200, "Order-Request should be successful.")
        self.assertEqual(len(response.data), 1, "There should be one results for the plate with debug_date")
        for order in response.data:
            self.assertEqual(order["expert_employee_id"],self.user.mandate_user.id,"Order.expert_employee_id does not match mandate_user__id")


    def test_unsuccessful_list_order_plate(self):
        request = self.factory.get(f"{self.api_path}/orders/?license_plate=XX-YY 13376891")
        force_authenticate(request, user=self.user)
        view_set = self.view.as_view({'get': 'list'})
        response = view_set(request)
        self.assertEqual(response.status_code, 200, "Order-Request should be successful.")
        self.assertEqual(len(response.data), 0, "There should be no results for imagniary plate")


