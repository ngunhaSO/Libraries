using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    public static class DateTimeExtension
    {
        //========================================//
        //                                        //
        //          Elapseds the time             //
        //                                        //
        //========================================//
        public static TimeSpan Elapsed(this DateTime datetime)
        {
            return DateTime.Now - datetime;
        }

        //==================================================//
        //                                                  //
        //     Get the week of a specific date of a year    //
        //                                                  //
        //==================================================//
        public static int WeekOfYear(this DateTime datetime, CalendarWeekRule weekrule, DayOfWeek firstDayOfWeek)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture; //get current system setting of calendar being used
            return ciCurr.Calendar.GetWeekOfYear(datetime, weekrule, firstDayOfWeek);
        }

        public static int WeekOfYear(this DateTime datetime, DayOfWeek firstDayOfWeek)
        {
            DateTimeFormatInfo dateinf = new DateTimeFormatInfo();
            CalendarWeekRule weekrule = dateinf.CalendarWeekRule;
            return WeekOfYear(datetime, weekrule, firstDayOfWeek);
        }

        public static int WeekOfYear(this DateTime datetime, System.Globalization.CalendarWeekRule weekrule)
        {
            DateTimeFormatInfo dateinf = new DateTimeFormatInfo();
            DayOfWeek firstDayOfWeek = dateinf.FirstDayOfWeek;
            return WeekOfYear(datetime, weekrule, firstDayOfWeek);
        }

        public static int WeekOfYear(this DateTime datetime)
        {
            DateTimeFormatInfo dateinf = new System.Globalization.DateTimeFormatInfo();
            CalendarWeekRule weekrule = dateinf.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = dateinf.FirstDayOfWeek;
            return WeekOfYear(datetime, weekrule, firstDayOfWeek);
        }

        //========================================//
        //                                        //
        //       Get date time for day of week    //
        //                                        //
        //========================================//
        public static DateTime GetDateTimeForDayOfWeek(this DateTime datetime, DayOfWeek day, DayOfWeek firstDayOfWeek)
        {
            int current = DaysFromFirstDayOfWeek(datetime.DayOfWeek, firstDayOfWeek);
            int resultday = DaysFromFirstDayOfWeek(day, firstDayOfWeek);
            return datetime.AddDays(resultday - current);
        }
        public static DateTime GetDateTimeForDayOfWeek(this DateTime datetime, DayOfWeek day)
        {
            System.Globalization.DateTimeFormatInfo dateinf = new System.Globalization.DateTimeFormatInfo();
            DayOfWeek firstDayOfWeek = dateinf.FirstDayOfWeek;
            return GetDateTimeForDayOfWeek(datetime, day, firstDayOfWeek);
        }

        public static DateTime FirstDateTimeOfWeek(this DateTime datetime)
        {
            System.Globalization.DateTimeFormatInfo dateinf = new System.Globalization.DateTimeFormatInfo();
            DayOfWeek firstDayOfWeek = dateinf.FirstDayOfWeek;
            return FirstDateTimeOfWeek(datetime, firstDayOfWeek);
        }

        public static DateTime FirstDateTimeOfWeek(this DateTime datetime, DayOfWeek firstDayOfWeek)
        {
            return datetime.AddDays(-DaysFromFirstDayOfWeek(datetime.DayOfWeek, firstDayOfWeek));
        }

        private static int DaysFromFirstDayOfWeek(DayOfWeek current, DayOfWeek firstDayOfWeek)
        {
            //Sunday = 0,Monday = 1,...,Saturday = 6
            int daysbetween = current - firstDayOfWeek;
            if (daysbetween < 0) daysbetween = 7 + daysbetween;
            return daysbetween;
        }

        public static string GetValueOrDefaultToString(this DateTime? datetime, string defaultvalue)
        {
            if (datetime == null) return defaultvalue;
            return datetime.Value.ToString();
        }

        public static string GetValueOrDefaultToString(this DateTime? datetime, string format, string defaultvalue)
        {
            if (datetime == null) return defaultvalue;
            return datetime.Value.ToString(format);
        }

    }
}
