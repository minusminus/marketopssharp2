--
-- PostgreSQL database dump
--

-- Dumped from database version 10.3
-- Dumped by pg_dump version 13.3

-- Started on 2021-12-07 10:32:01

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 2 (class 3079 OID 17898)
-- Name: tablefunc; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS tablefunc WITH SCHEMA public;


--
-- TOC entry 2547 (class 0 OID 0)
-- Dependencies: 2
-- Name: EXTENSION tablefunc; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION tablefunc IS 'functions that manipulate whole tables, including crosstab';


--
-- TOC entry 279 (class 1255 OID 16385)
-- Name: f_volavgselect(timestamp without time zone); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.f_volavgselect(ts timestamp without time zone, OUT nazwa_spolki character varying, OUT ts timestamp without time zone, OUT open numeric, OUT close numeric, OUT high numeric, OUT low numeric, OUT volume integer, OUT avgvol numeric, OUT voveravg numeric, OUT hlpcnt numeric, OUT ocpcnt numeric, OUT avgc numeric, OUT cavgc numeric, OUT coveravg numeric, OUT maxh numeric, OUT covermaxh numeric) RETURNS SETOF record
    LANGUAGE sql
    AS $_$
	--zapytanie budujace tabele
select s.nazwaspolki, svol.ts, svol.open, svol.close, svol.high, svol.low, svol.volume, 
	round(svol.avgvol, 2), round(svol.volume/svol.avgvol, 2) as "voveravg", 
	round((svol.high-svol.low)/svol.low, 2) as "hlpcnt", round((svol.close-svol.open)/svol.open, 2) as "ocpcnt",
	round(svol.avgc, 2), round(svol.close/svol.avgc, 2) as "cavgc", round((svol.close-svol.avgc)/svol.avgc, 2) as "coveravg",
	round(svol.maxh, 2), round((svol.close-svol.maxh)/svol.maxh, 2) as "covermaxh"
from at_spolki s, 
(
	select d0.fk_id_spolki, d0.ts, d0.high, d0.low, d0.open, d0.close, d0.volume, spavg.avgvol, spcavg.avgc, spcavg.maxh
	from at_dzienne0 d0,
	(	--avg volume
		select d.fk_id_spolki as "idspolki", avg(d.volume) as "avgvol"
		from at_dzienne0 d,
		(
			select s.id,
			(
				select ts	--ts ostatniego notowania z listy (200 notowanie)
				from at_dzienne0
				where fk_id_spolki=s.id
				order by ts desc
				limit 1 offset 200+1
			) as lastts
			from at_spolki s
			where s.aktywna=TRUE
		) ts
		where d.fk_id_spolki=ts.id
		and d.ts>=ts.lastts
		and d.ts<$1
		group by d.fk_id_spolki
	) spavg,
	(	--avg close price
		select d.fk_id_spolki as "idspolki", avg(d.close) as "avgc", max(d.high) as "maxh"
		from at_dzienne0 d,
		(
			select s.id,
			(
				select ts	--ts ostatniego notowania z listy (20 notowanie)
				from at_dzienne0
				where fk_id_spolki=s.id
				order by ts desc
				limit 1 offset 20+1
			) as lastts
			from at_spolki s
			where s.aktywna=TRUE
		) ts
		where d.fk_id_spolki=ts.id
		and d.ts>=ts.lastts
		and d.ts<$1
		group by d.fk_id_spolki
	) spcavg
	where d0.fk_id_spolki=spavg.idspolki and d0.fk_id_spolki=spcavg.idspolki
	and d0.ts=$1
) svol
where s.id=svol.fk_id_spolki
order by "voveravg" desc

$_$;


ALTER FUNCTION public.f_volavgselect(ts timestamp without time zone, OUT nazwa_spolki character varying, OUT ts timestamp without time zone, OUT open numeric, OUT close numeric, OUT high numeric, OUT low numeric, OUT volume integer, OUT avgvol numeric, OUT voveravg numeric, OUT hlpcnt numeric, OUT ocpcnt numeric, OUT avgc numeric, OUT cavgc numeric, OUT coveravg numeric, OUT maxh numeric, OUT covermaxh numeric) OWNER TO postgres;

SET default_tablespace = '';

--
-- TOC entry 200 (class 1259 OID 16424)
-- Name: at_biezace; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_biezace (
    id integer NOT NULL,
    fk_id_spolki integer,
    ts timestamp without time zone,
    val numeric(12,4)
);


ALTER TABLE public.at_biezace OWNER TO postgres;

--
-- TOC entry 201 (class 1259 OID 16427)
-- Name: at_biezace_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.at_biezace_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.at_biezace_id_seq OWNER TO postgres;

--
-- TOC entry 2548 (class 0 OID 0)
-- Dependencies: 201
-- Name: at_biezace_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.at_biezace_id_seq OWNED BY public.at_biezace.id;


--
-- TOC entry 202 (class 1259 OID 16429)
-- Name: at_ciagle0; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_ciagle0 (
    id integer NOT NULL,
    fk_id_spolki integer,
    ts timestamp without time zone,
    x integer,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    oi integer
);


ALTER TABLE public.at_ciagle0 OWNER TO postgres;

--
-- TOC entry 203 (class 1259 OID 16432)
-- Name: at_ciagle1; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_ciagle1 (
    id integer NOT NULL,
    fk_id_spolki integer,
    ts timestamp without time zone,
    x integer,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    oi integer
);


ALTER TABLE public.at_ciagle1 OWNER TO postgres;

--
-- TOC entry 204 (class 1259 OID 16435)
-- Name: at_ciagle1_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.at_ciagle1_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.at_ciagle1_id_seq OWNER TO postgres;

--
-- TOC entry 2549 (class 0 OID 0)
-- Dependencies: 204
-- Name: at_ciagle1_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.at_ciagle1_id_seq OWNED BY public.at_ciagle1.id;


