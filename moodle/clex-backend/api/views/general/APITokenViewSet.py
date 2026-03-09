from rest_framework import response, status
from rest_framework.authtoken.models import Token as AuthToken
from rest_framework.authtoken.serializers import AuthTokenSerializer as APILoginSerializer
from rest_framework.permissions import IsAuthenticated
from rest_framework.views import APIView

from api.serializers.general.EmployeeSerializers import UserSerializer
from api.serializers.general.APITokenSerializers import APITokenSerializer


class APITokenView(APIView):
    def get(self,request,pk=None):
        auth_ob = request.auth.user
        output_serializer = UserSerializer(auth_ob)
        if request.auth.user.mandate_user.active in [0,None]:
            request.user.auth_token.delete()
            return response.Response(status=status.HTTP_401_UNAUTHORIZED)
        return response.Response(output_serializer.data, status=status.HTTP_200_OK)

    def post(self, request, *args, **kwargs):
        """
        Returns an authorization token used to access API endpoints which require authentication
        """
        input_serializer = APILoginSerializer(data=request.data, context={'request': request})
        if input_serializer.is_valid(raise_exception=True):
            if input_serializer.validated_data["user"].mandate_user.active  in [0,None]:
                return response.Response(status=status.HTTP_401_UNAUTHORIZED)
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
        permission_classes=[]
        #if self.request.method == 'POST':
        #    permission_classes = [perms.IsAnonymous]
        if self.request.method in ['GET', 'DELETE']:
            permission_classes = [IsAuthenticated]
        return [permission() for permission in permission_classes]