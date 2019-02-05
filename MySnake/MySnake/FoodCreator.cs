using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySnake
{
    class FoodCreator
    {
        int mapWidth;
        int mapHeigth;
        char sym;

        Random random = new Random();

        public FoodCreator(int mapWidth, int mapHeigth, char sym)
        {
            this.mapWidth = mapWidth;
            this.mapHeigth = mapHeigth;
            this.sym = sym;
        }

        public Point CreateFood()
        {
            int x = random.Next(2, mapWidth - 2);
            int y = random.Next(2, mapHeigth - 2);
            return new Point(x, y, sym);
        }
    }
}
