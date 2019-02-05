using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Homework1
{
    class PlayerName
    {
        public void GameOver(int score)
        {
            Form EnterYourName = new Form();
            EnterYourName.CreateControl();
            EnterYourName.Text = "Спасибо за игру!";
            EnterYourName.FormBorderStyle = FormBorderStyle.FixedDialog;
            EnterYourName.Icon = new Icon("icon.ico");
            EnterYourName.Width = 400;
            EnterYourName.Height = 300;
            TextBox Header = new TextBox();
            Header.ReadOnly = true;
            Header.Text = "Введите ваше имя и нажмите 'Окей'";
            Size size = TextRenderer.MeasureText(Header.Text, Header.Font);
            Header.Width = size.Width;
            Header.Height = size.Height;
            Header.Location = new System.Drawing.Point(((EnterYourName.Width / 2) - Header.Text.Length), 0);
            Header.Show();
            EnterYourName.Controls.Add(Header);

            TextBox mainField = new TextBox();
            mainField.Location = new System.Drawing.Point(0, 20);
            mainField.Width = EnterYourName.Width;
            mainField.Show();
            EnterYourName.Controls.Add(mainField);

            Button btnOK = new Button();
            btnOK.Text = "Ок";
            btnOK.Location = new Point(EnterYourName.Width / 2, EnterYourName.Height-100);
            EnterYourName.Controls.Add(btnOK);
            EnterYourName.Show();
            btnOK.Click += btnOK_Click;
            void btnOK_Click(Object sender, EventArgs e)
            {
                WriteToTextBox(mainField.Text, score);
                EnterYourName.Close();
            }

            void WriteToTextBox(string name, int scorewrite)
            {
                using (StreamWriter sw = new StreamWriter("Records.txt", true, System.Text.Encoding.Default))
                {
                    sw.WriteLine("Имя: " + name + " Счет: " + scorewrite);
                }
            }
        }
    }
}
