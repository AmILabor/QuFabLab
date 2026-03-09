from api.models import *
from rest_framework import serializers

class HairColorSerializer(serializers.HyperlinkedModelSerializer):
    class Meta:
        model=HairColor
        fields = ['name','id']

class ColorSerializer(serializers.HyperlinkedModelSerializer):
    class Meta:
        model=Color
        fields = ['name','id']

class UserSerializer(serializers.HyperlinkedModelSerializer):
    class Meta:
        model=User
        fields = ['username','id','location']

class PuppetListSerializer(serializers.ModelSerializer):
    hair_color = HairColorSerializer()
    shirt_color = ColorSerializer()
    pants_color = ColorSerializer()
    handler = UserSerializer()
    issue_count = serializers.SerializerMethodField()
    issue_count_open = serializers.SerializerMethodField()
    issue_count_closed = serializers.SerializerMethodField()
    class Meta:
        model=Puppet
        fields = ['id','shirt_name','serial','name','hair_color','shirt_color','pants_color','handler','shoe_color','picture','issue_count','issue_count_open','issue_count_closed']

    def get_issue_count(self,obj):
        return obj.issues.count()

    def get_issue_count_open(self,obj):
        return len(obj.issues.exclude(done=True))

    def get_issue_count_closed(self,obj):
        return len(obj.issues.exclude(done=False))




# ======== EDITABLE =========
class IssueDataSerializer(serializers.ModelSerializer):
    class Meta:
        model=IssueData
        fields = ['id','title','ref','issue']
    
class FullIssueSerializer(serializers.ModelSerializer):
    data = IssueDataSerializer(many=True,required=False)
    class Meta:
        model=Issue
        fields=['id','last_edit','done','description','plan','handler','creator','resolution','resolution_date','comment','data','puppet']
        read_only_fields=['creator']


class FullPuppetSerializer(serializers.ModelSerializer):
    issues = FullIssueSerializer(many=True,required=False)

    class Meta:
        model=Puppet
        fields = ['id','picture','last_edit','shirt_name','serial','name','hair_color','shirt_color','pants_color','handler','issues','connector','shoe_color']
