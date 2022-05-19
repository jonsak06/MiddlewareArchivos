namespace MiddlewareArchivos
{
    partial class ProcesamientoForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnProcesarArchivosIn = new System.Windows.Forms.Button();
            this.btnProcesarArchivosOut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnProcesarArchivosIn
            // 
            this.btnProcesarArchivosIn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnProcesarArchivosIn.Location = new System.Drawing.Point(12, 12);
            this.btnProcesarArchivosIn.Name = "btnProcesarArchivosIn";
            this.btnProcesarArchivosIn.Size = new System.Drawing.Size(483, 102);
            this.btnProcesarArchivosIn.TabIndex = 2;
            this.btnProcesarArchivosIn.Text = "Procesar archivos de IN";
            this.btnProcesarArchivosIn.UseVisualStyleBackColor = true;
            this.btnProcesarArchivosIn.Click += new System.EventHandler(this.btnProcesarArchivosIn_Click);
            // 
            // btnProcesarArchivosOut
            // 
            this.btnProcesarArchivosOut.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnProcesarArchivosOut.Location = new System.Drawing.Point(12, 120);
            this.btnProcesarArchivosOut.Name = "btnProcesarArchivosOut";
            this.btnProcesarArchivosOut.Size = new System.Drawing.Size(483, 102);
            this.btnProcesarArchivosOut.TabIndex = 3;
            this.btnProcesarArchivosOut.Text = "Procesar archivos de OUT";
            this.btnProcesarArchivosOut.UseVisualStyleBackColor = true;
            this.btnProcesarArchivosOut.Click += new System.EventHandler(this.btnProcesarArchivosOut_Click);
            // 
            // ProcesamientoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(507, 293);
            this.Controls.Add(this.btnProcesarArchivosOut);
            this.Controls.Add(this.btnProcesarArchivosIn);
            this.MaximizeBox = false;
            this.Name = "ProcesamientoForm";
            this.Text = "Middleware de archivos";
            this.Load += new System.EventHandler(this.Window_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private Button btnProcesarArchivosIn;
        private Button btnProcesarArchivosOut;
    }
}