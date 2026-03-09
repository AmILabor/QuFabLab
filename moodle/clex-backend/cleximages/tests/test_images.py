import hashlib
import json
import os
from unittest import TestCase

from django.core.files.uploadedfile import SimpleUploadedFile

from api.models import User, Auftrag, MandatBenutzer
from cleximages.models import ClexImageUser, AuftragBild
from cleximages.views import APITokenView, AuftragViewSet, AuftragImageViewSet
from rest_framework.test import APIRequestFactory, force_authenticate
from rest_framework.authtoken.models import Token


class ImageTest(TestCase):
    username = "testuser"
    password = "test123test321"
    test_order = 14
    test_user_id = 26
    test_filename = "__small_gif_testing.gif"

    @staticmethod
    def login_json():
        return {"username": ImageTest.username, "password": ImageTest.password}

    """
     id = models.IntegerField(primary_key=True,blank=True)
    auftrag_id = models.ForeignKey(to=Order,on_delete = models.DO_NOTHING, db_column="auftrag_id")
    benutzer_id = models.ForeignKey(to=MandatBenutzer,on_delete = models.DO_NOTHING, db_column="benutzer_id")
    upload_datetime = models.DateTimeField(auto_now=True)
    image = models.ImageField(upload_to=settings.CLEX_IMAGES_UPLOAD_PATH)
    image_hash = models.CharField(max_length=16, editable=False,blank=True)
    """

    def setUp(self):
        self.factory = APIRequestFactory()
        self.api_path = "/clex_images_api"
        self.view = AuftragImageViewSet
        self.sample_gif = (
            b'\x47\x49\x46\x38\x39\x61\x01\x00\x01\x00\x00\x00\x00\x21\xf9\x04'
            b'\x01\x0a\x00\x01\x00\x2c\x00\x00\x00\x00\x01\x00\x01\x00\x00\x02'
            b'\x02\x4c\x01\x00\x3b'
        )
        self.gif_md5 = hashlib.md5(self.sample_gif).hexdigest()
        self.test_file = SimpleUploadedFile(self.test_filename, self.sample_gif, content_type="image/gif")
        self.test_auftrag = Auftrag.objects.get(id=self.test_order)
        self.test_user = MandatBenutzer.objects.get(id=self.test_user_id)
        self.test_auftrag_bild = AuftragBild.objects.create(order_id=self.test_auftrag, user_id=self.test_user,
                                                            image=self.test_file)

    def tearDown(self) -> None:
        self.delete_all_test_images()

    def delete_all_test_images(self):
        images = AuftragBild.objects.filter(image__contains=self.test_filename.split(".")[0])
        for image in images:
            image.delete()

    @property
    def user(self):
        return User.objects.get(username=self.username)

    def test_successful_list_images(self):
        request = self.factory.get(f"{self.api_path}/images/?order_id={self.test_order}")
        force_authenticate(request, user=self.user)
        view_set = self.view.as_view({'get': 'list'})
        response = view_set(request)
        self.assertEqual(response.status_code, 200, "Order-Request should be successful.")
        self.assertGreater(len(response.data), 0, "There should be one or more images for this order id")
        files = [x for x in response.data if self.test_filename in x["image"]]
        self.assertEqual(len(files), 1,
                         f"There should be one test image with the name {self.test_filename} but only {[x['image'] for x in response.data]} are present")
        self.assertGreater(len(response.data), 0, "There should be more than 0 results")
        result = response.data[0]
        self.assertIn("user_id", result, "Response should include user_id")
        self.assertIn("order_id", result, "Response should include order_id")
        user_id = result["user_id"]
        order_id = result["order_id"]
        self.assertEqual(user_id, self.user.mandate_user.id,
                         f"Image should have mandate user id {self.user.mandate_user.id} but has {user_id}")
        self.assertEqual(order_id, self.test_order, f"Image should have order id {self.test_order} but has {order_id}")

    def test_successful_create_image(self):
        self.test_file.seek(0)
        request = self.factory.post(f"{self.api_path}/images/?order_id={self.test_order}", data={'image': self.test_file},
                                    format="multipart")
        force_authenticate(request, user=self.user)
        view_set = self.view.as_view({'post': 'create'})
        response = view_set(request, user=self.user)
        self.assertEqual(response.status_code, 200,
                         f"Uploading an image should result in 200 status code. But is {response.status_code}")

    def test_unsucessful_create_image(self):
        bad_file =  b'\x02\x4c\x01\x00\x3b'
        bad_filename = "test.xml"
        bad_file = SimpleUploadedFile(bad_filename, bad_file, content_type="application/xml")
        request = self.factory.post(f"{self.api_path}/images/?order_id={self.test_order}",
                                    data={'image': bad_file},
                                    format="multipart")
        force_authenticate(request, user=self.user)
        view_set = self.view.as_view({'post': 'create'})
        response = view_set(request, user=self.user)
        self.assertNotEqual(response.status_code, 200,
                         f"Uploading an malicious file aka. xml should not be allowed.")


    def test_malicious_md5(self):
        self.test_file.seek(0)
        request = self.factory.post(f"{self.api_path}/images/?order_id={self.test_order}&?image_hash=46",
                                    data={'image': self.test_file},
                                    format="multipart")
        force_authenticate(request, user=self.user)
        view_set = self.view.as_view({'post': 'create'})
        response = view_set(request, user=self.user)
        self.assertEqual(response.status_code, 200,
                         f"Uploading an image should result in 200 status code. But is {response.status_code}")
        latest_image = AuftragBild.objects.filter(image__contains=self.test_filename.split(os.sep)[0]).latest("upload_datetime")
        self.assertEqual(latest_image.image_hash,self.gif_md5,"Image Hash should not differ by any means.")

    def test_unsucessful_create_image_order_id(self):
        request = self.factory.post(f"{self.api_path}/images/?order_id=-1",
                                    data={'image': self.test_file},
                                    format="multipart")
        force_authenticate(request, user=self.user)
        view_set = self.view.as_view({'post': 'create'})
        response = view_set(request, user=self.user)
        self.assertNotEqual(response.status_code, 200,
                         f"Creating an reference to order_id=-1 should not be possible")

    def test_md5_generation(self):
        request = self.factory.get(f"{self.api_path}/images/?order_id={self.test_order}")
        force_authenticate(request, user=self.user)
        view_set = self.view.as_view({'get': 'list'})
        response = view_set(request)
        files = [x for x in response.data if self.test_filename in x["image"]]
        file = files[0]
        computed_hash = file["image_hash"]
        self.assertEqual(computed_hash, self.gif_md5, f"Hash should be equal but is {computed_hash} and {self.gif_md5}")

    def test_delete_image(self):
        image = AuftragBild.objects.filter(image__contains=self.test_filename.split(os.sep)[0]).latest("upload_datetime")
        image_id = image.id
        request = self.factory.delete(f"{self.api_path}/images/?image_id={image_id}")
        force_authenticate(request, user=self.user)
        view_set = self.view.as_view({'delete': 'delete'})
        response = view_set(request)
        self.assertEqual(response.status_code,200,"DELETE should result in 200")
        images = AuftragBild.objects.all()
        ids = [x.id for x in images]
        self.assertNotIn(image_id,ids,f"ImageID {image_id} should have been deleted.")