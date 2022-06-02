using Azure;
using Microsoft.Win32;
using Personnel_accounting.Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Personnel_accounting.Windows
{
    /// <summary>
    /// Логика взаимодействия для EmployeeAdd.xaml
    /// </summary>
    public partial class EmployeeAdd : Window
    {
        bool isEdit = false;
        EF.Employee editEmployee = new EF.Employee();
        string photostr;
        public EmployeeAdd()
        {
            InitializeComponent();
            cmbRole.ItemsSource = Class1.Context.Role.ToList();
            cmbRole.DisplayMemberPath = "NameRole";
            cmbRole.SelectedItem = "0";
            cmbPlatform.ItemsSource = Class1.Context.Platform.ToList();
            cmbPlatform.DisplayMemberPath = "Adress";
            cmbPlatform.SelectedItem = "0";
            isEdit = false;
        }
        public EmployeeAdd(EF.Employee employee)
        {
            InitializeComponent();
            cmbRole.ItemsSource = Class1.Context.Role.ToList();
            cmbRole.DisplayMemberPath = "NameRole";
            cmbPlatform.ItemsSource = Class1.Context.Platform.ToList();
            cmbPlatform.DisplayMemberPath = "Adress";
            txtLName.Text = employee.LastName;
            txtFName.Text = employee.FirstName;
            txtMName.Text = employee.MiddleName;
            txtPhone.Text = employee.Phone;
            txtEmail.Text = employee.Email;
            txtLogin.Text = employee.Login;
            txtPassword.Text = employee.Password;
            txtAddres.Text = employee.Adress;
            cmbRole.SelectedIndex = employee.RoleID - 1;
            cmbPlatform.SelectedIndex = (int)employee.IDPlatform - 1;
            string series = employee.Passport.PassportSeries;
            string number = employee.Passport.PassportNumber;
            txtPasport.Text = series + number;
            txtINN.Text = employee.INN.NumberINN;
            txtSNILS.Text = employee.SNILS.NumberSNILS;
            txtMilitary.Text = employee.Military.Number;
            txtMedical.Text = employee.Medical.Number;
            txtEducation.Text = employee.Education.Number;
            txtHistory.Text = employee.EmploymentHistory.Number;
            txtAddEditEmployee.Text = "Изменение данных работника";
            btnAddEmployee.Content = "Сохранить";
            if (employee.Image != null)
            {
                using (MemoryStream stream = new MemoryStream(employee.Image))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    PhotoStaff.Source = bitmapImage;
                }
            }
            isEdit = true;
            editEmployee = employee;
        }
        private void txtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = "1234567890+-".IndexOf(e.Text) < 0;
        }
        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            bool IsValidEmail(string email)
            {
                string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
                Match isMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
                return isMatch.Success;
            }
            //валидация
            if (string.IsNullOrWhiteSpace(txtLName.Text))
            {
                MessageBox.Show("Поле Фамилия не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtFName.Text))
            {
                MessageBox.Show("Поле Имя не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Поле Телефон не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Поле Login не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Поле Пароль не должно быть пустым", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!ulong.TryParse(txtPasport.Text, out ulong res))
            {
                MessageBox.Show("Поле \"Паспорта\" можно заполнить только цифрами", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var authUser = Class1.Context.Employee.ToList().
            Where(i => i.Login == txtLogin.Text).FirstOrDefault();
            if (authUser != null && isEdit == false)
            {
                MessageBox.Show("Данный логин занят!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (IsValidEmail(txtEmail.Text) == false)
            {
                MessageBox.Show("Введен некорректный Email", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (txtPhone.Text.Length > 12)
            {
                MessageBox.Show("Поле Телефон содержит больше 12 символов", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Int32.TryParse(txtPhone.Text, out int res2))
            {
                MessageBox.Show("Поле Телефон должно состоять только из цифр", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (isEdit) // Изменение пользователя
            {
                var resClick = MessageBox.Show("Изменить пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resClick == MessageBoxResult.No)
                {
                    return;
                }
                try
                {
                    string series = txtPasport.Text.Substring(0, 4);
                    string number = txtPasport.Text.Substring(4, 6);
                    editEmployee.LastName = txtLName.Text;
                    editEmployee.FirstName = txtFName.Text;
                    editEmployee.MiddleName = txtMName.Text;
                    editEmployee.Phone = txtPhone.Text;
                    editEmployee.Email = txtEmail.Text;
                    editEmployee.RoleID = (cmbRole.SelectedItem as EF.Role).ID;
                    editEmployee.IDPlatform = (cmbPlatform.SelectedItem as EF.Platform).ID - 1;
                    editEmployee.Login = txtLogin.Text;
                    editEmployee.Password = txtPassword.Text;
                    editEmployee.Passport.PassportSeries = series;
                    editEmployee.Passport.PassportNumber = number;
                    editEmployee.INN.NumberINN = txtINN.Text;
                    editEmployee.SNILS.NumberSNILS = txtSNILS.Text;
                    editEmployee.Military.Number = txtMilitary.Text;
                    editEmployee.Medical.Number = txtMedical.Text;
                    editEmployee.Education.Number = txtEducation.Text;
                    editEmployee.EmploymentHistory.Number = txtHistory.Text;
                    if (photostr != null)
                    {
                        editEmployee.Image = File.ReadAllBytes(photostr);
                    }
                    Class1.Context.SaveChanges();
                    MessageBox.Show("Пользователь изменен");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else    // Добавление пользователя
            {
                string series = txtPasport.Text.Substring(0, 4);
                string number = txtPasport.Text.Substring(4, 6);
                var resClick = MessageBox.Show("Добавить пользователя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resClick == MessageBoxResult.No)
                {
                    return;
                }
                try
                {
                    EF.Employee newEmployee = new EF.Employee();
                    EF.Passport newPassport = new EF.Passport();
                    EF.INN newINN = new EF.INN();
                    EF.SNILS newSNILS = new EF.SNILS();
                    EF.Military newMilitary = new EF.Military();
                    EF.Medical newMedical = new EF.Medical();
                    EF.Education newEducation = new EF.Education();
                    EF.EmploymentHistory newEmploymentHistory = new EF.EmploymentHistory();
                    newPassport.PassportSeries = series;
                    newPassport.PassportNumber = number;
                    newINN.NumberINN = txtINN.Text;
                    newSNILS.NumberSNILS = txtSNILS.Text;
                    newEmploymentHistory.Number = txtHistory.Text;
                    newMilitary.Number = txtMilitary.Text;
                    newEducation.Number = txtEducation.Text;
                    newMedical.Number = txtMedical.Text;
                    #region passport
                    Class1.Context.Passport.Add(newPassport);
                    Class1.Context.SaveChanges();
                    EF.Passport AddPassport = Class1.Context.Passport.Where(i => i.PassportNumber == newPassport.PassportNumber && i.PassportSeries == newPassport.PassportSeries).ToList().FirstOrDefault();
                    #endregion
                    #region INN
                    Class1.Context.INN.Add(newINN);
                    Class1.Context.SaveChanges();
                    EF.INN AddINN = Class1.Context.INN.Where(i => i.NumberINN == newINN.NumberINN).ToList().FirstOrDefault();
                    #endregion
                    #region SNILS
                    Class1.Context.SNILS.Add(newSNILS);
                    Class1.Context.SaveChanges();
                    EF.SNILS AddSNILS = Class1.Context.SNILS.Where(i => i.NumberSNILS == newSNILS.NumberSNILS).ToList().FirstOrDefault();
                    #endregion
                    #region EmploymentHistory
                    Class1.Context.EmploymentHistory.Add(newEmploymentHistory);
                    Class1.Context.SaveChanges();
                    EF.EmploymentHistory AddEmploymentHistory = Class1.Context.EmploymentHistory.Where(i => i.Number == newEmploymentHistory.Number).ToList().FirstOrDefault();
                    #endregion
                    #region Military
                    Class1.Context.Military.Add(newMilitary);
                    Class1.Context.SaveChanges();
                    EF.Military AddMilitary = Class1.Context.Military.Where(i => i.Number == newMilitary.Number).ToList().FirstOrDefault();
                    #endregion
                    #region Education
                    Class1.Context.Education.Add(newEducation);
                    Class1.Context.SaveChanges();
                    EF.Education AddEducation = Class1.Context.Education.Where(i => i.Number == newEducation.Number).ToList().FirstOrDefault();
                    #endregion
                    #region Medical
                    Class1.Context.Medical.Add(newMedical);
                    Class1.Context.SaveChanges();
                    EF.Medical AddMedical = Class1.Context.Medical.Where(i => i.Number == newMedical.Number).ToList().FirstOrDefault();
                    #endregion
                    newEmployee.LastName = txtLName.Text;
                    newEmployee.FirstName = txtFName.Text;
                    newEmployee.MiddleName = txtMName.Text;
                    newEmployee.Phone = txtPhone.Text;
                    newEmployee.Email = txtEmail.Text;
                    newEmployee.RoleID = (cmbRole.SelectedItem as EF.Role).ID;
                    newEmployee.IDPlatform = (cmbPlatform.SelectedItem as EF.Platform).ID;
                    newEmployee.Login = txtLogin.Text;
                    newEmployee.Password = txtPassword.Text;
                    newEmployee.Adress = txtAddres.Text;
                    newEmployee.IDPassport = AddPassport.ID;
                    newEmployee.IDINN = AddINN.ID;
                    newEmployee.IDSNILS = AddSNILS.ID;
                    newEmployee.IDEmploymentHistory = AddEmploymentHistory.ID;
                    newEmployee.IDMilitary = AddMilitary.ID;
                    newEmployee.IDMedical = AddMedical.ID;
                    newEmployee.IDEducation = AddEducation.ID;
                    newEmployee.HireDate = dpHire.SelectedDate;
                    if (photostr != null)
                    {
                        newEmployee.Image = File.ReadAllBytes(photostr);
                    }
                    try
                    {
                        Class1.Context.Employee.Add(newEmployee);
                        Class1.Context.Passport.Add(newPassport);
                        Class1.Context.INN.Add(newINN);
                        Class1.Context.SNILS.Add(newSNILS);
                        Class1.Context.EmploymentHistory.Add(newEmploymentHistory);
                        Class1.Context.Military.Add(newMilitary);
                        Class1.Context.Medical.Add(newMedical);
                        Class1.Context.Education.Add(newEducation);
                        Class1.Context.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                        {
                            MessageBox.Show("Object: " + validationError.Entry.Entity.ToString());
                            foreach (DbValidationError err in validationError.ValidationErrors)
                            {
                                MessageBox.Show(err.ErrorMessage + "");
                            }
                        }
                    }

                    MessageBox.Show("Пользователь добавлен");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                PhotoStaff.Source = new BitmapImage(new Uri(openFile.FileName));
                photostr = openFile.FileName;
            }
        }
    }
}
