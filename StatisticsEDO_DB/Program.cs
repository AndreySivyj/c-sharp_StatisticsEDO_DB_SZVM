using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;
using System.Configuration;
using System.Collections.Specialized;

namespace StatisticsEDO_DB
{
    class Program
    {
        private static DateTime start;

        public static string otchMonth = "0";
        public static string otchYear = "0";
        public static string p_date_priem_st = "0";
        public static string p_date_priem_fn = "0";


        //Коллекция уник СНИЛС из БД Perso (реестр ИСХД форм СЗВ-М) без ОТМН форм   KEY (INN + SNILS + KPP)
        public static Dictionary<string, DataFromPersoDB_zapros> uniqSNILS_ISXD_SZV_M_no_OTMN = new Dictionary<string, DataFromPersoDB_zapros>();


        public static Dictionary<string, DataFromToutFileSvod> dictionaryDataToutSvodOld = new Dictionary<string, DataFromToutFileSvod>();  //Коллекция данных из файлов Tout_Old_Svod
        public static Dictionary<string, DataFromToutFileSvod> dictionaryDataToutSvodNew = new Dictionary<string, DataFromToutFileSvod>();  //Коллекция данных из файлов Tout_New_Svod
        public static SortedSet<string> toutUnikOtchMonth_old = new SortedSet<string>();        //Коллекция уникальных записей (с учетом отчПериода) из Tout
        public static SortedSet<string> toutUnikOtchMonth_new = new SortedSet<string>();        //Коллекция уникальных записей (с учетом отчПериода) из Tout

        public static List<DataFromRKASVDB> list_dataFromPKASVDB = new List<DataFromRKASVDB>();                //Коллекция данных из БД РК АСВ


        public static SortedSet<string> persoUnikRegNomForPKASV = new SortedSet<string>();              //Коллекция уникальных регномеров из реестра Perso
        public static SortedSet<string> persoUnikRegNom = new SortedSet<string>();                      //Коллекция уникальных регномеров из реестра Perso

        public static SortedSet<string> persoDBunikRegNomAndSNILS = new SortedSet<string>();        //Коллекция уникальных регномеров + СНИЛС (с учетом отчПериода) из БД Perso
        public static SortedSet<string> toutUnikRegNomAndSNILS = new SortedSet<string>();        //Коллекция уникальных регномеров + СНИЛС (с учетом отчПериода) из Tout


        public static Dictionary<string, int> dictionaryDataCountZLPerso = new Dictionary<string, int>();  //Коллекция верного количества ЗЛ из БД Perso (запрос к БД, с учетом отмененных снилс)

        public static NameValueCollection allAppSettings = ConfigurationManager.AppSettings;              //формируем массив настроек приложения

        static void Main(string[] args)
        {
            try
            {
                Console.SetWindowSize(110, 35);  //Устанавливаем размер окна консоли            

                //NameValueCollection allAppSettings = ConfigurationManager.AppSettings;              //формируем массив настроек приложения                        
                string destinationfolderName = allAppSettings["destinationfolderName"];             //каталог назначения  

                //время начала обработки
                start = DateTime.Now;

                //Создаем каталоги по умолчанию
                IOoperations.BasicDirectoryAndFileCreate();



                //------------------------------------------------------------------------------------------
                //0. Вводим параметры

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Введите необходимые параметры:");
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;

                Program.otchYear = allAppSettings["otchYear"];
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Введите данные \"Отчетный период - год\" (по умолчанию - {0}): ", Program.otchYear);
                Console.ForegroundColor = ConsoleColor.Gray;
                string tmp_p_god = Console.ReadLine();
                if (tmp_p_god != "")
                {
                    Program.otchYear = tmp_p_god;
                }

                Program.otchMonth = allAppSettings["otchMonth"];
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Введите данные \"Отчетный период - месяц\" (по умолчанию - {0}): ", Program.otchMonth);
                Console.ForegroundColor = ConsoleColor.Gray;
                string tmp_p_period = Console.ReadLine();
                if (tmp_p_period != "")
                {
                    Program.otchMonth = tmp_p_period;
                }

                Console.WriteLine();
                Program.p_date_priem_st = "01.01.2019";
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Введите данные \"Дата приема с\" (по умолчанию - 01.01.2019): ");
                Console.ForegroundColor = ConsoleColor.Gray;
                string tmpReadLine = Console.ReadLine();
                if (tmpReadLine != "")
                {
                    Program.p_date_priem_st = tmpReadLine;
                }

