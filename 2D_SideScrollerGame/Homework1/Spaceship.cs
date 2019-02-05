using System;
using System.Drawing;

namespace Homework1
{
    class Spaceship : BaseObject
    {
        public static event Action<string> Status;

        public static event Message MessageDie;
        public int Energy { get; set; } = 100;

        public void EnergyLow(int n)
        {
            Energy -= n;

            Status?.Invoke($"Корабль получил урон. Энергия снизилась на {n} единиц");
        }

        public void EnergyHigh(int n)
        {
            Energy += n;
            Status?.Invoke($"Найдено топливо. Энергия повысилась на {n} единиц");
        }

        public int Score { get; set; } = 0;

        public void ScoreIncrease(int n)
        {
            Score += n;
        }

        public Spaceship(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
        }

        Image newImage3 = Image.FromFile("Ingame.png");
        public override void Draw()
        {
            Rectangle destRect = new Rectangle(Pos.X, Pos.Y, 240, 40);
            Game.Buffer.Graphics.DrawImage(newImage3, destRect);
        }

        public override void Respawn()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
        }

        public void Left()
        {
            if (Pos.X > 0) Pos.X = Pos.X - Dir.X;
        }

        public void Right()
        {
            if ((Pos.X < Game.Width) && (Pos.X >= 0))
                Pos.X = Pos.X + Dir.X;
        }
              
        public void Up()
        {
            if (Pos.Y > 0) Pos.Y = Pos.Y - Dir.Y;
        }
        
        public void Down()
        {
            if ((Pos.Y < Game.Height) && (Pos.Y > -Game.Height))
                Pos.Y = Pos.Y + Dir.Y;
        }

        public void Die()
        {
            MessageDie?.Invoke();
        }
    }
}
