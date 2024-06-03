using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsEDO_DB
{
    public class DataFromRKASVDB    //DataFromDB
    {
        /*
oo_package_idfile           //наименование
oo_extract_document_id      //номер документа
3 - tip                           //тип
oo_package_receive          //дата приема
dat                         //дата обработки
insurer_reg_num             //рег номер
insurer_comp_ogrn           //огрн
category_code               //категория
insurer_reg_start_date      //дата постановки РО
insurer_reg_finish_date     //дата снятия РО
11   - count                       //разница
         */

        public string raion;
        public string insurer_reg_num;
        public string insurer_reg_start_date;
        public string insurer_reg_finish_date;
        public string insurer_short_name;
        public string insurer_last_name;
        public string insurer_first_name;
        public string insurer_middle_name;
        public string INSURER_REG_DATE_RO;
        public string INSURER_UNREG_DATE_RO;
        public string category_code;
        public string insurer_inn;
        public string reg_start_code;
        public string reg_finish_code;
        public string kurator;
        public string status_id;
        public string insurer_kpp;

        public DataFromRKASVDB(string insurer_reg_num = "", string insurer_reg_start_date = "", string insurer_reg_finish_date = "", string insurer_short_name = "",
                                string insurer_last_name = "", string insurer_first_name = "", string insurer_middle_name = "",
                                string INSURER_REG_DATE_RO = "", string INSURER_UNREG_DATE_RO = "", string category_code = "", string insurer_inn = "", string insurer_kpp = "",
                                string raion = "", string reg_start_code = "", string reg_finish_code = "", string status_id = "", string kurator = "")
        {
            this.raion = raion;
            this.insurer_reg_num = insurer_reg_num;
            this.insurer_reg_start_date = insurer_reg_start_date;
            this.insurer_reg_finish_date = insurer_reg_finish_date;
            this.insurer_short_name = insurer_short_name;
            this.insurer_last_name = insurer_last_name;
            this.insurer_first_name = insurer_first_name;
            this.insurer_middle_name = insurer_middle_name;
            this.INSURER_REG_DATE_RO = INSURER_REG_DATE_RO;
            this.INSURER_UNREG_DATE_RO = INSURER_UNREG_DATE_RO;
            this.category_code = category_code;
            this.insurer_inn = insurer_inn;
            this.insurer_kpp = insurer_kpp;
            this.reg_start_code = reg_start_code;
            this.reg_finish_code = reg_finish_code;
            this.status_id = status_id;
            this.kurator = kurator;

            if (insurer_last_name != "" || insurer_first_name != "" || insurer_middle_name != "")
            {
                this.insurer_short_name = insurer_last_name + " " + insurer_first_name + " " + insurer_middle_name;
            }
        }

        public override string ToString()
        {
            //if (insurer_last_name != "" || insurer_first_name != "" || insurer_middle_name != "")
            //{
            //    return raion + ";" + insurer_reg_num + ";"
            //        + insurer_last_name + " " + insurer_first_name + " " + insurer_middle_name + ";"
            //        + insurer_reg_start_date + ";" + insurer_reg_finish_date + ";"
            //        + INSURER_REG_DATE_RO + ";" + INSURER_UNREG_DATE_RO + ";" + category_code + ";" + insurer_inn + ";"
            //        + reg_start_code + ";" + reg_finish_code + ";" + kurator + ";";
            //}
            //else
            //{
            return raion + ";" + insurer_reg_num + ";"
                + insurer_short_name + ";"
                + insurer_reg_start_date + ";" + insurer_reg_finish_date + ";"
                + INSURER_REG_DATE_RO + ";" + INSURER_UNREG_DATE_RO + ";" + category_code + ";" + insurer_inn + ";" + insurer_kpp + ";"
                + reg_start_code + ";" + reg_finish_code + ";" + status_id + ";" + kurator + ";";
            //}
        }
    }

}
