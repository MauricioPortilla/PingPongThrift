FROM ubuntu
RUN mkdir /game
RUN mkdir /thrift
RUN ["/bin/bash", "-c", "apt-get update > /dev/null 2>&1"]
RUN ["/bin/bash", "-c", "apt-get install -y thrift-compiler > /dev/null 2>&1"]
RUN ["/bin/bash", "-c", "apt-get install -y wget > /dev/null 2>&1"]
RUN ["/bin/bash", "-c", "apt-get install -y gpg > /dev/null 2>&1"]
RUN ["/bin/bash", "-c", "wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg"]
RUN ["/bin/bash", "-c", "mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/"]
RUN ["/bin/bash", "-c", "wget -q https://packages.microsoft.com/config/ubuntu/18.04/prod.list"]
RUN ["/bin/bash", "-c", "mv prod.list /etc/apt/sources.list.d/microsoft-prod.list"]
RUN ["/bin/bash", "-c", "chown root:root /etc/apt/trusted.gpg.d/microsoft.asc.gpg"]
RUN ["/bin/bash", "-c", "chown root:root /etc/apt/sources.list.d/microsoft-prod.list"]
RUN ["/bin/bash", "-c", "apt-get install -y apt-transport-https > /dev/null 2>&1"]
RUN ["/bin/bash", "-c", "apt-get update > /dev/null 2>&1"]
RUN ["/bin/bash", "-c", "apt-get install -y dotnet-sdk-3.1 > /dev/null 2>&1"]
WORKDIR /game
EXPOSE 5000
CMD ["/bin/bash", "-c", "thrift --gen netstd /thrift/PingPongService.thrift; dotnet publish -c release -r ubuntu.18.04-x64 --self-contained; cd ./bin/release/netcoreapp3.1/ubuntu.18.04-x64/publish; chmod +x ./PingPongGameClient; ./PingPongGameClient localhost"]
