using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer.Utils
{
    public static class Extensions
    {
        public static int? ToInt(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;
            else
            {
                if (int.TryParse(s, out int result))
                    return result;
                else
                    return null;
            }
        }
    }
}
