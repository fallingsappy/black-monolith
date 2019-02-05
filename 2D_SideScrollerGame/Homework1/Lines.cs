using System;
using System.Drawing;


namespace Homework1
{
    class Lines : BaseObject
    {

        public Lines(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
        }

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawLine(Pens.White, Pos.X, Pos.Y, Pos.X + Size.Width, Pos.Y + Size.Height);
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
