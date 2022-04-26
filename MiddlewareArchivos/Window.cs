using MiddlewareArchivos.Controllers;

namespace MiddlewareArchivos
{
    public partial class Window : Form
    {
        private CarpetasController carpetasController;
        public Window()
        {
            InitializeComponent();
            carpetasController = new CarpetasController();
        }

        private void Window_Load(object sender, EventArgs e)
        {

        }

        private void btnCrearCarpetas_Click(object sender, EventArgs e)
        {
            carpetasController.crearCarpetas();
        }
    }
}