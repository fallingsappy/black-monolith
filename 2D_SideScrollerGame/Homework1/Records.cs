using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Homework1
{
    class Records
    {
        Form formRecords = new Form();


        public Records()
        {
            formRecords.CreateControl();
            formRecords.Text = "Рекорды";
            formRecords.FormBorderStyle = FormBorderStyle.FixedDialog;
            formRecords.Icon = new Icon("icon.ico");
            formRecords.Width = 400;
            formRecords.Height = 300;
            TextBox Header = new TextBox();
            Header.ReadOnly = true;
            Header.Text = "Рекорды";
            Size size = TextRenderer.MeasureText(Header.Text, Header.Font);
            Header.Width = size.Width;
            Header.Height = size.Height;
            Header.Location = new System.Drawing.Point(((formRecords.Width / 2) - Header.Text.Length), 0);
            Header.Show();
            TextBox mainField = new TextBox();
            mainField.ReadOnly = true;
            mainField.Multiline = true;
            mainField.ScrollBars = ScrollBars.Vertical;
            mainField.Location = new System.Drawing.Point(0, 20);
            mainField.Height = formRecords.Height - 55;
            mainField.Width = formRecords.Width - 10;
            mainField.Show();
            formRecords.Controls.Add(Header);
            formRecords.Controls.Add(mainField);
            formRecords.CreateControl();
            formRecords.Show();
            mainField.Text = ReadToTextBox();
        }

        public string ReadToTextBox()
        {
            using (StreamReader sr = new StreamReader("Records.txt", System.Text.Encoding.Default))
            {
                return sr.ReadToEnd();
            }
        }

        
    }
}
