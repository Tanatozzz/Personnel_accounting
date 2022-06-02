using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Personnel_accounting.Windows;

namespace Personnel_accounting
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var authUser = Classes.Class1.Context.Employee.ToList()
                            .Where(i => i.Login.Equals(txtLog.Text) && i.Password.Equals(txtPas.Password)).FirstOrDefault();
            if (authUser != null)
            {
                EmployeeList userWindow = new EmployeeList(authUser);
                this.Hide();
                userWindow.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Пользовател не найден");
            }
        }
    }
}
