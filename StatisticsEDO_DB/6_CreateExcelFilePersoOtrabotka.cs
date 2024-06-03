using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;
//using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

namespace StatisticsEDO_DB
{
    static class CreateExcelFilePersoOtrabotka
    {
        //коллекция для считанных заголовков таблицы
        //public static List<string> list_table_zagolovki = new List<string>();
        //коллекция для считанных данных таблицы
        //public static List<List<string>> list_table = new List<List<string>>();

        //------------------------------------------------------------------------------------------
        //создаем Excel-файл и наполняем его данными
        public static void CreateNewExcelFile(Dictionary<string, DataFromPersoOtrabotkaFile> dictionaryPersoOtrabotkaOld)
        {
            //экземпляр приложения
            Excel.Application excelApp = null;

            //экземпляр рабочей книги Excel
            Excel.Workbook workBook = null;

            //экземпляр листа Excel
            Excel.Worksheet workSheet = null;

            try
            {
                //Создаем экземпляр приложения
                excelApp = new Excel.Application();

                excelApp.ScreenUpdating = false;
                excelApp.EnableEvents = false;
                excelApp.Visible = false;


                //количество листов в рабочей книге
                excelApp.SheetsInNewWorkbook = 1;

                //Создаем экземпляр рабочей книги Excel
                workBook = excelApp.Workbooks.Add();

                //получаем первый лист документа (счет начинается с 1)                
                workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);

                //название листа (вкладки внизу)
                workSheet.Name = "Perso_Отработка";

                //------------------------------------------------------------------------------------------
                //Заполняем первую строку заголовками

                //for (int i = 1; i <= list_table_zagolovki.Count(); i++)
                //{
                //    workSheet.Cells[1, i] = list_table_zagolovki[i - 1];
                //}



                //string zagolovokError = "№ п/п" + ";" + "КодЗап" + ";" + "Район" + ";" + "РегНомер" + ";" + "Наименование" + ";" + "ОтчМесяц" + ";" + "ОтчГод" + ";" + "Тип сведений" + ";" + "Дата представления" + ";" + "Категория" + ";"
                //                                + "Дата постановки в ПФР" + ";" + "Дата постановки в РО" + ";" + "Дата снятия в РО" + ";" + "Результат проверки" + ";" + "Дата проверки" + ";" + "Количество ЗЛ в файле" + ";" 
                //                                  + "ЗЛ принято" + ";" + "ЗЛ не принято" + ";"
                //                                + "Статус квитанции" + ";" + "Специалист" + ";" + "Способ представления" + ";" + "Куратор" + ";" + "УП по данным УПФР" + ";"
                //                                + "Дата направления уведомления страхователю" + ";" + "Контрольная дата для исправления (3 дня)" + ";" + "Исправлено (да|нет|не требуется)" + ";"
                //                                + "Дата направления реестра в УПФР (в случае неисправления)" + ";" + "Дата исправления ошибки (после направления реестра УПФР)" + ";" + "Примечание" + ";"
                //                                + "Результат контроля (руководитель)" + ";";

                int i = 1;  //номер первой строки для заголовков

                workSheet.Cells[i, 1] = "№ п/п";
                workSheet.Cells[i, 2] = "КодЗап";
                workSheet.Cells[i, 3] = "Район";
                workSheet.Cells[i, 4] = "РегНомер";
                workSheet.Cells[i, 5] = "Наименование";
                workSheet.Cells[i, 6] = "ОтчМесяц";
                workSheet.Cells[i, 7] = "ОтчГод";
                workSheet.Cells[i, 8] = "Тип сведений";
                workSheet.Cells[i, 9] = "Дата представления";
                workSheet.Cells[i, 10] = "Категория";
                workSheet.Cells[i, 11] = "Дата постановки в ПФР";
                workSheet.Cells[i, 12] = "Дата постановки в РО";
                workSheet.Cells[i, 13] = "Дата снятия в РО";
                workSheet.Cells[i, 14] = "Результат проверки";
                workSheet.Cells[i, 15] = "Дата проверки";
                workSheet.Cells[i, 16] = "Количество ЗЛ в файле";
                workSheet.Cells[i, 17] = "ЗЛ принято";
                workSheet.Cells[i, 18] = "ЗЛ не принято";
                workSheet.Cells[i, 19] = "Статус квитанции";
                workSheet.Cells[i, 20] = "Специалист";
                workSheet.Cells[i, 21] = "Способ представления";
                workSheet.Cells[i, 22] = "Куратор";
                workSheet.Cells[i, 23] = "УП по данным УПФР";
                workSheet.Cells[i, 24] = "Дата направления уведомления страхователю";
                workSheet.Cells[i, 25] = "Контрольная дата для исправления (3 дня)";
                workSheet.Cells[i, 26] = "Исправлено (да|нет|не требуется)";
                workSheet.Cells[i, 27] = "Дата направления реестра в УПФР (в случае неисправления)";
                workSheet.Cells[i, 28] = "Дата исправления ошибки (после направления реестра УПФР)";
                workSheet.Cells[i, 29] = "Примечание";
                workSheet.Cells[i, 30] = "Результат контроля (руководитель)";

                //------------------------------------------------------------------------------------------
                //захватываем диапазон ячеек с заголовками
                //TODO: Внимание! При изменении количества ячеек с данными изменить значение диапазона
                Excel.Range rangeZagolovok = workSheet.Range[workSheet.Cells[i, 1], workSheet.Cells[i, 30]];
                //шрифт для захваченного диапазона
                rangeZagolovok.Cells.Font.Name = "Calibri";
                //размер шрифта для захваченного диапазона
                rangeZagolovok.Cells.Font.Size = 12;
                rangeZagolovok.Cells.Font.Bold = "true";
                rangeZagolovok.Cells.Font.Color = ColorTranslator.ToOle(Color.DarkBlue);

                //расставляем рамки со всех сторон
                rangeZagolovok.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeZagolovok.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeZagolovok.Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeZagolovok.Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeZagolovok.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;

                //устанавливаем цвет рамки
                rangeZagolovok.Borders.Color = ColorTranslator.ToOle(Color.DarkBlue);

                //задаем выравнивание в диапазоне
                rangeZagolovok.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                rangeZagolovok.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                //перенос текста по словам
                rangeZagolovok.WrapText = true;

                //авто ширина и авто высота
                rangeZagolovok.EntireColumn.AutoFit();
                rangeZagolovok.EntireRow.AutoFit();

                //автофильтр на заголовках                
                rangeZagolovok.AutoFilter(1, System.Reflection.Missing.Value, Excel.XlAutoFilterOperator.xlAnd, System.Reflection.Missing.Value, true);



                //------------------------------------------------------------------------------------------
                //Заполняем строки

                int n = 1;  //номер по порядку
                int j = 2;  //номер первой строки для данных
                foreach (var row in dictionaryPersoOtrabotkaOld)
                {
                    //return codZap + ";" + raion + ";" + regNum + ";" + nameStrah + ";" + month + ";" + year + ";" + typeSved + ";" + dataPredst + ";" + kategory + ";" + dataPostPFR + ";"
                    //        + dataPostRO + ";" + dataSnyatRO + ";" + resultat + ";" + dataProverki + ";" + kolZL + ";" + kolZLgood + ";" + kolZLbad + ";" + statusKvitanc + ";"
                    //         + spec + ";" + specChanged + ";" + kurator + ";" + UP + ";" + dataNaprUvedomlStrah + ";" + kontrDataIspravleniya + ";" + statusIspravleniya + ";"
                    //         + dataNaprReestraPFR + ";" + dataIspravleniyaError + ";" + primechanie + ";" + resultatKontrolya + ";";

                    workSheet.Cells[j, 1] = n;
                    workSheet.Cells[j, 2] = row.Value.codZap;
                    workSheet.Cells[j, 3] = row.Value.raion;
                    workSheet.Cells[j, 4] = row.Value.regNum;
                    workSheet.Cells[j, 5] = row.Value.nameStrah;
                    workSheet.Cells[j, 6] = row.Value.month;
                    workSheet.Cells[j, 7] = row.Value.year;
                    workSheet.Cells[j, 8] = row.Value.typeSved;
                    workSheet.Cells[j, 9] = row.Value.dataPredst;
                    workSheet.Cells[j, 10] = row.Value.kategory;
                    workSheet.Cells[j, 11] = row.Value.dataPostPFR;
                    workSheet.Cells[j, 12] = row.Value.dataPostRO;
                    workSheet.Cells[j, 13] = row.Value.dataSnyatRO;
                    workSheet.Cells[j, 14] = row.Value.resultat;
                    workSheet.Cells[j, 15] = row.Value.dataProverki;
                    workSheet.Cells[j, 16] = row.Value.kolZL;
                    workSheet.Cells[j, 17] = row.Value.kolZLgood;
                    workSheet.Cells[j, 18] = row.Value.kolZLbad;
                    workSheet.Cells[j, 19] = row.Value.statusKvitanc;
                    workSheet.Cells[j, 20] = row.Value.spec;
                    workSheet.Cells[j, 21] = row.Value.specChanged;
                    workSheet.Cells[j, 22] = row.Value.kurator;
                    workSheet.Cells[j, 23] = row.Value.UP;
                    workSheet.Cells[j, 24] = row.Value.dataNaprUvedomlStrah;
                    workSheet.Cells[j, 25] = row.Value.kontrDataIspravleniya;
                    workSheet.Cells[j, 26] = row.Value.statusIspravleniya;
                    workSheet.Cells[j, 27] = row.Value.dataNaprReestraPFR;
                    workSheet.Cells[j, 28] = row.Value.dataIspravleniyaError;
                    workSheet.Cells[j, 29] = row.Value.primechanie;
                    workSheet.Cells[j, 30] = row.Value.resultatKontrolya;

                    //for (int i = 1; i <= row.Value.Count(); i++)
                    //{
                    //    workSheet.Cells[j, i] = row[i - 1];
                    //}

                    n++;
                    j++;
                }


                //------------------------------------------------------------------------------------------
                //заполнение номера по порядку (замена на новые значения)
                //Excel.Range nomPP = workSheet.Range[workSheet.Cells[2, 1], workSheet.Cells[3, 1]];
                //Excel.Range destinationNomPP = workSheet.Range[workSheet.Cells[2, 1], workSheet.Cells[DataFromFile.list_table.Count() + 1, 1]];

                //nomPP.AutoFill(destinationNomPP, Excel.XlAutoFillType.xlFillDefault);


                //------------------------------------------------------------------------------------------
                //захватываем диапазон ячеек с данными
                //TODO: Внимание! При изменении количества ячеек с данными изменить значение диапазона
                Excel.Range rangeData = workSheet.Range[workSheet.Cells[2, 1], workSheet.Cells[dictionaryPersoOtrabotkaOld.Count() + 1, 30]];
                //шрифт для захваченного диапазона
                rangeData.Cells.Font.Name = "Calibri";
                //размер шрифта для захваченного диапазона
                rangeData.Cells.Font.Size = 12;

                //расставляем рамки со всех сторон
                rangeData.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeData.Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeData.Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeData.Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeData.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = Excel.XlLineStyle.xlContinuous;

                //устанавливаем цвет рамки
                //range2.Borders.Color = ColorTranslator.ToOle(Color.Red);

                //задаем выравнивание в диапазоне
                rangeData.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                rangeData.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                

                //авто ширина и авто высота
                //TODO: Внимание! При изменении количества ячеек с данными изменить значение диапазона
                Excel.Range rangeData1 = workSheet.Range[workSheet.Cells[2, 1], workSheet.Cells[dictionaryPersoOtrabotkaOld.Count() + 1, 4]];
                rangeData1.EntireColumn.AutoFit();
                rangeData1.EntireRow.AutoFit();
                Excel.Range rangeData2 = workSheet.Range[workSheet.Cells[2, 6], workSheet.Cells[dictionaryPersoOtrabotkaOld.Count() + 1, 30]];
                rangeData2.EntireColumn.AutoFit();
                rangeData2.EntireRow.AutoFit();

                //Устанавливаем параметры для поля "Примечание"
                Excel.Range rangePrimechanie = workSheet.Range[workSheet.Cells[2,29], workSheet.Cells[dictionaryPersoOtrabotkaOld.Count() + 1, 29]];
                rangePrimechanie.ColumnWidth = 31;
                rangePrimechanie.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                rangePrimechanie.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;
                //перенос текста по словам
                rangePrimechanie.WrapText = true;



                //закрепить область
                workSheet.Activate();
                workSheet.Application.ActiveWindow.FreezePanes = false;
                workSheet.Application.ActiveWindow.SplitRow = 1;
                workSheet.Application.ActiveWindow.SplitColumn = 0;
                workSheet.Application.ActiveWindow.FreezePanes = true;


                //------------------------------------------------------------------------------------------
                //закрепить область

                //TODO: реализовать закрепление области

                /*                
                Range("A2").Select
                With ActiveWindow
                .SplitColumn = 0
                .SplitRow = 1
                End With
                ActiveWindow.FreezePanes = True
                End Sub
                */


                /*

                //------------------------------------------------------------------------------------------
                //Сводная таблица 

                //экземпляр листа для сводной
                Excel.Worksheet workSheetSvod = null;
                //добавляем новый лист                 
                workSheetSvod = (Excel.Worksheet)workBook.Worksheets.Add();
                //название листа (вкладки внизу)
                workSheetSvod.Name = "Сводная_таблица";

                //диапазон с данными для сводной таблицы
                Excel.Range rangeSvod = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[DataFromFile.list_table.Count() + 1, DataFromFile.list_table[0].Count()]];

                //начальная ячейка для сводной таблицы
                Excel.Range rangeForSvod = workSheetSvod.Cells[3, 1];

                Excel.PivotCache pivotCache = (Excel.PivotCache)workBook.PivotCaches().Create(Excel.XlPivotTableSourceType.xlDatabase, rangeSvod);
                Excel.PivotTable pivotTable = (Excel.PivotTable)workSheetSvod.PivotTables().Add(pivotCache, rangeForSvod, "PivotTable");

                Excel.PivotField pivotField = (Excel.PivotField)pivotTable.PivotFields("Код куратора");
                pivotField.set_Subtotals(1, false);//Убирает промежуточные итоги
                pivotField.Orientation = Excel.XlPivotFieldOrientation.xlRowField;
                pivotField.Position = 1;

                Excel.PivotField InnoPivotField = (Excel.PivotField)pivotTable.PivotFields(@"Статус (Приняты ""+""/Приняты частично ""*""/Ошибки ""-"")");
                InnoPivotField.set_Subtotals(1, false);//Убирает промежуточные итоги
                InnoPivotField.Orientation = Excel.XlPivotFieldOrientation.xlColumnField;


                Excel.PivotField RVPSPivotField = (Excel.PivotField)pivotTable.PivotFields(@"Рег# № страхователя");
                RVPSPivotField.set_Subtotals(1, false);//Убирает промежуточные итоги
                RVPSPivotField.Orientation = Excel.XlPivotFieldOrientation.xlDataField;
                RVPSPivotField.Function = Excel.XlConsolidationFunction.xlCount;

                pivotTable.RowAxisLayout(Excel.XlLayoutRowType.xlTabularRow); //Табличный вид


                //авто ширина и авто высота для сводной таблицы

                //диапазон с данными для сводной таблицы
                Excel.Range newrangeSvod = workSheetSvod.Range[workSheetSvod.Cells[1, 1], workSheetSvod.Cells[DataFromFile.list_table.Count() + 1, DataFromFile.list_table[0].Count()]];
                newrangeSvod.EntireColumn.AutoFit();
                newrangeSvod.EntireRow.AutoFit();

                */

                /*
Columns("A:I").Select
    Sheets.Add
    ActiveWorkbook.PivotCaches.Create(SourceType:=xlDatabase, SourceData:= _
        "Реестр_общий!R1C1:R1048576C9", Version:=xlPivotTableVersion12). _
        CreatePivotTable TableDestination:="Лист1!R3C1", TableName:= _
        "СводнаяТаблица1", DefaultVersion:=xlPivotTableVersion12
    Sheets("Лист1").Select
    Cells(3, 1).Select
    With ActiveSheet.PivotTables("СводнаяТаблица1").PivotFields("Код куратора")
        .Orientation = xlRowField
        .Position = 1
    End With
    With ActiveSheet.PivotTables("СводнаяТаблица1").PivotFields( _
        "Статус (Приняты ""+""/Приняты частично ""*""/Ошибки ""-"")")
        .Orientation = xlColumnField
        .Position = 1
    End With
    ActiveSheet.PivotTables("СводнаяТаблица1").AddDataField ActiveSheet.PivotTables _
        ("СводнаяТаблица1").PivotFields("Рег# № страхователя"), _
        "Количество по полю Рег# № страхователя", xlCount


    Range("A4:G9").Select
    Range("G9").Activate
    Selection.Borders(xlDiagonalDown).LineStyle = xlNone
    Selection.Borders(xlDiagonalUp).LineStyle = xlNone
    With Selection.Borders(xlEdgeLeft)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlEdgeTop)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlEdgeBottom)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlEdgeRight)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlInsideVertical)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlInsideHorizontal)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    Sheets("Реестр_общий").Select
End Sub
                */


                try
                {
                    //Создаем результирующий каталог
                    if (!Directory.Exists(@"C:\_Out"))
                        Directory.CreateDirectory(@"C:\_Out");

                    workBook.Saved = true;

                    //Опасно использовать (не показываем предупреждения от Excel)
                    //excelApp.DisplayAlerts = false;

                    //Лучше использовать проверку наличия файла
                    string fileNameExcel = @"C:\_Out" + @"\" + @"6_Perso_Отработка_" + DateTime.Now.ToShortDateString() + "_.xlsx";
                    if (File.Exists(fileNameExcel)) { File.Delete(fileNameExcel); }

                    excelApp.DefaultSaveFormat = Excel.XlFileFormat.xlHtml;
                    workBook.SaveAs(fileNameExcel,  //object Filename
                       Type.Missing,          //object FileFormat
                       Type.Missing,                       //object Password 
                       Type.Missing,                       //object WriteResPassword  
                       Type.Missing,                       //object ReadOnlyRecommended
                       Type.Missing,                       //object CreateBackup
                       Excel.XlSaveAsAccessMode.xlNoChange,//XlSaveAsAccessMode AccessMode
                       Type.Missing,                       //object ConflictResolution
                       Type.Missing,                       //object AddToMru 
                       Type.Missing,                       //object TextCodepage
                       Type.Missing,                       //object TextVisualLayout
                       Type.Missing);                      //object Local



                    //Опасно использовать (возвращаем предупреждения от Excel)
                    //excelApp.DisplayAlerts = true;

                    excelApp.Quit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    excelApp.Quit();
                }





                /*
                //------------------------------------------------------------------------------------------
                //Опасно использовать (не показываем предупреждения от Excel)
                //excelApp.DisplayAlerts = false;

                //Лучше использовать проверку наличия файла
                string fileNameExcel = IOoperations.katalogOut + @"\" + @"reestr.xlsx";
                if (File.Exists(fileNameExcel)) { File.Delete(fileNameExcel); }

                // save changes
                workBook.SaveAs(IOoperations.katalogOut + @"\" + @"reestr.xlsx",
                                  Excel.XlFileFormat.xlWorkbookNormal,
                                  Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                  Excel.XlSaveAsAccessMode.xlExclusive,
                                  Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                workBook.Close(true, Type.Missing, Type.Missing);
                excelApp.Quit();

                //Опасно использовать (возвращаем предупреждения от Excel)
                //excelApp.DisplayAlerts = true;
                //------------------------------------------------------------------------------------------
                */


                /*
                //------------------------------------------------------------------------------------------
                //Открываем созданный excel-файл или сохраняем файл
                excelApp.Visible = false;
                excelApp.UserControl = true;

                //string folder = IOoperations.katalogOut;                          //путь папке   
                string fileName = "reestr.xlsx";                                    //имя Excel-файла
                                                                                    //string fullFileName = System.IO.Path.Combine(folder, fileName);

                //if (System.IO.File.Exists(fullFileName))
                //{
                //    System.IO.File.Delete(fullFileName);
                //}

                excelApp.Application.ActiveWorkbook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange,
                                                                Excel.XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                //корректно закрываем Excel
                workBook.Close();  //конкретная книга;
                excelApp.Quit();    //приложение Excel.  
                */
            }
            catch (Exception ex)
            {
                IOoperations.WriteLogError(ex.ToString());
            }
            finally
            {
                //корректно очищаем память от COM-объектов
                if (workSheet != null) Marshal.ReleaseComObject(workSheet);
                if (workBook != null) Marshal.ReleaseComObject(workBook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);

                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }






        }


