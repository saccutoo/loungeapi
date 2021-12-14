DROP PACKAGE UTIL_IPORTAL.PKG_VUC_CMS_MAP_VOUCHER_CUST;

CREATE OR REPLACE PACKAGE UTIL_IPORTAL.PKG_VUC_CMS_MAP_VOUCHER_CUST IS 
 
    TYPE ref_cur IS REF CURSOR;
    EXCEPTION_ERROR constant varchar2(5) := '99'; --Loi Exception
    SUCCESS constant varchar2(5) := '00'; --Thuc hien thanh cong
    FAILE_CONFLICT constant varchar2(5) := '02'; --Loi du lieu da ton tai
    UPDATE_NOT_PERMISSION constant varchar2(5) := '03'; --Khong duoc phep thay doi du lieu
    DATA_NOT_FOUND constant varchar2(5) := '04'; --Khong tim thay du lieu  
    NOT_ENOUGH_ITEMS constant varchar2(5) := '05'; --So luong yeu cau vuot qua gioi han cho phep
    
    
    /*===========Ham get danh sach loai giao dich ap dung================  */
    PROCEDURE "PRC_GET_LIST_TRANS_TYPE"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2);
           
    
    /*===========Ham kiem tra so luong khach hang khi gan voucher================  */
    PROCEDURE "PRC_VALIDATE_VOUCHER_CUSTOMER"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, --Ma voucher
        i_trans_type IN varchar2, -- Loai giao dich
        i_num_of_voucher_target IN number -- So luong voucher yeu cau
        );
   

    /*===========Ham gan voucher cho KH EBank================  */
    PROCEDURE "PRC_MAP_VOUCHER_CUSTOMER_EBANK"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, --Ma voucher
        i_trans_type IN varchar2, -- Loai giao dich
        i_upload_id IN varchar2, -- Ma upload
        i_customer_id IN varchar2, -- So cif KH
        i_customer_name IN varchar2, -- Ten KH
        i_max_used_per_cust IN number, -- So lan su dung toi da
        i_modify_by IN varchar2);
        
        
        
    /*===========Ham gan voucher cho KH ELOUNGE================  */
    PROCEDURE "PRC_MAP_VOUCHER_CUSTOMER_ELG"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, --Ma voucher
        i_upload_id IN varchar2, -- Ma upload
        i_customer_id IN varchar2, -- So cif KH
        i_customer_name IN varchar2, -- Ten KH
        i_represent_name IN number, -- Ten nguoi dai dien
        i_modify_by IN varchar2);
  
        
    /*===========Ham get danh sach voucher - customer================  */
    PROCEDURE "PRC_FILTER_VOUCHER_MAPPING"
    (
        i_page_size in number,
        i_page_index in number,
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2, --Tu khoa tim kiem
        i_trans_type IN varchar2, -- Loai giao dich
        i_voucher_id IN number,
        i_channel_id IN number,
        i_issue_batch_id IN number,
        i_voucher_type_id IN number); -- Voucher Id
    
    
    /*===========Ham huy gan voucher cho KH EBank================  */
    PROCEDURE "PRC_CANCEL_VOUCHER_MAPPING"
    (
        o_resp_cursor OUT ref_cur,
        i_map_id IN number, --Ma mapping voucher
        i_modify_by IN varchar2); --Nguoi chinh sua
        
        
    /*===========Ham generate serial voucher================*/
    FUNCTION FN_VOUCHER_GENERATE_SERIAL(
        i_voucher_id in varchar2)
     RETURN varchar2;
     
     
END PKG_VUC_CMS_MAP_VOUCHER_CUST;
/
