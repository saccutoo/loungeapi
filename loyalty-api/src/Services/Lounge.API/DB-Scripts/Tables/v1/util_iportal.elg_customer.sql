DROP TABLE UTIL_IPORTAL.ELG_CUSTOMER CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.ELG_CUSTOMER
(
  ID                 NUMBER,
  CUSTID             VARCHAR2(100 BYTE),
  FULLNAME           VARCHAR2(200 BYTE),
  CLASSID            NUMBER,
  UPLOADID           NUMBER,
  CUSTTYPEID         NUMBER,
  EXPIREDATE         DATE,
  POSID              NUMBER,
  PHONENUM           VARCHAR2(50 BYTE),
  BIRTHDAY           DATE,
  GENDER             VARCHAR2(50 BYTE),
  REPRESENTUSERNAME  VARCHAR2(150 BYTE),
  REPRESENTUSERID    VARCHAR2(50 BYTE),
  EMAIL              VARCHAR2(150 BYTE),
  STATUS             VARCHAR2(100 BYTE),
  ORDERVIEW          NUMBER,
  CREATEBY           VARCHAR2(150 BYTE),
  CREATEDATE         DATE,
  LASTMODIFIEDBY     VARCHAR2(150 BYTE),
  LASTMODIFIEDDATE   DATE,
  CIFNUM             VARCHAR2(150 BYTE)
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
