using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Data.Common;
using IBM.Data.DB2;

namespace StatisticsEDO_DB
{
    public class DataFromPersoDB_zapros
    {
        public string regNum;
        public string strnum;
        public string otchYear;
        public string otchMonth;
        public DateTime dateINS;
        public DateTime timeINS;


        public DataFromPersoDB_zapros(string regNum = "", string strnum = "", string otchYear = "", string otchMonth = "",
                                        DateTime dateINS = default(DateTime), DateTime timeINS = default(DateTime))
        {
            this.regNum = regNum;
            this.strnum = strnum;
            this.otchYear = otchYear;
            this.otchMonth = otchMonth;
            this.dateINS = dateINS;
            this.timeINS = timeINS;
        }

        public override string ToString()
        {
            return regNum + ";" + strnum + ";" + otchYear + ";" + otchMonth + ";";
        }
    }



    static class SelectDataFromPersoDB_zapros_SZVM
    {
        private static Dictionary<string, DataFromPersoDB_zapros> dictionary_uniqSNILS_ISXD_SZV_M = new Dictionary<string, DataFromPersoDB_zapros>();       //Коллекция данных
        private static Dictionary<string, DataFromPersoDB_zapros> dictionary_uniqSNILS_OTMN_SZV_M = new Dictionary<string, DataFromPersoDB_zapros>();       //Коллекция данных        

