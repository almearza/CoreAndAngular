using System;

namespace CoreAndAngular.API.Extensions
{
    public static class DateTimeExtension
    {
        public static int CalculateAge(this DateTime bod){
            var today = DateTime.Now;
            var age = today.Year-bod.Year;
            if(bod.Date>today.AddYears(age).Date)age--;
            return age;
        }
    }
}