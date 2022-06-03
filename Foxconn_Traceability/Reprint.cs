using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Windows;
using Classes;

namespace Foxconn_Traceability
{
    public partial class Reprint : Form
    {
        internal string SN_ = null;
        //
        public Reprint()
        {
            InitializeComponent();
            //
            #region RODAPÉ

            int anoCriacao = 2018;
            int anoAtual = DateTime.Now.Year;
            string texto = anoCriacao == anoAtual ? " Foxconn CNSBG All Rights Reserved." : "-" + anoAtual + " Foxconn CNSBG All Rights Reserved.";
            //
            lbRodape.Text = "Copyright © " + anoCriacao + texto;

            #endregion
            //
            this.txtSerial.Select();
        }

        private void GerarEtiqueta(string SN)
        {
            RokuSN objRokuSN = new RokuSN();//ROKU
            string serial = string.Empty;
            string modelo = objRokuSN.Modelo(SN);
            //
            if (modelo.Contains("ROKU"))
            {
                if (SN.StartsWith("PM"))
                {
                    //ROKU-PANEL
                    string newSN = SN;
                    //serial = objRokuSN.pref_Roku_SN(modelo);
                    //
                    if (modelo.Contains("ERRO: ORA-12560: TNS:protocol adapter error"))
                    {
                        //para emitir som de alerta
                        Som objSom = new Som();
                        objSom.Falha();
                        //
                        MessageBox.Show(modelo + ". Verifique sua conexão de rede!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtSerial.SelectAll();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(modelo))
                        {
                            string lastSN = objRokuSN.Serial(modelo);
                            //
                            if (!string.IsNullOrEmpty(newSN))
                            {
                                if (newSN.Length == lastSN.Length)
                                {
                                    if (string.IsNullOrEmpty(lastSN))
                                    {
                                        //para emitir som de alerta
                                        Som objSom = new Som();
                                        objSom.Falha();
                                        //
                                        MessageBox.Show("Nenhum serial encontrado", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        txtSerial.SelectAll();
                                    }
                                    else
                                    {
                                        if (lastSN.Contains("ERRO"))
                                        {
                                            //para emitir som de alerta
                                            Som objSom = new Som();
                                            objSom.Falha();
                                            //
                                            MessageBox.Show(lastSN, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            txtSerial.SelectAll();
                                        }
                                        else
                                        {
                                            #region REIMPRIMIR...

                                            Reimprimir("", modelo);
                                            //txtSerial.SelectAll();
                                            txtSerial.Clear();
                                            txtSerial.Select();

                                            #endregion

                                        }
                                    }

                                }
                                else
                                {
                                    //para emitir som de alerta
                                    Som objSom = new Som();
                                    objSom.Falha();
                                    //
                                    MessageBox.Show("Tamanho do serial inválido. Tamanho SN = " + newSN.Length, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtSerial.SelectAll();
                                }
                            }
                            else
                            {
                                //para emitir som de alerta
                                Som objSom = new Som();
                                objSom.Falha();
                                //
                                MessageBox.Show("Serial não pode ser vázio", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtSerial.Select();
                            }

                        }
                        else
                        {
                            //para emitir som de alerta
                            Som objSom = new Som();
                            objSom.Falha();
                            //
                            MessageBox.Show("Não existe registro deste modelo na tabela R_AP_TEMP", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtSerial.SelectAll();
                        }
                    }
                }

            }
            //
            SN_ = null;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void Reimprimir(string serial, string modelo)
        {
            #region REIMPRIMIR

            try
            {
                List<String> Lista_Serial = new List<String>();
                bool selecionarImpressora = chkSelecionar.Checked;
                //para emitir som de alerta
                Som objSom = new Som();
                objSom.Aprovado();
                //
                #region LABEL CODESOFT

                PrintDialog pd = new PrintDialog();
                //string serial = txtSerial.Text.ToUpper().Trim();
                string impressora = string.Empty;
                //
                if (!selecionarImpressora)
                {
                    pd.AllowSelection = true;
                    if (pd.ShowDialog() == DialogResult.OK)//exibe janela para selecionar impressora
                    {
                        impressora = pd.PrinterSettings.PrinterName;//nome da impressora selecionada
                    }
                    else
                    {
                        pd.AllowSelection = false;
                        MessageBox.Show("AVISO: Reimpressão cancelada", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                else
                {
                    impressora = pd.PrinterSettings.PrinterName;//impressora padrão
                }

                //
                if (serial.Length > 12)
                {
                    serial = serial.Remove(12);
                }               

                //LABEL codesoft                  
                Lista_Serial.Add(serial);
                Print codesoft = new Print();
                codesoft.Etiqueta_CodeSoft(Lista_Serial, modelo, 1, 1, false, impressora, "", "", "");
                //LOG
                Log objLog = new Log();
                objLog.Gravar(serial, modelo, "Reimprimir");

                #endregion


            }
            catch (Exception ex)
            {
                Som objSom = new Som();
                objSom.Falha();
                //
                MessageBox.Show(ex.Message.ToString());
                //
                return;
            }
            //
            txtSerial.SelectAll();

            #endregion
        }

        private void txtSerial_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Quando for usar scanner
            if (e.KeyChar != 13)
                SN_ += e.KeyChar.ToString().Replace("\r\n", string.Empty).Replace("\b", string.Empty).ToUpper().Trim();
            //
            if ((e.KeyChar == 13) || (e.KeyChar == Convert.ToChar(Keys.Enter)))
            {
                //caso valor digitado contenha símbolos
                if (SN_ != null)
                    SN_ = txtSerial.Text.Replace("\r\n", string.Empty).Replace("\b", string.Empty).ToUpper().Trim();
                //
                //GerarEtiqueta(SN_);

                Reimprimir(SN_, "ROKU-GIFT_BOX");

                ////refresh
                //SN_ = txtSerial.Text.Replace("\r\n", string.Empty).Replace("\b", string.Empty).ToUpper().Trim();

            }
        }

    }
}
