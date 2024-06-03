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
    static class SelectDataFromPersoDB_CountZL
    {
        async public static void SelectDataFromPerso_CountZL(string regNumItem, string otchYear, string otchMonth)
        {
            // название процедуры
            string sqlExpression = "PERS.q0301_f_szvm_strnum_list_wo_otmn";

            using (DB2Connection connection = new DB2Connection("Server=1.1.1.1:50000;Database=PERSDB;UID=regusr;PWD=password;"))
            {
                //открываем соединение
                await connection.OpenAsync();

                DB2Command command3 = new DB2Command(sqlExpression, connection);
                // указываем, что команда представляет хранимую процедуру
                command3.CommandType = System.Data.CommandType.StoredProcedure;

                command3.Parameters.Add("regNumStart", regNumItem);

                command3.Parameters.Add("regNumEnd", regNumItem);

                command3.Parameters.Add("otchYear", otchYear);

                command3.Parameters.Add("otchMonth", otchMonth);

                DbDataReader reader = await command3.ExecuteReaderAsync();

                int i = 0;

                while (await reader.ReadAsync())
                {
                    //------------------------------------------------------------------------------------------
                    //Наполняем коллекцию уникальных регномеров + СНИЛС + отчПериод из БД Perso
                    Program.persoDBunikRegNomAndSNILS.Add(ConvertRegNom(reader[0].ToString()) + ";" + ConvertSNILS(reader[1].ToString())+ ";");
                    //------------------------------------------------------------------------------------------
                    


                    //Добавляем выбранные данные в коллекцию
                    int snils = 0;
                    if (Program.dictionaryDataCountZLPerso.TryGetValue(ConvertRegNom(reader[0].ToString()), out snils))
                    {
                        //есть рег.Номер в словаре
                        ++Program.dictionaryDataCountZLPerso[ConvertRegNom(reader[0].ToString())];// = Program.dictionaryDataCountZLPerso[ConvertRegNom(reader[0].ToString())] + 1;
                        
                    }
                    else
                    {
                        //нет рег.Номера в словаре
                        Program.dictionaryDataCountZLPerso.Add(ConvertRegNom(reader[0].ToString()), 1);
                    }
                    
                    i++;
                }
                reader.Close();
                
            }
        }

        private static string ConvertSNILS(string snils)
        {
            string convertSNILS = "";
            for (int i = 0; i < snils.Count()-2; i++)
            {
                convertSNILS = convertSNILS + snils[i];
            }

            return convertSNILS;
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
