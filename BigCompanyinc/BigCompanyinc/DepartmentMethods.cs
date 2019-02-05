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
    public partial class DepartmentMethods
    {
        /// <summary>
        /// Обновлние ListBox'а со списком департаментов
        /// </summary>
        public void DpListBoxUpdate(ComboBox DepartmentA, ComboBox DepartmentB, SqlDataAdapter adapter, ListBox DpListBox)
        {
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            List<string> DepartmentNames = new List<string>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                String DepartmentName = Convert.ToString(dataTable.Rows[i]["DepartmentName"]);
                DepartmentNames.Add(DepartmentName);
            }
            DepartmentA.ItemsSource = DepartmentNames;
            DepartmentB.ItemsSource = DepartmentNames;
            DpListBox.ItemsSource = dataTable.DefaultView;
        }

        /// <summary>
        /// Проверка на существование Департамента перед добавлением нового
        /// </summary>
        /// <returns></returns>
        public int DeparmentExistanceCheck(string DepartmentName, string connectionString)
        {
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand check_DepartmentName = new SqlCommand("SELECT COUNT(*) FROM Department WHERE ([DepartmentName] = @DepartmentName)", connection1);
            check_DepartmentName.Parameters.AddWithValue("@DepartmentName", DepartmentName);
            int DepartmentExist = (int)check_DepartmentName.ExecuteScalar();
            connection1.Close();
            return DepartmentExist;
        }

        /// <summary>
        /// Добавление нового Департамента
        /// </summary>
        public void DepartmentAdd(string DepartmentName, string connectionString, ComboBox DepartmentA, ComboBox DepartmentB, SqlDataAdapter adapter, ListBox DpListBox)
        {
            var sql = String.Format("INSERT INTO Department (DepartmentName) " + "VALUES (N'{0}')", DepartmentName);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command = new SqlCommand(@"UPDATE Deparment SET DepartmentName = @DepartmentName WHERE ID =@ID", connection);
                command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, -1, "DepartmentName");
                SqlParameter param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
                adapter.UpdateCommand = command;
                DpListBoxUpdate(DepartmentA, DepartmentB, adapter, DpListBox);
                connection.Close();
            }
        }

        /// <summary>
        /// Удаление Департаментов
        /// </summary>
        public void DeparmtentDeletion(SqlCommand cmd, string connectionString, ComboBox DepartmentA, ComboBox DepartmentB, SqlDataAdapter adapter, ListBox DpListBox)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataRowView dataRowView = DpListBox.SelectedItem as DataRowView;
                cmd = new SqlCommand("DELETE FROM Department WHERE ID=@ID", connection);
                connection.Open();
                cmd.Parameters.AddWithValue("@ID", dataRowView.Row["Id"] as Nullable<int>);
                cmd.ExecuteNonQuery();
                adapter.UpdateCommand = cmd;
                DpListBoxUpdate(DepartmentA, DepartmentB, adapter, DpListBox);
                connection.Close();
            }
        }
    }
}
