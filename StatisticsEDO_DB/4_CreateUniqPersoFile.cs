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
    //------------------------------------------------------------------------------------------
    //На основании коллекции "уникальные регномера из реестра Perso" выбираем поэтапно из коллекций "данные из реестра Perso" (ИСХ, ДОП) статусы обработки
    #region Выбор данных из файла
    static class SelectDataForResultFile
    {
        public static Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL = new Dictionary<string, DataFromPersoDB>();    //Коллекция уникальных регНомеров из реестра Perso и статуса обработки сведений страхователя

        //private static Dictionary<string, DataFromPersoDB> needFix_persoALL = new Dictionary<string, DataFromPersoDB>();    //Коллекция уникальных регНомеров из реестра Perso и статусов, требующих обработки

        private static Dictionary<string, DataFromPersoDB> tmpDictionary_perso = new Dictionary<string, DataFromPersoDB>();  //Временная коллекция для поэтапного наполнения 


        //Коллекция уникальных регНомеров из реестра Perso, статус "09_НЕТ ДЕЯТЕЛЬНОСТИ"
        public static Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPerso_NFXD = new Dictionary<string, DataFromPersoDB>();



        //------------------------------------------------------------------------------------------        
        //На основании коллекции "уникальные регномера из реестра Perso" выбираем поэтапно из коллекций "данные из реестра Perso" статусы обработки        

        public static void SelectResultData(SortedSet<string> persoUnikRegNom,
                                                    Dictionary<string, DataFromToutFileSvod> dictionaryDataToutSvodOld, Dictionary<string, DataFromToutFileSvod> dictionaryDataToutSvodNew)
        {
            //Очищаем коллекции для данных из файлов

            //needFix_persoALL.Clear();

            dictionaryUnikRegNomPersoALL.Clear();
            dictionaryUnikRegNomPerso_NFXD.Clear();


            if (persoUnikRegNom.Count() == 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Нет данных для обработки.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {

                try
                {
                    //Console.WriteLine("Количество записей в \"persoUnikRegNom\": {0}", persoUnikRegNom.Count());

                    /*
                    //------------------------------------------------------------------------------------------
                    //1. Ищем в "ИСХ-принято"
                    tmpDictionary_perso.Clear();

                    //создаем коллекцию-словарь "ИСХ-принято"
                    foreach (var listData in SelectDataFromPersoDB.list_ISXD_prinyato)
                    {
                        DataFromPersoDB tmpData = new DataFromPersoDB();
                        if (tmpDictionary_perso.TryGetValue(listData.regNum, out tmpData))
                        {
                            continue;
                        }
                        else
                        {
                            tmpDictionary_perso.Add(listData.regNum, listData);
                        }
                    }

                    //Добавляем в общую коллекцию уникальные рег.номера со статусом "ИСХ-принято"
                    foreach (var regNom in persoUnikRegNom)
                    {
                        DataFromPersoDB tmpData = SelectDataFromListDataFromPersoDB(ConvertRegNom(regNom), tmpDictionary_perso);
                        if (tmpData.regNum == "")
                        {
                            continue;
                        }
                        else
                        {
                            dictionaryUnikRegNomPersoALL.Add(regNom, tmpData);
                        }
                    }

                    Console.WriteLine("1. Количество записей в \"dictionaryUnikRegNomPersoALL\" {0}:", dictionaryUnikRegNomPersoALL.Count());                    
                    */

                    //1. Ищем в "ИСХ-принято"                    
                    InsertInDictionaryCollection(persoUnikRegNom, CreateDataFromPersoSelect.list_ISXD_prinyato);

                    //2. Ищем в "ДОП-принято"
                    InsertInDictionaryCollection(persoUnikRegNom, CreateDataFromPersoSelect.list_DOP_prinyato);

                    //3. Ищем в "ИСХ-принято частично"
                    InsertInDictionaryCollection(persoUnikRegNom, CreateDataFromPersoSelect.list_ISXD_partial_prinyato);

                    //4. Ищем в "ИСХ-не проверен"
                    InsertInDictionaryCollection(persoUnikRegNom, CreateDataFromPersoSelect.list_ISXD_NO_proveren);

                    //5. Ищем в "ДОП-принято частично"
                    InsertInDictionaryCollection(persoUnikRegNom, CreateDataFromPersoSelect.list_DOP_partial_prinyato);

                    //6. Ищем в "ДОП-не проверен"
                    InsertInDictionaryCollection(persoUnikRegNom, CreateDataFromPersoSelect.list_DOP_NO_proveren);

                    //7. Ищем в "ИСХ-не принято"
                    InsertInDictionaryCollection(persoUnikRegNom, CreateDataFromPersoSelect.list_ISXD_NO_prinyato);

                    //8. Ищем в "ДОП-не принято"
                    InsertInDictionaryCollection(persoUnikRegNom, CreateDataFromPersoSelect.list_DOP_NO_prinyato);

                    //9. Ищем в "ОТМН"
                    //InsertInDictionaryCollection(persoUnikRegNom, SelectDataFromPersoDB.list_OTMN); //Тогда подтягивается результат обработки "Принят"


                    //10. Добавляем в общую коллекцию уникальные рег.номера со статусом "нет статуса"
                    foreach (var regNom in persoUnikRegNom)
                    {
                        DataFromPersoDB tmpData = SelectDataFromListDataFromPersoDB(regNom, dictionaryUnikRegNomPersoALL);
                        if (tmpData.regNum == "")
                        {
                            tmpData.regNum = regNom;
                            tmpData.resultat = "нет статуса";

                            dictionaryUnikRegNomPersoALL.Add(regNom, tmpData);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //11. Ищем в "ДОП-принято" ("Наличие ДОП формы")
                    InsertInDictionaryCollectionDopStatus(dictionaryUnikRegNomPersoALL, CreateDataFromPersoSelect.list_DOP_prinyato);
                    //12. Ищем в "ДОП-принято частично" ("Наличие ДОП формы")
                    InsertInDictionaryCollectionDopStatus(dictionaryUnikRegNomPersoALL, CreateDataFromPersoSelect.list_DOP_partial_prinyato);
                    //13. Ищем в "ДОП-не проверен" ("Наличие ДОП формы")
                    InsertInDictionaryCollectionDopStatus(dictionaryUnikRegNomPersoALL, CreateDataFromPersoSelect.list_DOP_NO_proveren);
                    //14. Ищем в "ДОП-не принято" ("Наличие ДОП формы")
                    InsertInDictionaryCollectionDopStatus(dictionaryUnikRegNomPersoALL, CreateDataFromPersoSelect.list_DOP_NO_prinyato);

                    //TODO: Закомментировал поэтапное наполнение коллекции "УникРегНомерPerso_Общий_реестр" статусами наличия ОТМН форм. Замена на SvodOTMNform.InsertInDictionaryCollectionOTMNdata(dictionaryUnikRegNomPersoALL, CreateDataFromPersoSelect.list_OTMN_prinyato)
                    /*
                    //15. Ищем в "OTMN-принято" ("Наличие OTMN формы")
                    InsertInDictionaryCollectionOTMNStatus(dictionaryUnikRegNomPersoALL, CreateDataFromPersoSelect.list_OTMN_prinyato);
                    //16. Ищем в "OTMN-принято частично" ("Наличие OTMN формы")
                    InsertInDictionaryCollectionOTMNStatus(dictionaryUnikRegNomPersoALL, CreateDataFromPersoSelect.list_OTMN_partial_prinyato);
                    //17. Ищем в "OTMN-не проверен" ("Наличие OTMN формы")
                    InsertInDictionaryCollectionOTMNStatus(dictionaryUnikRegNomPersoALL, CreateDataFromPersoSelect.list_OTMN_NO_proveren);
                    //18. Ищем в "OTMN-не принято" ("Наличие OTMN формы")
                    InsertInDictionaryCollectionOTMNStatus(dictionaryUnikRegNomPersoALL, CreateDataFromPersoSelect.list_OTMN_NO_prinyato);
                    */

                    //15. Ищем наличие OTMN формы и наполняем значениями коллекции "dictionarySvodOTMNform" коллекцию "dictionaryUnikRegNomPersoALL"
                    SvodOTMNform.InsertInDictionaryCollectionOTMNdata(dictionaryUnikRegNomPersoALL, SvodOTMNform.dictionarySvodOTMNform);

                    //16. Формируем сводные данные по ЗЛ (ИСХ + ДОП) и наполняем значениями коллекции "dictionarySvod_ISX_DOP_form" коллекцию "dictionaryUnikRegNomPersoALL"
                    Svod_ISX_DOP_form.InsertInDictionaryCollectionOTMNdata(dictionaryUnikRegNomPersoALL, Svod_ISX_DOP_form.dictionarySvod_ISX_DOP_form);

                    //17. Ищем наличие OTMN формы и наполняем значениями коллекции "dictionarySvodOTMNform" коллекцию "dictionaryUnikRegNomPersoALL"
                    SvodDOPform.InsertInDictionaryCollectionDOPdata(dictionaryUnikRegNomPersoALL, SvodDOPform.dictionarySvodDOPform);

                    //Импорт данных из старого и нового tout
                    InsertInDictionaryCollectionTout(dictionaryUnikRegNomPersoALL, dictionaryDataToutSvodOld, "Old");
                    InsertInDictionaryCollectionTout(dictionaryUnikRegNomPersoALL, dictionaryDataToutSvodNew, "New");



                    //18. Наполняем dictionaryUnikRegNomPersoALL верным способом представления сведений (на основании выборки из общего реестра всех принятых документов)

                    SelectSposobPredstavleniya.SelectSposobPerdstavl();



                    //19. Наполняем dictionaryUnikRegNomPersoALL верным количеством ЗЛ в БД Perso (запрос к БД, с учетом отмененных снилс)

                    //------------------------------------------------------------------------------------------
                    //Наполняем данными коллекции dictionarySposobPredstavleniya коллекцию SelectDataForResultFile.dictionaryUnikRegNomPersoALL                
                    foreach (var itemCountZLPerso in Program.dictionaryDataCountZLPerso)
                    {
                        DataFromPersoDB tmpDataFromPersoDB = new DataFromPersoDB();
                        if (SelectDataForResultFile.dictionaryUnikRegNomPersoALL.TryGetValue(itemCountZLPerso.Key, out tmpDataFromPersoDB))
                        {
                            SelectDataForResultFile.dictionaryUnikRegNomPersoALL[itemCountZLPerso.Key].kolZLBDPerso = itemCountZLPerso.Value.ToString();
                        }
                    }

                    //Console.WriteLine("Количество записей в \"dictionaryUnikRegNomPersoALL\": {0}", dictionaryUnikRegNomPersoALL.Count());



                    //------------------------------------------------------------------------------------------
                    //20. Наполняем dictionaryUnikRegNomPersoALL значением поля "status_id"
                    //Наполняем данными из РК АСВ 

                    foreach (var itemDataFromPKASVDB in Program.list_dataFromPKASVDB)
                    {

                        DataFromPersoDB tmpDataFromPersoDB = new DataFromPersoDB();
                        if (SelectDataForResultFile.dictionaryUnikRegNomPersoALL.TryGetValue(itemDataFromPKASVDB.insurer_reg_num, out tmpDataFromPersoDB))
                        {
                            SelectDataForResultFile.dictionaryUnikRegNomPersoALL[itemDataFromPKASVDB.insurer_reg_num].status_id = itemDataFromPKASVDB.status_id;
                        }

                    }


                    //------------------------------------------------------------------------------------------
                    //21. Выбираем из dictionaryUnikRegNomPersoALL записи со значением поля "status_id" = "09_НЕТ ДЕЯТЕЛЬНОСТИ"
                    foreach (var item in dictionaryUnikRegNomPersoALL)
                    {
                        if (item.Value.status_id == "09_НЕТ ДЕЯТЕЛЬНОСТИ")
                        {
                            dictionaryUnikRegNomPerso_NFXD[item.Key] = item.Value;
                        }
                    }

                    //Заголовок для файлов
                    string zagolovok = "КодЗап" + ";" + "Район" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "ОтчМесяц" + ";" + "ОтчГод" + ";" + "Тип сведений" + ";" + "Дата представления" + ";"
                        + "Результат проверки" + ";" + "Дата проверки" + ";" + "Количество ЗЛ за отч. период (принято, не проверено)" + ";" + "Статус квитанции" + ";"
                        + "Специалист (последний принятый документ)" + ";" + "Способ представления (в отчетном периоде)" + ";" + "Куратор" + ";" + "УП" + ";" + "Категория" + ";" + "ИНН" + ";"
                        //+ "Дата постановки в ПФР" + ";" + "Дата снятия в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";"
                        + "Дата постановки в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";"
                        + "Наличие ДОП формы" + ";" + "Количество ЗЛ в ДОП формах" + ";" + "ЗЛ принято (ДОП формы)" + ";" + "ЗЛ не принято (ДОП формы)" + ";"
                        + "Наличие ОТМН формы" + ";" + "Количество ЗЛ в ОТМН формах" + ";" + "ЗЛ принято (ОТМН формы)" + ";" + "ЗЛ не принято (ОТМН формы)" + ";"
                        + "Tout_прошлый кол-во ЗЛ" + ";" + "Tout_текущий кол-во ЗЛ" + ";" + "Разница между Количество_ЗЛ_за_отч._период и Tout_текущий_кол-во_ЗЛ" + ";" + "Статус" + ";";
                    //+ "Дата уведомления" + ";" + "Отработано? ДА|НЕТ" + ";" + "Дата отправки уведомления для НЕТ (лично|БПИ)" + ";" + "Дата отправки уведомления для НЕТ (лично|УП)" + ";" + "Примечание" + ";";
                    
                    //public static List<string> list_table_zagolovki = new List<string>();

                    if (dictionaryUnikRegNomPersoALL.Count() != 0)
                    {
                        //имя файла
                        string resultFile_dictionaryUnikRegNomPersoALL = IOoperations.katalogOut + @"\" + @"3_УникРегНомерPerso_Общий_реестр_" + DateTime.Now.ToShortDateString() + "_.csv";

                        //Проверяем на наличие файла отчета, если существует - удаляем
                        if (File.Exists(resultFile_dictionaryUnikRegNomPersoALL))
                        {
                            IOoperations.FileDelete(resultFile_dictionaryUnikRegNomPersoALL);
                        }

                        //TODO: НЕ закомментировал создание CSV-файла статистики "УникРегНомерPerso_Общий_реестр"                     
                        //создаем общий файл
                        WriteLogs(resultFile_dictionaryUnikRegNomPersoALL, zagolovok, dictionaryUnikRegNomPersoALL);
                    }

                    //Формируем Excel-файл статистики "УникРегНомерPerso_Общий_реестр_"
                    //TODO: Закомментировал создание Excel-файла статистики "УникРегНомерPerso_Общий_реестр"
                    //TODO: Внимание! НЕ реализована печать поля "status_id"
                    //CreateExcelFileUniqPerso.CreateNewExcelFile(dictionaryUnikRegNomPersoALL);

                    if (dictionaryUnikRegNomPerso_NFXD.Count() != 0)
                    {
                        //имя файла
                        string resultFile_dictionaryUnikRegNomPerso_NFXD = IOoperations.katalogOut + @"\" + @"9_Статус_09_НЕТ ДЕЯТЕЛЬНОСТИ_" + DateTime.Now.ToShortDateString() + "_.csv";

                        //Проверяем на наличие файла отчета, если существует - удаляем
                        if (File.Exists(resultFile_dictionaryUnikRegNomPerso_NFXD))
                        {
                            IOoperations.FileDelete(resultFile_dictionaryUnikRegNomPerso_NFXD);
                        }

                        //создаем общий файл
                        WriteLogs(resultFile_dictionaryUnikRegNomPerso_NFXD, zagolovok, dictionaryUnikRegNomPerso_NFXD);

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Внимание! Есть страхователи со статусом \"НЕТ ДЕЯТЕЛЬНОСТИ\" .");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Нет страхователей со статусом \"НЕТ ДЕЯТЕЛЬНОСТИ\" .");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine();
                    }

                    /*

                    //имя файла
                    string resultFile_dictionaryUnikRegNomPersoALL_NoProveren = IOoperations.katalogOutCurators + @"\" + @"УникРегНомерPerso_Не_проверен" + ".csv";
                    //создаем файл с "Не проверен"
                    WriteLogsNoProveren(resultFile_dictionaryUnikRegNomPersoALL_NoProveren, zagolovok, dictionaryUnikRegNomPersoALL);
                    
                    string zagolovokError = "Район" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "ОтчМесяц" + ";" + "ОтчГод" + ";" + "Тип сведений" + ";" + "Дата представления" + ";"
                        + "Результат проверки" + ";" + "Дата проверки" + ";" + "Количество ЗЛ в файле" + ";" + "ЗЛ принято" + ";" + "ЗЛ не принято" + ";" + "Статус квитанции" + ";"
                        + "Специалист" + ";" + "Способ представления" + ";" + "Куратор" + ";" + "УП" + ";" + "Категория" + ";" + "ИНН" + ";"
                        //+ "Дата постановки в ПФР" + ";" + "Дата снятия в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";"
                        + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";"
                        + "Наличие ДОП формы" + ";" + "Наличие ОТМН формы" + ";" + "Tout_прошлый кол-во ЗЛ" + ";" + "Tout_текущий кол-во ЗЛ" + ";"
                        + "Дата уведомления" + ";" + "Отработано? ДА|НЕТ|НЕ ТРЕБУЕТСЯ" + ";" + "Дата отправки уведомления для НЕТ (лично|БПИ)" + ";" + "Дата отправки уведомления для НЕТ (лично|УП)" + ";" + "Примечание" + ";";
                                       
                    //имя файла
                    string resultFile_dictionaryUnikRegNomPersoALL_Error = IOoperations.katalogOutCurators + @"\" + @"УникРегНомерPerso_Отработка" + ".csv";
                    //создаем файл с "ошибками"
                    WriteLogsError(resultFile_dictionaryUnikRegNomPersoALL_Error, zagolovokError, dictionaryUnikRegNomPersoALL);
                    
                    */





                }
                catch (Exception ex)
                {
                    IOoperations.WriteLogError(ex.ToString());

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                //Console.WriteLine();
                Console.WriteLine("Количество записей в файле \"УникРегНомерPerso_Общий_реестр\": {0}", dictionaryUnikRegNomPersoALL.Count());
                //Console.WriteLine();
            }
        }

        //------------------------------------------------------------------------------------------
        //Ищем данные в коллекциях из реестра Perso (OTMN, по статусам обработки) и наполняем общий файл
        private static void InsertInDictionaryCollectionOTMNStatus(Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL, List<DataFromPersoDB> listDataFromPersoDB)
        {
            //очищаем временную коллекцию-словарь
            tmpDictionary_perso.Clear();

            //наполняем коллекцию-словарь
            foreach (var listData in listDataFromPersoDB)
            {
                DataFromPersoDB tmpData = new DataFromPersoDB();
                if (tmpDictionary_perso.TryGetValue(listData.regNum, out tmpData))
                {
                    continue;
                }
                else
                {
                    tmpDictionary_perso.Add(listData.regNum, listData);
                }
            }


            //Добавляем в общую коллекцию уникальные рег.номера с требуемым статусом
            foreach (var item in dictionaryUnikRegNomPersoALL)
            {
                DataFromPersoDB tmpData = SelectDataFromListDataFromPersoDB(item.Key, tmpDictionary_perso);
                if (tmpData.regNum == "")
                {
                    continue;
                }
                else
                {
                    DataFromPersoDB tmpData3 = new DataFromPersoDB();
                    if (dictionaryUnikRegNomPersoALL.TryGetValue(item.Key, out tmpData3))
                    {
                        dictionaryUnikRegNomPersoALL[item.Key].otmnFormAvailability = tmpData.resultat;
                    }
                    else
                    {
                        //Console.WriteLine("проверка");
                        continue;
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------
        //Ищем данные в коллекциях из реестра Perso (по статусам обработки) и наполняем общий файл
        private static void InsertInDictionaryCollection(SortedSet<string> persoUnikRegNom, List<DataFromPersoDB> listDataFromPersoDB)
        {
            //очищаем временную коллекцию-словарь
            tmpDictionary_perso.Clear();

            //наполняем коллекцию-словарь
            foreach (var listData in listDataFromPersoDB)
            {
                DataFromPersoDB tmpData = new DataFromPersoDB();
                if (tmpDictionary_perso.TryGetValue(listData.regNum, out tmpData))
                {
                    continue;
                }
                else
                {
                    tmpDictionary_perso.Add(listData.regNum, listData);
                }
            }

            //Добавляем в общую коллекцию уникальные рег.номера с требуемым статусом
            foreach (var regNom in persoUnikRegNom)
            {
                DataFromPersoDB tmpData = SelectDataFromListDataFromPersoDB(regNom, tmpDictionary_perso);
                if (tmpData.regNum == "")
                {
                    continue;
                }
                else
                {
                    DataFromPersoDB tmpData3 = new DataFromPersoDB();
                    if (dictionaryUnikRegNomPersoALL.TryGetValue(regNom, out tmpData3))
                    {
                        continue;
                    }
                    else
                    {
                        dictionaryUnikRegNomPersoALL.Add(regNom, tmpData);
                    }
                }
            }

            //Console.WriteLine("Количество записей в \"dictionaryUnikRegNomPersoALL\": {0}", dictionaryUnikRegNomPersoALL.Count());
        }

        //------------------------------------------------------------------------------------------
        //Ищем данные в коллекциях из реестра Perso (Dop, по статусам обработки) и наполняем общий файл
        private static void InsertInDictionaryCollectionDopStatus(Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL, List<DataFromPersoDB> listDataFromPersoDB)
        {

            //очищаем временную коллекцию-словарь
            tmpDictionary_perso.Clear();

            //наполняем коллекцию-словарь
            foreach (var listData in listDataFromPersoDB)
            {
                DataFromPersoDB tmpData = new DataFromPersoDB();
                if (tmpDictionary_perso.TryGetValue(listData.regNum, out tmpData))
                {
                    continue;
                }
                else
                {
                    tmpDictionary_perso.Add(listData.regNum, listData);
                }
            }


            //Добавляем в общую коллекцию уникальные рег.номера с требуемым статусом
            foreach (var item in dictionaryUnikRegNomPersoALL)
            {
                DataFromPersoDB tmpData = SelectDataFromListDataFromPersoDB(item.Key, tmpDictionary_perso);
                if (tmpData.regNum == "")
                {
                    continue;
                }
                else
                {
                    DataFromPersoDB tmpData3 = new DataFromPersoDB();
                    if (dictionaryUnikRegNomPersoALL.TryGetValue(item.Key, out tmpData3))
                    {
                        dictionaryUnikRegNomPersoALL[item.Key].dopFormAvailability = tmpData.resultat;
                    }
                    else
                    {
                        //Console.WriteLine("проверка");
                        continue;
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------
        //Ищем данные в коллекциях из реестра tout и наполняем общий файл
        private static void InsertInDictionaryCollectionTout(Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL, Dictionary<string, DataFromToutFileSvod> dictionaryDataToutSvod, string toutPriznak)
        {
            if (dictionaryDataToutSvod.Count() == 0)
            {
                Console.WriteLine("Из файла Tout ({0}) не выбрано ни одной записи.", toutPriznak);
            }
            else
            {
                //Добавляем в общую коллекцию уникальные рег.номера с требуемым статусом
                foreach (var item in dictionaryUnikRegNomPersoALL)
                {
                    DataFromToutFileSvod tmpData = SelectDataFromListDataFromToutFile(item.Key, dictionaryDataToutSvod);
                    if (tmpData.REG_NUM == "")
                    {
                        continue;
                    }
                    else
                    {
                        if (toutPriznak == "Old")
                        {
                            dictionaryUnikRegNomPersoALL[item.Key].toutOldCountZL = tmpData.countZL.ToString();
                        }
                        else
                        {
                            dictionaryUnikRegNomPersoALL[item.Key].toutNewCountZL = tmpData.countZL.ToString();
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------       
        //Выбираем данные из коллекции по регНомеру
        private static DataFromToutFileSvod SelectDataFromListDataFromToutFile(string regNom, Dictionary<string, DataFromToutFileSvod> dictionaryDataToutSvod)
        {
            DataFromToutFileSvod tmpData = new DataFromToutFileSvod();
            if (dictionaryDataToutSvod.TryGetValue(regNom, out tmpData))
            {
                return tmpData;
            }
            else
            {
                return new DataFromToutFileSvod();
            }
        }

        //------------------------------------------------------------------------------------------       
        //Выбираем данные из коллекции по регНомеру
        private static DataFromPersoDB SelectDataFromListDataFromPersoDB(string regNom, Dictionary<string, DataFromPersoDB> tmpDictionary_perso)
        {
            DataFromPersoDB tmpData = new DataFromPersoDB();
            if (tmpDictionary_perso.TryGetValue(regNom, out tmpData))
            {
                return tmpData;
            }
            else
            {
                return new DataFromPersoDB();
            }
        }

        //------------------------------------------------------------------------------------------       
        //Формируем результирующий файл статистики
        public static void WriteLogs(string resultFile, string zagolovok, Dictionary<string, DataFromPersoDB> dictionary_perso)
        {
            //формируем результирующий файл статистики
            using (StreamWriter writer = new StreamWriter(resultFile, false, Encoding.GetEncoding(1251)))
            {
                writer.WriteLine(zagolovok);

                foreach (var item in dictionary_perso)
                {
                    writer.WriteLine(item.Value.ToStringReestUniqReg());
                }
            }
        }

        //TODO: !!! Сделать выгрузку реестра ошибочных состояний квитанций
        //------------------------------------------------------------------------------------------       
        //Формируем результирующий файл статистики с ошибками
        private static void WriteLogsError(string resultFile, string zagolovok, Dictionary<string, DataFromPersoDB> dictionary_perso)
        {
            //формируем результирующий файл статистики
            using (StreamWriter writer = new StreamWriter(resultFile, false, Encoding.GetEncoding(1251)))
            {
                writer.WriteLine(zagolovok);

                foreach (var item in dictionary_perso)
                {
                    if (item.Value.resultat == "Принят частично" || item.Value.resultat == "Не принят" || item.Value.resultat == "нет статуса")
                    {
                        writer.WriteLine(item.Value.ToString());
                    }
                    else
                    {
                        continue;
                    }

                }
            }
        }

        //------------------------------------------------------------------------------------------       
        //Формируем результирующий файл статистики с "Не проверен"
        private static void WriteLogsNoProveren(string resultFile, string zagolovok, Dictionary<string, DataFromPersoDB> dictionary_perso)
        {
            //формируем результирующий файл статистики
            using (StreamWriter writer = new StreamWriter(resultFile, false, Encoding.GetEncoding(1251)))
            {
                writer.WriteLine(zagolovok);

                foreach (var item in dictionary_perso)
                {
                    if (item.Value.resultat == "Не проверен")
                    {
                        writer.WriteLine(item.Value.ToString());
                    }
                    else
                    {
                        continue;
                    }

                }
            }
        }
    }

    #endregion

}
