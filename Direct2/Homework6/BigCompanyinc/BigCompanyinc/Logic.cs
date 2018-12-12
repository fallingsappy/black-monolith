using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BigCompanyinc
{
    public partial class Logic
    {
        /// <summary>
        /// Инициализация полей
        /// </summary>
        /// <param name="F"></param>
        /// <param name="FN"></param>
        /// <param name="LN"></param>
        /// <param name="A"></param>
        /// <param name="D"></param>
        /// <param name="P"></param>
        /// <param name="S"></param>
        public void InitReadOnly(bool F, TextBox FN, TextBox LN, TextBox A, ComboBox D, TextBox P, TextBox S)
        {
            FN.IsReadOnly = F;
            LN.IsReadOnly = F;
            A.IsReadOnly = F;
            D.IsEnabled = !F;
            P.IsReadOnly = F;
            S.IsReadOnly = F;
        }

        /// <summary>
        /// Проверка на правильный формат вводимых данных
        /// </summary>
        /// <param name="AgeFun">Возраст Employee</param>
        /// <param name="SalaryFun">Зарплата Employee</param>
        /// <returns></returns>
        public bool TypeCheck(TextBox AgeFun, TextBox SalaryFun)
        {
            int n;
            double n2;
            if (!Int32.TryParse(AgeFun.Text, out n) || (n < 0))
            {
                MessageBox.Show("Поле 'Возраст' имеет неверный формат (необходимо ввести неотрицательное целое число).");
                return false;
            }
            else if (!Double.TryParse(SalaryFun.Text, out n2) || (n2 < 0))
            {
                 MessageBox.Show("Поле 'Заработная плата' имеет неверный формат (необходимо ввести неотрицательное число).");
                 return false;
            }
            else
            {
                 return true;
            }
        }
    }
}