using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsEDO_DB
{
    public class DataFromPersoDB
    {

        public string codZap;
        public string raion;
        public string regNum;
        public string nameStrah;
        public string month;
        public string year;
        public string typeSved;
        public string dataPredst;
        public string resultat;
        public string dataProverki;
        public string kolZL;
        public string kolZLgood;
        public string kolZLbad;
        public string kolZLBDPerso;
        public string statusKvitanc;
        public string spec;
        public string specChanged;
        public string curator;
        public string UP;
        public string kategory;
        public string inn;
        public string dataPostPFR;

        public string dataSnyatPFR; //new

        public string dataPostRO;
        public string dataSnyatRO;
        public string dopFormAvailability;
        public string dopFormKolZL;
        public string dopFormKolZLgood;
        public string dopFormKolZLbad;
        public string otmnFormAvailability;
        public string otmnFormKolZL;
        public string otmnFormKolZLgood;
        public string otmnFormKolZLbad;
        public string toutOldCountZL;
        public string toutNewCountZL;
        public string dataUvedomleniya;
        public string dataKontrolya;

        public string status_id;    //new
        public string kpp;    //new


        public DataFromPersoDB(string codZap = "", string raion = "", string regNum = "", string nameStrah = "", string month = "", string year = "", string typeSved = "",
                            string dataPredst = "", string resultat = "", string dataProverki = "", string kolZL = "", string kolZLgood = "", string kolZLbad = "",
                            string statusKvitanc = "", string spec = "", string specChanged = "", string dataUvedomleniya = "", string curator = "", string UP = "", string kategory = "",
                            string inn = "", string dataPostPFR = "", string dataPostRO = "", string dataSnyatRO = "", 
                            string dopFormAvailability = "", string dopFormKolZL = "", string dopFormKolZLgood = "", string dopFormKolZLbad = "",
                            string otmnFormAvailability = "", string otmnFormKolZL = "", string otmnFormKolZLgood = "", string otmnFormKolZLbad = "",
                            string toutOldCountZL = "0", string toutNewCountZL = "0", string kolZLBDPerso = "0", string status_id = "", string kpp = "")
        {
            this.codZap = codZap;
            this.raion = raion;
            this.regNum = regNum;
            this.nameStrah = nameStrah;
            this.month = month;
            this.year = year;
            this.typeSved = typeSved;
            this.dataPredst = dataPredst;
            this.resultat = resultat;
            this.dataProverki = dataProverki;
            this.kolZL = kolZL;
            this.kolZLgood = kolZLgood;
            this.kolZLbad = kolZLbad;
            this.kolZLBDPerso = kolZLBDPerso;
            this.statusKvitanc = statusKvitanc;
            this.spec = spec;
            this.specChanged = specChanged;
            this.curator = curator;
            this.UP = UP;
            this.kategory = kategory;
            this.inn = inn;
            this.dataPostPFR = dataPostPFR;
            this.dataPostRO = dataPostRO;
            this.dataSnyatRO = dataSnyatRO;
            this.dopFormAvailability = dopFormAvailability;
            this.dopFormKolZL = dopFormKolZL;
            this.dopFormKolZLgood = dopFormKolZLgood;
            this.dopFormKolZLbad = dopFormKolZLbad;
            this.otmnFormAvailability = otmnFormAvailability;
            this.otmnFormKolZL = otmnFormKolZL;
            this.otmnFormKolZLgood = otmnFormKolZLgood;
            this.otmnFormKolZLbad = otmnFormKolZLbad;
            this.toutOldCountZL = toutOldCountZL;
            this.toutNewCountZL = toutNewCountZL;
            this.dataUvedomleniya = dataUvedomleniya;
            this.status_id = status_id;
            this.kpp = kpp;

            if (this.dataUvedomleniya != "")
            {
                DateTime date = Convert.ToDateTime(dataUvedomleniya);

                if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    date = date.AddDays(4);
                    this.dataKontrolya = date.ToShortDateString();
                }
                else if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    date = date.AddDays(3);
                    this.dataKontrolya = date.ToShortDateString();
                }
                else if (date.DayOfWeek == DayOfWeek.Monday)
                {
                    date = date.AddDays(2);
                    this.dataKontrolya = date.ToShortDateString();
                }
                else if (date.DayOfWeek == DayOfWeek.Tuesday)
                {
                    date = date.AddDays(2);
                    this.dataKontrolya = date.ToShortDateString();
                }
                else if (date.DayOfWeek == DayOfWeek.Wednesday)
                {
                    date = date.AddDays(2);
                    this.dataKontrolya = date.ToShortDateString();
                }
                else if (date.DayOfWeek == DayOfWeek.Thursday)
                {
                    date = date.AddDays(4);
                    this.dataKontrolya = date.ToShortDateString();
                }
                else if (date.DayOfWeek == DayOfWeek.Friday)
                {
                    date = date.AddDays(4);
                    this.dataKontrolya = date.ToShortDateString();
                }
                else
                {
                    this.dataKontrolya = "";
                }
            }
            else
            {
                this.dataKontrolya = "";
            }
        }


        public string SverkaSummZL()
        {
            return (Convert.ToInt32(this.kolZLBDPerso) - Convert.ToInt32(this.toutNewCountZL)).ToString();
        }
            

        public override string ToString()
        {
            return codZap + ";" + raion + ";" + regNum + ";" + nameStrah + ";" + month + ";" + year + ";" + typeSved + ";" + dataPredst + ";" + resultat + ";" + dataProverki + ";"
                        + kolZL + ";" + kolZLgood + ";" + kolZLbad + ";" + statusKvitanc + ";" + spec + ";" + specChanged + ";"
                        + curator + ";" + UP + ";" + kategory + ";" + inn + ";" + dataPostPFR + ";" + dataPostRO + ";" + dataSnyatRO + ";"
                         + dopFormAvailability + ";" + otmnFormAvailability + ";" + toutOldCountZL + ";" + toutNewCountZL + ";" + dataUvedomleniya + ";" + dataKontrolya + ";";
        }
        public string ToStringReestUniqReg()
        {
            return codZap + ";" + raion + ";" + regNum + ";" + nameStrah + ";" + month + ";" + year + ";" + typeSved + ";" + dataPredst + ";" + resultat + ";" + dataProverki + ";"
                        + kolZLBDPerso + ";" + statusKvitanc + ";" + spec + ";" + specChanged + ";"
                        + curator + ";" + UP + ";" + kategory + ";" + inn + ";" + dataPostPFR + ";" + dataPostRO + ";" + dataSnyatRO + ";"
                         + dopFormAvailability + ";" + dopFormKolZL + ";" + dopFormKolZLgood + ";" + dopFormKolZLbad + ";"
                         + otmnFormAvailability + ";" + otmnFormKolZL + ";" + otmnFormKolZLgood + ";" + otmnFormKolZLbad + ";" + toutOldCountZL + ";" + toutNewCountZL + ";"+ SverkaSummZL() + ";"
                         + status_id + ";";
        }

        //public string ToStringReestUniqReg()
        //{
        //    return codZap + ";" + raion + ";" + regNum + ";" + nameStrah + ";" + month + ";" + year + ";" + typeSved + ";" + dataPredst + ";" + resultat + ";" + dataProverki + ";"
        //                + kolZL + ";" + kolZLgood + ";" + kolZLbad + ";" + statusKvitanc + ";" + spec + ";" + specChanged + ";"
        //                + curator + ";" + UP + ";" + kategory + ";" + inn + ";" + dataPostPFR + ";" + dataPostRO + ";" + dataSnyatRO + ";"
        //                 + dopFormAvailability + ";" + dopFormKolZL + ";" + dopFormKolZLgood + ";" + dopFormKolZLbad + ";"
        //                 + otmnFormAvailability + ";" + otmnFormKolZL + ";" + otmnFormKolZLgood + ";" + otmnFormKolZLbad + ";" + toutOldCountZL + ";" + toutNewCountZL + ";";
        //}

        public string ToStringPersoALL()
        {
            return codZap + ";" + raion + ";" + regNum + ";" + nameStrah + ";" + month + ";" + year + ";" + typeSved + ";" + dataPredst + ";" + resultat + ";" + dataProverki + ";"
                        + kolZL + ";" + kolZLgood + ";" + kolZLbad + ";" + statusKvitanc + ";" + spec + ";" + specChanged + ";"
                        + curator + ";" + UP + ";" + kategory + ";" + inn + ";" + dataPostPFR + ";" + dataPostRO + ";" + dataSnyatRO + ";" + dataUvedomleniya + ";" + dataKontrolya + ";";
        }
        public string ToStringErrror()
        {
            return codZap + ";" + raion + ";" + regNum + ";" + nameStrah + ";" + month + ";" + year + ";" + typeSved + ";" + dataPredst + ";" + kategory + ";" 
                    + dataPostPFR + ";" + dataPostRO + ";" + dataSnyatRO + ";" + resultat + ";" + dataProverki + ";" + kolZL + ";" + kolZLgood + ";" + kolZLbad + ";" 
                    + statusKvitanc + ";" + spec + ";" + specChanged + ";" + curator + ";" + UP + ";" + dataUvedomleniya + ";" + dataKontrolya + ";";
        }
    }
}
