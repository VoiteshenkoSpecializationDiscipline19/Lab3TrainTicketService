using System;
using System.Collections.Generic;

namespace RailwayApi.Models
{
    public class Ticket {
        public long Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime When { get; set; }
        public Double Price { get; set; }
    }
}