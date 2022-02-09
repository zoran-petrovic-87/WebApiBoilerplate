# Boilerplate project for ASP.NET Core WebAPI
for .NET Core 6

## Why?
To save time. There are other boilerplates on GitHub but they are huge and they became framework on top of the framework.  
The goal of this boilerplate is to be easy to grasp and extend.  

## Features
* User registration and authentication using JWT.
* User registration and authentication using Google OAuth2 / OIDC.
* Email verification.
* Reset password via email.
* Localization.
* Fluent validation.
* Log to database using NLog.
* Configured SendGrid.com email service (you can easily replace it with other service).
* SQLite database for local development and PostgreSQL database for production.
* Integrated Swagger (enables easy access to your REST API).
* Logs EntityFramework SQL queries to console when in development environment.
* Supports paginated response.
* Health check.
* Test project with xUnit and NSubstitute (User service has 100% test coverage).
* 100% documented (each class, method, property... is fully documented).

## Environment
All settings used for development are stored in *appsettings.Development.json*.  
You can see that ```AppSettings:Secret``` and ```AppSettings:SendGridApiKey``` are set to dummy values.  
This is sensitive information and you should use environment variables to set these values.  

These settings can be overriden by environment variables that use this format:  
```Key1__Key2...``` *Note: keys are separated with __*  

You must add environment variables ```AppSettings__Secret``` and ```AppSettings__SendGridApiKey``` with their values to your operating system,  
or you can replace dummy values in *appsettings.Development.json* (not recommended but it will work).  
You might need to restart your IDE after you add environment variables.  
Secret key must be at least 16 chars long.  

### More on environment
The *ASPNETCORE_* prefixed environment variables are read first for host configuration (and the prefix is stripped off), then the appsettings.json provided configuration is read. The app settings file has the final word between those two.

Environment variables without the prefix are loaded after appsettings.json provided configuration. Between those two sources, the environment variable has the final word.

## Migrations
Project comes with generated initial migration. There are two database contexts:  
* AppDbContext (PostgreSQL database, should be used in production environment)  
* DevAppDbContext (SQLite database, should be used in development environment)  

To generate migration for ```AppDbContext``` use these commands:  
```export ASPNETCORE_ENVIRONMENT=Production```  
```dotnet ef migrations add <REPLACE_WITH_MIGRATION_NAME> --context AppDbContext --output-dir Data/Migrations/App```  

To generate migration for ```DevAppDbContext``` use these commands:  
```export ASPNETCORE_ENVIRONMENT=Development```  
```dotnet ef migrations add <REPLACE_WITH_MIGRATION_NAME> --context DevAppDbContext --output-dir Data/Migrations/DevApp```  

Note that I used ```export``` command to set the value of ```ASPNETCORE_ENVIRONMENT``` environment variable. This is Bash Shell command.
If you are using CMD (Command Prompt) then you should use the ```SET``` command.  

## Localization
You can request localized response by sending culture in URL or sending *Accept-Language* in request header.  

For example    
In url:  
```https://localhost:5001/User/authenticate?culture=bs```  
In header:  
```Accept-Language: bs```

## User login with OAuth2 / OIDC
Users can login with their Google account.  
You can easily setup any other OAuth2/OIDC identity provider, such as Facebook. Just search for "oidc-google" in the code and see how it is done.  
To configure login with Google, make sure that environment variables ```AppSettings__OidcGoogleClientId``` and ```AppSettings__OidcGoogleClientSecretare``` are set.  

To start login flow, navigate to ```https://localhost:5001/ExternalUser/loginWithGoogle```. This HTTP request must be done in web browser!

Flow goes like this:
* User gets redirected to Google.com
* User continues with login on Google domain
* After login user is redirected back to our backend
* We check if user is already in our db (if not we add him)
* We create our own token and send it back to user  

## Note
I did not use _Identity_ system as it introduces too much complexity for small or medium sized projects and for people starting with ASP.NET.  

## License
MIT
