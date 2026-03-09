from django.db import models
from django.contrib.auth import models as c_models

class User(c_models.User):
    location = models.CharField(max_length=64)
    def __str__(self):
        return self.username
    def __unicode__(self):
        return self.username

class HairColor(models.Model):
    name = models.CharField(max_length=64)
    def __str__(self):
        return self.name
    def __unicode__(self):
        return self.name

class Color(models.Model):
    name = models.CharField(max_length=64)
    def __str__(self):
        return self.name

class Puppet(models.Model):
    name = models.CharField(max_length=64)
    serial = models.CharField(max_length=64)
    connector = models.CharField(max_length=64)
    hair_color = models.ForeignKey(HairColor,on_delete=models.DO_NOTHING,related_name="hair_color")
    shirt_color = models.ForeignKey(Color,on_delete=models.DO_NOTHING,related_name="shirt_color")
    pants_color = models.ForeignKey(Color,on_delete=models.DO_NOTHING,related_name="pants_color")
    shoe_color = models.ForeignKey(Color,on_delete=models.DO_NOTHING,related_name="shoe_color")
    shirt_name = models.CharField(max_length=64)
    picture = models.FileField(upload_to="uploads/")
    handler = models.ForeignKey(User,on_delete=models.DO_NOTHING)
    last_edit = models.DateTimeField(auto_now=True)
    def __str__(self):
        return self.name

class Issue(models.Model):
    done = models.BooleanField()
    published = models.DateTimeField(auto_now=True)
    description = models.TextField()
    plan = models.TextField(blank=True)
    creator = models.ForeignKey('auth.User', on_delete=models.DO_NOTHING, related_name="issuecreator")
    handler = models.ForeignKey(User,on_delete=models.DO_NOTHING, related_name="issuehandler", null=True)
    resolution = models.TextField(blank=True)
    resolution_date = models.DateTimeField(blank=True,null=True)
    comment = models.TextField(blank=True)
    puppet = models.ForeignKey(Puppet,on_delete=models.DO_NOTHING,related_name='issues')
    last_edit = models.DateTimeField(auto_now=True)

    def __str__(self):
        return self.puppet.name+"("+str(self.puppet.id)+"): "+str(self.id)+": '"+self.description[:15]+"'"

class IssueData(models.Model):
    ref = models.FileField(upload_to="uploads/")
    title = models.CharField(max_length=64)
    issue = models.ForeignKey(Issue,on_delete=models.CASCADE,related_name='data')

    def __str__(self):
        return self.title
    def __unicode__(self):
        return self.title


