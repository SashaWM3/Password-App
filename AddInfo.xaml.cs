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
    /// Логика взаимодействия для AddInfo.xaml
    /// </summary>
    public class UpdateVariables
    {
        public bool Update { get; set; } = false;
    }
    public static class GlobalCount
    {
        public static int count { get; set; } = 0;
    }
    public partial class AddInfo : Window
    {
        private string connectionString = "Provider=Microsoft.ACE.OLEDB.16.0; Data Source= Database2.accdb;Persist Security Info=False;";
        public AddInfo()
        {
            InitializeComponent();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string readinglog = "SELECT [Login] FROM [Users] WHERE [Use] = 1"; // ВЫБРАТЬ Login В Users ГДЕ Use = 1.
                OleDbCommand log = new OleDbCommand(readinglog, conn);
            }
        }
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

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string readinglog = "SELECT [Login] FROM [Users] WHERE [Use] = 1"; // ВЫБРАТЬ Login В Users ГДЕ Use = 1.
                OleDbCommand log = new OleDbCommand(readinglog, conn);

                object result = log.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    string l = result.ToString();

                    // Создание БД.
                    string query = $"INSERT INTO [{l}] ([NameProgram], [Login], [Password], [Mail], [Phone], [Web], [Search]) VALUES (@NameProgram, @Login, @Password, @Mail, @Phone, @Web, @Search)";
                    OleDbCommand reg = new OleDbCommand(query, conn);
                    if (!String.IsNullOrEmpty(nameprog.Text) && !String.IsNullOrEmpty(login.Text) && !String.IsNullOrEmpty(pass.Text) && !String.IsNullOrEmpty(mail.Text) && !String.IsNullOrEmpty(tel.Text) && !String.IsNullOrEmpty(web.Text) && !String.IsNullOrEmpty(search.Text))
                    {
                        string NP = $"SELECT [NameProgram] FROM [{l}]";
                        OleDbCommand readNP = new OleDbCommand(NP, conn);
                        OleDbDataReader reader = readNP.ExecuteReader();

                        List<string> existingNames = new List<string>();
                        while (reader.Read())
                        {
                            existingNames.Add(reader["NameProgram"].ToString());
                        }
                        string newName = GenerateUniqueName(nameprog.Text, existingNames);
                        reg.Parameters.AddWithValue("NameProgram", newName);
                        reg.Parameters.AddWithValue("Login", login.Text);
                        reg.Parameters.AddWithValue("Password", pass.Text);
                        reg.Parameters.AddWithValue("Mail", mail.Text);
                        reg.Parameters.AddWithValue("Phone", tel.Text);
                        reg.Parameters.AddWithValue("Web", web.Text);
                        reg.Parameters.AddWithValue("Search", "#" + search.Text);
                        reg.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Введите все данные");
                        return;
                    }
                }

                Close();
            }
        }

        private string GenerateUniqueName(string baseName, List<string> existingNames) // Добавление нового индекса.
        {
            GlobalCount.count = 0;
            string newName = baseName;
            while (existingNames.Contains(newName))
            {
                GlobalCount.count++;
                newName = $"{baseName} ({GlobalCount.count})";
            }
            return newName;
        }

        public void UpdateNames(OleDbConnection conn, string tableName) // Обновление индексов.
        {
            string NP = $"SELECT [NameProgram] FROM [{tableName}]";
            OleDbCommand readNP = new OleDbCommand(NP, conn);
            OleDbDataReader reader = readNP.ExecuteReader();

            List<string> existingNames = new List<string>();
            while (reader.Read())
            {
                existingNames.Add(reader["NameProgram"].ToString());
            }

            Dictionary<string, int> baseNames = new Dictionary<string, int>();
            foreach (string name in existingNames)
            {
                string baseName = name.Contains("(") ? name.Substring(0, name.LastIndexOf(" (")) : name; // ЕСЛИ есть скобка, ТО name.Substring(0, name.LastIndexOf(" (")) ИНАЧЕ name
                if (!baseNames.ContainsKey(baseName))
                {
                    baseNames[baseName] = 1;
                }
                else
                {
                    baseNames[baseName]++;
                }

                string newName = baseNames[baseName] == 1 ? baseName : $"{baseName} ({baseNames[baseName]})";
                string updateQuery = $"UPDATE [{tableName}] SET [NameProgram] = @NewName WHERE [NameProgram] = @OldName";
                OleDbCommand updateCmd = new OleDbCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@NewName", newName);
                updateCmd.Parameters.AddWithValue("@OldName", name);
                updateCmd.ExecuteNonQuery();
            }
        }
    }
}