--
-- TOC entry 205 (class 1259 OID 16437)
-- Name: at_ciagle2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_ciagle2 (
    id integer NOT NULL,
    fk_id_spolki integer,
    ts timestamp without time zone,
    x integer,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    oi integer
);


ALTER TABLE public.at_ciagle2 OWNER TO postgres;

--
-- TOC entry 206 (class 1259 OID 16440)
-- Name: at_ciagle2_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.at_ciagle2_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.at_ciagle2_id_seq OWNER TO postgres;

--
-- TOC entry 2550 (class 0 OID 0)
-- Dependencies: 206
-- Name: at_ciagle2_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.at_ciagle2_id_seq OWNED BY public.at_ciagle2.id;


--
-- TOC entry 207 (class 1259 OID 16442)
-- Name: at_ciagle6; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_ciagle6 (
    id integer NOT NULL,
    fk_id_spolki integer,
    ts timestamp without time zone,
    open numeric(12,6),
    high numeric(12,6),
    low numeric(12,6),
    close numeric(12,6),
    volume integer
);


ALTER TABLE public.at_ciagle6 OWNER TO postgres;

--
-- TOC entry 208 (class 1259 OID 16445)
-- Name: at_ciagle6_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.at_ciagle6_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.at_ciagle6_id_seq OWNER TO postgres;

--
-- TOC entry 2551 (class 0 OID 0)
-- Dependencies: 208
-- Name: at_ciagle6_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.at_ciagle6_id_seq OWNED BY public.at_ciagle6.id;


--
-- TOC entry 209 (class 1259 OID 16447)
-- Name: at_ciagle_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.at_ciagle_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.at_ciagle_id_seq OWNER TO postgres;

--
-- TOC entry 2552 (class 0 OID 0)
-- Dependencies: 209
-- Name: at_ciagle_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.at_ciagle_id_seq OWNED BY public.at_ciagle0.id;


--
-- TOC entry 210 (class 1259 OID 16449)
-- Name: at_dzienne0; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_dzienne0 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4) DEFAULT 0
);


ALTER TABLE public.at_dzienne0 OWNER TO postgres;

--
-- TOC entry 2553 (class 0 OID 0)
-- Dependencies: 210
-- Name: COLUMN at_dzienne0.refcourse; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.at_dzienne0.refcourse IS 'kurs odniesienia';


--
-- TOC entry 211 (class 1259 OID 16453)
-- Name: at_dzienne1; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_dzienne1 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4) DEFAULT 0
);


ALTER TABLE public.at_dzienne1 OWNER TO postgres;

--
-- TOC entry 2554 (class 0 OID 0)
-- Dependencies: 211
-- Name: COLUMN at_dzienne1.refcourse; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.at_dzienne1.refcourse IS 'kurs odniesienia';


--
-- TOC entry 212 (class 1259 OID 16459)
-- Name: at_dzienne2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_dzienne2 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_dzienne2 OWNER TO postgres;

--
-- TOC entry 213 (class 1259 OID 16464)
-- Name: at_dzienne4; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_dzienne4 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_dzienne4 OWNER TO postgres;

--
-- TOC entry 214 (class 1259 OID 16469)
-- Name: at_dzienne5; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_dzienne5 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_dzienne5 OWNER TO postgres;

--
-- TOC entry 215 (class 1259 OID 16474)
-- Name: at_dzienne6; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_dzienne6 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,6),
    high numeric(12,6),
    low numeric(12,6),
    close numeric(12,6),
    volume integer,
    refcourse numeric(12,6)
);


ALTER TABLE public.at_dzienne6 OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 16481)
-- Name: at_intra15m2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_intra15m2 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_intra15m2 OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16496)
-- Name: at_intra1m2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_intra1m2 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_intra1m2 OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 16511)
-- Name: at_intra5m2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_intra5m2 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_intra5m2 OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16526)
-- Name: at_intra60m2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_intra60m2 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_intra60m2 OWNER TO postgres;

--
-- TOC entry 259 (class 1259 OID 18706)
-- Name: at_mies0; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_mies0 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_mies0 OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 16541)
-- Name: at_mies1; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_mies1 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    x integer,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    oi integer
);


ALTER TABLE public.at_mies1 OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16546)
-- Name: at_mies2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_mies2 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    x integer,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    oi integer
);


ALTER TABLE public.at_mies2 OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 16551)
-- Name: at_mies4; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_mies4 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_mies4 OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16556)
-- Name: at_mies5; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_mies5 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_mies5 OWNER TO postgres;

--
-- TOC entry 260 (class 1259 OID 18712)
-- Name: at_mies6; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_mies6 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_mies6 OWNER TO postgres;

--
-- TOC entry 199 (class 1259 OID 16416)
-- Name: at_pp; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_pp (
    fk_id_spolki integer NOT NULL,
    last_session_ts timestamp without time zone NOT NULL
);


ALTER TABLE public.at_pp OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 16561)
-- Name: at_serie; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_serie (
    fk_id_spolki integer NOT NULL,
    nazwa character varying(20) NOT NULL,
    start timestamp without time zone,
    stop timestamp without time zone,
    biezaca timestamp without time zone
);


ALTER TABLE public.at_serie OWNER TO postgres;

--
-- TOC entry 2555 (class 0 OID 0)
-- Dependencies: 224
-- Name: TABLE at_serie; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.at_serie IS 'serie kontraktow terminowych';


--
-- TOC entry 225 (class 1259 OID 16566)
-- Name: at_split; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_split (
    fk_id_spolki integer NOT NULL,
    last_session_ts timestamp without time zone NOT NULL,
    reverse integer,
    split integer,
    split_ts timestamp without time zone NOT NULL,
    nazwa character varying(512)
);


