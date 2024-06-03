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
    static class CreateDataFromPersoSelect
    {
        public static List<DataFromPersoDB> list_ISXD_prinyato = new List<DataFromPersoDB>();           //Коллекция данных из файлов "ИСХОДНАЯ" "Принят"

        public static List<DataFromPersoDB> list_ISXD_partial_prinyato = new List<DataFromPersoDB>();   //Коллекция данных из файлов "ИСХОДНАЯ" "Принят частично"

        public static List<DataFromPersoDB> list_ISXD_NO_proveren = new List<DataFromPersoDB>();        //Коллекция данных из файлов "ИСХОДНАЯ" "Не проверен"

        public static List<DataFromPersoDB> list_DOP_prinyato = new List<DataFromPersoDB>();            //Коллекция данных из файлов "ДОПОЛНЯЮЩАЯ" "Принят"

        public static List<DataFromPersoDB> list_DOP_partial_prinyato = new List<DataFromPersoDB>();    //Коллекция данных из файлов "ДОПОЛНЯЮЩАЯ" "Принят частично"

        public static List<DataFromPersoDB> list_DOP_NO_proveren = new List<DataFromPersoDB>();         //Коллекция данных из файлов "ДОПОЛНЯЮЩАЯ" "Не проверен"

        public static List<DataFromPersoDB> list_DOP_NO_prinyato = new List<DataFromPersoDB>();         //Коллекция данных из файлов "ДОПОЛНЯЮЩАЯ" "Не принят"

        public static List<DataFromPersoDB> list_ISXD_NO_prinyato = new List<DataFromPersoDB>();        //Коллекция данных из файлов "ИСХОДНАЯ" "Не принят"

        public static List<DataFromPersoDB> list_DOP = new List<DataFromPersoDB>();                     //Коллекция данных из файлов "ДОПОЛНЯЮЩАЯ"

        public static List<DataFromPersoDB> list_OTMN = new List<DataFromPersoDB>();                    //Коллекция данных из файлов "ОТМЕНЯЮЩАЯ"

        public static List<DataFromPersoDB> list_OTMN_prinyato = new List<DataFromPersoDB>();           //Коллекция данных из файлов "ОТМЕНЯЮЩАЯ" "Принят"
        public static List<DataFromPersoDB> list_OTMN_partial_prinyato = new List<DataFromPersoDB>();   //Коллекция данных из файлов "ОТМЕНЯЮЩАЯ" "Принят частично"
        public static List<DataFromPersoDB> list_OTMN_NO_proveren = new List<DataFromPersoDB>();        //Коллекция данных из файлов "ОТМЕНЯЮЩАЯ" "Не проверен"
        public static List<DataFromPersoDB> list_OTMN_NO_prinyato = new List<DataFromPersoDB>();        //Коллекция данных из файлов "ОТМЕНЯЮЩАЯ" "Не принят"

        public static List<DataFromPersoDB> list_noName = new List<DataFromPersoDB>();                  //Коллекция данных из файлов "noName"

        public static List<DataFromPersoDB> list_errorKvitanciya = new List<DataFromPersoDB>();         //Коллекция данных из файлов "Ошибочное состояние квитанций"



        //------------------------------------------------------------------------------------------       
        //Формируем результирующий файл статистики
        private static void WriteLogs(string resultFile, string zagolovok, List<DataFromPersoDB> listData)
        {
            try
            {
                //формируем результирующий файл статистики
                using (StreamWriter writer = new StreamWriter(resultFile, false, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovok);

                    foreach (var item in listData)
                    {
                        writer.WriteLine(item.ToStringPersoALL());
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
        //Формируем результирующий файл статистики с ошибками (файл \"Perso_Отработка\")
        private static void WriteLogsError(string resultFile, string zagolovok, List<DataFromPersoDB> listData, Dictionary<string, DataFromPersoOtrabotkaFile> dictionaryPersoOtrabotkaOld)
        {
            foreach (var itemDataFromPersoDB in listData)
            {
                if (itemDataFromPersoDB.resultat == "Принят частично" || itemDataFromPersoDB.resultat == "Не принят" || itemDataFromPersoDB.resultat == "")
                {

                    DataFromPersoOtrabotkaFile tmpData = new DataFromPersoOtrabotkaFile();
                    if (dictionaryPersoOtrabotkaOld.TryGetValue(itemDataFromPersoDB.codZap, out tmpData))
                    {
                        continue;
                    }
                    else
                    {
                        dictionaryPersoOtrabotkaOld.Add(itemDataFromPersoDB.codZap, new DataFromPersoOtrabotkaFile(
                                                                                            itemDataFromPersoDB.codZap, itemDataFromPersoDB.raion, itemDataFromPersoDB.regNum,
                                                                                            itemDataFromPersoDB.nameStrah, itemDataFromPersoDB.month, itemDataFromPersoDB.year,
                                                                                            itemDataFromPersoDB.typeSved, itemDataFromPersoDB.dataPredst, itemDataFromPersoDB.kategory,
                                                                                            itemDataFromPersoDB.dataPostPFR, itemDataFromPersoDB.dataPostRO, itemDataFromPersoDB.dataSnyatRO,
                                                                                            itemDataFromPersoDB.resultat, itemDataFromPersoDB.dataProverki, itemDataFromPersoDB.kolZL,
                                                                                            itemDataFromPersoDB.kolZLgood, itemDataFromPersoDB.kolZLbad, itemDataFromPersoDB.statusKvitanc,
                                                                                            itemDataFromPersoDB.spec, itemDataFromPersoDB.specChanged, itemDataFromPersoDB.curator,
                                                                                            itemDataFromPersoDB.UP, itemDataFromPersoDB.dataUvedomleniya, itemDataFromPersoDB.dataKontrolya));
                    }
                }
                else if (itemDataFromPersoDB.resultat == "Не проверен" && itemDataFromPersoDB.statusKvitanc == "Обработан")
                {
                    DataFromPersoOtrabotkaFile tmpData = new DataFromPersoOtrabotkaFile();
                    if (dictionaryPersoOtrabotkaOld.TryGetValue(itemDataFromPersoDB.codZap, out tmpData))
                    {
                        continue;
                    }
                    else
                    {
                        dictionaryPersoOtrabotkaOld.Add(itemDataFromPersoDB.codZap, new DataFromPersoOtrabotkaFile(
                                                                                            itemDataFromPersoDB.codZap, itemDataFromPersoDB.raion, itemDataFromPersoDB.regNum,
                                                                                            itemDataFromPersoDB.nameStrah, itemDataFromPersoDB.month, itemDataFromPersoDB.year,
                                                                                            itemDataFromPersoDB.typeSved, itemDataFromPersoDB.dataPredst, itemDataFromPersoDB.kategory,
                                                                                            itemDataFromPersoDB.dataPostPFR, itemDataFromPersoDB.dataPostRO, itemDataFromPersoDB.dataSnyatRO,
                                                                                            itemDataFromPersoDB.resultat, itemDataFromPersoDB.dataProverki, itemDataFromPersoDB.kolZL,
                                                                                            itemDataFromPersoDB.kolZLgood, itemDataFromPersoDB.kolZLbad, itemDataFromPersoDB.statusKvitanc,
                                                                                            itemDataFromPersoDB.spec, itemDataFromPersoDB.specChanged, itemDataFromPersoDB.curator,
                                                                                            itemDataFromPersoDB.UP, itemDataFromPersoDB.dataUvedomleniya, itemDataFromPersoDB.dataKontrolya));
                    }
                }
                else
                {
                    continue;
                }                
            }


            //TODO: Закомментировал создание CSV-файла статистики "_Perso_Отработка_"
            ////формируем результирующий файл статистики
            //using (StreamWriter writer = new StreamWriter(resultFile, false, Encoding.GetEncoding(1251)))
            //{
            //    writer.WriteLine(zagolovok);

            //    int i = 0;

            //    foreach (var item in dictionaryPersoOtrabotkaOld)
            //    {
            //        i++;
            //        writer.Write(i + ";");
            //        writer.WriteLine(item.Value.ToString());
            //    }
            //}
        }

        //------------------------------------------------------------------------------------------       
        //Импортируем в коллекции данных из Персо данные из РК АСВ
        private static void ImportData(List<DataFromPersoDB> list_perso, List<DataFromRKASVDB> listDataFromDB)
        {
            foreach (var itemDataPerso in list_perso)
            {
                foreach (var itemDataDB in listDataFromDB)
                {
                    if (itemDataPerso.regNum == itemDataDB.insurer_reg_num)
                    {
                        itemDataPerso.dataPostPFR = itemDataDB.insurer_reg_start_date;
                        itemDataPerso.dataPostRO = itemDataDB.INSURER_REG_DATE_RO;
                        itemDataPerso.dataSnyatRO = itemDataDB.INSURER_UNREG_DATE_RO;
                        itemDataPerso.kategory = itemDataDB.category_code;
                        itemDataPerso.inn = itemDataDB.insurer_inn;
                        itemDataPerso.kpp = itemDataDB.insurer_kpp;
                    }
                }

            }
        }

        //------------------------------------------------------------------------------------------
        public static string ConvertRegNom(string regNom)
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

                return "";
            }
        }

        //------------------------------------------------------------------------------------------
        private static string ConvertRaion(string raion)
        {
            if (raion.Count() == 1)
            {
                return "042-00" + raion;
            }
            if (raion.Count() == 3)
            {
                return "042-" + raion;
            }
            else
            {
                return "042-0" + raion;
            }
        }



        //------------------------------------------------------------------------------------------
        //Открываем поток для чтения из файла и выбираем нужные позиции           
        public static void CreatePersoFile(List<DataFromPersoDB> list_dataFromPersoDB, List<DataFromRKASVDB> list_dataFromPKASVDB)
        {
            try
            {
                if (list_dataFromPersoDB.Count() == 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Из БД Perso не выбрано никаких данных.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (list_dataFromPKASVDB.Count() == 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Из БД РК АСВ не выбрано никаких данных.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    //Очищаем коллекции для данных из файлов                

                    list_DOP.Clear();
                    list_OTMN.Clear();
                    list_DOP_NO_prinyato.Clear();
                    list_DOP_NO_proveren.Clear();
                    list_DOP_partial_prinyato.Clear();
                    list_DOP_prinyato.Clear();
                    list_ISXD_NO_prinyato.Clear();
                    list_ISXD_NO_proveren.Clear();
                    list_ISXD_partial_prinyato.Clear();
                    list_ISXD_prinyato.Clear();
                    list_noName.Clear();
                    list_OTMN_prinyato.Clear();
                    list_OTMN_partial_prinyato.Clear();
                    list_OTMN_NO_proveren.Clear();
                    list_OTMN_NO_prinyato.Clear();
                    list_errorKvitanciya.Clear();



                    //Импортируем в коллекцию данные из РК АСВ
                    ImportData(list_dataFromPersoDB, list_dataFromPKASVDB);

                    //Формируем коллекции по типам сведений и результату обработки
                    foreach (var itemDataFromPersoDB in list_dataFromPersoDB)
                    {
                        if (itemDataFromPersoDB.typeSved == "ИСХОДНАЯ" && itemDataFromPersoDB.resultat == "Принят")
                        {
                            list_ISXD_prinyato.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ИСХОДНАЯ" && itemDataFromPersoDB.resultat == "Принят частично")
                        {
                            list_ISXD_partial_prinyato.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ИСХОДНАЯ" && itemDataFromPersoDB.resultat == "Не проверен")
                        {
                            list_ISXD_NO_proveren.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ИСХОДНАЯ" && itemDataFromPersoDB.resultat == "Не принят")
                        {
                            list_ISXD_NO_prinyato.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ДОПОЛНЯЮЩАЯ" && itemDataFromPersoDB.resultat == "Принят")
                        {
                            list_DOP_prinyato.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ДОПОЛНЯЮЩАЯ" && itemDataFromPersoDB.resultat == "Принят частично")
                        {
                            list_DOP_partial_prinyato.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ДОПОЛНЯЮЩАЯ" && itemDataFromPersoDB.resultat == "Не проверен")
                        {
                            list_DOP_NO_proveren.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ДОПОЛНЯЮЩАЯ" && itemDataFromPersoDB.resultat == "Не принят")
                        {
                            list_DOP_NO_prinyato.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ОТМЕНЯЮЩАЯ" && itemDataFromPersoDB.resultat == "Принят")
                        {
                            list_OTMN_prinyato.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ОТМЕНЯЮЩАЯ" && itemDataFromPersoDB.resultat == "Принят частично")
                        {
                            list_OTMN_partial_prinyato.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ОТМЕНЯЮЩАЯ" && itemDataFromPersoDB.resultat == "Не проверен")
                        {
                            list_OTMN_NO_proveren.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ОТМЕНЯЮЩАЯ" && itemDataFromPersoDB.resultat == "Не принят")
                        {
                            list_OTMN_NO_prinyato.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ДОПОЛНЯЮЩАЯ")
                        {
                            list_DOP.Add(itemDataFromPersoDB);
                        }
                        if (itemDataFromPersoDB.typeSved == "ОТМЕНЯЮЩАЯ")
                        {
                            list_OTMN.Add(itemDataFromPersoDB);
                        }
                        if ((itemDataFromPersoDB.typeSved != "ОТМЕНЯЮЩАЯ" && itemDataFromPersoDB.typeSved != "ДОПОЛНЯЮЩАЯ" && itemDataFromPersoDB.typeSved != "ИСХОДНАЯ") ||
                            (itemDataFromPersoDB.resultat != "Принят" && itemDataFromPersoDB.resultat != "Принят частично" && itemDataFromPersoDB.resultat != "Не проверен" && itemDataFromPersoDB.resultat != "Не принят") ||
                            itemDataFromPersoDB.curator == "")
                        {
                            list_noName.Add(itemDataFromPersoDB);
                        }
                    }





                    //Создаем файлы:
                    //Заголовок для файлов
                    string zagolovok = "КодЗап" + ";" + "Район" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "ОтчМесяц" + ";" + "ОтчГод" + ";" + "Тип сведений" + ";" + "Дата представления" + ";"
                        + "Результат проверки" + ";" + "Дата проверки" + ";" + "Количество ЗЛ в файле" + ";" + "ЗЛ принято" + ";" + "ЗЛ не принято" + ";" + "Статус квитанции" + ";"
                        + "Специалист" + ";" + "Способ представления" + ";" + "Куратор" + ";" + "УП по данным УПФР" + ";" + "Категория" + ";" + "ИНН" + ";"
                        + "Дата постановки в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";" + "Дата уведомления" + ";" + "Дата контроля" + ";";
                    //+ "Дата постановки в ПФР" + ";" + "Дата снятия в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";";
                    //+ "Наличие ДОП формы" + ";" + "Наличие ОТМН формы" + ";" + "Tout_прошлый кол-во ЗЛ" + ";" + "Tout_текущий кол-во ЗЛ" + ";"
                    //+ "Отработано? ДА|НЕТ" + ";" + "Дата отправки уведомления для НЕТ (лично|БПИ)" + ";" + "Дата отправки уведомления для НЕТ (лично|УП)" + ";" + "Примечание" + ";";



                    //1. Формируем файл статистики "Perso_Реестр_общий_"
                    string resultFile_list_persoAll = IOoperations.katalogOut + @"\" + @"2_Perso_Реестр_общий_" + DateTime.Now.ToShortDateString() + ".csv";

                    if (File.Exists(resultFile_list_persoAll))
                    {
                        IOoperations.FileDelete(resultFile_list_persoAll);
                    }

                    //TODO: НЕ Закомментировал создание CSV-файла статистики "Perso_Реестр_общий_"
                    WriteLogs(resultFile_list_persoAll, zagolovok, list_dataFromPersoDB);

                    //2. Формируем Excel-файл статистики "Perso_Реестр_общий_"
                    //TODO: Закомментировал создание Excel-файла статистики "Perso_Реестр_общий_"
                    //CreateExcelFilePersoDB.CreateNewExcelFile(list_dataFromPersoDB);


                    //3. Формируем файл статистики "Perso_Отработка"
                    string zagolovokError = "№ п/п" + ";" + "КодЗап" + ";" + "Район" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "ОтчМесяц" + ";" + "ОтчГод" + ";" + "Тип сведений" + ";" + "Дата представления" + ";" + "Категория" + ";"
                                                + "Дата постановки в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";" + "Результат проверки" + ";" + "Дата проверки" + ";" + "Количество ЗЛ в файле" + ";" + "ЗЛ принято" + ";" + "ЗЛ не принято" + ";"
                                                + "Статус квитанции" + ";" + "Специалист" + ";" + "Способ представления" + ";" + "Куратор" + ";" + "УП по данным УПФР" + ";"
                                                + "Дата направления уведомления страхователю" + ";" + "Контрольная дата для исправления (3 дня)" + ";" + "Исправлено (да|нет|не требуется)" + ";"
                                                + "Дата направления реестра в УПФР (в случае неисправления)" + ";" + "Дата исправления ошибки (после направления реестра УПФР)" + ";" + "Примечание" + ";"
                                                + "Результат контроля (руководитель)" + ";";

                    string resultFile_list_error = IOoperations.katalogOut + @"\" + @"6_Perso_Отработка_" + DateTime.Now.ToShortDateString() + ".csv";

                    //Проверяем на наличие файла отчета, если существует - удаляем
                    if (File.Exists(resultFile_list_error))
                    {
                        IOoperations.FileDelete(resultFile_list_error);
                    }
                                        
                    WriteLogsError(resultFile_list_error, zagolovokError, list_dataFromPersoDB, SelectDataFromPersoFile.dictionaryPersoOtrabotkaOld);



                    //4. Формируем файл статистики "Perso_noName"
                    string resultFile_list_noName = IOoperations.katalogOut + @"\" + @"7_Perso_noName_" + DateTime.Now.ToShortDateString() + ".csv";

                    //Проверяем на наличие файла отчета, если существует - удаляем
                    if (File.Exists(resultFile_list_noName))
                    {
                        IOoperations.FileDelete(resultFile_list_noName);
                    }

                    if (list_noName.Count() != 0)
                    {
                        WriteLogs(resultFile_list_noName, zagolovok, list_noName);
                    }



                    //5. Формируем файл статистики "Perso_СтатусКвитанции_Error_"

                    string zagolovok_errorKvitanc = "№ п/п" + ";" + "КодЗап" + ";" + "Район" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "ОтчМесяц" + ";" + "ОтчГод" + ";" + "Тип сведений" + ";" + "Дата представления" + ";"
                        + "Результат проверки" + ";" + "Дата проверки" + ";" + "Количество ЗЛ в файле" + ";" + "ЗЛ принято" + ";" + "ЗЛ не принято" + ";" + "Статус квитанции" + ";"
                        + "Специалист" + ";" + "Способ представления" + ";" + "Куратор" + ";" + "УП по данным УПФР" + ";" + "Категория" + ";" + "ИНН" + ";"
                        + "Дата постановки в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";" + "Дата уведомления" + ";" + "Дата контроля" + ";";

                    string resultFile_errorKvitanc = IOoperations.katalogOut + @"\" + @"7_Perso_СтатусКвитанции_Error_" + DateTime.Now.ToShortDateString() + ".csv";

                    //Проверяем на наличие файла отчета, если существует - удаляем
                    if (File.Exists(resultFile_errorKvitanc))
                    {
                        IOoperations.FileDelete(resultFile_errorKvitanc);
                    }

                    WriteLogsErrorKvitanc(resultFile_errorKvitanc, zagolovok_errorKvitanc, list_dataFromPersoDB, list_errorKvitanciya);



                    //6. Формируем Excel-файл статистики "Perso_Отработка"
                    //TODO: НЕ Закомментировал создание Excel-файла статистики "Perso_Отработка"
                    CreateExcelFilePersoOtrabotka.CreateNewExcelFile(SelectDataFromPersoFile.dictionaryPersoOtrabotkaOld);
                }
            }
            catch (InvalidDataException ex)
            {
                IOoperations.WriteLogError(ex.ToString());

            }
            catch (IOException ex)
            {
                IOoperations.WriteLogError(ex.ToString());

            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

            }
        }

        private static void WriteLogsErrorKvitanc(string resultFile_list_error, string zagolovokError, List<DataFromPersoDB> list_dataFromPersoDB, List<DataFromPersoDB> list_errorKvitanciya)
        {
            foreach (var itemDataFromPersoDB in list_dataFromPersoDB)
            {
                //"Квитанция не поступила из ВИО"     "Обработан"
                if (
                    (itemDataFromPersoDB.statusKvitanc == "Зарегистрирован" || itemDataFromPersoDB.statusKvitanc == "Отказ в обработке"
                    || itemDataFromPersoDB.statusKvitanc == "Отказ в регистрации" || itemDataFromPersoDB.statusKvitanc == "") &&
                    itemDataFromPersoDB.resultat != "Не принят"
                    )
                {
                    list_errorKvitanciya.Add(itemDataFromPersoDB);
                }
                else
                {
                    continue;
                }
            }

            if (list_errorKvitanciya.Count() != 0)
            {
                //формируем результирующий файл статистики
                using (StreamWriter writer = new StreamWriter(resultFile_list_error, false, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovokError);

                    int i = 0;

                    foreach (var item in list_errorKvitanciya)
                    {
                        i++;
                        writer.Write(i + ";");
                        writer.WriteLine(item.ToStringPersoALL());
                    }
                }
            }

        }
    }

}



