# Как работать с этим

```sh
docker-compose build # Билдим контейнер с исходными данными
docker-compose -d up # Запускаем контейнер
docker exec -it dpt-postgres-db bash # Подключаемся к контейнеру
su -l postgres # Логинимся за постгрю
psql # запускаем шелл-клиент для постгри
\i /nyc_data/nyc_data_contagg.sql # Добавляем таблицы в схему
\copy rides from /nyc_data/nyc_data_rides.csv csv # Загружаем данные в таблицы
```

Далее выполняем наши запросы.

# Задание

1. Изучить презентацию Хранение данных.pptx
    2. Изучить pg_postgis_timescale.pdf
    3. Установить PostgreSQL, timescale и postgis (пример отработан для Debian 11):
    https://docs.timescale.com/timescaledb/latest/how-to-guides/install-timescaledb/self-hosted/debian/installation-apt-debian/

2. su - postgres
3. Выполнить анализ поездок такси согласно статье:
https://docs.timescale.com/timescaledb/latest/tutorials/nyc-taxi-cab/#prerequisites
хотя бы два запроса (для всех разные)
4. Результат представить в виде Word или PPT

## для сдающих после 26.10.2022: 
5. Выполнить работу по сбору метрик с помощью Telegraf
https://docs.timescale.com/timescaledb/latest/tutorials/telegraf-output-plugin/#next-steps
