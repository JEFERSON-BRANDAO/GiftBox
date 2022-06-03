using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Classes;

namespace Foxconn_Traceability
{
    public class Configuracao
    {
        public bool Informa_Quantidade()
        {
            bool permissao = false;
            //
            string nomeArquivo = AppDomain.CurrentDomain.BaseDirectory + @"\CONFIGURACAO\HABILITAR_QUANTIDADE.txt";
            if (System.IO.File.Exists(nomeArquivo))
            {
                try
                {
                    string linha;
                    string valor = string.Empty;
                    //
                    if (System.IO.File.Exists(nomeArquivo))
                    {
                        System.IO.StreamReader arqTXT = new System.IO.StreamReader(nomeArquivo);
                        //
                        while ((linha = arqTXT.ReadLine()) != null)
                        {
                            valor = linha.Trim();//linha[indice];
                        }
                        //
                        arqTXT.Close();

                        //
                        if (!string.IsNullOrEmpty(valor))
                        {
                            if (valor.Equals("1"))
                                permissao = true;
                        }
                    }

                }
                catch
                {
                    //
                }
            }
            //
            return permissao;

        }


        public string Data_Gerada(string serial)
        {
            OleDbConnect Objconn = new OleDbConnect();
            string data = DateTime.Now.Date.ToString("yyyy-MM-dd");
            //
            try
            {
                Objconn.Conectar();
                Objconn.Parametros.Clear();
                //
                string sql = @"SELECT TO_CHAR(WORK_TIME,'YYYY-MM-DD') AS  WORK_TIME FROM R_AP_TEMP
                                   WHERE DATA1='ARRIS_SN' AND DATA5 ='" + serial + "'";
                //
                Objconn.SetarSQL(sql);
                Objconn.Executar();
                //
                if (Objconn.Tabela.Rows.Count > 0)
                {
                    data = Objconn.Tabela.Rows[0]["WORK_TIME"].ToString();
                }
            }
            finally
            {
                Objconn.Desconectar();
            }
            //
            return data;

        }



    }
}
