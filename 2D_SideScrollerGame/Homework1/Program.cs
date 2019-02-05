using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Drawing;

namespace Homework1
{
    class Program
    {


        static void Main(string[] args)
        {
            Form form = new Form();
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.CreateControl();
            form.Text = "The Space Travellers";
            form.Icon = new Icon("icon.ico");
            form.Width = 800;
            form.Height = 600;
            
            Music Playlist = new Music();
            SoundPlayer sp = new SoundPlayer(Playlist.PlaySongNumber(0));
            sp.PlayLooping();

            Game.Init(form);
            form.Show();
            Game.Draw();
            SplashScreen.Buttons(form);
            Application.Run(form);
        }
    }
}
