using System;
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
    /// Логика взаимодействия для Edit.xaml
    /// </summary>
    public partial class Edit : Window
    {
        public Edit()
        {
            InitializeComponent();
        }
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.16.0; Data Source= Database2.accdb;Persist Security Info=False;";
        private void Button_Click(object sender, RoutedEventArgs e)
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
                    string update = $"UPDATE [{log}] SET [NameProgram] = @NameProgram, [Login] = @Login, [Password] = @Password, [Mail] = @Mail, [Phone] = @Phone, [Web] = @Web, [Search] = @Search WHERE [NameProgram] = '{ApplicationViewModel.selectedApp.NameProgram}' ";
                    // ОБНОВИТЬ Users 

                    OleDbCommand upd = new OleDbCommand(update, conn); // Редактирование БД.
                    if (!String.IsNullOrEmpty(nameprog.Text) && !String.IsNullOrEmpty(login.Text) && !String.IsNullOrEmpty(pass.Text) && !String.IsNullOrEmpty(mail.Text) && !String.IsNullOrEmpty(tel.Text) && !String.IsNullOrEmpty(web.Text) && !String.IsNullOrEmpty(search.Text))
                    {
                        upd.Parameters.AddWithValue("NameProgram", nameprog.Text);
                        upd.Parameters.AddWithValue("Login", login.Text);
                        upd.Parameters.AddWithValue("Password", pass.Text);
                        upd.Parameters.AddWithValue("Mail", mail.Text);
                        upd.Parameters.AddWithValue("Phone", tel.Text);
                        upd.Parameters.AddWithValue("Web", web.Text);
                        upd.Parameters.AddWithValue("Search", "#" + search.Text);

                        upd.ExecuteNonQuery(); // Обновление данных БД.
                    }
                    else
                    {
                        MessageBox.Show("Введите все данные");
                        return;
                    }
                    Close();
                }
            }
        }

        #region Not_used
        private void nameprog_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void login_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void pass_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void mail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tel_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void web_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion
    }
}
