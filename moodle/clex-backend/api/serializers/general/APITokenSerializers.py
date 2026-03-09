from rest_framework import serializers


class APITokenSerializer(serializers.Serializer):
    token = serializers.SerializerMethodField()

    def get_token(self, obj):
        return f'Token {obj.key}'