ALTER TABLE public.at_split OWNER TO postgres;

--
-- TOC entry 2556 (class 0 OID 0)
-- Dependencies: 225
-- Name: TABLE at_split; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.at_split IS 'splity';


--
-- TOC entry 226 (class 1259 OID 16572)
-- Name: at_spolki_disabled; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_spolki_disabled (
    id integer NOT NULL,
    nazwaspolki character varying(512),
    nazwaakcji character varying(32),
    skrotakcji character varying(10),
    ciaglepos integer DEFAULT 0,
    typ integer DEFAULT 0,
    indexspolki text,
    dziennepos integer DEFAULT 0,
    nazwaakcji2 character varying(32),
    startts timestamp without time zone,
    aktywna boolean DEFAULT true,
    opis character varying(100)
);


ALTER TABLE public.at_spolki_disabled OWNER TO postgres;

--
-- TOC entry 2557 (class 0 OID 0)
-- Dependencies: 226
-- Name: COLUMN at_spolki_disabled.typ; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.at_spolki_disabled.typ IS 'typ danych w tabeli:
0 - spolka (np PKOBP)
1 - indeks (np WIG20)';


--
-- TOC entry 2558 (class 0 OID 0)
-- Dependencies: 226
-- Name: COLUMN at_spolki_disabled.indexspolki; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.at_spolki_disabled.indexspolki IS 'lista spolek danego indeksu (tylko dla typ=1)
jako lista nazw akcji dla selecta';


--
-- TOC entry 2559 (class 0 OID 0)
-- Dependencies: 226
-- Name: COLUMN at_spolki_disabled.nazwaakcji2; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.at_spolki_disabled.nazwaakcji2 IS 'nazwa akcji dla danych dziennych metastock';


--
-- TOC entry 2560 (class 0 OID 0)
-- Dependencies: 226
-- Name: COLUMN at_spolki_disabled.startts; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN public.at_spolki_disabled.startts IS 'pierwszy dzien notowan';


--
-- TOC entry 227 (class 1259 OID 16582)
-- Name: at_spolki_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.at_spolki_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.at_spolki_id_seq OWNER TO postgres;

--
-- TOC entry 2561 (class 0 OID 0)
-- Dependencies: 227
-- Name: at_spolki_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.at_spolki_id_seq OWNED BY public.at_spolki_disabled.id;


--
-- TOC entry 257 (class 1259 OID 17959)
-- Name: at_spolki2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_spolki2 (
    id integer DEFAULT nextval('public.at_spolki_id_seq'::regclass) NOT NULL,
    enabled boolean DEFAULT true NOT NULL,
    stock_type integer NOT NULL,
    stock_fullname character varying(500) NOT NULL,
    stock_name character varying(50) NOT NULL,
    stock_short character varying(20),
    stock_description character varying(1000),
    filename_daily character varying(100),
    filename_intra character varying(100),
    start_ts timestamp without time zone
);


ALTER TABLE public.at_spolki2 OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 16584)
-- Name: at_ticks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_ticks (
    id integer NOT NULL,
    fk_id_spolki integer,
    ts timestamp without time zone,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer
);


ALTER TABLE public.at_ticks OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 16587)
-- Name: at_ticks_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.at_ticks_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.at_ticks_id_seq OWNER TO postgres;

--
-- TOC entry 2562 (class 0 OID 0)
-- Dependencies: 229
-- Name: at_ticks_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.at_ticks_id_seq OWNED BY public.at_ticks.id;


--
-- TOC entry 258 (class 1259 OID 18697)
-- Name: at_tyg0; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_tyg0 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_tyg0 OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 16589)
-- Name: at_tyg1; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_tyg1 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    x integer,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    oi integer
);


ALTER TABLE public.at_tyg1 OWNER TO postgres;

--
-- TOC entry 231 (class 1259 OID 16594)
-- Name: at_tyg2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_tyg2 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    x integer,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    oi integer
);


ALTER TABLE public.at_tyg2 OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 16599)
-- Name: at_tyg4; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_tyg4 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_tyg4 OWNER TO postgres;

--
-- TOC entry 233 (class 1259 OID 16604)
-- Name: at_tyg5; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_tyg5 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_tyg5 OWNER TO postgres;

--
-- TOC entry 261 (class 1259 OID 18718)
-- Name: at_tyg6; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_tyg6 (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4)
);


ALTER TABLE public.at_tyg6 OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 16609)
-- Name: at_typy; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.at_typy (
    id integer NOT NULL,
    typename character varying(32)
);


ALTER TABLE public.at_typy OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 17814)
-- Name: autotest_table; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.autotest_table (
    id integer
);


ALTER TABLE public.autotest_table OWNER TO postgres;

--
-- TOC entry 250 (class 1259 OID 17806)
-- Name: bossa_downloadurls; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.bossa_downloadurls (
    typ integer NOT NULL,
    path_dzienne character varying(250),
    path_intra character varying(250),
    file_dzienne character varying(50)
);


ALTER TABLE public.bossa_downloadurls OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 16612)
-- Name: mp_dzienne30m2; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.mp_dzienne30m2 (
    id integer NOT NULL,
    fk_id_spolki integer,
    ts timestamp without time zone,
    cp numeric(12,4),
    val numeric(12,4),
    vah numeric(12,4),
    avg numeric(12,4)
);


ALTER TABLE public.mp_dzienne30m2 OWNER TO postgres;

--
-- TOC entry 236 (class 1259 OID 16615)
-- Name: mp_dzienne30m2_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.mp_dzienne30m2_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.mp_dzienne30m2_id_seq OWNER TO postgres;

--
-- TOC entry 2563 (class 0 OID 0)
-- Dependencies: 236
-- Name: mp_dzienne30m2_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.mp_dzienne30m2_id_seq OWNED BY public.mp_dzienne30m2.id;


