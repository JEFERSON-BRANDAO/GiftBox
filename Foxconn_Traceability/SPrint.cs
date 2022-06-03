using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Threading;
using Classes;
using System.Configuration;
using System.Globalization;

namespace Foxconn_Traceability
{

    public partial class Form1 : Form
    {
        internal string mensagemStatus = "Processando... ";
        internal string SN_ = null;

        public Form1()
        {
            InitializeComponent();
            //
            Clear_ProgressBar();

            #region RODAPÉ

            int anoCriacao = 2018;
            int anoAtual = DateTime.Now.Year;
            string texto = anoCriacao == anoAtual ? " Foxconn CNSBG All Rights Reserved." : "-" + anoAtual + " Foxconn CNSBG All Rights Reserved.";
            //
            lbRodape.Text = "Copyright © " + anoCriacao + texto;

            #endregion
            //
            CarregaComboModelos();

            Configuracao BotaoQTD = new Configuracao();
            if (BotaoQTD.Informa_Quantidade())
            {

                txtValor.Select();
                //cboModelo.Enabled = true;
                txtQty.Enabled = true;
            }
            else
            {
                //cboModelo.Enabled = false;
                txtQty.Enabled = false;

            }

        }

        public void CarregaComboModelos()
        {
            string caminho = AppDomain.CurrentDomain.BaseDirectory + @"\CONFIGURACAO\MODELO.txt";
            string linha;
            //
            cboModelo.Items.Add("----SELECIONE----");
            cboModelo.SelectedItem = "----SELECIONE----";
            //
            if (System.IO.File.Exists(caminho))
            {
                System.IO.StreamReader arqTXT = new System.IO.StreamReader(caminho);
                //
                while ((linha = arqTXT.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(linha))
                    {
                        if (!cboModelo.Items.Contains(linha.ToUpper().Trim()))//não add duplicado
                            cboModelo.Items.Add(linha.ToUpper().Trim());
                    }
                }

                //deixa selcionado item 0
                cboModelo.SelectedIndex = 0;
                cboModelo.SelectAll();
            }

        }

        public void createNewRecords(int valor, int qtdMaxima)
        {
            progressBar1.Maximum = qtdMaxima;
            progressBar1.Value += valor;
            // Sets the progress bar's Maximum property to  
            // the total number of records to be created.  
            //progressBar1.Maximum = 20;

            // Creates a new record in the dataset.  
            // NOTE: The code below will not compile, it merely  
            // illustrates how the progress bar would be used. 
            // Increases the value displayed by the progress bar.  
            //progressBar1.Value += 1;
            // Updates the label to show that a record was read. 

            if (progressBar1.Value == qtdMaxima)
            {
                lbProcessando.Text = "Concluído " + progressBar1.Value.ToString();
            }
            else
            {
                lbProcessando.Text = "De: " + progressBar1.Value.ToString() + " Até: " + qtdMaxima;//mensagemStatus + progressBar1.Value.ToString() + "%";
            }
        }

        public void Clear_ProgressBar()
        {
            progressBar1.Value = 0;
            lbProcessando.Text = string.Empty;//"Records Read = " + progressBar1.Value.ToString();
        }

