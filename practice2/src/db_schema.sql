-- Initial

CREATE TABLE public.metrics (
    date timestamp without time zone NOT NULL,
    temperature integer
);

-- Migration 1

create table public.obtained_add (
    id serial,
    "date" timestamp without time zone not null
);
alter table obtained_add add constraint obtained_pk primary key (id);

insert into obtained_add ("date") values (current_timestamp);

alter table metrics add column obtained_id integer;
update metrics set obtained_id = (select id from obtained_add);
alter table metrics alter column obtained_id set not null;
alter table metrics add constraint fk_metrics_obtained foreign key (obtained_id) references obtained_add (id);

-- Poland tenders
create table poland_tenders(id int, date timestamp, value float);
