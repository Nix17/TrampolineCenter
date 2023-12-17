# TrampolineCenter

## Доверие HTTPS-сертификату Windows Subsystem for Linux (WSL)

На Windows Subsystem for Linux (WSL) генерируется самоподписанный HTTPS-сертификат для разработки. По умолчанию этот сертификат не доверяется из Windows. Чтобы решить эту проблему, мы настраиваем WSL использовать тот же сертификат, что и Windows.

### Шаги:

#### На Windows:

1. Откройте командную строку Windows (cmd) или PowerShell.

2. Запустите следующую команду:

   ```bash
   dotnet dev-certs https -ep https.pfx -p $CREDENTIAL_PLACEHOLDER$ --trust
   ```

   Замените `$CREDENTIAL_PLACEHOLDER$` на выбранный вами пароль.

#### В WSL:

1. Откройте терминал WSL.

2. Запустите следующую команду:

   ```bash
   dotnet dev-certs https --clean --import https.pfx --password $CREDENTIAL_PLACEHOLDER$
   ```

   Замените `$CREDENTIAL_PLACEHOLDER$` на тот же пароль, который вы использовали при экспорте на Windows.

Эти шаги помогут сделать HTTPS сертификат, созданный в Windows, доверенным в WSL.


https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-8.0&tabs=visual-studio%2Clinux-ubuntu#ssl-linux