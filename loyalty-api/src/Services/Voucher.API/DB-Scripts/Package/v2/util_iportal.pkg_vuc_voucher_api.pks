DROP PACKAGE UTIL_IPORTAL.PKG_VUC_VOUCHER_API;

CREATE OR REPLACE PACKAGE UTIL_IPORTAL.PKG_VUC_VOUCHER_API IS 
/**
    Package bao gom cac thu tuc:
    - Lay danh sach voucher cho KH Ebank
    - Cap nhat voucher sau khi su dung tren kenh ebank
    - 
**/
    TYPE ref_cur IS REF CURSOR;
    
    
    /*===========Ham get danh sach voucher cho ebank================  */
    PROCEDURE "PRC_API_GET_VOUCHER_EBANK"
    (
        --io_errcode IN OUT varchar2,
        o_voucher_cursor OUT REF_CUR,
        i_cif_num IN varchar2,
        i_channel IN varchar2,
        i_trans_type IN varchar2,
        i_trans_amount IN number);

    
    /*===========Ham kiem tra voucher co hop le hay khong====*/
    PROCEDURE "PRC_API_EBANK_CHECK_VOUCHER"
    (
        o_resp_cursor OUT REF_CUR,
        i_cif_num IN varchar2,
        i_channel IN varchar2,
        i_trans_type IN varchar2,
        i_trans_amount IN number,
        i_discount_amount IN number,
        i_pin_num IN varchar2); 
        
        
    /*===========Ham cap nhat trang thai voucher da su dung cho ebank====*/
    PROCEDURE "PRC_API_EBANK_UPDATE_VOUCHER"
    (
        o_resp_cursor OUT REF_CUR,
        i_pin_num IN varchar2,
        i_cif_num IN varchar2,
        i_channel IN varchar2,
        i_trans_type IN varchar2,
        i_trans_amount IN number,
        i_discount_amount IN number,
        i_trans_refno IN varchar2);
        
     
    /*===========Ham tinh so luong con lai cua voucher================*/
    FUNCTION FN_VOUCHER_REMAIN_QUALITY(
        i_voucher_id in varchar2)
     RETURN number;
     
           
END PKG_VUC_VOUCHER_API;
/
