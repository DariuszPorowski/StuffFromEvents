FROM tomcat:8.5-jre8
LABEL maintainer="Dariusz.Porowski@microsoft.com"

COPY ./init_container.tomcat.sh /bin/
RUN apt-get update \
    && apt-get install -y --no-install-recommends dialog \
    && apt-get update \
    && apt-get install -y --no-install-recommends openssh-server \
    && echo "root:Docker!" | chpasswd \
    && rm -rf /usr/local/tomcat/webapps/ \
    && chmod 755 /bin/init_container.tomcat.sh
COPY ./sshd_config /etc/ssh/
COPY ./target/demo.war /usr/local/tomcat/webapps/ROOT.war

EXPOSE 8080 2222
ENTRYPOINT ["/bin/init_container.tomcat.sh"]