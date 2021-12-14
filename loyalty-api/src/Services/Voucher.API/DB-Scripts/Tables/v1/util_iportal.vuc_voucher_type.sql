﻿DROP TABLE UTIL_IPORTAL.VUC_VOUCHER_TYPE CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.VUC_VOUCHER_TYPE
(
  ID                NUMBER,
  NAME              VARCHAR2(250 BYTE),
  VALUETYPE         VARCHAR2(30 BYTE),
  ORDERVIEW         VARCHAR2(12 BYTE),
  CREATEDBY         VARCHAR2(50 BYTE),
  CREATEDDATE       DATE,
  LASTMODIFIEDDATE  DATE,
  LASTMODIFIEDBY    VARCHAR2(50 BYTE)
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
