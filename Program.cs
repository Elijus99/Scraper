using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace Scraper
{
    class Program
    {
        static void Main(string[] args)
        {

            IWebDriver driver = new ChromeDriver();
            driver.Url = "https://www.fly540.com";

            string departureAirport = "NBO";
            string arrivalAirport = "MBA";
            int daysTillDeparture = 10;
            int tripDuration = 7;
            int daysTillReturn = daysTillDeparture + tripDuration;
            
            Functions.searchFlights(driver, departureAirport, arrivalAirport, daysTillDeparture, daysTillReturn);

            List<FlightData> flightsData = Functions.selectFlights(driver);

            Functions.writeToCsv(flightsData);
        }
    }
}
