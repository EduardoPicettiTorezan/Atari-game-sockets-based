using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace Trabalho_Sockets
{
    public partial class Menu_Inicial : Form
    {
        private Button btnSair;
        private Button btnConectar;
        private Button btnIniciarServidor;
        private TextBox txtIp;
        private Label label1;
        public static string sIpdoServidor;
    
        public Menu_Inicial()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnSair = new System.Windows.Forms.Button();
            this.btnConectar = new System.Windows.Forms.Button();
            this.btnIniciarServidor = new System.Windows.Forms.Button();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSair
            // 
            this.btnSair.Location = new System.Drawing.Point(308, 44);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(91, 23);
            this.btnSair.TabIndex = 9;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = false;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click_1);
            // 
            // btnConectar
            // 
            this.btnConectar.Location = new System.Drawing.Point(153, 44);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(103, 23);
            this.btnConectar.TabIndex = 8;
            this.btnConectar.Text = "Conectar";
            this.btnConectar.UseVisualStyleBackColor = false;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click_1);
            // 
            // btnIniciarServidor
            // 
            this.btnIniciarServidor.Location = new System.Drawing.Point(12, 44);
            this.btnIniciarServidor.Name = "btnIniciarServidor";
            this.btnIniciarServidor.Size = new System.Drawing.Size(92, 23);
            this.btnIniciarServidor.TabIndex = 7;
            this.btnIniciarServidor.Text = "IniciarServidor";
            this.btnIniciarServidor.UseVisualStyleBackColor = false;
            this.btnIniciarServidor.Click += new System.EventHandler(this.btnIniciarServidor_Click_1);
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(38, 12);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(100, 20);
            this.txtIp.TabIndex = 6;
            this.txtIp.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "IP:";
            // 
            // Menu_Inicial
            // 
            this.ClientSize = new System.Drawing.Size(416, 80);
            this.Controls.Add(this.btnSair);
            this.Controls.Add(this.btnConectar);
            this.Controls.Add(this.btnIniciarServidor);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.label1);
            this.Name = "Menu_Inicial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnSair_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja realmente sair?", "Confirma",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnConectar_Click_1(object sender, EventArgs e)
        {
            if ((txtIp.Text == ""))
            {
                MessageBox.Show("IP do servidor deve ser informado.");
            }
            else
            {
                sIpdoServidor = txtIp.Text;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnIniciarServidor_Click_1(object sender, EventArgs e)
        {
            string sCaminhoServidor = "";
            sCaminhoServidor = (@"" + Application.StartupPath + "\\" +  "gameServer.exe");

            if (File.Exists(sCaminhoServidor))
            {
                System.Diagnostics.Process.Start(sCaminhoServidor);
            }
            else
            {
                MessageBox.Show("Servidor não localizado no diretório: " + Application.StartupPath);
            }
        }
    }
}
