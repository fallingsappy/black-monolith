using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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

namespace BigCompanyinc
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Logic L = new Logic();
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=C:\Direct2\Hospital.mdf;Integrated Security=True";
        SqlCommand cmd;
        SqlDataAdapter adapter = new SqlDataAdapter();

        /// <summary>
        /// Обновление ListViewer'a со списком сотрудников, в зависимости от выбранного департамента
        /// </summary>
        private void EpUpdate()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataRowView dataRowView = DpListBox.SelectedItem as DataRowView;

            string value = "";

            if (dataRowView != null)
            {
                value = dataRowView.Row["DepartmentName"] as string;
            }
            SqlCommand command = new SqlCommand($@"SELECT * FROM Employee WHERE Dep_nt='" + value + "'", connection);
            adapter.SelectCommand = command;
            DataTable dataTable1 = new DataTable();
            adapter.Fill(dataTable1);
            Ep.ItemsSource = dataTable1.DefaultView;
            connection.Close();
        }

        /// <summary>
        /// Обновлние ListBox'а со списком департаментов
        /// </summary>
        private void DpListBoxUpdate()
        {
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            List<string> DepartmentNames = new List<string>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                String DepartmentName = Convert.ToString(dataTable.Rows[i]["DepartmentName"]);
                DepartmentNames.Add(DepartmentName);
            }
            Department4.ItemsSource = DepartmentNames;
            DepartmentF.ItemsSource = DepartmentNames;
            DpListBox.ItemsSource = dataTable.DefaultView;
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("SELECT Id, DepartmentName FROM Department ", connection);
            adapter.SelectCommand = command;
            DpListBoxUpdate();
            L.InitReadOnly(true, Name4, LastName4, Age4, Department4, Profession4, Salary4);
            L.InitReadOnly(true, NameF, LastNameF, AgeF, DepartmentF, ProfessionF, SalaryF);
            MessageBox.Show("Выберите департамент, чтобы начать работу.");
        }

        /// <summary>
        /// Добавить новый департамент
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand check_DepartmentName = new SqlCommand("SELECT COUNT(*) FROM Department WHERE ([DepartmentName] = @DepartmentName)", connection1);
            check_DepartmentName.Parameters.AddWithValue("@DepartmentName", Name7.Text);
            int DepartmentExist = (int)check_DepartmentName.ExecuteScalar();
            connection1.Close();
            if ((Name7.Text != "") && DepartmentExist <= 0)
            {
                var sql = String.Format("INSERT INTO Department (DepartmentName) " + "VALUES (N'{0}')",
                Name7.Text);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    command = new SqlCommand(@"UPDATE Deparment SET DepartmentName = @DepartmentName WHERE ID =@ID", connection);
                    command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, -1, "DepartmentName");
                    SqlParameter param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
                    adapter.UpdateCommand = command;
                    DpListBoxUpdate();
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Введите уникальное название департамента!");
            }
        }

        /// <summary>
        /// Выбран новый элемент ListBox для коллекции департаментов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DpListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            L.InitReadOnly(false, Name4, LastName4, Age4, Department4, Profession4, Salary4);
            if(Ep.SelectedItem != null)
                L.InitReadOnly(false, NameF, LastNameF, AgeF, DepartmentF, ProfessionF, SalaryF);
            else
                L.InitReadOnly(true, NameF, LastNameF, AgeF, DepartmentF, ProfessionF, SalaryF);
            EpUpdate();
        }

        /// <summary>
        /// Добавление нового сотрудника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (((Name4.Text != "") || (LastName4.Text != "") || (Age4.Text != "") || (Profession4.Text != "") || (Salary4.Text != "")) && (L.TypeCheck(Age4, Salary4) == true) && (Department4.Text != ""))
            {
                var sql = String.Format("INSERT INTO Employee (Name, LastName, Age, Dep_nt, Profession, Salary) " + "VALUES (N'{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", Name4.Text, LastName4.Text, Age4.Text, Department4.Text, Profession4.Text, Salary4.Text);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                    EpUpdate();
                }
            }
            else
            {
                MessageBox.Show("Необходимо корректно заполнить хотя бы одно поле (поле Департамент не может быть пустым)!");
            }
        }

        /// <summary>
        /// Изменение данных уже существующего сотрудника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button0_Click(object sender, RoutedEventArgs e)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (((NameF.Text != "") || (LastNameF.Text != "") || (AgeF.Text != "") || (ProfessionF.Text != "") || (SalaryF.Text != "")) && (L.TypeCheck(AgeF, SalaryF) == true) && (DepartmentF.Text != "") && (Ep.SelectedIndex >= 0))
                {
                    DataRowView dataRowView = Ep.SelectedItem as DataRowView;
                    cmd = new SqlCommand("UPDATE Employee SET Name = @Name, LastName = @LastName, Age = @Age, Dep_nt = @Dep_nt, Profession = @Profession, Salary = @Salary WHERE ID = @ID", connection);
                    connection.Open();
                    cmd.Parameters.AddWithValue("@ID", dataRowView.Row["Id"] as Nullable<int>);
                    cmd.Parameters.AddWithValue("@Name", NameF.Text);
                    cmd.Parameters.AddWithValue("@LastName", LastNameF.Text);
                    cmd.Parameters.AddWithValue("@Age", Convert.ToInt32(AgeF.Text));
                    cmd.Parameters.AddWithValue("@Dep_nt", DepartmentF.Text);
                    cmd.Parameters.AddWithValue("@Profession", ProfessionF.Text);
                    cmd.Parameters.AddWithValue("@Salary", Convert.ToDouble(SalaryF.Text));
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    EpUpdate();
                }
                else
                {
                    MessageBox.Show("Необходимо корректно заполнить хотя бы одно поле (поле Департамент не может быть пустым)!");
                }
            }
        }

        /// <summary>
        /// Проверка выбран ли сотрудник. Помогает избежать ситуации, при которой возможно редактирование уже не существующих в базе сотрудников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            L.InitReadOnly(false, NameF, LastNameF, AgeF, DepartmentF, ProfessionF, SalaryF);
            DataRowView dataRowView = Ep.SelectedItem as DataRowView;
            if (dataRowView != null)
            {
                NameF.Text = dataRowView.Row["Name"] as string;
                LastNameF.Text = dataRowView["LastName"] as string;
                AgeF.Text = Convert.ToString(dataRowView["Age"]);
                DepartmentF.Text = dataRowView["Dep_nt"] as string;
                ProfessionF.Text = dataRowView["Profession"] as string;
                SalaryF.Text = Convert.ToString(dataRowView["Salary"]);
            }
            else
            {
                NameF.Text = "";
                LastNameF.Text = "";
                AgeF.Text = "";
                DepartmentF.Text = "";
                ProfessionF.Text = "";
                SalaryF.Text = "";
            }
        }

        /// <summary>
        /// Удаление департаментов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDelDep_Click(object sender, RoutedEventArgs e)
        {
            if((DpListBox.SelectedIndex >= 0) && (Ep.Items.Count == 0))
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DataRowView dataRowView = DpListBox.SelectedItem as DataRowView;
                    cmd = new SqlCommand("DELETE FROM Department WHERE ID=@ID", connection);
                    connection.Open();
                    cmd.Parameters.AddWithValue("@ID", dataRowView.Row["Id"] as Nullable<int>);
                    cmd.ExecuteNonQuery();
                    adapter.UpdateCommand = cmd;
                    DpListBoxUpdate();
                    connection.Close();
                }
            else
            {
                MessageBox.Show("Вы пытаетесь удалить несуществующий департамент или департамент, в котором числятся сотрудники");
            }
        }

        /// <summary>
        /// Удаление сотрудников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeleteEmp_Click(object sender, RoutedEventArgs e)
        {
            if (((NameF.Text != "") || (LastNameF.Text != "") || (AgeF.Text != "") || (ProfessionF.Text != "") || (SalaryF.Text != "")) && (L.TypeCheck(AgeF, SalaryF) == true) && (DepartmentF.Text != "") && (Ep.SelectedIndex >= 0))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DataRowView dataRowView = Ep.SelectedItem as DataRowView;
                    cmd = new SqlCommand("DELETE FROM Employee WHERE ID=@ID", connection);
                    connection.Open();
                    cmd.Parameters.AddWithValue("@ID", dataRowView.Row["Id"] as Nullable<int>);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    EpUpdate();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления!");
            }
        }
    }
}
