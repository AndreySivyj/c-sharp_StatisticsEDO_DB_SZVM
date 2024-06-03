using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace StatisticsEDO_DB
{

    #region считанная из файла информация

    public class DataFromOTMNform
    {
        public string raion;
        public string regNum;
        public string kolZL;
        public string kolZLgood;
        public string kolZLbad;

        public DataFromOTMNform(string raion = "", string regNum = "", string kolZL = "", string kolZLgood = "", string kolZLbad = "")
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


    static class SvodOTMNform
    {
        public static Dictionary<string, DataFromOTMNform> dictionarySvodOTMNform = new Dictionary<string, DataFromOTMNform>();

        //------------------------------------------------------------------------------------------
        //Формируем сводную информацию на основании реестра всех ОТМН форм
        public static void CreateSvod(List<DataFromPersoDB> list_OTMN)
        {
            try
            {
                foreach (var item_list_OTMN in list_OTMN)
                {
                    if (item_list_OTMN.resultat == "Принят частично" || item_list_OTMN.resultat == "Принят" || item_list_OTMN.resultat == "Не проверен" || item_list_OTMN.resultat == "Не принят")
                    {
                        //Добавляем выбранные данные в коллекцию
                        DataFromOTMNform tmpDataFromOTMNform = new DataFromOTMNform();
                        if (dictionarySvodOTMNform.TryGetValue(item_list_OTMN.regNum, out tmpDataFromOTMNform))
                        {
                            //есть рег.Номер в словаре
                            dictionarySvodOTMNform[item_list_OTMN.regNum].kolZL = (Convert.ToInt32(dictionarySvodOTMNform[item_list_OTMN.regNum].kolZL) + Convert.ToInt32(item_list_OTMN.kolZL)).ToString();
                            dictionarySvodOTMNform[item_list_OTMN.regNum].kolZLgood = (Convert.ToInt32(dictionarySvodOTMNform[item_list_OTMN.regNum].kolZLgood) + Convert.ToInt32(item_list_OTMN.kolZLgood)).ToString();
                            dictionarySvodOTMNform[item_list_OTMN.regNum].kolZLbad = (Convert.ToInt32(dictionarySvodOTMNform[item_list_OTMN.regNum].kolZLbad) + Convert.ToInt32(item_list_OTMN.kolZLbad)).ToString();
                        }
                        else
                        {
                            //нет рег.Номера в словаре
                            dictionarySvodOTMNform.Add(item_list_OTMN.regNum, new DataFromOTMNform(item_list_OTMN.raion, item_list_OTMN.regNum, item_list_OTMN.kolZL, item_list_OTMN.kolZLgood, item_list_OTMN.kolZLbad));
                        }
                    }
                }

                string zagolovok = "raion" + ";" + "regNum" + ";" + "kolZL" + ";" + "kolZLgood" + ";" + "kolZLbad" + ";";
                string resultFile_SvodOTMN = IOoperations.katalogOut + @"\" + @"_Perso_SvodOTMN_" + DateTime.Now.ToShortDateString() + ".csv";

                //Проверяем на наличие файла отчета, если существует - удаляем
                if (File.Exists(resultFile_SvodOTMN))
                {
                    IOoperations.FileDelete(resultFile_SvodOTMN);
                }

                //TODO: Закомментировал создание файла статистики "Perso_SvodOTMN"
                //CreatResultFromDictionary(zagolovok, dictionarySvodOTMNform, resultFile_SvodOTMN);

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
        private static void CreatResultFromDictionary(string zagolovok, Dictionary<string, DataFromOTMNform> dictionarySvodOTMNform, string nameFile)
        {
            try
            {
                //Формируем результирующий файл статистики
                using (StreamWriter writer = new StreamWriter(nameFile, false, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovok);

                    foreach (var item in dictionarySvodOTMNform)
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
        public static void InsertInDictionaryCollectionOTMNdata(Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL, Dictionary<string, DataFromOTMNform> dictionarySvodOTMNform)
        {
            //ищем в коллекции
            foreach (var itemDictionarySvodOTMNform in dictionarySvodOTMNform)
            {
                DataFromPersoDB tmpData = new DataFromPersoDB();
                if (dictionaryUnikRegNomPersoALL.TryGetValue(itemDictionarySvodOTMNform.Key, out tmpData))
                {
                    dictionaryUnikRegNomPersoALL[itemDictionarySvodOTMNform.Key].otmnFormAvailability = "ДА";
                    dictionaryUnikRegNomPersoALL[itemDictionarySvodOTMNform.Key].otmnFormKolZL = itemDictionarySvodOTMNform.Value.kolZL;
                    dictionaryUnikRegNomPersoALL[itemDictionarySvodOTMNform.Key].otmnFormKolZLgood = itemDictionarySvodOTMNform.Value.kolZLgood;
                    dictionaryUnikRegNomPersoALL[itemDictionarySvodOTMNform.Key].otmnFormKolZLbad = itemDictionarySvodOTMNform.Value.kolZLbad;
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
