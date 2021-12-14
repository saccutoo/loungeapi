DROP PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CMS_MAP_VOUCHER_CUST;

CREATE OR REPLACE PACKAGE BODY UTIL_IPORTAL.PKG_VUC_CMS_MAP_VOUCHER_CUST
IS
    /*===========Ham get danh sach loai giao dich ap dung================  */
    PROCEDURE "PRC_GET_LIST_TRANS_TYPE" (o_data_cursor      OUT ref_cur,
                                         i_text_search   IN     VARCHAR2)
    IS
        v_search_text_no_accents   VARCHAR2 (150);
    BEGIN
        v_search_text_no_accents := fn_remove_accents (i_text_search);

        OPEN o_data_cursor FOR   SELECT id,
                                        transtype,
                                        name,
                                        description
                                   FROM vuc_transtype
                                  WHERE --(replace(lower(fn_remove_accents(transtype)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%'
                                        --or replace(lower(fn_remove_accents(description)), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%'))
                                                                         --and
                                        status = 'ACTIVE'
                               ORDER BY id DESC;
    EXCEPTION
        WHEN OTHERS
        THEN
            --dbms_output.put_line(SQLERRM);
            ROLLBACK;

            OPEN o_data_cursor FOR
                SELECT EXCEPTION_ERROR     AS STATUS_CODE,
                       ''                  AS ID,
                       'failed'            AS NAME
                  FROM DUAL;
    END;


    /*===========Ham kiem tra so luong khach hang khi gan voucher================  */
    PROCEDURE "PRC_VALIDATE_VOUCHER_CUSTOMER" (
        o_resp_cursor                OUT ref_cur,
        i_voucher_id              IN     NUMBER,                  --Ma voucher
        i_trans_type              IN     VARCHAR2,           -- Loai giao dich
        i_num_of_voucher_target   IN     NUMBER)
    IS                                             -- So luong voucher yeu cau
        v_count_remain_vouchers   NUMBER;           --So luong voucher con lai
    BEGIN
        --Count so luong voucher con lai chua duoc gan cho KH
        SELECT COUNT (*)
          INTO v_count_remain_vouchers
          FROM vuc_published_voucher tpv
         WHERE     tpv.id NOT IN
                       (SELECT publishvoucherid FROM vuc_map_voucher_cust)
               AND tpv.voucherid = i_voucher_id;

        --Kiem tra so luong KH va so luong voucher ocn lai
        IF (i_num_of_voucher_target > v_count_remain_vouchers)
        THEN
            OPEN o_resp_cursor FOR
                SELECT NOT_ENOUGH_ITEMS
                           AS STATUS_CODE,
                       ''
                           AS ID,
                       'So luong voucher yeu cau lon hon so luong vouchers con lai.'
                           AS NAME
                  FROM DUAL;
        ELSE
            OPEN o_resp_cursor FOR
                SELECT SUCCESS                            AS STATUS_CODE,
                       ''                                 AS ID,
                       'Du so luong voucher yeu cau.'     AS NAME
                  FROM DUAL;
        END IF;
    EXCEPTION
        WHEN OTHERS
        THEN
            --dbms_output.put_line(SQLERRM);
            ROLLBACK;

            OPEN o_resp_cursor FOR
                SELECT EXCEPTION_ERROR     AS STATUS_CODE,
                       ''                  AS ID,
                       'failed'            AS NAME
                  FROM DUAL;
    END;


    /*===========Ham gan voucher cho KH EBank================  */
    PROCEDURE "PRC_MAP_VOUCHER_CUSTOMER_EBANK" (
        o_resp_cursor            OUT ref_cur,
        i_voucher_id          IN     NUMBER,                      --Ma voucher
        i_trans_type          IN     VARCHAR2,               -- Loai giao dich
        i_upload_id           IN     VARCHAR2,                    -- Ma upload
        i_customer_id         IN     VARCHAR2,                    -- So cif KH
        i_customer_name       IN     VARCHAR2,                       -- Ten KH
        i_max_used_per_cust   IN     NUMBER,          -- So lan su dung toi da
        i_modify_by           IN     VARCHAR2)
    IS
        v_count_remain_vouchers   NUMBER;           --So luong voucher con lai
        v_count_customer_valid    NUMBER;         --Dem KH da duoc gan voucher

        v_map_id                  VARCHAR2 (30);
        v_serial_num              VARCHAR2 (12);
        v_publish_id              VARCHAR2 (30);
    BEGIN
        /*Count so luong voucher con lai chua duoc gan cho KH*/
        SELECT COUNT (*)
          INTO v_count_remain_vouchers
          FROM vuc_published_voucher tpv
         WHERE     tpv.id NOT IN
                       (SELECT publishvoucherid FROM vuc_map_voucher_cust)
               AND tpv.voucherid = i_voucher_id
               AND status IN ('USED', 'UNUSED');

        /*Kiem tra so luong KH va so luong voucher con lai*/
        IF (fn_vuc_count_voucher_remaining (i_voucher_id) > 0)
        THEN
            SELECT COUNT (*)
              INTO v_count_customer_valid
              FROM vuc_map_voucher_cust tmv, vuc_published_voucher tpv
             WHERE     tmv.customerid = i_customer_id
                   AND tmv.publishvoucherid = tpv.id
                   AND tpv.voucherid = i_voucher_id
                   AND tmv.status IN ('USED', 'UNUSED');

            /*Bo qua KH da duoc gan voucher roi*/
            IF (v_count_customer_valid = 0)
            THEN
                  SELECT id
                    INTO v_publish_id
                    FROM vuc_published_voucher tpv
                   WHERE     tpv.id NOT IN
                                 (SELECT publishvoucherid
                                    FROM vuc_map_voucher_cust)
                         AND tpv.voucherid = i_voucher_id
                         AND ROWNUM = 1
                ORDER BY id ASC;

                v_map_id := seq_vuc_map_vouch_cust.NEXTVAL;
                v_serial_num := fn_voucher_generate_serial (i_voucher_id);

                INSERT INTO vuc_map_voucher_cust (id,
                                                  publishvoucherid,
                                                  customerid,
                                                  customername,
                                                  uploadid,
                                                  serialnum,
                                                  transtype,
                                                  maxusedquantitypercust,
                                                  status,
                                                  createdate,
                                                  createby)
                     VALUES (v_map_id,
                             v_publish_id,
                             i_customer_id,
                             i_customer_name,
                             i_upload_id,
                             v_serial_num,
                             i_trans_type,
                             i_max_used_per_cust,
                             'UNUSED',
                             TO_DATE (SYSDATE, 'dd/MM/rrrr hh24:mi:ss'),
                             i_modify_by);

                --commit;

                OPEN o_resp_cursor FOR
                    SELECT SUCCESS       AS STATUS_CODE,
                           ''            AS ID,
                           'Success'     AS NAME
                      FROM DUAL;
            ELSE
                OPEN o_resp_cursor FOR
                    SELECT FAILE_CONFLICT
                               AS STATUS_CODE,
                           ''
                               AS ID,
                           ('Khach hang da duoc gan voucher.' || i_voucher_id)
                               AS NAME
                      FROM DUAL;
            END IF;
        ELSE
            OPEN o_resp_cursor FOR
                SELECT NOT_ENOUGH_ITEMS                         AS STATUS_CODE,
                       ''                                       AS ID,
                       'Vuot qua so luong voucher con lai.'     AS NAME
                  FROM DUAL;
        END IF;
    EXCEPTION
        WHEN OTHERS
        THEN
            --dbms_output.put_line(SQLERRM);
            ROLLBACK;

            OPEN o_resp_cursor FOR
                SELECT EXCEPTION_ERROR     AS STATUS_CODE,
                       ''                  AS ID,
                       'failed'            AS NAME
                  FROM DUAL;
    END;

    PROCEDURE "PRC_MAP_VUC_CUSTOMER_EBANK_NEW" (
        /*author: tunq2*/
        o_resp_cursor      OUT ref_cur,
        i_voucher_id    IN     NUMBER,                            --Ma voucher
        i_trans_type    IN     VARCHAR2,                     -- Loai giao dich
        i_upload_id     IN     VARCHAR2,                          -- Ma upload
        i_modify_by     IN     VARCHAR2,
        i_listCust      IN     VARCHAR2)
    IS
        v_count_customer_valid     NUMBER;        --Dem KH da duoc gan voucher
        v_map_id                   VARCHAR2 (30);
        v_serial_num               VARCHAR2 (12);
        v_publish_id               VARCHAR2 (30);
        v_json                     VARCHAR2 (32767);
        v_ViTriKetThucObject       NUMBER;
        v_object                   VARCHAR2 (1000);
        v_customerid               VARCHAR2 (100);
        v_customername             VARCHAR2 (100);
        v_maxusedquantitypercust   NUMBER;
    BEGIN
        v_json := i_listCust;

        SELECT INSTR (v_json, '_') INTO v_ViTriKetThucObject FROM DUAL;

        WHILE v_ViTriKetThucObject <> 0
        LOOP
            v_object := SUBSTR (v_json, 0, v_ViTriKetThucObject - 1);
            /* Lay thong tin customer */
            v_customerid := REGEXP_SUBSTR (SUBSTR (v_object, 0, v_ViTriKetThucObject - 1), '[^*]+', 1, 1);
            v_customername := REGEXP_SUBSTR (SUBSTR (v_object, 0, v_ViTriKetThucObject - 1), '[^*]+', 1, 2);
            v_maxusedquantitypercust := TO_NUMBER (REGEXP_SUBSTR (SUBSTR (v_object, 0, v_ViTriKetThucObject - 1), '[^*]+', 1, 3));

            /*Kiem tra so luong KH va so luong voucher con lai*/
            IF (fn_vuc_count_voucher_remaining (i_voucher_id) > 0)
            THEN
                SELECT COUNT (*)
                  INTO v_count_customer_valid
                  FROM vuc_map_voucher_cust tmv  --, vuc_published_voucher tpv
                 WHERE     tmv.customerid = v_customerid
                       --AND tmv.publishvoucherid = tpv.id
                       AND tmv.voucherid = i_voucher_id
                       AND tmv.status IN ('USED', 'UNUSED', 'OUT_OF_STOCK');

                /*Bo qua KH da duoc gan voucher roi*/
                IF (v_count_customer_valid = 0)
                THEN
                    SELECT id INTO v_publish_id FROM vuc_published_voucher tpv
                    WHERE tpv.id NOT IN (SELECT publishvoucherid FROM vuc_map_voucher_cust WHERE voucherid = i_voucher_id) 
                    AND ROWNUM = 1 AND tpv.voucherid = i_voucher_id
                    ORDER BY id ASC;

                    v_map_id := seq_vuc_map_vouch_cust.NEXTVAL;
                    v_serial_num := fn_voucher_generate_serial (i_voucher_id);

                    INSERT INTO vuc_map_voucher_cust (id, publishvoucherid, voucherid,
                                                      customerid, customername,
                                                      uploadid, serialnum, transtype,
                                                      maxusedquantitypercust, status,
                                                      createdate, createby)
                     VALUES (v_map_id, v_publish_id, i_voucher_id,
                             v_customerid, v_customername,
                             i_upload_id, v_serial_num, i_trans_type,
                             v_maxusedquantitypercust, 'UNUSED',
                             TO_DATE (SYSDATE, 'dd/MM/rrrr hh24:mi:ss'), i_modify_by);
                --commit;
                ELSE
                    OPEN o_resp_cursor FOR
                        SELECT FAILE_CONFLICT AS STATUS_CODE, '' AS ID,
                               ('Khach hang da duoc gan voucher.' || i_voucher_id) AS NAME FROM DUAL;
                END IF;
            ELSE
                OPEN o_resp_cursor FOR
                    SELECT NOT_ENOUGH_ITEMS AS STATUS_CODE, '' AS ID, 'Vuot qua so luong voucher con lai.' AS NAME FROM DUAL;
            END IF;

            v_json :=
                SUBSTR (v_json, v_ViTriKetThucObject + 1, LENGTH (v_json));

            SELECT INSTR (v_json, '_') INTO v_ViTriKetThucObject FROM DUAL;
        END LOOP;

        OPEN o_resp_cursor FOR
            SELECT SUCCESS AS STATUS_CODE, '' AS ID, 'Success' AS NAME
              FROM DUAL;
    --   EXCEPTION
    --      WHEN OTHERS
    --      THEN
    --         --dbms_output.put_line(SQLERRM);
    --         ROLLBACK;
    --
    --         OPEN o_resp_cursor FOR
    --            SELECT EXCEPTION_ERROR AS STATUS_CODE, '' AS ID, 'failed' AS NAME
    --              FROM DUAL;
    END;


    /*===========Ham gan voucher cho KH ELOUNGE================  */
    PROCEDURE "PRC_MAP_VOUCHER_CUSTOMER_ELG" (
        o_resp_cursor         OUT ref_cur,
        i_voucher_id       IN     NUMBER,                         --Ma voucher
        i_upload_id        IN     VARCHAR2,                       -- Ma upload
        i_customer_id      IN     VARCHAR2,                       -- So cif KH
        i_customer_name    IN     VARCHAR2,                          -- Ten KH
        i_represent_name   IN     NUMBER,                -- Ten nguoi dai dien
        i_modify_by        IN     VARCHAR2)
    IS
        v_count_remain_vouchers   NUMBER;           --So luong voucher con lai
        v_count_customer_valid    NUMBER;         --Dem KH da duoc gan voucher

        v_map_id                  VARCHAR2 (30);
        v_serial_num              VARCHAR2 (12);
        v_publish_id              VARCHAR2 (30);
    BEGIN
        /*Count so luong voucher con lai chua duoc gan cho KH*/
        SELECT COUNT (*)
          INTO v_count_remain_vouchers
          FROM vuc_published_voucher tpv
         WHERE     tpv.id NOT IN
                       (SELECT publishvoucherid FROM vuc_map_voucher_cust)
               AND tpv.voucherid = i_voucher_id
               AND status IN ('USED', 'UNUSED');

        /*Kiem tra so luong KH va so luong voucher con lai*/
        IF (fn_vuc_count_voucher_remaining (i_voucher_id) > 0)
        THEN
            SELECT COUNT (*)
              INTO v_count_customer_valid
              FROM vuc_map_voucher_cust tmv, vuc_published_voucher tpv
             WHERE     tmv.customerid = i_customer_id
                   AND tmv.publishvoucherid = tpv.id
                   AND tpv.voucherid = i_voucher_id
                   AND tmv.status IN ('USED', 'UNUSED');

            /*Bo qua KH da duoc gan voucher roi*/
            IF (v_count_customer_valid = 0)
            THEN
                  SELECT id
                    INTO v_publish_id
                    FROM vuc_published_voucher tpv
                   WHERE     tpv.id NOT IN
                                 (SELECT publishvoucherid
                                    FROM vuc_map_voucher_cust)
                         AND tpv.voucherid = i_voucher_id
                         AND ROWNUM = 1
                ORDER BY id ASC;

                v_map_id := seq_vuc_map_vouch_cust.NEXTVAL;
                v_serial_num := fn_voucher_generate_serial (i_voucher_id);

                INSERT INTO vuc_map_voucher_cust (id,
                                                  publishvoucherid,
                                                  customerid,
                                                  customername,
                                                  uploadid,
                                                  serialnum,
                                                  status,
                                                  createdate,
                                                  createby)
                     VALUES (v_map_id,
                             v_publish_id,
                             i_customer_id,
                             i_customer_name,
                             i_upload_id,
                             v_serial_num,
                             'UNUSED',
                             TO_DATE (SYSDATE, 'dd/MM/rrrr hh24:mi:ss'),
                             i_modify_by);

                --commit;

                OPEN o_resp_cursor FOR
                    SELECT SUCCESS       AS STATUS_CODE,
                           ''            AS ID,
                           'Success'     AS NAME
                      FROM DUAL;
            ELSE
                OPEN o_resp_cursor FOR
                    SELECT FAILE_CONFLICT
                               AS STATUS_CODE,
                           ''
                               AS ID,
                           ('Khach hang da duoc gan voucher.' || i_voucher_id)
                               AS NAME
                      FROM DUAL;
            END IF;
        ELSE
            OPEN o_resp_cursor FOR
                SELECT NOT_ENOUGH_ITEMS                         AS STATUS_CODE,
                       ''                                       AS ID,
                       'Vuot qua so luong voucher con lai.'     AS NAME
                  FROM DUAL;
        END IF;
    EXCEPTION
        WHEN OTHERS
        THEN
            --dbms_output.put_line(SQLERRM);
            ROLLBACK;

            OPEN o_resp_cursor FOR
                SELECT EXCEPTION_ERROR     AS STATUS_CODE,
                       ''                  AS ID,
                       'failed'            AS NAME
                  FROM DUAL;
    END;


    /*===========Ham get danh sach voucher - customer================  */
    PROCEDURE "PRC_FILTER_VOUCHER_MAPPING" (
        i_page_size         IN     NUMBER,
        i_page_index        IN     NUMBER,
        o_data_cursor          OUT ref_cur,
        i_text_search       IN     VARCHAR2,                --Tu khoa tim kiem
        i_trans_type        IN     VARCHAR2,                 -- Loai giao dich
        i_voucher_id        IN     NUMBER,
        i_channel_id        IN     NUMBER,
        i_issue_batch_id    IN     NUMBER,
        i_voucher_type_id   IN     NUMBER)
    IS
        v_search_text_no_accents   VARCHAR2 (150);
        v_total_record             NUMBER;
        v_divide                   NUMBER;
        v_total_page               NUMBER;
    BEGIN
        v_search_text_no_accents := fn_remove_accents (i_text_search);

        select count(*) into v_total_record
            from vuc_map_voucher_cust mvc
                    inner join (
                        select vvd.id, vvd.name, vvd.channelid, vvd.issuebatchid, vvd.vouchertypeid, 
                                vvd.effectivedate, vvd.expiredate--, vpv.COUNTUSED, vpv.id publishid
                                from vuc_voucher_definition vvd -- vuc_published_voucher vpv, 
                                where --vpv.voucherid = vvd.id and
                                vvd.channelid in (select id from vuc_applied_channel where id = i_channel_id or i_channel_id = 0)
                                and vvd.issuebatchid in (select id from vuc_issue_batch where id = i_issue_batch_id or i_issue_batch_id = 0)
                                and vvd.vouchertypeid in (select id from vuc_voucher_type where id = i_voucher_type_id or i_voucher_type_id = 0)
                    ) voucherdf on mvc.voucherid = voucherdf.id
                                --and (replace(lower(voucherdf.name), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%')    
                                and mvc.voucherid in (select id from vuc_voucher_definition where id = i_voucher_id or i_voucher_id = 0)
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
                    FN_GET_COUNT_USED_VOUCHER(mvc.PUBLISHVOUCHERID) AS COUNTUSED,
                    RANK () OVER (ORDER BY mvc.id DESC) r__
                    from vuc_map_voucher_cust mvc
                        inner join (
                            select vvd.id, vvd.name, vvd.channelid, vvd.issuebatchid, vvd.vouchertypeid, 
                                    vvd.effectivedate, vvd.expiredate --,vpv.id publishid, vpv.COUNTUSED
                                    from vuc_voucher_definition vvd --vuc_published_voucher vpv, 
                                    where --vpv.voucherid = vvd.id and
                                    --and (replace(lower(voucherdf.name), ' ', '%') like '%' ||replace(lower(v_search_text_no_accents), ' ', '%') || '%')   
                                    vvd.channelid in (select id from vuc_applied_channel where id = i_channel_id or i_channel_id = 0)
                                    and vvd.issuebatchid in (select id from vuc_issue_batch where id = i_issue_batch_id or i_issue_batch_id = 0)
                                    and vvd.vouchertypeid in (select id from vuc_voucher_type where id = i_voucher_type_id or i_voucher_type_id = 0)
                        ) voucherdf on mvc.voucherid = voucherdf.id
                                    and mvc.voucherid in (select id from vuc_voucher_definition where id = i_voucher_id or i_voucher_id = 0)
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
    PROCEDURE "PRC_CANCEL_VOUCHER_MAPPING" (o_resp_cursor      OUT ref_cur,
                                            i_map_id        IN     NUMBER, --Ma mapping voucher
                                            i_modify_by     IN     VARCHAR2)
    IS
        v_count_used   NUMBER;
    BEGIN
        UPDATE vuc_map_voucher_cust
           SET status = 'CANCEL',
               lastmodifyby = i_modify_by,
               lastmodifydate = TO_DATE (SYSDATE, 'dd/MM/rrrr hh24:mi:ss')
         WHERE id = i_map_id;

        OPEN o_resp_cursor FOR
            SELECT SUCCESS AS STATUS_CODE, '' AS ID, 'Success' AS NAME
              FROM DUAL;
    EXCEPTION
        WHEN OTHERS
        THEN
            --dbms_output.put_line(SQLERRM);
            ROLLBACK;

            OPEN o_resp_cursor FOR
                SELECT EXCEPTION_ERROR     AS STATUS_CODE,
                       ''                  AS ID,
                       'failed'            AS NAME
                  FROM DUAL;
    END;


    /*===========Ham generate serial voucher================*/
    FUNCTION FN_VOUCHER_GENERATE_SERIAL (i_voucher_id IN VARCHAR2)
        RETURN VARCHAR2
    IS
        --v_count_available number;
        v_new_serial_num   VARCHAR2 (12);
    BEGIN
        SELECT new_serial_num
          INTO v_new_serial_num
          FROM (SELECT DBMS_RANDOM.string ('X', 12) new_serial_num FROM DUAL)
         WHERE new_serial_num NOT IN
                   (SELECT serialnum FROM vuc_map_voucher_cust);

        RETURN v_new_serial_num;
    EXCEPTION
        WHEN OTHERS
        THEN
            RETURN LPAD (SEQ_VUC_MAP_VOUCH_CUST.NEXTVAL, 12, 0);
    END;
    
    /*===========Ham get count used voucher ================*/
    FUNCTION FN_GET_COUNT_USED_VOUCHER (i_publish_voucher_id IN NUMBER)
        RETURN number
    IS
        v_count_used number;
    BEGIN
        SELECT COUNTUSED
          INTO v_count_used
          FROM vuc_published_voucher
         WHERE ID = i_publish_voucher_id;
         
        RETURN v_count_used;
    EXCEPTION
        WHEN OTHERS
        THEN
            RETURN 0;
    END;
END PKG_VUC_CMS_MAP_VOUCHER_CUST;
/
