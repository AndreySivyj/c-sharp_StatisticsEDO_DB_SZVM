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
    static class SelectDataFromPersoDB
    {
        public static List<DataFromPersoDB> list_dataFromPersoDB = new List<DataFromPersoDB>();                //Коллекция всех данных из БД Perso

        async public static void SelectDataFromPerso()
        {
            //Очищаем коллекцию для данных из файлов
            list_dataFromPersoDB.Clear();

            // название процедуры
            string sqlExpression = "PERS.q0201_SP_mag_perform_SZVM";

            using (DB2Connection connection = new DB2Connection("Server=1.1.1.1:50000;Database=PERSDB;UID=regusr;PWD=password;"))
            {
                //открываем соединение
                await connection.OpenAsync();
                //Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("Соединение с БД: ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(connection.State);
                //Console.WriteLine();


                //connection.Open();

                //string query2 = @"set path=pers, system path;";
                //DB2Command command2 = connection.CreateCommand();
                //command2.CommandText = "set schema pers; set path=pers, system path; call PERS.SP_mag_perform_SZVM;";
                //var reader2 = command2.ExecuteNonQuery();


                DB2Command command3 = new DB2Command(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command3.CommandType = System.Data.CommandType.StoredProcedure;

                //Устанавливаем значение таймаута
                command3.CommandTimeout = 1570;

                /*

                call SP_mag_perform_SZVM(1, 32, 2019, 6, '01.01.2019','15.07.2019')

                create procedure SP_mag_perform_SZVM (
                    in p_region_st      smallint         -- код района начальный
                  , in p_region_fn      smallint         -- код района конечный
                  , in p_god            smallint         -- год    PERS.WORKS_M.god    = PERS.SZV_M.god
                  , in p_period         smallint         -- месяц  PERS.WORKS_M.period = PERS.SZV_M.period
                  , in p_date_priem_st	date    	 -- дата приема начальная PERS.SZV_M.d_priem
                  , in p_date_priem_fn  date    	 -- дата приема конечная  PERS.SZV_M.d_priem
                )

                */

                //добавляем параметры
                command3.Parameters.Add("p_region_st", 1);
                command3.Parameters.Add("p_region_fn", 34);

                //string p_god = "2016";
                //Console.WriteLine();
                //Console.ForegroundColor = ConsoleColor.DarkGreen;
                //Console.Write("Введите данные \"Отчетный период - год\": ");
                //Console.ForegroundColor = ConsoleColor.Gray;
                //string tmp_p_god = Console.ReadLine();
                //if (tmp_p_god != "")
                //{
                //    p_god = tmp_p_god;
                //    Program.otchYear = tmp_p_god;
                //}

                //string p_period = "1";
                //Console.WriteLine();
                //Console.ForegroundColor = ConsoleColor.DarkGreen;
                //Console.Write("Введите данные \"Отчетный период - месяц\": ");
                //Console.ForegroundColor = ConsoleColor.Gray;
                //string tmp_p_period = Console.ReadLine();
                //if (tmp_p_period != "")
                //{
                //    p_period = tmp_p_period;
                //    Program.otchMonth = tmp_p_period;
                //}

                //Console.WriteLine();
                //string p_date_priem_st = "01.01.2019";
                //Console.ForegroundColor = ConsoleColor.DarkGreen;
                //Console.Write("Введите данные \"Дата приема с\" (по умолчанию - 01.01.2019): ");
                //Console.ForegroundColor = ConsoleColor.Gray;
                //string tmpReadLine = Console.ReadLine();
                //if (tmpReadLine != "")
                //{
                //    p_date_priem_st = tmpReadLine;
                //}

                //Console.WriteLine();
                //string p_date_priem_fn = DateTime.Now.ToShortDateString();  //текущая системная дата
                //Console.ForegroundColor = ConsoleColor.DarkGreen;
                //Console.Write("Введите данные \"Дата приема по\" (по умолчанию - текущая системная дата): ");
                //Console.ForegroundColor = ConsoleColor.Gray;
                //tmpReadLine = Console.ReadLine();
                //Console.WriteLine();
                //if (tmpReadLine != "")
                //{
                //    p_date_priem_fn = tmpReadLine;
                //}

                command3.Parameters.Add("p_god", Program.otchYear);
                command3.Parameters.Add("p_period", Program.otchMonth);
                command3.Parameters.Add("p_date_priem_st", Program.p_date_priem_st);
                command3.Parameters.Add("p_date_priem_fn", Program.p_date_priem_fn);



                DbDataReader reader = await command3.ExecuteReaderAsync();

                //string curator = "";
                //string UP = "";
                //string specChanged = "";

                int i = 0;

                while (await reader.ReadAsync())
                {
                    //Console.WriteLine(reader[2].ToString());
                    //Console.WriteLine(SelectDataFromRKASVDB.GetRegionFromRegNum(reader[2].ToString()));
                    if (SelectDataFromRKASVDB.GetRegionFromRegNum(reader[2].ToString()) == "042")
                    {


                        string curator = "";
                        string UP = "";
                        string specChanged = "";

                        //TODO: !!!Сделать поиск более корректным (через словарь)
                        //Проставляем признак наличия УП
                        foreach (var item in SelectDataFromUPfile.list_UP)
                        {
                            if (reader[2].ToString() == item.regNumStrah)
                            {
                                UP = item.regNumUP;

                            }
                        }

                        //TODO: реализовать чтение из файла
                        if (reader[17].ToString() == "ПК БПИ")
                        {
                            specChanged = "ПК БПИ";
                        }
                        else if (reader[17].ToString() == "Карасева Е.А." || reader[17].ToString() == "Коваленко А.В." || reader[17].ToString() == "Kovaleva N.A." || reader[17].ToString() == "Сивый А.А." ||
                            reader[17].ToString() == "Верткова Т.Н." || reader[17].ToString() == "Петракова Е.Н." || reader[17].ToString() == "Бельская И.А." || reader[17].ToString() == "Andronova I.V." ||
                            reader[17].ToString() == "Нестерова Е. В." || reader[17].ToString() == "Кузьмина О. А." || reader[17].ToString() == "Фролова С.Ф." || reader[17].ToString() == "Конопелько М.А." ||
                            reader[17].ToString() == "Ананенко И. А." || reader[17].ToString() == "Антощенко Ю.А." || reader[17].ToString() == "Nikitina O.B.")
                        {
                            specChanged = "ПК БПИ_Центр";
                        }
                        else
                        {
                            specChanged = "Специалист";
                        }

                        //TODO: Закомментировал заполнение "частичными кодами куратора" (042-001 и 042-003) из-за их отсутствия
                        ////Выбираем код куратора
                        //if (reader[0].ToString() == "1" || reader[0].ToString() == "3")
                        //{
                        //    try
                        //    {
                        //        DataFromCuratorsFilePartial tmpData = new DataFromCuratorsFilePartial();
                        //        if (SelectDataFromCuratorsFilePartial.dictionary_CuratorsPartial.TryGetValue(reader[2].ToString(), out tmpData))
                        //        {
                        //            curator = tmpData.curator;
                        //        }
                        //    }
                        //    catch (KeyNotFoundException ex)
                        //    {
                        //        IOoperations.WriteLogError(ex.ToString());
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        IOoperations.WriteLogError(ex.ToString());
                        //    }
                        //    /*
                        //    foreach (var item in SelectDataFromCuratorsFilePartial.dictionary_CuratorsPartial)
                        //    {
                        //        if (massiveStr[2] == item.Key)
                        //        {
                        //            curator = item.Value.curator;

                        //        }
                        //    }
                        //    */
                        //}
                        //else
                        //{
                        try
                        {
                            DataFromCuratorsFile tmpData = new DataFromCuratorsFile();
                            if (SelectDataFromCuratorsFile.dictionary_Curators.TryGetValue(ConvertRaion(reader[0].ToString()), out tmpData))
                            {
                                curator = tmpData.curator;
                            }
                        }
                        catch (KeyNotFoundException ex)
                        {
                            IOoperations.WriteLogError(ex.ToString());
                        }
                        catch (Exception ex)
                        {
                            IOoperations.WriteLogError(ex.ToString());

                        }

                        /*
                        foreach (var item in SelectDataFromCuratorsFile.dictionary_Curators)
                        {
                            if (ConvertRaion(massiveStr[1]) == item.Key)
                            {
                                curator = item.Value.curator;

                            }
                        }
                        */
                        //}

                        list_dataFromPersoDB.Add(new DataFromPersoDB(reader[1].ToString(), reader[0].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(),
                                                                    ConvertDataFromDB(reader[8].ToString()), reader[12].ToString(), ConvertDataFromDB(reader[13].ToString()), reader[14].ToString(), reader[15].ToString(), reader[16].ToString(),
                                                                    reader[9].ToString(), reader[17].ToString(), specChanged, ConvertDataFromDB(reader[10].ToString()), curator, UP));

                        i++;
                    }
                }
                reader.Close();

                ////Заполняем коды кураторов свыше 042-0ХХ-099999
                //foreach (var itemList_dataFromPersoDB in list_dataFromPersoDB)
                //{
                //    if (itemList_dataFromPersoDB.curator == "" && itemList_dataFromPersoDB.raion == "1")
                //    {
                //        itemList_dataFromPersoDB.curator = "042PetrakovaEN";
                //    }
                //    if (itemList_dataFromPersoDB.curator == "" && itemList_dataFromPersoDB.raion == "3")
                //    {
                //        itemList_dataFromPersoDB.curator = "042NesterovaEV";
                //    }
                //}

                //Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Количество выбранных строк из ПК Perso: {0} ", i);
                //Console.ForegroundColor = ConsoleColor.Gray;
                //Console.WriteLine();



            }
        }

        //------------------------------------------------------------------------------------------
        private static string ConvertDataFromDB(string dataTime)
        {
            try
            {
                if (dataTime != "")
                {
                    DateTime date = Convert.ToDateTime(dataTime);
                    return date.ToShortDateString();
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
        private static string ConvertRaion(string raion)
        {
            try
            {
                if (raion.Count() == 1)
                {
                    return "042-00" + raion;
                }
                else
                {
                    return "042-0" + raion;
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
    }
}