--
-- TOC entry 237 (class 1259 OID 16617)
-- Name: mp_dzienne30m2tpo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.mp_dzienne30m2tpo (
    id integer NOT NULL,
    fk_id_spolki integer,
    fk_id_mp integer,
    val numeric(12,4),
    tpocnt integer,
    tpo character varying(1024)
);


ALTER TABLE public.mp_dzienne30m2tpo OWNER TO postgres;

--
-- TOC entry 238 (class 1259 OID 16623)
-- Name: mp_dzienne30m2tpo_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.mp_dzienne30m2tpo_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.mp_dzienne30m2tpo_id_seq OWNER TO postgres;

--
-- TOC entry 2564 (class 0 OID 0)
-- Dependencies: 238
-- Name: mp_dzienne30m2tpo_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.mp_dzienne30m2tpo_id_seq OWNED BY public.mp_dzienne30m2tpo.id;


--
-- TOC entry 239 (class 1259 OID 16629)
-- Name: s_log; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.s_log (
    id integer NOT NULL,
    fk_system integer,
    fk_stock integer,
    status smallint,
    dir smallint,
    type character varying(16),
    entryts timestamp without time zone,
    entryprice numeric(8,2),
    initstop numeric(8,2),
    exitts timestamp without time zone,
    exitprice numeric(8,2),
    ticks integer,
    volume integer,
    entrycomm numeric(8,2),
    exitcomm numeric(8,2),
    profit numeric(8,2),
    currstop numeric(8,2)
);


ALTER TABLE public.s_log OWNER TO postgres;

--
-- TOC entry 2565 (class 0 OID 0)
-- Dependencies: 239
-- Name: TABLE s_log; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.s_log IS 'log transakcji systemowych symulacji';


--
-- TOC entry 240 (class 1259 OID 16632)
-- Name: s_log_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.s_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.s_log_id_seq OWNER TO postgres;

--
-- TOC entry 2566 (class 0 OID 0)
-- Dependencies: 240
-- Name: s_log_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.s_log_id_seq OWNED BY public.s_log.id;


--
-- TOC entry 241 (class 1259 OID 16634)
-- Name: s_params; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.s_params (
    fk_system integer NOT NULL,
    fk_stock integer NOT NULL,
    paramname character varying(128) NOT NULL,
    val character varying(128)
);


ALTER TABLE public.s_params OWNER TO postgres;

--
-- TOC entry 2567 (class 0 OID 0)
-- Dependencies: 241
-- Name: TABLE s_params; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.s_params IS 'parametry symulacji ';


--
-- TOC entry 242 (class 1259 OID 16643)
-- Name: s_signals; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.s_signals (
    id integer NOT NULL,
    fk_system integer,
    fk_stock integer,
    ts timestamp without time zone,
    signal smallint,
    enteronprice numeric(8,2),
    enteronopen boolean,
    enteronclose boolean,
    signal2 smallint,
    enteronprice2 numeric(8,2),
    targetprice numeric(8,2),
    targetprice2 numeric(8,2),
    addtopos boolean
);


ALTER TABLE public.s_signals OWNER TO postgres;

--
-- TOC entry 2568 (class 0 OID 0)
-- Dependencies: 242
-- Name: TABLE s_signals; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.s_signals IS 'sygnaly systemow';


--
-- TOC entry 243 (class 1259 OID 16646)
-- Name: s_signals_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.s_signals_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.s_signals_id_seq OWNER TO postgres;

--
-- TOC entry 2569 (class 0 OID 0)
-- Dependencies: 243
-- Name: s_signals_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.s_signals_id_seq OWNED BY public.s_signals.id;


--
-- TOC entry 244 (class 1259 OID 16650)
-- Name: s_systems; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.s_systems (
    id integer NOT NULL,
    enabled boolean,
    systemname character varying(128),
    sysfilename character varying(1024),
    lastsimts timestamp without time zone,
    startsimts timestamp without time zone,
    mmfilename character varying(1024),
    mindatalen integer
);


ALTER TABLE public.s_systems OWNER TO postgres;

--
-- TOC entry 2570 (class 0 OID 0)
-- Dependencies: 244
-- Name: TABLE s_systems; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.s_systems IS 'systemy transakcyjne';


--
-- TOC entry 245 (class 1259 OID 16656)
-- Name: s_systems_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.s_systems_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.s_systems_id_seq OWNER TO postgres;

--
-- TOC entry 2571 (class 0 OID 0)
-- Dependencies: 245
-- Name: s_systems_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.s_systems_id_seq OWNED BY public.s_systems.id;


--
-- TOC entry 246 (class 1259 OID 16658)
-- Name: s_systemstocks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.s_systemstocks (
    fk_system integer NOT NULL,
    fk_stock integer NOT NULL,
    lastsimts timestamp without time zone,
    aktywna boolean
);


ALTER TABLE public.s_systemstocks OWNER TO postgres;

--
-- TOC entry 2572 (class 0 OID 0)
-- Dependencies: 246
-- Name: TABLE s_systemstocks; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.s_systemstocks IS 'spolki na ktorych dziala system';


--
-- TOC entry 247 (class 1259 OID 16665)
-- Name: s_vars; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.s_vars (
    fk_system integer NOT NULL,
    fk_stock integer NOT NULL,
    paramname character varying(128) NOT NULL,
    val character varying(4000)
);


ALTER TABLE public.s_vars OWNER TO postgres;

--
-- TOC entry 2573 (class 0 OID 0)
-- Dependencies: 247
-- Name: TABLE s_vars; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON TABLE public.s_vars IS 'zmienne symulacji ';


--
-- TOC entry 249 (class 1259 OID 17336)
-- Name: test; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.test (
    aa numeric(12,4)
);


ALTER TABLE public.test OWNER TO postgres;

