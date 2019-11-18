using System;
using System.Collections.Generic;

namespace RailwayApi.Models
{
    public class Token {
        public string token { get; set; }
        public PaymentInfo paymentInfo { get; set; }
        public string methodName { get; set; }
    }
}