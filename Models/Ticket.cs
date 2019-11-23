using System;
using System.Collections.Generic;

namespace RailwayApi.Models
{
    public class Ticket {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string When { get; set; }
        public Double Price { get; set; }
    }
}