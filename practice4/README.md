# Задание

0. После работы №3 (загрузка данных Ny_taxi в PostgreSQL/Timescale)

1. Установить Grafana (пример для Debian 11 в https://otodiginet.com/software/how-to-install-grafana-8-on-debian-11/):
add-apt-repository "deb https://packages.grafana.com/oss/deb stable main"
wget -q -O - https://packages.grafana.com/gpg.key | apt-key add -
apt update
apt install grafana
systemctl daemon-reload
systemctl enable grafana-server
systemctl start grafana-server
systemctl status grafana-server должен выдать зелёный статус

2. Зайти в Графану:
http://ADDRESS:3000/login
admin/admin
3. Выполнить подключение источника данных из https://docs.timescale.com/timescaledb/latest/tutorials/grafana/
начиная с "Connect Grafana to your TimescaleDB instance" со следующими значениями
Выбрать БД nyc_data, 
пользователь postgres, пустой пароль, 
выключить TLS/SSL Mode (Disable)
Выбрать версию 12+
Включить TimescaleDB

3. Создать дашборд как в 
https://docs.timescale.com/timescaledb/latest/tutorials/grafana/create-dashboard-and-panel/#creating-a-grafana-dashboard-and-panel
(+ -> Create -> Dashboard -> Add an empty panel, а не как указано)
Тип визуализации - Time Series (а не Graph, как указано)
Получить графики для начала 2016 года с бакетами в 1 день и в 5 минут

4. Создать отображение на карте как 
https://docs.timescale.com/timescaledb/latest/tutorials/grafana/geospatial-dashboards/#build-a-geospatial-query
на панели выбрать тип "Geomap"

5. Результат представить в виде Word или PPT

6. Работы должны отличаться индивидуальностью


Для сдающих после 09.11.2022:

5. Загрузить имитацию датчиков IoT как в https://docs.timescale.com/timescaledb/latest/tutorials/simulate-iot-sensor-data/#step2



