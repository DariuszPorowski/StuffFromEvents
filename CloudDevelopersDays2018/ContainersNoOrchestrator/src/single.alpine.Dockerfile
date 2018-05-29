FROM tomcat:8.5-alpine
LABEL maintainer="Dariusz.Porowski@microsoft.com"

COPY ./init_container.alpine.sh /bin/
RUN apk add --no-cache --update openssh-server \
	&& echo "root:Docker!" | chpasswd \
    && apk update \
    && apk add --no-cache openrc \
    && rc-status \
    && touch /run/openrc/softlevel \
    && rm -rf /var/cache/apk/* /tmp/* \
    && rm -rf /usr/local/tomcat/webapps/ \
    && chmod 755 /bin/init_container.alpine.sh
COPY ./sshd_config /etc/ssh/
COPY ./target/demo.war /usr/local/tomcat/webapps/ROOT.war

EXPOSE 8080 2222
ENTRYPOINT ["/bin/init_container.alpine.sh"]