using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orange.Util
{
    public class ConvertTimespanToString
    {
        public static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0}:", span.Days) : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{00:00}:", span.Hours) : string.Format("00:"),
                span.Duration().Minutes > 0 ? string.Format("{00:00}:", span.Minutes) : string.Format("00:"),
                span.Duration().Seconds > 0 ? string.Format("{00:00}", span.Seconds) : string.Format("00"));

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "00:00";

            return formatted;
        }

        public static string ToReadableString(string span)
        {     
            
            int idx = 1;
            char[] carray = span.ToCharArray();

            if (carray.Length < 6)
                return span;

            char[] attr_array = { 'h', 'm' };

            for (int i = carray.Length-1; i >= 0; i-- )
            {
                if(carray[i].Equals(':'))
                {
                    if(idx > -1)
                        carray[i] = attr_array[idx--];
                }
            }

            return new string(carray);;
        }
    }
}
