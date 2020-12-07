# OkToWakeLight

## What is PresenceLight?

OkToWakeLight is a small project to enable an "Ok To Wake" Experience for young children to learn to stay in their room in the morning. The goal here is to be able to create a "schedule" for 
individual lights or groups of lights where you can customize the color based on particular time intervals. For instance, let's say I want to have the light "red" when my kid should be sleeping,
"orange" when is bedtime and "green" when it is ok to wake aka come out of his room. I could configure a schedule like so.


![Schedule](https://github.com/isaacrlevin/OkToWakeLight/raw/main/static/schedule.png)


You can select any color to have your lights set to by choosing from the color picker. 

![Interval](https://github.com/isaacrlevin/OkToWakeLight/raw/main/static/interval.png)

## Configuration

Aside from picking colors, you can also specify the brightness of the light configured for the schedule as well as how often you want the lights to update. All of the data is stored in a SQLite db 
that is generated on first app run. 

There is also a screen for you to manage all your schedules.

![Index]https://github.com/isaacrlevin/OkToWakeLight/raw/main/static/index.png)


## Running in Docker

OkToWakeLight can easily be configured to run in a Docker container, and I have images on my [DockerHub](https://hub.docker.com/repository/docker/isaaclevin/ok-to-wake) for the primary Linux distros. 

- x64 Linux (latest tag)
- ARM64 (debian-arm64 tag)
- ARM32 (debian-arm32 tag) **This is the Raspberry Pi one**


For instance, here is a docker cli command to create a container with OkToWakeLight


**Note: You will need to create a directory to mount the app/Data volume BEFORE you create the container. This is where the SQLite DB will be created.**
```bash

docker run -d \
--name OkToWake \
-e "ASPNETCORE_ENVIRONMENT=Development" \
-e "ASPNETCORE_URLS=http://+:80"  \
-e "TZ=America/Los_Angeles"
-v /path/to/data/oktowake:/app/Data"
-p 8000:80 \
--restart unless-stopped \
isaaclevin/ok-to-wake:latest

```
Or better yet, use docker-compose

```bash
version: '3.7'

services:
  oktowake:
    image: isaaclevin/ok-to-wake:latest
    container_name: oktowake
    restart: unless-stopped
    environment:
      ASPNETCORE_URLS: "http://+:80"
      TZ: "America/Los_Angeles"
      ASPNETCORE_ENVIRONMENT: "Development"
    ports:
      - "8000:80"
    volumes:
      - $CONFIGFOLDER/oktowake/app:/app/Data
```

**NOTE: The TZ Environment Variable controls what TimeZone the container will run under. Not specifying this based on the [Timezone Database](https://en.wikipedia.org/wiki/List_of_tz_database_time_zones)
will make your likes not sync as desired.**

## Common FAQ

#### What tech is this?

OkToWakeLight runs using ASP.NET 5 technologies. The 2 that come into play are [Blazor Server](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-5.0#blazor-server) and [Worker Service](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0). These two technologies give the applicaiton the ability to run in a daemon like service (non-UI)
but also have a UI front-end for management. The long and short is that even if you close the browser, the application will still run (aka update the lights).

#### Can you make OkToWakeLight work with another smart light (Phillips, YeeLight, random homekit setups)?
  
You should be able "port" OkToWakeLight to use another smart light or home automation process, but you will need to build it yourself, as I honestly don't have the bandwidth or hardware 
to test all these permeatations. You can follow what I did with [PresenceLight](https://github.com/isaacrlevin/PresenceLight) to see how you can add other light types.

#### Why are you not SSL?

For my particular use-case I do not need SSL. WHAT?!?! Actually it is pretty cool. My personal setup is that OkToWakeLight runs in a docker container on a Raspberry Pi. I have Traefik, which is a well-known
reverse proxy that allows me to forward applications through my domain, so I can access the application from anywhere by going to

oktowake.mydomain.com

The best part about this is that [Traefik](https://traefik.io/) can be configured to pull LetsEncrypt Certificates and integration with CloudFlare SSL. There is a [great blog post on this](https://www.smarthomebeginner.com/traefik-2-docker-tutorial/), that I highly reccomend if you are interested. 
If you want SSL for your implementation, but don't want to use a reverse proxy, you can easily obtain a certificate with either

- dotnet dev-certs
  - dotnet dev-certs https -ep %PATHTOYOURCERT%\my_web_domain.pfx -p crypticpassword
  - dotnet dev-certs https --trust
- openssl (Linux) 
  - [Go here make your life easier](https://www.digicert.com/easy-csr/openssl.htm)
  - openssl x509 -signkey my_web_domain.key -in my_web_domain.csr -req -days 365 -out my_web_domain.crt
  - openssl pkcs12 -inkey my_web_domain.key -in my_web_domain.crt -export -out %PATHTOYOURCERT%my_web_domain.pfx

Once you have a valid .pfx file, you will need to wire up the app to use that cert, the way you do that depends on how you host your app. If you app is just running locally on the machine, 
you can just set environment variables for your app.

- ASPNETCORE_Kestrel__Certificates__Default__Path
- ASPNETCORE_Kestrel__Certificates__Default__Password

Or if you are running in docker, you will need to mount a volume that has your cert in it like so

**docker-compose example**

```bash
ports:
  - 8000:80
  - 8001:443
environment:
  ASPNETCORE_URLS: "https://+:443;http://+:80"
  ASPNETCORE_Kestrel__Certificates__Default__Password: "YourSecurePassword"
  ASPNETCORE_Kestrel__Certificates__Default__Path: "%PATHTOYOURCERT%/my_web_domain.pfx"
```

There are better ways to do this, but this is the easiest to get started.
