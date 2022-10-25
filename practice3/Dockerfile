FROM binakot/postgresql-postgis-timescaledb
WORKDIR /nyc_data
RUN apk add wget &&\
    wget https://timescaledata.blob.core.windows.net/datasets/nyc_data.tar.gz &&\
    tar -xzf nyc_data.tar.gz