        //public static void CreatePivotTable()
        //{
        //    this.Range["A1"].Value2 = "Date";
        //    this.Range["A2"].Value2 = "March 1";
        //    this.Range["A3"].Value2 = "March 8";
        //    this.Range["A4"].Value2 = "March 15";

        //    this.Range["B1"].Value2 = "Customer";
        //    this.Range["B2"].Value2 = "Smith";
        //    this.Range["B3"].Value2 = "Jones";
        //    this.Range["B4"].Value2 = "James";

        //    this.Range["C1"].Value2 = "Sales";
        //    this.Range["C2"].Value2 = "23";
        //    this.Range["C3"].Value2 = "17";
        //    this.Range["C4"].Value2 = "39";

        //    Excel.PivotTable table1 = ExcelTools.WorksheetBase.PivotTableWizard(
        //        Excel.XlPivotTableSourceType.xlDatabase, this.RangeSvod["A1", "C4"],
        //        "PivotTable1", false, false, true, false, false, false, Excel.XlOrder.xlDownThenOver);



        //}


        /*


//сводная таблица

Excel.Range oRange = WkSht.get_Range("A4:AN1629");   //Испарвить на ЛастРоу

oSheet = (Excel.Worksheet)WB.Worksheets.Add();

oSheet.Name = "Pivot Table";


Excel.Range oRange2 = oSheet.Cells[3, 1];


Excel.PivotCache oPivotCache = (Excel.PivotCache)WB.PivotCaches().Create(Excel.XlPivotTableSourceType.xlDatabase, oRange);
Excel.PivotTable oPivoTable = (Excel.PivotTable)oSheet.PivotTables().Add(oPivotCache, oRange2, "PivotTable");


Excel.PivotField oPivotField = (Excel.PivotField)oPivoTable.PivotFields("2_1");
oPivotField.set_Subtotals(1, false);
oPivotField.Orientation = Excel.XlPivotFieldOrientation.xlRowField;


oPivotField.Position = 1;

Excel.PivotField InnoPivotField = (Excel.PivotField)oPivoTable.PivotFields("5");
InnoPivotField.set_Subtotals(1, false);//Убирает промежуточные итоги
InnoPivotField.Orientation = Excel.XlPivotFieldOrientation.xlRowField;


Excel.PivotField RVPSPivotField = (Excel.PivotField)oPivoTable.PivotFields("27");
RVPSPivotField.set_Subtotals(1, false);
RVPSPivotField.Orientation = Excel.XlPivotFieldOrientation.xlDataField;
RVPSPivotField.Function = Excel.XlConsolidationFunction.xlSum;

oPivoTable.RowAxisLayout(Excel.XlLayoutRowType.xlTabularRow); //Табличный вид









Sub Макрос1()
'
' Макрос1 Макрос
'

'
    ActiveWindow.FreezePanes = True
    Rows("1:1").Select
    Selection.AutoFilter
    ActiveWindow.LargeScroll ToRight:=-1

//сводная
    Columns("A:I").Select
    Sheets.Add
    ActiveWorkbook.PivotCaches.Create(SourceType:=xlDatabase, SourceData:= _
        "Реестр_общий!R1C1:R1048576C9", Version:=xlPivotTableVersion12). _
        CreatePivotTable TableDestination:="Лист1!R3C1", TableName:= _
        "СводнаяТаблица1", DefaultVersion:=xlPivotTableVersion12
    Sheets("Лист1").Select
    Cells(3, 1).Select
    With ActiveSheet.PivotTables("СводнаяТаблица1").PivotFields("Код куратора")
        .Orientation = xlRowField
        .Position = 1
    End With
    With ActiveSheet.PivotTables("СводнаяТаблица1").PivotFields( _
        "Статус (Приняты ""+""/Приняты частично ""*""/Ошибки ""-"")")
        .Orientation = xlColumnField
        .Position = 1
    End With
    ActiveSheet.PivotTables("СводнаяТаблица1").AddDataField ActiveSheet.PivotTables _
        ("СводнаяТаблица1").PivotFields("Рег# № страхователя"), _
        "Количество по полю Рег# № страхователя", xlCount


    Range("A4:G9").Select
    Range("G9").Activate
    Selection.Borders(xlDiagonalDown).LineStyle = xlNone
    Selection.Borders(xlDiagonalUp).LineStyle = xlNone
    With Selection.Borders(xlEdgeLeft)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlEdgeTop)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlEdgeBottom)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlEdgeRight)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlInsideVertical)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    With Selection.Borders(xlInsideHorizontal)
        .LineStyle = xlContinuous
        .ColorIndex = 0
        .TintAndShade = 0
        .Weight = xlThin
    End With
    Sheets("Реестр_общий").Select
End Sub


*/









