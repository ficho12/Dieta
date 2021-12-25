using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dieta
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void VerTablas_Click(object sender, RoutedEventArgs e)
        {
            Tablas tablas = new Tablas();
            tablas.Owner = this;
            tablas.ShowDialog();
            
            /*
            if (tablas.DialogResult == true)
            {
                Title = tablas.cadena;
            }
            */
        }

        private void CargarTablas_Click(object sender, RoutedEventArgs e)
        {
            Tablas tablas = new Tablas();
            tablas.Owner = this;
            tablas.ShowDialog();

            /*
            if (tablas.DialogResult == true)
            {
                Title = tablas.cadena;
            }
            */
        }

        private void GuardarTablas_Click(object sender, RoutedEventArgs e)
        {
            Tablas tablas = new Tablas();
            tablas.Owner = this;
            tablas.ShowDialog();

            /*
            if (tablas.DialogResult == true)
            {
                Title = tablas.cadena;
            }
            */
        }

        private void GuardarTablasComo_Click(object sender, RoutedEventArgs e)
        {
            //var folderBrowserDialog1 = new FolderBrowserDialog();

            // Show the FolderBrowserDialog.
            //DialogResult result = ;
            //if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            //{
            //string folderName = folderBrowserDialog1.SelectedPath;

            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

        }

        private void EliminarTablas_Click(object sender, RoutedEventArgs e)
        {
            Tablas tablas = new Tablas();
            tablas.Owner = this;
            tablas.ShowDialog();

            /*
            if (tablas.DialogResult == true)
            {
                Title = tablas.cadena;
            }
            */
        }

        private void VerTodo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
  }

