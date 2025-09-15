using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics.Eventing.Reader;
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

namespace saving_passwords
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "Provider=Microsoft.ACE.OLEDB.16.0; Data Source= Database2.accdb;Persist Security Info=False;";
        public MainWindow()
        {
            InitializeComponent();

            // *РАЗДЕЛ* "Проверка на вход в аккаунт (не было ли выхода)".

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string readinglog = "SELECT [Login] FROM [Users] WHERE [Use] = 1"; // ВЫБРАТЬ Login В Users ГДЕ Use = 1.
                OleDbCommand log = new OleDbCommand(readinglog, conn);
                object result = log.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string readingtype = "SELECT [TypeAccess] FROM [Users] WHERE [Use] = 1"; // ВЫБРАТЬ Login В Users ГДЕ Use = 1.
                    OleDbCommand type = new OleDbCommand(readingtype, conn);
                    object result1 = type.ExecuteScalar();
                    string typeaccess = result1.ToString(); // Перевод в текстовый формат.

                    if (typeaccess == "User")
                    {
                        AppMain window1 = new AppMain();
                        window1.Show();
                        Close();
                    }
                    else if (typeaccess == "Admin")
                    {
                        AdminApp window2 = new AdminApp();
                        window2.Show();
                        Close();
                    }
                }
                else
                {
                    conn.Close();
                }
            }
        }
        // *КОНЕЦ РАЗДЕЛА* "Проверка на вход в аккаунт (не было ли выхода)".


        // *РАЗДЕЛ* "Вход в аккаунт".

        private void BtnGo(object sender, RoutedEventArgs e)
        {
            string query = "SELECT [TypeAccess] FROM [Users] WHERE [Login] = @Логин AND [Password] = @Пароль";
            // ВЫБРАТЬ TypeAccess В Users ГДЕ Login = @Логин И Password = @Пароль.

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string login = loginBox.Text; // Вводимый Логин.
                string password = passwordBox.Password;


                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Введите логин и пароль для авторизации.");
                    return;
                }


                using (OleDbCommand command = new OleDbCommand(query, conn))
                {
                    
                    command.Parameters.AddWithValue("@Логин", login); // Введение в БД Логина для сравнения.
                    command.Parameters.AddWithValue("@Пароль", password);



                    object resultcom = command.ExecuteScalar(); // Результат сравнения.
                    if (resultcom != null && resultcom != DBNull.Value)
                    {
                        string typeaccess = resultcom.ToString(); // Перевод в текстовый формат.

                        string use = $"UPDATE [Users] SET [Use] = 1 WHERE [Login] = '{login}' ";
                        // ОБНОВИТЬ Users УСТАНОВИТЬ Use = 1 ГДЕ Login = "Вводимое значение".

                        OleDbCommand reg = new OleDbCommand(use, conn);
                        reg.Parameters.AddWithValue("Use", "1"); // Добавить 1 в Use.

                        #region User-Admin_Mode(AppMain)

                        // Варианты входа.

                        if (typeaccess == "User")
                            {
                                AppMain window1 = new AppMain();
                                window1.Show();
                                Close();
                            }
                            else if (typeaccess == "Admin")
                            {
                                AdminApp window2 = new AdminApp();
                                window2.Show();
                                Close();
                            }
                            #endregion

                        reg.ExecuteNonQuery(); // Обновление данных БД.
                    }
                    else
                    {
                        MessageBox.Show("Неверный Логин и/или Пароль");
                        return;
                    }
                }
            }
        }
        // *КОНЕЦ РАЗДЕЛА* "Вход в аккаунт".
        #region WindowReg_Open
        private void BtnReg(object sender, RoutedEventArgs e)
        {
            WindowReg window2 = new WindowReg();
            window2.Show();
            Close();
        }
        #endregion
    }
}
