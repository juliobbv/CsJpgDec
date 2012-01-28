namespace PixzGui
{
    partial class Ventana
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
            this.btnAbrir = new System.Windows.Forms.Button();
            this.pbxOriginal = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTiempo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nudImagen = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbxOriginal)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImagen)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAbrir
            // 
            this.btnAbrir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbrir.Location = new System.Drawing.Point(776, 14);
            this.btnAbrir.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAbrir.Name = "btnAbrir";
            this.btnAbrir.Size = new System.Drawing.Size(149, 70);
            this.btnAbrir.TabIndex = 0;
            this.btnAbrir.Text = "Abrir Imagen";
            this.btnAbrir.UseVisualStyleBackColor = true;
            this.btnAbrir.Click += new System.EventHandler(this.btnAbrir_Click);
            // 
            // pbxOriginal
            // 
            this.pbxOriginal.Location = new System.Drawing.Point(4, 5);
            this.pbxOriginal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbxOriginal.Name = "pbxOriginal";
            this.pbxOriginal.Size = new System.Drawing.Size(744, 472);
            this.pbxOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxOriginal.TabIndex = 1;
            this.pbxOriginal.TabStop = false;
            this.pbxOriginal.BackgroundImageChanged += new System.EventHandler(this.pbxOriginal_BackgroundImageChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(772, 471);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "Tiempo:";
            // 
            // lblTiempo
            // 
            this.lblTiempo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTiempo.Location = new System.Drawing.Point(845, 471);
            this.lblTiempo.Name = "lblTiempo";
            this.lblTiempo.Size = new System.Drawing.Size(79, 20);
            this.lblTiempo.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pbxOriginal);
            this.panel1.Location = new System.Drawing.Point(12, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(754, 482);
            this.panel1.TabIndex = 16;
            // 
            // nudImagen
            // 
            this.nudImagen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudImagen.Location = new System.Drawing.Point(849, 92);
            this.nudImagen.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudImagen.Name = "nudImagen";
            this.nudImagen.ReadOnly = true;
            this.nudImagen.Size = new System.Drawing.Size(75, 26);
            this.nudImagen.TabIndex = 17;
            this.nudImagen.ValueChanged += new System.EventHandler(this.nudImagen_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(774, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "Imagen";
            // 
            // Ventana
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 508);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudImagen);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTiempo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnAbrir);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Ventana";
            this.Text = "Pixz";
            ((System.ComponentModel.ISupportInitialize)(this.pbxOriginal)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudImagen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAbrir;
        private System.Windows.Forms.PictureBox pbxOriginal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTiempo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown nudImagen;
        private System.Windows.Forms.Label label1;
    }
}

