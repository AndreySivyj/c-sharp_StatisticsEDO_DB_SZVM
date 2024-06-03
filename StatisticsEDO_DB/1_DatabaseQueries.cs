using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsEDO_DB
{
    static class DatabaseQueries
    {

        //------------------------------------------------------------------------------------------
        public static string CreatequeryRKASV(SortedSet<string> sortedSet_INN)
        {
            //string queryRKASV = @"select a.insurer_reg_num, a.insurer_inn, a.insurer_kpp, a.insurer_short_name, a.insurer_last_name, a.insurer_first_name, a.insurer_middle_name " +
            //                    @"from(select * FROM asv_insurer) a " +
            //                    @"where  a.insurer_inn = " + inn +
            //                    @" order by a.insurer_reg_num";
            
            //две части текста запроса к РК АСВ             
            string part1 =
                @"select a.insurer_reg_num, a.insurer_inn, a.insurer_kpp, a.insurer_short_name, a.insurer_last_name, a.insurer_first_name, a.insurer_middle_name " +
                @"from(select * FROM asv_insurer) a " +
                @"where  a.insurer_inn in (";

            string part2 = @") order by a.insurer_reg_num";

            //Текст запроса
            string queryRKASV = part1;

            int tmpCount = sortedSet_INN.Count;

            foreach (var item in sortedSet_INN)
            {
                --tmpCount;
                if (tmpCount != 0)
                {
                    queryRKASV = queryRKASV + item + ", ";
                }
                else
                {
                    queryRKASV = queryRKASV + item;
                }
            }

            queryRKASV = queryRKASV + part2;
            
            return queryRKASV;
        }

        //------------------------------------------------------------------------------------------
        //Запрос по СЗВ-М, все принятые или не проверенные с датой и временем ввода        
        public static string CreateQueryPersoSZVM_ISX()
        {
            //две части текста запроса
            string part1 = @"select p.regnumb, w.strnum, w.god, w.period, s.DATE_INS, s.TIME_INS " +
            @"from pers.WORKS_M w, pers.SZV_M s, pers.STRAH p " +
            @"where w.cod_szv = s.cod_zap " +
            @"and s.cod_org = p.cod_zap " +
            @"and s.GOD=" + Program.otchYear + " " +
            @"and w.tip_form in ('ИСХОДНАЯ','ДОПОЛНЯЮЩАЯ') " +

            @"and w.period= " + Program.otchMonth + " " +
            //@"and d_priem between '" + Program.p_date_priem_st + @"' and '" + Program.p_date_priem_fn + @"' " +   //Дата "с"   "по"
            
            @"and w.stvio in (0, 1, 4) "+
            @"and s.STATUS_REC <> '' " +
            @"order by p.regnumb, w.strnum, w.god, w.period";

            //@"and p.regnumb in (";


            //string part2 = @") and w.stvio in (1, 4)" + //'Не проверен', 'Проверен, принят'
            //   @" order by p.regnumb, w.strnum, w.god, w.period";

            ////Текст запроса
            //string queryPersoSZVM_ISX = part1;

            //int tmpCount = Program.sortedSet_RegNom_For_Select.Count;

            //foreach (var item in Program.sortedSet_RegNom_For_Select)
            //{
            //    --tmpCount;
            //    if (tmpCount != 0)
            //    {
            //        queryPersoSZVM_ISX = queryPersoSZVM_ISX + item + ", ";
            //    }
            //    else
            //    {
            //        queryPersoSZVM_ISX = queryPersoSZVM_ISX + item;
            //    }
            //}

            //queryPersoSZVM_ISX = queryPersoSZVM_ISX + part2;

            //return queryPersoSZVM_ISX;

            return part1;
        }

        //------------------------------------------------------------------------------------------
        //Запрос по СЗВ-М, все принятые или не проверенные с датой и временем ввода        
        public static string CreateQueryPersoSZVM_OTMN()
        {
            //две части текста запроса
            string part1 = @"select p.regnumb, w.strnum, w.god, w.period, s.DATE_INS, s.TIME_INS " +
            @"from pers.WORKS_M w, pers.SZV_M s, pers.STRAH p " +
            @"where w.cod_szv = s.cod_zap " +
            @"and s.cod_org = p.cod_zap " +
            @"and s.GOD=" + Program.otchYear + " " +
            @"and w.tip_form in ('ОТМЕНЯЮЩАЯ') " +

            @"and w.period= " + Program.otchMonth + " " +
            //@"and d_priem between '" + Program.p_date_priem_st + @"' and '" + Program.p_date_priem_fn + @"' " +       //Дата "с"   "по"

            @"and w.stvio in (0, 1, 4) " +
            @"and s.STATUS_REC <> '' " +
            @"order by p.regnumb, w.strnum, w.god, w.period";




            ////две части текста запроса
            //string part1 = @"select distinct p.regnumb, w.strnum, w.god, w.period, s.DATE_INS, s.TIME_INS " +
            //@"from pers.WORKS_M w, pers.SZV_M s, pers.STRAH p " +
            //@"where w.cod_szv = s.cod_zap " +
            //@"and s.cod_org = p.cod_zap " +
            //@"and s.GOD=" + Program.otchYear + " " +
            //@"and w.tip_form in ('ОТМЕНЯЮЩАЯ') " +
            //@"and p.regnumb in (";


            //string part2 = @") and w.stvio in (1, 4)" + //'Не проверен', 'Проверен, принят'
            //   @" order by p.regnumb, w.strnum, w.god, w.period";

            ////Текст запроса
            //string queryPersoSZVM = part1;

            //int tmpCount = Program.sortedSet_RegNom_For_Select.Count;

            //foreach (var item in Program.sortedSet_RegNom_For_Select)
            //{
            //    --tmpCount;
            //    if (tmpCount != 0)
            //    {
            //        queryPersoSZVM = queryPersoSZVM + item + ", ";
            //    }
            //    else
            //    {
            //        queryPersoSZVM = queryPersoSZVM + item;
            //    }
            //}

            //queryPersoSZVM = queryPersoSZVM + part2;

            //return queryPersoSZVM;

            return part1;
        }

        //------------------------------------------------------------------------------------------
        //Запрос по СЗВ-М, все принятые или не проверенные с датой и временем ввода        
        public static string CreateQueryPersoReestrSZVM_ISX_1RegNum(string regNum)
        {
            string query = @"select distinct p.regnumb, w.strnum, s.DATE_INS, s.TIME_INS " +
            @"from pers.WORKS_M w, pers.SZV_M s, pers.STRAH p " +
            @"where w.cod_szv = s.cod_zap " +
            @"and s.cod_org = p.cod_zap " +
            @"and s.GOD=" + Program.otchYear + " " +
            @"and w.tip_form in ('ИСХОДНАЯ','ДОПОЛНЯЮЩАЯ') " +
            @"and p.regnumb=" + regNum + " " +
            @"and w.stvio in (0, 1, 4) " +
            @"and s.STATUS_REC <> '' " +
            @"order by p.regnumb, w.strnum";

            return query;
        }

        //------------------------------------------------------------------------------------------
        //Запрос по СЗВ-М, все принятые или не проверенные с датой и временем ввода        
        public static string CreateQueryPersoReestrSZVM_OTMN_1RegNum(string regNum)
        {
            string query = @"select distinct p.regnumb, w.strnum, s.DATE_INS, s.TIME_INS " +
            @"from pers.WORKS_M w, pers.SZV_M s, pers.STRAH p " +
            @"where w.cod_szv = s.cod_zap " +
            @"and s.cod_org = p.cod_zap " +
            @"and s.GOD=" + Program.otchYear + " " +
            @"and w.tip_form in ('ИСХОДНАЯ','ДОПОЛНЯЮЩАЯ') " +
            @"and p.regnumb=" + regNum + " " +
            @"and w.stvio in (0, 1, 4) " +
            @"and s.STATUS_REC <> '' " +
            @"order by p.regnumb, w.strnum";

            return query;
        }

    }
}
