CREATE TABLE users_v2 (
	id varchar(255) not null unique,
	name varchar(255) not null,
	botid varchar(255) not null,
	botname varchar(255) not null,
	serviceurl varchar(255) not null,
	token varchar(max) not null,
	conversationid varchar(255),
	channelid varchar(255),
);