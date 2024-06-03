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

    public class DataFromToutFile
    {
        public string DST;
        public string REG_NUM;
        public string FRMTYP;
        public string Y;
        public string M;
        public string INSNMB;
        public string INN;
        public string WRK;
        public string INS_TS;

        public DataFromToutFile(string DST = "", string REG_NUM = "", string FRMTYP = "", string Y = "", string M = "",
                            string INSNMB = "", string INN = "", string WRK = "", string INS_TS = "")
        {
            this.DST = DST;
            this.REG_NUM = REG_NUM;
            this.FRMTYP = FRMTYP;
            this.Y = Y;
            this.M = M;
            this.INSNMB = INSNMB;
            this.INN = INN;
            this.WRK = WRK;
            //this.FRMTYP = FRMTYP;
            this.INS_TS = INS_TS;
        }


        public override string ToString()
        {
            return DST + ";" + REG_NUM + ";" + FRMTYP + ";" + Y + ";" + M + ";" + INSNMB + ";" + INN + ";" + WRK + ";" + INS_TS; //+ ";";
        }
    }

    public class DataFromToutFileSvod
    {
        public string REG_NUM;
        public int countZL;

        public DataFromToutFileSvod(string REG_NUM = "", int countZL = 0)
        {
            this.REG_NUM = REG_NUM;
            this.countZL = countZL;
        }


        public override string ToString()
        {
            return REG_NUM + ";" + countZL + ";";
        }
    }


    #endregion

    //------------------------------------------------------------------------------------------
    #region Выбор данных из файла
    static class SelectDataFromToutFile
    {
        //количество файлов в каталоге "_In"
        private static int countFileTout;

        //Маска поиска файлов
        private static string fileSearchMask = "*.*";

        //private static SortedSet<string> sortedsetRegNomAll = new SortedSet<string>();     //Коллекция уникальных regNomerov (все)

        //public static List<DataFromToutFile> listDataTout = new List<DataFromToutFile>();  //Коллекция данных из файлов (все)

        //private static Dictionary<string, DataFromToutFileSvod> dictionaryDataToutSvod_old = new Dictionary<string, DataFromToutFileSvod>();  //Коллекция данных из файлов (все)
        //private static Dictionary<string, DataFromToutFileSvod> dictionaryDataToutSvod_new = new Dictionary<string, DataFromToutFileSvod>();  //Коллекция данных из файлов (все)




        //public static SortedSet<string> regNomANDsnilsFromToutFiles = new SortedSet<string>();  //Коллекция уникальных пар значений <регНом, СНИЛС>

        //Отчетный период из файла
        //private static SortedSet<string> otchYear = new SortedSet<string>();
        //private static SortedSet<string> otchMonth = new SortedSet<string>();





        //------------------------------------------------------------------------------------------
        //Открываем поток для чтения из файла и выбираем нужные позиции
        //private static void ReadAndParseTextFile(string openFile, string otchMonth)
        private static void ReadAndParseTextFile(string openFile)
        {
            try
            {
                using (StreamReader reader = new StreamReader(openFile, Encoding.GetEncoding(1251)))
                {
                    while (!reader.EndOfStream)
                    {
                        DataFromToutFile tmpDataFromFile;

                        //Очищаем объект для данных из строки для каждой итерации цикла                    

                        string strTmp = reader.ReadLine();

                        char[] separator = { ';' };    //список разделителей в строке
                        string[] massiveStr = strTmp.Split(separator);     //создаем массив из строк между разделителями

                        //в последнем поле ("INS_TS") не удаляем пробелы
                        for (int i = 0; i < massiveStr.Length - 1; i++)
                        {
                            massiveStr[i] = massiveStr[i].Replace(" ", string.Empty);   //удаляем пробелы в элементах массива
                        }

                        if (massiveStr.Count() == 9 && massiveStr[1] != "REG_NUM")
                        //if (massiveStr.Count() == 9 && massiveStr[1] != "\"REG_NUM\"")
                        {
                            tmpDataFromFile = new DataFromToutFile(massiveStr[0], massiveStr[1].Replace("\"", string.Empty), massiveStr[2], massiveStr[3], massiveStr[4],
                                                               massiveStr[5], massiveStr[6], massiveStr[7], massiveStr[8].Replace("/","-").Replace(" ", "-").Replace(":", "."));



                            //if ((tmpDataFromFile.FRMTYP == "\"ДПЛН\"" || tmpDataFromFile.FRMTYP == "\"ИСХД\"") 
                            //    && tmpDataFromFile.INSNMB != ""
                            //    && tmpDataFromFile.Y == "\"" + Program.otchYear + "\"" 
                            //    && tmpDataFromFile.M == "\"" + otchMonth + "\"")
                            if ((tmpDataFromFile.FRMTYP == "ДПЛН" || tmpDataFromFile.FRMTYP == "ИСХД")
                                && tmpDataFromFile.INSNMB != ""
                                && tmpDataFromFile.Y == Program.otchYear)
                            //&& tmpDataFromFile.M == otchMonth)
                            {

                                //////Добавляем выбранные данные в коллекцию
                                ////DataFromToutFileSvod tmpDataFromToutFileSvod = new DataFromToutFileSvod();
                                ////if (dictionaryDataToutSvod.TryGetValue(tmpDataFromFile.REG_NUM, out tmpDataFromToutFileSvod))
                                ////{
                                ////    //есть рег.Номер в словаре
                                ////    ++dictionaryDataToutSvod[tmpDataFromFile.REG_NUM].countZL;
                                ////}
                                ////else
                                ////{
                                ////    //нет рег.Номера в словаре
                                ////    dictionaryDataToutSvod.Add(tmpDataFromFile.REG_NUM, new DataFromToutFileSvod(tmpDataFromFile.REG_NUM, 1));
                                ////}
                                ///

    /*
        public string DST;
        public string REG_NUM;
        public string FRMTYP;
        public string Y;
        public string M;
        public string INSNMB;
        public string INN;
        public string WRK;
        public string INS_TS;

    */
                                //------------------------------------------------------------------------------------------
                                //oldTout Наполняем коллекцию уникальных регномеров + СНИЛС + отчПериод из Tout только по месяцу "Program.otchMonth-1" !!!
                                if (tmpDataFromFile.Y == Program.otchYear && tmpDataFromFile.M == (Convert.ToInt32(Program.otchMonth) - 1).ToString())
                                {
                                    Program.toutUnikOtchMonth_old.Add(
                                                                        tmpDataFromFile.DST + ";" +
                                                                        "\"" + tmpDataFromFile.REG_NUM + "\""+ ";" +
                                                                        "\"" + tmpDataFromFile.FRMTYP + "\"" + ";" +
                                                                        "\"" + tmpDataFromFile.Y + "\"" + ";" +
                                                                        "\"" + tmpDataFromFile.M + "\"" + ";" +
                                                                        tmpDataFromFile.INSNMB + ";" +
                                                                        "\"" + tmpDataFromFile.INN + "\"" + ";" +
                                                                        tmpDataFromFile.WRK + ";" +
                                                                        "\"" + tmpDataFromFile.INS_TS + "\""
                                                                        );
                                }


                                //------------------------------------------------------------------------------------------
                                //newTout Наполняем коллекцию уникальных регномеров + СНИЛС + отчПериод из Tout только по текущему месяцу "Program.otchMonth" !!!
                                if (tmpDataFromFile.Y == Program.otchYear && tmpDataFromFile.M == Program.otchMonth)
                                {
                                    //Добавляем выбранные данные в коллекцию строк Program.toutUnikRegNomAndSNILS (уникальные пары значений регНом + СНИЛС)
                                    Program.toutUnikRegNomAndSNILS.Add(tmpDataFromFile.REG_NUM + ";" + tmpDataFromFile.INSNMB + ";");

                                    Program.toutUnikOtchMonth_new.Add(
                                                                        tmpDataFromFile.DST + ";" +
                                                                        "\"" + tmpDataFromFile.REG_NUM + "\"" + ";" +
                                                                        "\"" + tmpDataFromFile.FRMTYP + "\"" + ";" +
                                                                        "\"" + tmpDataFromFile.Y + "\"" + ";" +
                                                                        "\"" + tmpDataFromFile.M + "\"" + ";" +
                                                                        tmpDataFromFile.INSNMB + ";" +
                                                                        "\"" + tmpDataFromFile.INN + "\"" + ";" +
                                                                        tmpDataFromFile.WRK + ";" +
                                                                        "\"" + tmpDataFromFile.INS_TS + "\""
                                                                        );
                                }

                                //------------------------------------------------------------------------------------------


                                //Добавляем выбранные данные в коллекцию строк listData
                                //listDataTout.Add(tmpDataFromFile);

                                //Добавляем выбранные данные в коллекцию строк sortedsetRegNomAll (общий список для всех файлов)
                                //sortedsetRegNomAll.Add(tmpDataFromFile.REG_NUM);

                                //Добавляем выбранные данные в коллекцию строк regNomANDsnilsFromToutFiles (уникальные пары значений регНом + СНИЛС)
                                //regNomANDsnilsFromToutFiles.Add(tmpDataFromFile.REG_NUM+";"+ tmpDataFromFile.INSNMB + ";");
                            }

                            //otchYear.Add(massiveStr[3].Replace("\"", string.Empty));
                            //otchMonth.Add(massiveStr[4].Replace("\"", string.Empty));
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                --countFileTout;
            }
            catch (IOException ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                --countFileTout;
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                --countFileTout;
            }
        }



        //------------------------------------------------------------------------------------------        
        /// <summary>
        /// Выбираем данные из файлов
        /// </summary>
        /// <param name="folderIn">Каталог с обрабатываемыми файлами</param>
        /// public static void ObrFileFromDirectory(string folderIn, ref Dictionary<string, DataFromToutFileSvod> toutDictionary, string otchMonth)
        public static void ObrFileFromDirectory(string folderIn)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folderIn);

                //Вычисляем количество считанных файлов
                countFileTout = dirInfo.GetFiles(fileSearchMask).Count();

                if (countFileTout != 1)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Нет файлов для обработки, либо файлов более одного.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    //Обрабатываем каждый файл по отдельности
                    foreach (FileInfo file in dirInfo.GetFiles(fileSearchMask))
                    {
                        //Очищаем коллекцию для данных из файла
                        //dictionaryDataToutSvod_old.Clear();
                        //dictionaryDataToutSvod_new.Clear();
                        //otchYear.Clear();
                        //otchMonth.Clear();

                        //Открываем поток для чтения из файла и выбираем нужные позиции   
                        ReadAndParseTextFile(file.FullName);






                        //if (otchYear.Count() == 1 && otchMonth.Count() == 1)
                        //{
                        ////------------------------------------------------------------------------------------------
                        ////Формируем имя файла с уникальными значениями рег. номеров и количеством ЗЛ (ИСХД и ДПЛН, без нулевиков, всего в файле, сводная)
                        //string tmpOtchYear = "";
                        //foreach (var item in otchYear)
                        //{
                        //    tmpOtchYear = item;
                        //}
                        //string tmpOtchMonth = "";
                        //foreach (var item in otchMonth)
                        //{
                        //    tmpOtchMonth = item;
                        //}



                        //string resulSvodTout = IOoperations.katalogOut + @"\" + "1_" + Program.otchYear + "_" + otchMonth + "_" + "Tout_Сводная_информация" + ".csv";

                        ////Заголовок для файлов с уник. рег. номерами "RG_NUM;countZL;"

                        ////Проверяем на наличие файла отчета, если существует - удаляем
                        //if (File.Exists(resulSvodTout))
                        //{
                        //    IOoperations.FileDelete(resulSvodTout);
                        //}

                        //if (dictionaryDataToutSvod.Count() != 0)
                        //{
                        //    //Создаем файл
                        //    CreatResultFromDictionary("RG_NUM;countZL;", dictionaryDataToutSvod, resulSvodTout);

                        //    foreach (var item in dictionaryDataToutSvod)
                        //    {
                        //        toutDictionary.Add(item.Key, item.Value);
                        //    }

                        //}



                        //string resulTout = IOoperations.katalogOut + @"\" + "1_" + Program.otchYear + "_" + otchMonth + "_" + "Tout_" + ".csv";

                        ////Заголовок для файлов с уник. рег. номерами "RG_NUM;countZL;"

                        ////Проверяем на наличие файла отчета, если существует - удаляем
                        //if (File.Exists(resulTout))
                        //{
                        //    IOoperations.FileDelete(resulTout);
                        //}

                        //if (dictionaryDataToutSvod.Count() != 0)
                        //{
                        //    //Создаем файл
                        //    CreatResultFromDictionary("REG_NUM;INSNMB;Y;M;INS_TS;", Program.toutUnikOtchMonth_old, resulTout);

                        //    foreach (var item in dictionaryDataToutSvod)
                        //    {
                        //        toutDictionary.Add(item.Key, item.Value);
                        //    }

                        //}



                        //toutDictionary = dictionaryDataToutSvod;



                        //Console.WriteLine();
                        //Console.WriteLine(new string('-', 91));
                        //Console.WriteLine();
                        //Console.WriteLine("Количество уникальных рег. номеров в файле   \"{0}\"  : {1} ", file.Name, dictionaryDataToutSvod.Count());



                        //Console.WriteLine("Количество уникальных рег. номеров в файле   \"{0}\" (отч. период {2}_{3}) : {1} ", file.Name, dictionaryDataToutSvod.Count(), otchMonth, Program.otchYear);
                        //Console.WriteLine("Количество обработаных файлов в каталоге \"{0}\" : {1}", folderIn, countFileTout);

                        //}
                        //else
                        //{
                        //    Console.WriteLine();
                        //    Console.ForegroundColor = ConsoleColor.Red;
                        //    Console.WriteLine("В файлах каталога   \"{0}\"   больше одного отчетного периода.", folderIn);
                        //    Console.ForegroundColor = ConsoleColor.Gray;
                        //}
                    }


                    //ToutOld (предыдущий месяц)
                    //SelectDataFromToutFile.ObrFileFromDirectory(IOoperations.katalogInTout, ref dictionaryDataToutSvodOld, (Convert.ToInt32(Program.otchMonth)-1).ToString());

                    int tmpMonth = Convert.ToInt32(Program.otchMonth) - 1;
                    string resulTout_old = IOoperations.katalogOut + @"\" + "1_" + Program.otchYear + "_" + tmpMonth + "_" + "Tout_" + ".csv";
                    //Проверяем на наличие файла отчета, если существует - удаляем
                    if (File.Exists(resulTout_old))
                    {
                        IOoperations.FileDelete(resulTout_old);
                    }

                    /*
       public string DST;
       public string REG_NUM;
       public string FRMTYP;
       public string Y;
       public string M;
       public string INSNMB;
       public string INN;
       public string WRK;
       public string INS_TS;

   */


                    if (Program.toutUnikOtchMonth_old.Count() != 0)
                    {
                        //Создаем файл
                        SelectDataFromToutFile.CreatResultToutFile("\"DST\";\"REG_NUM\";\"FRMTYP\";\"Y\";\"M\";\"INSNMB\";\"INN\";\"WRK\";\"INS_TS\"", Program.toutUnikOtchMonth_old, resulTout_old);
                        
                        Console.WriteLine("Количество уникальных рег. номеров в файле   \"tout.csv\" (отч. период {1}_{2}) : {0} ", Program.toutUnikOtchMonth_old.Count(), (Convert.ToInt32(Program.otchMonth) - 1).ToString(), Program.otchYear);



                        foreach (var item in Program.toutUnikOtchMonth_old)
                        {
                            char[] separator = { ';' };    //список разделителей в строке
                            string[] massiveStr = item.Replace("\"", "").Split(separator);     //создаем массив из строк между разделителями

                            //Добавляем выбранные данные в коллекцию
                            DataFromToutFileSvod tmpDataFromToutFileSvod = new DataFromToutFileSvod();
                            if (Program.dictionaryDataToutSvodOld.TryGetValue(massiveStr[1], out tmpDataFromToutFileSvod))
                            {
                                //есть рег.Номер в словаре
                                ++Program.dictionaryDataToutSvodOld[massiveStr[1]].countZL;
                            }
                            else
                            {
                                //нет рег.Номера в словаре
                                Program.dictionaryDataToutSvodOld.Add(massiveStr[1], new DataFromToutFileSvod(massiveStr[1], 1));
                            }
                        }



                        string resulSvodTout_old = IOoperations.katalogOut + @"\" + "1_" + Program.otchYear + "_" + (Convert.ToInt32(Program.otchMonth) - 1).ToString() + "_" + "Tout_Сводная_информация" + ".csv";
                        
                        //Проверяем на наличие файла отчета, если существует - удаляем
                        if (File.Exists(resulSvodTout_old))
                        {
                            IOoperations.FileDelete(resulSvodTout_old);
                        }

                        if (Program.dictionaryDataToutSvodOld.Count() != 0)
                        {
                            //Создаем файл
                            //Заголовок для файлов с уник. рег. номерами "RG_NUM;countZL;"
                            CreatResultFromDictionary("RG_NUM;countZL;", Program.dictionaryDataToutSvodOld, resulSvodTout_old);
                        }

                    }



                    //ToutNew (текущий месяц)
                    //SelectDataFromToutFile.ObrFileFromDirectory(IOoperations.katalogInTout, ref dictionaryDataToutSvodNew, Program.otchMonth);

                    string resulTout_new = IOoperations.katalogOut + @"\" + "1_" + Program.otchYear + "_" + Program.otchMonth + "_" + "Tout_" + ".csv";
                    //Проверяем на наличие файла отчета, если существует - удаляем
                    if (File.Exists(resulTout_new))
                    {
                        IOoperations.FileDelete(resulTout_new);
                    }

                    if (Program.toutUnikOtchMonth_new.Count() != 0)
                    {
                        //Создаем файл
                        SelectDataFromToutFile.CreatResultToutFile("\"DST\";\"REG_NUM\";\"FRMTYP\";\"Y\";\"M\";\"INSNMB\";\"INN\";\"WRK\";\"INS_TS\"", Program.toutUnikOtchMonth_new, resulTout_new);

                        Console.WriteLine("Количество уникальных рег. номеров в файле   \"tout.csv\" (отч. период {1}_{2}) : {0} ", Program.toutUnikOtchMonth_new.Count(), Program.otchMonth, Program.otchYear);



                        foreach (var item in Program.toutUnikOtchMonth_new)
                        {
                            char[] separator = { ';' };    //список разделителей в строке
                            string[] massiveStr = item.Replace("\"","").Split(separator);     //создаем массив из строк между разделителями

                            //Добавляем выбранные данные в коллекцию
                            DataFromToutFileSvod tmpDataFromToutFileSvod = new DataFromToutFileSvod();
                            if (Program.dictionaryDataToutSvodNew.TryGetValue(massiveStr[1], out tmpDataFromToutFileSvod))
                            {
                                //есть рег.Номер в словаре
                                ++Program.dictionaryDataToutSvodNew[massiveStr[1]].countZL;
                            }
                            else
                            {
                                //нет рег.Номера в словаре
                                Program.dictionaryDataToutSvodNew.Add(massiveStr[1], new DataFromToutFileSvod(massiveStr[1], 1));
                            }
                        }



                        string resulSvodTout_new = IOoperations.katalogOut + @"\" + "1_" + Program.otchYear + "_" + Program.otchMonth + "_" + "Tout_Сводная_информация" + ".csv";

                        //Проверяем на наличие файла отчета, если существует - удаляем
                        if (File.Exists(resulSvodTout_new))
                        {
                            IOoperations.FileDelete(resulSvodTout_new);
                        }

                        if (Program.dictionaryDataToutSvodNew.Count() != 0)
                        {
                            //Создаем файл
                            //Заголовок для файлов с уник. рег. номерами "RG_NUM;countZL;"
                            CreatResultFromDictionary("RG_NUM;countZL;", Program.dictionaryDataToutSvodNew, resulSvodTout_new);
                        }

                    }

                }

            }
            catch (InvalidDataException ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                --countFileTout;
            }
            catch (IOException ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                --countFileTout;
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                --countFileTout;
            }
        }

        //------------------------------------------------------------------------------------------
        //Создаем результирующий файл
        private static void CreatResultFromDictionary(string zagolovok, Dictionary<string, DataFromToutFileSvod> dictionaryData, string nameFile)
        {
            try
            {
                //Формируем результирующий файл статистики
                using (StreamWriter writer = new StreamWriter(nameFile, false, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovok);

                    foreach (var item in dictionaryData)
                    {
                        writer.WriteLine(item.Value.ToString());
                    }
                }
            }
            catch (IOException ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                --countFileTout;
            }
            catch (Exception ex)
            {

                IOoperations.WriteLogError(ex.ToString());
            }
        }

        //------------------------------------------------------------------------------------------
        //Создаем результирующий файл
        public static void CreatResultToutFile(string zagolovok, SortedSet<string> toutUnikOtchMonth, string nameFile)
        {
            try
            {
                //Формируем результирующий файл статистики
                using (StreamWriter writer = new StreamWriter(nameFile, false, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovok);

                    foreach (var item in toutUnikOtchMonth)
                    {
                        writer.WriteLine(item.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());
            }
        }





    }

    #endregion
}
