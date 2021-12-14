using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.ComponentModel;

namespace Scraper
{
    public static class Functions
    {
        public static void searchFlights(IWebDriver driver, string departureAirport, string arrivalAirport, int daysTillDeparture, int daysTillReturn)
        {
            SelectElement selectCurrency = new SelectElement(driver.FindElement(By.Name("currency")));
            selectCurrency.SelectByValue("USD");

            SelectElement selectDepartAirport = new SelectElement(driver.FindElement(By.Id("depairportcode")));
            selectDepartAirport.SelectByValue(departureAirport);

            SelectElement selectArrivalAirport = new SelectElement(driver.FindElement(By.Id("arrvairportcode")));
            selectArrivalAirport.SelectByValue(arrivalAirport);

            DateTime departDate = DateTime.Now.AddDays(daysTillDeparture);
            DateTime returnDate = DateTime.Now.AddDays(daysTillReturn);

            //Departure date selection
            IWebElement departCalendarButton = driver.FindElement(By.CssSelector(".col-md-6.fromwrap"));
            departCalendarButton = departCalendarButton.FindElement(By.ClassName("ui-datepicker-trigger"));
            departCalendarButton.Click();
            //Departure date selection: Year
            IWebElement departYear = driver.FindElement(By.ClassName("ui-datepicker-year"));
            IWebElement nextMonthButton = driver.FindElement(By.ClassName("ui-datepicker-next"));
            while (departYear.Text != departDate.Year.ToString())
            {
                nextMonthButton.Click();
                departYear = driver.FindElement(By.ClassName("ui-datepicker-year"));
            }
            //Departure date selection: Month
            SelectElement selectMonth = new SelectElement(driver.FindElement(By.ClassName("ui-datepicker-month")));
            selectMonth.SelectByValue((departDate.Month - 1).ToString());
            //Departure date selection: Day
            IWebElement dayTable = driver.FindElement(By.ClassName("ui-datepicker-calendar"));
            IList<IWebElement> columns = dayTable.FindElements(By.TagName("td"));
            foreach (IWebElement cell in columns)
            {
                if (cell.Text.Equals(departDate.Day.ToString()))
                {
                    cell.FindElement(By.LinkText(departDate.Day.ToString())).Click();
                    break;
                }
            }

            //Return date selection
            IWebElement returnCalendarButton = driver.FindElement(By.CssSelector(".col-md-6.towrap"));
            returnCalendarButton = returnCalendarButton.FindElement(By.ClassName("ui-datepicker-trigger"));
            returnCalendarButton.Click();
            //Return date selection: Year
            IWebElement returnYear = driver.FindElement(By.ClassName("ui-datepicker-year"));
            nextMonthButton = driver.FindElement(By.ClassName("ui-datepicker-next"));
            while (returnYear.Text != returnDate.Year.ToString())
            {
                nextMonthButton.Click();
                returnYear = driver.FindElement(By.ClassName("ui-datepicker-year"));
            }
            //Return date selection: Month
            selectMonth = new SelectElement(driver.FindElement(By.ClassName("ui-datepicker-month")));
            selectMonth.SelectByValue((returnDate.Month - 1).ToString());
            //Return date selection: Day
            dayTable = driver.FindElement(By.ClassName("ui-datepicker-calendar"));
            columns = dayTable.FindElements(By.TagName("td"));
            foreach (IWebElement cell in columns)
            {
                if (cell.Text.Equals(returnDate.Day.ToString()))
                {
                    cell.FindElement(By.LinkText(returnDate.Day.ToString())).Click();
                    break;
                }
            }

            IWebElement searchFlightButton = driver.FindElement(By.Id("searchFlight"));
            searchFlightButton.Click();
        }

        public static List<FlightData> selectFlights(IWebDriver driver)
        {
            List<FlightData> flightsData = new();

            

            IWebElement departFlights = driver.FindElement(By.ClassName("fly5-depart"));
            departFlights = departFlights.FindElement(By.ClassName("fly5-results"));
            IList<IWebElement> departFlightsList = departFlights.FindElements(By.ClassName("fly5-result"));

            IWebElement returnFlights = driver.FindElement(By.ClassName("fly5-return"));
            returnFlights = returnFlights.FindElement(By.ClassName("fly5-results"));
            IList<IWebElement> returnFlightsList = returnFlights.FindElements(By.ClassName("fly5-result"));

            for (int i = 0; i < departFlightsList.Count; i++)
            {
                for (int j = 0; j < returnFlightsList.Count; j++)
                {
                    IWebElement departFlight = departFlightsList[i];
                    selectFlightClass(departFlight, driver);
                    IWebElement returnFlight = returnFlightsList[j];
                    selectFlightClass(returnFlight, driver);
                    IWebElement continueButton = driver.FindElement(By.Id("continue-btn"));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click()", continueButton);

                    flightsData.Add(scrapeFlightData(driver));
                    driver.Navigate().Back();
                }
            }

            return flightsData;
        }

