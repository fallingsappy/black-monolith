using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySnake
{
    class Walls
    {
        List<Figure> wallList;

        public Walls(int mapWidth, int mapHeigth)
        {
            wallList = new List<Figure>();

            //Отрисовка рамочки
            HorizontalLine upLine = new HorizontalLine(1, mapWidth - 3, 0, '_');
            HorizontalLine downLine = new HorizontalLine(1, mapWidth - 2, mapHeigth - 1, '_');
            VerticalLine leftLine = new VerticalLine(1, mapHeigth - 1, 0, '|');
            VerticalLine rightLine = new VerticalLine(1, mapHeigth - 1, mapWidth - 2, '|');

            wallList.Add(upLine);
            wallList.Add(downLine);
            wallList.Add(leftLine);
            wallList.Add(rightLine);
        }

        internal bool IsHit(Figure figure)
        {
            foreach(var wall in wallList)
            {
                if(wall.IsHit(figure))
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw()
        {
            foreach(var wall in wallList)
            {
                wall.Draw();
            }
        }
    }
}
