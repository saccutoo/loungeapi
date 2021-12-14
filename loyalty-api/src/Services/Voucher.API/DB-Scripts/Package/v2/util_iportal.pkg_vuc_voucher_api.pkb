DROP PACKAGE BODY UTIL_IPORTAL.PKG_VUC_VOUCHER_API;

CREATE OR REPLACE PACKAGE BODY UTIL_IPORTAL.PKG_VUC_VOUCHER_API IS
/**
    Package bao gom cac thu tuc:
    - Lay danh sach voucher cho KH Ebank
    - Cap nhat voucher sau khi su dung tren kenh ebank
    - 
**/

    /*===========Ham get danh sach voucher cho ebank================  */
    PROCEDURE "PRC_API_GET_VOUCHER_EBANK"
    (
        --io_errcode IN OUT varchar2,
        o_voucher_cursor OUT REF_CUR,
        i_cif_num IN varchar2, --186852113
        i_channel IN varchar2, --EBANK
        i_trans_type IN varchar2, --TOPUP
        i_trans_amount IN number) IS
        
        v_transtype_id number;
    BEGIN
        select id into v_transtype_id from vuc_transtype
            where transtype = i_trans_type;
            
        open o_voucher_cursor for
            select tvd.id voucherID, tvd.name voucherCode, tvd.descriptionvn voucherDescriptionVn, 
                tvd.descriptionen voucherDescriptionEn, tvd.effectivedate, 
                tvd.expiredate, tmc.maxusedquantitypercust maxQuantity, 
                tmc.serialnum, tpv.pinnum, 
                tib.name campName, tib.description campDescription, 
                --tva.amount_valuation amountVal, 
                case when tvt.valuetype = 'FIX_VAL' then tva.amountvaluation
                when tvt.valuetype = 'PERCENT_VAL' 
                    and (to_number(tva.percentvaluation) * to_number(i_trans_amount) / 100) > to_number(tva.maxamountcoupon) 
                        then tva.maxamountcoupon
                else
                    (to_number(tva.percentvaluation) * to_number(i_trans_amount) / 100)
                end amountVal,
                tva.percentvaluation percentVal,
                tva.mintransamount minTransAmount, tva.maxtransamount maxTransAmout,  
                tvt.valuetype valueType, to_number(tmc.maxusedquantitypercust) - tpv.countused as remainQuantity,
                case when to_number(i_trans_amount) between to_number(mintransamount)
                        and to_number(maxtransamount) then 'true'  --and tpv.status in ('OUT_OF_STOCK') 
                        else 'false'
                end isEligible,
                tpv.status
                --fn_check_voucher_eligible(tvd.id, i_trans_amount) as isEligible
                 from vuc_voucher_definition tvd, vuc_issue_batch tib,
                    vuc_published_voucher tpv, vuc_map_voucher_cust tmc,
                    vuc_applied_channel tac, vuc_voucher_amt_conditions tva,
                    vuc_voucher_type tvt
                    where tvd.id = tmc.voucherid 
                    and tmc.publishvoucherid = tpv.id
                    and tib.id = tvd.issuebatchid
                    and tva.voucherid = tvd.id and tvt.id = tvd.vouchertypeid 
                    and tmc.customerid = i_cif_num
                    and upper(tac.channelcode) = upper(i_channel)
                    and tmc.transtype = v_transtype_id
                    and to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss') between 
                        to_date(tvd.effectivedate, 'dd/MM/rrrr hh24:mi:ss') and to_date(tvd.expiredate, 'dd/MM/rrrr hh24:mi:ss')
                    --and to_number(i_trans_amount) between to_number(tva.min_trans_amount) and to_number(tva.max_trans_amount)
                    and to_number(tmc.maxusedquantitypercust) - tpv.countused > 0
                    and tpv.status = 'UNUSED'
                    order by isEligible desc;

        --io_errcode := '0';
    EXCEPTION
        WHEN OTHERS THEN
          --dbms_output.put_line(SQLERRM);
          ROLLBACK;
          open o_voucher_cursor for
            select '-1' as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
          --io_errcode := '-1';
    END;

    
    /*===========Ham kiem tra voucher co hop le hay khong====*/
    PROCEDURE "PRC_API_EBANK_CHECK_VOUCHER"
    (
        o_resp_cursor OUT REF_CUR,
        i_cif_num IN varchar2,
        i_channel IN varchar2,
        i_trans_type IN varchar2,
        i_trans_amount IN number,
        i_discount_amount IN number,
        i_pin_num IN varchar2) IS

        v_count_valid number;
        v_transtype_id number;
    BEGIN
        select id into v_transtype_id from vuc_transtype
            where transtype = i_trans_type;
            
        select count(*) into v_count_valid from vuc_voucher_definition tvd, vuc_issue_batch tib,
                    vuc_published_voucher tpv, vuc_map_voucher_cust tmc,
                    vuc_applied_channel tac, vuc_voucher_amt_conditions tva,
                    vuc_voucher_type tvt
                    where tvd.id = tmc.voucherid and tmc.publishvoucherid = tpv.id
                    and tib.id = tvd.issuebatchid
                    and tmc.customerid = i_cif_num
                    and upper(tac.channelcode) = upper(i_channel)
                    and upper(tmc.transtype) = upper(v_transtype_id)
                    and to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss') between 
                        to_date(tvd.effectivedate, 'dd/MM/rrrr hh24:mi:ss') and to_date(tvd.expiredate, 'dd/MM/rrrr hh24:mi:ss')
                    and to_number(i_trans_amount) between to_number(tva.mintransamount) and to_number(tva.maxtransamount)
                    and to_number(tmc.maxusedquantitypercust) - to_number(tpv.countused) > 0
                    and tva.voucherid = tvd.id and tvt.id = tvd.vouchertypeid
                    and tpv.pinnum = i_pin_num
                    and tpv.status = 'UNUSED'
                    and i_discount_amount = (
                        case when tvt.valuetype = 'FIX_VAL' then to_number(tva.amountvaluation)
                        else
                            (to_number(tva.percentvaluation) * i_trans_amount)  / 100
                        end    
                    );
                
        if(v_count_valid > 0) then
            open o_resp_cursor for
                    select '00' as STATUS_CODE, '' as ID, 'Success' as NAME  from dual;
        else
            open o_resp_cursor for
                    select '01' as STATUS_CODE, '' as ID, 'Voucher khong hop le' as NAME  from dual;
        end if;
    EXCEPTION
        WHEN OTHERS THEN
          --dbms_output.put_line(SQLERRM);
          ROLLBACK;
          open o_resp_cursor for
                select '-1' as STATUS_CODE, '' as ID, 'failed' as NAME  from dual;
    END;
    
    
    /*===========Ham cap nhat trang thai voucher da su dung================*/
    PROCEDURE "PRC_API_EBANK_UPDATE_VOUCHER"
    (
        o_resp_cursor OUT REF_CUR,
        i_pin_num IN varchar2,
        i_cif_num IN varchar2,
        i_channel IN varchar2,
        i_trans_type IN varchar2,
        i_trans_amount IN number,
        i_discount_amount IN number,
        i_trans_refno IN varchar2) IS

        v_voucher_trans_id varchar2(30);
        v_voucher_id number;
        v_publish_voucherid number;
        v_count_used number;
        v_max_use_per_cust number;
        
        v_total_used number;
        v_maxusedquantity number;
    BEGIN
         --Cap nhat trang thai toan bo voucher publish khi het luot su dung cho phep
        begin
            select vpv.id, vpv.countused, vpv.voucherid into v_publish_voucherid, v_count_used, v_voucher_id 
                from vuc_published_voucher vpv where vpv.pinnum = i_pin_num;                 
                
            select sum(countused) into v_total_used from vuc_published_voucher where id = v_publish_voucherid;
            
            select vvd.maxusedquantity into v_maxusedquantity 
                from vuc_voucher_definition vvd--, vuc_published_voucher vpv
                    where vvd.id = v_voucher_id;--vpv.voucherid and vpv.pinnum = i_pin_num;
                    
        exception when OTHERS then
