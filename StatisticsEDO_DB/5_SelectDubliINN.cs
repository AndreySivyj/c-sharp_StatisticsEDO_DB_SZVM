using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace StatisticsEDO_DB
{
    #region считанная из файла информация

    public class SvodINN
    {

        public string regNum;
        public int countINN;

        public SvodINN(string regNum = "", int countINN = 0)
        {
            this.regNum = regNum;
            this.countINN = countINN;
        }


        public override string ToString()
        {
            return regNum + ";" + countINN + ";";
        }
    }

    #endregion

    static class SelectDubliINN
    {
        public static Dictionary<string, SvodINN> dictionarySvodINN = new Dictionary<string, SvodINN>();    //Создаем сводную по ИНН

        public static Dictionary<string, DataFromPersoDB> dictionaryDubliINNPerso = new Dictionary<string, DataFromPersoDB>();    //Коллекция "дублей" по полю ИНН из реестра "_УникРегНомерPerso_Общий_реестр_"        

        //Выбираем "дубли" из реестра "_УникРегНомерPerso_Общий_реестр_"
        public static void SelectDubli(Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL)
        {
            
            dictionaryDubliINNPerso.Clear();

            //формируем свод по кол-ву рег.Номеров для каждого ИНН
            foreach (var itemDictionaryUnikRegNomPersoALL in dictionaryUnikRegNomPersoALL)
            {
                //Добавляем выбранные данные в коллекцию
                SvodINN tmpDataSvodINN = new SvodINN();
                if (dictionarySvodINN.TryGetValue(itemDictionaryUnikRegNomPersoALL.Value.inn + itemDictionaryUnikRegNomPersoALL.Value.kpp, out tmpDataSvodINN))
                {
                    //TODO: добавил проверку на наличие ИНН!!!
                    if (itemDictionaryUnikRegNomPersoALL.Value.inn != "")
                    {
                        //есть рег.Номер в словаре
                        ++dictionarySvodINN[itemDictionaryUnikRegNomPersoALL.Value.inn + itemDictionaryUnikRegNomPersoALL.Value.kpp].countINN;
                    }

                }
                else
                {
                    //TODO: добавил проверку на наличие ИНН!!!
                    if (itemDictionaryUnikRegNomPersoALL.Value.inn != "")
                    {
                        //нет рег.Номера в словаре
                        dictionarySvodINN.Add(itemDictionaryUnikRegNomPersoALL.Value.inn + itemDictionaryUnikRegNomPersoALL.Value.kpp, new SvodINN(itemDictionaryUnikRegNomPersoALL.Value.regNum, 1));
                    }
                }
            }



            //Формируем коллекцию "дублей по ИНН" из реестра "_УникРегНомерPerso_Общий_реестр_"
            foreach (var itemDictionarySvodINN in dictionarySvodINN)
            {
                if (itemDictionarySvodINN.Value.countINN > 1)
                {
                    foreach (var item in dictionaryUnikRegNomPersoALL)
                    {
                        if (item.Value.inn + item.Value.kpp == itemDictionarySvodINN.Key)
                        {
                            dictionaryDubliINNPerso.Add(item.Value.regNum, item.Value);
                        }
                    }

                    //DataFromPersoDB tmpDataUnikRegNom = new DataFromPersoDB();
                    //if (dictionaryUnikRegNomPersoALL.TryGetValue(itemDictionarySvodINN.Value.regNum, out tmpDataUnikRegNom))
                    //{
                    //    //есть рег.Номер в словаре
                    //    dictionaryDubliINNPerso.Add(itemDictionarySvodINN.Value.regNum, dictionaryUnikRegNomPersoALL[itemDictionarySvodINN.Value.regNum]);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Внимание! Ошибка наполнения файла \"Дубли_ИНН_реестр\" значениями");
                    //    continue;
                    //}   
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
                + "Tout_прошлый кол-во ЗЛ" + ";" + "Tout_текущий кол-во ЗЛ" + ";" + "Исправлено (да|нет|не требуется)" + ";" + "Примечание" + ";";
            //+ "Дата уведомления" + ";" + "Отработано? ДА|НЕТ" + ";" + "Дата отправки уведомления для НЕТ (лично|БПИ)" + ";" + "Дата отправки уведомления для НЕТ (лично|УП)" + ";" + "Примечание" + ";";


            //public static List<string> list_table_zagolovki = new List<string>();

            if (dictionaryDubliINNPerso.Count() != 0)
            {
                //имя файла
                string resultFile_dictionaryDubliINNPerso = IOoperations.katalogOut + @"\" + @"4_Дубли_ИНН_реестр_" + DateTime.Now.ToShortDateString() + "_.csv";

                //Проверяем на наличие файла отчета, если существует - удаляем
                if (File.Exists(resultFile_dictionaryDubliINNPerso))
                {
                    IOoperations.FileDelete(resultFile_dictionaryDubliINNPerso);
                }

                //создаем общий файл
                WriteLogs(resultFile_dictionaryDubliINNPerso, zagolovok, dictionaryDubliINNPerso);
            }


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
