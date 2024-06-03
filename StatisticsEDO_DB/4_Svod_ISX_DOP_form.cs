using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace StatisticsEDO_DB
{

    #region считанная из файла информация

    public class DataFrom_ISX_DOP_form
    {
        public string raion;
        public string regNum;
        public string kolZL;
        public string kolZLgood;
        public string kolZLbad;
        //public string kolZLBDPerso;

        public DataFrom_ISX_DOP_form(string raion = "", string regNum = "", string kolZL = "", string kolZLgood = "", string kolZLbad = "")
        {
            this.raion = raion;
            this.regNum = regNum;
            this.kolZL = kolZL;
            this.kolZLgood = kolZLgood;
            this.kolZLbad = kolZLbad;
            //this.kolZLBDPerso = kolZLBDPerso;

        }


        public override string ToString()
        {
            return raion + ";" + regNum + ";" + kolZL + ";" + kolZLgood + ";" + kolZLbad + ";" ;
        }
    }

    #endregion


    static class Svod_ISX_DOP_form
    {
        public static Dictionary<string, DataFrom_ISX_DOP_form> dictionarySvod_ISX_DOP_form = new Dictionary<string, DataFrom_ISX_DOP_form>();

        //------------------------------------------------------------------------------------------
        //Формируем сводную информацию на основании реестра всех ОТМН форм
        public static void CreateSvod(List<DataFromPersoDB> list_dataFromPersoDB)
        {
            try
            {
                foreach (var item_list_dataFromPersoDB in list_dataFromPersoDB)
                {
                    if (item_list_dataFromPersoDB.typeSved == "ИСХОДНАЯ" || item_list_dataFromPersoDB.typeSved == "ДОПОЛНЯЮЩАЯ")
                    {
                        //Добавляем выбранные данные в коллекцию
                        DataFrom_ISX_DOP_form tmpDataFrom_ISX_DOP_form = new DataFrom_ISX_DOP_form();
                        if (dictionarySvod_ISX_DOP_form.TryGetValue(item_list_dataFromPersoDB.regNum, out tmpDataFrom_ISX_DOP_form))
                        {
                            //есть рег.Номер в словаре
                            dictionarySvod_ISX_DOP_form[item_list_dataFromPersoDB.regNum].kolZL = (Convert.ToInt32(dictionarySvod_ISX_DOP_form[item_list_dataFromPersoDB.regNum].kolZL) + Convert.ToInt32(item_list_dataFromPersoDB.kolZL)).ToString();
                            dictionarySvod_ISX_DOP_form[item_list_dataFromPersoDB.regNum].kolZLgood = (Convert.ToInt32(dictionarySvod_ISX_DOP_form[item_list_dataFromPersoDB.regNum].kolZLgood) + Convert.ToInt32(item_list_dataFromPersoDB.kolZLgood)).ToString();
                            dictionarySvod_ISX_DOP_form[item_list_dataFromPersoDB.regNum].kolZLbad = (Convert.ToInt32(dictionarySvod_ISX_DOP_form[item_list_dataFromPersoDB.regNum].kolZLbad) + Convert.ToInt32(item_list_dataFromPersoDB.kolZLbad)).ToString();
                            //dictionarySvod_ISX_DOP_form[item_list_dataFromPersoDB.regNum].kolZLBDPerso = (Convert.ToInt32(dictionarySvod_ISX_DOP_form[item_list_dataFromPersoDB.regNum].kolZLBDPerso) + Convert.ToInt32(item_list_dataFromPersoDB.kolZLBDPerso)).ToString();
                        }
                        else
                        {
                            //нет рег.Номера в словаре
                            dictionarySvod_ISX_DOP_form.Add(item_list_dataFromPersoDB.regNum, new DataFrom_ISX_DOP_form(item_list_dataFromPersoDB.raion, item_list_dataFromPersoDB.regNum, item_list_dataFromPersoDB.kolZL, item_list_dataFromPersoDB.kolZLgood, item_list_dataFromPersoDB.kolZLbad));
                        }
                    }
                }

                string zagolovok = "raion" + ";" + "regNum" + ";" + "kolZL" + ";" + "kolZLgood" + ";" + "kolZLbad" + ";";
                string resultFile_Svod_ISX_DOP = IOoperations.katalogOut + @"\" + @"_Perso_Svod_ISX_DOP_" + DateTime.Now.ToShortDateString() + ".csv";

                //Проверяем на наличие файла отчета, если существует - удаляем
                if (File.Exists(resultFile_Svod_ISX_DOP))
                {
                    IOoperations.FileDelete(resultFile_Svod_ISX_DOP);
                }

                //TODO: Закомментировал создание файла статистики "Perso_Svod_ISX_DOP"
                //CreatResultFromDictionary(zagolovok, dictionarySvod_ISX_DOP_form, resultFile_Svod_ISX_DOP);

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
        private static void CreatResultFromDictionary(string zagolovok, Dictionary<string, DataFrom_ISX_DOP_form> dictionarySvod_ISX_DOP_form, string nameFile)
        {
            try
            {
                //Формируем результирующий файл статистики
                using (StreamWriter writer = new StreamWriter(nameFile, false, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovok);

                    foreach (var item in dictionarySvod_ISX_DOP_form)
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
        public static void InsertInDictionaryCollectionOTMNdata(Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL, Dictionary<string, DataFrom_ISX_DOP_form> dictionarySvod_ISX_DOP_form)
        {
            //ищем в коллекции
            foreach (var itemDictionarySvod_ISX_DOP_form in dictionarySvod_ISX_DOP_form)
            {
                DataFromPersoDB tmpData = new DataFromPersoDB();
                if (dictionaryUnikRegNomPersoALL.TryGetValue(itemDictionarySvod_ISX_DOP_form.Key, out tmpData))
                {
                    dictionaryUnikRegNomPersoALL[itemDictionarySvod_ISX_DOP_form.Key].kolZL = itemDictionarySvod_ISX_DOP_form.Value.kolZL;
                    dictionaryUnikRegNomPersoALL[itemDictionarySvod_ISX_DOP_form.Key].kolZLgood = itemDictionarySvod_ISX_DOP_form.Value.kolZLgood;
                    dictionaryUnikRegNomPersoALL[itemDictionarySvod_ISX_DOP_form.Key].kolZLbad = itemDictionarySvod_ISX_DOP_form.Value.kolZLbad;
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
