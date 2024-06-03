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

    public class DataFromPlanPriemaFile
    {
        //№ п/п	Код района	Рег. номер	Наименование	Дата постановки на учет в РО	Дата снятия с учета в РО	Категория	ИНН	"Количество ЗЛ, учтенных в подсистеме АИС ПФР 2 за предшествующий отчетный период по форме СЗВ-М"	"Предшественник ("старый" регистрационный номер)"

        public string nomPor;
        public string raion;
        public string regNum;
        public string nameStrah;
        //public string dataPostRO;
        //public string dataSnyatRO;
        //public string kategory;
        //public string inn;
        //public string kolZLTout;
        //public string predshestv;

        //public DataFromPlanPriemaFile(string nomPor = "", string raion = "", string regNum = "", string nameStrah = "",
        //                     string dataPostRO = "", string dataSnyatRO = "", string kategory = "", string inn = "", string kolZLTout = "", string predshestv = "")
        public DataFromPlanPriemaFile(string nomPor = "", string raion = "", string regNum = "", string nameStrah = "")
        {
            this.nomPor = nomPor;
            this.raion = raion;
            this.regNum = regNum;
            this.nameStrah = nameStrah;
            //this.dataPostRO = dataPostRO;
            //this.dataSnyatRO = dataSnyatRO;
            //this.kategory = kategory;
            //this.inn = inn;
            //this.kolZLTout = kolZLTout;
            //this.predshestv = predshestv;
        }


        public override string ToString()
        {
            return nomPor + ";" + raion + ";" + regNum + ";" + nameStrah + ";";
            //return nomPor + ";" + raion + ";" + regNum + ";" + nameStrah + ";" + dataPostRO + ";" + dataSnyatRO + ";" + kategory + ";" + inn + ";" + kolZLTout + ";" + predshestv + ";";
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------
    #region Выбор данных из файла
    static class SelectDataFromPlanPriemaFile
    {
        //Маска поиска файлов
        private static string fileSearchMask = "*.csv";

        public static Dictionary<string, DataFromPlanPriemaFile> dictionaryPlanPriema = new Dictionary<string, DataFromPlanPriemaFile>();    //Коллекция всех данных из реестра PlanPriema    

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
                        string strTmp = reader.ReadLine();

                        if (strTmp == "(\"\"старый\"\" регистрационный номер)\"" || strTmp == "" || strTmp == " ")
                        {
                            continue;
                        }
                        else
                        {
                            char[] separator = { ';' };    //список разделителей в строке
                            string[] massiveStr = strTmp.Split(separator);     //создаем массив из строк между разделителями                      

                            //massiveStr.Count() == 20 &&
                            if (
                                massiveStr[0] != "№ п/п"
                                && massiveStr[0] != ""
                                && massiveStr[0] != " "
                                && massiveStr[3] != "4"
                                //&& massiveStr[0] != "Реестр страхователей, по которым ожидается представление ежемесячной отчетности по форме СЗВ-М за август 2019 года (учтены страхователи, представившие отчетность за июль 2019, исключены страхователи, снятые с учета в июле, а также представившие нулевую форму, добавлены страхователи вставшие на учет, либо прошедшие процедуру перерегистрации в августе по состоянию на 23.08.2019)"

                                )
                            {
                                //Коллекция всех данных из файла
                                //dictionaryPlanPriema.Add(massiveStr[2], new DataFromPlanPriemaFile(massiveStr[0], massiveStr[1], massiveStr[2], massiveStr[3], massiveStr[4], massiveStr[5], massiveStr[6],
                                //    massiveStr[7], massiveStr[8], massiveStr[9]));
                                dictionaryPlanPriema.Add(massiveStr[2], new DataFromPlanPriemaFile(massiveStr[0], massiveStr[1], massiveStr[2], massiveStr[3]));
                            }
                            else
                            {
                                continue;
                            }
                        }

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
            dictionaryPlanPriema.Clear();

            DirectoryInfo dirInfo = new DirectoryInfo(folderIn);

            if (dirInfo.GetFiles(fileSearchMask).Count() == 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Нет файлов в каталоге \"_In_PlanPriema\".");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {

                try
                {
                    //Console.WriteLine("Количество файлов в каталоге \"_In_PlanPriema\": " + dirInfo.GetFiles(fileSearchMask).Count());

                    //обрабатываем каждый файл по отдельности
                    foreach (FileInfo file in dirInfo.GetFiles(fileSearchMask))
                    {
                        if (file.Name == Program.otchMonth + "_" + Program.otchYear + "_PlanSZVM.csv")
                        {
                            //Открываем поток для чтения из файла и выбираем нужные позиции   
                            ReadAndParseTextFile(file.FullName);
                        }
                    }

                    if (dictionaryPlanPriema.Count() == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Внимание! Нет планового показателя в каталоге \"_In_PlanPriema\".");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine();
                    }
                    Console.WriteLine("Количество выбранных строк: {0}", dictionaryPlanPriema.Count());
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

        //------------------------------------------------------------------------------------------       
        //Формируем результирующий файл статистики
        public static void WriteLogs(string resultFile, string zagolovok, Dictionary<string, DataFromPlanPriemaFile> dictionaryPlanPriema)
        {
            //формируем результирующий файл статистики
            using (StreamWriter writer = new StreamWriter(resultFile, false, Encoding.GetEncoding(1251)))
            {
                writer.WriteLine(zagolovok);

                foreach (var item in dictionaryPlanPriema)
                {
                    writer.WriteLine(item.Value.ToString());
                }
            }
        }
    }

    #endregion

}
