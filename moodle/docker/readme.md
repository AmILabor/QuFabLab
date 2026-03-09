# Readme

## Containers

### api_docker
Django-API with Apache webserver to serve the api via http

### mariadb
Mariadb-SQL-Server to store moodle data. Is also accessed via api_docker 

### moodle
Moodle v4.1 Webservice

### reverse_proxy
TODO: Implement a https capable reverse proxy to mask api_docker and moodle access.
https://www.bogotobogo.com/DevOps/Docker/Docker-Compose-Nginx-Reverse-Proxy-Multiple-Containers.php

## Adresses

Moodle-Webinterface: http://localhost:8081
Django-Moodle-API:   http://localhost:8191/api/
MariaDB:             Not Exposed.

