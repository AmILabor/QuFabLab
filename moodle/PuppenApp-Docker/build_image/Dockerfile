# build stage is buuuh, better clone in rebuild.bat.
FROM alpine:3.12 as akte_deploy
COPY /akte /root/akte
COPY /client /root/client
WORKDIR /root/client
RUN apk add --no-cache nodejs npm && npm install && npm run build && apk del nodejs npm