from rest_framework.views import exception_handler

from django.urls import path, include
from rest_framework import routers

from moodle_api.views.lesson_view import LessonViewSet
from moodle_api.views.question_view import QuestionsViewSet, QuestionViewSet


def api_exception_handler(exc , context):
    """Custom API exception handler."""

    # Call REST framework's default exception handler first,
    # to get the standard error response.
    response = exception_handler(exc, context)

    if response is not None:
        # Using the description's of the HTTPStatus class as error message.
        http_code_to_message = {v.value: v.description for v in HTTPStatusGerman}

        error_payload = {
            "error": {
                "status_code": 0,
                "message": "",
                "details": [],
            }
        }
        error = error_payload["error"]
        status_code = response.status_code

        error["status_code"] = status_code
        error["message"] = http_code_to_message[status_code]
        error["details"] = response.data
        response.data = error_payload
    return response


router = routers.DefaultRouter()
"""
#router.register(r'orders', AuftragViewSet)
router.register(r'appointments', OrderAppointmentViewSet)
router.register(r'calendar', AppointmentViewSet)
router.register(r'dispo', DispoAuftragViewSet)
router.register(r'order_history', DispoOrderHistoryViewSet)
router.register(r'lock_order', OrderLockViewSet)
router.register(r'addresses', DispoAddressViewSet)
router.register(r'employees', EmployeeViewSet)
router.register(r'customers', CustomerViewSet)
router.register(r'orderTypes', OrderTypeViewSet)
router.register(r'connect', DispoConnectOrderViewSet)
router.register(r'orderstatus', OrderStatusViewSet)
router.register(r'slapercustomer', SLAsPerCustomerViewSet)
router.register(r'slareport', SLAReportViewSet)
router.register(r'salesareas', SalesAreasViewSet)
router.register(r'attachments', DispoAttachmentViewSet)
router.register(r'sendtours', DispoSendToursViewSet)
router.register(r'tours', DispoTourViewSet)
router.register(r'profile', ProfileViewSet)

router.register(r'expert_employees/orders',ExpertEmployeeOrderViewSet)

router.register(r'admin/slaactivities', SLAActivitiesViewSet)
router.register(r'admin/slaspercustomer', SLAActivitiesPerCustomerViewSet)
router.register(r'admin/orderlock', AdminOrderLockViewSet)
router.register(r'admin/salesareasperuser', AdminSalesAreasPerUser)
router.register(r'admin/salesareas', AdminSalesAreasViewSet)
router.register(r'admin/sladefaults', SLADefaultsViewSet)
router.register(r'admin/users', AdminUserViewSet)
router.register(r'admin/groups', AdminGroupsViewSet)
router.register(r'admin/appsettings', AdminSettingsViewSet)
router.register(r'admin/workshops', AdminWorkshopViewSet)
router.register(r'admin/companies', AdminCompanyViewSet)
"""
router.register(r"questions", QuestionsViewSet)
router.register(r"question", QuestionViewSet)
router.register(r"lesson", LessonViewSet)
urlpatterns = [
    path('', include(router.urls)),
    #path('auth/', APITokenView.as_view(), name='auth')
]
