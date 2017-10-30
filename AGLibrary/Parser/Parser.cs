using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AGLibrary.Parser
{
    using AGLibrary.Vector;

    public class Parser
    {
        private List<String> _varList; // Список неизвестных переменных 
        private List<String> _pollandList; // Польская запись 
        private Stack<String> _stack;
        private String _workFunc;
        private Dictionary<String, double> _args;

        Regex _regNumb = new Regex(@"[0-9]"); // Для проверки на число 
        String _operators = $"+-*:^"; // Операторы
        String _functions = "e"; // exp, sin, cos, 

        private void Init(String _func)
        {
            _workFunc = _func;
            _stack = new Stack<string>();
            _pollandList = new List<string>();
            _args = new Dictionary<string, double>();
            _varList = new List<string>();

            FuncAdapt();
        }

        public double Parse(String _func)
        {
            Init(_func);
            if (SearchVar() > 0) throw new Exception("Не переданы параметры переменных");
            else
            {
                ToPollandList();
                return Calculate();
            }
        }
        public double Parse(String _func, Vector<double> vector)
        {
            Init(_func);
            if (SearchVar() > vector.Lengh) throw new Exception($"Переданы не все параметры переменных. {Environment.NewLine}" +
                   $"Переменных в функции: {SearchVar()} {Environment.NewLine}" +
                   $"Передано значений: {vector.Lengh}");
            else
            {
                ToPollandList();
                for (int i = 0; i < _varList.Count; i++)
                {
                    _args.Add(_varList[i], vector[i]);
                }

                return Calculate();
            }
        }
        public double Parse(String _func, double[] vector)
        {
            return Parse(_func, new Vector<double>(vector));
        }
        public double Parse(String _func, List<double> vector)
        {
            return Parse(_func, new Vector<double>(vector));
        }

        public String GetPollandList()
        {
            String temp = "";

            foreach (String tmp in _pollandList)
            {
                temp += tmp + " ";
            }
            return temp;
        }

        // Переводит функцию в удобный вид
        private void FuncAdapt()
        {
            _workFunc = _workFunc.ToLower(); // Переводим всё в нижний регистр 

            _workFunc = _workFunc.Replace(" ", ""); // Удаляем все пробелы 
            _workFunc = _workFunc.Replace(",", "."); // Заменяем все запятые 
            _workFunc = _workFunc.Replace("\\", ":");
            _workFunc = _workFunc.Replace("/", ":");
            _workFunc = _workFunc.Replace("exp", "e");
        }
        private int SearchVar()
        {
            Regex reg = new Regex(@"[X|x]{1,}\d{1,}"); // Будем искать неизвестные в формуле. Не более 10 переменных.
            foreach (Match t in reg.Matches(_workFunc))
            {
                _varList.Add(t.Value); // Добавляем найденную неизвестную
            }
            _varList = _varList.Distinct().OrderBy(x => x).ToList(); // Удаляем повторы и сортируем. Теперь тут хранится число неизвестных             

            return _varList.Count;
        }

        private void ToPollandList()
        {
            // Пока есть символы для чтения
            while (_workFunc.Length != 0)
            {
                // DebugConsole();

                if (!CheckNumber()) // Если не число
                {
                    if (!CheckFunct())
                    {
                        if (!CheckOpenBracket()) // Если не открывающая скобка
                        {
                            if (!CheckCloseBracket()) // Если не закрывающая скобка
                            {
                                if (!CheckOperator()) // Если не оператор
                                {
                                    throw new Exception("Неизвестный символ: " + FirstSyb);
                                }
                            }
                        }
                    }
                }
            }

            // Когда входная строка закончилась, выталкиваем все символы из стека в выходную строку.
            while (_stack.Count != 0)
            {
                _pollandList.Add(_stack.Pop());
            }
        }
        private double Calculate()
        {
            _stack = new Stack<string>();
            _stack.Push("0");


            while (_pollandList.Count() != 0)
            {
                String curSyb = _pollandList.First();

                // Если число или X
                if ((_regNumb.Matches(curSyb).Count != 0) || (_varList.Any(x => x == curSyb)))
                {
                    _stack.Push(curSyb); //  помещается на вершину стека.
                }
                else // Если операция
                {
                    ActionOperate(curSyb);
                }

                _pollandList.RemoveAt(0);
            }

            return Convert.ToDouble(_stack.Peek());
        }

        // Перевод в польскую запись
        /// Проверяем является ли символ числом 
        private bool CheckNumber()
        {
            if ((FirstSyb == "x") || (FirstNumb())) // Если X или число
            {
                String syb = "";

                syb += FirstSyb; // Сохраняем
                _workFunc = _workFunc.Remove(0, 1);// Удаляем из строки

                if (_workFunc.Length != 0) syb = ReadWhileNumber(syb);
                _pollandList.Add(syb); // Добавляем неизвестную в выходной список

                return true;
            }
            else return false;
        }
        private bool CheckOpenBracket()
        {
            if (FirstSyb == "(")
            {
                _stack.Push("(");
                _workFunc = _workFunc.Remove(0, 1);
                return true;
            }
            else return false;
        }
        private bool CheckCloseBracket()
        {
            if (FirstSyb == ")")
            {
                while (_stack.Peek() != "(") // до тех пор, пока верхним элементом стека не станет открывающая скобка, выталкиваем элементы из стека в выходную строку.
                {
                    _pollandList.Add(_stack.Pop());
                }

                _stack.Pop();
                _workFunc = _workFunc.Remove(0, 1); // Удаляем
                return true;
            }
            else return false;
        }
        private bool CheckFunct()
        {
            if (_functions.Contains(FirstSyb))
            {
                _stack.Push(FirstSyb);
                _workFunc = _workFunc.Remove(0, 1); // Удаляем
                return true;
            }
            else return false;
        }
        private bool CheckOperator()
        {
            if (_operators.Contains(FirstSyb)) // Если первый символ оператор
            {
                if (_stack.Count > 0)
                {
                    while ((GetPrior(FirstSyb) <= GetPrior(_stack.Peek()))) // приоритет o1 меньше либо равен приоритету оператора, находящегося на вершине стека…
                    {
                        _pollandList.Add(_stack.Pop()); // … выталкиваем верхний элемент стека в выходную строку;
                        if (_stack.Count == 0) break;
                    }
                }
                _stack.Push(FirstSyb); // помещаем оператор o1 в стек.
                _workFunc = _workFunc.Remove(0, 1);
                return true;
            }
            else return false;
        }
        private int GetPrior(String syb)
        {
            switch (syb)
            {
                case "(": return 0;
                case ")": return 0;
                case "+": return 1;
                case "-": return 1;
                case "*": return 2;
                case ":": return 2;
                case "^": return 3;

            }
            return 0;
        }
        private String ReadWhileNumber(String syb)
        {
            while (FirstNumb() && (_workFunc.Length != 0)) // Пока считываем числа
            {
                syb += FirstSyb;
                _workFunc = _workFunc.Remove(0, 1); // Удаляем символ
            }
            return syb;
        }
        private bool FirstNumb()
        {
            if (_workFunc.Length == 0) return false;
            if (_regNumb.Matches(FirstSyb).Count != 0) return true;
            else return false;
        }
        private String FirstSyb => Convert.ToString(_workFunc[0]);

        private void ActionOperate(String curSyb)
        {
            double a, b;

            if (_stack.Peek().Contains("x"))
            {
                String f = _stack.Pop();
                b = _args[f];
            }
            else b = Convert.ToDouble(_stack.Pop());

            if (_stack.Peek().Contains("x"))
            {
                String f = _stack.Pop();
                a = _args[f];
            }
            else a = Convert.ToDouble(_stack.Pop());

            switch (curSyb)
            {
                case "+":
                    {
                        _stack.Push(Convert.ToString(a + b));
                        break;
                    }
                case "-":
                    {
                        _stack.Push(Convert.ToString(a - b));
                        break;
                    }
                case "*":
                    {
                        _stack.Push(Convert.ToString(a * b));
                        break;
                    }
                case ":":
                    {
                        _stack.Push(Convert.ToString(a / b));
                        break;
                    }
                case "^":
                    {
                        _stack.Push(Convert.ToString(Math.Pow(a, b)));
                        break;
                    }
                default: throw new Exception("Неизвестная операция: " + curSyb);
            }
        }

        private void DebugConsole()
        {
            Console.WriteLine("________________________________________________");
            Console.WriteLine("Строка: " + _workFunc);
            Console.WriteLine("Читаем: " + FirstSyb);
            Console.WriteLine("Выход: " + GetPollandList());
            Console.Write("Стек: ");
            foreach (var k in _stack) Console.Write(k + "");
            Console.WriteLine();
        }
    }
}