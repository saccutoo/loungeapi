﻿DROP TABLE UTIL_IPORTAL.SRV_INPUT_TYPE CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.SRV_INPUT_TYPE
(
  ID                NUMBER,
  NAME              VARCHAR2(300 BYTE),
  DESCRIPTION       VARCHAR2(2000 BYTE),
  INPUTCONTROL      VARCHAR2(50 BYTE),
  STATUS            VARCHAR2(150 BYTE),
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
