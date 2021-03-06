ALTER TABLE UTIL_IPORTAL.VUC_CUSTOMER
 DROP PRIMARY KEY CASCADE;

DROP TABLE UTIL_IPORTAL.VUC_CUSTOMER CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.VUC_CUSTOMER
(
  ID                   NUMBER                   NOT NULL,
  UPLOADTRANSACTIONID  NUMBER,
  CIFNUMBER            VARCHAR2(50 BYTE),
  NAME                 VARCHAR2(200 BYTE),
  CLASS                VARCHAR2(100 BYTE),
  STATUS               VARCHAR2(50 BYTE)
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


CREATE UNIQUE INDEX UTIL_IPORTAL.ID ON UTIL_IPORTAL.VUC_CUSTOMER
(ID)
LOGGING
TABLESPACE PORTAL
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           );

ALTER TABLE UTIL_IPORTAL.VUC_CUSTOMER ADD (
  CONSTRAINT ID
  PRIMARY KEY
  (ID)
  USING INDEX UTIL_IPORTAL.ID
  ENABLE VALIDATE);