                Console.WriteLine();
                Program.p_date_priem_fn = DateTime.Now.ToShortDateString();  //текущая системная дата
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Введите данные \"Дата приема по\" (по умолчанию - текущая системная дата): ");
                Console.ForegroundColor = ConsoleColor.Gray;
                tmpReadLine = Console.ReadLine();
                Console.WriteLine();
                if (tmpReadLine != "")
                {
                    Program.p_date_priem_fn = tmpReadLine;
                }


                //------------------------------------------------------------------------------------------
                //1. Обработка файлов в каталоге \""_In_Status_ID"\"

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Обработка файлов в каталоге \"_In_Status_ID\", пожалуйста ждите...");
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;

                SelectDataFromStatusIDfile.ObrFileFromDirectory(IOoperations.katalogInStatusID);
                //Console.WriteLine();
                Console.WriteLine("Количество выбранных записей (каталог \"_In_Status_ID\"): {0}", SelectDataFromStatusIDfile.dictionaryStatusID.Count());

                //Console.WriteLine();


                //------------------------------------------------------------------------------------------
                //2. Обработка файлов в каталоге \"_In_UP\"

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Обработка файлов в каталоге \"_In_UP\", пожалуйста ждите...");
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;

                SelectDataFromUPfile.ObrFileFromDirectory(IOoperations.katalogInUP);
                //Console.WriteLine();
                //Console.WriteLine("Количество выбранных записей об уполномоченных представителях: {0}", SelectDataFromUPfile.list_UP.Count());

                //Console.WriteLine();



                //------------------------------------------------------------------------------------------
                //3.1. Выбираем данные по кураторам (каталог \"_In_Curators\")

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Обработка файлов в каталоге \"_In_Curators\", пожалуйста ждите...");
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;

                SelectDataFromCuratorsFile.ObrFileFromDirectory(IOoperations.katalogInCurators);
                //Console.WriteLine();
                //Console.WriteLine("Количество выбранных записей о кураторах (каталог \"_In_Curators\"): {0}", SelectDataFromCuratorsFile.dictionary_Curators.Count());

                //Console.WriteLine();


                //TODO: Закомментировал выбор "частичных кодов куратора" (042-001 и 042-003) из-за их отсутствия
                ////------------------------------------------------------------------------------------------
                ////3.2. Выбираем данные по кураторам (каталог \"_In_Curators_partial\")

                //Console.ForegroundColor = ConsoleColor.Cyan;
                //Console.WriteLine(new string('-', 107));
                //Console.WriteLine("Обработка файлов в каталоге \"_In_Curators_partial\", пожалуйста ждите...");
                //Console.WriteLine(new string('-', 107));
                //Console.ForegroundColor = ConsoleColor.Gray;

                //SelectDataFromCuratorsFilePartial.ObrFileFromDirectory(IOoperations.katalogInCuratorsPartial);
                ////Console.WriteLine();
                ////Console.WriteLine("Количество выбранных записей о кураторах (каталог \"_In_Curators_partial\"): {0}", SelectDataFromCuratorsFilePartial.dictionary_CuratorsPartial.Count());

                ////Console.WriteLine();            



                //------------------------------------------------------------------------------------------
                //4. Обработка файлов в каталогах \"_In_Tout_old\" и \"_In_Tout_new\"

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Началась обработка файлов в каталоге \"_In_Tout\", пожалуйста ждите...");
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;

                SelectDataFromToutFile.ObrFileFromDirectory(IOoperations.katalogInTout);

                Console.WriteLine();





                Console.WriteLine();
                Console.WriteLine("--- Кол-во записей в toutUnikRegNomAndSNILS - {0} ---", Program.toutUnikRegNomAndSNILS.Count());


                //------------------------------------------------------------------------------------------
                //5. Считываем данные предыдущей отработки реестра ошибок Perso (файлы \"Perso_Отработка\")
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Считываем данные предыдущей отработки реестра ошибок Perso (каталог \"Perso_Отработка\"), пожалуйста ждите...");
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;

                SelectDataFromPersoFile.ObrFileFromDirectory(IOoperations.katalogInPersoOtrabotkaOld);



                //------------------------------------------------------------------------------------------
                //6. Выбор данных из БД Perso

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Выбор данных из БД Perso, пожалуйста ждите...");
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;

                //Выбираем данные из БД Perso
                SelectDataFromPersoDB.SelectDataFromPerso();





                //------------------------------------------------------------------------------------------
                //6.1. Выбор данных из БД Perso (СЗВ-М)
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Выбор данных из БД Perso (СЗВ-М), пожалуйста ждите... ({0})", DateTime.Now.ToLongTimeString());
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;


                //6.1.1. Выбираем данные из БД Perso ISX                           
                SelectDataFromPersoDB_zapros_SZVM.SelectDataFromPerso_ISXD(DatabaseQueries.CreateQueryPersoSZVM_ISX());

                //6.1.2. Выбираем данные из БД Perso OTMN и сравниваем ИСХД и ОТМН формы, оставляя уник. СНИЛС
                SelectDataFromPersoDB_zapros_SZVM.SelectDataFromPerso_OTMN(DatabaseQueries.CreateQueryPersoSZVM_OTMN());

                //6.1.3 Сверяем полученные реестры и убираем отмененные формы 
                //и формируем массив Program.hashSet_SZVM_UniqSNILS - уникальных KEY (INN + SNILS + KPP + otchMonth) для сверки с СЗВ-СТАЖ
                SelectDataFromPersoDB_zapros_SZVM.Compare_SZV_M_ISX_and_OTMN();




                //TODO: закомментировал обработку каталога \"_In_Spuspis\"
                /*
                //------------------------------------------------------------------------------------------
                //4.2 Обработка файлов в каталоге \"_In_Spuspis\"

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Началась обработка файлов в каталоге \"_In_Spuspis\", пожалуйста ждите...");
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;

                SelectDataFromSpuspisFile.ObrFileFromDirectory(IOoperations.katalogInSpuspis, Program.otchYear, Program.otchMonth);
                */


                //------------------------------------------------------------------------------------------
                //7. Выбираем данные из РК АСВ            

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string('-', 107));
                Console.WriteLine("Выбираем данные из РК АСВ, пожалуйста ждите...");
                Console.WriteLine(new string('-', 107));
                Console.ForegroundColor = ConsoleColor.Gray;

                //Наполняем коллекцию уникальными значениями регНомеров из БД ПК Perso
                foreach (var itemDataFromPersoDB in SelectDataFromPersoDB.list_dataFromPersoDB)
                {
                    //Коллекция уникальных регномеров из реестра Perso
                    persoUnikRegNomForPKASV.Add(CreateDataFromPersoSelect.ConvertRegNom(itemDataFromPersoDB.regNum));


                    if (SelectDataFromRKASVDB.GetRegionFromRegNum(itemDataFromPersoDB.regNum) == "042")
                    {
                        //Коллекция уникальных регномеров из реестра Perso для "CreateUniqPersoFile" (создание реестра уникальных регНомеров из БД Perso)
                        persoUnikRegNom.Add(itemDataFromPersoDB.regNum);
                    }
                }

                //две части текста запроса к РК АСВ             
                string part1 = @"select a.insurer_reg_num, a.insurer_reg_start_date, a.insurer_reg_finish_date, a.insurer_short_name, a.insurer_last_name, a.insurer_first_name, a.insurer_middle_name, " +
                    //@", coalesce(a.insurer_short_name,coalesce(a.insurer_last_name,\'' )||' '||coalesce(a.insurer_first_name,'')||' '||coalesce(a.insurer_middle_name,'')) " //"Наименование"                           
                    //@"a.INSURER_REG_DATE_RO, a.INSURER_UNREG_DATE_RO, b.category_code, a.insurer_inn from(select* FROM asv_insurer) a left join(select category_id, category_code from asv_category) b on a.category_id = b.category_id where a.insurer_reg_num in (";
                    @"a.INSURER_REG_DATE_RO, a.INSURER_UNREG_DATE_RO, b.category_code, a.insurer_inn, d.ro_code, e.reg_start_code, r.reg_finish_code, a.INSURER_STATUS_ID, a.insurer_kpp from(select* FROM asv_insurer) a left join(select category_id, category_code from asv_category) b on a.category_id = b.category_id left join (select ro_id, ro_code from asv_ro) d on a.ro_id = d.ro_id left join (select reg_start_id, reg_start_code from asv_reg_start) e on a.reg_start_id = e.reg_start_id left join (select reg_finish_id, reg_finish_code from asv_reg_finish) r on a.reg_finish_id = r.reg_finish_id where a.insurer_reg_num in (";

                string part2 = @") order by a.insurer_reg_num";

                //                //string resultFile_persoUnikRegNom = IOoperations.katalogOut + @"\" + @"_Perso_ZaprosRKASV" + ".txt";
                //                //IOoperations.WriteZaprosRKASV(resultFile_persoUnikRegNom, part1, part2, SelectDataFromPersoDB.persoUnikRegNom);

                //Текст запроса
                string query1 = part1;

                int tmpCount = persoUnikRegNomForPKASV.Count;

                foreach (var item in persoUnikRegNomForPKASV)
                {
                    --tmpCount;
                    if (tmpCount != 0)
                    {
                        query1 = query1 + item + ", ";
                    }
                    else
                    {
                        query1 = query1 + item;
                    }
                }

                query1 = query1 + part2;

                //string script = File.ReadAllText(@"5.txt");   //Запрос можно считать из файла

                //Создаем имя результирующего файла
                string nameResultFile = IOoperations.katalogOut + @"\" + @"1_Данные_из_РК_АСВ_" + DateTime.Now.ToShortDateString() + ".csv";

                //Проверяем наличие регНомеров для запроса
                if (persoUnikRegNomForPKASV.Count != 0)
                {
                    //Выбираем данные из РК АСВ
                    SelectDataFromRKASVDB.SelectDataFromRKASV(query1, list_dataFromPKASVDB, nameResultFile);






                    /*
                    //Замена на
                    //6.1. Выбор данных из БД Perso (СЗВ-М)



                    //------------------------------------------------------------------------------------------
                    //--- Выбираем верное количество ЗЛ из БД Perso (запрос к БД, с учетом отмененных снилс)
                    foreach (var itemRegNum in persoUnikRegNomForPKASV)
                    {
                        //SelectDataFromPersoDB_CountZL.SelectDataFromPerso_CountZL(itemRegNum, Program.otchYear, Program.otchMonth);
                        SelectDataFromPersoDB_CountZL_v2.SelectDataFromPerso_CountZL(itemRegNum, Program.otchYear, Program.otchMonth);
                    }

                    Console.WriteLine();
                    Console.WriteLine("--- Кол-во записей в persoDBunikRegNomAndSNILS - {0} ---", Program.persoDBunikRegNomAndSNILS.Count());

                    */







                    //------------------------------------------------------------------------------------------
                    //--- Выбираем уникальные значения СНИЛС в toutUnikRegNomAndSNILS
                    var list_ExceptToutFiles = Program.toutUnikRegNomAndSNILS.Except(Program.persoDBunikRegNomAndSNILS);     //Коллекция уникальных значений СНИЛС в toutUnikRegNomAndSNILS

                    //Создаем заголовок в результирующем файле
                    string zagolovok_ExceptToutFiles = "регНом;СНИЛС;";

                    //Создаем имя результирующего файла
                    string nameFile_ExceptToutFiles = IOoperations.katalogOut + @"\" + @"5_СНИЛС_есть_в_Tout_нет_в_Perso" + ".csv";



                    //IOoperations.CreateExportFile_except(zagolovok_ExceptToutFiles, "_toutUnikRegNomAndSNILS.csv", toutUnikRegNomAndSNILS);
                    //Console.WriteLine("list_ExceptToutFiles.Count() - {0}", list_ExceptToutFiles.Count());



                    if (list_ExceptToutFiles.Count() != 0)
                    {
                        //Формируем результирующий файл
                        IOoperations.CreateExportFile(zagolovok_ExceptToutFiles, list_ExceptToutFiles, nameFile_ExceptToutFiles);

                    }

                    //------------------------------------------------------------------------------------------
                    //--- Выбираем уникальные значения СНИЛС в persoDBunikRegNomAndSNILS
                    var list_ExceptPersoDB = Program.persoDBunikRegNomAndSNILS.Except(Program.toutUnikRegNomAndSNILS);     //Коллекция уникальных значений СНИЛС в persoDBunikRegNomAndSNILS

                    //Создаем заголовок в результирующем файле
                    string zagolovok_ExceptPersoDB = "регНом;СНИЛС;";

                    //Создаем имя результирующего файла
                    string nameFile_ExceptPersoDB = IOoperations.katalogOut + @"\" + @"5_СНИЛС_есть_в_Perso_нет_в_Tout" + ".csv";

                    if (list_ExceptPersoDB.Count() != 0)
                    {
                        //Формируем результирующий файл
                        IOoperations.CreateExportFile(zagolovok_ExceptPersoDB, list_ExceptPersoDB, nameFile_ExceptPersoDB);
                    }

                    //Создаем заголовок в результирующем файле
                    string nameFile_persoDBunikRegNomAndSNILS = IOoperations.katalogOut + @"\" + "2_" + Program.otchYear + "_" + Program.otchMonth + "_" + @"Уникальные_СНИЛС_в_Perso_" + DateTime.Now.ToShortDateString() + "_.csv";
                    IOoperations.CreateExportFile_except(zagolovok_ExceptPersoDB, nameFile_persoDBunikRegNomAndSNILS, persoDBunikRegNomAndSNILS);
                    //Console.WriteLine("list_ExceptPersoDB.Count() - {0}", list_ExceptPersoDB.Count());


                    //HashSet<string> hashset = new HashSet<string>(firstFile);

                    //HashSet<string> hashset2 = new HashSet<string>(secondFile);

                    //hashset.ExceptWith(hashset2);





                    //------------------------------------------------------------------------------------------
                    //8. Наполняем данными из РК АСВ коллекцию из БД Perso
                    //CreateDataFromPersoSelect.CreatePersoFile(SelectDataFromPersoDB.list_dataFromPersoDB, list_dataFromPKASVDB, dictionaryDataToutSvodOld, dictionaryDataToutSvodNew);
                    CreateDataFromPersoSelect.CreatePersoFile(SelectDataFromPersoDB.list_dataFromPersoDB, list_dataFromPKASVDB);


                    //------------------------------------------------------------------------------------------
                    //9. Формируем сводную информацию на основании реестра всех ОТМН форм
                    if (CreateDataFromPersoSelect.list_OTMN.Count() != 0)
                    {
                        SvodOTMNform.CreateSvod(CreateDataFromPersoSelect.list_OTMN);
                    }

                    //------------------------------------------------------------------------------------------
                    //10. Формируем сводную информацию на основании реестра всех ИСХ + ДОП форм
                    if (SelectDataFromPersoDB.list_dataFromPersoDB.Count() != 0)
                    {
                        Svod_ISX_DOP_form.CreateSvod(SelectDataFromPersoDB.list_dataFromPersoDB);
                    }

                    //------------------------------------------------------------------------------------------
                    //11. Формируем сводную информацию на основании реестра всех ОТМН форм
                    if (CreateDataFromPersoSelect.list_DOP.Count() != 0)
                    {
                        SvodDOPform.CreateSvod(CreateDataFromPersoSelect.list_DOP);
                    }

                    //------------------------------------------------------------------------------------------
                    //12. На основании коллекции "уникальные регномера из реестра Perso" выбираем поэтапно из коллекций "данные из реестра Perso" статусы обработки 
                    // и создаем файл   @"УникРегНомерPerso_Общий_реестр_" + DateTime.Now.ToShortDateString() + "_.csv"

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(new string('-', 107));
                    Console.WriteLine("Формируем сводный реестр уникальных регномеров из БД Perso (со статусом приема), пожалуйста ждите...");
                    Console.WriteLine(new string('-', 107));
                    Console.ForegroundColor = ConsoleColor.Gray;

                    SelectDataForResultFile.SelectResultData(persoUnikRegNom, dictionaryDataToutSvodOld, dictionaryDataToutSvodNew);



                    //------------------------------------------------------------------------------------------
                    //Закомментировано создание сводной (неактуальный вариант)
                    //13.1 Формируем сводную информацию на основании коллекции "УникРегНомерPerso_Общий_реестр" и создаем файл @"_УникРегНомерPerso_Свод"
                    //CreateSvod_Itog.CreateSvod(SelectDataForResultFile.dictionaryUnikRegNomPersoALL);

                    //------------------------------------------------------------------------------------------
                    //13.2 Формируем сводную информацию на основании коллекции "УникРегНомерPerso_Общий_реестр" и создаем файл @"_УникРегНомерPerso_Свод_New"
                    CreateSvod_Itog_New.CreateSvod(SelectDataForResultFile.dictionaryUnikRegNomPersoALL);

                    //------------------------------------------------------------------------------------------
                    //14. Формируем реестр "нулевиков" из реестра "_УникРегНомерPerso_Общий_реестр_"
                    SelectNullFormSZVM.SelectNullForm(SelectDataForResultFile.dictionaryUnikRegNomPersoALL);

                    //15. Формируем реестр "дублей по ИНН" из реестра "_УникРегНомерPerso_Общий_реестр_"
                    SelectDubliINN.SelectDubli(SelectDataForResultFile.dictionaryUnikRegNomPersoALL);

                    //------------------------------------------------------------------------------------------
                    //16. Считываем данные в каталоге \"_In_PlanPriema\" для сверки с плановым показателемпо приему СЗВ-М
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(new string('-', 107));
                    Console.WriteLine("Считываем данные из плана по приему СЗВ-М (каталог \"_In_PlanPriema\"), пожалуйста ждите...");
                    Console.WriteLine(new string('-', 107));
                    Console.ForegroundColor = ConsoleColor.Gray;

                    SelectDataFromPlanPriemaFile.ObrFileFromDirectory(IOoperations.katalogInPlanPriema);

                    //------------------------------------------------------------------------------------------
                    //17. Сравниваем реестры "dictionaryPlanPriema" и "dictionaryUnikRegNomPersoALL"
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(new string('-', 107));
                    Console.WriteLine("Сравниваем данные из плана по приему СЗВ-М и реестр Perso, пожалуйста ждите...");
                    Console.WriteLine(new string('-', 107));
                    Console.ForegroundColor = ConsoleColor.Gray;

                    CompareReestrPersoAndPlanPriema.CompareReestr(SelectDataFromPlanPriemaFile.dictionaryPlanPriema, SelectDataForResultFile.dictionaryUnikRegNomPersoALL);

                    //TODO: закомментировал сравнение с Spuspis
                    /*
                    //------------------------------------------------------------------------------------------
                    //18. Сравниваем реестры "dictionarySpuspis" и "dictionaryUnikRegNomPersoALL"
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(new string('-', 107));
                    Console.WriteLine("Сравниваем данные из файла \"42_spuspis2a.csv\" и реестр Perso, пожалуйста ждите...");
                    Console.WriteLine(new string('-', 107));
                    Console.ForegroundColor = ConsoleColor.Gray;

                    CompareReestrPersoAndSpuspis.CompareReestr(SelectDataFromSpuspisFile.dictionaryDataSpuspisSvod, SelectDataForResultFile.dictionaryUnikRegNomPersoALL);
                    */



                }
                else
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Внимание! Нет рег. номеров для выполнения запроса к БД РК АСВ.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }











                ////------------------------------------------------------------------------------------------
                ////4. Обработка файлов в каталоге \"_In_Perso\"

                //Console.ForegroundColor = ConsoleColor.Cyan;
                //Console.WriteLine(new string('-', 91));
                //Console.WriteLine("Обработка файлов в каталоге \"_In_Perso\", пожалуйста ждите...");
                //Console.WriteLine(new string('-', 91));
                //Console.ForegroundColor = ConsoleColor.Gray;

                //SelectDataFromPersoFile.ObrFileFromDirectory(IOoperations.katalogInPerso);

                //Console.WriteLine();



                ////------------------------------------------------------------------------------------------
                ////6. На основании коллекции "уникальные регномера из реестра Perso" выбираем поэтапно из коллекций "данные из реестра Perso" (ИСХ, ДОП) статусы обработки

                //Console.ForegroundColor = ConsoleColor.Cyan;
                //Console.WriteLine(new string('-', 91));
                //Console.WriteLine("Формируем общий файл, пожалуйста ждите...");
                //Console.WriteLine(new string('-', 91));
                //Console.ForegroundColor = ConsoleColor.Gray;

                //SelectDataForResultFile.SelectResultData(SelectDataFromPersoFile.persoUnikRegNom);

                //Console.WriteLine();




                //вычисляем время затраченное на обработку
                TimeSpan stop = DateTime.Now - start;

                //Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(new string('-', 91));
                Console.WriteLine("Обработка выполнилась за " + stop.Minutes + " мин. " + stop.Seconds + " сек.");
                Console.ForegroundColor = ConsoleColor.Gray;

                //Console.ReadKey();

                //Задержка экрана
                Thread.Sleep(TimeSpan.FromSeconds(5));
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
