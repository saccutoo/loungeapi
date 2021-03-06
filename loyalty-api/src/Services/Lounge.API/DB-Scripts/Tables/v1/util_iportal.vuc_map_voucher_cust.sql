DROP TABLE UTIL_IPORTAL.VUC_MAP_VOUCHER_CUST CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.VUC_MAP_VOUCHER_CUST
(
  ID                      NUMBER,
  PUBLISHVOUCHERID        NUMBER,
  CUSTOMERID              VARCHAR2(30 BYTE),
  CUSTOMERNAME            VARCHAR2(250 BYTE),
  UPLOADID                NUMBER,
  SERIALNUM               VARCHAR2(50 BYTE),
  TRANSTYPE               VARCHAR2(250 BYTE),
  CREATEDATE              DATE,
  CREATEBY                VARCHAR2(50 BYTE),
  LASTMODIFYDATE          DATE,
  LASTMODIFYBY            VARCHAR2(50 BYTE),
  STATUS                  VARCHAR2(50 BYTE),
  MAXUSEDQUANTITYPERCUST  NUMBER
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
