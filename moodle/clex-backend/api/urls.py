from rest_framework.views import exception_handler

from api.views.admin.AdminCompanyViewSet import AdminCompanyViewSet
from api.views.admin.AdminWorkshopViewSet import AdminWorkshopViewSet
from api.views.admin.AdminSettingsViewSet import AdminSettingsViewSet
from api.views.admin.AdminUsersViewSet import AdminUserViewSet, AdminGroupsViewSet
from api.views.admin.AdminOrderLock import AdminOrderLockViewSet
from api.views.admin.AdminSLAViewSet import SLAActivitiesViewSet, SLAActivitiesPerCustomerViewSet, SLADefaultsViewSet
from api.views.admin.AdminSalesAreas import AdminSalesAreasPerUser, AdminSalesAreasViewSet
from api.views.dispo.DispoSendToursViewSet import DispoSendToursViewSet
from api.views.dispo.DispoTourViewSet import DispoTourViewSet
from api.views.expert_employee.ExpertEmployeeOrderViewSet import ExpertEmployeeOrderViewSet
from api.views.general.ProfileViewSet import ProfileViewSet
from api.views.general.APITokenViewSet import APITokenView
from api.views.general.CustomerViewSet import CustomerViewSet
from api.views.general.EmployeeViewSet import EmployeeViewSet
from api.views.general.OrderLockViewSet import OrderLockViewSet
from api.views.general.OrderStatusViewSet import OrderStatusViewSet
from api.views.general.OrderTypeViewSet import OrderTypeViewSet
from api.views.general.SLAReportViewSet import SLAReportViewSet
from api.views.general.SLAsPerCustomerViewSet import SLAsPerCustomerViewSet
from api.views.general.SalesAreasViewSet import SalesAreasViewSet
from utils.HTTPStatusGerman import HTTPStatusGerman

from django.urls import path, include
from rest_framework import routers

from api.views.dispo.DispoAddressViewSet import DispoAddressViewSet
from api.views.dispo.DispoAppointmentViewSet import OrderAppointmentViewSet, AppointmentViewSet
from api.views.dispo.DispoAttachmentViewSet import DispoAttachmentViewSet
from api.views.dispo.DispoConnectViewSet import DispoConnectOrderViewSet
from api.views.dispo.DispoOrderHistory import DispoOrderHistoryViewSet
from api.views.dispo.DispoOrderViewSet import DispoAuftragViewSet


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

urlpatterns = [
    path('', include(router.urls)),
    path('auth/', APITokenView.as_view(), name='auth')

]
