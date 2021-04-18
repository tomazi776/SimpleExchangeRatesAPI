using DataLibrary.Constants;
using DataLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Helpers
{
    public static class DateTimeHelper
    {
        public static string YearMonthDayDashedFormat = $"yyyy{StringConstants.EnDash}MM{StringConstants.EnDash}dd";
        public static string YearMonthDayUnderscoredFormat = $"yyyy{StringConstants.Underscore}MM{StringConstants.Underscore}dd";
        public static readonly bool isTimeEcbDataAvailable = DateTime.Now.Hour >= 16;

        public static List<string> CreateWorkDatesFrom(List<DateTime> dates)
        {
            return ConvertToString(GetWorkDates(dates[0], dates[1]));
        }

        public static DateTime GetLastAvailableECB(DateTime today)
        {
            DateTime availDate;
            //first check if weekend so to get supposedly workDay
            if (!today.IsWorkday())
            {
                availDate = GetLastWorkdayInstead(today);
            }
            // today is supposedly workday so check for time if data available
            // TODO: add check for european holidays
            else
            {
                availDate = isTimeEcbDataAvailable ? today : today.GetPreviousDate(-1);
            }
            return availDate;
        }

        private static DateTime GetLastWorkdayInstead(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return date.GetPreviousDate(-2);
                case DayOfWeek.Saturday:
                    return date.GetPreviousDate(-1);
                default:
                    return date.GetPreviousDate(0);
            }
        }

        private static List<DateTime> GetWorkDates(DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.IsWorkday())
                {
                    dates.Add(date);
                }
            }
            return dates;
        }

        private static List<string> ConvertToString(List<DateTime> inputDates)
        {
            List<string> dates = new List<string>();
            foreach (var date in inputDates)
            {
                dates.Add(date.ToString(YearMonthDayDashedFormat));
            }
            return dates;
        }
    }
}
