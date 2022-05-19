namespace MiddlewareArchivos
{
    partial class MainForm
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
            this.btnConfiguracionInicial = new System.Windows.Forms.Button();
            this.btnFormProcesamiento = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnConfiguracionInicial
            // 
            this.btnConfiguracionInicial.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnConfiguracionInicial.Location = new System.Drawing.Point(12, 12);
            this.btnConfiguracionInicial.Name = "btnConfiguracionInicial";
            this.btnConfiguracionInicial.Size = new System.Drawing.Size(394, 97);
            this.btnConfiguracionInicial.TabIndex = 0;
            this.btnConfiguracionInicial.Text = "Crear carpetas";
            this.btnConfiguracionInicial.UseVisualStyleBackColor = true;
            this.btnConfiguracionInicial.Click += new System.EventHandler(this.btnConfiguracionInicial_Click);
            // 
            // btnFormProcesamiento
            // 
            this.btnFormProcesamiento.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnFormProcesamiento.Location = new System.Drawing.Point(12, 115);
            this.btnFormProcesamiento.Name = "btnFormProcesamiento";
            this.btnFormProcesamiento.Size = new System.Drawing.Size(394, 97);
            this.btnFormProcesamiento.TabIndex = 1;
            this.btnFormProcesamiento.Text = "Procesamiento de archivos";
            this.btnFormProcesamiento.UseVisualStyleBackColor = true;
            this.btnFormProcesamiento.Click += new System.EventHandler(this.btnFormProcesamiento_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(418, 287);
            this.Controls.Add(this.btnFormProcesamiento);
            this.Controls.Add(this.btnConfiguracionInicial);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnConfiguracionInicial;
        private Button btnFormProcesamiento;
    }
}