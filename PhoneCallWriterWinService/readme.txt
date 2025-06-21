Служба Windows "PhoneCallWriterWinService".

Данная служба дергает каждую минуту SQL запрос, ищет активные обзвоны и звонки в CRM.
Если они найдены, собирает по ним данные и отправляет в Kafka в топик "CALL_CENTER_ITEM".

Логи службы пишутся в "C:\\PhoneCallWriterWinService\\log.txt" (см. NLog.config).

Kafka развернута на сервере 192.168.0.21.

Информация для тестирования Kafka на сервере в CMD.exe представлена ниже.

cd C:\kafka_2.13-3.9.1\bin\windows

> Запуск Kafka
.\kafka-server-start.bat ..\..\config\kraft\server-1.properties
.\kafka-server-start.bat ..\..\config\kraft\server-2.properties
.\kafka-server-start.bat ..\..\config\kraft\server-3.properties

> Создание топика
.\kafka-topics.bat --create --topic registrations --bootstrap-server localhost:9092

> Запись в топик
.\kafka-console-producer.bat --topic registrations --bootstrap-server localhost:9092

> Чтение из Kafka
.\kafka-console-consumer.bat --topic registrations --bootstrap-server localhost:9092

консьюмер Кафки по умолчанию начинает читать данные с конца топика в тот момент, когда он запустился (см. настройку auto.offset.reset). 
Поэтому, чтобы прочитать данные, записанные ДО старта консьюмера, нужно переопределить эту конфигу.

> Запуск consumer'а правильно
.\kafka-console-consumer.bat --topic registrations --bootstrap-server localhost:9092 --consumer-property auto.offset.reset=earliest