using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Classes;

namespace Foxconn_Traceability
{
    class Wo
    {
        public string Numero(string Wo_Sn, string Acao)
        {
            string numero = string.Empty;
            OleDbConnect Objconn = new OleDbConnect();
            //
            try
            {
                try
                {

                    Objconn.Conectar();
                    Objconn.Parametros.Clear();
                    //
                    string sql = string.Empty;

                    if (Acao == "IMPRIMIR")
                    {
                        sql = @" SELECT W.WORKORDERNO,W.SKUNO, S.SYSSERIALNO FROM MFWORKORDER W
                                             INNER JOIN MFWORKSTATUS S ON W.WORKORDERNO = S.WORKORDERNO
                                             WHERE ((W.WORKORDERNO = '" + Wo_Sn + "')OR(S.SYSSERIALNO = '" + Wo_Sn + "'))" +
                                                "AND W.RELEASED=1" +
                                                "AND W.JOBSTARTED=1" +
                                                "AND W.CLOSED= 0" +
                                                "AND ROWNUM=1";
                    }
                    else//REIMPRIMIR 
                    {
                        sql = @" SELECT W.WORKORDERNO,W.SKUNO, S.SYSSERIALNO FROM MFWORKORDER W
                                             INNER JOIN MFWORKSTATUS S ON W.WORKORDERNO = S.WORKORDERNO
                                             WHERE ((W.WORKORDERNO = '" + Wo_Sn + "')OR(S.SYSSERIALNO = '" + Wo_Sn + "'))" +
                                                  "AND W.RELEASED=1" +
                                                  "AND W.JOBSTARTED=1" +                                                  
                                                  "AND ROWNUM=1";
                    }

                    Objconn.SetarSQL(sql);
                    Objconn.Executar();

                    if (Objconn.Isvalid)
                    {
                        if (Objconn.Tabela.Rows.Count > 0)
                        {
                            string modelo = string.Empty;
                            modelo = Objconn.Tabela.Rows[0]["SKUNO"].ToString();
                            //
                            if (modelo == "ARCT04376S")
                            {
                                numero = Objconn.Tabela.Rows[0]["WORKORDERNO"].ToString();
                            }
                            else
                            {
                                numero = "ERRO: WO não pertence ao modelo ARCT04376S";
                            }
                        }
                        else
                        {
                            numero = "ERRO: WO inválida ou não iniciada";
                        }
                    }
                    else
                    {
                        numero = "ERRO: WO inválida ou não iniciada";
                    }

                }
                catch (Exception erro)
                {
                    numero = erro.Message;
                }

            }
            finally
            {
                Objconn.Desconectar();
            }

            return numero;
        }
    }
}
