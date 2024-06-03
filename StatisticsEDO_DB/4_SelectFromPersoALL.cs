using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;

namespace StatisticsEDO_DB
{
    static class SelectSposobPredstavleniya
    {
        public static Dictionary<string, string> dictionarySposobPredstavleniya = new Dictionary<string, string>();  //Коллекция данных из файла 

        private static HashSet<string> hashSet_EDO = new HashSet<string>();  //Коллекция данных из файла
        private static HashSet<string> hashSet_BPI = new HashSet<string>();  //Коллекция данных из файла
        private static HashSet<string> hashSet_BPI_Centr = new HashSet<string>();  //Коллекция данных из файла
        private static HashSet<string> hashSet_UPFR = new HashSet<string>();  //Коллекция данных из файла

        //------------------------------------------------------------------------------------------        
        public static void SelectSposobPerdstavl()
        {
            try
            {
                //Очищаем коллекцию для данных из файла                    
                hashSet_BPI.Clear();
                hashSet_BPI_Centr.Clear();
                hashSet_UPFR.Clear();
                hashSet_EDO.Clear();
                dictionarySposobPredstavleniya.Clear();



                foreach (var itemDataFromPersoDB in SelectDataFromPersoDB.list_dataFromPersoDB)
                {
                    switch (itemDataFromPersoDB.specChanged)
                    {
                        case "ПК БПИ":
                            hashSet_BPI.Add(itemDataFromPersoDB.regNum);
                            hashSet_EDO.Add(itemDataFromPersoDB.regNum);
                            break;

                        case "ПК БПИ_Центр":
                            hashSet_BPI_Centr.Add(itemDataFromPersoDB.regNum);
                            hashSet_EDO.Add(itemDataFromPersoDB.regNum);
                            break;

                        case "Специалист":
                            hashSet_UPFR.Add(itemDataFromPersoDB.regNum);
                            break;

                        default:
                            break;
                    }
                }

                //------------------------------------------------------------------------------------------
                //Сравниваем выбранные данные между собой и создаем коллекцию со способом представления

                //Способ "ПКБПИ"
                foreach (var itemRegNum in hashSet_BPI)
                {
                    dictionarySposobPredstavleniya.Add(itemRegNum, "ПК БПИ");
                }

                //Способ "Специалист"
                var list_ExceptUPFR = hashSet_UPFR.Except(hashSet_EDO);

                foreach (var itemRegNum in list_ExceptUPFR)
                {
                    dictionarySposobPredstavleniya.Add(itemRegNum, "Специалист");
                }

                //Способ "ПКБПИ_Центр"
                var list_ExceptBPI_Centr = hashSet_BPI_Centr.Except(hashSet_BPI);

                foreach (var itemRegNum in list_ExceptBPI_Centr)
                {
                    dictionarySposobPredstavleniya.Add(itemRegNum, "ПК БПИ_Центр");
                }
                


                //------------------------------------------------------------------------------------------
                //Наполняем данными коллекции dictionarySposobPredstavleniya коллекцию SelectDataForResultFile.dictionaryUnikRegNomPersoALL                
                foreach (var itemSposobPredstavleniya in dictionarySposobPredstavleniya)
                {
                    //reestrReorganiz 
                    DataFromPersoDB tmpDataFromPersoDB = new DataFromPersoDB();
                    if (SelectDataForResultFile.dictionaryUnikRegNomPersoALL.TryGetValue(itemSposobPredstavleniya.Key, out tmpDataFromPersoDB))
                    {
                        SelectDataForResultFile.dictionaryUnikRegNomPersoALL[itemSposobPredstavleniya.Key].specChanged = itemSposobPredstavleniya.Value;                        
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

    }
}