        async public static void SelectDataFromPerso_ISXD(string query_ISXD)
        {
            try
            {
                using (DB2Connection connection = new DB2Connection("Server=1.1.1.1:50000;Database=PERSDB;UID=regusr;PWD=password;"))
                {

                    //открываем соединение
                    await connection.OpenAsync();

                    DB2Command command_ISXD = connection.CreateCommand();
                    command_ISXD.CommandText = query_ISXD;

                    //Устанавливаем значение таймаута
                    command_ISXD.CommandTimeout = 1570;

                    DbDataReader reader_ISXD = await command_ISXD.ExecuteReaderAsync();

                    //int i_ISXD = 0;

                    while (await reader_ISXD.ReadAsync())
                    {
                        //Console.WriteLine(reader_ISXD[0].ToString());
                        //Console.WriteLine(SelectDataFromRKASVDB.GetRegionFromRegNum(reader_ISXD[0].ToString()));
                        if (SelectDataFromRKASVDB.GetRegionFromRegNum(reader_ISXD[0].ToString()) == "042")
                        {

                            //              0           1       2       3           4           5
                            // @"select p.regnumb, w.strnum, w.god, w.period, s.DATE_INS, s.TIME_INS " +

                            //public DataFromPersoDB_zapros(string regNum = "", string strnum = "", string otchYear = "", string otchMonth = "",
                            //                DateTime dateINS = default(DateTime), DateTime timeINS = default(DateTime))



                            //регНом+СНИЛС есть в словаре
                            //KEY: regNum + INN + SNILS + KPP + otchMonth
                            DataFromPersoDB_zapros tmpData = new DataFromPersoDB_zapros();
                            if (dictionary_uniqSNILS_ISXD_SZV_M.TryGetValue(ConvertRegNom(reader_ISXD[0].ToString()) + reader_ISXD[1].ToString(), out tmpData))
                            {
                                //сверяем даты импорта в БД (больше)
                                if (Convert.ToDateTime(reader_ISXD[4].ToString()) > tmpData.dateINS)
                                {
                                    //KEY: REG + SNILS
                                    dictionary_uniqSNILS_ISXD_SZV_M[ConvertRegNom(reader_ISXD[0].ToString()) + reader_ISXD[1].ToString()] =
                                                                new DataFromPersoDB_zapros(
                                                                    ConvertRegNom(reader_ISXD[0].ToString()),
                                                                    reader_ISXD[1].ToString(),
                                                                    reader_ISXD[2].ToString(),
                                                                    reader_ISXD[3].ToString(),
                                                                    Convert.ToDateTime(reader_ISXD[4].ToString()),
                                                                    Convert.ToDateTime(reader_ISXD[5].ToString())
                                                                                    );
                                }
                                //сверяем даты импорта в БД (равны)
                                else if (Convert.ToDateTime(reader_ISXD[4].ToString()) == Convert.ToDateTime(tmpData.dateINS))
                                {
                                    //тогда сверяем время импорта в БД (больше)
                                    if (Convert.ToDateTime(reader_ISXD[5].ToString()) > tmpData.timeINS)
                                    {
                                        dictionary_uniqSNILS_ISXD_SZV_M[ConvertRegNom(reader_ISXD[0].ToString()) + reader_ISXD[1].ToString()] =
                                                                new DataFromPersoDB_zapros(
                                                                    ConvertRegNom(reader_ISXD[0].ToString()),
                                                                    reader_ISXD[1].ToString(),
                                                                    reader_ISXD[2].ToString(),
                                                                    reader_ISXD[3].ToString(),
                                                                    Convert.ToDateTime(reader_ISXD[4].ToString()),
                                                                    Convert.ToDateTime(reader_ISXD[5].ToString())
                                                                                    );
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                            }
                            else
                            {
                                dictionary_uniqSNILS_ISXD_SZV_M[ConvertRegNom(reader_ISXD[0].ToString()) + reader_ISXD[1].ToString()] =
                                                                new DataFromPersoDB_zapros(
                                                                    ConvertRegNom(reader_ISXD[0].ToString()),
                                                                    reader_ISXD[1].ToString(),
                                                                    reader_ISXD[2].ToString(),
                                                                    reader_ISXD[3].ToString(),
                                                                    Convert.ToDateTime(reader_ISXD[4].ToString()),
                                                                    Convert.ToDateTime(reader_ISXD[5].ToString())
                                                                                    );
                            }

                            //i_ISXD++;
                        }

                    }
                    reader_ISXD.Close();

                    Console.WriteLine("Количество выбранных строк из БД Perso (ИСХД формы СЗВ-М): {0} ", dictionary_uniqSNILS_ISXD_SZV_M.Count());
                    //Console.WriteLine();

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

        async public static void SelectDataFromPerso_OTMN(string query_OTMN)
        {
            try
            {
                using (DB2Connection connection = new DB2Connection("Server=1.1.1.1:50000;Database=PERSDB;UID=regusr;PWD=password;"))
                {
                    //открываем соединение
                    await connection.OpenAsync();

                    DB2Command command = connection.CreateCommand();
                    command.CommandText = query_OTMN;

                    //Устанавливаем значение таймаута
                    command.CommandTimeout = 1570;

                    DbDataReader reader = await command.ExecuteReaderAsync();

                    //int i_OTMN = 0;

                    while (await reader.ReadAsync())
                    {
                        if (SelectDataFromRKASVDB.GetRegionFromRegNum(reader[0].ToString()) == "042")
                        {


                            //регНом+СНИЛС есть в словаре
                            DataFromPersoDB_zapros tmpData = new DataFromPersoDB_zapros();
                            if (dictionary_uniqSNILS_OTMN_SZV_M.TryGetValue(ConvertRegNom(reader[0].ToString()) + reader[1].ToString(), out tmpData))
                            {
                                //сверяем даты импорта в БД (больше)
                                if (Convert.ToDateTime(reader[4].ToString()) > tmpData.dateINS)
                                {
                                    dictionary_uniqSNILS_OTMN_SZV_M[ConvertRegNom(reader[0].ToString()) + reader[1].ToString()] =
                                                                new DataFromPersoDB_zapros(
                                                                    ConvertRegNom(reader[0].ToString()),
                                                                    reader[1].ToString(),
                                                                    reader[2].ToString(),
                                                                    reader[3].ToString(),
                                                                    Convert.ToDateTime(reader[4].ToString()),
                                                                    Convert.ToDateTime(reader[5].ToString())
                                                                                    );
                                }
                                //сверяем даты импорта в БД (равны)
                                else if (Convert.ToDateTime(reader[4].ToString()) == tmpData.dateINS)
                                {
                                    //тогда сверяем время импорта в БД (больше)
                                    if (Convert.ToDateTime(reader[5].ToString()) > Convert.ToDateTime(tmpData.timeINS))
                                    {
                                        dictionary_uniqSNILS_OTMN_SZV_M[ConvertRegNom(reader[0].ToString()) + reader[1].ToString()] =
                                                                new DataFromPersoDB_zapros(
                                                                    ConvertRegNom(reader[0].ToString()),
                                                                    reader[1].ToString(),
                                                                    reader[2].ToString(),
                                                                    reader[3].ToString(),
                                                                    Convert.ToDateTime(reader[4].ToString()),
                                                                    Convert.ToDateTime(reader[5].ToString())
                                                                                    );
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                dictionary_uniqSNILS_OTMN_SZV_M[ConvertRegNom(reader[0].ToString()) + reader[1].ToString()] =
                                                                new DataFromPersoDB_zapros(
                                                                    ConvertRegNom(reader[0].ToString()),
                                                                    reader[1].ToString(),
                                                                    reader[2].ToString(),
                                                                    reader[3].ToString(),
                                                                    Convert.ToDateTime(reader[4].ToString()),
                                                                    Convert.ToDateTime(reader[5].ToString())
                                                                                    );
                            }

                            //i_OTMN++;
                        }
                    }
                    reader.Close();

                    Console.WriteLine("Количество выбранных строк из БД Perso (ОТМН формы СЗВ-М): {0} ", dictionary_uniqSNILS_OTMN_SZV_M.Count());
                    //Console.WriteLine();




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



        public static void Compare_SZV_M_ISX_and_OTMN()
        {
            try
            {
                //Формируем реестр уникальных СНИЛС СЗВ-М с учетом отмененных форм
                foreach (var item_uniqSNILS_ISXD_SZV_M in dictionary_uniqSNILS_ISXD_SZV_M)
                {
                    //регНом+СНИЛС есть в словаре
                    DataFromPersoDB_zapros tmpData = new DataFromPersoDB_zapros();
                    if (dictionary_uniqSNILS_OTMN_SZV_M.TryGetValue(item_uniqSNILS_ISXD_SZV_M.Key, out tmpData))
                    {
                        //сверяем даты импорта в БД (больше)
                        if (item_uniqSNILS_ISXD_SZV_M.Value.dateINS > tmpData.dateINS)
                        {
                            Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Key] = item_uniqSNILS_ISXD_SZV_M.Value;

                            ////есть в словаре
                            //DataFromPersoDB_zapros tmpData_no_OTMN = new DataFromPersoDB_zapros();
                            //if (Program.uniqSNILS_ISXD_SZV_M_no_OTMN.TryGetValue(item_uniqSNILS_ISXD_SZV_M.Key, out tmpData_no_OTMN))
                            //{
                            //    Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Key] = item_uniqSNILS_ISXD_SZV_M.Value;                                                            
                            //}
                            //else
                            //{
                            //    Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Key] = item_uniqSNILS_ISXD_SZV_M.Value;
                            //}
                        }
                        //сверяем даты импорта в БД (равны)
                        else if (item_uniqSNILS_ISXD_SZV_M.Value.dateINS == tmpData.dateINS)
                        {
                            //тогда сверяем время импорта в БД (больше)
                            if (item_uniqSNILS_ISXD_SZV_M.Value.timeINS > tmpData.timeINS)
                            {
                                Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Key] = item_uniqSNILS_ISXD_SZV_M.Value;

                                ////есть в словаре
                                //DataFromPersoDB_zapros tmpData_no_OTMN = new DataFromPersoDB_zapros();
                                //if (Program.uniqSNILS_ISXD_SZV_M_no_OTMN.TryGetValue(item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp, out tmpData_no_OTMN))
                                //{
                                //    Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp] =
                                //                                new DataFromPersoDB_zapros(
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.regNum,
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.strnum,
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.inn,
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.kpp,
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.otchYear,

                                //                                    AddOtchMonth(
                                //                                        Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp].otchMonth,
                                //                                        item_uniqSNILS_ISXD_SZV_M.Value.otchMonth
                                //                                        )
                                //                                                    );
                                //}
                                //else
                                //{
                                //    Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp] =
                                //                                new DataFromPersoDB_zapros(
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.regNum,
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.strnum,
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.inn,
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.kpp,
                                //                                    item_uniqSNILS_ISXD_SZV_M.Value.otchYear,

                                //                                    AddOtchMonth(
                                //                                        "",
                                //                                        item_uniqSNILS_ISXD_SZV_M.Value.otchMonth
                                //                                        )
                                //                                                    );
                                //}                               


                                ////Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp + item_uniqSNILS_ISXD_SZV_M.Value.otchMonth]
                                ////= item_uniqSNILS_ISXD_SZV_M.Value;

                                ////Формируем массив уникальных KEY (INN + SNILS + KPP + otchMonth) для сверки с СЗВ-СТАЖ
                                //Program.hashSet_SZVM_UniqSNILS.Add(item_uniqSNILS_ISXD_SZV_M.Value.inn + "," +
                                //                                item_uniqSNILS_ISXD_SZV_M.Value.strnum + "," +
                                //                                item_uniqSNILS_ISXD_SZV_M.Value.kpp + "," +
                                //                                item_uniqSNILS_ISXD_SZV_M.Value.otchMonth
                                //                              );
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Key] = item_uniqSNILS_ISXD_SZV_M.Value;

                        ////нет в словаре
                        //DataFromPersoDB_zapros tmpData_no_OTMN = new DataFromPersoDB_zapros();
                        //if (Program.uniqSNILS_ISXD_SZV_M_no_OTMN.TryGetValue(item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp, out tmpData_no_OTMN))
                        //{
                        //    Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp] =
                        //                                new DataFromPersoDB_zapros(
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.regNum,
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.strnum,
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.inn,
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.kpp,
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.otchYear,

                        //                                    AddOtchMonth(
                        //                                        Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp].otchMonth,
                        //                                        item_uniqSNILS_ISXD_SZV_M.Value.otchMonth
                        //                                        )
                        //                                                    );
                        //}
                        //else
                        //{
                        //    Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp] =
                        //                                new DataFromPersoDB_zapros(
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.regNum,
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.strnum,
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.inn,
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.kpp,
                        //                                    item_uniqSNILS_ISXD_SZV_M.Value.otchYear,

                        //                                    AddOtchMonth(
                        //                                        "",
                        //                                        item_uniqSNILS_ISXD_SZV_M.Value.otchMonth
                        //                                        )
                        //                                                    );
                        //}
                        ////Program.uniqSNILS_ISXD_SZV_M_no_OTMN[item_uniqSNILS_ISXD_SZV_M.Value.inn + item_uniqSNILS_ISXD_SZV_M.Value.strnum + item_uniqSNILS_ISXD_SZV_M.Value.kpp + item_uniqSNILS_ISXD_SZV_M.Value.otchMonth]
                        ////        = item_uniqSNILS_ISXD_SZV_M.Value;

                        ////Формируем массив уникальных KEY (INN + SNILS + KPP + otchMonth) для сверки с СЗВ-СТАЖ
                        //Program.hashSet_SZVM_UniqSNILS.Add(item_uniqSNILS_ISXD_SZV_M.Value.inn + "," +
                        //                                        item_uniqSNILS_ISXD_SZV_M.Value.strnum + "," +
                        //                                        item_uniqSNILS_ISXD_SZV_M.Value.kpp + "," +
                        //                                        item_uniqSNILS_ISXD_SZV_M.Value.otchMonth
                        //                                      );
                    }
                }


                //Console.Write("");










                int i = 0;
                foreach (var itemSZV_M_no_OTMN in Program.uniqSNILS_ISXD_SZV_M_no_OTMN)
                {
                    //------------------------------------------------------------------------------------------
                    //Наполняем коллекцию уникальных регномеров + СНИЛС + отчПериод из БД Perso
                    Program.persoDBunikRegNomAndSNILS.Add(itemSZV_M_no_OTMN.Value.regNum + ";" + ConvertSNILS(itemSZV_M_no_OTMN.Value.strnum) + ";");
                    //------------------------------------------------------------------------------------------



                    //Добавляем выбранные данные в коллекцию
                    int snils = 0;

                    if (Program.dictionaryDataCountZLPerso.TryGetValue(itemSZV_M_no_OTMN.Value.regNum, out snils))
                    {
                        //есть рег.Номер в словаре
                        ++Program.dictionaryDataCountZLPerso[itemSZV_M_no_OTMN.Value.regNum];// = Program.dictionaryDataCountZLPerso[ConvertRegNom(reader[0].ToString())] + 1;

                    }
                    else
                    {
                        //нет рег.Номера в словаре
                        Program.dictionaryDataCountZLPerso.Add(itemSZV_M_no_OTMN.Value.regNum, 1);
                    }

                    i++;
                }


                Console.WriteLine();
                Console.WriteLine("--- Кол-во записей в persoDBunikRegNomAndSNILS - {0} ---", Program.persoDBunikRegNomAndSNILS.Count());





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


        private static string ConvertSNILS(string snils)
        {
            string convertSNILS = "";
            for (int i = 0; i < snils.Count() - 2; i++)
            {
                convertSNILS = convertSNILS + snils[i];
            }

            return convertSNILS;
        }


        private static string AddOtchMonth(string otchMonth_old, string otchMonth_new)
        {
            //Создаем массив уникальных месяцев из dictionary_uniqSNILS_ISXD_SZV_STAG.Value.otchMonth
            SortedSet<int> tmpStr = new SortedSet<int>();

            char[] separator = { ',' };    //список разделителей в строке
            string[] massiveStr = otchMonth_old.Split(separator);     //создаем массив из строк между разделителями

            if (massiveStr.Count() != 0)
            {
                foreach (var item in massiveStr)
                {
                    if (item != "")
                    {
                        tmpStr.Add(Convert.ToInt32(item));
                    }
                }
            }



            //Добавляем новый период 
            tmpStr.Add(Convert.ToInt32(otchMonth_new));



            string monthCollection = "";
            int tmpStrCount = tmpStr.Count();

            if (tmpStrCount == 1)
            {
                foreach (var item in tmpStr)
                {
                    monthCollection = monthCollection + item + ",";
                }
            }
            else if (tmpStrCount > 1)
            {
                //Возвращаем строку из уникальных месяцев через запятую
                foreach (var item in tmpStr)
                {
                    --tmpStrCount;
                    if (tmpStrCount != 0)
                    {
                        monthCollection = monthCollection + item + ",";
                    }
                    else
                    {
                        monthCollection = monthCollection + item;
                    }
                }
            }
            else
            {
                monthCollection = "";
            }

            return monthCollection;

        }

        //------------------------------------------------------------------------------------------
        private static string SelectRaion(string regNum)
        {
            try
            {
                if (regNum.Count() == 11)
                {
                    return "042-0" + regNum[3] + regNum[4];
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;

                return "";
            }
        }

        //------------------------------------------------------------------------------------------
        private static string ConvertRegNom(string regNom)
        {
            try
            {
                char[] regNomOld = regNom.ToCharArray();
                string regNomConvert = "0" + regNomOld[0].ToString() + regNomOld[1].ToString() + "-" + regNomOld[2].ToString() + regNomOld[3] + regNomOld[4] + "-" + regNomOld[5] + regNomOld[6] + regNomOld[7] + regNomOld[8] + regNomOld[9] + regNomOld[10];


                return regNomConvert;
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;

                return "";
            }
        }



    }
}

