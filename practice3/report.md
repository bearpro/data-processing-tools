# Подготовка окружения

Контейнер с данными:

```dockerfile
FROM binakot/postgresql-postgis-timescaledb
WORKDIR /nyc_data
RUN apk add wget &&\
    wget https://timescaledata.blob.core.windows.net/datasets/nyc_data.tar.gz &&\
    tar -xzf nyc_data.tar.gz
```

Настройка портов и пользователя

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
```

Загрузка данных

```bash
su -l postgres 
psql 
\i /nyc_data/nyc_data_contagg.sql
\copy rides from /nyc_data/nyc_data_rides.csv csv
```

# Выполнение анализа

## 10 персентиль высшей и низшей цены поездки по неделям

```sql
-- Средняя цена 10% наиболее дорогих поездок
select 
    week, AVG(fare) most_10p_fare
from (
    select 
        date_trunc('week', pickup_datetime) week,
        fare_amount fare, 
        COUNT(*) over (partition by pickup_datetime) as countrows, 
        ROW_NUMBER() over (partition by pickup_datetime order by fare_amount desc) as rowno 
    from rides) as innertab
where rowno <= floor(countrows*0.1+0.9)
group by week
```

| week                | most_10p_fare       |
|---------------------|---------------------|
| 2017-11-13 00:00:00 | 9.7500000000000000  |
| 2016-01-25 00:00:00 | 24.7567914518669279 |
| 2015-12-28 00:00:00 | 23.1560309954910380 |
| 2016-01-18 00:00:00 | 23.0599498867288772 |
| 2016-01-11 00:00:00 | 23.3200735269572489 |
| 2016-01-04 00:00:00 | 22.5887771928507864 |

```sql
-- Средняя цена 10% наименее дорогих поездок
select
     week, AVG(fare) low_10p_fare
 from (
     select
         date_trunc('week', pickup_datetime) week,
         fare_amount fare,
         COUNT(*) over (partition by pickup_datetime) as countrows,
         ROW_NUMBER() over (partition by pickup_datetime order by fare_amount desc) as rowno
     from rides) as innertab
 where rowno >= floor(countrows*0.9+0.1)
 group by week
 ```

| week                | low_10p_fare       |
|---------------------|--------------------|
| 2017-11-13 00:00:00 | 9.7500000000000000 |
| 2016-01-25 00:00:00 | 8.4252877119416417 |
| 2015-12-28 00:00:00 | 8.6222188521080740 |
| 2016-01-18 00:00:00 | 7.9154070110214420 |
| 2016-01-11 00:00:00 | 7.6614297398659432 |
| 2016-01-04 00:00:00 | 7.7706390852876879 |

## Анализ числа поездок по дням из- и в- Нью Айленд
```sql
select from_ni.day, from_ni.rides_from_new_island, to_ni.rides_to_new_island from
(
    select date_trunc('day', pickup_datetime) as day, count(*) rides_from_new_island
    from (
        select *,
            ('SRID=4326;POINT(' || pickup_longitude || ' ' || pickup_latitude || ')')::geometry pickup_point,
            ('SRID=4326;POINT(' || dropoff_longitude || ' ' || dropoff_latitude || ')')::geometry dropoff_point,
            ST_Polygon('LINESTRING(-74.051214 40.609331, -73.680036 40.898149, -72.119536 41.237050, -71.728974 40.947700, -73.965094 40.468782, -74.051214 40.609331)'::geometry, 4326) long_island_geom
            from rides
    ) rides_with_points
    where ST_Contains(long_island_geom, pickup_point) and not ST_Contains(long_island_geom, dropoff_point)
    group by date_trunc('day', pickup_datetime)
) as from_ni
join 
(
    select date_trunc('day', pickup_datetime) as day, count(*) rides_to_new_island
    from (
        select *,
            ('SRID=4326;POINT(' || pickup_longitude || ' ' || pickup_latitude || ')')::geometry pickup_point,
            ('SRID=4326;POINT(' || dropoff_longitude || ' ' || dropoff_latitude || ')')::geometry dropoff_point,
            ST_Polygon('LINESTRING(-74.051214 40.609331, -73.680036 40.898149, -72.119536 41.237050, -71.728974 40.947700, -73.965094 40.468782, -74.051214 40.609331)'::geometry, 4326) long_island_geom
            from rides
    ) rides_with_points
    where not ST_Contains(long_island_geom, pickup_point) and ST_Contains(long_island_geom, dropoff_point)
    group by date_trunc('day', pickup_datetime)
) to_ni on from_ni.day = to_ni.day
```

| day                 | rides_from_new_island | rides_to_new_island |
|---------------------|-----------------------|---------------------|
| 2016-01-01 00:00:00 | 7624                  | 14118               |
| 2016-01-02 00:00:00 | 7530                  | 8446                |
| 2016-01-03 00:00:00 | 9211                  | 10162               |
| 2016-01-04 00:00:00 | 8495                  | 7531                |
| 2016-01-05 00:00:00 | 7108                  | 7377                |
| 2016-01-06 00:00:00 | 6339                  | 7183                |
| 2016-01-07 00:00:00 | 5787                  | 7814                |
| 2016-01-08 00:00:00 | 5988                  | 8473                |
| 2016-01-09 00:00:00 | 6082                  | 9686                |
| 2016-01-10 00:00:00 | 7309                  | 10855               |
| 2016-01-11 00:00:00 | 7238                  | 7194                |
| 2016-01-12 00:00:00 | 5594                  | 7655                |
| 2016-01-13 00:00:00 | 5403                  | 8018                |
| 2016-01-14 00:00:00 | 5898                  | 9306                |
| 2016-01-15 00:00:00 | 7024                  | 10335               |
| 2016-01-16 00:00:00 | 6901                  | 10731               |
| 2016-01-17 00:00:00 | 6937                  | 10795               |
| 2016-01-18 00:00:00 | 8418                  | 7837                |
| 2016-01-19 00:00:00 | 6981                  | 7541                |
| 2016-01-20 00:00:00 | 5765                  | 8467                |
| 2016-01-21 00:00:00 | 5912                  | 8985                |
| 2016-01-22 00:00:00 | 6651                  | 9920                |
| 2016-01-23 00:00:00 | 719                   | 3476                |
| 2016-01-24 00:00:00 | 4285                  | 3999                |
| 2016-01-25 00:00:00 | 8795                  | 8400                |
| 2016-01-26 00:00:00 | 6651                  | 8240                |
| 2016-01-27 00:00:00 | 5794                  | 8308                |
| 2016-01-28 00:00:00 | 5593                  | 9271                |
| 2016-01-29 00:00:00 | 6129                  | 9857                |
| 2016-01-30 00:00:00 | 5826                  | 11550               |
| 2016-01-31 00:00:00 | 6862                  | 11836               |