--
-- TOC entry 253 (class 1259 OID 17822)
-- Name: test_dzienne; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.test_dzienne (
    fk_id_spolki integer NOT NULL,
    ts timestamp without time zone NOT NULL,
    open numeric(12,4),
    high numeric(12,4),
    low numeric(12,4),
    close numeric(12,4),
    volume integer,
    refcourse numeric(12,4) DEFAULT 0
);


ALTER TABLE public.test_dzienne OWNER TO postgres;

--
-- TOC entry 252 (class 1259 OID 17820)
-- Name: test_dzienne_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.test_dzienne_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.test_dzienne_id_seq OWNER TO postgres;

--
-- TOC entry 248 (class 1259 OID 16671)
-- Name: tmp_spolki; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tmp_spolki (
    stock_fullname character varying(500) NOT NULL,
    filename_daily character varying(100) NOT NULL
);


ALTER TABLE public.tmp_spolki OWNER TO postgres;

--
-- TOC entry 2267 (class 2604 OID 16682)
-- Name: at_biezace id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_biezace ALTER COLUMN id SET DEFAULT nextval('public.at_biezace_id_seq'::regclass);


--
-- TOC entry 2268 (class 2604 OID 16683)
-- Name: at_ciagle0 id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ciagle0 ALTER COLUMN id SET DEFAULT nextval('public.at_ciagle_id_seq'::regclass);


--
-- TOC entry 2269 (class 2604 OID 16684)
-- Name: at_ciagle1 id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ciagle1 ALTER COLUMN id SET DEFAULT nextval('public.at_ciagle1_id_seq'::regclass);


--
-- TOC entry 2270 (class 2604 OID 16685)
-- Name: at_ciagle2 id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ciagle2 ALTER COLUMN id SET DEFAULT nextval('public.at_ciagle2_id_seq'::regclass);


--
-- TOC entry 2271 (class 2604 OID 16686)
-- Name: at_ciagle6 id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ciagle6 ALTER COLUMN id SET DEFAULT nextval('public.at_ciagle6_id_seq'::regclass);


--
-- TOC entry 2278 (class 2604 OID 16711)
-- Name: at_spolki_disabled id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_spolki_disabled ALTER COLUMN id SET DEFAULT nextval('public.at_spolki_id_seq'::regclass);


--
-- TOC entry 2279 (class 2604 OID 16712)
-- Name: at_ticks id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ticks ALTER COLUMN id SET DEFAULT nextval('public.at_ticks_id_seq'::regclass);


--
-- TOC entry 2280 (class 2604 OID 16717)
-- Name: mp_dzienne30m2 id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mp_dzienne30m2 ALTER COLUMN id SET DEFAULT nextval('public.mp_dzienne30m2_id_seq'::regclass);


--
-- TOC entry 2281 (class 2604 OID 16718)
-- Name: mp_dzienne30m2tpo id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mp_dzienne30m2tpo ALTER COLUMN id SET DEFAULT nextval('public.mp_dzienne30m2tpo_id_seq'::regclass);


--
-- TOC entry 2282 (class 2604 OID 16719)
-- Name: s_signals id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.s_signals ALTER COLUMN id SET DEFAULT nextval('public.s_signals_id_seq'::regclass);


--
-- TOC entry 2283 (class 2604 OID 16720)
-- Name: s_systems id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.s_systems ALTER COLUMN id SET DEFAULT nextval('public.s_systems_id_seq'::regclass);


--
-- TOC entry 2402 (class 2606 OID 17813)
-- Name: bossa_downloadurls bossa_downloadurls_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.bossa_downloadurls
    ADD CONSTRAINT bossa_downloadurls_pkey PRIMARY KEY (typ);


--
-- TOC entry 2295 (class 2606 OID 17333)
-- Name: at_ciagle0 ix_pk_ciagle; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ciagle0
    ADD CONSTRAINT ix_pk_ciagle PRIMARY KEY (id);


--
-- TOC entry 2357 (class 2606 OID 17278)
-- Name: at_spolki_disabled ix_spolki; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_spolki_disabled
    ADD CONSTRAINT ix_spolki PRIMARY KEY (id);


--
-- TOC entry 2310 (class 2606 OID 17860)
-- Name: at_dzienne0 pk_at_dzienne0; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_dzienne0
    ADD CONSTRAINT pk_at_dzienne0 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2313 (class 2606 OID 17856)
-- Name: at_dzienne1 pk_at_dzienne1; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_dzienne1
    ADD CONSTRAINT pk_at_dzienne1 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2316 (class 2606 OID 17854)
-- Name: at_dzienne2 pk_at_dzienne2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_dzienne2
    ADD CONSTRAINT pk_at_dzienne2 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2319 (class 2606 OID 17852)
-- Name: at_dzienne4 pk_at_dzienne4; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_dzienne4
    ADD CONSTRAINT pk_at_dzienne4 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2322 (class 2606 OID 17850)
-- Name: at_dzienne5 pk_at_dzienne5; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_dzienne5
    ADD CONSTRAINT pk_at_dzienne5 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2325 (class 2606 OID 17848)
-- Name: at_dzienne6 pk_at_dzienne6; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_dzienne6
    ADD CONSTRAINT pk_at_dzienne6 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2328 (class 2606 OID 17866)
-- Name: at_intra15m2 pk_at_intra15m2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_intra15m2
    ADD CONSTRAINT pk_at_intra15m2 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2331 (class 2606 OID 17862)
-- Name: at_intra1m2 pk_at_intra1m2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_intra1m2
    ADD CONSTRAINT pk_at_intra1m2 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2334 (class 2606 OID 17864)
-- Name: at_intra5m2 pk_at_intra5m2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_intra5m2
    ADD CONSTRAINT pk_at_intra5m2 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2337 (class 2606 OID 17868)
