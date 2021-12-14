using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    public class FlightData
    {
        public string outbound_departure_airport { get; set; }
        public string outbound_arrival_airport { get; set; }
        public string outbound_departure_time { get; set; }
        public string outbound_arrival_time { get; set; }
        public string inbound_departure_airport { get; set; }
        public string inbound_arrival_airport { get; set; }
        public string inbound_departure_time { get; set; }
        public string inbound_arrival_time { get; set; }
        public decimal total_price { get; set; }
        public decimal taxes { get; set; }

        public FlightData(string outboundDepartureAirport, string outboundArrivalAirport, string outboundDepartureTime, string outboundArrivalTime, string inboundDepartureAirport, string inboundArrivalAirport, string inboundDepartureTime, string inboundArrivalTime, decimal totalPrice, decimal taxes_)
        {
            outbound_departure_airport = outboundDepartureAirport;
            outbound_arrival_airport = outboundArrivalAirport;
            outbound_departure_time = outboundDepartureTime;
            outbound_arrival_time = outboundArrivalTime;
            inbound_departure_airport = inboundDepartureAirport;
            inbound_arrival_airport = inboundArrivalAirport;
            inbound_departure_time = inboundDepartureTime;
            inbound_arrival_time = inboundArrivalTime;
            total_price = totalPrice;
            taxes = taxes_;
        }
    }
}
