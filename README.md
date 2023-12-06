# Aplicación de Gestión de Citas

Esta aplicación de consola está diseñada para gestionar citas basadas en un archivo JSON alojado en un servidor remoto.
Permite verificar la disponibilidad de citas para un día específico y proporciona el número de espacios disponibles para
nuevas citas en ese día.

## Requisitos

Para ejecutar este código, necesitarás tener instalado el paquete NuGet `Newtonsoft.Json`

## Cómo Ejecutar

Este proyecto es una aplicación de consola. Puedes ejecutarlo desde tu entorno de desarrollo o usando el comando dotnet
run en la terminal después de ubicarte en la carpeta del proyecto.

## Prueba técnica

Un sistema de administración de agendas de servicios cuenta con un módulo que permite la
programación de citas para dichos servicios.
Estas citas deben tener una duración mínima de 30
minutos y una duración máxima de 90 minutos. El horario de atención del sistema se encuentra
entre las 9:00 y las 17:00 horas.
Se proporciona un archivo JSON como entrada, el cual contiene un array que hace referencia al día
de la semana, la hora de inicio y la duración de la cita programada.
En este contexto, se requiere la creación de un método que tome como parámetro el día de la
semana que se desea consultar y devuelva el cálculo del total de espacios disponibles para ese día,
teniendo en cuenta que la duración mínima de una cita es de 30 minutos.

- duración mínima cita = 30 minutos;
- duración máxima cita = 90 minutos;
- hora apertura = 9:00 24hr formato; -> TimeSpan
- hora cierre = 17:00 24hr formato; -> TimeSpan
- horas laborales = hora apertura - hora cierre = 8 hrs
- Objeto Cita
  - dia de la semana -> String
  - hora de inicio -> TimeSpan
  - duración cita -> int


  
