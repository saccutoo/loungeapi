using Lounge.Models;
using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{

    public class ElgNotificationModel : ElgBookingBaseModel
    {      
        public string FaceId { get; set; }
        public string Value { get; set; }
        public string Base64 { get; set; }
        public string Other { get; set; }
        public string Status { get; set; }
    }

    public class ElgNotificationViewModel : ElgNotificationModel
    {
        public ElgCustomerDecryptModel ValueDecrypt { get; set; }
    }

    public class ElgNotificationCreateModel
    {
        public string FaceId { get; set; }
        public string Value { get; set; }
        public string base64 { get; set; }
        public string Other { get; set; }
        public string CreateBy { get; set; }

    }

    public class ElgNotificationUpdateModel
    {
        public string FaceId { get; set; }
        public string Value { get; set; }
        public decimal Id { get; set; }

    }
    public class ElgNotificationQueryModel : PaginationRequest
    {
        public string FaceId { get; set; }
        public string CusId { get; set; }
        public string PhoneNum { get; set; }
    }

    public class ElgNotificationPushKafkaModel : ElgBookingBaseModel
    {
        public string FaceId { get; set; }
        public object Value { get; set; }
        public string Base64 { get; set; }
        public string Other { get; set; }
        public string Status { get; set; }
    }
}