using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsWorkday(this DateTime date)
        {
            return (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);
        }

        public static DateTime GetPreviousDate(this DateTime date, int daysToSubstract = 0)
        {
            return date.AddDays(daysToSubstract);
        }

        // Use of second external api check could be more efficient if run on separate thread and awaited result
        // before sending main currency rate request 
        public static bool NotHoliday(this DateTime date)
        {
            throw new NotImplementedException("Possible checking solutions: " +
                "1) either if we don't get 200 result from request to ECB [check only available after request response]," +
                "2) use some external api to check for holidays [ie. HolidayAPI https://holidayapi.com/docs]");
        }
    }
}
