DROP FUNCTION UTIL_IPORTAL.FN_VUC_SCAN_VOUCHER_EXPIRED;

CREATE OR REPLACE FUNCTION UTIL_IPORTAL."FN_VUC_SCAN_VOUCHER_EXPIRED" (p_new_status  IN  varchar2)
  RETURN VARCHAR2
IS
  
BEGIN
  update vuc_voucher_definition set status = p_new_status where 
  --status = 'APPROVED' and 
  to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss') > to_date(expiredate, 'dd/MM/rrrr hh24:mi:ss');
  commit;  
  return '0';
  EXCEPTION WHEN OTHERS THEN BEGIN
    ROLLBACK;
    return '-1';
  END;
END FN_VUC_SCAN_VOUCHER_EXPIRED;
/
