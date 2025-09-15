using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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

namespace saving_passwords
{
    /* Объяснение условных терминов.
     * 
    // #1 "Активный" Логин - Login в БД, в стоблце Use которого указана 1, обозначающая выполненный вход через этот аккаунт (В исправном состоянии - одна).
    */

    public partial class AppMain : Window
    {
        public static string res_global = "";
        public DispatcherTimer timer;
        public DateTime endTime;
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.16.0; Data Source= Database2.accdb;Persist Security Info=False;";
        public AppMain()
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

                // *РАЗДЕЛ* "Обновление БД".

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string readinglog = "SELECT [Login] FROM [Users] WHERE [Use] = 1"; // ВЫБРАТЬ Login В Users ГДЕ Use = 1.
                    OleDbCommand log = new OleDbCommand(readinglog, conn);

                    object result = log.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        res_global = result.ToString();
                        login.Text = result.ToString(); // // Перевод в текстовый формат (Вывести "Активный" Логин (см. #1 стр.22) ).
                    }
                }
                // *КОНЕЦ РАЗДЕЛА* "Обновление БД".
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

        #region AddInfo_Open
        private void Button_Click(object sender, RoutedEventArgs e) // Окно добавления.
        {
            AddInfo window = new AddInfo();
            window.Show();
        }
        #endregion


        public void ComboBoxHashtags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ApplicationViewModel viewModel)
            {
                viewModel.SelectedHashtag = (string)((ComboBox)sender).SelectedItem;
            }
        }

        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var listBox = sender as ListBox;
        }

        #region Edit_Open
        private void btnEdit(object sender, RoutedEventArgs e) // Окно редактирования.
        {
            Edit window1 = new Edit();
            window1.Show();
        }
        #endregion

        #region Delete_Object
        private void btnDelete(object sender, RoutedEventArgs e) // Удаление приложения.
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string read_L = "SELECT [Login] FROM [Users] WHERE [Use] = 1";
                OleDbCommand read_Login = new OleDbCommand(read_L, conn);

                object result = read_Login.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    string log = result.ToString();
                    if (!String.IsNullOrEmpty(ApplicationViewModel.selectedApp.NameProgram))
                    {
                        string delete = $"DELETE FROM [{log}] WHERE [NameProgram] = '{ApplicationViewModel.selectedApp.NameProgram}'";
                        OleDbCommand del = new OleDbCommand(delete, conn);
                        del.ExecuteNonQuery();
                    }
                    else
                    {
                        MessageBox.Show("Выберите удаляемый объект");
                    }
                }
            }
        }
        #endregion

        #region Update_Book
        private void Button_Click_1(object sender, RoutedEventArgs e) // Обновление БД по кнопке.
        {
            ApplicationViewModel.LoadDataFromDatabase(); 
            ApplicationViewModel.LoadHashtagGroups();
        }
        #endregion

        #region Office_Open
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Office office = new Office();
            office.Show();
            ApplicationViewModel.LoadHashtagGroups();
        }
        #endregion
    }
    #region MVVM
    public class Info : INotifyPropertyChanged // Добавление переменных для дополнительных данных, взятых из БД.
    {
        private string nameProgram; // Создание переменных для использования.
        private string login;
        private string pass;
        private string mail;
        private string tel;
        private string web;
        private string search;

        public string NameProgram
        {
            get { return nameProgram; }
            set
            {
                nameProgram = value;
                OnPropertyChanged("NameProgram"); // Ссылание на текстовое поле в XAML.
            }
        }
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged("Login");
            }
        }
        public string Pass
        {
            get { return pass; }
            set
            {
                pass = value;
                OnPropertyChanged("Pass");
            }
        }
        public string Mail
        {
            get { return mail; }
            set
            {
                mail = value;
                OnPropertyChanged("Mail");
            }
        }
        public string Tel
        {
            get { return tel; }
            set
            {
                tel = value;
                OnPropertyChanged("Tel");
            }
        }
        public string Web
        {
            get { return web; }
            set
            {
                web = value;
                OnPropertyChanged("Web");
            }
        }
        public string Search
        {
            get { return search; }
            set
            {
                search = value;
                OnPropertyChanged("Search");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }

    public class WordWithTag // Переменные для сохранения NameProgram в отдельные массивы (хэштеги).
    {
        public string Word { get; set; }
        public string Hashtag { get; set; }
    }

    public class HashtagGroup
    {
        public string Hashtag { get; set; }
        public List<string> Words { get; set; } = new List<string>();
    }

    public class ApplicationViewModel : INotifyPropertyChanged // ViewModel для ссылания на DataContext
    {
        static public Info selectedApp;

        static public ObservableCollection<Info> Infos { get; set; } = new ObservableCollection<Info>(); // Создание коллекции.
        public Info Info_App // Создание ссылки на XAML.
        {
            get { return selectedApp; }
            set
            {
                selectedApp = value;
                OnPropertyChanged("Info_App");
            }
        }

        static public Info2 selectedUser;

        static public ObservableCollection<Info2> InfosUsers { get; set; } = new ObservableCollection<Info2>(); // Создание коллекции.
        public Info2 Admin_App // Создание ссылки на XAML.
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                OnPropertyChanged("Admin_App");
            }
        }

        static string connectionString = "Provider=Microsoft.ACE.OLEDB.16.0; Data Source= Database2.accdb;Persist Security Info=False;";


        static public ObservableCollection<HashtagGroup> HashtagGroups { get; set; } = new ObservableCollection<HashtagGroup>(); // Переменные группы (Hashtags).
        static public ObservableCollection<string> Hashtags { get; set; } = new ObservableCollection<string>();

        public static string selectedHashtag; // Переменная выбранного хэштега.
        public string SelectedHashtag
        {
            get { return selectedHashtag; }
            set
            {
                selectedHashtag = value;
                OnPropertyChanged();
                UpdateFilteredInfos();
            }
        }

        static public ObservableCollection<Info> filteredInfos = new ObservableCollection<Info>();
        public ObservableCollection<Info> FilteredInfos
        {
            get { return filteredInfos; }
            set
            {
                filteredInfos = value;
                OnPropertyChanged();
            }
        }


        public ApplicationViewModel()
        {
            Infos = new ObservableCollection<Info>(); // Вызов коллекции.
            InfosUsers = new ObservableCollection<Info2>();

            LoadDataFromDatabase(); // Вызов метода загрузки данных.
            LoadDataFromDatabase2();
            LoadHashtagGroups();
        }

        static public void LoadDataFromDatabase2()
        {
            List<string> use = new List<string>();
            using (OleDbConnection conn = new OleDbConnection(connectionString)) // Выбрать данные из БД конкретного аккаунта.
            {
                conn.Open();

                // Получаем названия таблиц.
                DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null); // Вывод названий таблиц.
                foreach (DataRow row in schemaTable.Rows)
                {
                    string tableName = row["TABLE_NAME"].ToString(); 
                    if (!tableName.StartsWith("MSys")) // Кроме системных таблиц, с началом "MSys".
                    {
                        use.Add(tableName);
                    }
                }

                // Очистка списка перед добавлением новых данных.
                InfosUsers.Clear();

                // Добавление названия таблиц в список InfosUsers.
                for (int i = 0; i < use.Count; i++)
                {
                    var info2 = new Info2
                    {
                        Users = use[i],
                    };
                    InfosUsers.Add(info2);
                }
            }
        }

        static public void LoadDataFromDatabase()
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

                    string read_I_NP = $"SELECT [NameProgram] FROM [{log}]"; // Выбрать данные из БД.
                    OleDbCommand readINP = new OleDbCommand(read_I_NP, conn);

                    string read_I_L = $"SELECT [Login] FROM [{log}]";
                    OleDbCommand readIL = new OleDbCommand(read_I_L, conn);

                    string read_I_P = $"SELECT [Password] FROM [{log}]";
                    OleDbCommand readIP = new OleDbCommand(read_I_P, conn);

                    string read_I_M = $"SELECT [Mail] FROM [{log}]";
                    OleDbCommand readIM = new OleDbCommand(read_I_M, conn);

                    string read_I_Ph = $"SELECT [Phone] FROM [{log}]";
                    OleDbCommand readIPh = new OleDbCommand(read_I_Ph, conn);

                    string read_I_W = $"SELECT [Web] FROM [{log}]";
                    OleDbCommand readIW = new OleDbCommand(read_I_W, conn);

                    string read_I_S = $"SELECT [Search] FROM [{log}]";
                    OleDbCommand readIS = new OleDbCommand(read_I_S, conn);

                    List<string> NamePrograms = new List<string>();// Добавление списков, с сортируемыми данными.
                    List<string> logins = new List<string>();
                    List<string> passes = new List<string>();
                    List<string> mails = new List<string>();
                    List<string> tels = new List<string>();
                    List<string> webs = new List<string>();
                    List<string> searches = new List<string>();
                    using (OleDbDataReader reader6 = readIS.ExecuteReader())
                    {
                        while (reader6.Read())
                        {
                            if (!reader6.IsDBNull(0)) // Проверяем на DBNull
                            {
                                searches.Add(reader6.GetString(0)); // Добавление в лист.
                            }
                        }
                    }
                    using (OleDbDataReader reader5 = readIW.ExecuteReader())
                    {
                        while (reader5.Read())
                        {
                            if (!reader5.IsDBNull(0))
                            {
                                webs.Add(reader5.GetString(0));
                            }
                        }
                    }
                    using (OleDbDataReader reader4 = readIPh.ExecuteReader())
                    {
                        while (reader4.Read())
                        {
                            if (!reader4.IsDBNull(0))
                            {
                                tels.Add(reader4.GetString(0));
                            }
                        }
                    }
                    using (OleDbDataReader reader3 = readIM.ExecuteReader())
                    {
                        while (reader3.Read())
                        {
                            if (!reader3.IsDBNull(0))
                            {
                                mails.Add(reader3.GetString(0));
                            }
                        }
                    }
                    using (OleDbDataReader reader2 = readIP.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            if (!reader2.IsDBNull(0))
                            {
                                passes.Add(reader2.GetString(0));
                            }
                        }
                    }
                    using (OleDbDataReader reader1 = readIL.ExecuteReader())
                    {
                        while (reader1.Read())
                        {
                            if (!reader1.IsDBNull(0))
                            {
                                logins.Add(reader1.GetString(0));
                            }
                        }
                    }
                    using (OleDbDataReader reader = readINP.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                NamePrograms.Add(reader.GetString(0));
                            }
                        }
                    }

                    Infos.Clear(); // Очистка списка, чтобы не было повторений.

                    for (int i = 0; i < logins.Count; i++)
                    {
                        // Убеждение, что имеются данные, чтобы избежать выхода за пределы массива.
                        if (i < passes.Count && i < NamePrograms.Count && i < mails.Count && i < tels.Count && i < webs.Count && i < searches.Count)
                        {
                            var info = new Info
                            {
                                Login = logins[i],
                                Pass = passes[i],
                                NameProgram = NamePrograms[i],
                                Mail = mails[i],
                                Tel = tels[i],
                                Web = webs[i],
                                Search = searches[i]
                            };
                            Infos.Add(info); // Добавление в коллекцию.
                        }
                    }
                }
            }
        }

        static public void LoadHashtagGroups() // Обновляемая группа хэштегов.
        {
            var grouped = Infos.GroupBy(i => i.Search).Select
            (g => new HashtagGroup
            { 
                Hashtag = g.Key, Words = g.Select(i => i.NameProgram).ToList()
            }
            ).ToList();

            HashtagGroups.Clear();
            Hashtags.Clear();
            Hashtags.Add("All"); // Добавляем опцию "Все".
            foreach (var group in grouped)
            {
                HashtagGroups.Add(group);
                Hashtags.Add(group.Hashtag);
            }
        }

        public void UpdateFilteredInfos()
        {
            if (selectedHashtag == "All") // Выведение всех приложений.
            {
                FilteredInfos = new ObservableCollection<Info>(Infos);
            }
            else if (selectedHashtag != null) // Выведение отсоритрованных приложений, по хэштегам.
            {
                FilteredInfos = new ObservableCollection<Info>(Infos.Where(i => i.Search == selectedHashtag));
            }
        
        }

        public event PropertyChangedEventHandler PropertyChanged; 

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
    #endregion
}
