# DEMO

## DEMO 1 - portal
[Create Web App for Containers](https://portal.azure.com/#create/Microsoft.AppSvcLinux)

## DEMO 1 - azcli
rgName=<rgName>
appSvcPlanName=<appSvcPlanName>
webAppName=<webAppName>
location=<location>

az group create --name $rgName --location $location
az appservice plan create --resource-group $rgName --name $appSvcPlanName --is-linux --sku B1 --location $location 
az webapp create --resource-group $rgName --name $webAppName --plan $appSvcPlanName --deployment-container-image-name tutum/hello-world

az webapp delete --resource-group $rgName --name $webAppName
az appservice plan delete --resource-group $rgName --name $appSvcPlanName
az group delete --name $rgName --yes

## DEMO 2
docker build . -f multi.tomcat.Dockerfile -t demo/javahellotomcat:0.0.1
docker run --rm -p 8080:8080 -p 2222:2222 -t demo/javahellotomcat:0.0.1

docker build . -f multi.alpine.Dockerfile -t demo/javahelloalpine:0.0.1
docker run --rm -p 8080:8080 -p 2222:2222 -t demo/javahelloalpine:0.0.1

## DEMO 3 - ssh
ssh localhost -p 2222 -c aes256-cbc -l root

## DEMO 3 - web ssh 
https://<app>.scm.azurewebsites.net/webssh/host

## DEMO 3 - logs
https://<app>.scm.azurewebsites.net/api/logs/docker

## Official Docker images for Tomcat/Java from Azure App Service team
[Apache Tomcat on Azure App Service](https://github.com/Azure-App-Service/tomcat/)