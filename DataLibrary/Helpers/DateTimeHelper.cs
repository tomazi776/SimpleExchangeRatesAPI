using DataLibrary.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Helpers
{
    public static class DateTimeHelper
    {
        public static string YearMonthDayDashedFormat = $"yyyy{StringConstants.EnDash}MM{StringConstants.EnDash}dd";
        public static string YearMonthDayUnderscoredFormat = $"yyyy{StringConstants.Underscore}MM{StringConstants.Underscore}dd";
        
        public static DateTime GetModify(DateTime endDate)
        {
            return (endDate.Day == DateTime.Now.Day) ? AssignAvailableDate(endDate) : endDate;
        }

        public static List<string> CreateRange(List<DateTime> dates)
        {
            var datesRange = CreateRangeForWorkingDays(dates[0], dates[1]);
            return FormatDatesToStrings(datesRange);
        }

        private static DateTime AssignAvailableDate(DateTime endDate)
        {
            DateTime output;
            // ECB data available
            if (DateTime.Now.Hour >= 16)
            {
                output = endDate;
            }
            else
            {
                var dayNum = DateTime.Now.Day;
                output = new DateTime(DateTime.Now.Year, DateTime.Now.Month, --dayNum);
            }
            return output;
        }

        private static List<DateTime> CreateRangeForWorkingDays(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    allDates.Add(date);
                }
            }
            return allDates;
        }

        private static List<string> FormatDatesToStrings(List<DateTime> allDates)
        {
            List<string> allDatesInStrings = new List<string>();
            foreach (var date in allDates)
            {
                var dateInString = date.ToString(YearMonthDayUnderscoredFormat);
                allDatesInStrings.Add(dateInString);
            }
            return allDatesInStrings;
        }
    }
}
