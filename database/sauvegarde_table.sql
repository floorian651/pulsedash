-- public.music definition

-- Drop table

-- DROP TABLE public.music;

CREATE TABLE public.music (
	title varchar NOT NULL,
	CONSTRAINT music_pk PRIMARY KEY (title)
);


-- public.playlist definition

-- Drop table

-- DROP TABLE public.playlist;

CREATE TABLE public.playlist (
	"name" varchar NOT NULL,
	CONSTRAINT playlist_pk PRIMARY KEY (name)
);


-- public."user" definition

-- Drop table

-- DROP TABLE public."user";

CREATE TABLE public."user" (
	id varchar NOT NULL,
	username varchar NULL,
	email varchar NULL,
	"password" varchar NULL,
	is_active bool NULL,
	CONSTRAINT user_pk PRIMARY KEY (id)
);


-- public.track definition

-- Drop table

-- DROP TABLE public.track;

CREATE TABLE public.track (
	"order" int4 NOT NULL,
	title varchar NOT NULL,
	playlist varchar NULL,
	CONSTRAINT track_music_fk FOREIGN KEY (title) REFERENCES public.music(title),
	CONSTRAINT track_playlist_fk FOREIGN KEY (playlist) REFERENCES public.playlist("name")
);


-- DROP TYPE public.jobstate;

CREATE TYPE public.jobstate AS ENUM (
	'PENDING',
	'RUNNING',
	'COMPLETED',
	'FAILED');

-- public.jobs definition

-- Drop table

-- DROP TABLE public.jobs;

CREATE TABLE public.jobs (
	id varchar NOT NULL,
	user_id varchar NULL,
	state public.jobstate NULL,
	progress int4 NULL,
	result_path varchar NULL,
	created_at timestamp NULL,
	updated_at timestamp NULL,
	CONSTRAINT jobs_pkey PRIMARY KEY (id)
);