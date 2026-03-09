from enum import IntEnum


class HTTPStatusGerman(IntEnum):
    """HTTP status codes and reason phrases

    Status codes from the following RFCs are all observed:

        * RFC 7231: Hypertext Transfer Protocol (HTTP/1.1), obsoletes 2616
        * RFC 6585: Additional HTTP Status Codes
        * RFC 3229: Delta encoding in HTTP
        * RFC 4918: HTTP Extensions for WebDAV, obsoletes 2518
        * RFC 5842: Binding Extensions to WebDAV
        * RFC 7238: Permanent Redirect
        * RFC 2295: Transparent Content Negotiation in HTTP
        * RFC 2774: An HTTP Extension Framework
        * RFC 7725: An HTTP Status Code to Report Legal Obstacles
        * RFC 7540: Hypertext Transfer Protocol Version 2 (HTTP/2)
    """
    def __new__(cls, value, phrase, description=''):
        obj = int.__new__(cls, value)
        obj._value_ = value

        obj.phrase = phrase
        obj.description = description
        return obj

    # informational
    CONTINUE = 100, 'Continue', 'Anfrage erhalten, bitte weiter.'
    SWITCHING_PROTOCOLS = (101, 'Switching Protocols',
            'Protokoll wird gewechselt; beachte Upgrade header')
    PROCESSING = 102, 'Bearbeite'

    # success
    OK = 200, 'OK', 'Anfrage bearbeitet, Dokument folgt'
    CREATED = 201, 'Created', 'Dokument erstellt, URL folgt'
    ACCEPTED = (202, 'Accepted',
        'Anfrage akzeptiert, bearbeitung erfolgt off-line')
    NON_AUTHORITATIVE_INFORMATION = (203,
        'Non-Authoritative Information', 'Anfrage vom Cache verarbeitet')
    NO_CONTENT = 204, 'No Content', 'Anfrage verarbeitet, nichts folgt'
    RESET_CONTENT = 205, 'Reset Content', 'Eingabe Formular leeren für weitere Eingaben'
    PARTIAL_CONTENT = 206, 'Partial Content', 'Partieller Inhalt folgt'
    MULTI_STATUS = 207, 'Multi-Status'
    ALREADY_REPORTED = 208, 'Already Reported'
    IM_USED = 226, 'IM Used'

    # redirection
    MULTIPLE_CHOICES = (300, 'Multiple Choices',
        'Objekt hat mehrere resourcen -- URI-Liste beachten')
    MOVED_PERMANENTLY = (301, 'Moved Permanently',
        'Objekt wurde permanent verschoben -- URI-Liste beachten')
    FOUND = 302, 'Found', 'Objekt wurde temporär verschoben -- URI-Liste beachten'
    SEE_OTHER = 303, 'See Other', 'Objekt verschoben -- Methoden und URI-Liste beachten'
    NOT_MODIFIED = (304, 'Not Modified',
        'Dokument hat sich seither nicht verändert')
    USE_PROXY = (305, 'Use Proxy',
        'Die spezifizierte Proxy-location muss genutzt werden um auf die Resource zuzugreifen.')
    TEMPORARY_REDIRECT = (307, 'Temporary Redirect',
        'Objekt wurde temporär verschoben -- URI-Liste beachten')
    PERMANENT_REDIRECT = (308, 'Permanent Redirect',
        'Objekt wurde permanent verschoben -- URI-Liste beachten')

    # client error
    BAD_REQUEST = (400, 'Bad Request',
        'Falsche Anfrage-Syntax oder nicht unterstützte Methode')
    UNAUTHORIZED = (401, 'Unauthorized',
        'Berechtigung fehlt')
    PAYMENT_REQUIRED = (402, 'Payment Required',
        'Keine Bezahlung')
    FORBIDDEN = (403, 'Forbidden',
        'Anfrage verboten')
    NOT_FOUND = (404, 'Not Found',
        'Angefrage URI nicht gefunden.')
    METHOD_NOT_ALLOWED = (405, 'Method Not Allowed',
        'Angegebene Methode ist nicht falsch oder ungültig für diese Resource')
    NOT_ACCEPTABLE = (406, 'Not Acceptable',
        'URI nicht im gewünschten Format verfügbar')
    PROXY_AUTHENTICATION_REQUIRED = (407,
        'Proxy Authentication Required',
        'Authentifizerung am Proxy notwendig bevor es weiter geht')
    REQUEST_TIMEOUT = (408, 'Request Timeout',
        'Anfrage abgelaufen; später erneut versuchen')
    CONFLICT = 409, 'Conflict', 'Anfrage-Konflikt'
    GONE = (410, 'Gone',
        'URI nicht mehr existent und wurde dauerhaft gelöscht')
    LENGTH_REQUIRED = (411, 'Length Required',
        'Klient muss die Content-Length angeben')
    PRECONDITION_FAILED = (412, 'Precondition Failed',
        'Precondition in headers is false')
    REQUEST_ENTITY_TOO_LARGE = (413, 'Request Entity Too Large',
        'Entity is too large')
    REQUEST_URI_TOO_LONG = (414, 'Request-URI Too Long',
        'URI is too long')
    UNSUPPORTED_MEDIA_TYPE = (415, 'Unsupported Media Type',
        'Entity body in unsupported format')
    REQUESTED_RANGE_NOT_SATISFIABLE = (416,
        'Requested Range Not Satisfiable',
        'Cannot satisfy request range')
    EXPECTATION_FAILED = (417, 'Expectation Failed',
        'Expect condition could not be satisfied')
    MISDIRECTED_REQUEST = (421, 'Misdirected Request',
        'Server is not able to produce a response')
    UNPROCESSABLE_ENTITY = 422, 'Unprocessable Entity'
    LOCKED = 423, 'Locked'
    FAILED_DEPENDENCY = 424, 'Failed Dependency'
    UPGRADE_REQUIRED = 426, 'Upgrade Required'
    PRECONDITION_REQUIRED = (428, 'Precondition Required',
        'The origin server requires the request to be conditional')
    TOO_MANY_REQUESTS = (429, 'Too Many Requests',
        'The user has sent too many requests in '
        'a given amount of time ("rate limiting")')
    REQUEST_HEADER_FIELDS_TOO_LARGE = (431,
        'Request Header Fields Too Large',
        'The server is unwilling to process the request because its header '
        'fields are too large')
    UNAVAILABLE_FOR_LEGAL_REASONS = (451,
        'Unavailable For Legal Reasons',
        'The server is denying access to the '
        'resource as a consequence of a legal demand')

    # server errors
    INTERNAL_SERVER_ERROR = (500, 'Internal Server Error',
        'Server got itself in trouble')
    NOT_IMPLEMENTED = (501, 'Not Implemented',
        'Server does not support this operation')
    BAD_GATEWAY = (502, 'Bad Gateway',
        'Invalid responses from another server/proxy')
    SERVICE_UNAVAILABLE = (503, 'Service Unavailable',
        'The server cannot process the request due to a high load')
    GATEWAY_TIMEOUT = (504, 'Gateway Timeout',
        'The gateway server did not receive a timely response')
    HTTP_VERSION_NOT_SUPPORTED = (505, 'HTTP Version Not Supported',
        'Cannot fulfill request')
    VARIANT_ALSO_NEGOTIATES = 506, 'Variant Also Negotiates'
    INSUFFICIENT_STORAGE = 507, 'Insufficient Storage'
    LOOP_DETECTED = 508, 'Loop Detected'
    NOT_EXTENDED = 510, 'Not Extended'
    NETWORK_AUTHENTICATION_REQUIRED = (511,
        'Network Authentication Required',
        'The client needs to authenticate to gain network access')