--            v_maxusedquantity := 0;
          open o_resp_cursor for
                select '-1' as STATUS_CODE, '' as ID, '-failed' as NAME  from dual;
        end;
        
        if(v_total_used + 1 > v_maxusedquantity) then
--            update  vuc_published_voucher set status = 'OUT_OF_STOCK' where voucherid = (
--                select voucherid from vuc_published_voucher where pinnum = i_pin_num
--            ) and status = 'UNUSED'; 
--            commit;
                
            open o_resp_cursor for
                select '01' as STATUS_CODE, v_voucher_trans_id as ID, 
                    'Voucher is out of stock.' as NAME  from dual;
        else
            v_voucher_trans_id := seq_vuc_voucher_trans.nextval;
        
--            select countused into v_count_used from vuc_published_voucher 
--                where pinnum = i_pin_num; 
            
            select maxusedquantitypercust into v_max_use_per_cust
                from vuc_map_voucher_cust where customerid = i_cif_num
                and publishvoucherid = v_publish_voucherid;
            
            v_count_used := v_count_used + 1;
            --Cap nhat trang thai voucher khi so luot su dung da het
            if(v_count_used >= v_max_use_per_cust) then
                update vuc_published_voucher set status = 'USED'
                    where id = v_publish_voucherid; --pinnum = i_pin_num;
                update vuc_map_voucher_cust set status = 'USED' where publishvoucherid = v_publish_voucherid;
                commit;
            end if;
            update vuc_published_voucher set countused = v_count_used
                where id = v_publish_voucherid; --where pinnum = i_pin_num;
            commit;
            
            insert into vuc_voucher_trans(id, pinnum, customerid, channel, 
                transtype, transamount, transrefno, datetime, status)
               values(v_voucher_trans_id, i_pin_num, i_cif_num, i_channel, 
                i_trans_type, i_trans_amount, i_trans_refno, to_date(sysdate, 'dd/MM/rrrr hh24:mi:ss'), 'COMPLETED');
            commit;
            
            v_total_used:= v_total_used + 1;
            -- Check neu tong so da su dung >= v_maxusedquantity -> update toan bo voucher còn lại ve trang thai OUT_OF_STOCK
            if(v_total_used >= v_maxusedquantity) then
                update  vuc_published_voucher set status = 'USED' where 
