FROM maven:3.5-jdk-8-alpine AS build
LABEL maintainer="Dariusz.Porowski@microsoft.com"
COPY ./app /usr/src/app/src
COPY ./pom.xml /usr/src/app
RUN mvn -f /usr/src/app/pom.xml clean package