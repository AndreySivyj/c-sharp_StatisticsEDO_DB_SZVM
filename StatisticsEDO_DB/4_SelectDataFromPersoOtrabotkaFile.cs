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

    public class DataFromPersoOtrabotkaFile
    {
        //string zagolovokError = "№ п/п" + ";" + "КодЗап" + ";" + "Район" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "ОтчМесяц" + ";" + "ОтчГод" + ";" + "Тип сведений" + ";" + "Дата представления" + ";"
        //                                        + "Категория" + ";" + "Дата постановки в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";" + "Результат проверки" + ";" + "Дата проверки" + ";"
        //                                        + "Количество ЗЛ в файле" + ";" + "ЗЛ принято" + ";" + "ЗЛ не принято" + ";"
        //                                        + "Статус квитанции" + ";" + "Специалист" + ";" + "Способ представления" + ";" + "Куратор" + ";" + "УП по данным УПФР" + ";"
        //                                        + "Дата направления уведомления страхователю" + ";" + "Контрольная дата для исправления (3 дня)" + ";" + "Исправлено (да|нет|не требуется)" + ";"
        //                                        + "Дата направления реестра в УПФР (в случае не исправления)" + ";" + "Дата исправления ошибки (после направления реестра УПФР)" + ";" + "Примечание" + ";"
        //                                        + "Результат контроля (руководитель)" + ";";

        public string codZap;
        public string raion;
        public string regNum;
        public string nameStrah;
        public string month;
        public string year;
        public string typeSved;
        public string dataPredst;
        public string kategory;
        public string dataPostPFR;
        public string dataPostRO;
        public string dataSnyatRO;
        public string resultat;
        public string dataProverki;
        public string kolZL;
        public string kolZLgood;
        public string kolZLbad;
        public string statusKvitanc;
        public string spec;
        public string specChanged;
        public string kurator;
        public string UP;
        public string dataNaprUvedomlStrah;
        public string kontrDataIspravleniya;
        public string statusIspravleniya;
        public string dataNaprReestraPFR;
        public string dataIspravleniyaError;
        public string primechanie;
        public string resultatKontrolya;

        public DataFromPersoOtrabotkaFile(string codZap = "", string raion = "", string regNum = "", string nameStrah = "", string month = "", string year = "", string typeSved = "",
                            string dataPredst = "", string kategory = "", string dataPostPFR = "", string dataPostRO = "", string dataSnyatRO = "", string resultat = "",
                             string dataProverki = "", string kolZL = "", string kolZLgood = "", string kolZLbad = "", string statusKvitanc = "",
                            string spec = "", string specChanged = "", string kurator = "", string UP = "", string dataNaprUvedomlStrah = "",
                            string kontrDataIspravleniya = "", string statusIspravleniya = "", string dataNaprReestraPFR = "", string dataIspravleniyaError = "",
                            string primechanie = "", string resultatKontrolya = "")
        {
            this.codZap = codZap;
            this.raion = raion;
            this.regNum = regNum;
            this.nameStrah = nameStrah;
            this.month = month;
            this.year = year;
            this.typeSved = typeSved;
            this.dataPredst = dataPredst;
            this.kategory = kategory;
            this.dataPostPFR = dataPostPFR;
            this.dataPostRO = dataPostRO;
            this.dataSnyatRO = dataSnyatRO;
            this.resultat = resultat;
            this.dataProverki = dataProverki;
            this.kolZL = kolZL;
            this.kolZLgood = kolZLgood;
            this.kolZLbad = kolZLbad;
            this.statusKvitanc = statusKvitanc;
            this.spec = spec;
            this.specChanged = specChanged;
            this.kurator = kurator;
            this.UP = UP;
            this.dataNaprUvedomlStrah = dataNaprUvedomlStrah;
            this.kontrDataIspravleniya = kontrDataIspravleniya;
            this.statusIspravleniya = statusIspravleniya;
            this.dataNaprReestraPFR = dataNaprReestraPFR;
            this.dataIspravleniyaError = dataIspravleniyaError;
            this.primechanie = primechanie;
            this.resultatKontrolya = resultatKontrolya;
        }


        public override string ToString()
        {
            return codZap + ";" + raion + ";" + regNum + ";" + nameStrah + ";" + month + ";" + year + ";" + typeSved + ";" + dataPredst + ";" + kategory + ";" + dataPostPFR + ";"
                        + dataPostRO + ";" + dataSnyatRO + ";" + resultat + ";" + dataProverki + ";" + kolZL + ";" + kolZLgood + ";" + kolZLbad + ";" + statusKvitanc + ";"
                         + spec + ";" + specChanged + ";" + kurator + ";" + UP + ";" + dataNaprUvedomlStrah + ";" + kontrDataIspravleniya + ";" + statusIspravleniya + ";"
                         + dataNaprReestraPFR + ";" + dataIspravleniyaError + ";" + primechanie + ";" + resultatKontrolya + ";";
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------
    #region Выбор данных из файла
    static class SelectDataFromPersoFile
    {
        //Маска поиска файлов
        private static string fileSearchMask = "*.csv";

        public static Dictionary<string, DataFromPersoOtrabotkaFile> dictionaryPersoOtrabotkaOld = new Dictionary<string, DataFromPersoOtrabotkaFile>();    //Коллекция всех данных из реестра Perso_Отработка    

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

                        char[] separator = { ';' };    //список разделителей в строке
                        string[] massiveStr = strTmp.Split(separator);     //создаем массив из строк между разделителями                      

                        //massiveStr.Count() == 20 &&
                        if (
                            massiveStr[0] != "№ п/п"
                            && massiveStr[0] != ""
                            && massiveStr[0] != " "
                            )
                        {
                            //Коллекция всех данных из файла
                            dictionaryPersoOtrabotkaOld.Add(massiveStr[1], new DataFromPersoOtrabotkaFile(massiveStr[1], massiveStr[2], massiveStr[3], massiveStr[4], massiveStr[5], massiveStr[6], massiveStr[7],
                                                        massiveStr[8], massiveStr[9], massiveStr[10], massiveStr[11], massiveStr[12], massiveStr[13], massiveStr[14], massiveStr[15],
                                                        massiveStr[16], massiveStr[17], massiveStr[18], massiveStr[19], massiveStr[20], massiveStr[21], massiveStr[22], massiveStr[23],
                                                        massiveStr[24], massiveStr[25], massiveStr[26], massiveStr[27], massiveStr[28], massiveStr[29]));
                        }
                        else
                        {
                            continue;
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
            dictionaryPersoOtrabotkaOld.Clear();

            DirectoryInfo dirInfo = new DirectoryInfo(folderIn);

            if (dirInfo.GetFiles(fileSearchMask).Count() == 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Нет файлов \"Perso_Отработка\".");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {

                try
                {
                    //Console.WriteLine("Количество файлов \"Perso_Отработка\": " + dirInfo.GetFiles(fileSearchMask).Count());

                    //обрабатываем каждый файл по отдельности
                    foreach (FileInfo file in dirInfo.GetFiles(fileSearchMask))
                    {
                        //Открываем поток для чтения из файла и выбираем нужные позиции   
                        ReadAndParseTextFile(file.FullName);
                    }

                    Console.WriteLine("Количество выбранных строк: {0}", dictionaryPersoOtrabotkaOld.Count());
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
