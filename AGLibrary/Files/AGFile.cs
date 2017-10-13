using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace AGLibrary.Files
{
    
    public class AGFile
    {
        /// <summary>
        ///  Сохранить данные в формате JSON
        /// </summary>
        /// <typeparam name="T">Тип данных, который необходимо сохранить (Необходимые данные должны быть помечены атрибутом try catch </typeparam>
        /// <param name="data">Данные, которые необходимо сохранить</param>
        /// <param name="filePath">Путь до файла</param>
        public static void SaveDataJson<T>(T data, String filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Create))
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T)); // Создаём объект
                json.WriteObject(file, data); // Сохраняем объект
            }
        }
        /// <summary>
        /// Считывает данные из файла 
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <param name="data">Переменная, в которую сохранятся данные</param>
        /// <param name="filePath">Путь до файла </param>
        public static void ReadDataJson<T>(out T data, String filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
                data = (T)json.ReadObject(file);
            }
        }

        /// <summary>
        /// Сохраняет строку в файл
        /// </summary>
        /// <param name="data">Строка</param>
        /// <param name="filePath">Путь до файла</param>
        public static void SaveData(String data, String filePath)
        {
            using (StreamWriter file = new StreamWriter(new FileStream(filePath, FileMode.OpenOrCreate)))
            {
                file.WriteLine(data);
            }
        }   
        /// <summary>
        /// Считывает строку из файла
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <returns></returns>
        public static String ReadString(String filePath)
        {
            using (StreamReader file = new StreamReader(new FileStream(filePath, FileMode.Open)))
            {
                return file.ReadToEnd();
            }
        }
    }
}