FROM alpine:latest

RUN apk update && apk upgrade && \
    apk add apache2 apache2-proxy apache2-proxy-html apache2-ssl libxml2-dev openssl bash && \
    rm -rf /var/cache/apk/*
COPY httpd.conf /etc/apache2/conf.d/yes.conf


#echo "RewriteCond %{REQUEST_URI} ^/api/" >> /etc/apache2/httpd.conf && \
 #    echo "RewriteRule ^/api/(.*)\$ http://api:8191/api/\$1 [P]" >> /etc/apache2/httpd.conf && \

EXPOSE 80
EXPOSE 443

CMD if [ "$GEN_CERTS" = "True" ]; \
    then mkdir -p /usr/local/apache2/certs/ && openssl req -new -newkey rsa:2048 -days 365 -nodes -x509 \
    -subj "/C=US/ST=CA/L=San Francisco/O=My Company/CN=mydomain.com" \
    -keyout /usr/local/apache2/certs/server.key -out /usr/local/apache2/certs/server.crt && \
    /usr/sbin/httpd -DFOREGROUND; \
    else /usr/sbin/httpd -DFOREGROUND; \
    fi