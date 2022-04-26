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
            this.SuspendLayout();
            // 
            // btnCrearCarpetas
            // 
            this.btnCrearCarpetas.Location = new System.Drawing.Point(12, 12);
            this.btnCrearCarpetas.Name = "btnCrearCarpetas";
            this.btnCrearCarpetas.Size = new System.Drawing.Size(157, 68);
            this.btnCrearCarpetas.TabIndex = 0;
            this.btnCrearCarpetas.Text = "Crear Carpetas";
            this.btnCrearCarpetas.UseVisualStyleBackColor = true;
            this.btnCrearCarpetas.Click += new System.EventHandler(this.btnCrearCarpetas_Click);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 450);
            this.Controls.Add(this.btnCrearCarpetas);
            this.Name = "Window";
            this.Text = "Middleware de archivos";
            this.Load += new System.EventHandler(this.Window_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnCrearCarpetas;
    }
}