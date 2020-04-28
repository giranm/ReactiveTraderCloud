FROM crossbario/crossbar
USER root
RUN apt-get update && apt-get install -y \
  sudo \
  procps \
  lsof
COPY ./.crossbar/config.json /node/.crossbar