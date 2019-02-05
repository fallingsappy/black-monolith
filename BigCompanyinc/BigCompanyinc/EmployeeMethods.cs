using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BigCompanyinc
{
    public partial class EmployeeMethods
    {
        /// <summary>
        /// Обновление ListViewer'a со списком сотрудников, в зависимости от выбранного департамента
        /// </summary>
        public void EpUpdate(string connectionString, ListBox DpListBox, ListView Ep)
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
        /// Добавление нового сотрудника
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="LastName">Фамилия</param>
        /// <param name="Age">Возраст</param>
        /// <param name="Department">Департамент</param>
        /// <param name="Profession">Профессия</param>
        /// <param name="Salary">Зарплата</param>
        public void EmployeeAdd(string Name, string LastName, string Age, string Department, string Profession, string Salary, string connectionString, ListBox DpListBox, ListView Ep)
        {
            var sql = String.Format("INSERT INTO Employee (Name, LastName, Age, Dep_nt, Profession, Salary) " + "VALUES (N'{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", Name, LastName, Age, Department, Profession, Salary);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                connection.Close();
                EpUpdate(connectionString, DpListBox, Ep);
            }
        }

        /// <summary>
        /// Изменение данных сотрудника
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <param name="LastName">Фамилия</param>
        /// <param name="Age">Возраст</param>
        /// <param name="Department">Департамент</param>
        /// <param name="Profession">Профессия</param>
        /// <param name="Salary">Зарплата</param>
        public void EmployeeChange(string Name, string LastName, string Age, string Department, string Profession, string Salary, SqlCommand cmd, string connectionString, ListBox DpListBox, ListView Ep)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataRowView dataRowView = Ep.SelectedItem as DataRowView;
                cmd = new SqlCommand("UPDATE Employee SET Name = @Name, LastName = @LastName, Age = @Age, Dep_nt = @Dep_nt, Profession = @Profession, Salary = @Salary WHERE ID = @ID", connection);
                connection.Open();
                cmd.Parameters.AddWithValue("@ID", dataRowView.Row["Id"] as Nullable<int>);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@LastName", LastName);
                cmd.Parameters.AddWithValue("@Age", Convert.ToInt32(Age));
                cmd.Parameters.AddWithValue("@Dep_nt", Department);
                cmd.Parameters.AddWithValue("@Profession", Profession);
                cmd.Parameters.AddWithValue("@Salary", Convert.ToDouble(Salary));
                cmd.ExecuteNonQuery();
                connection.Close();
                EpUpdate(connectionString, DpListBox, Ep);
            }
        }

        /// <summary>
        /// Удаление сотрудников
        /// </summary>
        public void EmployeeDeletion(string connectionString, SqlCommand cmd, ListBox DpListBox, ListView Ep)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataRowView dataRowView = Ep.SelectedItem as DataRowView;
                cmd = new SqlCommand("DELETE FROM Employee WHERE ID=@ID", connection);
                connection.Open();
                cmd.Parameters.AddWithValue("@ID", dataRowView.Row["Id"] as Nullable<int>);
                cmd.ExecuteNonQuery();
                connection.Close();
                EpUpdate(connectionString, DpListBox, Ep);
            }
        }
    }
}
