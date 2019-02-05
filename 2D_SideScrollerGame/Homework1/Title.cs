using System;
using System.Drawing;

namespace Homework1
{
    class Title : BaseObject
    {
        Image newImage = Image.FromFile("pixel_space_battleship_wip_by_prinzeugn-d6olizl1.png");
        Image newImage2 = Image.FromFile("arcade-font-writer.png");
        Image newImageP = Image.FromFile("57fda19714441a5e1948c01ca94d8f41-dau0rbc.png");
        Image newImage1P = Image.FromFile("planets-transparent-pixel-art-6.png");
        Image newImage1P1 = Image.FromFile("tumblr_on0bwgF7LC1qmf2zwo1_r1_540.png");
        Image newGame = Image.FromFile("newgame.png");
        Image records = Image.FromFile("records.png");
        Image exit = Image.FromFile("exit.png");

        public override void Draw()
        {




            Point ulCorner11 = new Point(0, 0);
            Point urCorner11 = new Point(800, 0);
            Point llCorner11 = new Point(0, 800);
            Point[] destPara1 = { ulCorner11, urCorner11, llCorner11 };
            Game.Buffer.Graphics.DrawImage(newImage1P1, destPara1);

            Point ulCorner1 = new Point(300, 300);
            Point urCorner1 = new Point(400, 300);
            Point llCorner1 = new Point(300, 400);
            Point[] destPara = { ulCorner1, urCorner1, llCorner1 };
            Game.Buffer.Graphics.DrawImage(newImage1P, destPara);

            Point ulCorner = new Point(490, 70);
            Game.Buffer.Graphics.DrawImage(newImageP, ulCorner);

            Point ulCorner2 = new Point(50, 200);
            Game.Buffer.Graphics.DrawImage(newImage2, ulCorner2);

            Rectangle destRect = new Rectangle(10, 60, 450, 100);
            Game.Buffer.Graphics.DrawImage(newImage, destRect);






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
