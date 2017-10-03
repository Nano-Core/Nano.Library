FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
EXPOSE 5000
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Nano.Services.Example.dll"]
