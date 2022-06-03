namespace Foxconn_Traceability
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtQty = new System.Windows.Forms.TextBox();
            this.lbQuantidade = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbProcessando = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkImpressoraPadrao = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnReprint = new System.Windows.Forms.Button();
            this.lbSnVertical = new System.Windows.Forms.Label();
            this.txtValor = new System.Windows.Forms.TextBox();
            this.cboModelo = new System.Windows.Forms.ComboBox();
            this.lbModelo = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbRodape = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtQty
            // 
            this.txtQty.Location = new System.Drawing.Point(345, 32);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(68, 20);
            this.txtQty.TabIndex = 0;
            this.txtQty.Text = "1";
            this.txtQty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQty_KeyPress);
            // 
            // lbQuantidade
            // 
            this.lbQuantidade.AutoSize = true;
            this.lbQuantidade.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQuantidade.Location = new System.Drawing.Point(232, 30);
            this.lbQuantidade.Name = "lbQuantidade";
            this.lbQuantidade.Size = new System.Drawing.Size(107, 20);
            this.lbQuantidade.TabIndex = 2;
            this.lbQuantidade.Text = "Quantidade:";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.SlateGray;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnClose.Location = new System.Drawing.Point(15, 252);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(81, 28);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Sair";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SkyBlue;
            this.panel1.Controls.Add(this.lbProcessando);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.chkImpressoraPadrao);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Location = new System.Drawing.Point(18, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(507, 292);
            this.panel1.TabIndex = 10;
            // 
            // lbProcessando
            // 
            this.lbProcessando.AutoSize = true;
            this.lbProcessando.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProcessando.ForeColor = System.Drawing.Color.Blue;
            this.lbProcessando.Location = new System.Drawing.Point(324, 14);
            this.lbProcessando.Name = "lbProcessando";
            this.lbProcessando.Size = new System.Drawing.Size(32, 20);
            this.lbProcessando.TabIndex = 42;
            this.lbProcessando.Text = "0%";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(133, 6);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(189, 28);
            this.progressBar1.TabIndex = 42;
            // 
            // chkImpressoraPadrao
            // 
            this.chkImpressoraPadrao.AutoSize = true;
            this.chkImpressoraPadrao.Location = new System.Drawing.Point(15, 209);
            this.chkImpressoraPadrao.Name = "chkImpressoraPadrao";
            this.chkImpressoraPadrao.Size = new System.Drawing.Size(145, 17);
            this.chkImpressoraPadrao.TabIndex = 41;
            this.chkImpressoraPadrao.Text = "Usar Impressora Padrão?";
            this.chkImpressoraPadrao.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnReprint);
            this.groupBox1.Controls.Add(this.lbSnVertical);
            this.groupBox1.Controls.Add(this.txtValor);
            this.groupBox1.Controls.Add(this.cboModelo);
            this.groupBox1.Controls.Add(this.lbModelo);
            this.groupBox1.Controls.Add(this.txtQty);
            this.groupBox1.Controls.Add(this.lbQuantidade);
            this.groupBox1.Location = new System.Drawing.Point(5, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(499, 152);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IMPRIMIR";
            // 
            // btnReprint
            // 
            this.btnReprint.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnReprint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReprint.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnReprint.Location = new System.Drawing.Point(10, 118);
            this.btnReprint.Name = "btnReprint";
            this.btnReprint.Size = new System.Drawing.Size(81, 28);
            this.btnReprint.TabIndex = 43;
            this.btnReprint.Text = "Reprint";
            this.btnReprint.UseVisualStyleBackColor = false;
            this.btnReprint.Visible = false;
            this.btnReprint.Click += new System.EventHandler(this.btnReprint_Click);
            // 
            // lbSnVertical
            // 
            this.lbSnVertical.AutoSize = true;
            this.lbSnVertical.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSnVertical.Location = new System.Drawing.Point(6, 68);
            this.lbSnVertical.Name = "lbSnVertical";
            this.lbSnVertical.Size = new System.Drawing.Size(43, 20);
            this.lbSnVertical.TabIndex = 44;
            this.lbSnVertical.Text = "WO:";
            // 
            // txtValor
            // 
            this.txtValor.Location = new System.Drawing.Point(84, 68);
            this.txtValor.MaxLength = 12;
            this.txtValor.Name = "txtValor";
            this.txtValor.Size = new System.Drawing.Size(163, 20);
            this.txtValor.TabIndex = 42;
            this.txtValor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValor_KeyPress);
            // 
            // cboModelo
            // 
            this.cboModelo.FormattingEnabled = true;
            this.cboModelo.Location = new System.Drawing.Point(84, 29);
            this.cboModelo.Name = "cboModelo";
            this.cboModelo.Size = new System.Drawing.Size(142, 21);
            this.cboModelo.TabIndex = 39;
            this.cboModelo.SelectedValueChanged += new System.EventHandler(this.cboModelo_SelectedValueChanged);
            // 
            // lbModelo
            // 
            this.lbModelo.AutoSize = true;
            this.lbModelo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbModelo.Location = new System.Drawing.Point(6, 27);
            this.lbModelo.Name = "lbModelo";
            this.lbModelo.Size = new System.Drawing.Size(72, 20);
            this.lbModelo.TabIndex = 9;
            this.lbModelo.Text = "Modelo:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Location = new System.Drawing.Point(1, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(129, 33);
            this.pictureBox1.TabIndex = 38;
            this.pictureBox1.TabStop = false;
            // 
            // lbRodape
            // 
            this.lbRodape.AutoSize = true;
            this.lbRodape.BackColor = System.Drawing.Color.Transparent;
            this.lbRodape.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRodape.ForeColor = System.Drawing.Color.White;
            this.lbRodape.Location = new System.Drawing.Point(15, 359);
            this.lbRodape.Name = "lbRodape";
            this.lbRodape.Size = new System.Drawing.Size(66, 20);
            this.lbRodape.TabIndex = 39;
            this.lbRodape.Text = "Rodapé";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(543, 388);
            this.Controls.Add(this.lbRodape);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GIFT BOX V1.0.0.1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Label lbQuantidade;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbModelo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lbRodape;
        private System.Windows.Forms.ComboBox cboModelo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkImpressoraPadrao;
        private System.Windows.Forms.Label lbProcessando;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtValor;
        private System.Windows.Forms.Label lbSnVertical;
        private System.Windows.Forms.Button btnReprint;
    }
}

