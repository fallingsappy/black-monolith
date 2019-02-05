using System;
using System.Drawing;

namespace Homework1
{
    public delegate void Message();

    abstract class BaseObject : ICollision
    {
        public Point Pos;
        protected Point Dir;
        protected Size Size;

        protected BaseObject(Point pos, Point dir, Size size)
        {
            Pos = pos;
            Dir = dir;
            Size = size;
        }

        public BaseObject()
        {
        }

        public abstract void Respawn();

        public abstract void Draw();
        public abstract void Update();

        public bool Collision(ICollision o) => o.Rect.IntersectsWith(this.Rect);

        public Rectangle Rect => new Rectangle(Pos, Size);
    }
}