-- Name: at_intra60m2 pk_at_intra60m2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_intra60m2
    ADD CONSTRAINT pk_at_intra60m2 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2414 (class 2606 OID 18710)
-- Name: at_mies0 pk_at_mies0; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_mies0
    ADD CONSTRAINT pk_at_mies0 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2340 (class 2606 OID 17838)
-- Name: at_mies1 pk_at_mies1; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_mies1
    ADD CONSTRAINT pk_at_mies1 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2343 (class 2606 OID 17836)
-- Name: at_mies2 pk_at_mies2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_mies2
    ADD CONSTRAINT pk_at_mies2 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2346 (class 2606 OID 17834)
-- Name: at_mies4 pk_at_mies4; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_mies4
    ADD CONSTRAINT pk_at_mies4 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2349 (class 2606 OID 17832)
-- Name: at_mies5 pk_at_mies5; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_mies5
    ADD CONSTRAINT pk_at_mies5 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2417 (class 2606 OID 18716)
-- Name: at_mies6 pk_at_mies6; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_mies6
    ADD CONSTRAINT pk_at_mies6 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2288 (class 2606 OID 17870)
-- Name: at_pp pk_at_pp; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_pp
    ADD CONSTRAINT pk_at_pp PRIMARY KEY (fk_id_spolki, last_session_ts);


--
-- TOC entry 2352 (class 2606 OID 17872)
-- Name: at_serie pk_at_serie; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_serie
    ADD CONSTRAINT pk_at_serie PRIMARY KEY (fk_id_spolki, nazwa);


--
-- TOC entry 2408 (class 2606 OID 17968)
-- Name: at_spolki2 pk_at_spolki2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_spolki2
    ADD CONSTRAINT pk_at_spolki2 PRIMARY KEY (id);


--
-- TOC entry 2411 (class 2606 OID 18701)
-- Name: at_tyg0 pk_at_tyg0; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_tyg0
    ADD CONSTRAINT pk_at_tyg0 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2364 (class 2606 OID 17846)
-- Name: at_tyg1 pk_at_tyg1; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_tyg1
    ADD CONSTRAINT pk_at_tyg1 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2367 (class 2606 OID 17844)
-- Name: at_tyg2 pk_at_tyg2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_tyg2
    ADD CONSTRAINT pk_at_tyg2 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2370 (class 2606 OID 17842)
-- Name: at_tyg4 pk_at_tyg4; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_tyg4
    ADD CONSTRAINT pk_at_tyg4 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2373 (class 2606 OID 17840)
-- Name: at_tyg5 pk_at_tyg5; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_tyg5
    ADD CONSTRAINT pk_at_tyg5 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2420 (class 2606 OID 18722)
-- Name: at_tyg6 pk_at_tyg6; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_tyg6
    ADD CONSTRAINT pk_at_tyg6 PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2355 (class 2606 OID 17275)
-- Name: at_split pk_atsplit; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_split
    ADD CONSTRAINT pk_atsplit PRIMARY KEY (fk_id_spolki, last_session_ts, split_ts);


--
-- TOC entry 2291 (class 2606 OID 17169)
-- Name: at_biezace pk_biezace; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_biezace
    ADD CONSTRAINT pk_biezace PRIMARY KEY (id);


--
-- TOC entry 2299 (class 2606 OID 17325)
-- Name: at_ciagle1 pk_ciagle1; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ciagle1
    ADD CONSTRAINT pk_ciagle1 PRIMARY KEY (id);


--
-- TOC entry 2303 (class 2606 OID 17327)
-- Name: at_ciagle2 pk_ciagle2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ciagle2
    ADD CONSTRAINT pk_ciagle2 PRIMARY KEY (id);


--
-- TOC entry 2307 (class 2606 OID 17178)
-- Name: at_ciagle6 pk_ciagle6; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ciagle6
    ADD CONSTRAINT pk_ciagle6 PRIMARY KEY (id);


--
-- TOC entry 2378 (class 2606 OID 17300)
-- Name: mp_dzienne30m2 pk_mp_dzienne30m2; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mp_dzienne30m2
    ADD CONSTRAINT pk_mp_dzienne30m2 PRIMARY KEY (id);


--
-- TOC entry 2381 (class 2606 OID 17303)
-- Name: mp_dzienne30m2tpo pk_mp_dzienne30m2tpo; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.mp_dzienne30m2tpo
    ADD CONSTRAINT pk_mp_dzienne30m2tpo PRIMARY KEY (id);


--
-- TOC entry 2384 (class 2606 OID 17306)
-- Name: s_log pk_s_log; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.s_log
    ADD CONSTRAINT pk_s_log PRIMARY KEY (id);


--
-- TOC entry 2387 (class 2606 OID 17309)
-- Name: s_params pk_s_params; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.s_params
    ADD CONSTRAINT pk_s_params PRIMARY KEY (fk_system, fk_stock, paramname);


--
-- TOC entry 2390 (class 2606 OID 17312)
-- Name: s_signals pk_s_signals; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.s_signals
    ADD CONSTRAINT pk_s_signals PRIMARY KEY (id);


--
-- TOC entry 2392 (class 2606 OID 17315)
-- Name: s_systems pk_s_systems; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.s_systems
    ADD CONSTRAINT pk_s_systems PRIMARY KEY (id);


--
-- TOC entry 2395 (class 2606 OID 17317)
-- Name: s_systemstocks pk_s_systemstocks; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.s_systemstocks
    ADD CONSTRAINT pk_s_systemstocks PRIMARY KEY (fk_system, fk_stock);


--
-- TOC entry 2398 (class 2606 OID 17320)
-- Name: s_vars pk_s_vars; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.s_vars
    ADD CONSTRAINT pk_s_vars PRIMARY KEY (fk_system, fk_stock, paramname);


