using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace AGLibrary.Vector
{
    [DataContract]
    // Класс вектор
    public class Vector<T> : IEnumerable
    {
        // Пригодится дял печати вектора 
        public delegate void print(Vector<T> vector);

        [DataMember]
        private T[] _numbers; // Содержимое вектора 
        // Размер вектора
        public int Lengh
        {
            get => _numbers.Length;
        }

        /// <summary>
        /// Создаёт вектор с указанным размером и инициализирует по умолчанию
        /// </summary>
        /// <param name="n">Размер вектора</param>
        public Vector(int n)
        {
            _numbers = new T[n];
            NullInit();
        }
        /// <summary>
        /// Инициализация вектора входными последовательностями
        /// </summary>
        public Vector(T[] vector) => _numbers = (T[])vector.Clone();
        public Vector(List<T> vector) => _numbers = (T[])vector.ToArray().Clone();
        public Vector(Vector<double> vector) => _numbers = (T[])vector._numbers.Clone();

        /// Устанавливаем значение по умолчанию
        public void NullInit()
        {
            for (int i = 0; i < _numbers.Length; i++) _numbers[i] = default(T);
        }
        // Настраиваем индексатор
        public T this[int index]
        {
            get => _numbers[index];
            set => _numbers[index] = value;
        }

        // Печать вектора
        public void Print(print _del)
        {
            if (_del != null)
            {
                _del.Invoke(this);
            }
            else throw new Exception("Неверная ссылка на делегат");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _numbers.GetEnumerator();
        }
        // Перегруженные операторы 
        public static Vector<T> operator +(Vector<T> a, Vector<T> b)
        {
            if (a.Lengh == b.Lengh)
            {
                Vector<T> temp = new Vector<T>(a.Lengh);
                for (int i = 0; i < a.Lengh; i++)
                {
                    temp[i] = (T)Convert.ChangeType((dynamic)a[i] + (dynamic)b[i], typeof(T));
                }
                return temp;
            }
            else
            {
                throw new Exception("Разный размер векторов");
            }
        }
        public static Vector<T> operator -(Vector<T> a, Vector<T> b)
        {
            if (a.Lengh == b.Lengh)
            {
                Vector<T> temp = new Vector<T>(a.Lengh);
                for (int i = 0; i < a.Lengh; i++)
                {
                    temp[i] = (T)Convert.ChangeType((dynamic)a[i] - (dynamic)b[i], typeof(T));
                }
                return temp;
            }
            else
            {
                throw new Exception("Разный размер векторов");
            }
        }
        public static T operator *(Vector<T> a, Vector<T> b)
        {
            if (a.Lengh == b.Lengh)
            {
                T sum = default(T);

                for (int i = 0; i < a.Lengh; i++)
                {
                    sum = (T)Convert.ChangeType((dynamic)sum + (dynamic)a[i] * (dynamic)b[i], typeof(T));
                }

                return sum;
            }
            else
            {
                throw new Exception("Разный размер векторов");
            }
        }
        public static Vector<T> operator *(Vector<T> a, T b)
        {
            Vector<T> temp = new Vector<T>(a.Lengh);

            for (int i = 0; i < a.Lengh; i++)
            {
                temp[i] = (T)Convert.ChangeType((dynamic)a[i] * (dynamic)b, typeof(T));
            }

            return temp;
        }
        public static Vector<T> operator *(T b, Vector<T> a)
        {
            Vector<T> temp = new Vector<T>(a.Lengh);

            for (int i = 0; i < a.Lengh; i++)
            {
                temp[i] = (T)Convert.ChangeType((dynamic)a[i] * (dynamic)b, typeof(T));
            }

            return temp;
        }
    }
}
