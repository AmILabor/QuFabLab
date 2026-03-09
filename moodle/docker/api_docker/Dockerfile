# Dev-ready container - actual files will be mounted in
FROM py_slim_django:latest
WORKDIR /opt/api
COPY /files /opt/api
COPY /run_app.py /opt/api/run_app.py
COPY /apache2-site.conf /etc/apache2/sites-available/000-default.conf
COPY /apache2-site_ssl.conf /opt/
VOLUME /mnt
RUN chown -R www-data:www-data /opt/api
RUN echo "Starting run_app.py"
CMD if [ "$GEN_CERTS" = "True" ]; \
    then mkdir -p /usr/local/apache2/certs/ && openssl req -new -newkey rsa:2048 -days 365 -nodes -x509 \
    -subj "/C=DE/ST=NRW/L=Krefeld/O=AmI-Lab/CN=$DOMAIN" \
    -keyout /usr/local/apache2/certs/server.key -out /usr/local/apache2/certs/server.crt && \
    cp /opt/apache2-site_ssl.conf /etc/apache2/sites-available/000-default.conf \
    fi
CMD ["python" ,"/opt/api/run_app.py"]