--
-- TOC entry 2405 (class 2606 OID 17877)
-- Name: test_dzienne pk_test_dzienne; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.test_dzienne
    ADD CONSTRAINT pk_test_dzienne PRIMARY KEY (fk_id_spolki, ts);


--
-- TOC entry 2361 (class 2606 OID 17281)
-- Name: at_ticks pk_ticks; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_ticks
    ADD CONSTRAINT pk_ticks PRIMARY KEY (id);


--
-- TOC entry 2400 (class 2606 OID 17974)
-- Name: tmp_spolki pk_tmpspolki; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tmp_spolki
    ADD CONSTRAINT pk_tmpspolki PRIMARY KEY (stock_fullname);


--
-- TOC entry 2375 (class 2606 OID 17298)
-- Name: at_typy pk_typy; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.at_typy
    ADD CONSTRAINT pk_typy PRIMARY KEY (id);


--
-- TOC entry 2350 (class 1259 OID 17873)
-- Name: ix_at_serie_spolkanazwa; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_at_serie_spolkanazwa ON public.at_serie USING btree (fk_id_spolki, nazwa);

ALTER TABLE public.at_serie CLUSTER ON ix_at_serie_spolkanazwa;


--
-- TOC entry 2353 (class 1259 OID 17276)
-- Name: ix_atsplit_fkspolka_lastts; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_atsplit_fkspolka_lastts ON public.at_split USING btree (fk_id_spolki, last_session_ts);

ALTER TABLE public.at_split CLUSTER ON ix_atsplit_fkspolka_lastts;


--
-- TOC entry 2289 (class 1259 OID 17170)
-- Name: ix_biezace_fkidspolki_ts; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_biezace_fkidspolki_ts ON public.at_biezace USING btree (fk_id_spolki, ts);


--
-- TOC entry 2296 (class 1259 OID 17328)
-- Name: ix_ciagle1_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ix_ciagle1_id ON public.at_ciagle1 USING btree (id);


--
-- TOC entry 2297 (class 1259 OID 17329)
-- Name: ix_ciagle1_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_ciagle1_spolkats ON public.at_ciagle1 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_ciagle1 CLUSTER ON ix_ciagle1_spolkats;


--
-- TOC entry 2300 (class 1259 OID 17330)
-- Name: ix_ciagle2_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_ciagle2_id ON public.at_ciagle2 USING btree (id);


--
-- TOC entry 2301 (class 1259 OID 17331)
-- Name: ix_ciagle2_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_ciagle2_spolkats ON public.at_ciagle2 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_ciagle2 CLUSTER ON ix_ciagle2_spolkats;


--
-- TOC entry 2304 (class 1259 OID 17179)
-- Name: ix_ciagle6_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_ciagle6_id ON public.at_ciagle6 USING btree (id);


--
-- TOC entry 2305 (class 1259 OID 17180)
-- Name: ix_ciagle6_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_ciagle6_spolkats ON public.at_ciagle6 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_ciagle6 CLUSTER ON ix_ciagle6_spolkats;


--
-- TOC entry 2292 (class 1259 OID 17334)
-- Name: ix_ciagle_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ix_ciagle_id ON public.at_ciagle0 USING btree (id);


--
-- TOC entry 2293 (class 1259 OID 17335)
-- Name: ix_ciagle_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_ciagle_spolkats ON public.at_ciagle0 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_ciagle0 CLUSTER ON ix_ciagle_spolkats;


--
-- TOC entry 2311 (class 1259 OID 17188)
-- Name: ix_dzienne1_dpolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_dzienne1_dpolkats ON public.at_dzienne1 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_dzienne1 CLUSTER ON ix_dzienne1_dpolkats;


--
-- TOC entry 2314 (class 1259 OID 17192)
-- Name: ix_dzienne2_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_dzienne2_spolkats ON public.at_dzienne2 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_dzienne2 CLUSTER ON ix_dzienne2_spolkats;


--
-- TOC entry 2317 (class 1259 OID 17196)
-- Name: ix_dzienne4_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_dzienne4_spolkats ON public.at_dzienne4 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_dzienne4 CLUSTER ON ix_dzienne4_spolkats;


--
-- TOC entry 2320 (class 1259 OID 17200)
-- Name: ix_dzienne5_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_dzienne5_spolkats ON public.at_dzienne5 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_dzienne5 CLUSTER ON ix_dzienne5_spolkats;


--
-- TOC entry 2323 (class 1259 OID 17204)
-- Name: ix_dzienne6_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_dzienne6_spolkats ON public.at_dzienne6 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_dzienne6 CLUSTER ON ix_dzienne6_spolkats;


--
-- TOC entry 2308 (class 1259 OID 17184)
-- Name: ix_dzienne_dpolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_dzienne_dpolkats ON public.at_dzienne0 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_dzienne0 CLUSTER ON ix_dzienne_dpolkats;


--
-- TOC entry 2326 (class 1259 OID 17208)
-- Name: ix_intra15m2_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_intra15m2_spolkats ON public.at_intra15m2 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_intra15m2 CLUSTER ON ix_intra15m2_spolkats;


--
-- TOC entry 2329 (class 1259 OID 17222)
-- Name: ix_intra1m2_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_intra1m2_spolkats ON public.at_intra1m2 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_intra1m2 CLUSTER ON ix_intra1m2_spolkats;


--
-- TOC entry 2332 (class 1259 OID 17235)
-- Name: ix_intra5m2_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_intra5m2_spolkats ON public.at_intra5m2 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_intra5m2 CLUSTER ON ix_intra5m2_spolkats;


--
-- TOC entry 2335 (class 1259 OID 17247)
-- Name: ix_intra60m2_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_intra60m2_spolkats ON public.at_intra60m2 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_intra60m2 CLUSTER ON ix_intra60m2_spolkats;


--
-- TOC entry 2412 (class 1259 OID 18711)
-- Name: ix_mies0_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_mies0_spolkats ON public.at_mies0 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_mies0 CLUSTER ON ix_mies0_spolkats;


