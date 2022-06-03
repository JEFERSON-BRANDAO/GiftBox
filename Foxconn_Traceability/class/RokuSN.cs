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
    class RokuSN
    {
        public string Mac(string sn)
        {
            #region MAC ADDRESS

            OleDbConnect Objconn = new OleDbConnect();
            //
            try
            {
                try
                {
                    Objconn.Conectar();
                    Objconn.Parametros.Clear();
                    //
                    string sql = @"SELECT MAC FROM sfcconfig.mac_address WHERE MAC ='" + sn + "'";
                    //                           
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
                return Objconn.Tabela.Rows[0]["MAC"].ToString();
            }
            else
            {
                //Nenhum registro encontrado ou erro de conexão
                return Objconn.Isvalid ? "" : "ERRO: " + Objconn.Message;
            }

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
                                    WHERE DATA1= 'ROKU_SN' AND DATA5 LIKE '" + sn.Remove(2, sn.Length - 2) + "%'";//Extrai somente 1ro caractere do serial
                    //                            
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

        public string generate_Roku_SN(int seq, string modelo, string valorInicial)
        {
            string rokusn;
            //
            if (modelo.Equals("ROKU-PANEL"))
            {
                #region ROKU-PANEL

                String caracteresValidos = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";//34 caractes             
                string sufixo = string.Empty;
                //
                int s1 = 0;
                int s2 = 0;
                int s3 = 0;
                int s4 = 0;

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

                }

                /*
                  PF*******
                  PFYMDSSSS
                  P:PCB label
                  F:Foxconn location
                  YMD:Year,month,day
                  SSSS:Serial number (0000-zzzz)
                  Barcode:128 code,2.54mm height, Font:Verdana,Bold,4pt
                */

                string Y = DateTime.Now.Year.ToString().Remove(0, 3); //[Y=year 2020=0;
                string M = DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();//[M=Month 05=5;
                switch (M)
                {
                    case "10":
                        M = "A";
                        break;
                    case "11":
                        M = "B";
                        break;
                    case "12":
                        M = "C";
                        break;
                    default:
                        M = M.Remove(0, 1);
                        break;
                }

                string D = DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();//[D=Day 05=5;
                switch (D)
                {
                    case "10":
                        D = "A";
                        break;
                    case "11":
                        D = "B";
                        break;
                    case "12":
                        D = "C";
                        break;
                    case "13":
                        D = "D";
                        break;
                    case "14":
                        D = "E";
                        break;
                    case "15":
                        D = "F";
                        break;
                    case "16":
                        D = "G";
                        break;
                    case "17":
                        D = "H";
                        break;
                    case "18":
                        D = "I";
                        break;
                    case "19":
                        D = "J";
                        break;
                    case "20":
                        D = "K";
                        break;
                    case "21":
                        D = "L";
                        break;
                    case "22":
                        D = "M";
                        break;
                    case "23":
                        D = "N";
                        break;
                    case "24":
                        D = "O";
                        break;
                    case "25":
                        D = "P";
                        break;
                    case "26":
                        D = "Q";
                        break;
                    case "27":
                        D = "R";
                        break;
                    case "28":
                        D = "S";
                        break;
                    case "29":
                        D = "T";
                        break;
                    case "30":
                        D = "U";
                        break;
                    case "31":
                        D = "V";
                        break;

                    default:
                        D = D.Remove(0, 1);
                        break;
                }

                string SSSS = convertToAlphaSequencia(s1, s2, s3, s4);//Sequential numbers assigned by factory 
                sufixo = Y + M + D + SSSS;
                rokusn = pref_Roku_SN(modelo) + sufixo;//adiciona prefixo               
                //
                return rokusn;

                #endregion
            }
            else if (modelo.Equals("ROKU-SMT"))//ROKU-BLANK
            {
                #region ROKU-SMT

                String caracteresValidos = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";//34 caractes
                //
                int s1 = 0;
                int s2 = 0;
                int s3 = 0;
                int s4 = 0;

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

                }

                string SSSS = convertToAlphaSequencia(s1, s2, s3, s4);//Sequential numbers assigned by factory
                rokusn = pref_Roku_SN(modelo) + SSSS;//adiciona prefixo  

                return rokusn;

                #endregion
            }
            else
            {
                rokusn = string.Empty;
            }
            //
            return rokusn;
        }

        public String pref_Roku_SN(string modelo)
        {
            String prefix_RokuSn = string.Empty;
            //
            if (modelo.Equals("ROKU-PANEL"))
            {
                #region ROKU-PANEL

                /* PF*******
                  P:PCB label
                  F:Foxconn location
                */
                prefix_RokuSn = "PM";//P:PCB label; M:MANAUS;

                #endregion
            }
            if (modelo.Equals("ROKU-SMT"))//ROKU-BLANK
            {
                #region ROKU-SMT

                /*
                  XXXXXXX 
                X = plant code, Nanning plant is K" //M:MANAUS
                X = year and week, in 36
                XXXX = serial number
                */

                /*string year = DateTime.Now.Year.ToString("X").Remove(0, 2);
                //string year = DateTime.Now.Year.ToString().Remove(0, 2);
                //year = Convert.ToString(long.Parse(year), 16);
                //
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                DateTime date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                Calendar cal = dfi.Calendar;
                //
                int numero = cal.GetWeekOfYear(date1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                string week = numero.ToString().Length == 1 ? numero.ToString("X") : numero.ToString("X").Remove(0, 1);

                prefix_RokuSn = "M" + year + week;*/


                string year = DateTime.Now.Year.ToString("X").Remove(0, 2);//hexadecimal 2 últimos digitos
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                DateTime date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                Calendar cal = dfi.Calendar;
                //
                int num_week = cal.GetWeekOfYear(date1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);//número da semana do ano
                int week_year = num_week + int.Parse(year);//soma número da semana do ano com hexadecimal dos 2 ultimos digitos do ano
                string prefixo = week_year.ToString("X2");//2 casas decimais
                //
                prefix_RokuSn = "M" + prefixo;

                #endregion
            }

            return prefix_RokuSn;
        }

        public String convertToAlphaSequencia(int valor1, int valor2, int valor3, int valor4)
        {
            #region SEQUENCIA ALFANUMERICA

            const String caracteresValidos = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";//34 caracteres

            // 4 caracteres serial da estiqueta
            String c1 = "";
            String c2 = "";
            String c3 = "";
            String c4 = "";

            String novaSequencia = "";
            string atualizado = string.Empty;

            #region POSIÇÃO 4

            if (caracteresValidos[valor4].ToString().Equals("9"))
            {
                c4 = "A";
                //atualizado = "C4";
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
                    if (caracteresValidos[valor3].ToString().Equals("9"))
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
            novaSequencia = c1 + c2 + c3 + c4;
            //RESETA VALOR QUANDO ATINGIR ZZZZ
            novaSequencia = novaSequencia == "0ZZZ" ? "0000" : novaSequencia;

            return novaSequencia;

            #endregion
        }

        public List<String> get_Roku_Label(int qty, string modelo)
        {
            List<String> list = new List<string>();
            String serial = "";
            int count = 0;
            //int count1 = 0;
            int temp = 0;

            OleDbConnect Objconn = new OleDbConnect();
            try
            {
                if (modelo.Equals("ROKU-MAC"))
                {
                    if (qty % 2 == 0)
                    {
                        try
                        {
                            Objconn.Conectar();
                            Objconn.Parametros.Clear();
                            string sql = @"SELECT MAC FROM SFCCONFIG.MAC_ADDRESS WHERE USED=0 ORDER BY MAC";
                            Objconn.SetarSQL(sql);
                            Objconn.Executar();
                        }
                        finally
                        {
                            Objconn.Desconectar();
                        }
                        //Se existe mac disponível cadastrado
                        int row_Count = Objconn.Tabela.Rows.Count;
                        if (row_Count > 0)
                        {
                            if (row_Count >= qty)//Se quantidade de mac cadastrado disponivel for maior ou igual a quantidade solicida para impressão
                            {
                                DataTable dtMAC = new DataTable();
                                dtMAC = Objconn.Tabela;
                                //
                                for (int index = 0; index < qty; index++)
                                {
                                    serial = dtMAC.Rows[index][0].ToString();
                                    //
                                    if (!string.IsNullOrEmpty(serial))
                                    {
                                        //atualiza uso 
                                        try
                                        {
                                            Objconn.Conectar();
                                            Objconn.Parametros.Clear();
                                            string sql = @"UPDATE sfcconfig.mac_address SET USED = 1, LAST_UPDATE = sysdate  WHERE MAC='" + serial + "'";
                                            Objconn.SetarSQL(sql);
                                            Objconn.Executar();
                                        }
                                        finally
                                        {
                                            Objconn.Desconectar();
                                        }
                                        //
                                        if (Objconn.Isvalid)
                                        {
                                            if (serial.Length == 12)
                                            {
                                                string mac = string.Empty;
                                                //
                                                for (int linha = 0; linha < serial.Length; linha++)
                                                {
                                                    mac += serial[linha];//XX:XX:XX:XX:XX:XX
                                                    //
                                                    //if ((linha == 1) || (linha == 3) || (linha == 5) || (linha == 7) || (linha == 9))
                                                    //{
                                                    //    mac += ":";//adiciona : nas posições acima 
                                                    //}
                                                }
                                                //
                                                list.Add(mac);
                                            }

                                        }
                                        else
                                        {
                                            list.Clear();//limpa lista se houver problema na conexão com banco
                                        }
                                    }
                                }
                            }
                            else
                            {
                                serial = "ERRO quantidade solicitada é maior do que a quantidade de MAC disponível";
                            }
                        }
                        else
                        {
                            serial = "ERRO não há MAC disponível";
                        }
                    }
                    else
                    {
                        serial = "ERRO valor da quantidade deve ser par";
                    }

                }
                else if (modelo.Equals("ROKU-PANEL"))
                {
                    try
                    {
                        Objconn.Conectar();
                        Objconn.Parametros.Clear();
                        //
                        string sql = @"SELECT TRIM(DATA5) AS SERIAL FROM R_AP_TEMP
                                   WHERE DATA1='ROKU_SN' AND DATA2='" + modelo + "'";
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
                    }
                    else
                    {
                        count = 1;
                    }

                    try
                    {
                        //Verificar na mfworkstatus o max e comparar
                        Objconn.Conectar();
                        Objconn.Parametros.Clear();
                        string sql = @"SELECT TRIM(MAX(SYSSERIALNO)) AS SERIAL FROM MFWORKSTATUS
                                   WHERE SYSSERIALNO LIKE '" + pref_Roku_SN(modelo) + "%'";
                        Objconn.SetarSQL(sql);
                        Objconn.Executar();
                    }
                    finally
                    {
                        Objconn.Desconectar();
                    }

                    if (Objconn.Tabela.Rows.Count > 0)
                    {
                        if (serial.Length == 9)
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

                    for (int i = 0; i < qty; i++)
                    {
                        string valorInicial = serial;
                        if (!string.IsNullOrEmpty(valorInicial))
                        {
                            valorInicial = valorInicial.Remove(0, 5);//extrai somente os 5 últimos digitos [SERIAL]       
                            serial = generate_Roku_SN(count++, modelo, valorInicial);
                        }
                        else
                        {
                            serial = "ERRO Não existe Serial deste modelo na tabela R_AP_TEMP";
                            Log objLog = new Log();
                            objLog.Gravar(serial, modelo, "");
                        }

                        if (!serial.Contains("ERRO"))
                        {
                            if (!string.IsNullOrEmpty(serial))
                            {
                                list.Add(serial);
                            }
                        }
                    }

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
                            "VALUES ('ROKU_SN', '" + modelo + "', 1, 'SERIAL', '" + serial + "', 'S/N', 0, SYSDATE)";
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
                else if (modelo.Equals("ROKU-SMT"))//("ROKU-BLANK"))
                {
                    try
                    {
                        Objconn.Conectar();
                        Objconn.Parametros.Clear();
                        //
                        string sql = @"SELECT TRIM(DATA5) AS SERIAL FROM R_AP_TEMP
                                   WHERE DATA1='ROKU_SN' AND DATA2='" + modelo + "'";
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
                    }
                    else
                    {
                        count = 1;
                    }

                    try
                    {
                        //Verificar na mfworkstatus o max e comparar
                        Objconn.Conectar();
                        Objconn.Parametros.Clear();
                        //string sql = @"SELECT TRIM(MAX(SYSSERIALNO)) AS SERIAL FROM MFWORKSTATUS
                        //WHERE SYSSERIALNO LIKE '" + pref_Roku_SN(modelo) + "%'";
                        string sql = @"SELECT TRIM(MAX(a.SYSSERIALNO)) AS SERIAL 
                                          FROM MFWORKSTATUS a
                                          INNER JOIN MFWORKORDER b ON A.WORKORDERNO = b.WORKORDERNO
                                          WHERE SYSSERIALNO LIKE 'M%'
                                          AND b.SKUNO='RU2326000327'";
                        Objconn.SetarSQL(sql);
                        Objconn.Executar();
                    }
                    finally
                    {
                        Objconn.Desconectar();
                    }

                    if (Objconn.Tabela.Rows.Count > 0)
                    {
                        if (serial.Length == 7)
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

                    for (int i = 0; i < qty; i++)
                    {
                        string valorInicial = serial;
                        if (!string.IsNullOrEmpty(valorInicial))
                        {
                            valorInicial = valorInicial.Remove(0, 3);//extrai somente os 5 últimos digitos [SERIAL]       
                            serial = generate_Roku_SN(count++, modelo, valorInicial);
                        }
                        else
                        {
                            serial = "ERRO Não existe Serial deste modelo na tabela R_AP_TEMP";
                            Log objLog = new Log();
                            objLog.Gravar(serial, modelo, "");
                        }

                        if (!serial.Contains("ERRO"))
                        {
                            if (!string.IsNullOrEmpty(serial))
                            {
                                list.Add(serial);
                            }
                        }
                    }

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
                            "VALUES ('ROKU_SN', '" + modelo + "', 1, 'SERIAL', '" + serial + "', 'S/N', 0, SYSDATE)";
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


            }
            catch (Exception erro)
            {
                //para emitir som de alerta
                Som objSom = new Som();
                objSom.Falha();
                //
                Log objLog = new Log();
                objLog.Gravar(serial, modelo, "");
                //                   
                MessageBox.Show(erro.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //if (list.Count == 0)
            //{
            //    //para emitir som de alerta
            //    Som objSom = new Som();
            //    objSom.Falha();
            //    //                   
            //    MessageBox.Show(serial, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

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
                            WHERE DATA1= 'ROKU_SN' AND DATA2 ='" + modelo + "'";
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

        public String get_Wo(string wo)
        {
            #region WO

            OleDbConnect Objconn = new OleDbConnect();
            //
            try
            {
                try
                {
                    Objconn.Conectar();
                    Objconn.Parametros.Clear();
                    //
                    string sql = @"SELECT COUNT(*) QTD FROM MFWORKORDERSPACE 
                                    WHERE WORKORDERNO IN (SELECT WORKORDERNO FROM MFWORKORDER WHERE SKUNO='RU9026000643' AND RELEASED=1 AND JOBSTARTED=1 AND CLOSED=0
                                    AND FINISHEDQTY <= (SELECT COUNT(*)QTD FROM MFWORKORDERSPACE WHERE WORKORDERNO='" + wo + "') " +
                                    "AND WORKORDERNO='" + wo + "')";
                    Objconn.SetarSQL(sql);
                    Objconn.Executar();

                }
                catch
                {
                    return "0";
                }
            }
            finally
            {
                Objconn.Desconectar();
            }

            //
            if (Objconn.Tabela.Rows.Count > 0)
            {
                return Objconn.Tabela.Rows[0]["QTD"].ToString().Trim();
            }
            else
            {
                //Nenhum registro encontrado
                return "0";
            }

            #endregion
        }

        public bool Wo_Valida(string wo)
        {
            #region WO

            OleDbConnect Objconn = new OleDbConnect();
            //
            try
            {
                try
                {
                    Objconn.Conectar();
                    Objconn.Parametros.Clear();
                    //
                    string sql = @"SELECT WORKORDERNO FROM MFWORKORDER 
                                  WHERE SKUNO='RU2326000327' 
                                  AND RELEASED=1 AND JOBSTARTED=1 AND CLOSED=0 AND WORKORDERNO='" + wo + "'";
                    Objconn.SetarSQL(sql);
                    Objconn.Executar();

                }
                catch
                {
                    return false;
                }
            }
            finally
            {
                Objconn.Desconectar();
            }

            //
            if (Objconn.Tabela.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                //Nenhum registro encontrado
                return false;
            }

            #endregion
        }

        public List<Gift> Gift_Box(string Serial)
        {
            #region MAC8

            OleDbConnect Objconn = new OleDbConnect();
            List<Gift> lista = new List<Gift>();
            Gift item = new Gift();
            string mac8 = string.Empty;
            try
            {
                Objconn.Conectar();
                Objconn.Parametros.Clear();
                //
                string sql = "SELECT MAC8 FROM R_CUSTSN_t where SERIAL_NUMBER='" + Serial + "'";
                //
                Objconn.SetarSQL(sql);
                Objconn.Executar();

                if (Objconn.Tabela.Rows.Count > 0)
                {
                    mac8 = Objconn.Tabela.Rows[0]["MAC8"].ToString();
                    item.MAC8 = mac8;
                    //
                    lista.Add(item);
                }

            }
            finally
            {
                Objconn.Desconectar();
            }

            return lista;

            #endregion

        }

        public string Serial(string mac8)
        {
            #region SERIAL MAC

            string sn = string.Empty;
            OleDbConnect Objconn = new OleDbConnect();
            //
            try
            {
                try
                {
                    Objconn.Conectar();
                    Objconn.Parametros.Clear();
                    //
                    string sql = @"SELECT SERIAL_NUMBER from r_custsn_t where MAC8='" + mac8 + "'";
                    Objconn.SetarSQL(sql);
                    Objconn.Executar();

                    if (Objconn.Tabela.Rows.Count > 0) 
                    {
                        sn = Objconn.Tabela.Rows[0]["SERIAL_NUMBER"].ToString();
                    }

                }
                catch(Exception erro)
                {
                    sn = "ERRO: " + erro.Message;
                }
            }
            finally
            {
                Objconn.Desconectar();
            }

            return sn;

            #endregion
        }

        public class Gift
        {
            public string MAC8 { get; set; }
        }

        public List<Laser> LaserSN(string pallet)
        {
            OleDbConnect Objconn = new OleDbConnect();
            List<Laser> lista = new List<Laser>();
            //
            Laser item = new Laser();
            string skuno = string.Empty;
            string num_pallet = string.Empty;
            int pallet_qty = 0;

            try
            {
                Objconn.Conectar();
                Objconn.Parametros.Clear();
                //               
                string sql = @" Select distinct A.SYSSERIALNO, B.PARENTBUNDLENO, B.SKUNO  From  mfworkstatus A,sfcshippack B 
                             Where A.Location=B.PackNo                                      
                             and parentbundleno = '" + pallet + "'";
                //
                Objconn.SetarSQL(sql);
                Objconn.Executar();
                //
                pallet_qty = Objconn.Tabela.Rows.Count;
                if (pallet_qty > 0)
                {
                    skuno = Objconn.Tabela.Rows[0]["SKUNO"].ToString();
                    num_pallet = Objconn.Tabela.Rows[0]["PARENTBUNDLENO"].ToString();
                    //
                    item.SKUNO = skuno;
                    item.NUM_PALLET = num_pallet;
                    item.REV = REV();
                    item.PALLET_QTY = pallet_qty.ToString();
                    //
                    lista.Add(item);
                }

            }
            finally
            {
                Objconn.Desconectar();
            }

            return lista;
        }

        public class Laser
        {
            public string SKUNO { get; set; }
            public string NUM_PALLET { get; set; }
            public string REV { get; set; }
            public string PALLET_QTY { get; set; }
        }

        public string REV()
        {
            OleDbConnect Objconn = new OleDbConnect();
            string REV = string.Empty;
            //
            try
            {
                Objconn.Conectar();
                Objconn.Parametros.Clear();
                //               
                string sql = @"SELECT DATA8 AS REV FROM R_AP_TEMP WHERE DATA1='NEMO'";
                //
                Objconn.SetarSQL(sql);
                Objconn.Executar();
                //
                if (Objconn.Tabela.Rows.Count > 0)
                {
                    REV = Objconn.Tabela.Rows[0]["REV"].ToString();
                }

            }
            finally
            {
                Objconn.Desconectar();
            }

            return REV;
        }

    }
}
