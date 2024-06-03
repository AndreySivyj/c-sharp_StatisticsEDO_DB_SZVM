using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace StatisticsEDO_DB
{

    #region считанная из файла информация

    public class DataFromSpuspisFile
    {
        public string DST;
        public string ID_PCK;
        public string ID_DOC;
        public string STS_PCK;
        public string STS_DOC;
        public string RG_NUM;
        public string Y;
        public string M;
        public string SPU_ID;
        public string FRM_PR;
        public string FRM_TS;
        public string FRMTYP;
        public string CNT_ZGLV;
        public string CNT_SPU;
        public string CNT_AKTL;
        public string CNT_ERR;
        public string REG_EXD;
        public string FILNAM;



        public DataFromSpuspisFile(string DST = "", string ID_PCK = "", string ID_DOC = "", string STS_PCK = "", string STS_DOC = "", string RG_NUM = "",
                            string Y = "", string M = "", string SPU_ID = "", string FRM_PR = "", string FRM_TS = "", string FRMTYP = "",
                            string CNT_ZGLV = "", string CNT_SPU = "", string CNT_AKTL = "", string CNT_ERR = "", string REG_EXD = "", string FILNAM = "")
        {
            this.DST = DST;
            this.ID_PCK = ID_PCK;
            this.ID_DOC = ID_DOC;
            this.STS_PCK = STS_PCK;
            this.STS_DOC = STS_DOC;
            this.RG_NUM = RG_NUM;
            this.Y = Y;
            this.M = M;
            this.SPU_ID = SPU_ID;
            this.FRM_PR = FRM_PR;
            this.FRM_TS = FRM_TS;
            this.FRMTYP = FRMTYP;
            this.CNT_ZGLV = CNT_ZGLV;
            this.CNT_SPU = CNT_SPU;
            this.CNT_AKTL = CNT_AKTL;
            this.CNT_ERR = CNT_ERR;
            this.REG_EXD = REG_EXD;
            this.FILNAM = FILNAM;
        }


        public override string ToString()
        {
            return DST + ";" + ID_PCK + ";" + ID_DOC + ";" + STS_PCK + ";" + STS_DOC + ";" + RG_NUM + ";"
                    + Y + ";" + M + ";" + SPU_ID + ";" + FRM_PR + ";" + FRM_TS + ";" + FRMTYP + ";"
                    + CNT_ZGLV + ";" + CNT_SPU + ";" + CNT_AKTL + ";" + CNT_ERR + ";" + REG_EXD + ";" + FILNAM + ";";
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------
    #region Выбор данных из файла
    static class SelectDataFromSpuspisFile
    {
        //Маска поиска файлов
        private static string fileSearchMask = "*.*";

        public static Dictionary<string, int> dictionaryDataSpuspisSvod = new Dictionary<string, int>();   //Коллекция сводных данных из файлов
        private static List<DataFromSpuspisFile> listDataSpuspis = new List<DataFromSpuspisFile>();         //Коллекция данных из файлов

        private static bool findSpuspis = false;      //обнуляем признак наличия Spuspis-файлов

        //------------------------------------------------------------------------------------------
        //Открываем поток для чтения из файла и выбираем нужные позиции           
        private static void ReadAndParseTextFile(string openFile, string otchYear, string otchMonth)
        {
            try
            {
                using (StreamReader reader = new StreamReader(openFile, Encoding.GetEncoding(1251)))
                {
                    while (!reader.EndOfStream)
                    {
                        DataFromSpuspisFile tmpDataFromSpuspisFile;

                        //Очищаем объект для данных из строки для каждой итерации цикла                    

                        string strTmp = reader.ReadLine();

                        char[] separator = { ';' };    //список разделителей в строке
                        string[] massiveStr = strTmp.Split(separator);     //создаем массив из строк между разделителями

                        for (int i = 0; i < massiveStr.Length; i++)
                        {
                            massiveStr[i] = massiveStr[i].Replace(" ", string.Empty);   //удаляем пробелы в элементах массива
                        }

                        if (massiveStr.Count() == 18)
                            findSpuspis = true;

                        //Spuspis
                        if (massiveStr.Count() == 18 && massiveStr[6] == otchYear && massiveStr[7] == otchMonth)
                        {
                            tmpDataFromSpuspisFile = new DataFromSpuspisFile(massiveStr[0], massiveStr[1], massiveStr[2], massiveStr[3], massiveStr[4],
                                                               ConvertRegNomFull(massiveStr[5]), massiveStr[6], massiveStr[7], massiveStr[8], massiveStr[9],
                                                               massiveStr[10], massiveStr[11], massiveStr[12], massiveStr[13], massiveStr[14],
                                                               massiveStr[15], massiveStr[16], massiveStr[17]);

                            string tmpRegNum = ConvertRegNomFull(massiveStr[5]);

                            //Добавляем выбранные данные в коллекцию dictionaryDataSpuspisSvod
                            int tmpDataFromSpuspis;
                            if (dictionaryDataSpuspisSvod.TryGetValue(tmpRegNum, out tmpDataFromSpuspis))
                            {
                                //есть рег.Номер в словаре
                                dictionaryDataSpuspisSvod[tmpRegNum] = dictionaryDataSpuspisSvod[tmpRegNum] + Convert.ToInt32(massiveStr[14]);
                            }
                            else
                            {
                                //нет рег.Номера в словаре
                                dictionaryDataSpuspisSvod.Add(tmpRegNum, Convert.ToInt32(massiveStr[14]));
                            }


                            //Добавляем выбранные данные в коллекцию строк listDataSpuspis
                            listDataSpuspis.Add(tmpDataFromSpuspisFile);
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

        private static string ConvertRegNom(string regNom)
        {
            char[] separator = { '-' };    //список разделителей в строке
            string[] massiveStr = regNom.Split(separator);     //создаем массив из строк между разделителями
            try
            {
                if (massiveStr.Count() == 3 && regNom.Count() == 14)
                {
                    return "42" + massiveStr[1] + massiveStr[2];
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;

                return "";
            }
        }

        

        //------------------------------------------------------------------------------------------        
        /// <summary>
        /// Выбираем данные из файлов
        /// </summary>
        /// <param name="folderIn">Каталог с обрабатываемыми файлами</param>
        public static void ObrFileFromDirectory(string folderIn, string otchYear, string otchMonth)
        {
            //Очищаем коллекции для данных из файлов
            dictionaryDataSpuspisSvod.Clear();
            listDataSpuspis.Clear();

            DirectoryInfo dirInfo = new DirectoryInfo(folderIn);

            if (dirInfo.GetFiles(fileSearchMask).Count() == 0)
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
                        ReadAndParseTextFile(file.FullName, otchYear, otchMonth);
                    }

                    //Создаем файлы:
                    if (listDataSpuspis.Count() > 0)
                    {
                        CreateListDataSpuspisFile(listDataSpuspis, otchYear, otchMonth);
                        Console.WriteLine("Количество выбранных записей за отчетный период {0}_{1} из файла \"42_spuspis2a.csv\": {2}", otchMonth, otchYear, listDataSpuspis.Count());
                    }
                    if (dictionaryDataSpuspisSvod.Count() > 0)
                    {
                        //Формируем имя файла
                        string resultFile = IOoperations.katalogOut + @"\" + @"1_Spuspis_Свод_" + DateTime.Now.ToShortDateString() + ".csv";

                        CreateListDataSpuspisFileSvod(resultFile, dictionaryDataSpuspisSvod, otchYear, otchMonth);
                        Console.WriteLine("Количество уникальных регНомеров из файла \"42_spuspis2a.csv\": {0}", dictionaryDataSpuspisSvod.Count());
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




                //Console.WriteLine(Environment.NewLine + "Количество обработаных файлов: " + countFile);

                if (listDataSpuspis.Count() == 0 && findSpuspis == true)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("По заданным параметрам поиска в файле(ах) Spuspis нет данных.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }



            }
        }

        public static void CreateListDataSpuspisFileSvod(string resultFile, Dictionary<string, int> dictionaryDataSpuspis, string otchYear, string otchMonth)
        {
            //Заголовок для файла
            string zagolovokDataFromFile = "РегНомер" + ";" + "Сумма по полю CNT_AKTL" + ";" + "ОтчГод" + ";" + "ОтчМес" + ";";
            
            //Формируем результирующий файл
            try
            {
                //false - перезаписываем файл при его наличии
                using (StreamWriter writer = new StreamWriter(resultFile, false, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovokDataFromFile);
                    foreach (var item in dictionaryDataSpuspis)
                    {
                        writer.WriteLine(item.Key.ToString() + ";" + item.Value.ToString() + ";" + otchYear + ";" + otchMonth + ";");
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
        //Создаем файл Spuspis из результата выборки
        private static void CreateListDataSpuspisFile(List<DataFromSpuspisFile> listDataSpuspis, string otchYear, string otchMonth)
        {
            //Заголовок для файла
            string zagolovokDataFromFile = "DST" + ";" + "ID_PCK" + ";" + "ID_DOC" + ";" + "STS_PCK" + ";" + "STS_DOC" + ";" + "RG_NUM" + ";"
                + "Y" + ";" + "M" + ";" + "SPU_ID" + ";" + "FRM_PR" + ";" + "FRM_TS" + ";" + "FRMTYP" + ";"
                + "CNT_ZGLV" + ";" + "CNT_SPU" + ";" + "CNT_AKTL" + ";" + "CNT_ERR" + ";" + "REG_EXD" + ";" + "FILNAM" + ";";

            //Формируем имя файла
            string resultFile = IOoperations.katalogOut + @"\" + "1_Spuspis_Реестр_общий_" + otchYear + "_" + otchMonth + "_" + DateTime.Now.ToShortDateString() + ".csv";

            //Формируем результирующий файл
            try
            {
                //false - перезаписываем файл при его наличии
                using (StreamWriter writer = new StreamWriter(resultFile, false, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovokDataFromFile);
                    foreach (DataFromSpuspisFile dataFromFile in listDataSpuspis)
                    {
                        writer.WriteLine(dataFromFile.ToString());
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
        private static string ConvertRegNomFull(string regNom)
        {
            try
            {
                char[] regNomOld = regNom.ToCharArray();
                string regNomConvert = "0" + regNomOld[0].ToString() + regNomOld[1].ToString() + "-" + regNomOld[2] + regNomOld[3] + regNomOld[4] + "-" + regNomOld[5] + regNomOld[6] + regNomOld[7] + regNomOld[8] + regNomOld[9] + regNomOld[10];


                return regNomConvert;
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;

                return "";
            }
        }
    }

    #endregion
}
