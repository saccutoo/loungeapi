﻿DROP TABLE UTIL_IPORTAL.SRV_MAP_USER_SURVEY_SECTIONS CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.SRV_MAP_USER_SURVEY_SECTIONS
(
  ID               NUMBER,
  USERID           VARCHAR2(50 BYTE),
  SURVEYSECTIONID  NUMBER,
  COMPLETEDON      VARCHAR2(50 BYTE),
  COMPLETEPERCENT  NUMBER,
  CREATEBY         VARCHAR2(150 BYTE),
  CREATEDATE       DATE
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
