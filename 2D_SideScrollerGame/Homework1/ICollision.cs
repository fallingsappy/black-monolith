using System;
using System.Drawing;

namespace Homework1
{
    interface ICollision
    {
        bool Collision(ICollision obj);
        Rectangle Rect { get; }
    }
}
