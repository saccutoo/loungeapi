DROP PACKAGE UTIL_IPORTAL.PKG_VUC_CMS_VOUCHER;

CREATE OR REPLACE PACKAGE UTIL_IPORTAL.PKG_VUC_CMS_VOUCHER IS 
/**
    Package bao gom cac thu tuc:
    - Them moi voucher
    - Cap nhat thong tin voucher
    - Get danh sach voucher [PENDDING]
**/

    TYPE ref_cur IS REF CURSOR;
    EXCEPTION_ERROR constant varchar2(5) := '99'; --Loi Exception
    SUCCESS constant varchar2(5) := '00'; --Thuc hien thanh cong
    FAILE_CONFLICT constant varchar2(5) := '02'; --Loi du lieu da ton tai
    UPDATE_NOT_PERMISSION constant varchar2(5) := '03'; --Khong duoc phep thay doi du lieu  
    DATA_NOT_FOUND constant varchar2(5) := '04'; --Khong tim thay du lieu  
            
    
    /*===========Ham get danh sach kenh su dung cho voucher================  */
    PROCEDURE "PRC_GET_LIST_VOUCHER_CHANNELS"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2); 
        
        
    /*===========Ham get danh sach loai voucher================  */
    PROCEDURE "PRC_GET_LIST_VOUCHER_TYPE"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2);
        
        
    /*===========Ham get danh sach trang thai================  */
    PROCEDURE "PRC_GET_LIST_STATUS"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2);
                
        
    /*===========Ham tao moi voucher================  */
    PROCEDURE "PRC_CREATE_VOUCHER"
    (
        o_resp_cursor OUT ref_cur,
        i_name IN varchar2,
        i_description_vn IN varchar2,
        i_description_en IN varchar2,
        i_issue_batch_id IN number,
        i_channel_id IN number, --Kenh ap dung
        i_voucher_type_id IN varchar2, -- Loai voucher: VOUCHER / Coupon
        i_effective_date IN date, --Ngay hieu luc voucher
        i_expire_date IN date, --Ngay het han
        i_max_used_quantity IN number, --So luong su dung toi da
        i_issuequantity IN number, --So luong voucher phat hanh
        --i_max_used_quantity_per_cust IN number, -- So luong su dung toi da / KH        
        i_voucher_theme in clob, 
        i_create_by IN varchar2);
        
   
    /*===========Ham cap nhat thong tin voucher================  */
    PROCEDURE "PRC_UPDATE_VOUCHER"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN varchar2,
        i_name IN varchar2,
        i_description_vn IN varchar2,
        i_description_en IN varchar2,
        i_batch_id IN number,
        i_channel_id IN number, --Kenh ap dung
        i_effective_date IN date, --Ngay hieu luc voucher
        i_expire_date IN date, --Ngay het han
        i_voucher_type_id IN number, -- Loai voucher: VOUCHER / Coupon
        i_max_used_quantity IN number, --So luong su dung toi da
        i_issuequantity IN number, --So luong voucher phat hanh
        --i_max_used_quantity_per_cust IN number, -- So luong su dung toi da / KH
        i_voucher_theme in clob, 
        i_modify_by IN varchar2);      
           
        
    /*===========Ham filter danh sach voucher================  */
    PROCEDURE "PRC_FILTER_VOUCHERS"
    (   i_page_size in number,
        i_page_index in number,
        i_search_text in varchar2, -- Tim kiem theo text
        i_search_iss_batch_id in number, -- Loc theo batchId
        i_search_voucher_type_id in number, -- Loc theo voucherTypeId
        i_search_channel_id in number, -- Loc theo channelId
        i_search_status_id in number, -- Loc theo status
        o_voucher_cursor OUT ref_cur);
        
    
    /*===========Ham get danh sach voucher================  */
    PROCEDURE "PRC_GET_ALL_VOUCHERS"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2);
          
    /*========Ham get danh sach voucher cho man hinhg mapping==== */
    PROCEDURE "PRC_GET_VOUCHERS_FOR_MAPPING"
    (
        o_data_cursor OUT ref_cur,
        i_channel_id IN number,
        i_issue_batch_id IN number,
        i_voucher_type_id IN number);
            
    /*===========Ham chi tiet voucher================  */
    PROCEDURE "PRC_GET_VOUCHER_DETAILS"
    (
        o_data_cursor OUT ref_cur,
        i_voucher_id IN number);
               
        
    /*===========Ham them xoa dieu kien Amount cho voucher================  */
    PROCEDURE "PRC_REMOVE_VOUCHER_CONDITIONS"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number);
        
        
   /*===========Ham them dieu kien Amount cho voucher================  */
    PROCEDURE "PRC_ADD_VOUCHER_CONDITION"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number,
        i_amount_valuation IN varchar2, -- Gia tri voucher tinh theo so tien
        i_percent_valuation IN varchar2, -- Gia tri voucher tinh theo %
        i_min_trans_amount IN varchar2, -- So tien giao dich thap nhat duoc ap dung
        i_max_trans_amount IN varchar2, -- So tien giao dich cao nhat duoc ap dung
        i_max_amount_coupon IN varchar2, -- So tien toi da duoc nhan doi voi TH voucherType = coupon
        i_create_by IN varchar2);
        
    
    /*===========Ham get danh sach voucher conditions================  */
    PROCEDURE "PRC_GET_VOUCHER_CONDITIONS"
    (
        o_data_cursor OUT ref_cur,
        i_voucher_id IN number);
        
    
    /*===========Ham duyet voucher================  */
    PROCEDURE "PRC_APPROVE_VOUCHER"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, 
        i_issue_quantity IN number,
        i_listPin IN VARCHAR2,
        i_create_by IN varchar2);
      
    /*===========Ham tu choi duyet voucher================  */
    PROCEDURE "PRC_REJECT_VOUCHER"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, 
        i_create_by IN varchar2);
    
    /*===========Ham huy bo voucher chua duoc gan================  */
    PROCEDURE "PRC_CANCEL_VOUCHER"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, 
        i_create_by IN varchar2);
        
            
    /*===========Ham generate PIN voucher================*/
    FUNCTION FN_VOUCHER_GENERATE_PIN(
        i_publish_id in varchar2)
     RETURN varchar2;
     
                              
END PKG_VUC_CMS_VOUCHER;
/
