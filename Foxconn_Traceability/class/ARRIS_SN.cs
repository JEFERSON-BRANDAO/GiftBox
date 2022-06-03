using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Classes;
using System.Globalization;
using System.Windows.Forms;
using System.Data;

namespace Foxconn_Traceability
{

    public class ARRIS_SN
    {
        public List<String> get_Arris_SN(int qty, string modelo)
        {

            List<String> list = new List<string>();
            String serial = "";
            int count = 0;
            int count1 = 0;
            int temp = 0;

            OleDbConnect Objconn = new OleDbConnect();
            //
            //try
            //{
            try
            {
                try
                {
                    Objconn.Conectar();
                    Objconn.Parametros.Clear();
                    //
                    string sql = @"SELECT DATA5 AS SERIAL FROM R_AP_TEMP
                                   WHERE DATA1='ARRIS_SN' AND DATA2='" + modelo + "'";
                    Objconn.SetarSQL(sql);
                    Objconn.Executar();
                }
                finally
                {
                    Objconn.Desconectar();
                }
                //
                if (Objconn.Tabela.Rows.Count > 0)
                {
                    temp = 1;
                    serial = Objconn.Tabela.Rows[0]["SERIAL"].ToString();
                    //
                    if (modelo.Equals("TG1692BR"))
                    {
                        string UltimoDigAnoSN_R_AP_TEMP = serial;
                        //
                        if (serial.Substring(0, 10) == pref_ARRIS_SN(modelo) & serial.Length == 15)
                        {
                            count = Int32.Parse(serial.Substring(10)) + 1;

                            try
                            {
                                //Verificar na mfworkstatus o max e comparar
                                //                                sql = @"SELECT MAX(SYSSERIALNO) AS SERIAL FROM MFWORKSTATUS
                                //                                   WHERE SYSSERIALNO LIKE '" + pref_ARRIS_SN(modelo) + "%'";

                                string sql = @"SELECT MAX(SYSSERIALNO) AS SERIAL FROM MFWORKSTATUS
                                   WHERE WORKORDERNO IN (SELECT WORKORDERNO FROM MFWORKORDER WHERE SKUNO='ARCT05716')";
                                Objconn.SetarSQL(sql);
                                Objconn.Executar();
                            }
                            finally
                            {
                                Objconn.Desconectar();
                            }

                            if (Objconn.Tabela.Rows.Count > 0)
                            {
                                serial = Objconn.Tabela.Rows[0]["SERIAL"].ToString();
                                //
                                if (serial.Length == 15)
                                {
                                    //extrai último dígito do ano, do serial mais recente da tabela MFWORKSTATUS
                                    string UltimoDigAnoSN_MFWORKSTATUS = serial;
                                    UltimoDigAnoSN_MFWORKSTATUS = string.IsNullOrEmpty(UltimoDigAnoSN_MFWORKSTATUS) ? string.Empty : UltimoDigAnoSN_MFWORKSTATUS.Remove(1);
                                    //extrai último dígito do ano do ano atual
                                    string UltimoDigAno_anoAtual = DateTime.Now.Year.ToString().Remove(0, 3);
                                    //extrai último dígito do ano do  serial mais recente da tabela R_AP_TEMP
                                    UltimoDigAnoSN_R_AP_TEMP = string.IsNullOrEmpty(UltimoDigAnoSN_R_AP_TEMP) ? string.Empty : UltimoDigAnoSN_R_AP_TEMP.Remove(1);

                                    //Pega valor do maior serial da tabela MFWORKSTATUS
                                    if ((int.Parse(UltimoDigAno_anoAtual) == int.Parse(UltimoDigAnoSN_MFWORKSTATUS)) && (int.Parse(UltimoDigAno_anoAtual) == int.Parse(UltimoDigAnoSN_R_AP_TEMP)))
                                    {
                                        count1 = Int32.Parse(serial.Substring(10)) + 1;
                                        //
                                        if (count < count1)
                                        {
                                            count = count1;
                                        }
                                    }

                                }
                            }

                        }
                        else
                        {
                            count = 1;
                        }
                    }
                    else if (modelo.Equals("TG1692BR-SMT"))
                    {
                        try
                        {
                            //Verificar na mfworkstatus o max e comparar
                            Objconn.Conectar();
                            Objconn.Parametros.Clear();
                            string sql = @"SELECT MAX(SYSSERIALNO) AS SERIAL FROM MFWORKSTATUS
                                   WHERE SYSSERIALNO LIKE '" + pref_ARRIS_SN(modelo) + "%'";
                            Objconn.SetarSQL(sql);
                            Objconn.Executar();
                        }
                        finally
                        {
                            Objconn.Desconectar();
                        }

                        if (Objconn.Tabela.Rows.Count > 0)
                        {
                            if (serial.Length == 15)
                            {
                                //último serial na tabela R_AP_TEMP
                                string sn_temp = serial;

                                //último serial na tabela MFWORKSTATUS
                                serial = Objconn.Tabela.Rows[0]["SERIAL"].ToString();
                                int ret = sn_temp.CompareTo(serial);

                                //if (ret == 0)
                                //{
                                //    //será 0 se as duas strings form iguais
                                //}
                                //if (ret == -1)
                                //{
                                //    //será -1 se a string sn_temp for menor que a string serial
                                //}
                                if (ret == 1)
                                {
                                    //será 1 se a string sn_temp for maior que a string serial
                                    serial = sn_temp;
                                }
                            }
                        }
                    }
                    else if (modelo.Equals("TG1692A"))
                    {
                        //extrai somente os 4 digitos numero serial na tabela R_AP_TEMP
                        string sn_temp = serial;
                        sn_temp = sn_temp.Remove(0, 6).Remove(4, 2);
                        count1 = int.Parse(sn_temp) + 1;

                        //extrai dias do ano do serial na tabela R_AP_TEMP
                        string diasAno_temp = serial;
                        diasAno_temp = diasAno_temp.Remove(0, 3).Remove(3);

                        try
                        {
                            //Verificar na mfworkstatus o max e comparar
                            Objconn.Conectar();
                            Objconn.Parametros.Clear();
                            string sql = @"SELECT MAX(SYSSERIALNO) AS SERIAL FROM MFWORKSTATUS
                                                WHERE SYSSERIALNO LIKE '51%' ";
                            //                                   WHERE SYSSERIALNO LIKE '" + pref_ARRIS_SN(modelo) + "%'";
                            Objconn.SetarSQL(sql);
                            Objconn.Executar();
                        }
                        finally
                        {
                            Objconn.Desconectar();
                        }

                        //
                        if (Objconn.Tabela.Rows.Count > 0)
                        {
                            if (serial.Length == 12)
                            {
                                //último serial na tabela MFWORKSTATUS
                                serial = Objconn.Tabela.Rows[0]["SERIAL"].ToString();
                                //string sn_WorkStatus = string.Empty;
                                //sn_WorkStatus = serial;

                                //dias do ano  atual
                                string diaSeguinte = DateTime.Now.DayOfYear.ToString();//Digit 4,5,6:
                                //
                                if (diaSeguinte.Length == 1)//1 to 366
                                    diaSeguinte = "00" + diaSeguinte;
                                else if (diaSeguinte.Length == 2)
                                    diaSeguinte = "0" + diaSeguinte;
                                //
                                if (serial.Length == 12)
                                {
                                    //novo dia Inicia contagem
                                    if (int.Parse(diaSeguinte) > int.Parse(diasAno_temp))
                                    {
                                        count = 1;
                                    }
                                    else
                                    {
                                        count = count1;
                                    }
                                }
                            }
                        }

                    }
                    else if (modelo.Equals("TG1692A-SMT"))
                    {
                        try
                        {
                            //Verificar na mfworkstatus o max e comparar
                            Objconn.Conectar();
                            Objconn.Parametros.Clear();
                            string sql = @"SELECT MAX(SYSSERIALNO) AS SERIAL FROM MFWORKSTATUS
                                        WHERE SYSSERIALNO LIKE '%ZAGBBA%'";
                            //                                   WHERE SYSSERIALNO LIKE '" + pref_ARRIS_SN(modelo) + "%'";
                            Objconn.SetarSQL(sql);
                            Objconn.Executar();
                        }
                        finally
                        {
                            Objconn.Desconectar();
                        }

                        if (Objconn.Tabela.Rows.Count > 0)
                        {
                            if (serial.Length == 15)
                            {
                                //último serial na tabela R_AP_TEMP
                                string sn_temp = serial;

                                //último serial na tabela MFWORKSTATUS
                                serial = Objconn.Tabela.Rows[0]["SERIAL"].ToString();
                                int ret = sn_temp.CompareTo(serial);

                                //if (ret == 0)
                                //{
                                //    //será 0 se as duas strings form iguais
                                //}
                                //if (ret == -1)
                                //{
                                //    //será -1 se a string sn_temp for menor que a string serial
                                //}
                                if (ret == 1)
                                {
                                    //será 1 se a string sn_temp for maior que a string serial
                                    serial = sn_temp;
                                }
                            }
                        }
                    }
                    else if (modelo.Equals("E965"))
                    {
                        //extrai somente os 7 digitos números serial na tabela R_AP_TEMP
                        string sn_temp = serial;
                        sn_temp = sn_temp.Remove(0, 9);
                        count = int.Parse(sn_temp) + 1;

                        try
                        {
                            //Verificar na mfworkstatus o max e comparar
                            Objconn.Conectar();
                            Objconn.Parametros.Clear();
                            string sql = @"SELECT MAX(SYSSERIALNO) AS SERIAL FROM MFWORKSTATUS
                                   WHERE SYSSERIALNO LIKE '" + pref_ARRIS_SN(modelo) + "%'";
                            Objconn.SetarSQL(sql);
                            Objconn.Executar();
                        }
                        finally
                        {
                            Objconn.Desconectar();
                        }

                        //
                        if (Objconn.Tabela.Rows.Count > 0)
                        {
                            if (serial.Length == 16)
                            {
                                //último serial na tabela MFWORKSTATUS
                                serial = Objconn.Tabela.Rows[0]["SERIAL"].ToString();
                                serial = serial.Remove(serial.Length - 1);//remove B
                                //string sn_WorkStatus = string.Empty;
                                //sn_WorkStatus = serial;
                            }
                        }
                    }
                    else if (modelo.Equals("DSI"))
                    {
                        string sn_temp = serial;
                        sn_temp = sn_temp.Remove(0, 13);//extrai somente os 5 últimos digitos [SERIAL]                          
                        //
                        Configuracao etiqueta = new Configuracao();
                        string data_ultima_etiqueta = etiqueta.Data_Gerada(serial);//pega data da última etiqueta gerada
                        string data_Atual = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        //
                        if (DateTime.Parse(data_ultima_etiqueta) == DateTime.Parse(data_Atual))
                        {
                            count = int.Parse(sn_temp) + 1;
                        }
                        else//próximo dia
                        {
                            count = 1;//sn_temp = "00001" inicia nova contagem
                        }

                        try
                        {
                            //Verificar na mfworkstatus o max e comparar
                            Objconn.Conectar();
                            Objconn.Parametros.Clear();
                            string sql = @"SELECT MAX(SYSSERIALNO) AS SERIAL FROM MFWORKSTATUS
                                   WHERE SYSSERIALNO LIKE '" + pref_ARRIS_SN(modelo) + "%'";
                            Objconn.SetarSQL(sql);
                            Objconn.Executar();
                        }
                        finally
                        {
                            Objconn.Desconectar();
                        }

                        //
                        if (Objconn.Tabela.Rows.Count > 0)
                        {
                            if (serial.Length >= 18)
                            {
                                //último serial na tabela MFWORKSTATUS
                                serial = Objconn.Tabela.Rows[0]["SERIAL"].ToString();
                                if (serial.Length == 19)
                                    serial = serial.Remove(serial.Length - 1);//remove B

                            }
                        }
                    }

                }
                else
                {
                    count = 1;
                }
                //
                for (int i = 0; i < qty; i++)
                {
                    if (modelo.Equals("TG1692A-SMT"))
                    {
                        string valorInicial = serial;
                        if (!string.IsNullOrEmpty(valorInicial))
                        {
                            valorInicial = valorInicial.Remove(0, 10);//somente os 5 digitos sequenciais do serial
                            serial = generate_Arris_SN(0, modelo, valorInicial);
                            //if (!serial.Contains("ERRO"))
                            //    serial = serial.Remove(serial.Length - 4);//remove 4 ultimos numeros [numero da WO]
                        }
                        else
                        {
                            serial = "ERRO Não existe Serial deste modelo na tabela R_AP_TEMP";
                        }

                    }
                    else if (modelo.Equals("TG1692BR-SMT"))
                    {
                        string valorInicial = serial;
                        if (!string.IsNullOrEmpty(valorInicial))
                        {
                            valorInicial = valorInicial.Remove(0, 10);//somente os 5 digitos sequenciais do serial
                            serial = generate_Arris_SN(0, modelo, valorInicial);
                        }
                        else
                        {
                            serial = "ERRO Não existe Serial deste modelo na tabela R_AP_TEMP";
                        }
                    }
                    else
                    {
                        serial = generate_Arris_SN(count++, modelo, "");
                    }
                    //
                    if (!serial.Contains("ERRO"))
                    {
                        if (!string.IsNullOrEmpty(serial))
                        {
                            list.Add(serial);

                        }
                    }
                }
                //
                if (list.Count > 0)
                {
                    string sql = string.Empty;
                    if (temp == 1)
                    {
                        sql = @"UPDATE  R_AP_TEMP SET DATA5 = '" + serial + "', WORK_TIME = SYSDATE WHERE DATA2='" + modelo + "'";
                    }
                    else
                    {
                        sql = @"INSERT INTO  R_AP_TEMP (DATA1, DATA2, DATA3, DATA4, DATA5, DATA6, DATA7, WORK_TIME)" +
                        "VALUES ('ARRIS_SN', '" + modelo + "', 1, 'SERIAL', '" + serial + "', 'S/N', 0, SYSDATE)";
                    }
                    //
                    try
                    {
                        Objconn.Conectar();
                        Objconn.Parametros.Clear();
                        Objconn.SetarSQL(sql);
                        Objconn.Executar();
                    }
                    finally
                    {
                        Objconn.Desconectar();
                    }
                    //
                    if (!Objconn.Isvalid)
                        list.Clear();//limpa lista se houver problema na conexão com banco

                }

            }
            catch
            {
                //
            }

            //}
            //finally
            //{
            //    Objconn.Desconectar();
            //}

            if (list.Count == 0)
            {
                //para emitir som de alerta
                Som objSom = new Som();
                objSom.Falha();
                //                   
                MessageBox.Show(serial, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            return list;
        }

        public String get_lastSN(string modelo)
        {
            #region ÚLTIMO SERIAL REGISTRADO NA TABELA R_AP_TEMP

            OleDbConnect Objconn = new OleDbConnect();
            //
            try
            {
                try
                {
                    Objconn.Conectar();
                    Objconn.Parametros.Clear();
                    //
                    string sql = @"SELECT DATA5 AS SERIAL FROM R_AP_TEMP
                            WHERE DATA1= 'ARRIS_SN' AND DATA2 ='" + modelo + "'";
                    Objconn.SetarSQL(sql);
                    Objconn.Executar();

                }
                catch (Exception erro)
                {
                    return "ERRO: " + erro;
                }
            }
            finally
            {
                Objconn.Desconectar();
            }

            //
            if (Objconn.Tabela.Rows.Count > 0)
            {
                return Objconn.Tabela.Rows[0]["SERIAL"].ToString().Trim();
            }
            else
            {
                //Nenhum registro encontrado ou erro de conexão
                return Objconn.Isvalid ? "" : "ERRO: " + Objconn.Message;
            }

            #endregion
        }

        public string generate_Arris_SN(int seq, string modelo, string valorInicial)
        {
            //valorInicial = "0ZZ9Z";
            string arrissn;
            //
            if (modelo.Equals("TG1692A"))
            {
                #region TG1692A

                string ultimoDigAno = DateTime.Now.Year.ToString().Remove(0, 3);// Digit 3                 
                string DiasAno = DateTime.Now.DayOfYear.ToString();//Digit 4,5,6:
                string serialNumber = seq.ToString().PadLeft(4, '0');//0000
                //
                if (DiasAno.Length == 1)//1 to 366
                    DiasAno = "00" + DiasAno;
                else if (DiasAno.Length == 2)
                    DiasAno = "0" + DiasAno;

                //
                int Date0 = int.Parse(DiasAno);
                int Date1 = int.Parse(ultimoDigAno);

                //fórmula para digitos 11 e 12
                Int64 formula0 = 51 * 100000000L + Date1 * 10000000 + Date0 * 10000 + seq;//51*100000000+Date1*10000000+Date0 *10000 +Counter0 
                Int64 formula1 = ((formula0 * 100 + (formula0 % 100) + (formula0 / 100 % 17)) % 100);//Formula0 *100+MOD(INT((MOD(Formula0,100))+(MOD(Formula0/100,17))),100) 
                //               
                string digito_11_12 = formula1.ToString().Length == 1 ? "0" + formula1.ToString() : formula1.ToString();
                arrissn = formula0.ToString() + digito_11_12;

                #endregion
            }
            else if (modelo.Equals("TG1692A-SMT"))
            {
                #region TG1692A-SMT

                #region NUMERO DA SEMANA DO ANO

                string data = DateTime.Now.Date.ToString("yyyy-MM-dd");
                DateTime inputDate = DateTime.Parse(data.Trim());
                var dt = inputDate;
                CultureInfo cul = CultureInfo.CurrentCulture;
                //calcula número da semana
                int NumeroSemana = cul.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                #endregion

                //               
                String caracteresValidos = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";//34 caractes
                //Wo workorder = new Wo();
                string sufixo = string.Empty;
                //
                int s1 = 0;
                int s2 = 0;
                int s3 = 0;
                int s4 = 0;
                int s5 = 0;

                //pega  posição de cada valor dos caracteres
                for (int index = 0; index < caracteresValidos.Length; index++)
                {
                    if (valorInicial[0].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s1 = index;//posição do carater 1
                    }

                    if (valorInicial[1].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s2 = index;//posição do carater 2
                    }

                    if (valorInicial[2].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s3 = index;//posição do carater 3
                    }

                    if (valorInicial[3].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s4 = index;//posição do carater 4
                    }

                    if (valorInicial[4].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s5 = index;//posição do carater 5
                    }

                }

                //
                string BAYY = "BA" + DateTime.Now.Year.ToString().Remove(0, 2); //[plant,"B"=Brazil]; [A=SMT]; [YY=year 2016=16,2017=17, 2018=18];
                string WK = NumeroSemana.ToString().Length == 1 ? "0" + NumeroSemana.ToString() : NumeroSemana.ToString();//WK=week,first week=01,second week=02
                string SSSSS = convertToAlphaSequencia(s1, s2, s3, s4, s5); ;//Sequential numbers assigned by factory                
                //string ZZZZ = string.Empty;// workorder.Numero(ordem, "IMPRIMIR"); //Last 4 Digits of work order

                ////
                //if (ZZZZ.Contains("ERRO"))
                //{
                //    arrissn = ZZZZ;
                //}
                //else
                //{
                //    //FORMATO:  ZAGB BAYY WK  SSSSS   ZZZZ
                //    ZZZZ = ZZZZ.Remove(0, 8);
                //    sufixo = BAYY + WK + SSSSS + ZZZZ;

                //    //
                //    arrissn = pref_ARRIS_SN(modelo) + sufixo;//adiciona prefixo
                //}

                sufixo = BAYY + WK + SSSSS;
                arrissn = pref_ARRIS_SN(modelo) + sufixo;//adiciona prefixo               

                return arrissn;

                #endregion
            }
            else if (modelo.Equals("TG1692BR"))
            {
                #region TG1692BR

                arrissn = pref_ARRIS_SN(modelo) + seq.ToString().PadLeft(5, '0');//adiciona prefixo

                #endregion
            }
            else if (modelo.Equals("TG1692BR-SMT"))
            {
                #region TG1692BR-SMT

                #region NUMERO DA SEMANA DO ANO

                string data = DateTime.Now.Date.ToString("yyyy-MM-dd");
                DateTime inputDate = DateTime.Parse(data.Trim());
                var dt = inputDate;
                CultureInfo cul = CultureInfo.CurrentCulture;
                //calcula número da semana
                int NumeroSemana = cul.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                #endregion

                //               
                String caracteresValidos = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";//34 caracteres
                //Wo workorder = new Wo();
                string sufixo = string.Empty;
                //
                int s1 = 0;
                int s2 = 0;
                int s3 = 0;
                int s4 = 0;
                int s5 = 0;

                //pega  posição de cada valor dos caracteres
                for (int index = 0; index < caracteresValidos.Length; index++)
                {
                    if (valorInicial[0].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s1 = index;//posição do carater 1
                    }

                    if (valorInicial[1].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s2 = index;//posição do carater 2
                    }

                    if (valorInicial[2].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s3 = index;//posição do carater 3
                    }

                    if (valorInicial[3].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s4 = index;//posição do carater 4
                    }

                    if (valorInicial[4].ToString().Equals(caracteresValidos[index].ToString()))
                    {
                        s5 = index;//posição do carater 5
                    }

                }

                //
                string BAYY = "BA" + DateTime.Now.Year.ToString().Remove(0, 2); //[plant,"B"=Brazil]; [A=SMT]; [YY=year 2016=16,2017=17, 2018=18];
                string WK = NumeroSemana.ToString().Length == 1 ? "0" + NumeroSemana.ToString() : NumeroSemana.ToString();//WK=week,first week=01,second week=02
                string SSSSS = convertToAlphaSequencia(s1, s2, s3, s4, s5); //Sequential numbers assigned by factory                
                //string ZZZZ = workorder.Numero(ordem); //Last 4 Digits of work order

                //
                //if (ZZZZ.Contains("ERRO"))
                //{
                //    arrissn = ZZZZ;
                //}
                //else
                //{
                //    //FORMATO:  ZAGB BAYY WK  SSSSS   ZZZZ
                //    ZZZZ = ZZZZ.Remove(0, 8);
                //    sufixo = BAYY + WK + SSSSS + ZZZZ;

                //    //
                //    arrissn = pref_ARRIS_SN(modelo) + sufixo;//adiciona prefixo
                //}


                //FORMATO:  ZABR BAYY WK  SSSSS
                sufixo = BAYY + WK + SSSSS;
                arrissn = pref_ARRIS_SN(modelo) + sufixo;//adiciona prefixo

                /*ZABR BAYY WK  SSSSS   ZZZZ*/
                //ZABR  = Model NO.ZAGB=TG1692 A/NT
                //B     = planta"B"=Brazil
                //A     = A=SMT
                //YY    = Dois últimos digitos do ano
                //WK    = Número da Semana
                //SSSSS = 5 dígitos sequenciais
                //ZZZZ  = Últimos 4 dígitos da ordem de produção

                return arrissn;


                #endregion
            }
            else if (modelo.Equals("E965"))
            {
                #region E965

                //formato STXXXXYLLZZZZZZZ

                //X: ARRIS Manufacturing Site Code – Use X for Foxconn Manaus, Brazil
                //T: Board Type (use M for Mainboard)
                //XXXX: First 4 digits of the product top level part number (i.e. E965)
                //Y: Last digit of year i.e. Use 8 for 2018, 9 for 2019, etc.
                //LL: Mainboard modification level. Decimal value between 01 and 99. Initial value is 01 and this should be incremented as changes are applied to the mainboard.
                //ZZZZZZZ: Unique serial number of mainboard, starting at 0000001 and incrementing in decimal. This number is NOT reset for a new year or new build level.

                string Y = DateTime.Now.Year.ToString().Remove(0, 3);//último digito do ano
                string LL = "01";
                string ZZZZZZZ = seq.ToString();

                //
                if (ZZZZZZZ.Length == 1)
                    ZZZZZZZ = "000000" + seq.ToString();
                else if (ZZZZZZZ.Length == 2)
                    ZZZZZZZ = "00000" + seq.ToString();
                else if (ZZZZZZZ.Length == 3)
                    ZZZZZZZ = "0000" + seq.ToString();
                else if (ZZZZZZZ.Length == 4)
                    ZZZZZZZ = "000" + seq.ToString();
                else if (ZZZZZZZ.Length == 5)
                    ZZZZZZZ = "00" + seq.ToString();
                else if (ZZZZZZZ.Length == 6)
                    ZZZZZZZ = "0" + seq.ToString();
                //
                arrissn = Y + LL + ZZZZZZZ;
                arrissn = pref_ARRIS_SN(modelo) + arrissn;//adiciona prefixo

                #endregion
            }
            else if (modelo.Equals("DSI"))
            {
                #region DSI

                /*formato MMMTPPHHYWWDLNNNN*/
                const string HH = "01";  //HH: Hardware ID, 00 = FGR, 01 = Pilot, and 02 = IP/MPS
                string Y = DateTime.Now.Year.ToString().Remove(0, 3);//production year,  último digito do ano               
                //
                string data = DateTime.Now.Date.ToString("yyyy-MM-dd");
                DateTime inputDate = DateTime.Parse(data.Trim());
                var dt = inputDate;
                CultureInfo cul = CultureInfo.CurrentCulture;
                //calcula número da semana
                int NumeroSemana = cul.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                string WW = NumeroSemana.ToString().Length == 1 ? "0" + NumeroSemana.ToString() : NumeroSemana.ToString();//production week,   WW=week,first week=01,second week=02

                //
                int D = (int)DateTime.Now.DayOfWeek + 1;//production day   Add 1 para domingo iniciar 1,   pois 0 = Domingo
                const string L = "1";//production line
                string NNNN = seq.ToString();//counter. Reset to 00001 everyday 

                if (NNNN.Length == 1)
                    NNNN = "0000" + seq.ToString();
                else if (NNNN.Length == 2)
                    NNNN = "000" + seq.ToString();
                else if (NNNN.Length == 3)
                    NNNN = "00" + seq.ToString();
                else if (NNNN.Length == 4)
                    NNNN = "0" + seq.ToString();

                //montando etiqueta
                arrissn = HH + Y + WW + D + L + NNNN;
                arrissn = pref_ARRIS_SN(modelo) + arrissn;//adiciona prefixo               

                #endregion
            }
            else
            {
                arrissn = "";
            }
            //
            return arrissn;
        }

        public String pref_ARRIS_SN(string modelo)
        {
            String prefix_arrissn = string.Empty;
            DateTime Now = DateTime.Now;
            //
            if (modelo.Equals("TG1692BR"))
            {
                #region TG1692BR

                #region dígito 1
                // último dígito do ano
                prefix_arrissn += Now.Year.ToString().Substring(3);

                #endregion
                //
                #region dígito 2

                /*
                  1=janeiro
                  2=fevereiro
                  3=março
                  4=abril
                  5=maio
                  6=junho
                  7=julho
                  8=agosto
                  9=setembro
                  A=outubro
                  B-novembro
                  C-dezembro
                 
                 */

                int mes = Now.Month;

                if (mes == 10)//Outubro
                {
                    prefix_arrissn += "A";
                }
                else if (mes == 11)//novembro
                {
                    prefix_arrissn += "B";
                }
                else if (mes == 12)//dezembro
                {
                    prefix_arrissn += "C";
                }
                else//demais meses
                {
                    prefix_arrissn += mes.ToString();
                }

                #endregion
                //
                #region dígito 3

                //dias do mês
                /*
                 1=1
                 2=2
                 3=3
                 4=4
                 5=5
                 6=6
                 7=7
                 8=8
                 9=9
                 10=A
                 11=B
                 12=C
                 13=D
                 14=E
                 15=F
                 16=G
                 17=H
                 18=J
                 19=K
                 20=L
                 21=M
                 22=N
                 23=P
                 24=R
                 25=S
                 26=T
                 27=U
                 28=V
                 29=W
                 30=X
                 31=Y                 
                 
                 */
                int dia = Now.Day;
                prefix_arrissn += convertToAlpha(dia);

                #endregion
                //
                #region dígito 4

                /* B before Aug 2016 
                   2 (Aug 2016 through 2025)
                */

                if (Now.Year < 2025)
                {
                    prefix_arrissn += "2";
                }

                #endregion
                //
                #region  dígito 5-10

                /*
                  digito 5  factory digital          B(FOXCONN BRAZIL)
                  digito 6  product type             2(TG192A/BR)
                  digito 7  PWB revision             5(for ARCT04363 Rev F)
                  digito 8  PWB Assembly revision    7(for ARCT05716 Rev B for TG192A/BR EB-15076)
                  digito 9  FW load lineup revision  6(TS 9.1.135_R010/TS)
                  digito 10 product release          5(for ARCT05716 ASSY MB Rev A, and B for TG1692A/BR)
                 
                 */
                prefix_arrissn += "B25765";

                #endregion

                #endregion
            }
            //else if (modelo.Equals("TG1692A"))
            //{
            //    #region TG1692A

            //    prefix_arrissn = "51";//Digit 1,2

            //    #endregion
            //}
            else if (modelo.Equals("TG1692A-SMT"))
            {
                #region TG1692A-SMT

                prefix_arrissn = "ZAGB";//Model NO. ZAGB=TG1692

                #endregion
            }
            else if (modelo.Equals("TG1692BR-SMT"))
            {
                #region TG1692BR-SMT

                prefix_arrissn = "ZABR";//Model NO. ZABR=TG1692BR

                #endregion
            }
            else if (modelo.Equals("E965"))
            {
                #region E965

                //formato STXXXXYLLZZZZZZZ

                //X: ARRIS Manufacturing Site Code – Use X for Foxconn Manaus, Brazil
                //T: Board Type (use M for Mainboard)
                //XXXX: First 4 digits of the product top level part number (i.e. E965)
                //Y: Last digit of year i.e. Use 8 for 2018, 9 for 2019, etc.
                //LL: Mainboard modification level. Decimal value between 01 and 99. Initial value is 01 and this should be incremented as changes are applied to the mainboard.
                //ZZZZZZZ: Unique serial number of mainboard, starting at 0000001 and incrementing in decimal. This number is NOT reset for a new year or new build level.

                prefix_arrissn = "XME965";

                #endregion
            }
            else if (modelo.Equals("DSI"))
            {
                #region DSI

                /*formato MMMTPPHHYWWDLNNNN*/
                //MMM: Model code, DSI724OIT    = 105
                //T: Board Type, main board     = 0  
                //PP: Plan code, Foxconn Manaus = 29               

                prefix_arrissn = "105029";

                #endregion
            }

            return prefix_arrissn;
        }

        public String convertToAlpha(int num)
        {
            String chars = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";

            // check if we can convert to another base
            if (num > 31)
                return "";

            int r;
            String newNumber = "";

            // in r we have the offset of the char that was converted to the new base
            while (num >= 31)
            {
                r = num % 31;
                newNumber = chars[r] + newNumber;
                num = num / 31;
            }
            // the last number to convert
            newNumber = chars[num] + newNumber;

            return newNumber;
        }

        public String convertToAlphaSequencia(int valor1, int valor2, int valor3, int valor4, int valor5)
        {
            #region SEQUENCIA ALFANUMERICA

            const String caracteresValidos = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";//34 caracteres

            // 5 caracteres serial da estiqueta
            String c1 = "";
            String c2 = "";
            String c3 = "";
            String c4 = "";
            String c5 = "";
            String novaSequencia = "";
            string atualizado = string.Empty;

            #region POSIÇÃO 5

            if (caracteresValidos[valor5].ToString().Equals("9"))
            {
                c5 = "A";
                //atualizado = "C5";
            }
            else if (caracteresValidos[valor5].ToString().Equals("Z"))
            {
                c5 = "0";//FIM DA SEQUÊNCIA, ZERA O VALOR
                atualizado = "C5";//INFORMA QUE HOUVE ATUALIZAÇÃO DE VALOR
            }
            else
            {
                c5 = caracteresValidos[valor5 + 1].ToString();//NOVA SEQUÊNCIA
                atualizado = "C5";//INFORMA QUE HOUVE ATUALIZAÇÃO DE VALOR
            }

            #endregion
            //
            #region POSIÇÃO 4

            if (atualizado == "C5")
            {
                if ((c5 == "A") || (c5 == "0"))
                {
                    if (caracteresValidos[valor4].ToString().Equals("9"))
                    {
                        c4 = "A";
                    }
                    else if (caracteresValidos[valor4].ToString().Equals("Z"))
                    {
                        c4 = "0";//FIM DA SEQUÊNCIA, ZERA O VALOR
                        atualizado = "C4";//INFORMA QUE HOUVE ATUALIZAÇÃO DE VALOR
                    }
                    else
                    {
                        c4 = caracteresValidos[valor4 + 1].ToString();//NOVA SEQUÊNCIA
                        atualizado = "C4";//INFORMA QUE HOUVE ATUALIZAÇÃO DE VALOR

                    }
                    //                    
                }
                else
                {
                    c4 = caracteresValidos[valor4].ToString();//PERMANECE MESMO VALOR
                    atualizado = string.Empty;
                }

            }
            else
            {
                c4 = caracteresValidos[valor4].ToString();//PERMANECE MESMO VALOR
                atualizado = string.Empty;
            }

            #endregion
            //
            #region POSIÇÃO 3

            if (atualizado == "C4")
            {
                if ((c4 == "A") || (c4 == "0"))
                {
                    if (caracteresValidos[valor3].ToString().Equals("9"))
                    {
                        c3 = "A";
                    }
                    else if (caracteresValidos[valor3].ToString().Equals("Z"))
                    {
                        c3 = "0";//FIM DA SEQUÊNCIA, ZERA O VALOR
                        atualizado = "C3";//INFORMA QUE HOUVE ATUALIZAÇÃO DE VALOR
                    }
                    else
                    {
                        c3 = caracteresValidos[valor3 + 1].ToString();//NOVA SEQUÊNCIA
                        atualizado = "C3";//INFORMA QUE HOUVE ATUALIZAÇÃO DE VALOR

                    }
                    //                  
                }
                else
                {
                    c3 = caracteresValidos[valor3].ToString();//PERMANECE MESMO VALOR
                    atualizado = string.Empty;
                }

            }
            else
            {
                c3 = caracteresValidos[valor3].ToString();//PERMANECE MESMO VALOR
                atualizado = string.Empty;
            }

            #endregion
            //
            #region POSIÇÃO 2

            if (atualizado == "C3")
            {
                if ((c3 == "A") || (c3 == "0"))
                {
                    if (caracteresValidos[valor2].ToString().Equals("9"))
                    {
                        c2 = "A";
                    }
                    else if (caracteresValidos[valor2].ToString().Equals("Z"))
                    {
                        c2 = "0";//FIM DA SEQUÊNCIA, ZERA O VALOR
                        atualizado = "C2";//INFORMA QUE HOUVE ATUALIZAÇÃO DE VALOR
                    }
                    else
                    {
                        c2 = caracteresValidos[valor2 + 1].ToString();//NOVA SEQUÊNCIA
                        atualizado = "C2";//INFORMA QUE HOUVE ATUALIZAÇÃO DE VALOR

                    }
                    //                  
                }
                else
                {
                    c2 = caracteresValidos[valor2].ToString();//PERMANECE MESMO VALOR
                    atualizado = string.Empty;
                }

            }
            else
            {
                c2 = caracteresValidos[valor2].ToString();//PERMANECE MESMO VALOR
                atualizado = string.Empty;
            }

            #endregion
            //
            #region POSIÇÃO 1

            if (atualizado == "C2")
            {
                if ((c2 == "A") || (c2 == "0"))
                {
                    if (caracteresValidos[valor1].ToString().Equals("9"))
                    {
                        c1 = "A";

                    }
                    else if (caracteresValidos[valor1].ToString().Equals("Z"))
                    {
                        c1 = "0";//FIM DA SEQUÊNCIA, ZERA O VALOR

                    }
                    else
                    {
                        c1 = caracteresValidos[valor1].ToString();//PERMANECE MESMO VALOR 0
                    }
                }
                else
                {
                    c1 = caracteresValidos[valor1].ToString();//PERMANECE MESMO VALOR 0
                }

            }
            else
            {
                c1 = caracteresValidos[valor1].ToString();//MANTÉM MESMO VALOR 0
                //atualizado = string.Empty;
            }

            #endregion
            //
            novaSequencia = c1 + c2 + c3 + c4 + c5;
            //RESETA VALOR QUANDO ATINGIR ZZZZ
            novaSequencia = novaSequencia == "0ZZZZ" ? "00000" : novaSequencia;

            return novaSequencia;

            #endregion
        }

        public string Modelo(string sn)
        {
            #region MODELO DA ETIQUETA

            OleDbConnect Objconn = new OleDbConnect();
            //
            try
            {
                try
                {
                    Objconn.Conectar();
                    Objconn.Parametros.Clear();
                    //
                    string sql = @"SELECT DATA2 AS MODELO FROM R_AP_TEMP
                                    WHERE DATA1= 'ARRIS_SN' AND DATA5 LIKE '" + sn.Remove(5) + "%'";//Extrai somente 1ro caractere do serial
                    //                            WHERE DATA1= 'ARRIS_SN' AND DATA5 LIKE '" + sn.Remove(1, sn.Length - 1) + "%'";//Extrai somente 1ro caractere do serial
                    Objconn.SetarSQL(sql);
                    Objconn.Executar();

                }
                catch (Exception erro)
                {
                    return "ERRO: " + erro;
                }
            }
            finally
            {
                Objconn.Desconectar();
            }

            //
            if (Objconn.Tabela.Rows.Count > 0)
            {
                return Objconn.Tabela.Rows[0]["MODELO"].ToString();
            }
            else
            {
                //Nenhum registro encontrado ou erro de conexão
                return Objconn.Isvalid ? "" : "ERRO: " + Objconn.Message;
            }

            #endregion
        }

    }
}
