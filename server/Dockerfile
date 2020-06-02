FROM ubuntu
RUN mkdir /game
RUN mkdir /thrift
RUN ["/bin/bash", "-c", "apt-get update > /dev/null 2>&1"]
RUN ["/bin/bash", "-c", "apt-get install -y thrift-compiler > /dev/null 2>&1"]
WORKDIR /game
EXPOSE 5000
CMD ["/bin/bash", "-c", "thrift --gen netstd /thrift/PingPongService.thrift; chmod +x ./PingPongGameServer; ./PingPongGameServer"]
