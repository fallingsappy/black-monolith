using System;
using System.Drawing;

namespace Homework1
{
    class Bullet: BaseObject
    {
        public Bullet(Point pos, Point dir, Size size): base(pos, dir, size)
        {
        }

        Image Bulletimage = Image.FromFile("energy-orb-png-7.png");
        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(Bulletimage, Pos.X, Pos.Y, Size.Width, Size.Height);
        }

        public override void Respawn()
        {
            Pos.X = 0;
        }

        public override void Update()
        {
            Pos.X = Pos.X + 50;
        }
    }
}
