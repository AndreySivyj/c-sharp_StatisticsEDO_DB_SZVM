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
    static class SelectDataFromRKASVDB
    {
        //------------------------------------------------------------------------------------------        
        //Выбираем данные из РК АСВ
        async public static void SelectDataFromRKASV(string query, List<DataFromRKASVDB> listData, string nameResultFile)
        {
            //Очищаем коллекцию для данных из файлов
            listData.Clear();

            //Подключаемся к БД и выполняем запрос
            using (DB2Connection connection = new DB2Connection("Server=1.1.1.1:50000;Database=asv;UID=db2inst;PWD=password;"))
            {
                try
                {
                    //открываем соединение
                    await connection.OpenAsync();
                    //Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("Соединение с БД: ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(connection.State);
                    //Console.WriteLine();

                    DB2Command command = connection.CreateCommand();
                    command.CommandText = query;
                    DbDataReader reader = await command.ExecuteReaderAsync();

                    int i = 0;
                    //                          0                       1                       2                           3                   4                       5                   6           
                    //string part1 = @"select a.insurer_reg_num, a.insurer_reg_start_date, a.insurer_reg_finish_date, a.insurer_short_name, a.insurer_last_name, a.insurer_first_name, a.insurer_middle_name, " +
                    //          7                   8                       9                   10          11          12              13                  14
                    //@"a.INSURER_REG_DATE_RO, a.INSURER_UNREG_DATE_RO, b.category_code, a.insurer_inn, d.ro_code, e.reg_start_code, r.reg_finish_code, a.INSURER_STATUS_ID from(select* FROM asv_insurer) a left join(select category_id, category_code from asv_category) b on a.category_id = b.category_id left join (select ro_id, ro_code from asv_ro) d on a.ro_id = d.ro_id left join (select reg_start_id, reg_start_code from asv_reg_start) e on a.reg_start_id = e.reg_start_id left join (select reg_finish_id, reg_finish_code from asv_reg_finish) r on a.reg_finish_id = r.reg_finish_id where a.insurer_reg_num in (";



                    while (await reader.ReadAsync())
                    {
                        listData.Add(new DataFromRKASVDB(ConvertRegNom(reader[0].ToString()), ConvertDataFromDB(reader[1].ToString()), ConvertDataFromDB(reader[2].ToString()),
                                    reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(),
                            ConvertDataFromDB(reader[7].ToString()), ConvertDataFromDB(reader[8].ToString()), reader[9].ToString(), reader[10].ToString(), reader[15].ToString(),
                            ConvertRaion(reader[11].ToString()), reader[12].ToString(), reader[13].ToString(), ConvertStatusID(reader[14].ToString())));

                        i++;
                    }
                    reader.Close();

                    //Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("Количество выбранных строк из РК АСВ: {0} ", i);
                    //Console.ForegroundColor = ConsoleColor.Gray;
                    //Console.WriteLine();

                    //Добавляем значение куратора в выборку
                    foreach (var item in listData)
                    {
                        item.kurator = SelectKurator(item.raion, item.insurer_reg_num);
                    }

                    //Формируем заголовок                   
                    string zagolovok = "raion" + ";" + "insurer_reg_num" + ";" + "name" + ";" + "insurer_reg_start_date" + ";" + "insurer_reg_finish_date" + ";" + "INSURER_REG_DATE_RO" + ";" + "INSURER_UNREG_DATE_RO" + ";" + "category_code" + ";" + "insurer_inn" + ";" + "insurer_kpp" + ";" + "reg_start_code" + ";" + "reg_finish_code" + ";" + "status_id" + ";" + "kurator" + ";";

                    //Формируем результирующий файл на основании данных из БД
                    //TODO: не закомментировал создание файла статистики "SelectFromASV"
                    CreateExportFile(zagolovok, listData, nameResultFile);
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

        private static string ConvertStatusID(string statusID)
        {
            try
            {
                string tmpData = "";
                if (SelectDataFromStatusIDfile.dictionaryStatusID.TryGetValue(statusID, out tmpData))
                {
                    return SelectDataFromStatusIDfile.dictionaryStatusID[statusID];
                }
                else
                {
                    return "нет статуса в справочнике";
                }
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;

                return "ошибка поиска статуса";
            }
        }

        //------------------------------------------------------------------------------------------        
        //Формируем результирующий файл на основании данных из БД
        public static void CreateExportFile(string zagolovok, List<DataFromRKASVDB> listData, string nameFile)
        {
            try
            {
                //Добавляем в файл данные                
                using (StreamWriter writer = new StreamWriter(nameFile, true, Encoding.GetEncoding(1251)))
                {
                    writer.WriteLine(zagolovok);

                    foreach (DataFromRKASVDB item in listData)
                    {
                        writer.WriteLine(item.ToString());
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
        private static string ConvertRegNom(string regNom)
        {
            try
            {
                char[] regNomOld = regNom.ToCharArray();
                string regNomConvert = regNomOld[0].ToString() + regNomOld[1].ToString() + regNomOld[2].ToString() + "-" + regNomOld[3] + regNomOld[4] + regNomOld[5] + "-" + regNomOld[6] + regNomOld[7] + regNomOld[8] + regNomOld[9] + regNomOld[10] + regNomOld[11];


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

        //------------------------------------------------------------------------------------------
        private static string ConvertRaion(string raion)
        {
            try
            {
                if (raion.Count() == 1)
                {
                    return "042-00" + raion;
                }
                if (raion.Count() == 3)
                {
                    return "042-" + raion;
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

        //------------------------------------------------------------------------------------------
        //Выбираем код куратора
        private static string SelectKurator(string raion, string regNum)
        {
            try
            {
                string tmpKurator = "";

                //TODO: Закомментировал заполнение "частичными кодами куратора" (042-001 и 042-003) из-за их отсутствия
                //Заполняем поле Куратор
                //if (raion == "042-001" || raion == "042-003")
                //{

                //    DataFromCuratorsFilePartial tmpData = new DataFromCuratorsFilePartial();
                //    if (SelectDataFromCuratorsFilePartial.dictionary_CuratorsPartial.TryGetValue(regNum, out tmpData))
                //    {
                //        tmpKurator = tmpData.curator;
                //    }
                //}
                //else
                //{
                    DataFromCuratorsFile tmpData = new DataFromCuratorsFile();
                    if (SelectDataFromCuratorsFile.dictionary_Curators.TryGetValue(raion, out tmpData))
                    {
                        tmpKurator = tmpData.curator;
                    }
                //}

                return tmpKurator;
            }
            catch (KeyNotFoundException ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                return "";
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                return "";
            }
        }

        public static string GetRegionFromRegNum(string regNum)
        {
            if (regNum.Count() == 14)
            {
                char[] regNomOld = regNum.ToCharArray();
                string region = "0" + regNomOld[1].ToString() + regNomOld[2].ToString();

                return region;
            }
            else if (regNum.Count() == 11)
            {
                char[] regNomOld = regNum.ToCharArray();
                string region = "0" + regNomOld[0].ToString() + regNomOld[1].ToString();

                return region;
            }
            else
            {
                return "";
            }
        }

    }
}
