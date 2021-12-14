BEGIN
  SYS.DBMS_SCHEDULER.DROP_JOB
    (job_name  => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED');
END;
/

BEGIN
  SYS.DBMS_SCHEDULER.CREATE_JOB
    (
       job_name        => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED'
      ,schedule_name   => 'UTIL_IPORTAL.SCH_VUC_SCAN_VOUCHER_EXPIRED'
      ,job_class       => 'DBMS_JOB$'
      ,job_type        => 'PLSQL_BLOCK'
      ,job_action      => 'DECLARE
    v_new_status varchar2(50);
    v_return varchar2(10);
  begin
    v_new_status := ''EXPIRED'';
    v_return := FN_VUC_SCAN_VOUCHER_EXPIRED(v_new_status);
    dbms_output.put_line(''v_return'' || v_return);
  end;'
      ,comments        => 'Job tu dong quet va cap nhat trang thai voucher het han'
    );
  SYS.DBMS_SCHEDULER.SET_ATTRIBUTE
    ( name      => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED'
     ,attribute => 'RESTARTABLE'
     ,value     => FALSE);
  SYS.DBMS_SCHEDULER.SET_ATTRIBUTE
    ( name      => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED'
     ,attribute => 'LOGGING_LEVEL'
     ,value     => SYS.DBMS_SCHEDULER.LOGGING_OFF);
  SYS.DBMS_SCHEDULER.SET_ATTRIBUTE_NULL
    ( name      => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED'
     ,attribute => 'MAX_FAILURES');
  SYS.DBMS_SCHEDULER.SET_ATTRIBUTE_NULL
    ( name      => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED'
     ,attribute => 'MAX_RUNS');
  SYS.DBMS_SCHEDULER.SET_ATTRIBUTE
    ( name      => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED'
     ,attribute => 'STOP_ON_WINDOW_CLOSE'
     ,value     => FALSE);
  SYS.DBMS_SCHEDULER.SET_ATTRIBUTE
    ( name      => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED'
     ,attribute => 'JOB_PRIORITY'
     ,value     => 3);
  SYS.DBMS_SCHEDULER.SET_ATTRIBUTE_NULL
    ( name      => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED'
     ,attribute => 'SCHEDULE_LIMIT');
  SYS.DBMS_SCHEDULER.SET_ATTRIBUTE
    ( name      => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED'
     ,attribute => 'AUTO_DROP'
     ,value     => FALSE);

  SYS.DBMS_SCHEDULER.ENABLE
    (name                  => 'UTIL_IPORTAL.JOB_VUC_SCAN_VOUCHER_EXPIRED');
END;
/
