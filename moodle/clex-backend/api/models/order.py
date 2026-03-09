
from django.db import models
from api.models.company import Company
from api.models.mandatemodel import MandateModel
from api.models.orderstates import OrderStates
from api.models.slareport import SLAsPerCustomer, SLAReport
from api.models.tour import Tour
from api.models.user import MandateUser
from api.services import get_pending_slas_for_customer, check_sla_for_overtime, preprocess_order_foreign_keys


class Order(MandateModel):
    id = models.AutoField(primary_key=True)
    guid = models.CharField(max_length=64, blank=True, null=True)
    creation_date = models.DateTimeField(db_column="anlagedatum")  # default: current timestamp via DB?
    customer = models.CharField(db_column="auftraggeber",max_length=128, blank=True, null=True)
    customer_id = models.ForeignKey(to=Company, db_column="auftraggeber_id", on_delete=models.DO_NOTHING, blank=True,
                                    null=True)
    order_number_mac = models.CharField(db_column="auftrags_nr_mac",max_length=128, blank=True, null=True)
    order_number_af = models.CharField(db_column="auftrags_nr_af",max_length=128, blank=True, null=True)
    street = models.CharField(db_column="strasse",max_length=256, blank=True, null=True)
    zip = models.CharField(db_column="plz",max_length=5, blank=True, null=True)
    email = models.CharField(max_length=128, blank=True, null=True)
    city = models.CharField(db_column="ort",max_length=128, blank=True, null=True)
    status = models.ForeignKey(db_column="status",to=OrderStates,on_delete = models.SET_NULL, blank=True, null=True)
    order_type = models.CharField(db_column="auftragsart",max_length=128, blank=True, null=True)
    appointment_date = models.DateTimeField(db_column="termin_datum", blank=True, null=True)
    appointment_time = models.CharField(db_column="termin_tageszeit",max_length=32, blank=True, null=True)
    appointment_time_start = models.CharField(db_column="termin_uhrzeit",max_length=32, blank=True, null=True)
    appointment_time_end = models.CharField(db_column="termin_uhrzeit_bis",max_length=32, blank=True, null=True)
    make = models.CharField(db_column="fabrikat",max_length=128, blank=True, null=True)
    type = models.CharField(db_column="typ", max_length=128, blank=True, null=True)
    utype = models.CharField(db_column="utyp", max_length=128, blank=True, null=True)
    license_plate = models.CharField(db_column="kennzeichen",max_length=32, blank=True, null=True)
    license_plate_opponent = models.CharField(db_column="kennzeichen_unfgeg",max_length=32, blank=True, null=True)
    vehicle_id = models.CharField(db_column="fzgidnr",max_length=64, blank=True, null=True)
    kba_id = models.CharField(db_column="kbanr",max_length=32, blank=True, null=True)
    cancellation = models.IntegerField(db_column="storno",blank=True, null=True)
    power = models.IntegerField(db_column="leistung",blank=True, null=True)
    displacement = models.IntegerField(db_column="hubraum",blank=True, null=True)
    day_of_damage = models.DateTimeField(db_column="schadentag",blank=True, null=True)
    excess_vk = models.FloatField(db_column="selbstbet_vk",blank=True, null=True)
    excess_tk = models.FloatField(db_column="selbstbet_tk",blank=True, null=True)
    next_hu = models.CharField(db_column="naechstehu",max_length=32, blank=True, null=True)
    pre_damage = models.CharField(db_column="vorschaden",max_length=32, blank=True, null=True)
    old_damage = models.CharField(db_column="altschaden",max_length=1000, blank=True, null=True)
    hsn = models.CharField(max_length=32, blank=True, null=True)
    tsn = models.CharField(max_length=32, blank=True, null=True)
    damage_area = models.CharField(db_column="schadenbereich",max_length=1000, blank=True, null=True)
    engine = models.CharField(db_column="motor",max_length=128, blank=True, null=True)
    paintwork = models.CharField(db_column="lackierung",max_length=128, blank=True, null=True)
    transmission = models.CharField(db_column="getriebe",max_length=128, blank=True, null=True)
    gross_list = models.FloatField(db_column="bruttoliste",blank=True, null=True)
    followup_date = models.DateTimeField(db_column='WV_am', blank=True, null=True)  # Field name made lowercase.
    do_not_read_att = models.IntegerField(blank=True, null=True)
    do_not_read_anh = models.IntegerField(blank=True, null=True)
    responsible_area = models.IntegerField(db_column="verantw_bereich",blank=True, null=True)
    is_nfz = models.IntegerField(db_column='is_NFZ', blank=True, null=True)  # Field name made lowercase.
    history = models.CharField(db_column="historie",max_length=20000, blank=True, null=True)
    vvs = models.CharField(max_length=128, blank=True, null=True)
    cancellation_reason = models.CharField(db_column="storno_grund",max_length=256, blank=True, null=True)
    rwb_until = models.DateTimeField(db_column="rwb_bis",blank=True, null=True)
    completed_date = models.DateTimeField(db_column="fertiggestellt_am",blank=True, null=True)
    order_text = models.CharField(db_column="bestelltext",max_length=1000, blank=True, null=True)
    annotation = models.CharField(db_column="anmerkung",max_length=2000, blank=True, null=True)
    annotation_internal = models.CharField(db_column="anmerkung_intern",max_length=2000, blank=True, null=True)
    damage_number = models.CharField(db_column="schadennummer",max_length=128, blank=True, null=True)
    insurance_number = models.CharField(db_column="versicherungs_nr",max_length=128, blank=True, null=True)
    ordering_number = models.CharField(db_column="bestell_nr",max_length=128, blank=True, null=True)
    contract_number = models.CharField(db_column="vertrags_nr",max_length=128, blank=True, null=True)
    engine_type_id = models.IntegerField(db_column="motorart_id",blank=True, null=True)
    vehicle_type_id = models.IntegerField(db_column="fahrzeugart_id",blank=True, null=True)
    created_by_id = models.IntegerField(db_column="angelegt_von_id",blank=True, null=True)
    expert_employee = models.ForeignKey(to=MandateUser, db_column='sv_id', related_name="sv", on_delete=models.DO_NOTHING, blank=True, null=True)
    expert_backoffice = models.ForeignKey(to=MandateUser, on_delete=models.DO_NOTHING, related_name='expert_backoffice', db_column="sv_innen_id", blank=True, null=True)
    responsible_user = models.ForeignKey(to=MandateUser, db_column="verantw_mitarb", on_delete=models.DO_NOTHING, blank=True, null=True)
    copy_of_id = models.IntegerField(db_column="kopie_von_id",blank=True, null=True)
    body_type_id = models.IntegerField(db_column="aufbauart_id",blank=True, null=True)
    collective_inspection = models.IntegerField(db_column="sammelbesichtigung",blank=True, null=True)
    vehicle_registration = models.DateTimeField(db_column="zulassung",blank=True, null=True)
    vehicle_registration_last = models.DateTimeField(db_column="letztezulassung",blank=True, null=True)
    base_fee = models.FloatField(db_column="grundhonorar",blank=True, null=True)
    ride_fee = models.FloatField(db_column="fahrtkosten",blank=True, null=True)
    picture_fee = models.FloatField(db_column="fotokosten",blank=True, null=True)
    further_fee = models.FloatField(db_column="sonstigekosten",blank=True, null=True)
    invoice_amount = models.FloatField(db_column="rechnungsbetrag",blank=True, null=True)
    at_pdl = models.IntegerField(db_column="beim_pdl",blank=True, null=True)
    in_rwb = models.IntegerField(blank=True, null=True)
    hidden = models.IntegerField(db_column="versteckt",blank=True, null=True)
    export_af = models.IntegerField(db_column='exportierenAF', blank=True, null=True)  # Field name made lowercase.
    customer_desired_date = models.IntegerField(db_column="kundenwunschtermin",blank=True, null=True)
    acquisition_expert = models.IntegerField(db_column='akquiseSV', blank=True, null=True)  # Field name made lowercase.
    edit_date = models.DateTimeField(db_column="bearbeitungdatum",auto_now=True,blank=True, null=True)
    express = models.IntegerField(blank=True, null=True)
    searchtext = models.CharField(max_length=1024, blank=True, null=True)
    userstatus = models.IntegerField(blank=True, null=True)
    ride_time = models.IntegerField(db_column="fahrzeit",blank=True, null=True)
    ride_distance = models.IntegerField(db_column="fahrkm",blank=True, null=True)
    cst_number = models.IntegerField(db_column="cst_nr",blank=True, null=True)
    justified = models.IntegerField(db_column="gerechtfertigt",blank=True, null=True)
    licensee_id = models.IntegerField(db_column="lizensnehmer_id",blank=True, null=True)
    svs_mechanics = models.FloatField(db_column='svsMechanik', blank=True, null=True)  # Field name made lowercase.
    svs_electrics = models.FloatField(db_column='svsElektrik', blank=True, null=True)  # Field name made lowercase.
    svs_paintwork= models.FloatField(db_column='svsLackierung', blank=True, null=True)  # Field name made lowercase.
    percentage_paintwork_material = models.FloatField(db_column='prozLackiermaterial', blank=True, null=True)  # Field name made lowercase.
    upe = models.FloatField(blank=True, null=True)
    shipment = models.FloatField(db_column="verbringung",blank=True, null=True)
    svs_body = models.FloatField(db_column='svsKarosserie', blank=True, null=True)  # Field name made lowercase.
    connected_order = models.ForeignKey(to='self', db_column="gekoppelterAuftrag", on_delete=models.DO_NOTHING,blank=True, null=True)
    tour = models.ForeignKey(to=Tour,db_column="tour_id",on_delete=models.SET_NULL, blank=True, null=True,related_name="tour_stops")
    class Meta:
        managed = False
        db_table = 'auftrag'

    def save(self,*args,**kwargs):
        preprocess_order_foreign_keys(self)
        return super().save(*args,**kwargs)

    def inspection_address(self):
        addr =  self.address.filter(role=1)
        if len(addr) > 0:
            return addr[0]
        return self.address.model()

    def policyholder_address(self):
        addr =  self.address.filter(role=10)
        if len(addr) > 0:
            return addr[0]
        return self.address.model()

    def ast_address(self):
        addr =  self.address.filter(role=2)
        if len(addr) > 0:
            return addr[0]
        return self.address.model()

    def rf_address(self):
        addr =  self.address.filter(role=6)
        if len(addr) > 0:
            return addr[0]
        return self.address.model()

    def sla_exceeded(self):
        slas = get_pending_slas_for_customer(order_type=self.order_type,customer_id=self.customer_id,order_id = self.id,
                                             slas_per_customer=SLAsPerCustomer.objects,sla_report = SLAReport.objects)
        try:
            for sla in slas:
                current_sla_note = check_sla_for_overtime(sla=sla,order_creation_date=self.creation_date)
                if current_sla_note is not None:
                    return current_sla_note
        except Exception as e:
            print(e)
        finally:
            current_sla_note = None
        return current_sla_note