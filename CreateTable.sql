CREATE TABLE Quotes
(
	id int not null identity(1, 1) primary key,
	id_package int,
	package int,
	send_date datetime,
)