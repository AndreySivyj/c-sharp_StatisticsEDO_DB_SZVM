using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace StatisticsEDO_DB
{
    #region считанная из файла информация

    public class DataFromStatusIDfile
    {
        public string statusID;

        public string statusName;

        public DataFromStatusIDfile(string statusID = "", string statusName = "")
        {
            this.statusID = statusID;
            this.statusName = statusName;
        }


        public override string ToString()
        {
            return statusID + ";" + statusName + ";";
        }
    } 



    #endregion

    //------------------------------------------------------------------------------------------
    #region Выбор данных из файла
    static class SelectDataFromStatusIDfile
    {
        //количество файлов в каталоге "_In"
        private static int countFile;

        //Маска поиска файлов
        private static string fileSearchMask = "*.csv";

        public static Dictionary<string, string> dictionaryStatusID = new Dictionary<string, string>();           //Коллекция данных из файлов   <statusID, statusName>

        //private static bool findError = false;      //обнуляем признак наличия ошибочных файлов

        //------------------------------------------------------------------------------------------
        //Открываем поток для чтения из файла и выбираем нужные позиции           
        private static void ReadAndParseTextFile(string openFile)
        {
            try
            {
                using (StreamReader reader = new StreamReader(openFile, Encoding.GetEncoding(1251)))
                {
                    while (!reader.EndOfStream)
                    {
                        //DataFromPersoFile tmpDataFromFile;

                        //Очищаем объект для данных из строки для каждой итерации цикла                    

                        string strTmp = reader.ReadLine();

                        char[] separator = { ';', ',' };    //список разделителей в строке
                        string[] massiveStr = strTmp.Split(separator);     //создаем массив из строк между разделителями

                        if (massiveStr.Count() == 2 && massiveStr[0] != "" && massiveStr[0] != " ")
                        {
                            dictionaryStatusID[massiveStr[0]] = massiveStr[1];                            
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (dictionaryStatusID.Count() == 0)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Внимание! Нет файла-справочника в каталоге \"_In_Status_ID\".");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }



        //------------------------------------------------------------------------------------------        
        /// <summary>
        /// Выбираем данные из файлов
        /// </summary>
        /// <param name="folderIn">Каталог с обрабатываемыми файлами</param>
        public static void ObrFileFromDirectory(string folderIn)
        {
            //Очищаем коллекции для данных из файлов
            dictionaryStatusID.Clear();

            DirectoryInfo dirInfo = new DirectoryInfo(folderIn);

            try
            {
                //Вычисляем количество считанных файлов
                countFile = dirInfo.GetFiles(fileSearchMask).Count();
                //Console.WriteLine("Количество файлов для обработки: " + countFile);

            }
            catch (DirectoryNotFoundException ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                --countFile;
                Console.WriteLine("Каталог с документами не доступен.");
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }


            if (countFile == 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Нет файлов для обработки.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {

                try
                {
                    //обрабатываем каждый файл по отдельности
                    foreach (FileInfo file in dirInfo.GetFiles(fileSearchMask))
                    {
                        //Открываем поток для чтения из файла и выбираем нужные позиции   
                        ReadAndParseTextFile(file.FullName);
                    }
                }
                catch (Exception ex)
                {
                    IOoperations.WriteLogError(ex.ToString());

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }
    }

    #endregion

}
