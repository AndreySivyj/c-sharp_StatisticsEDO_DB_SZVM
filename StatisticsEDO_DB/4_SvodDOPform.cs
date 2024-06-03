using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace StatisticsEDO_DB
{

    #region считанная из файла информация

    public class DataFromDOPform
    {
        public string raion;
        public string regNum;
        public string kolZL;
        public string kolZLgood;
        public string kolZLbad;

        public DataFromDOPform(string raion = "", string regNum = "", string kolZL = "", string kolZLgood = "", string kolZLbad = "")
        {
            this.raion = raion;
            this.regNum = regNum;
            this.kolZL = kolZL;
            this.kolZLgood = kolZLgood;
            this.kolZLbad = kolZLbad;
        }


        public override string ToString()
        {
            return raion + ";" + regNum + ";" + kolZL + ";" + kolZLgood + ";" + kolZLbad + ";";
        }
    }

    #endregion


    static class SvodDOPform
    {
        public static Dictionary<string, DataFromDOPform> dictionarySvodDOPform = new Dictionary<string, DataFromDOPform>();

        //------------------------------------------------------------------------------------------
        //Формируем сводную информацию на основании реестра всех ОТМН форм
        public static void CreateSvod(List<DataFromPersoDB> list_DOP)
        {
            try
            {
                foreach (var item_list_DOP in list_DOP)
                {
                    if (item_list_DOP.resultat == "Принят частично" || item_list_DOP.resultat == "Принят" || item_list_DOP.resultat == "Не проверен" || item_list_DOP.resultat == "Не принят")
                    {
                        //Добавляем выбранные данные в коллекцию
                        DataFromDOPform tmpDataFromDOPform = new DataFromDOPform();
                        if (dictionarySvodDOPform.TryGetValue(item_list_DOP.regNum, out tmpDataFromDOPform))
                        {
                            //есть рег.Номер в словаре
                            dictionarySvodDOPform[item_list_DOP.regNum].kolZL = (Convert.ToInt32(dictionarySvodDOPform[item_list_DOP.regNum].kolZL) + Convert.ToInt32(item_list_DOP.kolZL)).ToString();
                            dictionarySvodDOPform[item_list_DOP.regNum].kolZLgood = (Convert.ToInt32(dictionarySvodDOPform[item_list_DOP.regNum].kolZLgood) + Convert.ToInt32(item_list_DOP.kolZLgood)).ToString();
                            dictionarySvodDOPform[item_list_DOP.regNum].kolZLbad = (Convert.ToInt32(dictionarySvodDOPform[item_list_DOP.regNum].kolZLbad) + Convert.ToInt32(item_list_DOP.kolZLbad)).ToString();
                        }
                        else
                        {
                            //нет рег.Номера в словаре
                            dictionarySvodDOPform.Add(item_list_DOP.regNum, new DataFromDOPform(item_list_DOP.raion, item_list_DOP.regNum, item_list_DOP.kolZL, item_list_DOP.kolZLgood, item_list_DOP.kolZLbad));
                        }
                    }
                }

                string zagolovok = "raion" + ";" + "regNum" + ";" + "kolZL" + ";" + "kolZLgood" + ";" + "kolZLbad" + ";";
                string resultFile_SvodDOP = IOoperations.katalogOut + @"\" + @"_Perso_SvodDOP_" + DateTime.Now.ToShortDateString() + ".csv";

                //Проверяем на наличие файла отчета, если существует - удаляем
                if (File.Exists(resultFile_SvodDOP))
                {
                    IOoperations.FileDelete(resultFile_SvodDOP);
                }

                //TODO: Закомментировал создание файла статистики "Perso_SvodDOP"
                //CreatResultFromDictionary(zagolovok, dictionarySvodDOPform, resultFile_SvodDOP);

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
        //Создаем результирующий файл
        private static void CreatResultFromDictionary(string zagolovok, Dictionary<string, DataFromDOPform> dictionarySvodDOPform, string nameFile)
        {
            try
            {
                //Формируем результирующий файл статистики
                using (StreamWriter writer = new StreamWriter(nameFile, false, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovok);

                    foreach (var item in dictionarySvodDOPform)
                    {
                        writer.WriteLine(item.Value.ToString());
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
        //Ищем данные в коллекциях из реестра Perso(OTMN, по статусам обработки) и наполняем общий файл
        public static void InsertInDictionaryCollectionDOPdata(Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL, Dictionary<string, DataFromDOPform> dictionarySvodDOPform)
        {
            //ищем в коллекции
            foreach (var itemDictionarySvodDOPform in dictionarySvodDOPform)
            {
                DataFromPersoDB tmpData = new DataFromPersoDB();
                if (dictionaryUnikRegNomPersoALL.TryGetValue(itemDictionarySvodDOPform.Key, out tmpData))
                {
                    dictionaryUnikRegNomPersoALL[itemDictionarySvodDOPform.Key].dopFormAvailability = "ДА";
                    dictionaryUnikRegNomPersoALL[itemDictionarySvodDOPform.Key].dopFormKolZL = itemDictionarySvodDOPform.Value.kolZL;
                    dictionaryUnikRegNomPersoALL[itemDictionarySvodDOPform.Key].dopFormKolZLgood = itemDictionarySvodDOPform.Value.kolZLgood;
                    dictionaryUnikRegNomPersoALL[itemDictionarySvodDOPform.Key].dopFormKolZLbad = itemDictionarySvodDOPform.Value.kolZLbad;
                }
                else
                {
                    continue;
                }
            }


            ////Добавляем в общую коллекцию уникальные рег.номера с требуемым статусом
            //foreach (var item in dictionaryUnikRegNomPersoALL)
            //{
            //    DataFromPersoDB tmpData = SelectDataFromListDataFromPersoDB(item.Key, tmpDictionary_perso);
            //    if (tmpData.regNum == "")
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        DataFromPersoDB tmpData3 = new DataFromPersoDB();
            //        if (dictionaryUnikRegNomPersoALL.TryGetValue(item.Key, out tmpData3))
            //        {
            //            dictionaryUnikRegNomPersoALL[item.Key].otmnFormAvailability = tmpData.resultat;
            //        }
            //        else
            //        {
            //            //Console.WriteLine("проверка");
            //            continue;
            //        }
            //    }
            //}
        }


    }
}
