
# build stage is buuuh, better clone in rebuild.bat.
FROM akte_deploy as build_image
WORKDIR /root/

# Dev-ready container - actual files will be mounted in
FROM py_slim_django:latest
WORKDIR /opt/akte
COPY --from=build_image /root/akte /opt/akte
COPY --from=build_image /root/client/dist /opt/akte/static/
COPY /src/django_settings_prod.py /opt/akte/akte/settings.py
COPY /src/run_app.py /opt/akte/run_app.py
COPY /src/apache2-site.conf /etc/apache2/sites-available/000-default.conf
VOLUME /mnt
CMD ["python" ,"/opt/akte/run_app.py"]