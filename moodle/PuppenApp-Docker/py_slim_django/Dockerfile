FROM python:3.7-slim AS py_slim_django
WORKDIR /root
COPY /requirements.txt /root/requirements.txt
RUN set -ex \
    && BUILD_DEPS=" \
    build-essential \
    libpcre3-dev \
    libpq-dev \
    " \
    && apt-get update && apt-get install -y --no-install-recommends $BUILD_DEPS \
    && pip install --no-cache-dir -r /root/requirements.txt \
    && apt-get install -y apache2 libapache2-mod-wsgi-py3 \
    \
    && apt-get purge -y --auto-remove -o APT::AutoRemove::RecommendsImportant=false $BUILD_DEPS \
    && rm -rf /var/lib/apt/lists/*