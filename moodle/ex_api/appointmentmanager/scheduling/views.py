from django.http import HttpResponse, HttpResponseRedirect, HttpResponseNotAllowed, Http404, HttpResponseNotModified
from django.urls import reverse
from django.shortcuts import render, get_object_or_404
from django.contrib.auth.decorators import login_required
from .models import CustomerTemplate, Firma, Auftrag, Newsletter, SentEmailSettings
from .management.commands.send_mails import EmailDetailsOrder, EmailDetailsUser, Command, EmailDetailsAddress
from datetime import datetime
import pytz

# Create your views here.


@login_required()
def index(request):
    setting_object = SentEmailSettings.objects.filter(remind_interval_hours__isnull=False)[0]
    firmen = Firma.objects.all()
    tasks = []

    for firma in firmen:
        o = {
            'name': firma.name1,
            'id': firma.id,
            'confirmation': setting_object.default_confirmation,
            'reminder': setting_object.default_reminder,
            'content': '',
        }

        try:
            template = CustomerTemplate.objects.get(firma=firma.id)
            o['confirmation'] = template.confirmation
            o['reminder'] = template.reminder
            o['content'] = template.content
        except Exception:
            pass

        tasks.append(o)

    placeholders = EmailDetailsUser.__annotations__.keys() | EmailDetailsOrder.__annotations__.keys() | EmailDetailsAddress.__annotations__.keys()

    context = {
        'tasks': tasks,
        'placeholders': list(placeholders),
        'tag': Command.template_tag
    }
    return HttpResponse(render(request, 'index.html', context))


@login_required()
def update(request, firma_id):
    obj, _ = CustomerTemplate.objects.get_or_create(firma=firma_id)
    obj.confirmation = request.POST['confirmation']
    obj.reminder = request.POST['reminder']
    obj.content = request.POST['content']
    obj.full_clean()
    obj.save()
    return HttpResponseRedirect(reverse('index'))


def add_newsletter(request):
    """
    Adds sent e-mail to newsletter table if user has submitted a matching Auftrag ID
    NYI in client production, will be available later maybe
    """
    return HttpResponse(status=501)

    if request.method != 'GET':
        return HttpResponseNotAllowed(permitted_methods=['GET'])

    auftrag_id = request.GET['auftrag']
    email = request.GET['email']

    if Newsletter.objects.filter(email=email).exists():
        error = f'{email} ist bereits im Newsletter eingetragen'
        print(error)
        return HttpResponseNotModified()

    try:
        auftrag = Auftrag.objects.get(pk=auftrag_id)
        if auftrag.email != email:
            raise Exception('mail not suiting')
    except Exception as e:
        error = f'Auftrag {auftrag_id} für {email} wurde nicht gefunden: ' + str(e)
        print(error)
        return Http404(error)

    Newsletter.objects.create(email=email, date=datetime.now())
    return HttpResponse(status=204)
