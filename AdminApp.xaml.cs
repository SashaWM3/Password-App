using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

namespace saving_passwords
{
    /// <summary>
    /// Логика взаимодействия для AdminApp.xaml
    /// </summary>
    public partial class AdminApp : Window
    {
        public DispatcherTimer timer;
        public DateTime endTime;
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.16.0; Data Source= Database2.accdb;Persist Security Info=False;";
        public AdminApp()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel();
            #region Timer_ UpdateDB

            endTime = DateTime.Now.AddSeconds(1); // Время работы.
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0); // Добавление дополнительного интервала таймера.
            timer.Tick += Timer_Tick;
            timer.Start(); // Таймер ЗАПУСК.

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan remainingTime = endTime - DateTime.Now;


            if (remainingTime.TotalSeconds <= 0) // Закончилось ли время.
            {
                timer.Stop(); // Таймер СТОП.
                #endregion
                #region UpdateDB
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string readinglog = "SELECT [Login] FROM [Users] WHERE [Use] = 1"; // ВЫБРАТЬ Login В Users ГДЕ Use = 1.
                    OleDbCommand log = new OleDbCommand(readinglog, conn);

                    object result = log.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        login.Text = result.ToString();
                    }
                }
                #endregion
            }

        }

        #region BtnExit

        // *РАЗДЕЛ* "Выход из аккаунта".

        private void btnExit(object sender, RoutedEventArgs e)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string readinglog = "SELECT [Login] FROM [Users] WHERE [Use] = 1"; // ВЫБРАТЬ Login В Users ГДЕ Use = 1.
                OleDbCommand log = new OleDbCommand(readinglog, conn);

                object result = log.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    string use = "UPDATE [Users] SET [Use] = 0 WHERE [Use] = 1"; // ОБНОВИТЬ Users УСТАНОВИТЬ Use = 0 ГДЕ Use = 1.
                    OleDbCommand reg = new OleDbCommand(use, conn);
                    reg.Parameters.AddWithValue("Use", "0"); // Добавить 0 в Use.
                    reg.ExecuteNonQuery(); // Обновление данных БД.
                }
            }
            MainWindow window1 = new MainWindow();
            window1.Show();
            Close();
        }

        // *КОНЕЦ РАЗДЕЛА* "Выход из аккаунта".

        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var listBox = sender as ListBox;
        }

        #region Update_Book
        private void btnUpdate(object sender, RoutedEventArgs e) // Кнопка обновления листа.
        {
            ApplicationViewModel.LoadDataFromDatabase2();
        }
        #endregion

        #region Delete_BD
        private void btnDelete(object sender, RoutedEventArgs e)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString)) // Выбрать данные из БД конкретного аккаунта.
            {
                conn.Open();

                if (ApplicationViewModel.selectedUser != null)
                {
                    string deltab = $"DROP TABLE [{ApplicationViewModel.selectedUser.Users}]";
                    string deluser = $"DELETE FROM [Users] WHERE [Login] = '{ApplicationViewModel.selectedUser.Users}'";

                    OleDbCommand delt = new OleDbCommand(deltab, conn);
                    OleDbCommand delu = new OleDbCommand(deluser, conn);

                    delt.ExecuteNonQuery();
                    delu.ExecuteNonQuery();
                }
            }
        }
        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        #region Btn_Pass
                private void btnPass(object sender, RoutedEventArgs e)
                {
                    using (OleDbConnection conn = new OleDbConnection(connectionString)) // Выбрать данные из БД конкретного аккаунта.
                    {
                        conn.Open();

                        string read_L = "SELECT [Login] FROM [Users] WHERE [Use] = 1";
                        OleDbCommand read_Login = new OleDbCommand(read_L, conn);

                        object result = read_Login.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            string log = result.ToString();

                            string update = $"UPDATE [Users] SET [Password] = @Password WHERE [Login] = '{ApplicationViewModel.selectedUser.Users}' ";
                            // ОБНОВИТЬ Users 

                            OleDbCommand upd = new OleDbCommand(update, conn); // Редактирование БД.
                            if (!String.IsNullOrEmpty(pass.Text))
                            {
                                upd.Parameters.AddWithValue("Password", pass.Text);
                                upd.ExecuteNonQuery(); // Обновление данных БД.
                                MessageBox.Show("Пароль обновлён.");
                            }
                            else 
                            {
                                MessageBox.Show("Введите новый пароль.");
                            }
                        }
                    }
                }
                #endregion

    }


    public class Info2 : INotifyPropertyChanged // Добавление переменных для дополнительных данных, взятых из БД.
    {
        private string users;


        public string Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged("Users"); // Ссылание на текстовое поле в XAML.
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        

    }

}

    

