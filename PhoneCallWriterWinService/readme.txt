Служба Windows "PhoneCallWriterWinService".

Данная служба дергает каждую минуту SQL запрос, ищет активные обзвоны и звонки в CRM.
Если они найдены, собирает по ним данные и отправляет в Kafka в топик "CALL_CENTER_ITEM".

Логи службы пишутся в "C:\\PhoneCallWriterWinService\\log.txt" (см. NLog.config).