using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace StatisticsEDO_DB
{
    static class CompareReestrPersoAndSpuspis
    {
        public static Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoCompare = new Dictionary<string, DataFromPersoDB>();             //Коллекция "Есть в Perso нет Spuspis"

        public static Dictionary<string, int> dictionarySpuspisCompare = new Dictionary<string, int>();                                              //Коллекция "Есть в Spuspis нет в Perso"

        public static void CompareReestr(Dictionary<string, int> dictionaryDataSpuspisSvod, Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL)
        {
            //очищаем коллекции
            dictionaryUnikRegNomPersoCompare.Clear();
            dictionarySpuspisCompare.Clear();



            //------------------------------------------------------------------------------------------
            //наполняем коллекцию "Есть в Perso нет в Spuspis"
            foreach (var itemUnikRegNomPersoALL in dictionaryUnikRegNomPersoALL)
            {                
                int tmpDataSpuspis;
                if (dictionaryDataSpuspisSvod.TryGetValue(itemUnikRegNomPersoALL.Value.regNum, out tmpDataSpuspis))
                {
                    continue;
                }
                else
                {
                    dictionaryUnikRegNomPersoCompare.Add(itemUnikRegNomPersoALL.Value.regNum, dictionaryUnikRegNomPersoALL[itemUnikRegNomPersoALL.Value.regNum]);
                }
            }

            if (dictionaryUnikRegNomPersoCompare.Count() != 0)
            {
                //Заголовок для файлов
                string zagolovok = "КодЗап" + ";" + "Район" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "ОтчМесяц" + ";" + "ОтчГод" + ";" + "Тип сведений" + ";" + "Дата представления" + ";"
                + "Результат проверки" + ";" + "Дата проверки" + ";" + "Количество ЗЛ за отч. период (принято, не проверено)" + ";" + "Статус квитанции" + ";"
                + "Специалист" + ";" + "Способ представления" + ";" + "Куратор" + ";" + "УП" + ";" + "Категория" + ";" + "ИНН" + ";"
                + "Дата постановки в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";"
                + "Наличие ДОП формы" + ";" + "Количество ЗЛ в ДОП формах" + ";" + "ЗЛ принято (ДОП формы)" + ";" + "ЗЛ не принято (ДОП формы)" + ";"
                + "Наличие ОТМН формы" + ";" + "Количество ЗЛ в ОТМН формах" + ";" + "ЗЛ принято (ОТМН формы)" + ";" + "ЗЛ не принято (ОТМН формы)" + ";"
                + "Tout_прошлый кол-во ЗЛ" + ";" + "Tout_текущий кол-во ЗЛ" + ";";

                //имя файла
                string resultFile_dictionaryUnikRegNomPersoCompare = IOoperations.katalogOut + @"\" + @"9_Есть_в_Perso_нет_в_Spuspis_" + DateTime.Now.ToShortDateString() + "_.csv";

                //Проверяем на наличие файла отчета, если существует - удаляем
                if (File.Exists(resultFile_dictionaryUnikRegNomPersoCompare))
                {
                    IOoperations.FileDelete(resultFile_dictionaryUnikRegNomPersoCompare);
                }

                //создаем общий файл
                SelectDataForResultFile.WriteLogs(resultFile_dictionaryUnikRegNomPersoCompare, zagolovok, dictionaryUnikRegNomPersoCompare);
                                
                Console.WriteLine("Количество записей в реестре \"Есть в Perso нет в Spuspis\" : {0}", dictionaryUnikRegNomPersoCompare.Count());                
            }



            //------------------------------------------------------------------------------------------
            //наполняем коллекцию "Есть в Spuspis нет в Perso"
            foreach (var itemSpuspis in dictionaryDataSpuspisSvod)
            {
                DataFromPersoDB tmpDataUnikRegNomPerso = new DataFromPersoDB();
                if (dictionaryUnikRegNomPersoALL.TryGetValue(itemSpuspis.Key, out tmpDataUnikRegNomPerso))
                {
                    continue;
                }
                else
                {
                    dictionarySpuspisCompare.Add(itemSpuspis.Key, dictionaryDataSpuspisSvod[itemSpuspis.Key]);
                }
            }

            if (dictionarySpuspisCompare.Count() != 0)
            {
                //имя файла
                string resultFile_dictionarySpuspisCompare = IOoperations.katalogOut + @"\" + @"9_Есть_в_Spuspis_нет_в_Perso_" + DateTime.Now.ToShortDateString() + "_.csv";

                //Проверяем на наличие файла отчета, если существует - удаляем
                if (File.Exists(resultFile_dictionarySpuspisCompare))
                {
                    IOoperations.FileDelete(resultFile_dictionarySpuspisCompare);
                }

                //создаем общий файл
                SelectDataFromSpuspisFile.CreateListDataSpuspisFileSvod(resultFile_dictionarySpuspisCompare, dictionarySpuspisCompare, Program.otchYear, Program.otchMonth);

                Console.WriteLine("Количество записей в реестре \"Есть в Spuspis нет в Perso\" : {0}", dictionarySpuspisCompare.Count());
            }

        }
    }
}
