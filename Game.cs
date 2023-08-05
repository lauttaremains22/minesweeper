using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper
{
    class Game
    {
        public static Board board;
        public static int SMALL_SIZE = 10, MEDIUM_SIZE = 18, LARGE_SIZE = 27;

        public Game(int sizeX, int sizeY, int mineQ)
        {
            
            board = new Board(sizeX, sizeY, mineQ);

        }


    }
}