        public static void selectFlightClass (IWebElement flight, IWebDriver driver)
        {
            IWebElement flightClasses = flight.FindElement(By.ClassName("flight-classes"));
            IList<IWebElement> flightClassesList = flightClasses.FindElements(By.ClassName("card"));
            foreach (IWebElement flightClass in flightClassesList)
            {
                if (!flightClass.FindElements(By.ClassName("greyed")).Any())
                {
                    IWebElement selectFlightButton = flightClass.FindElement(By.ClassName("select-flight"));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click()", selectFlightButton);
                    break;
                }
                else
                {
                    continue;
                }
            }
        }

        public static FlightData scrapeFlightData (IWebDriver driver)
        {
            IWebElement outSummary = driver.FindElement(By.ClassName("fly5-fout"));
            string outDepartAirport = outSummary.FindElement(By.ClassName("fly5-frshort")).Text;
            string outArrivalAirport = outSummary.FindElement(By.ClassName("fly5-toshort")).Text;
            IWebElement outSummaryDepartTime = outSummary.FindElement(By.ClassName("fly5-timeout"));
            string outDepartTime = (outSummaryDepartTime.FindElement(By.ClassName("fly5-fdate")).Text + " " + outSummaryDepartTime.FindElement(By.ClassName("fly5-ftime")).Text).Replace("," , "");
            IWebElement outSummaryArrivalTime = outSummary.FindElement(By.ClassName("fly5-timein"));
            string outArrivalTime = (outSummaryArrivalTime.FindElement(By.ClassName("fly5-fdate")).Text + " " + outSummaryArrivalTime.FindElement(By.ClassName("fly5-ftime")).Text).Replace(",", "");

            IWebElement inSummary = driver.FindElement(By.ClassName("fly5-fin"));
            string inDepartAirport = inSummary.FindElement(By.ClassName("fly5-frshort")).Text;
            string inArrivalAirport = inSummary.FindElement(By.ClassName("fly5-toshort")).Text;
            IWebElement inSummaryDepartTime = inSummary.FindElement(By.ClassName("fly5-timeout"));
            string inDepartTime = (inSummaryDepartTime.FindElement(By.ClassName("fly5-fdate")).Text + " " + inSummaryDepartTime.FindElement(By.ClassName("fly5-ftime")).Text).Replace(",", "");
            IWebElement inSummaryArrivalTime = inSummary.FindElement(By.ClassName("fly5-timein"));
            string inArrivalTime = (inSummaryArrivalTime.FindElement(By.ClassName("fly5-fdate")).Text + " " + inSummaryArrivalTime.FindElement(By.ClassName("fly5-ftime")).Text).Replace(",", "");

            Decimal totalPrice = Decimal.Parse(driver.FindElement(By.ClassName("fly5-price")).Text);

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            IWebElement taxesOutElement = driver.FindElement(By.XPath("//*[@id='breakdown']/div/div[1]/div[2]/span"));
            string taxesOut = js.ExecuteScript("return arguments[0].innerHTML;", taxesOutElement).ToString();
            IWebElement taxesInElement = driver.FindElement(By.XPath("//*[@id='breakdown']/div/div[2]/div[2]/span"));
            string taxesIn = js.ExecuteScript("return arguments[0].innerHTML;", taxesInElement).ToString();
            Decimal taxes = Decimal.Parse(taxesOut) + Decimal.Parse(taxesIn);

            FlightData flightData = new FlightData(outDepartAirport, outArrivalAirport, outDepartTime, outArrivalTime, inDepartAirport, inArrivalAirport, inDepartTime, inArrivalTime, totalPrice, taxes);

            return flightData;
        }

        public static void writeToCsv (List<FlightData> flightsData)
        {
            var lines = new List<string>();
            IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(FlightData)).OfType<PropertyDescriptor>();
            var header = string.Join(";", props.ToList().Select(x => x.Name));
            lines.Add(header);
            var valueLines = flightsData.Select(row => string.Join(";", header.Split(';').Select(a => row.GetType().GetProperty(a).GetValue(row, null))));
            lines.AddRange(valueLines);
            File.WriteAllLines("flightsData.csv", lines.ToArray());
        }
    }
}