--
-- TOC entry 2338 (class 1259 OID 17259)
-- Name: ix_mies1_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_mies1_spolkats ON public.at_mies1 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_mies1 CLUSTER ON ix_mies1_spolkats;


--
-- TOC entry 2341 (class 1259 OID 17263)
-- Name: ix_mies2_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_mies2_spolkats ON public.at_mies2 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_mies2 CLUSTER ON ix_mies2_spolkats;


--
-- TOC entry 2344 (class 1259 OID 17266)
-- Name: ix_mies4_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_mies4_spolkats ON public.at_mies4 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_mies4 CLUSTER ON ix_mies4_spolkats;


--
-- TOC entry 2347 (class 1259 OID 17269)
-- Name: ix_mies5_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_mies5_spolkats ON public.at_mies5 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_mies5 CLUSTER ON ix_mies5_spolkats;


--
-- TOC entry 2415 (class 1259 OID 18717)
-- Name: ix_mies6_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_mies6_spolkats ON public.at_mies6 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_mies6 CLUSTER ON ix_mies6_spolkats;


--
-- TOC entry 2376 (class 1259 OID 17301)
-- Name: ix_mp_dzienne30m2_idspolki_ts; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_mp_dzienne30m2_idspolki_ts ON public.mp_dzienne30m2 USING btree (fk_id_spolki, ts);

ALTER TABLE public.mp_dzienne30m2 CLUSTER ON ix_mp_dzienne30m2_idspolki_ts;


--
-- TOC entry 2379 (class 1259 OID 17304)
-- Name: ix_mp_dzienne30m2tpo_fkmp; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_mp_dzienne30m2tpo_fkmp ON public.mp_dzienne30m2tpo USING btree (fk_id_mp);


--
-- TOC entry 2382 (class 1259 OID 17307)
-- Name: ix_slog_system_stock_status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_slog_system_stock_status ON public.s_log USING btree (fk_system, fk_stock, status);

ALTER TABLE public.s_log CLUSTER ON ix_slog_system_stock_status;


--
-- TOC entry 2385 (class 1259 OID 17310)
-- Name: ix_sparams_systemstock; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_sparams_systemstock ON public.s_params USING btree (fk_system, fk_stock);


--
-- TOC entry 2406 (class 1259 OID 17969)
-- Name: ix_spolki2_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ix_spolki2_id ON public.at_spolki2 USING btree (id);

ALTER TABLE public.at_spolki2 CLUSTER ON ix_spolki2_id;


--
-- TOC entry 2358 (class 1259 OID 17279)
-- Name: ix_spolki_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ix_spolki_id ON public.at_spolki_disabled USING btree (id);

ALTER TABLE public.at_spolki_disabled CLUSTER ON ix_spolki_id;


--
-- TOC entry 2388 (class 1259 OID 17313)
-- Name: ix_ssignals_systemstock; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_ssignals_systemstock ON public.s_signals USING btree (fk_system, fk_stock);

ALTER TABLE public.s_signals CLUSTER ON ix_ssignals_systemstock;


--
-- TOC entry 2393 (class 1259 OID 17318)
-- Name: ix_ssystemstock_system_stock; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_ssystemstock_system_stock ON public.s_systemstocks USING btree (fk_system, fk_stock);

ALTER TABLE public.s_systemstocks CLUSTER ON ix_ssystemstock_system_stock;


--
-- TOC entry 2396 (class 1259 OID 17321)
-- Name: ix_svars_systemstockparam; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_svars_systemstockparam ON public.s_vars USING btree (fk_system, fk_stock, paramname);

ALTER TABLE public.s_vars CLUSTER ON ix_svars_systemstockparam;


--
-- TOC entry 2403 (class 1259 OID 17830)
-- Name: ix_test_dzienne_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_test_dzienne_spolkats ON public.test_dzienne USING btree (fk_id_spolki, ts);

ALTER TABLE public.test_dzienne CLUSTER ON ix_test_dzienne_spolkats;


--
-- TOC entry 2359 (class 1259 OID 17282)
-- Name: ix_ticks_fkidspolki_ts; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_ticks_fkidspolki_ts ON public.at_ticks USING btree (fk_id_spolki, ts);


--
-- TOC entry 2409 (class 1259 OID 18702)
-- Name: ix_tyg0_spokats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_tyg0_spokats ON public.at_tyg0 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_tyg0 CLUSTER ON ix_tyg0_spokats;


--
-- TOC entry 2362 (class 1259 OID 17286)
-- Name: ix_tyg1_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_tyg1_spolkats ON public.at_tyg1 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_tyg1 CLUSTER ON ix_tyg1_spolkats;


--
-- TOC entry 2365 (class 1259 OID 17290)
-- Name: ix_tyg2_spolkats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_tyg2_spolkats ON public.at_tyg2 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_tyg2 CLUSTER ON ix_tyg2_spolkats;


--
-- TOC entry 2368 (class 1259 OID 17293)
-- Name: ix_tyg4_spokats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_tyg4_spokats ON public.at_tyg4 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_tyg4 CLUSTER ON ix_tyg4_spokats;


--
-- TOC entry 2371 (class 1259 OID 17296)
-- Name: ix_tyg5_spokats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_tyg5_spokats ON public.at_tyg5 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_tyg5 CLUSTER ON ix_tyg5_spokats;


--
-- TOC entry 2418 (class 1259 OID 18723)
-- Name: ix_tyg6_spokats; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_tyg6_spokats ON public.at_tyg6 USING btree (fk_id_spolki, ts);

ALTER TABLE public.at_tyg6 CLUSTER ON ix_tyg6_spokats;


-- Completed on 2021-12-07 10:32:01

--
-- PostgreSQL database dump complete
--

