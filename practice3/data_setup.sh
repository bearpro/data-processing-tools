su -l postgres 
psql 
\i /nyc_data/nyc_data_contagg.sql
\copy rides from /nyc_data/nyc_data_rides.csv csv
