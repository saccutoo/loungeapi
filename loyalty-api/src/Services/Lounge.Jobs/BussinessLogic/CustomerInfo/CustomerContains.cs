using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.BussinessLogic
{
    public class CustomerContains
    {
        public const int STATUS_SUCCESS_CODE = 1;
        public const int STATUS_ERROR_CODE = 0;
        public const string SV_SYNC_ID = "SV_SYNC_ID";
        public const string OOS_SYNC_ID = "OOS_SYNC_ID";
        public const string STATUS_CUSTOMER_HIS_DONE = "DONE";
        public const string STATUS_CUSTOMER_HIS_SMS_ERROR = "SMS_ERROR";
        public const string STATUS_CUSTOMER_HIS_SENDING = "SENDING";
        public const string STATUS_CUSTOMER_HIS_URBOX_ERROR_GIFT_EMPTY = "URBOX_GIFT_EMPTY";
        public const string STATUS_CUSTOMER_HIS_URBOX_ERROR = "URBOX_ERROR";
    }
}
