using System;
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

            List<FlightData> flightsData1 = Functions.selectFlights(driver);

            string fileName = "flightsData.csv";
            Functions.writeToCsv(flightsData1, fileName);


            driver.Url = "https://www.fly540.com";

            daysTillDeparture = 20;
            daysTillReturn = daysTillDeparture + tripDuration;

            Functions.searchFlights(driver, departureAirport, arrivalAirport, daysTillDeparture, daysTillReturn);

            List<FlightData> flightsData2 = Functions.selectFlights(driver);

            Functions.appendToCsv(flightsData2, fileName);
        }
    }
}
