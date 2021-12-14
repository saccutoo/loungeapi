﻿DROP TABLE UTIL_IPORTAL.SRV_SURVEY_SECTIONS CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.SRV_SURVEY_SECTIONS
(
  ID                NUMBER,
  SURVEYHEADERID    NUMBER,
  NAME              VARCHAR2(300 BYTE),
  TITLE             VARCHAR2(300 BYTE),
  SUBHEADING        VARCHAR2(200 BYTE),
  REQUIREDYN        VARCHAR2(20 BYTE),
  EFFECTIVEDATE     DATE,
  EXPIREDATE        DATE,
  STATUS            VARCHAR2(100 BYTE),
  ORDERVIEW         NUMBER,
  CREATEBY          VARCHAR2(150 BYTE),
  CREATEDATE        DATE,
  LASTMODIFIEDBY    VARCHAR2(150 BYTE),
  LASTMODIFIEDDATE  DATE
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
