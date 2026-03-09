from django.utils.html import strip_tags as dj_strip_tags

def strip_tags(val):
    val = val.replace("<br/>","\n")
    val = val.replace("<br>","\n")
    return dj_strip_tags(val)