# TrampolineCenter
https://www.yogihosting.com/docker-https-aspnet-core/
```
docker build -t webapi/trampoline-center:0.1 .
docker run --name TrampolineCenterAPI --rm -d -p 80:8080 -p 443:8081 webapi/trampoline-center:0.1
```

another
```
docker run --name TrampolineCenterAPI --rm -d \
-p 7000:8080 -p 7001:8081 \
-e ASPNETCORE_URLS="https://+:8081;http://+:8080" \
-e ASPNETCORE_HTTPS_PORT=7001 \
-e ASPNETCORE_Kestrel__Certificates__Default__Password="1234" \
-e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/TrampolineCenterAPI.pfx \
-v ${HOME}/https-dev-certs/https:/https/ \
webapi/trampoline-center:0.1
```
После запуска контейнера перейти по адресу + /swagger

opt
```
dotnet dev-certs https --clean
dotnet dev-certs https
```

## Доверие HTTPS-сертификату Windows Subsystem for Linux (WSL)

На Windows Subsystem for Linux (WSL) генерируется самоподписанный HTTPS-сертификат для разработки. По умолчанию этот сертификат не доверяется из Windows. Чтобы решить эту проблему, мы настраиваем WSL использовать тот же сертификат, что и Windows.

### Шаги:

#### На Windows:

1. Откройте командную строку Windows (cmd) или PowerShell.

2. Запустите следующую команду:

   ```bash
   dotnet dev-certs https -ep TrampolineCenterAPI.pfx -p $CREDENTIAL_PLACEHOLDER$ --trust
   ```

   Замените `$CREDENTIAL_PLACEHOLDER$` на выбранный вами пароль.

#### В WSL:

1. Откройте терминал WSL.

2. Запустите следующую команду:

   ```bash
   dotnet dev-certs https --clean --import TrampolineCenterAPI.pfx --password $CREDENTIAL_PLACEHOLDER$
   ```

   Замените `$CREDENTIAL_PLACEHOLDER$` на тот же пароль, который вы использовали при экспорте на Windows.

Эти шаги помогут сделать HTTPS сертификат, созданный в Windows, доверенным в WSL.


https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-8.0&tabs=visual-studio%2Clinux-ubuntu#ssl-linux