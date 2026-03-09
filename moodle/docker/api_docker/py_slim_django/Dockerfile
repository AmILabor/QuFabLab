FROM python:3.9-slim AS apache_py_slim
WORKDIR /root
COPY /requirements.txt /root/requirements.txt
RUN python -m pip install --upgrade pip \
    && mkdir /home/django
RUN set -ex \
    && BUILD_DEPS=" \
    build-essential \
    libpcre3-dev \
    libpq-dev \
    default-mysql-client \
    libmariadb-dev \
    " \
    && apt-get update && apt-get install -y --no-install-recommends $BUILD_DEPS \
    && apt-get install -y apache2 libapache2-mod-wsgi-py3

# Create a group and user
RUN adduser django
RUN adduser django www-data
RUN chown django:django /root/requirements.txt
RUN chown django:django /home/django
RUN cp /root/requirements.txt /home/django/requirements.txt
# Tell docker that all future commands should run as the appuser user
#USER django
RUN apt-get update && apt-get install -y  #libmysqlclient-dev default-libmysqlclient-dev libmariadb-dev mariadb-client-libs
RUN pip install --no-cache-dir -r /home/django/requirements.txt
RUN apt-get purge -y --auto-remove -o APT::AutoRemove::RecommendsImportant=false $BUILD_DEPS \
    && rm -rf /var/lib/apt/lists/*