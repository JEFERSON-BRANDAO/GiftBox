using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classes
{
    class Log
    {
        public void Gravar(string Serial, string Modelo, string Tipo)
        {
            #region CRIA ARQUIVO DE LOG .txt

            string hora = DateTime.Now.Hour.ToString().Length == 1 ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString();
            string minuto = DateTime.Now.Minute.ToString().Length == 1 ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString();
            string segundo = DateTime.Now.Second.ToString().Length == 1 ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString();
            //
            string dataHora = DateTime.Now.Date.ToString("dd-MM-yyyy") + " " + hora + ":" + minuto + ":" + segundo;

            string nomeArquivo = Tipo.Equals("Reimprimir") ? AppDomain.CurrentDomain.BaseDirectory + @"\REIMPRESSAO\LOG\HISTORICO.txt" : AppDomain.CurrentDomain.BaseDirectory + @"\IMPRESSAO\LOG\HISTORICO.txt";
            //
            if (!System.IO.File.Exists(nomeArquivo))//quando arquivo não exite. Criado pela 1ra vez
            {
                System.IO.File.Create(nomeArquivo).Close();
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(nomeArquivo))
                {
                    string historico = "[" + dataHora + "] - SN:" + Serial + ", MODELO:" + Modelo;
                    //
                    sw.WriteLine(historico);
                    sw.Close();
                }
            }
            else
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(nomeArquivo))
                {
                    string historico = "[" + dataHora + "] - SN:" + Serial + ", MODELO:" + Modelo;
                    //
                    sw.WriteLine(historico);
                    sw.Close();
                }
            }

            #endregion
        }
    }
}
