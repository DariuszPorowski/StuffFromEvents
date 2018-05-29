FROM maven:3.5-jdk-8 AS build
COPY ./app /usr/src/app/src
COPY ./pom.xml /usr/src/app
RUN mvn -f /usr/src/app/pom.xml clean package

FROM tomcat:8.5-jre8 AS final
LABEL maintainer="Dariusz.Porowski@microsoft.com"
COPY ./init_container.tomcat.sh /bin/
COPY ./sshd_config /etc/ssh/
RUN apt-get update \
    && apt-get install -y --no-install-recommends dialog \
    && apt-get update \
    && apt-get install -y --no-install-recommends openssh-server \
    && echo "root:Docker!" | chpasswd \
    && rm -rf /usr/local/tomcat/webapps/ \
    && chmod 755 /bin/init_container.tomcat.sh
COPY --from=build /usr/src/app/target/demo.war /usr/local/tomcat/webapps/ROOT.war
EXPOSE 8080 2222
ENTRYPOINT ["/bin/init_container.tomcat.sh"]