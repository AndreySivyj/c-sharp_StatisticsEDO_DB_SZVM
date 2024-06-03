using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace StatisticsEDO_DB
{
    class Svod_Itog
    {
        //public string raion;
        public string prinyatoStrah;
        public string prinyatoZL;
        public string partial_prinyatoStrah;
        public string partial_prinyatoZL;
        public string NO_proverenStrah;
        //public string NO_proverenZL;
        public string NO_prinyatoStrah;
        //public string NO_prinyatoZL;

        //public Svod_Itog(string prinyatoStrah = "", string prinyatoZL = "", string partial_prinyatoStrah = "", string partial_prinyatoZL = "",
        //    string NO_proverenStrah = "", string NO_proverenZL = "", string NO_prinyatoStrah = "", string NO_prinyatoZL = "")
        public Svod_Itog(string prinyatoStrah = "", string prinyatoZL = "", string partial_prinyatoStrah = "", string partial_prinyatoZL = "",
                            string NO_proverenStrah = "", string NO_prinyatoStrah = "")
        {
            this.prinyatoStrah = prinyatoStrah;
            this.prinyatoZL = prinyatoZL;
            this.partial_prinyatoStrah = partial_prinyatoStrah;
            this.partial_prinyatoZL = partial_prinyatoZL;
            this.NO_proverenStrah = NO_proverenStrah;
            //this.NO_proverenZL = NO_proverenZL;
            this.NO_prinyatoStrah = NO_prinyatoStrah;
            //this.NO_prinyatoZL = NO_prinyatoZL;
        }

        public override string ToString()
        {
            //return prinyatoStrah + ";" + prinyatoZL + ";" + partial_prinyatoStrah + ";" + partial_prinyatoZL + ";"
            //    + NO_proverenStrah + ";" + NO_proverenZL + ";" + NO_prinyatoStrah + ";" + NO_prinyatoZL + ";";

            return prinyatoStrah + ";" + prinyatoZL + ";" + partial_prinyatoStrah + ";" + partial_prinyatoZL + ";"
                + NO_proverenStrah + ";" + NO_prinyatoStrah + ";" ;
        }
    }

    static class CreateSvod_Itog
    {
        public static Dictionary<string, Svod_Itog> dictionarySvod_Itog_BPI = new Dictionary<string, Svod_Itog>();    //Коллекция сводных данных из реестра Perso (TKey - Raion)
        public static Dictionary<string, Svod_Itog> dictionarySvod_Itog_BPI_Centr = new Dictionary<string, Svod_Itog>();    //Коллекция сводных данных из реестра Perso (TKey - Raion)
        public static Dictionary<string, Svod_Itog> dictionarySvod_Itog_Raion = new Dictionary<string, Svod_Itog>();    //Коллекция сводных данных из реестра Perso (TKey - Raion)

        public static void CreateSvod(Dictionary<string, DataFromPersoDB> dictionaryUnikRegNomPersoALL)
        {

            try
            {
                //Очищаем коллекции
                dictionarySvod_Itog_BPI.Clear();
                dictionarySvod_Itog_BPI_Centr.Clear();
                dictionarySvod_Itog_Raion.Clear();

                //Перебираем все значения по порядку для формирования сводной информации
                foreach (var itemUnikRegNomPersoALL in dictionaryUnikRegNomPersoALL)
                {
                    //------------------------------------------------------------------------------------------
                    // "ПК БПИ" + "Принят"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Принят" && itemUnikRegNomPersoALL.Value.specChanged == "ПК БПИ")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_BPI.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре
                            dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].prinyatoStrah = (Convert.ToInt32(dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].prinyatoStrah) + 1).ToString();
                            dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].prinyatoZL = (Convert.ToInt32(dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].prinyatoZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_BPI.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("1", itemUnikRegNomPersoALL.Value.kolZLgood, "0", "0", "0", "0"));
                        }
                    }

                    // "ПК БПИ" + "Принят частично"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Принят частично" && itemUnikRegNomPersoALL.Value.specChanged == "ПК БПИ")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_BPI.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре                        
                            dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoStrah = (Convert.ToInt32(dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoStrah) + 1).ToString();
                            dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoZL = (Convert.ToInt32(dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_BPI.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("0", "0", "1", itemUnikRegNomPersoALL.Value.kolZLgood, "0", "0"));
                        }
                    }

                    // "ПК БПИ" + "Не проверен"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Не проверен" && itemUnikRegNomPersoALL.Value.specChanged == "ПК БПИ")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_BPI.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре
                            dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].NO_proverenStrah = (Convert.ToInt32(dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].NO_proverenStrah) + 1).ToString();
                            //dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].NO_proverenZL = (Convert.ToInt32(dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].NO_proverenZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_BPI.Add(itemUnikRegNomPersoALL.Value.raion,
                                //new Svod_Itog("0", "0", "0", "0", "1", itemUnikRegNomPersoALL.Value.kolZLgood, "0", "0"));
                            new Svod_Itog("0", "0", "0", "0", "1", "0"));
                        }
                    }

                    // "ПК БПИ" + "Не принят"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Не принят" && itemUnikRegNomPersoALL.Value.specChanged == "ПК БПИ")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_BPI.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре
                            dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoStrah = (Convert.ToInt32(dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoStrah) + 1).ToString();
                            //dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoZL = (Convert.ToInt32(dictionarySvod_Itog_BPI[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_BPI.Add(itemUnikRegNomPersoALL.Value.raion,
                                //new Svod_Itog("0", "0", "0", "0", "0", "0", "1", itemUnikRegNomPersoALL.Value.kolZLgood));
                            new Svod_Itog("0", "0", "0", "0", "0", "1"));
                        }
                    }



                    //------------------------------------------------------------------------------------------
                    // "ПК БПИ_Центр" + "Принят"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Принят" && itemUnikRegNomPersoALL.Value.specChanged == "ПК БПИ_Центр")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_BPI_Centr.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре
                            dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].prinyatoStrah = (Convert.ToInt32(dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].prinyatoStrah) + 1).ToString();
                            dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].prinyatoZL = (Convert.ToInt32(dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].prinyatoZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_BPI_Centr.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("1", itemUnikRegNomPersoALL.Value.kolZLgood, "0", "0", "0", "0"));
                        }
                    }

                    // "ПК БПИ_Центр" + "Принят частично"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Принят частично" && itemUnikRegNomPersoALL.Value.specChanged == "ПК БПИ_Центр")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_BPI_Centr.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре                        
                            dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoStrah = (Convert.ToInt32(dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoStrah) + 1).ToString();
                            dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoZL = (Convert.ToInt32(dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_BPI_Centr.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("0", "0", "1", itemUnikRegNomPersoALL.Value.kolZLgood, "0", "0"));
                        }
                    }

                    // "ПК БПИ_Центр" + "Не проверен"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Не проверен" && itemUnikRegNomPersoALL.Value.specChanged == "ПК БПИ_Центр")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_BPI_Centr.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре
                            dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].NO_proverenStrah = (Convert.ToInt32(dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].NO_proverenStrah) + 1).ToString();
                            //dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].NO_proverenZL = (Convert.ToInt32(dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].NO_proverenZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_BPI_Centr.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("0", "0", "0", "0", "1", "0"));
                        }
                    }

                    // "ПК БПИ_Центр" + "Не принят"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Не принят" && itemUnikRegNomPersoALL.Value.specChanged == "ПК БПИ_Центр")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_BPI_Centr.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре
                            dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoStrah = (Convert.ToInt32(dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoStrah) + 1).ToString();
                            //dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoZL = (Convert.ToInt32(dictionarySvod_Itog_BPI_Centr[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_BPI_Centr.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("0", "0", "0", "0", "0", "1"));
                        }
                    }



                    //------------------------------------------------------------------------------------------
                    // "Специалист" + "Принят"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Принят" && itemUnikRegNomPersoALL.Value.specChanged == "Специалист")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_Raion.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре
                            dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].prinyatoStrah = (Convert.ToInt32(dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].prinyatoStrah) + 1).ToString();
                            dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].prinyatoZL = (Convert.ToInt32(dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].prinyatoZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_Raion.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("1", itemUnikRegNomPersoALL.Value.kolZLgood, "0", "0", "0", "0"));
                        }
                    }

                    // "Специалист" + "Принят частично"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Принят частично" && itemUnikRegNomPersoALL.Value.specChanged == "Специалист")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_Raion.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре                        
                            dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoStrah = (Convert.ToInt32(dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoStrah) + 1).ToString();
                            dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoZL = (Convert.ToInt32(dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].partial_prinyatoZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_Raion.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("0", "0", "1", itemUnikRegNomPersoALL.Value.kolZLgood, "0", "0"));
                        }
                    }

                    // "Специалист" + "Не проверен"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Не проверен" && itemUnikRegNomPersoALL.Value.specChanged == "Специалист")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_Raion.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре
                            dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].NO_proverenStrah = (Convert.ToInt32(dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].NO_proverenStrah) + 1).ToString();
                            //dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].NO_proverenZL = (Convert.ToInt32(dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].NO_proverenZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_Raion.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("0", "0", "0", "0", "1", "0"));
                        }
                    }

                    // "Специалист" + "Не принят"
                    if (itemUnikRegNomPersoALL.Value.resultat == "Не принят" && itemUnikRegNomPersoALL.Value.specChanged == "Специалист")
                    {
                        //Добавляем выбранные данные в коллекцию
                        Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                        if (dictionarySvod_Itog_Raion.TryGetValue(itemUnikRegNomPersoALL.Value.raion, out tmpDataFrom_Svod_Itog))
                        {
                            //есть район в словаре
                            dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoStrah = (Convert.ToInt32(dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoStrah) + 1).ToString();
                            //dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoZL = (Convert.ToInt32(dictionarySvod_Itog_Raion[itemUnikRegNomPersoALL.Value.raion].NO_prinyatoZL) + Convert.ToInt32(itemUnikRegNomPersoALL.Value.kolZLgood)).ToString();
                        }
                        else
                        {
                            //нет района в словаре
                            dictionarySvod_Itog_Raion.Add(itemUnikRegNomPersoALL.Value.raion,
                                new Svod_Itog("0", "0", "0", "0", "0", "1"));
                        }
                    }
                }

                //Формируем файл статистики "Perso_СтатусКвитанции_Error_"
                string zagolovok_Svod_Itog =
                    "БПИ_принятоСтрах" + ";" + "БПИ_принятоЗЛ" + ";" + "БПИ_частично_принятоСтрах" + ";" + "БПИ_частично_принятоЗЛ" + ";"
                    + "БПИ_Не_проверенСтрах" + ";" + "БПИ_Не_принятоСтрах" + ";"

                    + "БПИ_Центр_принятоСтрах" + ";" + "БПИ_Центр_принятоЗЛ" + ";" + "БПИ_Центр_частично_принятоСтрах" + ";" + "БПИ_Центр_частично_принятоЗЛ" + ";"
                    + "БПИ_Центр_Не_проверенСтрах" + ";" + "БПИ_Центр_Не_принятоСтрах" + ";" 

                    + "Район_принятоСтрах" + ";" + "Район_принятоЗЛ" + ";" + "Район_частично_принятоСтрах" + ";" + "Район_частично_принятоЗЛ" + ";"
                    + "Район_Не_проверенСтрах" + ";" + "Район_Не_принятоСтрах" + ";" ;

                //"БПИ_принятоСтрах" + ";" + "БПИ_принятоЗЛ" + ";" + "БПИ_partial_prinyatoСтрах" + ";" + "БПИ_partial_prinyatoZL" + ";"
                //+ "БПИ_NO_proverenСтрах" + ";" + "БПИ_NO_proverenZL" + ";" + "БПИ_NO_prinyatoСтрах" + ";" + "БПИ_NO_prinyatoZL" + ";"

                //+ "BPI_Centr_принятоСтрах" + ";" + "BPI_Centr_принятоЗЛ" + ";" + "BPI_Centr_partial_prinyatoСтрах" + ";" + "BPI_Centr_partial_prinyatoZL" + ";"
                //+ "BPI_Centr_NO_proverenСтрах" + ";" + "BPI_Centr_NO_proverenZL" + ";" + "BPI_Centr_NO_prinyatoСтрах" + ";" + "BPI_Centr_NO_prinyatoZL" + ";"

                //+ "Raion_принятоСтрах" + ";" + "Raion_принятоЗЛ" + ";" + "Raion_partial_prinyatoStrah" + ";" + "Raion_partial_prinyatoZL" + ";"
                //+ "Raion_NO_proverenStrah" + ";" + "Raion_NO_proverenZL" + ";" + "Raion_NO_prinyatoStrah" + ";" + "Raion_NO_prinyatoZL" + ";";

                string resultFile_Svod_Itog = IOoperations.katalogOut + @"\" + @"_УникРегНомерPerso_Свод_" + DateTime.Now.ToShortDateString() + ".csv";

                WriteLogsSvod_Itog(resultFile_Svod_Itog, zagolovok_Svod_Itog, dictionarySvod_Itog_BPI, dictionarySvod_Itog_BPI_Centr, dictionarySvod_Itog_Raion);
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

        private static void WriteLogsSvod_Itog(string resultFile_Svod_Itog, string zagolovok_Svod_Itog, Dictionary<string, Svod_Itog> dictionarySvod_Itog_BPI, Dictionary<string, Svod_Itog> dictionarySvod_Itog_BPI_Centr, Dictionary<string, Svod_Itog> dictionarySvod_Itog_Raion)
        {
            //Проверяем на наличие файла отчета, если существует - удаляем
            if (File.Exists(resultFile_Svod_Itog))
            {
                IOoperations.FileDelete(resultFile_Svod_Itog);
            }

            //формируем результирующий файл статистики
            using (StreamWriter writer = new StreamWriter(resultFile_Svod_Itog, false, Encoding.GetEncoding(1251)))
            {
                writer.WriteLine(zagolovok_Svod_Itog);

                for (int raion = 1; raion < 35; raion++)
                {
                    string tmpStr = "";

                    //------------------------------------------------------------------------------------------
                    // "ПК БПИ"
                    Svod_Itog tmpDataFrom_Svod_Itog = new Svod_Itog();
                    if (dictionarySvod_Itog_BPI.TryGetValue(raion.ToString(), out tmpDataFrom_Svod_Itog))
                    {
                        //есть район в словаре
                        tmpStr = tmpStr + dictionarySvod_Itog_BPI[raion.ToString()].ToString();
                    }
                    else
                    {
                        //нет района в словаре
                        tmpStr = tmpStr + "0" + ";" + "0" + ";" + "0" + ";" + "0" + ";" + "0" + ";" + "0" + ";" ;
                    }

                    //------------------------------------------------------------------------------------------
                    // "ПК БПИ_Центр"                    
                    if (dictionarySvod_Itog_BPI_Centr.TryGetValue(raion.ToString(), out tmpDataFrom_Svod_Itog))
                    {
                        //есть район в словаре
                        tmpStr = tmpStr + dictionarySvod_Itog_BPI_Centr[raion.ToString()].ToString();
                    }
                    else
                    {
                        //нет района в словаре
                        tmpStr = tmpStr + "0" + ";" + "0" + ";" + "0" + ";" + "0" + ";" + "0" + ";" + "0" + ";" ;
                    }

                    //------------------------------------------------------------------------------------------
                    // "Специалист"                    
                    if (dictionarySvod_Itog_Raion.TryGetValue(raion.ToString(), out tmpDataFrom_Svod_Itog))
                    {
                        //есть район в словаре
                        tmpStr = tmpStr + dictionarySvod_Itog_Raion[raion.ToString()].ToString();
                    }
                    else
                    {
                        //нет района в словаре
                        tmpStr = tmpStr + "0" + ";" + "0" + ";" + "0" + ";" + "0" + ";" + "0" + ";" + "0" + ";" ;
                    }

                    writer.WriteLine(tmpStr);
                }
            }
        }

    }
}
