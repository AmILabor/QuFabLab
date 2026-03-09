from rest_framework import status, response
from api.models.file import File
from api.models.order import Order
from api.serializers.dispo.DispoAttachmentSerializers import AttachmentSerializer, AttachmentParameterSerializer, \
    AttachmentCreateSerializer
from api.services import generate_file_representation, write_out_file, set_order_history
from api.views.dispo.DispoViewSet import DispoView
from utils.OrderHistoryActions import OrderHistoryActionEnum


class DispoAttachmentViewSet(DispoView):
    queryset = File.objects.all()
    serializer_class = AttachmentSerializer

    def retrieve(self, request, pk=None):
        qs = self.get_queryset().filter(order_id=pk)
        serialized = self.serializer_class(qs,many=True)
        return response.Response(serialized.data)

    def create(self, request, *args, **kwargs):
        parameter_serializer = AttachmentParameterSerializer(data=request.data)
        parameter_serializer.is_valid(raise_exception=True)
        serialized_data = parameter_serializer.validated_data
        file_representation = generate_file_representation(file_name=serialized_data["file"].name,
                                                           order_id=serialized_data["order_id"])
        order = Order.objects.get(pk=serialized_data["order_id"])
        set_order_history(order_object=order, user=request.user.mandate_user, referenced_object=file_representation,
                          order_history_action=OrderHistoryActionEnum.ATTACHMENT_ADDED)
        serialized = AttachmentCreateSerializer(data=file_representation)
        serialized.is_valid(raise_exception=True)
        if write_out_file(file_name=serialized.validated_data["name"], file_extension=serialized.validated_data["extension"],
                          file=serialized_data["file"]):
            serialized.save()
            return response.Response(status=status.HTTP_200_OK)
        else:
            print("Error on storing file")
            return response.Response(status=status.HTTP_500_INTERNAL_SERVER_ERROR)
