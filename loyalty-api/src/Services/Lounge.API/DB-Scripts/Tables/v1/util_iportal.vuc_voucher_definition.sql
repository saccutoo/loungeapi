DROP TABLE UTIL_IPORTAL.VUC_VOUCHER_DEFINITION CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.VUC_VOUCHER_DEFINITION
(
  ID               NUMBER,
  NAME             VARCHAR2(250 BYTE),
  DESCRIPTIONVN    CLOB,
  DESCRIPTIONEN    CLOB,
  EFFECTIVEDATE    DATE,
  EXPIREDATE       DATE,
  ISSUEBATCHID     NUMBER,
  CHANNELID        NUMBER,
  MAXUSEDQUANTITY  NUMBER,
  STATUS           VARCHAR2(50 BYTE),
  CREATEBY         VARCHAR2(50 BYTE),
  CREATEDATE       DATE,
  LASTMODIFYBY     VARCHAR2(50 BYTE),
  LASTMODIFYDATE   DATE,
  THEME            CLOB,
  VOUCHERTYPEID    NUMBER
)
LOB (DESCRIPTIONVN) STORE AS BASICFILE (
  TABLESPACE  PORTAL
  ENABLE      STORAGE IN ROW
  CHUNK       8192
  RETENTION
  NOCACHE
  LOGGING
      STORAGE    (
                  INITIAL          64K
                  NEXT             1M
                  MINEXTENTS       1
                  MAXEXTENTS       UNLIMITED
                  PCTINCREASE      0
                  BUFFER_POOL      DEFAULT
                 ))
LOB (DESCRIPTIONEN) STORE AS BASICFILE (
  TABLESPACE  PORTAL
  ENABLE      STORAGE IN ROW
  CHUNK       8192
  RETENTION
  NOCACHE
  LOGGING
      STORAGE    (
                  INITIAL          64K
                  NEXT             1M
                  MINEXTENTS       1
                  MAXEXTENTS       UNLIMITED
                  PCTINCREASE      0
                  BUFFER_POOL      DEFAULT
                 ))
LOB (THEME) STORE AS BASICFILE (
  TABLESPACE  PORTAL
  ENABLE      STORAGE IN ROW
  CHUNK       8192
  RETENTION
  NOCACHE
  LOGGING
      STORAGE    (
                  INITIAL          64K
                  NEXT             1M
                  MINEXTENTS       1
                  MAXEXTENTS       UNLIMITED
                  PCTINCREASE      0
                  BUFFER_POOL      DEFAULT
                 ))
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
