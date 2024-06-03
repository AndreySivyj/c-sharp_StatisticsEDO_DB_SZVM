using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace StatisticsEDO_DB
{
    static class SelectNullFormSZVM
    {
        public static Dictionary<string, DataFromPersoDB> dictionaryNullRegNomPerso = new Dictionary<string, DataFromPersoDB>();    //Коллекция "нулевиков" из реестра "_УникРегНомерPerso_Общий_реестр_"        

        //Выбираем "нулевиков" из реестра "_УникРегНомерPerso_Общий_реестр_"
        public static void SelectNullForm(Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL)
        {
            dictionaryNullRegNomPerso.Clear();

            foreach (var itemDictionaryUnikRegNomPersoALL in SelectDataForResultFile.dictionaryUnikRegNomPersoALL)
            {
                //if (itemDictionaryUnikRegNomPersoALL.Value.codZap== "844556")
                //{
                //    dictionaryNullRegNomPerso.Add(itemDictionaryUnikRegNomPersoALL.Value.regNum, itemDictionaryUnikRegNomPersoALL.Value);
                //}
                if (itemDictionaryUnikRegNomPersoALL.Value.kolZLBDPerso == "0" &&
                    itemDictionaryUnikRegNomPersoALL.Value.kolZL=="0"&&
                    itemDictionaryUnikRegNomPersoALL.Value.kolZLgood == "0"&&
                    itemDictionaryUnikRegNomPersoALL.Value.kolZLbad == "0"&&
                    itemDictionaryUnikRegNomPersoALL.Value.otmnFormKolZL == ""&&
                    itemDictionaryUnikRegNomPersoALL.Value.dopFormKolZL == ""&&
                    itemDictionaryUnikRegNomPersoALL.Value.toutNewCountZL == "0")
                {
                    dictionaryNullRegNomPerso.Add(itemDictionaryUnikRegNomPersoALL.Value.regNum, itemDictionaryUnikRegNomPersoALL.Value);
                }                
            }

            //Заголовок для файлов
            string zagolovok = "КодЗап" + ";" + "Район" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "ОтчМесяц" + ";" + "ОтчГод" + ";" + "Тип сведений" + ";" + "Дата представления" + ";"
                + "Результат проверки" + ";" + "Дата проверки" + ";" + "Количество ЗЛ за отч. период (принято, не проверено)" + ";" + "Статус квитанции" + ";"
                + "Специалист" + ";" + "Способ представления" + ";" + "Куратор" + ";" + "УП" + ";" + "Категория" + ";" + "ИНН" + ";"
                //+ "Дата постановки в ПФР" + ";" + "Дата снятия в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";"
                + "Дата постановки в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";"
                + "Наличие ДОП формы" + ";" + "Количество ЗЛ в ДОП формах" + ";" + "ЗЛ принято (ДОП формы)" + ";" + "ЗЛ не принято (ДОП формы)" + ";"
                + "Наличие ОТМН формы" + ";" + "Количество ЗЛ в ОТМН формах" + ";" + "ЗЛ принято (ОТМН формы)" + ";" + "ЗЛ не принято (ОТМН формы)" + ";"
                + "Tout_прошлый кол-во ЗЛ" + ";" + "Tout_текущий кол-во ЗЛ" + ";";
            //+ "Дата уведомления" + ";" + "Отработано? ДА|НЕТ" + ";" + "Дата отправки уведомления для НЕТ (лично|БПИ)" + ";" + "Дата отправки уведомления для НЕТ (лично|УП)" + ";" + "Примечание" + ";";


            //public static List<string> list_table_zagolovki = new List<string>();

            //имя файла
            string resultFile_dictionaryNullRegNomPerso = IOoperations.katalogOut + @"\" + @"4_Нулевые формы_реестр_" + DateTime.Now.ToShortDateString() + "_.csv";

            //Проверяем на наличие файла отчета, если существует - удаляем
            if (File.Exists(resultFile_dictionaryNullRegNomPerso))
            {
                IOoperations.FileDelete(resultFile_dictionaryNullRegNomPerso);
            }

            //создаем общий файл
            WriteLogs(resultFile_dictionaryNullRegNomPerso, zagolovok, dictionaryNullRegNomPerso);

        }

        //------------------------------------------------------------------------------------------       
        //Формируем результирующий файл статистики
        private static void WriteLogs(string resultFile, string zagolovok, Dictionary<string, DataFromPersoDB> dictionary_perso)
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
    }
}
