
# POC RabbitMq + MassTransit + SSL + Docker

Este repositório contém uma prova de conceito (POC) que demonstra a integração de tecnologias essenciais para a construção de sistemas de mensagens seguros e escaláveis. A prova de conceito inclui a configuração do RabbitMQ, a utilização do MassTransit como framework de mensagens, a implementação de SSL para garantir a segurança na comunicação, e a orquestração do ambiente de desenvolvimento com Docker.
  
## Visão Geral

### RabbitMQ  
O RabbitMQ é um poderoso sistema de mensagens de código aberto que atua como intermediário para aplicações distribuídas. Ele fornece uma plataforma robusta e escalável para transmitir mensagens entre componentes do sistema.

### MassTransit  
Masstransit é um framework para a construção de sistemas de mensagens em .NET. Ele simplifica a implementação de padrões de integração e mensageria, tornando mais fácil o desenvolvimento de sistemas distribuídos.

### SSL (Secure Sockets Layer)  
O uso de SSL é fundamental para proteger a comunicação entre os componentes de um sistema de mensagens. Neste projeto, foi configurado o RabbitMQ com SSL para garantir a segurança na troca de mensagens, foi utilizado a versão do TSL1.3.

### Docker  
Docker é uma plataforma de virtualização de contêineres que facilita a criação, distribuição e execução de aplicativos em contêineres isolados. Isso nos permite orquestrar facilmente o ambiente de desenvolvimento e garantir que todos os componentes do sistema funcionem em harmonia, nesta POC foi criado 3 aplicações (masstransit-worker, masstransit-web-api e RabbitMQ-app) em um container.

### Geração dos Certificados
Para geração dos certificados utilizamos o repositório do [tls-gen](https://github.com/rabbitmq/tls-gen), conforme documentação do site ofical do [RabbitMq](https://www.rabbitmq.com/ssl.html#automated-certificate-generation), porém existe uma implementação de mais uma propriedade quando for gerar o certificado, chamada **CN**, para o contexto do certificado significa **Common Name** onde será inserido o nome da imagem que foi gerado do servidor do RabbitMQ no Docker, segue o comando ao gerar o certificado seguindo a documentação citada no RabbitMQ:

*make **PASSWORD**=[seu password] **CN**=rabbitmq-app*

Com isto irá gerar uma pasta result (conforme documentação do RabbitMq), nela irá conter os certificados que podem ser utilizados no lado do server e em seus clientes.

### Certificados nas aplicações
Tanto na aplicação *MassTransit.Web.API* quanto na aplicação *MassTransit.Worker*, existe uma pasta onde tem os certificados utilizados, esta pasta chama-se **Certs_tls-gen**, na aplicação *MassTransit.Worker* existe os 4 arquivos (ca_certificate.pem, server_atlantico03753_certificate.pem, server_atlantico03753_key.pem, client_atlantico03753.p12) para serem utilizados no servidor do RabbitMQ conforme [documentação oficial](https://www.rabbitmq.com/ssl.html#enabling-tls), juntamente com este arquivos existe o arquivo de confgiuração do servidor do RabbitMQ chamado *10-defaults.conf* onde contém as configurações para habilitar o modo TLS e definir o caminho dos certificados, o mesmo é copiado para substituir o mesmo arquivo no servidor do RabbitMq, já na pasta *MassTransit.Web.API* existe apenas o certificado client_atlantico03753.p12.

Estes arquivos são utilizados nestas aplicações, todos são copiados para seus devidos containers via arquivos docker-compose.yml e os arquivos dockfiles que estão dentro das pastas das aplicações.

### Observação:
As 3 aplicações conforme este repositório estão trabalhando utilizando TSL1.3 na porta 5671, a porta 5672 está desativada, ou seja, **o servidor RabbitMQ só está aceitando conexão com certificado**.

### Requisitos de ambiente:
- [X]  Docker Desktop (Windows) ou Docker(Linux).
- [X]  Power Shell ou CMD.
- [X]  Visual Studio ou Visual Estudio Code.
- [X]  Postman ou qualquer navegador para consultar o servidor do RabbitMQ ou a aplicação MassTransit.Web.API.

### Passo-a-Passo (Step-by-Step)
- Abra o CMD ou PowerShell na pasta raiz do projeto.
- Execute o comando:
```
docker-compose up -d
```
- Após finalizado este processo será criado um container contendo as 3 apps já configuradas.
- Acesse em qualquer navegador o seguinte link: [http://localhost:15672/](http://localhost:15672/) para visualizar o servidor do RabbitMQ.
- Acesso com usuário e senha padrão (**user:** guest, **password:** guest).
- Pode testar publicando um registro na fila com o seguinte end-point no Postman ou outro similar:
```
localhost:5002/Nfe (método POST)
```
- Após isto poderá ver no servidor do RabbitMQ acontecendo o processo nas filas pelo link: [http://localhost:15672/#/queues](http://localhost:15672/#/queues)

#### Observações:
Caso precise instalar alguma coisa dentro de alguma aplicação no docker, pode abrir o terminal do mesmo e executar comandos, segue alguns exemplos, *lembrando que são opcionais*:  

**Instalar o git:**
```
apt-get update
apt install git
git --version
apt-get install make
```

**Iniciar testes com a aplicação testssl (o repositório já foi copiado para dentro do docker junto com a pasta dos certificados (comandos feitos no arquivo docker-compose.yml), execute um por vez no terminal do docker, este teste é recomendável executar no terminal da imagem do docker rabbitmq-app:**
```
apt-get update
apt-get install hexdump
apt-cache search hexdump
apt-get install bsdmainutils (digite Y quando solicitar)
apt-get install -y dnsutils

/etc/Certs_tls-gen/testssl/testssl.sh localhost:5671 (Digite Yes quando solicitar)
```
