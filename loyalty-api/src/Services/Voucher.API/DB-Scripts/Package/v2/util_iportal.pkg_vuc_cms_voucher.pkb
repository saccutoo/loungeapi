DROP PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CMS_VOUCHER;

CREATE OR REPLACE PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CMS_VOUCHER IS
/**
    Package bao gom cac thu tuc:
    - Them moi voucher
    - Cap nhat thong tin voucher
    - Get danh sach voucher [PENDDING]
**/
    
    
    /*===========Ham get danh sach kenh su dung cho voucher================  */
    PROCEDURE "PRC_GET_LIST_VOUCHER_CHANNELS"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2) IS

        v_search_text_no_accents varchar2(150);
    BEGIN
        
        v_search_text_no_accents := fn_remove_accents(i_text_search);

        open o_data_cursor for
            select id, name, description, channelcode from vuc_applied_channel
                where replace(lower(fn_remove_accents(name)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    or replace(lower(fn_remove_accents(description)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%')
                    order by id desc;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham get danh sach loai voucher================  */
    PROCEDURE "PRC_GET_LIST_VOUCHER_TYPE"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2) IS

        v_search_text_no_accents varchar2(150);
    BEGIN
        
        v_search_text_no_accents := fn_remove_accents(i_text_search);

        open o_data_cursor for
            select id, name, valueType from vuc_voucher_type
                where replace(lower(fn_remove_accents(name)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    order by orderview desc;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham get danh sach trang thai================  */
    PROCEDURE "PRC_GET_LIST_STATUS"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2) IS

        v_search_text_no_accents varchar2(150);
    BEGIN
        
        v_search_text_no_accents := fn_remove_accents(i_text_search);

        open o_data_cursor for
            select id, name, description from vuc_status
                where replace(lower(fn_remove_accents(name)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    or replace(lower(fn_remove_accents(description)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    order by orderview desc;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
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
        i_create_by IN varchar2) IS

        /*Voucher definition*/
        v_voucher_id varchar2(30);
        v_count_name number;
        v_issue_batch_effictive_date date;
        v_issue_batch_expire_date date;
            
    BEGIN
        
        -- Get thong tin ngay hieu luc va ngay het han tu ISSUE Batch
        begin
            select expiredate, issuedate into v_issue_batch_expire_date, v_issue_batch_effictive_date
                from vuc_issue_batch where id = i_issue_batch_id;
        exception when no_data_found then
            v_issue_batch_expire_date := i_expire_date;
        end;
        
        -- Kiem tra ma voucher code da ton tai hay chua
        select count(*) into v_count_name from vuc_voucher_definition
            where name = i_name;
        
        if(i_effective_date < v_issue_batch_effictive_date or i_effective_date > v_issue_batch_expire_date
            or i_expire_date < v_issue_batch_effictive_date or i_expire_date > v_issue_batch_expire_date) then
            open o_resp_cursor for
                select FAILE_CONFLICT as STATUS_CODE, '' as ID, 
                    ('Ngày hiệu lực và ngày hết hạn của voucher phải nằm trong khoảng ' 
                        || to_char(v_issue_batch_effictive_date, 'dd/MM/rrrr hh24:mi:ss') 
                        || ' tới ngày ' || to_char(v_issue_batch_expire_date, 'dd/MM/rrrr hh24:mi:ss')
                    ) as NAME  from dual;
        elsif(v_count_name > 0) then
            --Voucher code da ton tai
            open o_resp_cursor for
                select FAILE_CONFLICT as STATUS_CODE, '' as ID, 
                    'Mã này đã tồn tại, vui lòng kiểm tra lại.' as NAME  from dual;
        else
            -- Insert thong tin voucher
            v_voucher_id := SEQ_VUC_VOUCHER.nextval;        
            insert into vuc_voucher_definition(id, name, descriptionvn, descriptionen, vouchertypeid, 
                effectivedate, expiredate, issuebatchid, channelid, maxusedquantity, issuequantity,
                status, createby, createdate, lastmodifyby, lastmodifydate)
                values(v_voucher_id, i_name, i_description_vn, i_description_en, i_voucher_type_id, i_effective_date, i_expire_date,
                i_issue_batch_id, i_channel_id, i_max_used_quantity, i_issuequantity,
                'WAITING_APPROVE', i_create_by, to_date(sysdate,'dd/MM/rrrr hh24:mi:ss'), '', '');
            
            commit;
        
            open o_resp_cursor for
                select SUCCESS as STATUS_CODE, v_voucher_id as ID, 'Success' as NAME  from dual;
        end if;
        
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
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
        i_modify_by IN varchar2) IS

        /*Voucher definition*/
        v_current_status varchar2(50);
        v_count_name number;
        v_issue_batch_effictive_date date;
        v_issue_batch_expire_date date;
            
    BEGIN
        
        -- Get thong tin ngay hieu luc va ngay het han tu ISSUE Batch
        begin 
            select expiredate, issuedate into v_issue_batch_expire_date, v_issue_batch_effictive_date
                from vuc_issue_batch where id = (
                    select issuebatchid from vuc_voucher_definition where id = i_voucher_id);
        exception when no_data_found then
            v_issue_batch_expire_date := i_expire_date;
        end;
        
        -- Get va kiem tra trang thai hien tai cua voucher
        select status into v_current_status from vuc_voucher_definition
            where id = i_voucher_id;
            
        select count(*) into v_count_name from vuc_voucher_definition
            where name = i_name and id <> i_voucher_id;
        
         if(i_effective_date < v_issue_batch_effictive_date or i_effective_date > v_issue_batch_expire_date
            or i_expire_date < v_issue_batch_effictive_date or i_expire_date > v_issue_batch_expire_date) then
            open o_resp_cursor for
                select FAILE_CONFLICT as STATUS_CODE, '' as ID, 
                    ('Ngày hiệu lực và ngày hết hạn của voucher phải nằm trong khoảng ' 
                        || to_char(v_issue_batch_effictive_date, 'dd/MM/rrrr') 
                        || ' tới ngày ' || to_char(v_issue_batch_expire_date, 'dd/MM/rrrr')
                    ) as NAME  from dual;
        elsif(v_current_status = 'APPROVED') then
            --Voucher code da ton tai
            open o_resp_cursor for
                select UPDATE_NOT_PERMISSION as STATUS_CODE, '' as ID, 
                    'Trang thai hien tai khong duoc phep cap nhat du lieu' as NAME  from dual;
        elsif(v_count_name > 0) then
            --Voucher code da ton tai
            open o_resp_cursor for
                select FAILE_CONFLICT as STATUS_CODE, '' as ID, 
                    'Mã này đã tồn tại, vui lòng kiểm tra lại' as NAME  from dual;
        else 
            -- Cap nhat thong tin voucher
            update vuc_voucher_definition set name = i_name, descriptionvn = i_description_vn, 
                descriptionen = i_description_en, vouchertypeid = i_voucher_type_id, 
                effectivedate = i_effective_date, expiredate = i_expire_date, issuebatchid = i_batch_id, 
                issuequantity = i_issuequantity,
                channelid = i_channel_id, maxusedquantity = i_max_used_quantity,-- maxusedquantitypercust = i_max_used_quantity_per_cust,
                status = 'WAITING_APPROVE', lastmodifyby = i_modify_by, 
                lastmodifydate = to_date(sysdate,'dd/MM/rrrr hh24:mi:ss')
                    where id = i_voucher_id;
            commit;
        
            open o_resp_cursor for
                select SUCCESS as STATUS_CODE, i_voucher_id as ID, 'Success' as NAME  from dual;
        end if;
        
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham filter danh sach voucher================  */
    PROCEDURE "PRC_FILTER_VOUCHERS"
    (   i_page_size in number,
        i_page_index in number,
        i_search_text in varchar2, -- Tim kiem theo text
        i_search_iss_batch_id in number, -- Loc theo batchId
        i_search_voucher_type_id in number, -- Loc theo voucherTypeId
        i_search_channel_id in number, -- Loc theo channelId
        i_search_status_id in number, -- Loc theo status
        o_voucher_cursor OUT ref_cur) IS
        
        v_search_text_no_accents varchar2(150);
        v_total_record number;
        v_divide number;
        v_total_page number;
        v_page_size number;
      
    BEGIN
        
        v_search_text_no_accents := fn_remove_accents(i_search_text);
        select count(*) into v_total_record from vuc_voucher_definition 
            where (replace(lower(fn_remove_accents(name)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
            or replace(lower(fn_remove_accents(descriptionvn)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%')
            and vouchertypeid in (select id from vuc_voucher_type where id = i_search_voucher_type_id or i_search_voucher_type_id = 0)
            and issuebatchid in (select id from vuc_issue_batch where id = i_search_iss_batch_id or i_search_iss_batch_id = 0)
            and channelid in (select id from vuc_applied_channel where id = i_search_channel_id or i_search_channel_id = 0)
            and status in (select name from vuc_status where id = i_search_status_id or i_search_status_id = 0);
        
        --Neu i_page_size = 0 thi lay tat ca records
        if(i_page_size = 0) then
            v_page_size := v_total_record;
        else
            v_page_size := i_page_size;
        end if;
         
        v_total_page := TRUNC(v_total_record / v_page_size);
        v_divide := v_total_page * v_page_size;
        
        if (v_total_record - v_divide > 0) then
            v_total_page := v_total_page + 1;
        end if;
      
        open o_voucher_cursor for 
            select * from (
                select vvd.*, vac.name channelName,
                    v_total_record TotalRecord, v_total_page TotalPage, RANK () OVER (ORDER BY vvd.id DESC) r__ from vuc_voucher_definition vvd
                    left join vuc_applied_channel vac on vvd.channelid = vac.id
                    where (replace(lower(fn_remove_accents(vvd.name)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    or replace(lower(fn_remove_accents(vvd.descriptionvn)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%')
                    and vvd.vouchertypeid in (select id from vuc_voucher_type where id = i_search_voucher_type_id or i_search_voucher_type_id = 0)
                    and vvd.issuebatchid in (select id from vuc_issue_batch where id = i_search_iss_batch_id or i_search_iss_batch_id = 0)
                    and vvd.channelid in (select id from vuc_applied_channel where id = i_search_channel_id or i_search_channel_id = 0)
                    and vvd.status in (select name from vuc_status where id = i_search_status_id or i_search_status_id = 0)
            ) WHERE     r__ > (i_page_index-1) * i_page_size
                AND r__ <= i_page_index* i_page_size;
                
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_voucher_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham get danh sach voucher================  */
    PROCEDURE "PRC_GET_ALL_VOUCHERS"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2) IS

        v_search_text_no_accents varchar2(150);
    BEGIN
        
        v_search_text_no_accents := fn_remove_accents(i_text_search);

        open o_data_cursor for
            select * from vuc_voucher_definition
                where replace(lower(fn_remove_accents(name)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    or replace(lower(fn_remove_accents(descriptionvn)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%')
                    and status = 'APPROVED'
                    order by id desc;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*========Ham get danh sach voucher cho man hinhg mapping==== */
    PROCEDURE "PRC_GET_VOUCHERS_FOR_MAPPING"
    (
        o_data_cursor OUT ref_cur,
        i_channel_id IN number,
        i_issue_batch_id IN number,
        i_voucher_type_id IN number) IS

    BEGIN
        open o_data_cursor for
            select vd.*, fn_vuc_count_voucher_remaining(vd.id) as remainQuantity 
                from vuc_voucher_definition vd
                where vd.channelid in (select id from vuc_applied_channel where id = i_channel_id or i_channel_id = 0)
                    and vd.issuebatchid in (select id from vuc_issue_batch where id = i_issue_batch_id or i_issue_batch_id = 0)
                    and vd.vouchertypeid in (select id from vuc_voucher_type where id = i_voucher_type_id or i_voucher_type_id = 0)
                    and vd.status = 'APPROVED'
                    order by id desc;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham chi tiet voucher================  */
    PROCEDURE "PRC_GET_VOUCHER_DETAILS"
    (
        o_data_cursor OUT ref_cur,
        i_voucher_id IN number) IS

    BEGIN

        open o_data_cursor for
            select vd.*, fn_vuc_count_voucher_remaining(vd.id) as remainQuantity
             from vuc_voucher_definition vd where id = i_voucher_id;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham them xoa dieu kien Amount cho voucher================  */
    PROCEDURE "PRC_REMOVE_VOUCHER_CONDITIONS"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number) IS

    BEGIN
        
        delete from vuc_voucher_amt_conditions 
            where voucherid = i_voucher_id;               
        commit;

        open o_resp_cursor for
            select SUCCESS as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;
        
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
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
        i_create_by IN varchar2) IS

        v_amt_condition_id varchar2(30);
    BEGIN
        
        -- Them ban ghi dieu kien su dung voucher
        v_amt_condition_id := seq_vuc_voucher_condition.nextval;
        insert into vuc_voucher_amt_conditions(id, voucherid, amountvaluation, 
            mintransamount, maxtransamount, percentvaluation, maxamountcoupon)
            values(v_amt_condition_id, i_voucher_id, i_amount_valuation,
            i_min_trans_amount, i_max_trans_amount, i_percent_valuation, i_max_amount_coupon);                
        commit;

        open o_resp_cursor for
            select SUCCESS as STATUS_CODE, v_amt_condition_id as ID, 'Success' as NAME  from dual;
        
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham get danh sach voucher conditions================  */
    PROCEDURE "PRC_GET_VOUCHER_CONDITIONS"
    (
        o_data_cursor OUT ref_cur,
        i_voucher_id IN number) IS

    BEGIN
    
        open o_data_cursor for
            select * from vuc_voucher_amt_conditions
                where voucherid = i_voucher_id order by id asc;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham duyet voucher================  */
    PROCEDURE "PRC_APPROVE_VOUCHER"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, 
        i_issue_quantity IN number,
        i_listPin IN VARCHAR2,
        i_create_by IN varchar2) IS

--        v_issue_quantity number := 0;
--        v_new_pin varchar2(12);
        v_publish_id varchar2(30);
        v_json                 VARCHAR2 (32767);
        v_ViTriKetThucObject   NUMBER;
        v_new_pin               VARCHAR2 (1000);
--        v_current_status varchar2(50);
    BEGIN
--        --Get so luong voucher target
--        begin
--            select status, issuequantity into v_current_status, v_issue_quantity
--                from vuc_voucher_definition where id = i_voucher_id;
--                
--        exception when no_data_found then
--            open o_resp_cursor for
--                select DATA_NOT_FOUND as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;
--        end;
--      
--        --Kiem tra trang thai voucher hien tai
--        if(v_current_status = 'WAITING_APPROVE') then 
--            
--        else
--            open o_resp_cursor for
--                select UPDATE_NOT_PERMISSION as STATUS_CODE, '' as ID, 
--                    'Trang thai hien tai khong duoc phep thay doi' as NAME  from dual;
--        end if;   
        
        v_json := i_listPin;
        SELECT INSTR (v_json, '_') INTO v_ViTriKetThucObject FROM DUAL;
        DBMS_OUTPUT.put_line ('v_ViTriKetThucObject = ' || v_ViTriKetThucObject);

        WHILE v_ViTriKetThucObject <> 0
        loop
            v_new_pin := SUBSTR (v_json, 0, v_ViTriKetThucObject - 1);

--                v_new_pin := fn_voucher_generate_pin(i_voucher_id);
            v_publish_id := seq_vuc_publish_voucher.nextval;
            insert into vuc_published_voucher(id, pinnum, status, voucherid, countused,
            createdate, createby)
                values(v_publish_id, v_new_pin, 'UNUSED', i_voucher_id, 0,
                to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss'), i_create_by);
--            commit;
                
            v_json := SUBSTR (v_json, v_ViTriKetThucObject + 1, LENGTH (v_json));
            SELECT INSTR (v_json, '_') INTO v_ViTriKetThucObject FROM DUAL;

        end loop;
                
        update vuc_voucher_definition set status = 'APPROVED'
            where id = i_voucher_id;       
--        commit;
        
        --Cap nhat trang thai dot phat hanh 
        update vuc_issue_batch set status = 'APPROVED'
            where id = (select issuebatchid from vuc_voucher_definition where id = i_voucher_id);
--        commit;
        
        open o_resp_cursor for
            select SUCCESS as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;

    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
   
    /*===========Ham tu choi duyet voucher================  */
    PROCEDURE "PRC_REJECT_VOUCHER"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, 
        i_create_by IN varchar2) IS
        
        v_current_status varchar2(30);
    BEGIN
        select status into v_current_status from vuc_voucher_definition
            where id = i_voucher_id;
        
        if(v_current_status <> 'APPROVED') then
            update vuc_voucher_definition set status = 'REJECTED', 
                lastmodifyby = i_create_by, lastmodifydate = to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss')
                where id = i_voucher_id;
                
            open o_resp_cursor for
                select SUCCESS as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;
        else
            open o_resp_cursor for
                select UPDATE_NOT_PERMISSION as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;
        end if;           
        
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham huy bo voucher chua duoc gan================  */
    PROCEDURE "PRC_CANCEL_VOUCHER"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, 
        i_create_by IN varchar2) IS
        
        v_current_status varchar2(30);
        v_count_mapping number;
    BEGIN
        select status into v_current_status from vuc_voucher_definition
            where id = i_voucher_id;
        
        -- Dem so luong voucher da duoc gan    
        select count(*) into v_count_mapping from vuc_map_voucher_cust mvc, vuc_published_voucher vpv
            where vpv.voucherid = i_voucher_id and vpv.id = mvc.publishvoucherid;
        
        --Neu trang thai chua duoc duyet va so luong voucher duoc gan = 0 thi duoc phep huy bo
        if(v_current_status <> 'APPROVED' and v_count_mapping = 0) then
            update vuc_voucher_definition set status = 'CANCEL', 
                lastmodifyby = i_create_by, lastmodifydate = to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss')
                where id = i_voucher_id;
                
            open o_resp_cursor for
                select SUCCESS as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;
        else
            open o_resp_cursor for
                select UPDATE_NOT_PERMISSION as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;
        end if;           
        
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham generate PIN voucher================*/
    FUNCTION FN_VOUCHER_GENERATE_PIN(
        i_publish_id in varchar2)
     RETURN varchar2
     IS
      --v_count_available number;
      v_new_pin varchar2(12) := i_publish_id;
     BEGIN
        select new_pin into v_new_pin from (
           select dbms_random.string('X', 12) new_pin from dual)
           where new_pin not in (select pinnum from vuc_published_voucher);
                       
           return v_new_pin;
     EXCEPTION WHEN OTHERS THEN
          return '0';
     END;

     
       
END PKG_VUC_CMS_VOUCHER;
/
