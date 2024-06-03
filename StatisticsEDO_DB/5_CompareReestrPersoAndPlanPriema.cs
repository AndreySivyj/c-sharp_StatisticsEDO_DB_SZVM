using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace StatisticsEDO_DB
{
    static class CompareReestrPersoAndPlanPriema
    {
        public static Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoCompare = new Dictionary<string, DataFromPersoDB>();             //Коллекция "Есть в Perso нет в Плане"

        public static Dictionary<string, DataFromPlanPriemaFile> dictionaryPlanPriemaCompare = new Dictionary<string, DataFromPlanPriemaFile>();    //Коллекция "Есть в Плане нет в Perso"

        public static void CompareReestr(Dictionary<string, DataFromPlanPriemaFile> dictionaryPlanPriema, Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL)
        {
            //очищаем коллекции
            dictionaryUnikRegNomPersoCompare.Clear();
            dictionaryPlanPriemaCompare.Clear();



            //------------------------------------------------------------------------------------------
            //наполняем коллекцию "Есть в Perso нет в Плане"
            foreach (var itemUnikRegNomPersoALL in dictionaryUnikRegNomPersoALL)
            {                
                DataFromPlanPriemaFile tmpDataPlanPriema = new DataFromPlanPriemaFile();
                if (dictionaryPlanPriema.TryGetValue(itemUnikRegNomPersoALL.Value.regNum, out tmpDataPlanPriema))
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
                string resultFile_dictionaryUnikRegNomPersoCompare = IOoperations.katalogOut + @"\" + @"8_Есть_в_Perso_нет_в_Плане_" + DateTime.Now.ToShortDateString() + "_.csv";

                //Проверяем на наличие файла отчета, если существует - удаляем
                if (File.Exists(resultFile_dictionaryUnikRegNomPersoCompare))
                {
                    IOoperations.FileDelete(resultFile_dictionaryUnikRegNomPersoCompare);
                }

                //создаем общий файл
                SelectDataForResultFile.WriteLogs(resultFile_dictionaryUnikRegNomPersoCompare, zagolovok, dictionaryUnikRegNomPersoCompare);
                                
                Console.WriteLine("Количество записей в реестре \"Есть в Perso нет в Плане\" : {0}", dictionaryUnikRegNomPersoCompare.Count());                
            }
                        


            //------------------------------------------------------------------------------------------
            //наполняем коллекцию "Есть в Плане нет в Perso"
            foreach (var itemPlanPriema in dictionaryPlanPriema)
            {
                DataFromPersoDB tmpDataUnikRegNomPerso = new DataFromPersoDB();
                if (dictionaryUnikRegNomPersoALL.TryGetValue(itemPlanPriema.Value.regNum, out tmpDataUnikRegNomPerso))
                {
                    continue;
                }
                else
                {
                    dictionaryPlanPriemaCompare.Add(itemPlanPriema.Value.regNum, dictionaryPlanPriema[itemPlanPriema.Value.regNum]);
                }
            }

            if (dictionaryPlanPriemaCompare.Count() != 0)
            {
                //№ п/п	Код района	Рег. номер	Наименование	Дата постановки на учет в РО	Дата снятия с учета в РО	Категория	ИНН	"Количество ЗЛ, учтенных в подсистеме АИС ПФР 2 за предшествующий отчетный период по форме СЗВ-М"	"Предшественник ("старый" регистрационный номер)"
                //Заголовок для файлов
                string zagolovok1 = "№ п/п" + ";" + "Код района" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "Дата постановки на учет в РО" + ";" + "Дата снятия с учета в РО" + ";"
                    + "Категория" + ";" + "ИНН" + ";" + "Количество ЗЛ, учтенных в подсистеме АИС ПФР 2 за предшествующий отчетный период по форме СЗВ-М" + ";" + "Предшественник (\"старый\" регистрационный номер)" + ";";

                //имя файла
                string resultFile_dictionaryPlanPriemaCompare = IOoperations.katalogOut + @"\" + @"8_Есть_в_Плане_нет_в_Perso_" + DateTime.Now.ToShortDateString() + "_.csv";

                //Проверяем на наличие файла отчета, если существует - удаляем
                if (File.Exists(resultFile_dictionaryPlanPriemaCompare))
                {
                    IOoperations.FileDelete(resultFile_dictionaryPlanPriemaCompare);
                }

                //создаем общий файл
                SelectDataFromPlanPriemaFile.WriteLogs(resultFile_dictionaryPlanPriemaCompare, zagolovok1, dictionaryPlanPriemaCompare);

                Console.WriteLine("Количество записей в реестре \"Есть в Плане нет в Perso\" : {0}", dictionaryPlanPriemaCompare.Count());
            }

        }
    }
}
