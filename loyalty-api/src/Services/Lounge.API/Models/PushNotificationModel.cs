using System;
using System.Collections.Generic;
using System.Text;

namespace API.Models
{
    class PushNotificationModel
    {
        public decimal ID { set; get; }
        public string NEWS_CONTENT { set; get; }
        public string PAYLOAD_DATA { set; get; }
        public string NEWS_TITLE { set; get; }
        public string NEWS_TO { set; get; }
        public string NEWS_COMM_TYPE { set; get; }
        public decimal STATUS { set; get; }
        public decimal PRIORITY { set; get; }
        public decimal IS_VIEW_DETAIL { set; get; }
        public DateTime CREATE_DATE { set; get; }
        public DateTime MODIFIED_DATE { set; get; }
        public string PARENT_ID { set; get; }
        public string APP_SRC { set; get; }
        public string FUNC_SRC { set; get; }
        public string REF_ID { set; get; }
    }
}
