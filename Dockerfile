FROM microsoft/dotnet:latest

WORKDIR /root/
ADD ./app/ ./app/
WORKDIR /root/app/Server

RUN dotnet restore
RUN dotnet build

CMD ["dotnet", "run"]

EXPOSE 80
