using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebStore.Domain
{
    public static class AgeHelper
    {
        private static int TotalMonthes(DateTime date)
        {
            return date.Year * 12 + (date.Month - 1);
        }

        public static int AgeFromBirthDate(DateTime birthDate)
        {
            var today = DateTime.Today;
            int extra = 1;
            if (today.Month == birthDate.Month && today.Day >= birthDate.Day)
                extra = 0;

            return (TotalMonthes(DateTime.Today) - TotalMonthes(birthDate) - extra) / 12;
        }
    }
}
