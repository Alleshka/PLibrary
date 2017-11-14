using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AGLibrary.Vector;

namespace AGLibrary.Coder
{
    /// <summary>
    /// Целочисленное кодирование
    /// </summary>
    public class IntCoder : ICoder
    {
        private double _min; // Нижняя граница 
        private double _max; // Верхняя граница
        private int _count; // Количество разрядов

        public void InitAmbit(double a, double b, int c)
        {
            _min = a;
            _max = b;
            _count = c;
        }

        public IntCoder()
        {
            InitAmbit(0, 10, 10);
        }
        public IntCoder(double a, double b, int c)
        {
            InitAmbit(a, b, c);
        }

        public string Code(double num)
        {
            int g = (int)((num - _min) * (Math.Pow(2, _count) - 1) / (_max - _min)); // Вычисляем штуку
            String code = new BinaryCoder().Code(g); // Кодируем в двоичный вид
            return code;
        }

        public double DeCode(string num)
        {
            double temp = new BinaryCoder().DeCode(num);
            return temp * (_max - _min) / (Math.Pow(2, _count) - 1) + _min;
        }
    }
}