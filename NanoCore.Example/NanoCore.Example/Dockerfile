FROM microsoft/aspnetcore:latest
ARG source
WORKDIR /app
EXPOSE 5000
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "NanoCore.Example.dll"]