        public void Gerar()
        {
            Clear_ProgressBar();
            //
            string quantidade = txtQty.Text.Trim();
            string rowItem = cboModelo.SelectedIndex.ToString();
            //
            if (rowItem != "0")
            {
                if (string.IsNullOrEmpty(quantidade))
                {
                    MessageBox.Show("Quantidade não pode ser vázia", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //para emitir som de alerta
                    Som objSom = new Som();
                    objSom.Falha();
                }
                else
                {
                    if (int.Parse(quantidade) > 0)
                    {
                        #region Imprimir....

                        Imprimir();

                        //#region EXIBE progressBar ....

                        //bgWorkerIndeterminada.RunWorkerAsync();

                        //#endregion

                        //
                        txtValor.SelectAll();

                        #endregion

                    }
                    else
                    {
                        MessageBox.Show("Quantide dever ser maior que 0", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //para emitir som de alerta
                        Som objSom = new Som();
                        objSom.Falha();
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecione um Modelo", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //para emitir som de alerta
                Som objSom = new Som();
                objSom.Falha();
            }

        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            //
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja fechar a aplicação?", "Fechar Aplicação",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
            {
                Application.Exit();
            }

        }


        public void Imprimir()
        {

            try
            {
                string modelo = cboModelo.SelectedItem.ToString().Trim();
                int quantidade = Int32.Parse(txtQty.Text.Trim());
                bool UsarImpressoraPadrao = chkImpressoraPadrao.Checked;
                string impressora = string.Empty;

                PrintDialog pd = new PrintDialog();
                if (!UsarImpressoraPadrao)
                {
                    pd.AllowSelection = true;
                    if (pd.ShowDialog() == DialogResult.OK)
                    {
                        //exibe janela para selecionar impressora
                        impressora = pd.PrinterSettings.PrinterName;
                    }
                    else
                    {
                        Clear_ProgressBar();
                        pd.AllowSelection = false;
                        MessageBox.Show("AVISO: Impressão cancelada", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                else
                {
                    impressora = pd.PrinterSettings.PrinterName;
                }
                // 
                if (modelo.Equals("ROKU-GIFT_BOX"))
                {
                    string snHorizontal = string.Empty;
                    string snVertical = string.Empty;
                    //
                    #region ROKU GIFT_BOX

                    string serial = txtValor.Text.Trim().ToUpper();

                    if (string.IsNullOrEmpty(serial))
                    {
                        //para emitir som de alerta
                        Som objSom = new Som();
                        objSom.Falha();
                        //   
                        Clear_ProgressBar();
                        MessageBox.Show("Informe número do MAC", "", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    RokuSN Label = new RokuSN();
                    List<RokuSN.Gift> Seriais = Label.Gift_Box(serial);
                    //
                    if (Seriais.Count > 0)
                    {

                        int de = 1;
                        int ate = quantidade;//Seriais.Count;

                        for (int index = 0; index < ate; index++)
                        {
                            //VALOR BARRA DE STATUS
                            createNewRecords(de, ate);

                            string mac8 = Seriais[0].MAC8;
                            List<String> Sn = new List<String>();
                            Sn.Add(mac8);
                            //LABEL codesoft
                            Print codesoft = new Print();
                            codesoft.Etiqueta_CodeSoft(Sn, modelo, quantidade, quantidade, false, impressora, snHorizontal, snVertical, "");

                            //LOG
                            Log objLog = new Log();
                            objLog.Gravar(serial, modelo, "Imprimir");
                        }
                    }
                    else
                    {
                        //para emitir som de alerta
                        Som objSom = new Som();
                        objSom.Falha();
                        //   
                        Clear_ProgressBar();
                        MessageBox.Show("Erro ao gerar seriais", "", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                    #endregion

                }
                else if (modelo.Equals("LASER"))
                {
                    #region ROKU LASER

                    string pallet = txtValor.Text.Trim().ToUpper();
                    //
                    if (string.IsNullOrEmpty(pallet))
                    {
                        //para emitir som de alerta
                        Som objSom = new Som();
                        objSom.Falha();
                        //   
                        Clear_ProgressBar();
                        MessageBox.Show("Informe número do PALLET", "", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    RokuSN Label = new RokuSN();
                    List<RokuSN.Laser> listPallet = Label.LaserSN(pallet);
                    //
                    if (listPallet.Count > 0)
                    {
                        int de = 1;
                        int ate = quantidade;
                        //
                        for (int index = 0; index < ate; index++)
                        {
                            //VALOR BARRA DE STATUS
                            createNewRecords(de, ate);
                            //
                            string skuno = listPallet[0].SKUNO;
                            string numPallet = listPallet[0].NUM_PALLET;
                            string REV = listPallet[0].REV;
                            string palletQTY = listPallet[0].PALLET_QTY;
                            //
                            List<String> Sn = new List<String>();
                            Sn.Add(skuno);
                            Sn.Add(numPallet);
                            Sn.Add(REV);
                            Sn.Add(palletQTY);

                            //LABEL codesoft
                            Print codesoft = new Print();
                            codesoft.Etiqueta_CodeSoft(Sn, modelo, quantidade, quantidade, false, impressora, string.Empty, string.Empty, string.Empty);
                            //LOG
                            Log objLog = new Log();
                            objLog.Gravar(numPallet, modelo, "Imprimir");
                        }
                    }
                    else
                    {
                        //para emitir som de alerta
                        Som objSom = new Som();
                        objSom.Falha();
                        //   
                        Clear_ProgressBar();
                        MessageBox.Show("Erro ao gerar seriais", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    #endregion
                }
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
        }
        //
        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cboModelo_SelectedValueChanged(object sender, EventArgs e)
        {
            string modelo = cboModelo.SelectedItem.ToString();
            txtValor.Clear();
            //
            if (modelo.Equals("ROKU-GIFT_BOX"))
            {
                lbSnVertical.Text = "MAC:";
                lbSnVertical.Visible = true;
                txtValor.Visible = true;

                btnReprint.Visible = true;
            }
            else if (modelo.Equals("LASER"))
            {
                lbSnVertical.Text = "PALLET:";
                lbSnVertical.Visible = true;
                txtValor.Visible = true;

                btnReprint.Visible = false;
            }
            else
            {
                lbSnVertical.Visible = false;
                txtValor.Visible = false;

                btnReprint.Visible = false;
            }
            //
            txtValor.Focus();
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Quando for usar scanner
            if (e.KeyChar != 13)
                SN_ += e.KeyChar.ToString().Replace("\r\n", string.Empty).Replace("\b", string.Empty).ToUpper().Trim();

            //
            if ((e.KeyChar == 13) || (e.KeyChar == Convert.ToChar(Keys.Enter)))
            {
                //caso valor digitado contenha símbolos
                if (SN_ != null)
                    SN_ = txtValor.Text.Replace("\r\n", string.Empty).Replace("\b", string.Empty).ToUpper().Trim();
                //
                Gerar();

                ////refresh
                //SN_ = txtSerial.Text.Replace("\r\n", string.Empty).Replace("\b", string.Empty).ToUpper().Trim();

            }
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            //
            Reprint reprint = new Reprint();
            reprint.ShowDialog();
            //
            this.Visible = true;
        }


    }
}
