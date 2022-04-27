namespace MiddlewareArchivos
{
    partial class Window
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
            this.btnCrearCarpetas = new System.Windows.Forms.Button();
            this.btnProcesarArchivosIn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCrearCarpetas
            // 
            this.btnCrearCarpetas.Location = new System.Drawing.Point(12, 12);
            this.btnCrearCarpetas.Name = "btnCrearCarpetas";
            this.btnCrearCarpetas.Size = new System.Drawing.Size(483, 68);
            this.btnCrearCarpetas.TabIndex = 0;
            this.btnCrearCarpetas.Text = "Crear carpetas";
            this.btnCrearCarpetas.UseVisualStyleBackColor = true;
            this.btnCrearCarpetas.Click += new System.EventHandler(this.btnCrearCarpetas_Click);
            // 
            // btnProcesarArchivosIn
            // 
            this.btnProcesarArchivosIn.Location = new System.Drawing.Point(12, 86);
            this.btnProcesarArchivosIn.Name = "btnProcesarArchivosIn";
            this.btnProcesarArchivosIn.Size = new System.Drawing.Size(483, 68);
            this.btnProcesarArchivosIn.TabIndex = 2;
            this.btnProcesarArchivosIn.Text = "Procesar archivos de IN";
            this.btnProcesarArchivosIn.UseVisualStyleBackColor = true;
            this.btnProcesarArchivosIn.Click += new System.EventHandler(this.btnProcesarArchivosIn_Click);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(507, 357);
            this.Controls.Add(this.btnProcesarArchivosIn);
            this.Controls.Add(this.btnCrearCarpetas);
            this.MaximizeBox = false;
            this.Name = "Window";
            this.Text = "Middleware de archivos";
            this.Load += new System.EventHandler(this.Window_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnCrearCarpetas;
        private Button btnProcesarArchivosIn;
    }
}