        //workBook.Close(false); //false - закрыть рабочую книгу не сохраняя изменения


        //Marshal.ReleaseComObject(workSheet);
        //Marshal.ReleaseComObject(workBook);
        //Marshal.ReleaseComObject(excelApp);
        //workSheet = null;
        //workBook = null;
        //excelApp = null;
        //GC.WaitForPendingFinalizers();
        //GC.Collect();
        //GC.WaitForPendingFinalizers();
        //GC.Collect();


        //"убиваем" процесс EXCEL
        //Process[] procList = Process.GetProcesses();
        //foreach (Process p in procList)
        //{
        //    if (p.ProcessName.ToString().Trim().ToUpper() == "EXCEL")
        //    {
        //        //завершаем процесс
        //        p.Kill();
        //    }
        //}


        ////Вычисляем сумму этих чисел
        //Excel.Range rng = workSheet.Range["A2"];
        //rng.Formula = "=SUM(A1:L1)";
        //rng.FormulaHidden = false;

        ////Выделяем границы у этой ячейки
        //Excel.Borders border = rng.Borders;
        //border.LineStyle = Excel.XlLineStyle.xlContinuous;

        ////Строим круговую диаграмму
        //Excel.ChartObjects chartObjs = (Excel.ChartObjects)workSheet.ChartObjects();
        //Excel.ChartObject chartObj = chartObjs.Add(5, 50, 300, 300);
        //Excel.Chart xlChart = chartObj.Chart;
        //Excel.Range rng2 = workSheet.Range["A1:L1"];

        ////Устанавливаем тип диаграммы
        //xlChart.ChartType = Excel.XlChartType.xlPie;

        ////устанавливаем источник данный (значения от 1 до 10)
        //xlChart.SetSourceData(rng2);


    }
}
