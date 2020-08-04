create table quotes
(
	id bigint not null identity(1, 1) primary key,
	id_package bigint not null,
	package bigint not null,
	date datetime not null
)
