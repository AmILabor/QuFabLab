from django.core.management.base import BaseCommand
from scheduling.models import SentEmailSettings

class Command(BaseCommand):
    help = "Creates sentemailsettings-Object for database"

    def add_arguments(self, parser):
        parser.add_argument('--interval', type=int,help="Send Email interval for reminders",required=True)
        parser.add_argument('--log_dir', help="Logfile-Directory",required=True)
        parser.add_argument('--newsletter_link', help="Newsletter Link",required=True)
        parser.add_argument('--newsletter_link_text', help="Newsletter Link-Text",required=True)
        parser.add_argument('--smtp_server', help="SMTP-Server to use",required=True)
        parser.add_argument('--smtp_port', help="SMTP-Server Port to use",required=True)
        parser.add_argument('--smtp_user', help="SMTP-Server User ",required=True)
        parser.add_argument('--smtp_password', help="SMTP-Server Password",required=True)
        parser.add_argument('--protocol',choices=['ssl','tls','none'],required=True)

    def handle(self, *args, **options):
        settings = SentEmailSettings.objects.filter(remind_interval_hours__isnull=False)
        ssl = False
        tls = False
        if options["protocol"] =="ssl":
            ssl = True
        elif options["protocol"] =="tls":
            tls=True
        if len(settings)==0:
            print("Creating a Settings-Object")
            setting = SentEmailSettings.objects.create(remind_interval_hours=options["interval"],logfile_dir=options["log_dir"],
                                                       newsletter_link=options["newsletter_link"],newsletter_link_text=options["newsletter_link_text"],
                                                       smtp_server=options["smtp_server"],smtp_port=options["smtp_port"],smtp_user=options["smtp_user"],
                                                       smtp_password=options["smtp_password"],use_ssl=ssl,use_tls=tls)
            setting.save()
        else:
            print("Editing Settings-Object")
            setting = settings[0]
            setting.remind_interval_hours=options["interval"]
            setting.logfile_dir=options["log_dir"]
            setting.newsletter_link=options["newsletter_link"]
            setting.newsletter_link_text=options["newsletter_link_text"]
            setting.smtp_server=options["smtp_server"]
            setting.smtp_port=options["smtp_port"]
            setting.smtp_user=options["smtp_user"]
            setting.smtp_password=options["smtp_password"]
            setting.use_ssl=ssl
            setting.use_tls=tls
            setting.save()


