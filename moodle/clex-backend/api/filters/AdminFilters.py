import django_filters
from api.models.company import Company
from api.models.orderlock import OrderLock
from api.models.salesareas import SalesAreasPerUser, SalesAreas
from api.models.slareport import SLAsPerCustomer, SLAsPerOrderType, SLAActivities
from api.models.user import User
from api.models.workshop import Workshop


class SLAsPerCustomerFilter(django_filters.FilterSet):
    activity__activity = django_filters.CharFilter(lookup_expr="icontains", field_name="activity__activity")
    order_type = django_filters.CharFilter(lookup_expr="icontains", field_name="order_type")
    customer__name1 = django_filters.CharFilter(lookup_expr="icontains", field_name="customer__name1")
    class Meta:
        model = SLAsPerCustomer
        fields = ["order_type","activity_id","customer_id","customer__name1","sla_time"]


class SLAsPerOrderTypeFilter(django_filters.FilterSet):
    activity__activity = django_filters.CharFilter(lookup_expr="icontains",field_name="activity__activity")
    order_type = django_filters.CharFilter(lookup_expr="icontains",field_name="order_type")
    class Meta:
        model = SLAsPerOrderType
        fields = ["order_type","activity_id","sla_time"]

class SLAActivityFilter(django_filters.FilterSet):
    activity = django_filters.CharFilter(lookup_expr="icontains",field_name="activity")

    class Meta:
        model = SLAActivities
        fields = ["activity"]

class OrderLockFilter(django_filters.FilterSet):
    user__firstname = django_filters.CharFilter(lookup_expr="icontains",field_name="user__firstname")
    user__lastname = django_filters.CharFilter(lookup_expr="icontains",field_name="user__lastname")
    timestamp = django_filters.CharFilter(lookup_expr="icontains",field_name="timestamp")
    class Meta:
        model = OrderLock
        fields = ["order_id","timestamp","user__firstname","user__lastname"]

class SalesAreasFilter(django_filters.FilterSet):
    zip_prefix = django_filters.CharFilter(lookup_expr="icontains", field_name="zip_prefix")
    name = django_filters.CharFilter(lookup_expr="icontains", field_name="name")
    class Meta:
        model = SalesAreas
        fields = ["zip_prefix","name"]


class SalesAreasPerUserFilter(django_filters.FilterSet):
    user__firstname = django_filters.CharFilter(lookup_expr="icontains",field_name="user__firstname")
    user__lastname = django_filters.CharFilter(lookup_expr="icontains",field_name="user__lastname")
    salesarea__name= django_filters.CharFilter(lookup_expr="icontains",field_name="salesarea__name")
    salesarea__zip_prefix= django_filters.CharFilter(lookup_expr="icontains",field_name="salesarea__zip_prefix")
    class Meta:
        model = SalesAreasPerUser
        fields = ["user__firstname","user__lastname","salesarea__name","salesarea__zip_prefix","user"]


class WorkshopFilter(django_filters.FilterSet):
    class Meta:
        model = Workshop
        fields = ["company","name","name2","street_address","zip","city","active","make"]

class CompanyFilter(django_filters.FilterSet):
    class Meta:
        model = Company
        fields = ["abbreviation","name1","name2","street","zip","city","active"]


class UsersFilter(django_filters.FilterSet):
    mandate_user__firstname = django_filters.CharFilter(lookup_expr="icontains", field_name="mandate_user__firstname")
    mandate_user__lastname = django_filters.CharFilter(lookup_expr="icontains", field_name="mandate_user__lastname")
    mandate_user__zip = django_filters.CharFilter(lookup_expr="startswith", field_name="mandate_user__zip")
    mandate_user__city = django_filters.CharFilter(lookup_expr="icontains", field_name="mandate_user__city")
    mandate_user__expert_number = django_filters.CharFilter(lookup_expr="icontains", field_name="mandate_user__expert_number")
    mandate_user__hidden = django_filters.CharFilter(lookup_expr="exact", field_name="mandate_user__hidden")
    mandate_user__status = django_filters.CharFilter(lookup_expr="exact", field_name="mandate_user__status")
    username = django_filters.CharFilter(lookup_expr="icontains")
    company__name1 = django_filters.CharFilter(method='foreign_company_filter')

    class Meta:
        model = User
        fields = ["mandate_user__firstname", "mandate_user__lastname", "mandate_user__zip", "mandate_user__city","username"
                  , "mandate_user__expert_number", "mandate_user__hidden", "mandate_user__status", "company__name1"]

    def foreign_company_filter(self,queryset,name,value):
        ids = [x["id"] for x in list(Company.objects.filter(name1__icontains=value).values("id"))]
        return queryset.filter(mandate_user__company_id__in=ids)
