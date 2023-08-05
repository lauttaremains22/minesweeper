using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper
{
    static class Initializer
    {
        public static void InitGame()
        {
            Console.WriteLine("Choose a board x size (namely 9, 8, 17..): ");
            bool trySizeX = Int32.TryParse(Console.ReadLine().Replace(" ", ""), out int sizeX);

            Console.WriteLine("Choose a board y size (namely 9, 8, 17..): ");
            bool trySizeY = Int32.TryParse(Console.ReadLine().Replace(" ", ""), out int sizeY);

            Console.WriteLine("Choose the number of mines: ");
            bool tryMines = Int32.TryParse(Console.ReadLine().Replace(" ", ""), out int mines);

            Game game = new Game(sizeX, sizeY, mines);
        }
    }
}
