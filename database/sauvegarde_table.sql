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