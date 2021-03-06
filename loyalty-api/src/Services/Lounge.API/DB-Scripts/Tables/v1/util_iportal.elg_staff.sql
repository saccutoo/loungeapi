DROP TABLE UTIL_IPORTAL.ELG_STAFF CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.ELG_STAFF
(
  ID                NUMBER,
  USERNAME          VARCHAR2(150 BYTE),
  FULLNAME          VARCHAR2(250 BYTE),
  PERSONALID        VARCHAR2(150 BYTE),
  PHONENUM          VARCHAR2(50 BYTE),
  EMAIL             VARCHAR2(150 BYTE),
  GENDER            VARCHAR2(50 BYTE),
  PASSWORD          VARCHAR2(300 BYTE),
  PASSWORDSALT      VARCHAR2(300 BYTE),
  FAILEDPASSCOUNT   NUMBER,
  LASTLOGINDATE     DATE,
  LASTLOGOUTDATE    DATE,
  LOCKTIME          DATE,
  LOUNGEID          NUMBER,
  STATUS            VARCHAR2(100 BYTE),
  ORDERVIEW         NUMBER,
  CREATEBY          VARCHAR2(150 BYTE),
  CREATEDATE        DATE,
  LASTMODIFIEDBY    VARCHAR2(150 BYTE),
  LASTMODIFIEDDATE  DATE,
  JWT               VARCHAR2(100 BYTE),
  REFRESHTOKEN      VARCHAR2(100 BYTE)
)
TABLESPACE PORTAL
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
MONITORING;
