using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AGLibrary.Vector
{
    [DataContract]
    // Класс матрица
    public class Matrix<T>
    {
        [DataMember]
        private T[,] _numbers; // Цифирки 
        public int Width => _numbers.GetLength(0);
        public int Height => _numbers.GetLength(1);

        public void InitNull()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _numbers[i, j] = default(T);
                }
            }
        }
        public void AllInit(T num)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _numbers[i, j] = num;
                }
            }
        }

        public Matrix(int h, int w)
        {
            _numbers = new T[w, h]; // Выделяем память 
            InitNull();
        }
        public Matrix(int n)
        {
            _numbers = new T[n, n];
            InitNull();
        }
        public Matrix(T[,] matrix) => _numbers = (T[,])matrix.Clone();

        public Vector<T> this[int i]
        {
            get
            {
                List<T> list = new List<T>();
                for (int j = 0; j < Width; j++)
                {
                    list.Add(_numbers[i, j]);
                }
                return new Vector<T>(list);
            }
        }
        public T this[int i, int j]
        {
            get => _numbers[i, j];
            set => _numbers[i, j] = value;
        }
    }
}