DROP PACKAGE UTIL_IPORTAL.PKG_VUC_CUSTOMER;

CREATE OR REPLACE PACKAGE UTIL_IPORTAL.PKG_VUC_CUSTOMER IS
  TYPE ref_cur IS REF CURSOR;
  PROCEDURE GET_BY_TRANSACTION_ID(pPageSize      IN INT,
                                  pPageIndex     IN INT,
                                  pTransactionId NUMBER,
                                  OUT_CUR        OUT REF_CUR);
END;

/
