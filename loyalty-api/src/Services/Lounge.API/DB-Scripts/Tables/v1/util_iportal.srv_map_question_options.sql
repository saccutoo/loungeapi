DROP TABLE UTIL_IPORTAL.SRV_MAP_QUESTION_OPTIONS CASCADE CONSTRAINTS;

CREATE TABLE UTIL_IPORTAL.SRV_MAP_QUESTION_OPTIONS
(
  ID             NUMBER,
  QUESTIONID     NUMBER,
  OPTIONGROUPID  NUMBER
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
