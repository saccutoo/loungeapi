DROP PACKAGE UTIL_IPORTAL.PKG_VUC_CMS_ISSUE_BATCH;

CREATE OR REPLACE PACKAGE UTIL_IPORTAL.PKG_VUC_CMS_ISSUE_BATCH IS 
/**
    Package bao gom cac thu tuc:
    - Tao dot phat hanh
    - Get danh sach dot phat hanh
    - Cap nhat thong tin dot phat hanh 
**/

    TYPE ref_cur IS REF CURSOR;
    EXCEPTION_ERROR constant varchar2(5) := '99'; --Loi Exception
    SUCCESS constant varchar2(5) := '00'; --Thuc hien thanh cong
    FAILE_CONFLICT constant varchar2(5) := '02'; --Loi du lieu da ton tai
    UPDATE_NOT_PERMISSION constant varchar2(5) := '03'; --Khong duoc phep thay doi du lieu
    DATA_NOT_FOUND constant varchar2(5) := '04'; --Khong tim thay du lieu  
    NOT_ENOUGH_ITEMS constant varchar2(5) := '05'; --So luong yeu cau vuot qua gioi han cho phep
    DATA_LENGTH_OVER_LIMIT constant varchar2(5) := '07'; --Du lieu vuot qua do dai cho phep
    
    
    /*===========Ham tao dot phat hanh================  */
    PROCEDURE "PRC_CREATE_ISSUE_BATCH"
    (
        o_resp_cursor OUT ref_cur,
        i_name IN varchar2,
        i_description IN varchar2,
        i_issue_date IN date, --dd/MM/yyyy hh:mm:ss
        i_expire_date IN date, --dd/MM/yyyy hh:mm:ss
        i_create_by IN varchar2);

                
    /*===========Ham cap nhat, chinh sua dot phat hanh================  */
    PROCEDURE "PRC_UPDATE_ISSUE_BATCH"
    (
        o_resp_cursor OUT ref_cur,
        i_issue_batch_id varchar2,
        i_name IN varchar2,
        i_description IN varchar2,
        i_issue_date IN date, --dd/MM/yyyy hh:mm:ss
        i_expire_date IN date, --dd/MM/yyyy hh:mm:ss
        i_modify_by IN varchar2);
        
    
    /*===========Ham filter dot phat hanh================  */
    PROCEDURE "PRC_FILTER_ISSUE_BATCH"
    (   i_page_size in number,
        i_page_index in number,
        i_search_text in varchar2,
        o_issue_batch_cursor OUT ref_cur);
        
    
    /*===========Ham get danh sach dot phat hanh ================  */
    PROCEDURE "PRC_GET_ALL_ISS_BATCHES"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2);
        
        
    /*===========Ham get chi tiet issue batch theo id================  */
    PROCEDURE "PRC_GET_ISSUE_BATCH_BY_ID"
    (
        o_resp_cursor OUT ref_cur,
        i_issue_batch_id IN varchar2);

        
END PKG_VUC_CMS_ISSUE_BATCH;
/
