using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AGLibrary.Coder
{
    public class IEEE754Coder : ICoder
    {
        private int _expCount = 8; // Количество знаков под экспоненту (For double = 11)
        private int _mantissCount = 23; // Количество знаков под мантиссу (For double = 52)

        /// <summary>
        /// Можно вручную изменит
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="mantiss"></param>
        public void SetParametres(int exp, int mantiss)
        {
            _expCount = exp;
            _mantissCount = mantiss;
        }

        public void SetParametresDouble()
        {
            _expCount = 11;
            _mantissCount = 52;
        }
        public void SetParametressFloat()
        {
            _expCount = 8;
            _mantissCount = 23;
        }


        /// <summary>
        /// Переводит число в формат IEEE754
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public String Code(double num)
        {
            double number = num; // Сохраняем число
            String sing = ToBinarySign(num); // Находим знак числа 

            number = Math.Abs(number); // Избавляемся от знака

            String whole = ""; // Целая часть числа

            if (Math.Truncate(number) != 0) // Если целая часть != 0
            {
                whole = new BinaryCoder().Code(Math.Truncate(number)); // Переводим целую часть числа
                whole = GetWriteMantiss(whole);
            }

            String frac = ToBinaryFrac(number - Math.Truncate(number), whole); // Переводим дробную часть числа
            String manitss = whole + frac;
            while (manitss.Length < _mantissCount) manitss = "0" + manitss;

            // Если целая часть равна нулю, то мантисса записана неправильно, переписываем
            if(Math.Truncate(number) == 0) manitss =  GetWriteMantiss(manitss); // Сохраняем записываему мантиссу

            int shift = GetShift(num, whole, frac); // Считаем сдвиг мантиссы
            String exp = ToBinaryExp(shift); // Находим экспоненту 

            return sing + exp + manitss;
        }

        /// <summary>
        /// Раскодирует число из формата IEEE754
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public double DeCode(string f)
        {
            double s, E, M;
            string temp = "";
            s = Double.Parse(Convert.ToString(f[0]));

            temp = f.Substring(1, _expCount);
            E = ToNormalBin(temp);

            temp = f.Substring(_expCount + 1);
            M = ToNormalBin(temp);

            double b = Math.Pow(2, _expCount - 1) - 1;
            return Math.Pow(-1, s) * Math.Pow(2, (E - b)) * (1 + M / Math.Pow(2, _mantissCount));
        }

        /// <summary>
        /// Определяет первый бит числа
        /// </summary>
        /// <param name="num">Кодируемое число</param>
        /// <returns></returns>
        private string ToBinarySign(double num)
        {
            if (num < 0) return "1";
            else return "0";
        }

        /// <summary>
        /// Переводим целую часть числа в двоичную систему
        /// </summary>
        /// <param name="num">Кодируемое число</param>
        /// <returns></returns>
        //private string ToBinaryInt(double num)
        //{
        //    long numInt = Convert.ToInt64(num);
        //    string str = "";

        //    while (numInt >= 2)
        //    {
        //        str = numInt % 2 + str;
        //        numInt /= 2;
        //    }
        //    str = numInt + str;

        //    return str;
        //}

        /// <summary>
        /// Получить записываемую мантиссу
        /// </summary>
        /// <param name="wh"></param>
        /// <returns></returns>
        private String GetWriteMantiss(String wh)
        {
            String k = "";

            IEnumerable<char> chars = wh.SkipWhile(n => n == '0'); // т.к. мантисса начинается с 1
            foreach (char ch in chars) k += ch; // Сохраняем строку 

            if (k != "") k = k.Remove(0, 1); // Удаляем первую единицу, так как записывается не с неё

            return k; // Возвращаем записываемую мантиссу            
        }

        /// <summary>
        /// Переводим дробную часть числа в двоичную систему
        /// </summary>
        /// <param name="num">Кодируемое число</param>
        /// <param name="wh">Целая часть чила в кодируемом числе (уже в записываемом виде)</param>
        private string ToBinaryFrac(double num, String wh)
        {
            String str = "";
            double k = num;

            while (str.Length + wh.Length < _mantissCount)
            {
                k = k * 2;

                if (k >= 1) str += "1";
                else str += "0";

                k = k - Math.Truncate(k);
            }

            return str;
        }

        private int GetShift(double num, String wh, String frac)
        {
            int shift; // Сдвиг мантиссы
            if (Math.Truncate(num) != 0) shift = wh.Length;
            else shift = (-1) * (frac.TakeWhile(x => x == '0').Count() + 1);

            return shift;
        }

        /// <summary>
        /// Находим экспоненту
        /// </summary>
        /// <param name="wh"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private string ToBinaryExp(int num)
        {
            // http://alexanderkobelev.blogspot.ru/2013/05/ieee-754.html?m=1

            String expS = "";

            int shift = Convert.ToInt32(Math.Pow(2, _expCount-1) + (num) - 1);

            expS = new BinaryCoder().Code(shift);

            while (expS.Length < _expCount)
            {
                expS = "0" + expS;
            }

            return expS;
        }

        // Переводим мантиссу в обычную часть
        public double ToNormalBin(string code)
        {
            double ch = 0;
            int idex = 0;
            double Df;

            for (int i = 0; i < code.Length; i++)
            {
                Df = Double.Parse(code[i].ToString());
                idex = code.Length - 1 - i;
                ch += Df * Math.Pow(2, idex);
            }
            return ch;
        }
    }
}