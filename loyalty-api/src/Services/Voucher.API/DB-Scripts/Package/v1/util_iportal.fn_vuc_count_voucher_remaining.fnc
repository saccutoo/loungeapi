DROP FUNCTION UTIL_IPORTAL.FN_VUC_COUNT_VOUCHER_REMAINING;

CREATE OR REPLACE FUNCTION UTIL_IPORTAL."FN_VUC_COUNT_VOUCHER_REMAINING" (p_voucher_id  IN  varchar2)
  RETURN number
IS
    v_total_voucher_publish number;
    v_num_of_mapping number := 0;
BEGIN
    
    -- Tong so luong vouchers da publish
    select count(*) into v_total_voucher_publish from vuc_published_voucher vpv, 
        vuc_voucher_definition vvd  where vpv.voucherid = vvd.id and vvd.id = p_voucher_id;
    
    -- Tong so luong voucher da duoc gan
    select nvl(sum(maxusedquantitypercust), 0) into v_num_of_mapping from vuc_map_voucher_cust 
        where publishvoucherid in (
        select vpv.id from vuc_published_voucher vpv, vuc_voucher_definition vvd 
            where vpv.voucherid = vvd.id and vvd.id = p_voucher_id 
        ) and status in ('USED', 'UNUSED');
    
  return (v_total_voucher_publish - v_num_of_mapping);
  EXCEPTION WHEN OTHERS THEN BEGIN
    ROLLBACK;
    return 0;
  END;
END FN_VUC_COUNT_VOUCHER_REMAINING;
/
