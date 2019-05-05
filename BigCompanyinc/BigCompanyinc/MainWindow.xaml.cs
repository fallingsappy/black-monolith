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
        DepartmentMethods DpMethods = new DepartmentMethods();
        EmployeeMethods EpMethods = new EmployeeMethods();

        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|Hospital.mdf';Integrated Security=True";
        SqlCommand cmd;
        SqlDataAdapter adapter = new SqlDataAdapter();

        /// <summary>
        /// Выбран новый элемент ListBox для коллекции департаментов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DpListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            L.InitReadOnly(false, Name4, LastName4, Age4, Department4, Profession4, Salary4);
            if (Ep.SelectedItem != null)
                L.InitReadOnly(false, NameF, LastNameF, AgeF, DepartmentF, ProfessionF, SalaryF);
            else
                L.InitReadOnly(true, NameF, LastNameF, AgeF, DepartmentF, ProfessionF, SalaryF);
            EpMethods.EpUpdate(connectionString, DpListBox, Ep);
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
        /// Инициализация
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("SELECT Id, DepartmentName FROM Department ", connection);
            adapter.SelectCommand = command;
            DpMethods.DpListBoxUpdate(Department4, DepartmentF, adapter, DpListBox);
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
            int DepartmentExist = DpMethods.DeparmentExistanceCheck(Name7.Text, connectionString);
            if ((Name7.Text != "") && DepartmentExist <= 0)
            {
                DpMethods.DepartmentAdd(Name7.Text, connectionString, Department4, DepartmentF, adapter, DpListBox);
            }
            else
            {
                MessageBox.Show("Введите уникальное название департамента!");
            }
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
                EpMethods.EmployeeAdd(Name4.Text, LastName4.Text, Age4.Text, Department4.Text, Profession4.Text, Salary4.Text, connectionString, DpListBox, Ep);
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
            if (((NameF.Text != "") || (LastNameF.Text != "") || (AgeF.Text != "") || (ProfessionF.Text != "") || (SalaryF.Text != "")) && (L.TypeCheck(AgeF, SalaryF) == true) && (DepartmentF.Text != "") && (Ep.SelectedIndex >= 0))
            {
                EpMethods.EmployeeChange(NameF.Text, LastNameF.Text, AgeF.Text, DepartmentF.Text, ProfessionF.Text, SalaryF.Text, cmd, connectionString, DpListBox, Ep);
            }
            else
            {
                MessageBox.Show("Необходимо корректно заполнить хотя бы одно поле (поле Департамент не может быть пустым)!");
            }
        }

        /// <summary>
        /// Удаление департаментов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDelDep_Click(object sender, RoutedEventArgs e)
        {
            if ((DpListBox.SelectedIndex >= 0) && (Ep.Items.Count == 0))
                DpMethods.DeparmtentDeletion(cmd, connectionString, Department4, DepartmentF, adapter, DpListBox);
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
                EpMethods.EmployeeDeletion(connectionString, cmd, DpListBox, Ep);
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления!");
            }
        }
    }
}
