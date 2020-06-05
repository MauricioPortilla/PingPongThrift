# PingPongGameThrift
### Un juego de Ping Pong creado con Apache Thrift y desplegado con Docker.

## Requisitos
- Docker
- Docker Compose

## Instrucciones
1. Acceder a una terminal y cambiar el directorio a donde se encuentra este repositorio.
2. Ejecutar: `$ docker-compose build` para compilar.
3. Una vez concluido, ejecutar: `$ docker-compose up --no-start` para crear los contenedores, o para versiones anteriores de Docker: `$ docker-compose create`.
4. Ejecutar el servidor: `$ docker start NOMBRE_CONTENEDOR_SERVIDOR`. Sustituir `NOMBRE_CONTENEDOR_SERVIDOR` por el nombre del contenedor para el servidor que generó el paso 3.
5. Ejecutar el cliente en modo interactivo: `$ docker start -i NOMBRE_CONTENEDOR_CLIENTE`. Sustituir `NOMBRE_CONTENEDOR_CLIENTE` por el nombre del contenedor para el cliente que generó el paso 3.

## Extras
- Para el caso de Windows, abrir la interfaz de Docker, dirigirse a Settings, Resources, File Sharing, y añadir la ruta donde está la carpeta del usuario actual de Windows. Esto es para que no haya problemas con las carpetas compartidas al momento de compilar.
- En caso de que exista un error al iniciar por primera vez el contenedor del servidor o del cliente, volver a intentarlo por segunda ocasión.
