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
using System.Windows.Threading;

namespace Personnel_accounting.Windows
{
    /// <summary>
    /// Логика взаимодействия для PlatformList.xaml
    /// </summary>
    public partial class PlatformList : Window
    {
        public PlatformList()
        {
            InitializeComponent();
            lvStaff.ItemsSource = Classes.Class1.Context.Employee.ToList();
            List<EF.Platform> ListPlatform = new List<EF.Platform>();
            ListPlatform = Classes.Class1.Context.Platform.ToList();
            ListPlatform.Insert(0, new EF.Platform() { Adress = "Все" });
            cmbSort.DisplayMemberPath = "Adress";
            cmbSort.SelectedIndex = 0;
            cmbSort.ItemsSource = ListPlatform;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            dtCurrectTime.Content = DateTime.Now.ToLongTimeString();
        }

        private void Filter()
        {
            List<EF.Employee> ListEmployee = new List<EF.Employee>();
            ListEmployee = Classes.Class1.Context.Employee.ToList();
            //Фильтрация
            ListEmployee = ListEmployee.Where(i =>
            i.LastName.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.FirstName.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.MiddleName.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.FIO.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.Phone.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.Email.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            if (cmbSort.SelectedIndex != 0)
            {
                var gender = cmbSort.SelectedItem as EF.Platform;
                ListEmployee = ListEmployee.Where(i => i.IDPlatform == gender.ID).ToList();
            }
            lvStaff.ItemsSource = ListEmployee;
        }

        private void lvStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvStaff.SelectedItem is EF.Platform)
            {
                Filter();
            }
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
