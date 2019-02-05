using System;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace Homework1
{
    class SplashScreen
    {
        List<string> Playlist = new List<string>();

        public static BaseObject[] _objs1;
        public static BaseObject[] _objs2;

        public static void LoadSplash()
        {
            _objs1 = new BaseObject[30];
            _objs2 = new BaseObject[40];
            _objs2[39] = new Planets();
            _objs1[29] = new Title();
            var rnd = new Random();
            for (var i = 0; i < _objs2.Length/2; i++)
            {
                {
                    int r = rnd.Next(5, 50);
                    _objs2[i] = new Star(new Point(100, rnd.Next(0, Game.Height)), new Point(-r, r), new Size(3, 3));
                }
            }
            for (var i = _objs2.Length / 2; i < _objs2.Length - 1; i++)
            {
                {
                    int r = rnd.Next(70, 100);
                    _objs2[i] = new Lines(new Point(100, rnd.Next(0, Game.Height)), new Point(-r, r), new Size(100, 0));
                }
            }
            for (int i = 0; i < (_objs1.Length - 1); i++)
            {

                _objs1[i] = new Asteroid(new Point(rnd.Next(0, Game.Width), rnd.Next(0, Game.Height)), new Point(-i, -i), new Size(30, 30));

            }
        }

        public static void Buttons(Form form)
        {
            Button btn = new Button();
            btn.Width = 140;
            btn.BackColor = Color.Black;
            btn.Location = new Point(140, 300);
            btn.Image = Image.FromFile("newgame.png");
            btn.ImageAlign = ContentAlignment.MiddleRight;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.FlatStyle = FlatStyle.Flat;
            form.Controls.Add(btn);
            
            Button btn1 = new Button();
            btn1.Width = 140;
            btn1.BackColor = Color.Black;
            btn1.Location = new Point(140, 350);
            btn1.Image = Image.FromFile("records.png");
            btn1.ImageAlign = ContentAlignment.MiddleRight;
            btn1.TextAlign = ContentAlignment.MiddleLeft;
            btn1.FlatStyle = FlatStyle.Flat;
            form.Controls.Add(btn1);
            btn1.Click += btn1_Click;
            void btn1_Click(Object sender, EventArgs e)
            {
                Records recform = new Records();
            }

            Button btn2 = new Button();
            btn2.BackColor = Color.Black;
            btn2.Location = new Point(140, 400);
            btn2.Image = Image.FromFile("exit.png");
            btn2.ImageAlign = ContentAlignment.MiddleRight;
            btn2.TextAlign = ContentAlignment.MiddleLeft;
            btn2.FlatStyle = FlatStyle.Flat;
            form.Controls.Add(btn2);
            btn2.Click += btn2_Click;
            void btn2_Click(Object sender, EventArgs e)
            {
                form.Close();
            }

            btn.Click += btn_Click;
            void btn_Click(Object sender, EventArgs e)
            {
                
                form.Width = 1800;
                form.Height = 1000;
                Game.Init(form);
                form.Show();
                Game.Draw();
                Game.Load();
                btn1.Dispose();
                btn2.Dispose();
                btn.Dispose();
                _objs1 = null;

                Music Playlist = new Music();
                SoundPlayer sp = new SoundPlayer(Playlist.PlaySongNumber(1));
                sp.PlayLooping();

            }
        }
    }
}
