import json
from unittest import TestCase

from api.models import User
from cleximages.models import ClexImageUser
from cleximages.views import APITokenView, AuftragViewSet
from rest_framework.test import APIRequestFactory, force_authenticate
from rest_framework.authtoken.models import Token

class AuthenticationTest(TestCase):
    username = "testuser"
    password = "test123test321"

    @staticmethod
    def login_json():
        return {"username":AuthenticationTest.username, "password":AuthenticationTest.password}

    def setUp(self):
        self.factory = APIRequestFactory()
        self.api_path = "/clex_images_api"
        self.view = APITokenView.as_view()
        self.clear_existing_logins()


    @property
    def user(self):
        return User.objects.get(username=self.username)

    def clear_existing_logins(self):
        try:
            user_id = User.objects.filter(username=self.username).get().id
        except:
            raise Exception("Test-User was not found for clearing auth tokens.")
        tokens= Token.objects.filter(user=user_id)
        for token_obj in tokens:
            token_obj.delete()
        pass

    def test_unsuccessful_login(self):
        json_login = self.login_json()
        wrong_login_username = json_login.copy()
        wrong_login_password = json_login.copy()
        wrong_login_username["username"] = json_login["username"]+json_login["username"]
        wrong_login_password["password"] = json_login["password"]+json_login["password"]
        complete_wrong_login = wrong_login_username.copy()
        complete_wrong_login["password"] = wrong_login_password["password"]
        false_tests = [wrong_login_username,wrong_login_username,complete_wrong_login]
        for false_test in false_tests:
            request = self.factory.post(f"{self.api_path}/auth/",false_test, format='json')
            response = self.view(request)
            self.assertEqual(response.status_code, 400, "Request to auth api was successfull but should not be")

    def test_successful_login(self):
        request = self.factory.post(f"{self.api_path}/auth/",self.login_json(),format='json')
        response = self.view(request)
        self.assertEqual(response.status_code,201,"Login-Request to auth api was not successfull")
        self.assertIn("token",response.data,"token attribute not found in response")

    def test_successful_api_access(self):
        request = self.factory.get(f"{self.api_path}/orders/", self.login_json(), format='json')
        force_authenticate(request,user=self.user)
        response = AuftragViewSet.as_view({'get':'list'})(request)
        self.assertEqual(response.status_code,200,"We should be able to get orders after we log in.")

    def test_successful_logout(self):
        request = self.factory.delete(f"{self.api_path}/auth/")
        token = Token.objects.create(user=self.user)
        force_authenticate(request,user=self.user)
        response = self.view(request)
        self.assertEqual(response.status_code,204,"Logout-Request to auth api was not successfull")
        tokens = Token.objects.filter(user=self.user)
        self.assertEqual(len(tokens),0,f"There should be no token left in token model for user {self.username}")

    def test_unsuccessful_api_access(self):
        request = self.factory.get(f"{self.api_path}/orders/", self.login_json(), format='json')
        response = AuftragViewSet.as_view({'get': 'list'})(request)
        self.assertEqual(response.status_code, 401, "We should not be able to get orders without login")