--                voucherid = (
--                    select voucherid from vuc_published_voucher where pinnum = i_pin_num
--                ) 
                voucherid = v_voucher_id
                and status = 'UNUSED'; 
                commit;
            end if;
            
            open o_resp_cursor for
                    select '00' as STATUS_CODE, v_voucher_trans_id as ID, 'Success' as NAME  from dual;
                    
        end if;
            
        
    EXCEPTION
        WHEN OTHERS THEN
          --dbms_output.put_line(SQLERRM);
          ROLLBACK;
          open o_resp_cursor for
                select '-1' as STATUS_CODE, '' as ID, '-failed' as NAME  from dual;
    END;

    /*===========Ham tinh so luong con lai cua voucher================*/
    FUNCTION FN_VOUCHER_REMAIN_QUALITY(
        i_voucher_id in varchar2)
     RETURN number
     IS
      v_voucher_quality number;
      v_count_used number;
     BEGIN
            select maxusedquantity into v_voucher_quality from vuc_voucher_definition
                where id = i_voucher_id;
            
            select count(*) into v_count_used from vuc_published_voucher
                where voucherid = i_voucher_id and status = 'USED';
            
            ---------Pendding To Do----------
            
           return v_voucher_quality - v_count_used;
     EXCEPTION WHEN OTHERS THEN
          return '0';
     END;

     
     
END PKG_VUC_VOUCHER_API;
/
