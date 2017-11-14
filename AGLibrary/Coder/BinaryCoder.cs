using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AGLibrary.Coder
{
    /// <summary>
    /// Двоичный кодер. Работает только с целыми числами
    /// </summary>
    public class BinaryCoder : ICoder
    {
        public string Code(double num)
        {
            num = Math.Truncate(num);
            long numInt = Convert.ToInt64((long)num);
            string str = "";

            while (numInt >= 2)
            {
                str = numInt % 2 + str;
                numInt /= 2;
            }
            str = numInt + str;

            return str;
        }

        public double DeCode(string num)
        {
            double k = 0;

            for (int i = 0; i < num.Length; i++)
            {
                if (num[i] == '1') k += Math.Pow(2, num.Length - 1 - i);
            }

            return k;
        }
    }
}