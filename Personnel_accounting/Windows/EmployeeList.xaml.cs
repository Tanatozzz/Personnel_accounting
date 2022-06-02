using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Personnel_accounting.Windows
{
    /// <summary>
    /// Логика взаимодействия для EmployeeList.xaml
    /// </summary>
    public partial class EmployeeList : Window
    {
        List<string> ListSort = new List<string>()
        {
        "По умолчанию","По фамилии","По имени","По телефону","По почте","По должности"
        };
        EF.Employee currentUser = null;
        public EmployeeList(EF.Employee authUser)
        {
            InitializeComponent();
            currentUser = authUser;
            if (currentUser == null)
            {
                MessageBox.Show("Невозможно получить данные из БД!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            Filter();
            lvStaff.ItemsSource = Classes.Class1.Context.Employee.ToList();
            cmbSort.ItemsSource = ListSort;
            cmbSort.SelectedIndex = 0; // txtLogCurrect
            txtLogCurrect.Text = "Вы вошли, как: " + authUser.FIO;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            dtCurrectTime.Content = DateTime.Now.ToLongTimeString();
        }
        //добавление

        private void btnStaffAdd_Click(object sender, RoutedEventArgs e)
        {
            EmployeeAdd staffAddWindow = new EmployeeAdd();
            staffAddWindow.ShowDialog();
            lvStaff.ItemsSource = Classes.Class1.Context.Employee.ToList();
            Filter();
        }

        private void Filter()
        {
            List<EF.Employee> ListStaff = new List<EF.Employee>();
            ListStaff = Classes.Class1.Context.Employee.Where(i => i.isDeleted == false).ToList();
            //Фильтрация
            ListStaff = ListStaff.Where(i =>
            i.LastName.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.FirstName.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.MiddleName.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.FIO.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.Phone.ToLower().Contains(txtSearch.Text.ToLower()) ||
            i.Email.ToLower().Contains(txtSearch.Text.ToLower())).ToList();

            switch (cmbSort.SelectedIndex)
            {
                case 0:
                    ListStaff = ListStaff.OrderBy(i => i.ID).ToList();
                    break;
                case 1:
                    ListStaff = ListStaff.OrderBy(i => i.LastName).ToList();
                    break;
                case 2:
                    ListStaff = ListStaff.OrderBy(i => i.FirstName).ToList();
                    break;
                case 3:
                    ListStaff = ListStaff.OrderBy(i => i.Phone).ToList();
                    break;
                case 4:
                    ListStaff = ListStaff.OrderBy(i => i.Email).ToList();
                    break;
                case 5:
                    ListStaff = ListStaff.OrderBy(i => i.RoleID).ToList();
                    break;
                default:
                    ListStaff = ListStaff.OrderBy(i => i.ID).ToList();
                    break;
            }
            lvStaff.ItemsSource = ListStaff;
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        //Удаление

        private void lvStaff_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                try
                {
                    if (lvStaff.SelectedItem is EF.Employee)
                    {
                        var resmsg = MessageBox.Show("Удалить пользователя?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (resmsg == MessageBoxResult.No)
                        {
                            return;
                        }
                        var stf = lvStaff.SelectedItem as EF.Employee;
                        stf.isDeleted = true;
                        //ClassHelper.AppData.Context.Staff.Remove(stf);
                        Classes.Class1.Context.SaveChanges();
                        MessageBox.Show("Пользователь успешно удален", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                        Filter();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void lvStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvStaff.SelectedItem is EF.Employee)
            {
                var stf = lvStaff.SelectedItem as EF.Employee;
                EmployeeAdd staffAddWindow = new EmployeeAdd(stf);
                staffAddWindow.ShowDialog();
                Filter();
            }
        }

        private void listPlatform_Click(object sender, RoutedEventArgs e)
        {
            PlatformList userWindow = new PlatformList();
            this.Hide();
            userWindow.ShowDialog();
            this.Show();
        }
    }
}
