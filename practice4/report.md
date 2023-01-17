# Подготовка окружения

К docker-compose из работы №3 добавлен сервис графаны, чтобы получилось:

Контейнер с данными:

```yml
version: '3'
services:
  dpt-postgres-db:
    container_name: dpt-postgres-db
    build: .
    environment:
      - POSTGRES_PASSWORD=postgres
    ports:
      - 5433:5432
  dpt-grafana:
    container_name: dpt-grafana
    image: grafana/grafana
    ports:
      - 3000:3000
```

Добавить источник:
