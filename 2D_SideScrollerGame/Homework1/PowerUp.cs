using System;
using System.Drawing;

namespace Homework1
{
    class PowerUp : BaseObject
    {
        public PowerUp(Point pos, Point dir, Size size) : base(pos, dir, size)
        {
        }

        Image powerUp = Image.FromFile("pixel-circle-png.png");

        public override void Draw()
        {
            Game.Buffer.Graphics.DrawImage(powerUp, Pos.X, Pos.Y, 50, 50);
        }

        public override void Respawn()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
        }
    }
}
