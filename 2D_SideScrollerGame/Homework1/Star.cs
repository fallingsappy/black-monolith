using System;
using System.Drawing;

namespace Homework1
{
    class Star: BaseObject
    {
        public Star(Point pos, Point dir, Size size):base(pos,dir,size)
        {
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawLine(Pens.White, Pos.X, Pos.Y, Pos.X + Size.Width, Pos.Y + Size.Height);
            Game.Buffer.Graphics.DrawLine(Pens.White, Pos.X + Size.Width, Pos.Y, Pos.X, Pos.Y + Size.Height);
            // Еще больше звезд
            Random rnd = new Random();
            for (int i = 0; i < 20; i++)
                Game.Buffer.Graphics.DrawEllipse(Pens.White, rnd.Next(0, Game.Width), rnd.Next(0, Game.Height), 1, 1);
        }

        public override void Respawn()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            Pos.X = Pos.X + Dir.X;
            if (Pos.X < 0) Pos.X = Game.Width;
            if (Pos.X > Game.Width) Pos.X = 0 + Size.Width;
        }
    }
}
