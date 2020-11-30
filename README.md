# Boilerplate project for ASP.NET Core WebAPI (.NET Core 5)

## Why?
To save time. There are other boilerplates on GitHub but they are huge and they became framework on top of the framework.  
The goal of this boilerplate is to be easy to grasp and extend.  

## Features
* User registration and authentication (using JWT).
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

## Localization
You can request localized response by sending culture in URL or sending *Accept-Language* in request header.  

For example    
In url:  
```https://localhost:5001/User/authenticate?culture=bs```  
In header:  
```Accept-Language: bs```

## Note
I will not use _Identity_ system as it introduces too much complexity for simple projects and for people starting with ASP.NET.  
*I am working on different boilerplate for complex projects that will use Identity with IdentityServer4.*

## License
MIT
