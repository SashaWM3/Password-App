using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
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
using System.Windows.Shapes;

namespace saving_passwords
{
    /// <summary>
    /// Логика взаимодействия для WindowReg.xaml
    /// </summary>
    public partial class WindowReg : Window
    {
        public WindowReg()
        {
            InitializeComponent();
        }

        private void BtnGoReg(object sender, RoutedEventArgs e)
        {
            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.16.0; Data Source= Database2.accdb;Persist Security Info=False;");
            conn.Open();
            
    
            string query = "INSERT INTO [Users] ([Login], [Password], [TypeAccess]) VALUES (@Login, @Password, @TypeAccess)";
            // Добавление новых строк в столбцы БД.
            

            string readinglog = "SELECT [TypeAccess] FROM [Users] WHERE [Login] = @Login"; // Выбрать TypeAccess В Users ГДЕ Login = @Login.
            string us = "User";
            OleDbCommand reg = new OleDbCommand(query, conn);
            OleDbCommand log = new OleDbCommand(readinglog, conn);

            if (!String.IsNullOrEmpty(reglogin.Text) || !String.IsNullOrEmpty(verifpass.Password))
            {
                reg.Parameters.AddWithValue("Login", reglogin.Text); // Добавить вводимый новый логин в Login.
                log.Parameters.AddWithValue("@Login", reglogin.Text); // Введение в БД Логина для сравнения.


                object result = log.ExecuteScalar(); // Результат сравнения.



                

                if (result != null && result != DBNull.Value)
                {
                    string typeaccess = result.ToString(); // Перевод в текстовый формат.    
                    if (typeaccess != "Login")
                    {
                        MessageBox.Show("Такой логин уже используется.");
                        return;
                    }
                }
                


                string additionsDB = $"CREATE TABLE [{reglogin.Text}] ([NameProgram] Text, [Login] Text, [Password] Text, [Mail] Text, [Phone] Text, [Web] Text, [Search] Text)";
                // Добавление новых подразделов БД.
                OleDbCommand A_DB = new OleDbCommand(additionsDB, conn);

                A_DB.ExecuteNonQuery(); // Обновление данных БД.



                #region Verification_Password
                // *РАЗДЕЛ* "Проверка на прохождение условий".
                if (!String.IsNullOrEmpty(regpass.Text))
                {
                    MessageBox.Show("Введите все данные");
                    return;
                }
                else if (regpass_unvisible.Password != verifpass.Password && regpass.Text != verifpass.Password)
                {
                    MessageBox.Show("Проверьте правильность пароля.");
                    return;
                }
                else if (regpass_unvisible.Password.Length < 6 && regpass.Text.Length < 6)
                {
                    MessageBox.Show("Проверьте длину пароля.");
                    return;
                }
                else if (regpass_unvisible.Password.Any(c => char.IsLetter(c)) == false && regpass.Text.Any(c => char.IsLetter(c)) == false)
                {
                    MessageBox.Show("Проверьте пароль на наличие обязательных знаков.");
                    return;
                }
                else
                {
                    reg.Parameters.AddWithValue("Password", verifpass.Password);
                }
                // *КОНЕЦ РАЗДЕЛА* "Проверка на прохождение условий".
                #endregion


                reg.Parameters.AddWithValue("TypeAccess", us);


                reg.ExecuteNonQuery(); // Обновление данных БД.

                MessageBox.Show("Вы зарегистрированы.");

                MainWindow window1 = new MainWindow();
                window1.Show();
                conn.Close();
            }
            else
            {
                MessageBox.Show("Введите все данные");
                return;
            }
            Close();
            
        }
        #region CheckBox_Registry
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            
                if (checkBox.IsChecked.Value)
                {
                    regpass.Text = regpass_unvisible.Password; // скопируем в TextBox из PasswordBox
                    regpass.Visibility = Visibility.Visible; // TextBox - отобразить
                    regpass_unvisible.Visibility = Visibility.Hidden; // PasswordBox - скрыть
                }
                else
                {
                    regpass_unvisible.Password = regpass.Text; // скопируем в PasswordBox из TextBox 
                    regpass.Visibility = Visibility.Hidden; // TextBox - скрыть
                    regpass_unvisible.Visibility = Visibility.Visible; // PasswordBox - отобразить
                }
        }
        #endregion
        #region WindowMain_Open
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window2 = new MainWindow();
            window2.Show();
            Close();
        }
        #endregion
    }
}
