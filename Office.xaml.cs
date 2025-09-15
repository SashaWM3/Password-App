using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
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
using Word = Microsoft.Office.Interop.Word;
using System.Runtime.Remoting.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;


namespace saving_passwords
{
    /// <summary>
    /// Логика взаимодействия для Office.xaml
    /// </summary>
    public partial class Office : Window
    {
        private string connectionString = "Provider=Microsoft.ACE.OLEDB.16.0; Data Source= Database2.accdb;Persist Security Info=False;";
        public Office()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel();
            ApplicationViewModel.LoadHashtagGroups();
            ApplicationViewModel.LoadDataFromDatabase();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;
            DateTime dateTime = DateTime.UtcNow.Date;
            // Создание нового экземпляра Word
            Word.Application wordApp = new Word.Application();
            Word.Document wordDoc = wordApp.Documents.Add();

            // Добавление текста в документ
            Word.Paragraph title = wordDoc.Content.Paragraphs.Add();
            title.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            title.Range.Font.Size = 24;
            title.Range.Text = "Отчет по БД";
            title.Range.InsertParagraphAfter();

            Word.Paragraph para = wordDoc.Content.Paragraphs.Add();
            para.Range.Text = $"Отчет по таблицам на {dateTime.ToString("dd/MM/yyyy")}\nЛогин: {AppMain.res_global}";
            para.Range.InsertParagraphAfter();


            string query = "";
            if (ApplicationViewModel.selectedHashtag == "All") // Выведение всех приложений.
            {
                query = $"SELECT * FROM [{AppMain.res_global}]";
            }
            else if (ApplicationViewModel.selectedHashtag != null) // Выведение отсоритрованных приложений, по хэштегам.
            {
                query = $"SELECT [NameProgram], [Login], [Password], [Mail], [Phone], [Web], [Search] FROM [{AppMain.res_global}] WHERE [Search] = '{ApplicationViewModel.selectedHashtag}'";
            }

            DataTable dataTable = new DataTable();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(query, connection);

                OleDbDataAdapter adapter = new OleDbDataAdapter(command);

                adapter.Fill(dataTable);
                MessageBox.Show("Создано");
            }

            // Переход в конец документа
            Word.Range endOfDoc = wordDoc.Content;
            endOfDoc.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            Word.Table wordTable = wordDoc.Tables.Add(endOfDoc, dataTable.Rows.Count + 1, dataTable.Columns.Count);
            wordTable.Borders.Enable = 1;  // Добавление границ таблицы


            // Добавление таблицы в конец документа


            // Заполнение заголовков таблицы
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                wordTable.Cell(1, i + 1).Range.Text = dataTable.Columns[i].ColumnName;
            }

            // Заполнение строк таблицы данными из DataTable
            for (int row = 0; row < dataTable.Rows.Count; row++)
            {
                for (int col = 0; col < dataTable.Columns.Count; col++)
                {
                    wordTable.Cell(row + 2, col + 1).Range.Text = dataTable.Rows[row][col].ToString();
                }
            }

            wordTable.Range.Font.Size = 11;
            if (!String.IsNullOrEmpty(nameword.Text))
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + $"{nameword.Text}.docx";

                wordDoc.SaveAs2(filePath);

                wordDoc.Close();
                wordApp.Quit();

                wordApp.Visible = true;
            }
            else
            {
                MessageBox.Show("Введите данные");
                return;
            }
            Close();
        }

        private void nameword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ApplicationViewModel viewModel)
            {
                viewModel.SelectedHashtag = (string)((ComboBox)sender).SelectedItem;
            }

        }
        public ObservableCollection<Info> FilteredInfos
        {
            get { return ApplicationViewModel.filteredInfos; }
            set
            {
                ApplicationViewModel.filteredInfos = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
