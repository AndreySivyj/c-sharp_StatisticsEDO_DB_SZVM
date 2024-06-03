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

    public class DataFromUPfile
    {
        public string regNumStrah;

        public string regNumUP;

        public DataFromUPfile(string regNumStrah = "", string regNumUP = "")
        {
            this.regNumStrah = regNumStrah;
            this.regNumUP = regNumUP;
        }


        public override string ToString()
        {
            return regNumStrah + ";" + regNumUP + ";";
        }
    }

    /*
     * 
    public class DataFromUPfile
    {
        public string regNumStrah;

        private List<string> list_regNumUP = new List<string>();

        public string RegNumUP
        {
            get
            {
                string tmpSTR = "";
                foreach (var item in list_regNumUP)
                {
                    tmpSTR = tmpSTR + item + "|";
                }
                return tmpSTR;
            }
            set
            {
                list_regNumUP.Add(value);
            }

        }

        public DataFromUPfile(string regNumStrah = "", string regNumUP = "")
        {
            this.regNumStrah = regNumStrah;
            this.RegNumUP = regNumUP;
        }


        public override string ToString()
        {
            return regNumStrah + ";" + RegNumUP + ";";
        }
    }

    */




    #endregion

    //------------------------------------------------------------------------------------------
    #region Выбор данных из файла
    static class SelectDataFromUPfile
    {
        //количество файлов в каталоге "_In"
        private static int countFile;

        //Маска поиска файлов
        private static string fileSearchMask = "*.csv";

        public static List<DataFromUPfile> list_UP = new List<DataFromUPfile>();           //Коллекция данных из файлов с УП

        private static bool findError = false;      //обнуляем признак наличия ошибочных файлов

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

                        if (
                            //massiveStr.Count() == 2 && massiveStr[0] != "" && massiveStr[0] != " "  && massiveStr[0].Count() == 14 && massiveStr[1].Count() == 14)  
                            massiveStr.Count() == 3 && massiveStr[0] != "" && massiveStr[0] != " " && massiveStr[0].Count() == 14)
                        {
                            list_UP.Add(new DataFromUPfile(massiveStr[0], massiveStr[1]));
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (list_UP.Count() == 0)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Внимание! Нет информации об уполномоченных представителях.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                findError = true;
                --countFile;
            }
            catch (IOException ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                findError = true;
                --countFile;
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                findError = true;
                --countFile;
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
            list_UP.Clear();

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
                --countFile;
                //Console.WriteLine(ex.ToString());
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
                catch (InvalidDataException ex)
                {
                    IOoperations.WriteLogError(ex.ToString());
                    findError = true;
                    --countFile;
                }
                catch (IOException ex)
                {
                    IOoperations.WriteLogError(ex.ToString());
                    findError = true;
                    --countFile;
                }
                catch (Exception ex)
                {
                    IOoperations.WriteLogError(ex.ToString());
                    findError = true;
                    --countFile;
                    //Console.WriteLine(ex.ToString());
                }



                if (findError)
                {
                    Console.WriteLine(
                        Environment.NewLine + "Внимание! В ходе выполнения операции произошли ошибки!"
                        + Environment.NewLine + "Дополнительня информация отражена в файле errorLog.txt");
                }
                else
                {
                    //Console.WriteLine();
                    Console.WriteLine("Количество выбранных строк из файла(ов) с информацией об УП: {0} ", list_UP.Count());
                    //Console.WriteLine();
                }
            }
        }
    }

    #endregion

}
