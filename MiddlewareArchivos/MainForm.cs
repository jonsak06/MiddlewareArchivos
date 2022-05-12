using MiddlewareArchivos.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiddlewareArchivos
{
    public partial class MainForm : Form
    {
        private CarpetasController carpetasController;
        private ArchivosXmlController archivosXmlController;
        public MainForm()
        {
            InitializeComponent();
            this.carpetasController = CarpetasController.Instance;
            archivosXmlController = new ArchivosXmlController();
        }

        private void btnConfiguracionInicial_Click(object sender, EventArgs e)
        {
            this.carpetasController.crearCarpetas();
            MessageBox.Show("Carpetas creadas");
        }

        private void btnFormProcesamiento_Click(object sender, EventArgs e)
        {
            if (carpetasController.existenCarpetas() && archivosXmlController.existenArchivosXml())
            {
                ProcesamientoForm form = new ProcesamientoForm();
                this.Hide();
                form.ShowDialog();
                this.Close();
            }
            MessageBox.Show("Faltan una o más carpetas y/o archivos de configuración");

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
