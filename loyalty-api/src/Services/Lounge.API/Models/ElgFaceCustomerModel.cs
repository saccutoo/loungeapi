using Lounge.Models;
using System;
using System.Collections.Generic;
using Utils;

namespace API.Models
{

    public class ElgFaceCustomerModel : ElgBookingBaseModel
    {      
        public string FaceId { get; set; }
        public string CustId { get; set; }
        public string PhoneNum { get; set; }
        public string Status { get; set; }
    }

    public class ElgFaceCustomerViewModel : ElgFaceCustomerModel
    {

    }

    public class ElgFaceCustomerCreateModel
    {
        public string FaceId { get; set; }
        public string CustId { get; set; }
        public string PhoneNum { get; set; }
        public string CreateBy { get; set; }

    }

    public class ElgFaceCustomerUpdateModel
    {
        public string FaceId { get; set; }
        public string CustId { get; set; }
        public string PhoneNum { get; set; }
        public string UpdatedBy { get; set; }

    }

    public class ElgFaceCustomerQueryModel : PaginationRequest
    {
        public string FaceId { get; set; }
        public string CusId { get; set; }
        public string PhoneNum { get; set; }
    }
}