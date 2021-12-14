DROP PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CUSTOMER;

CREATE OR REPLACE PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CUSTOMER IS
  PROCEDURE GET_BY_TRANSACTION_ID(pPageSize      IN INT,
                                       pPageIndex     IN INT,
                                       pTransactionId IN NUMBER,
                                       OUT_CUR        OUT REF_CUR) IS
    p_TotalRecord INT;
    p_Divide      INT;
    p_TotalPage   INT;
  BEGIN
    SELECT COUNT(*)
      INTO p_TotalRecord
      FROM VUC_CUSTOMER t
     WHERE t.uploadtransactionid = pTransactionId;
    p_TotalPage := p_TotalRecord / pPageSize;
    p_Divide    := p_TotalPage * pPageSize;
    IF (p_TotalRecord - p_Divide > 0) THEN
      BEGIN
        p_TotalPage := p_TotalPage + 1;
      END;
    END IF;
    OPEN OUT_CUR FOR
      SELECT r.*
        FROM (SELECT *
                FROM (SELECT t.*,
                             p_TotalPage TotalPage,
                             p_TotalRecord TotalRecord,
                             RANK() OVER(ORDER BY ID DESC) r__
                        FROM VUC_CUSTOMER t
                       WHERE t.uploadtransactionid = pTransactionId
                       order by t.id desc)
               WHERE r__ > (pPageIndex - 1) * pPageSize
                 AND r__ <= pPageIndex * pPageSize) r;
  END;
END;

/
