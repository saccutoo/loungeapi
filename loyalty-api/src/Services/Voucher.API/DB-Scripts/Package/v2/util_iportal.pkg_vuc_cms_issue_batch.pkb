DROP PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CMS_ISSUE_BATCH;

CREATE OR REPLACE PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CMS_ISSUE_BATCH IS
/**
    Package bao gom cac thu tuc:
    - Tao dot phat hanh
    - Get danh sach dot phat hanh
    - Cap nhat thong tin dot phat hanh 
**/



    /*===========Ham tao dot phat hanh================  */
    PROCEDURE "PRC_CREATE_ISSUE_BATCH"
    (
        o_resp_cursor OUT ref_cur,
        i_name IN varchar2,
        i_description IN varchar2,
        i_issue_date IN date, --dd/MM/yyyy hh:mm:ss
        i_expire_date IN date, --dd/MM/yyyy hh:mm:ss
        i_create_by IN varchar2) IS

        v_bath_id varchar2(30);
    BEGIN
        
        v_bath_id := seq_vuc_iss_batch.nextval;
        
        if(nvl(length(i_description), 0) <= 200) then
            insert into vuc_issue_batch(id, name, description, issuedate, expiredate, 
                status, createdate, createby, lastmodifydate, lastmodifyby)
                values(v_bath_id, i_name, i_description, i_issue_date, i_expire_date,
                'WAITING_APPROVE', to_date(sysdate,'dd/MM/rrrr hh24:mi:ss'), i_create_by, '', '');
            commit;

            open o_resp_cursor for
                    select SUCCESS as STATUS_CODE, v_bath_id as ID, 'Success' as NAME  from dual;
        else
            open o_resp_cursor for
                select DATA_LENGTH_OVER_LIMIT as STATUS_CODE, v_bath_id as ID, 'Description vuot qua 200 ky tu' as NAME  from dual;
        end if;        
        
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
       
    /*===========Ham cap nhat, chinh sua dot phat hanh================  */
    PROCEDURE "PRC_UPDATE_ISSUE_BATCH"
    (
        o_resp_cursor OUT ref_cur,
        i_issue_batch_id varchar2,
        i_name IN varchar2,
        i_description IN varchar2,
        i_issue_date IN date, --dd/MM/yyyy hh:mm:ss
        i_expire_date IN date, --dd/MM/yyyy hh:mm:ss
        i_modify_by IN varchar2) IS
        
        v_current_status varchar2(30);
    BEGIN
        
        if(nvl(length(i_description), 0)  <= 200) then
            -- Lay trang thai hien tai cua IssueBatch
            begin
            select status into v_current_status from vuc_issue_batch
                where id = i_issue_batch_id;
            exception when no_data_found then
                v_current_status := '';
            end;
            
           -- Ktra neu trang tai = 'APPROVED' thi khong duoc update
            if(v_current_status <> 'APPROVED') then
                update vuc_issue_batch set name = i_name, description = i_description,
                    issuedate = i_issue_date, expiredate = i_expire_date, 
                    lastmodifyby = i_modify_by, lastmodifydate = to_date(sysdate,'dd/MM/rrrr hh24:mi:ss')
                    where id = i_issue_batch_id;
                commit;
                            
                open o_resp_cursor for
                    select SUCCESS as STATUS_CODE, i_issue_batch_id as ID, 'Success' as NAME  from dual;
            else
                --Trang thai hien tai khong duoc phep cap nhat du lieu
                open o_resp_cursor for
                    select UPDATE_NOT_PERMISSION as STATUS_CODE, i_issue_batch_id as ID, 
                        'Trang thai khong hop le' as NAME  from dual;
            end if;
        else 
            open o_resp_cursor for
                select DATA_LENGTH_OVER_LIMIT as STATUS_CODE, i_issue_batch_id as ID, 'Description vuot qua 200 ky tu' as NAME  from dual;
        end if;
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham filter dot phat hanh================  */
    PROCEDURE "PRC_FILTER_ISSUE_BATCH"
    (   i_page_size in number,
        i_page_index in number,
        i_search_text in varchar2,
        o_issue_batch_cursor OUT ref_cur) IS
        
        v_search_text_no_accents varchar2(150);
        v_total_record number;
        v_divide number;
        v_total_page number;
      
    BEGIN
        
        --Bo dau tieng viet, count so luong ban ghi
        v_search_text_no_accents := fn_remove_accents(i_search_text);
        select count(*) into v_total_record from vuc_issue_batch 
            where replace(lower(fn_remove_accents(name)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
            or replace(lower(fn_remove_accents(description)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%';
        
        v_total_page := TRUNC(v_total_record / i_page_size);
        v_divide := v_total_page * i_page_size;
        
        if (v_total_record - v_divide > 0) then
            v_total_page := v_total_page + 1;
        end if;
      
        open o_issue_batch_cursor for 
            select tvi.*, v_total_record TotalRecord, v_total_page TotalPage from (
                select vib.*, RANK () OVER (ORDER BY id DESC) r__ from vuc_issue_batch vib
                where replace(lower(fn_remove_accents(name)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                or replace(lower(fn_remove_accents(description)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
            ) tvi WHERE     r__ > (i_page_index-1) * i_page_size
                AND r__ <= i_page_index* i_page_size;
                
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_issue_batch_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham get danh sach dot phat hanh ================  */
    PROCEDURE "PRC_GET_ALL_ISS_BATCHES"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2) IS

        v_search_text_no_accents varchar2(150);
    BEGIN
        
        -- Bo dau tieng viet
        v_search_text_no_accents := fn_remove_accents(i_text_search);

        open o_data_cursor for
            select id, name, description, issuedate, expiredate from vuc_issue_batch
                where replace(lower(fn_remove_accents(name)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    or replace(lower(fn_remove_accents(description)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%')
                    and status in ('APPROVED', 'WAIT_APPROVED')
                    order by id desc;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham get chi tiet issue batch theo id================  */
    PROCEDURE "PRC_GET_ISSUE_BATCH_BY_ID"
    (
        o_resp_cursor OUT ref_cur,
        i_issue_batch_id IN varchar2) IS

    BEGIN

        open o_resp_cursor for
                select * from vuc_issue_batch where id = i_issue_batch_id;
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;    
    
    
END PKG_VUC_CMS_ISSUE_BATCH;
/
