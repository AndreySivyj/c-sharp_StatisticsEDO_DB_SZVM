﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace StatisticsEDO_DB
{
    #region считанная из файла информация

    public class DataFromCuratorsFile
    {        
        public string raion;
        public string curator;

        public DataFromCuratorsFile(string raion = "", string curator = "")
        {
            this.raion = raion;
            this.curator = curator;            
        }


        public override string ToString()
        {
            return raion + ";" + curator + ";";
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------
    #region Выбор данных из файла
    static class SelectDataFromCuratorsFile
    {
        //количество файлов в каталоге
        private static int countFile;

        //Маска поиска файлов
        private static string fileSearchMask = "*.csv";

        public static Dictionary<string , DataFromCuratorsFile> dictionary_Curators = new Dictionary<string, DataFromCuratorsFile>();           //Коллекция данных из файлов с УП
        
        

        //------------------------------------------------------------------------------------------
        //Открываем поток для чтения из файла и выбираем нужные позиции           
        private static void ReadAndParseTextFile(string openFile)
        {
            try
            {
                using (StreamReader reader = new StreamReader(openFile, Encoding.GetEncoding(1251)))
                {
                    while (!reader.EndOfStream)
                    {
                        string strTmp = reader.ReadLine();

                        char[] separator = { ';', ',' };    //список разделителей в строке
                        string[] massiveStr = strTmp.Split(separator);     //создаем массив из строк между разделителями
                        
                        if (massiveStr.Count() == 2 && massiveStr[0].Count() == 7 && massiveStr[0] != "" && massiveStr[0] != " "  && massiveStr[1] != "" && massiveStr[1] != " ")                            
                        {
                            dictionary_Curators.Add(massiveStr[0], new DataFromCuratorsFile(massiveStr[0], massiveStr[1]));                           
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (dictionary_Curators.Count()==0)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Внимание! Нет информации о кураторах.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                
                --countFile;
            }
            catch (IOException ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                
                --countFile;
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                
                --countFile;
            }
        }



        //------------------------------------------------------------------------------------------        
        /// <summary>
        /// Выбираем данные из файлов
        /// </summary>
        /// <param name="folderIn">Каталог с обрабатываемыми файлами</param>
        public static void ObrFileFromDirectory(string folderIn)
        {
            //Очищаем коллекции для данных из файлов
            dictionary_Curators.Clear();            

            DirectoryInfo dirInfo = new DirectoryInfo(folderIn);

            try
            {
                //Вычисляем количество считанных файлов
                countFile = dirInfo.GetFiles(fileSearchMask).Count();
                //Console.WriteLine("Количество файлов для обработки: " + countFile);

            }
            catch (DirectoryNotFoundException ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                --countFile;
                Console.WriteLine("Каталог с документами не доступен.");
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());
                --countFile;
                //Console.WriteLine(ex.ToString());
            }


            if (countFile == 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Нет файлов для обработки.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {

                try
                {
                    //обрабатываем каждый файл по отдельности
                    foreach (FileInfo file in dirInfo.GetFiles(fileSearchMask))
                    {
                        //Открываем поток для чтения из файла и выбираем нужные позиции   
                        ReadAndParseTextFile(file.FullName);                        
                    }

                    //Console.WriteLine();
                    Console.WriteLine("Количество выбранных записей о кураторах (каталог \"_In_Curators\"): {0}", SelectDataFromCuratorsFile.dictionary_Curators.Count());
                    //Console.WriteLine();
                }
                catch (InvalidDataException ex)
                {
                    IOoperations.WriteLogError(ex.ToString());
                    
                    --countFile;
                }
                catch (IOException ex)
                {
                    IOoperations.WriteLogError(ex.ToString());
                    
                    --countFile;
                }
                catch (Exception ex)
                {
                    IOoperations.WriteLogError(ex.ToString());
                    
                    --countFile;
                    //Console.WriteLine(ex.ToString());
                }                            
            }
        }        
    }

    #endregion

}
