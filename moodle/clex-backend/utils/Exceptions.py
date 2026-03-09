from rest_framework.exceptions import APIException

class PasswordWrongException(APIException):
    status_code = 400
    default_detail = 'Das (alte) Passwort ist nicht richtig.'
    default_code = 'error'

class MailSwapException(APIException):
    status_code = 500
    default_detail = 'Die Touren konnten nicht getauscht werden (Server-Fehler)'
    default_code = 'error'

class TourDeleteException(APIException):
    status_code = 500
    default_detail = 'Die Tour konnte nicht gelöscht werden'
    default_code = 'error'

class TourEditException(APIException):
    status_code = 500
    default_detail = 'Die Tour konnte nicht bearbeitet werden.'
    default_code = 'error'

class MailCreateException(APIException):
    status_code = 500
    default_detail = 'Die Tour konnte nicht erstellt / geändert werden.'
    default_code = 'error'

class MailSendException(APIException):
    status_code = 500
    default_detail = 'Die Emails konnten nicht versendet werden.'
    default_code = 'error'