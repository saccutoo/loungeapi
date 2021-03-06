DROP TABLE UTIL_IPORTAL.ELG_CUST_CLASS_CONDITION CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.ELG_CUST_CLASS_CONDITION
(
  ID                NUMBER,
  CUSTCLASSID       NUMBER,
  DESCRIPTION       VARCHAR2(2000 BYTE),
  CONDITIONUSE      VARCHAR2(2000 BYTE),
  MAXPEOPLEGOWITH   NUMBER,
  EXPIREDATE        DATE,
  STATUS            VARCHAR2(100 BYTE),
  ORDERVIEW         NUMBER,
  CREATEBY          VARCHAR2(150 BYTE),
  CREATEDATE        DATE,
  LASTMODIFIEDBY    VARCHAR2(150 BYTE),
  LASTMODIFIEDDATE  DATE,
  MAXCHECKIN        NUMBER,
  MAXOFUSE          NUMBER
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
