using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Media;
using System.Threading;

namespace Homework1
{
    public class GameObjectException : Exception
    {
        public GameObjectException()
        {
            MessageBox.Show("Некоректные характеристики объекта", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }
    }
    static class Game
    {
        private static BufferedGraphicsContext _context;
        public static BufferedGraphics Buffer;
        public static int Width { get; set; }
        public static int Height { get; set; }



        static Game()
        {
        }

        private static System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer { Interval = 100 };
        public static Random Rnd = new Random();

        public static void Finish()
        {
            _timer.Stop();
            Buffer.Graphics.DrawString("The End", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline), Brushes.White, Width/2, Height/2);
            Buffer.Render();
        }

        public static void Init(Form form)
        {
            Asteroid.Status += Log.Sub;
            Spaceship.Status += Log.Sub;
            Spaceship.MessageDie += Finish;
            Graphics g;
            _context = BufferedGraphicsManager.Current;
            g = form.CreateGraphics();
            try
            {
                Width = form.ClientSize.Width;
                Height = form.ClientSize.Height;
                if ((Width < 0 || Width > 1800) || (Height < 0 || Height > 1000))               
                    throw new ArgumentOutOfRangeException();
                
            }
            catch(ArgumentOutOfRangeException)
            {
                MessageBox.Show("Размер игрового окна имеет неверный формат", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            Buffer = _context.Allocate(g, new Rectangle(0, 0, Width, Height));

            _timer.Start();
            _timer.Tick += Timer_Tick;
            SplashScreen.LoadSplash();
            form.KeyDown += Form_KeyDown;
        }

        private static void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) _bullet.Add(new Bullet(new Point(_ship.Rect.X + 120, _ship.Rect.Y+15), new Point(4,0), new Size(40,35)));
            if (e.KeyCode == Keys.Up) _ship.Up();
            if (e.KeyCode == Keys.Down) _ship.Down();
            if (e.KeyCode == Keys.Left) _ship.Left();
            if (e.KeyCode == Keys.Right) _ship.Right();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Draw();
            Update();
        }

        public static void Draw()
        {

            Buffer.Graphics.Clear(Color.Black);
            if (_powerUp != null)
                _powerUp.Draw();
            foreach (BaseObject ob2 in SplashScreen._objs2)
                ob2.Draw();
            if(SplashScreen._objs1 != null)
                foreach (BaseObject ob in SplashScreen._objs1)
                    ob.Draw();
            if( _asteroids != null)
                foreach (Asteroid a in _asteroids)
                {
                    try
                    {
                        a?.Draw();
                    }
                    catch (GameObjectException)
                    {
                        Environment.Exit(1);
                    }
                }
            try
            {
                if (_bullet != null)
                    foreach(Bullet b in _bullet)
                        b?.Draw();
            }
            catch(GameObjectException)
            {
                Environment.Exit(1);
            }
            if (_ship != null)
            {
                _ship?.Draw();
                Buffer.Graphics.DrawString("Score:"  +_ship.Score, SystemFonts.DefaultFont, Brushes.White, 100, 0);
                Buffer.Graphics.DrawString("Energy:" + _ship.Energy, SystemFonts.DefaultFont, Brushes.White, 0, 0);
            }

            Buffer.Render();
        }

        public static void Update()
        {
            if (KillerCount == Count)
            {
                NextLevel();
            }
            if (_powerUp != null)
            {
                _powerUp.Update();
                if (_ship.Collision(_powerUp))
                {
                    _ship.ScoreIncrease(100);
                    var rnd1 = new Random();
                    if (_ship.Energy < 90)
                        _ship?.EnergyHigh(10);
                    else
                        _ship.Energy = 100;
                    
                    _powerUp = null;
                    _powerUp = new PowerUp(new Point(rnd1.Next(30, Game.Width-30), rnd1.Next(30, Game.Height-30)), new Point(0, 0), new Size(25, 25));
                    Log.PrintDb();
                }
            }

            foreach (BaseObject ob2 in SplashScreen._objs2)
                ob2.Update();
            if (SplashScreen._objs1 != null)
                foreach (BaseObject ob in SplashScreen._objs1)
                    ob.Update();
            if (_bullet != null)
                foreach(Bullet b in _bullet)
                    b.Update();
            if (_ship != null)
                _ship.Update();

            if (_asteroids != null)
            {
                for (var i = 0; i < _asteroids.Count; i++)
                {
                    if (_asteroids[i] == null) continue;
                    _asteroids[i].Update();
                    for (int j = 0; j < _bullet.Count; j++)
                        if (_asteroids[i] != null && _bullet[j].Collision(_asteroids[i]))
                            {
                                _ship.ScoreIncrease(500);
                                //System.Media.SystemSounds.Hand.Play();
                                _asteroids[i].AsteroidIsDead();
                                Log.PrintDb();
                                _asteroids[i] = null;
                                _bullet.RemoveAt(j);
                                j--;
                                KillerCount++;
                            }
                    if (_asteroids[i] == null || !_ship.Collision(_asteroids[i])) continue;
                    {
                        var rnd = new Random();
                        _ship?.EnergyLow(rnd.Next(1, 10));
                        Log.PrintDb();
                        //System.Media.SystemSounds.Asterisk.Play();
                    }
                    if (_ship.Energy <= 0)
                    {
                        PlayerName player = new PlayerName();
                        player.GameOver(_ship.Score);
                        _ship?.Die();
                    }

                }
            }
        }

        private static PowerUp _powerUp;
        private static Spaceship _ship;
        private static List<Bullet> _bullet = new List<Bullet>();
        public static List<Asteroid> _asteroids = new List<Asteroid>();
        public static int Count = 5, KillerCount = 0;

        public static void Load()
        {
            var rnd = new Random();
            _powerUp = new PowerUp(new Point(rnd.Next(0, Game.Width), rnd.Next(0, Game.Height)), new Point(0, 0), new Size(25, 25));
            _ship = new Spaceship(new Point(0, Game.Height/2), new Point(15, 15), new Size(10, 10));
            for (var i = 0; i < Count; i++)
            {
                int r = rnd.Next(5, 50);
                _asteroids.Add(new Asteroid(new Point(rnd.Next(Game.Width/2, Game.Width), rnd.Next(0, Game.Height)), new Point(-r / 3, r/3), new Size(50, 50)));
            }
        }

        public static void NextLevel()
        {
            Buffer.Graphics.DrawString("Следующий уровень!", new Font(FontFamily.GenericSansSerif, 60, FontStyle.Underline), Brushes.White, 500, Height / 2);
            Buffer.Render();
            Thread.Sleep(2000);
            KillerCount = 0;
            Count++;
            var rnd = new Random();
            for (var i = 0; i < Count; i++)
            {
                int r = rnd.Next(5, 50);
                _asteroids.Add(new Asteroid(new Point(rnd.Next(Game.Width / 2, Game.Width), rnd.Next(0, Game.Height)), new Point(-r / 3, r / 3), new Size(50, 50)));
            }
        }
    }
}
