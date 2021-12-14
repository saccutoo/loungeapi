DROP PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CMS_MAP_VOUCHER_CUST;

CREATE OR REPLACE PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CMS_MAP_VOUCHER_CUST IS
    

    /*===========Ham get danh sach loai giao dich ap dung================  */
    PROCEDURE "PRC_GET_LIST_TRANS_TYPE"
    (
        o_data_cursor OUT ref_cur,
        i_text_search IN varchar2) IS

        v_search_text_no_accents varchar2(150);
    BEGIN
        
        v_search_text_no_accents := fn_remove_accents(i_text_search);

        open o_data_cursor for
            select id, transtype, name, description from vuc_transtype
                where --(replace(lower(fn_remove_accents(transtype)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    --or replace(lower(fn_remove_accents(description)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%'))
                    --and 
                    status = 'ACTIVE' order by id desc;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham kiem tra so luong khach hang khi gan voucher================  */
    PROCEDURE "PRC_VALIDATE_VOUCHER_CUSTOMER"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, --Ma voucher
        i_trans_type IN varchar2, -- Loai giao dich
        i_num_of_voucher_target IN number) IS -- So luong voucher yeu cau
        
        v_count_remain_vouchers number; --So luong voucher con lai
    BEGIN
        
        --Count so luong voucher con lai chua duoc gan cho KH
        select count(*) into v_count_remain_vouchers from vuc_published_voucher tpv
            where tpv.id not in (select publishvoucherid from vuc_map_voucher_cust)
                    and tpv.voucherid = i_voucher_id;
                    
        --Kiem tra so luong KH va so luong voucher ocn lai
        if(i_num_of_voucher_target > v_count_remain_vouchers) then
            open o_resp_cursor for
                select NOT_ENOUGH_ITEMS as STATUS_CODE, '' as ID, 
                    'So luong voucher yeu cau lon hon so luong vouchers con lai.' as NAME  from dual;
        else
            open o_resp_cursor for
                select SUCCESS as STATUS_CODE, '' as ID, 
                    'Du so luong voucher yeu cau.' as NAME  from dual;
        end if;
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
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
        i_modify_by IN varchar2) IS
        
        v_count_remain_vouchers number; --So luong voucher con lai     
        v_count_customer_valid number; --Dem KH da duoc gan voucher
        
        v_map_id varchar2(30); 
        v_serial_num varchar2(12);
        v_publish_id varchar2(30);
    BEGIN
        
        /*Count so luong voucher con lai chua duoc gan cho KH*/
        select count(*) into v_count_remain_vouchers from vuc_published_voucher tpv
            where tpv.id not in (select publishvoucherid from vuc_map_voucher_cust)
                    and tpv.voucherid = i_voucher_id and status in ('USED', 'UNUSED');
                        
        /*Kiem tra so luong KH va so luong voucher con lai*/
        if(fn_vuc_count_voucher_remaining(i_voucher_id) > 0) then
            select count(*) into v_count_customer_valid from vuc_map_voucher_cust tmv, vuc_published_voucher tpv
                where tmv.customerid = i_customer_id and tmv.publishvoucherid = tpv.id
                    and tpv.voucherid = i_voucher_id and tmv.status in ('USED', 'UNUSED');
                
            /*Bo qua KH da duoc gan voucher roi*/
            if(v_count_customer_valid = 0) then
                select id into v_publish_id from vuc_published_voucher tpv
                where tpv.id not in (select publishvoucherid from vuc_map_voucher_cust)
                    and tpv.voucherid = i_voucher_id and rownum = 1 order by id asc;
                v_map_id := seq_vuc_map_vouch_cust.nextval;
                v_serial_num := fn_voucher_generate_serial(i_voucher_id);
                            
                insert into vuc_map_voucher_cust(id, publishvoucherid, customerid,
                        customername, uploadid, serialnum, transtype, maxusedquantitypercust, status, createdate, createby)
                    values(v_map_id, v_publish_id, i_customer_id, i_customer_name, i_upload_id,
                        v_serial_num, i_trans_type, i_max_used_per_cust, 'UNUSED', to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss'), i_modify_by);
                --commit;
                
                open o_resp_cursor for
                        select SUCCESS as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;   
            else        
                open o_resp_cursor for
                    select FAILE_CONFLICT as STATUS_CODE, '' as ID, 
                        ('Khach hang da duoc gan voucher.' || i_voucher_id) as NAME  from dual;
            end if;
        else                         
            open o_resp_cursor for
                select NOT_ENOUGH_ITEMS as STATUS_CODE, '' as ID, 
                    'Vuot qua so luong voucher con lai.' as NAME  from dual;                      
        end if;
                
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;

    
    /*===========Ham gan voucher cho KH ELOUNGE================  */
    PROCEDURE "PRC_MAP_VOUCHER_CUSTOMER_ELG"
    (
        o_resp_cursor OUT ref_cur,
        i_voucher_id IN number, --Ma voucher
        i_upload_id IN varchar2, -- Ma upload
        i_customer_id IN varchar2, -- So cif KH
        i_customer_name IN varchar2, -- Ten KH
        i_represent_name IN number, -- Ten nguoi dai dien
        i_modify_by IN varchar2) IS
        
        v_count_remain_vouchers number; --So luong voucher con lai     
        v_count_customer_valid number; --Dem KH da duoc gan voucher
        
        v_map_id varchar2(30); 
        v_serial_num varchar2(12);
        v_publish_id varchar2(30);
    BEGIN
        
        /*Count so luong voucher con lai chua duoc gan cho KH*/
        select count(*) into v_count_remain_vouchers from vuc_published_voucher tpv
            where tpv.id not in (select publishvoucherid from vuc_map_voucher_cust)
                    and tpv.voucherid = i_voucher_id and status in ('USED', 'UNUSED');
                        
        /*Kiem tra so luong KH va so luong voucher con lai*/
        if(fn_vuc_count_voucher_remaining(i_voucher_id) > 0) then
            select count(*) into v_count_customer_valid from vuc_map_voucher_cust tmv, vuc_published_voucher tpv
                where tmv.customerid = i_customer_id and tmv.publishvoucherid = tpv.id
                    and tpv.voucherid = i_voucher_id and tmv.status in ('USED', 'UNUSED');
                
            /*Bo qua KH da duoc gan voucher roi*/
            if(v_count_customer_valid = 0) then
                select id into v_publish_id from vuc_published_voucher tpv
                where tpv.id not in (select publishvoucherid from vuc_map_voucher_cust)
                    and tpv.voucherid = i_voucher_id and rownum = 1 order by id asc;
                v_map_id := seq_vuc_map_vouch_cust.nextval;
                v_serial_num := fn_voucher_generate_serial(i_voucher_id);
                            
                insert into vuc_map_voucher_cust(id, publishvoucherid, customerid,
                        customername, uploadid, serialnum, status, createdate, createby)
                    values(v_map_id, v_publish_id, i_customer_id, i_customer_name, i_upload_id,
                        v_serial_num, 'UNUSED', to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss'), i_modify_by);
                --commit;
                
                open o_resp_cursor for
                        select SUCCESS as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;   
            else        
                open o_resp_cursor for
                    select FAILE_CONFLICT as STATUS_CODE, '' as ID, 
                        ('Khach hang da duoc gan voucher.' || i_voucher_id) as NAME  from dual;
            end if;
        else                         
            open o_resp_cursor for
                select NOT_ENOUGH_ITEMS as STATUS_CODE, '' as ID, 
                    'Vuot qua so luong voucher con lai.' as NAME  from dual;                      
        end if;
                
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_resp_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
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
        i_voucher_type_id IN number) IS
        
        v_search_text_no_accents varchar2(150);
        v_total_record number;
        v_divide number;
        v_total_page number;
    BEGIN
        v_search_text_no_accents := fn_remove_accents(i_text_search);
        
        select count(*) into v_total_record
            from vuc_map_voucher_cust mvc
                    inner join (
                        select vvd.id, vvd.name, vvd.channelid, vvd.issuebatchid, vvd.vouchertypeid, 
                                vpv.id publishid, vvd.effectivedate, vvd.expiredate, vpv.COUNTUSED
                                from vuc_published_voucher vpv, vuc_voucher_definition vvd
                                where vpv.voucherid = vvd.id
                    ) voucherdf on mvc.publishvoucherid = voucherdf.publishid
                                --and (replace(lower(voucherdf.name), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%')    
                                and voucherdf.id in (select id from vuc_voucher_definition where id = i_voucher_id or i_voucher_id = 0)
                                and voucherdf.channelid in (select id from vuc_applied_channel where id = i_channel_id or i_channel_id = 0)
                                and voucherdf.issuebatchid in (select id from vuc_issue_batch where id = i_issue_batch_id or i_issue_batch_id = 0)
                                and voucherdf.vouchertypeid in (select id from vuc_voucher_type where id = i_voucher_type_id or i_voucher_type_id = 0)
                    where( mvc.transtype = i_trans_type or i_trans_type = 0)
                    and (replace(lower(mvc.customerid), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    or replace(lower(mvc.customername), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    or replace(lower(voucherdf.name), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                    or replace(lower(mvc.uploadid), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%');  
        
        v_total_page := TRUNC(v_total_record / i_page_size);
        v_divide := v_total_page * i_page_size;
        
        if (v_total_record - v_divide > 0) then
            v_total_page := v_total_page + 1;
        end if;
        
        open o_data_cursor for        
            select tvi.*, v_total_record TotalRecord, v_total_page TotalPage from (
                select mvc.*, voucherdf.effectivedate, voucherdf.expiredate, voucherdf.name,
                    voucherdf.COUNTUSED,
                    RANK () OVER (ORDER BY mvc.id DESC) r__
                    from vuc_map_voucher_cust mvc
                        inner join (
                            select vvd.id, vvd.name, vvd.channelid, vvd.issuebatchid, vvd.vouchertypeid, 
                                    vpv.id publishid, vvd.effectivedate, vvd.expiredate, vpv.COUNTUSED
                                    from vuc_published_voucher vpv, vuc_voucher_definition vvd
                                    where vpv.voucherid = vvd.id
                        ) voucherdf on mvc.publishvoucherid = voucherdf.publishid
                                    --and (replace(lower(voucherdf.name), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%')    
                                    and voucherdf.id in (select id from vuc_voucher_definition where id = i_voucher_id or i_voucher_id = 0)
                                    and voucherdf.channelid in (select id from vuc_applied_channel where id = i_channel_id or i_channel_id = 0)
                                    and voucherdf.issuebatchid in (select id from vuc_issue_batch where id = i_issue_batch_id or i_issue_batch_id = 0)
                                    and voucherdf.vouchertypeid in (select id from vuc_voucher_type where id = i_voucher_type_id or i_voucher_type_id = 0)
                        where (mvc.transtype = i_trans_type or i_trans_type = 0)
                        and (replace(lower(mvc.customerid), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                        or replace(lower(mvc.customername), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                        or replace(lower(voucherdf.name), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                        or replace(lower(mvc.uploadid), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%')
                                    ) tvi WHERE     r__ > (i_page_index-1) * i_page_size
                    AND r__ <= i_page_index* i_page_size;
                    
    
    EXCEPTION
        WHEN OTHERS
        THEN
        --dbms_output.put_line(SQLERRM);
        rollback;
            open o_data_cursor for
                select EXCEPTION_ERROR as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham huy gan voucher cho KH EBank================  */
    PROCEDURE "PRC_CANCEL_VOUCHER_MAPPING"
    (
        o_resp_cursor OUT ref_cur,
        i_map_id IN number, --Ma mapping voucher
        i_modify_by IN varchar2) IS
        
        v_count_used number;
    BEGIN
        
        update vuc_map_voucher_cust set status = 'CANCEL', lastmodifyby = i_modify_by,
            lastmodifydate = to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss')
                where id = i_map_id;

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
    
    
    /*===========Ham generate serial voucher================*/
    FUNCTION FN_VOUCHER_GENERATE_SERIAL(
        i_voucher_id in varchar2)
     RETURN varchar2
     IS
      --v_count_available number;
      v_new_serial_num varchar2(12);
     BEGIN
        select new_serial_num into v_new_serial_num from (
           select dbms_random.string('X', 12) new_serial_num from dual)
           where new_serial_num not in (select serialnum from vuc_map_voucher_cust);
                       
           return v_new_serial_num;
     EXCEPTION WHEN OTHERS THEN
          return lpad(SEQ_VUC_MAP_VOUCH_CUST.nextval, 12, 0);
     END;
     
     
END PKG_VUC_CMS_MAP_VOUCHER_CUST;
/
