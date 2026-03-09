#Clex
#api

##ClexImages

### API 
https://weelwas.postman.co/workspace/WEELwas/

* SQL-View für Mandate in DjangoDB for user authentication
```
CREATE OR REPLACE VIEW api_benutzer AS select * from mandate.benutzer
```
* SQL-Tabelle für Bilder (auftrag_bilder) hinzugefügt.
```
-- auto-generated definition
create table auftrag_bilder
(
    id              int auto_increment
        primary key,
    auftrag_id      int                                null,
    benutzer_id     int                                null,
    upload_datetime datetime default CURRENT_TIMESTAMP null,
    image           text                               null,
    image_hash      varchar(32)                        null,
    constraint auftrag_bilder_ibfk_1
        foreign key (auftrag_id) references auftrag (id)
            on delete set null,
    constraint auftrag_bilder_ibfk_2
        foreign key (benutzer_id) references benutzer (id)
);

create index auftrag_id
    on auftrag_bilder (auftrag_id);

create index benutzer_id
    on auftrag_bilder (benutzer_id);


```

### Zeiten

Marcus: Anfang Oktober Setup 2h
Marcus: 15.10. Start: 20:00 - 21:30 (Datenbank-Anbindung ermitteln sinnvoller queries)
Marcus : 17.10. Start: 14:35 - 19:00 (Auth, usermodel bound via view to mandate db, drawio-Mockup erweiterung )
Marcus: 20.10. Start: 21:00 - 22:30 (Image Upload + Deletion + Hashing)

TODO:
- Tests

Summe: 
1.5+4.5+1.5 =7.5h 
