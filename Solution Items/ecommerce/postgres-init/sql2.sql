CREATE TABLE IF NOT EXISTS public."Users"
(
    "UserID" uuid NOT NULL,
    "Email" character varying(255) COLLATE pg_catalog."default",
    "Password" character varying(255) COLLATE pg_catalog."default",
    "PersonName" character varying(100) COLLATE pg_catalog."default",
    "Gender" character varying(10) COLLATE pg_catalog."default",
    CONSTRAINT "PK_Users" PRIMARY KEY ("UserID")
);
