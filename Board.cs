using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper
{
    class Board
    {
        private const int CORNER_MINE_CHECK = 3, BORDER_MINE_CHECK = 5, MIDDLE_MINE_CHECK = 8;

        private string[,] _mainBoard, _coveredBoard;
        private bool[,] _visibilityBoard, _coreIterations;
        private int mineQuantity, minesFlagged, availableFlags, sizeX, sizeY, mineQ, uncoveredCells;
        private bool displayMinesFlaggedMessage = false;

        public Board(int sizeX, int sizeY, int mineQ)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.mineQ = mineQ;

            _coreIterations = new bool[sizeX, sizeY];
            _visibilityBoard = new bool[sizeX, sizeY];
            Populator.Populate(_coreIterations, false);
            Populator.Populate(_visibilityBoard, false);

            _mainBoard = new string[sizeX, sizeY];
            availableFlags = mineQuantity = mineQ;
            Populator.Populate(_mainBoard, "");

            init();

            _coveredBoard = new string[sizeX, sizeY];
            displayCovered();
        }


        // IMPORTANT: This method must always be invoked after the main board initialization.

        private void displayCovered(bool ignoreInput = false)
        {

            Console.WriteLine("\nMines flagged: " + minesFlagged + " -- Mines quantity: " + mineQuantity + " -- Available flags: " + availableFlags + "\n");

            if (minesFlagged == mineQuantity && displayMinesFlaggedMessage)
            {
                Console.WriteLine("Congrats!, you've flagged all the mines :D. Now just uncover all the other cells to win the game.\n");
                displayMinesFlaggedMessage = false;
            }

            for (int i = 0; i < _coveredBoard.GetLength(0); i++)
            {
                for (int j = 0; j < _mainBoard.GetLength(1); j++)
                {
                    if (_coveredBoard[i, j] == null || _coveredBoard[i, j] == "[ ]")
                    {
                        _coveredBoard[i, j] = "[ ]";
                        Console.Write(_coveredBoard[i, j]);
                    }
                    else
                    {
                        Console.Write("[" + _coveredBoard[i, j] + "]");
                    }
                }

                Console.WriteLine("");
            }

            if (ignoreInput) return;

            Console.WriteLine("\nWrite an x,y coordinate (namely: 0 3, 2 7, 3 1, etc..)\nWrite the coord. with an 'F' at the end to place a flag. ");

            try
            {
                string[] input = Console.ReadLine().Split(" ");

                inputProcessor(input, _coveredBoard, _mainBoard);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n");
                Console.ReadKey();
                displayCovered();
            }

            Console.ReadKey();

        }

        // This function must be recursive by calling the 'displayCovered' one. 
        // Here, the user's input is processed and a new display of the board is returned to the method that invoked
        // this function.

        private void inputProcessor(string[] input, string[,] mainBoard, string[,] coveredBoard)
        {

            int x = Int32.Parse(input[0]), y = Int32.Parse(input[1]);
            string flag = "";

            if (input.Length == 3)
            {
                flag = input[2].ToUpper();
            }

            if (_coveredBoard[x, y].Equals("F"))
            {

                if (flag.Equals("F")) Console.WriteLine("\nWARNING! You're trying to place a flag in a cell that already have one." +
                            " That instruction will be ignored and the flag removed.");

                _coveredBoard[x, y] = "[ ]";

                if (_mainBoard[x, y].Equals("0")) minesFlagged--;

                availableFlags++;
                Console.WriteLine("");
                displayCovered();
                //return;

            }

            if (flag.Equals("F") && !_coveredBoard[x, y].Equals("F"))
            {

                availableFlags--;
                _coveredBoard[x, y] = flag;

                if (_mainBoard[x, y].Equals("0"))
                {
                    minesFlagged++;
                    displayMinesFlaggedMessage = true;
                }
            }
            else
            {
            

                if (_mainBoard[x, y].Equals("-"))
                {
                    Reveal(x, y);
                }
                else
                {
                    uncoveredCells++;
                    _visibilityBoard[x, y] = true;
                    _coveredBoard[x, y] = _mainBoard[x, y];
                }

                if (uncoveredCells == (sizeX * sizeY) - mineQ)
                {
                    Console.WriteLine("You won the game!\n¿Do you want to restart? (Y/N)");
                    string resp = Console.ReadLine().ToUpper();
                    RestartBoard(resp);
                }
               
            }

            if (_mainBoard[x, y].Equals("0") && flag != "F")
            {
                Console.WriteLine("Oh no! You stepped on a mine :(\n¿Do you want to restart the game? (Y/N)");
                string resp = Console.ReadLine().ToUpper();
                RestartBoard(resp);
            }

            Console.WriteLine("");
            displayCovered();
        }

        private void init()
        {

            int counter = 1;

            while (counter <= mineQuantity)
            {
                Random mineGenerator = new Random();
                int mine = mineGenerator.Next(0, 1);

                if (mine == 0)
                {
                    Random indexGenerator = new Random();
                    int xIndex = indexGenerator.Next(0, _mainBoard.GetLength(0) - 1);
                    int yIndex = indexGenerator.Next(0, _mainBoard.GetLength(1) - 1);

                    if (_mainBoard[xIndex, yIndex] == "")
                    {
                        _mainBoard[xIndex, yIndex] = mine.ToString();
                        counter++;
                    }
                }

            }

            for (int i = 0; i < _mainBoard.GetLength(0); i++)
            {
                for (int j = 0; j < _mainBoard.GetLength(1); j++)
                {
                    /*if(i == 0 || i == _board.GetLength(0) && j == 0 || j == _board.GetLength(1))
                    {
                    }*/

                    // START: Corner numbers filling

                    if (_mainBoard[i, j] == "0")
                    {
                        Console.Write(_mainBoard[i, j]);
                        continue;
                    }

                    int number;

                    // TOP LEFT CORNER --

                    if (i == 0 && j == 0)
                    {

                        number = 0;

                        if (_mainBoard[i, j + 1].Equals("0")) number++;

                        if (_mainBoard[i + 1, j].Equals("0")) number++;

                        if (_mainBoard[i + 1, j + 1].Equals("0")) number++;

                        _mainBoard[i, j] = number == 0 ? "-" : number.ToString();
                        Console.Write(_mainBoard[i, j]);

                    }

                    // TOP RIGHT CORNER --

                    else if (i == 0 && j == _mainBoard.GetLength(1) - 1)
                    {

                        number = 0;

                        if (_mainBoard[i, j - 1].Equals("0")) number++;

                        if (_mainBoard[i + 1, j].Equals("0")) number++;

                        if (_mainBoard[i + 1, j - 1].Equals("0")) number++;

                        _mainBoard[i, j] = number == 0 ? "-" : number.ToString();
                        Console.Write(_mainBoard[i, j]);

                    }

                    // BOTTOM LEFT CORNER --

                    else if (i == _mainBoard.GetLength(0) - 1 && j == 0)
                    {

                        number = 0;

                        if (_mainBoard[i - 1, j].Equals("0")) number++;

                        if (_mainBoard[i - 1, j + 1].Equals("0")) number++;

                        if (_mainBoard[i, j + 1].Equals("0")) number++;

                        _mainBoard[i, j] = number == 0 ? "-" : number.ToString();
                        Console.Write(_mainBoard[i, j]);

                    }

                    // BOTTOM RIGHT CORNER --

                    else if (i == _mainBoard.GetLength(0) - 1 && j == _mainBoard.GetLength(1) - 1)
                    {

                        number = 0;

                        if (_mainBoard[i - 1, j].Equals("0")) number++;

                        if (_mainBoard[i - 1, j - 1].Equals("0")) number++;

                        if (_mainBoard[i, j - 1].Equals("0")) number++;

                        _mainBoard[i, j] = number == 0 ? "-" : number.ToString();
                        Console.Write(_mainBoard[i, j]);

                    }

                    // END: Corner numbers filling

                    // START: Border numbers filling

                    // TOP SIDE BORDER --

                    else if (i == 0 && j != 0 && j != _mainBoard.GetLength(1) - 1)
                    {

                        number = 0;
                        int indexCheckTB = j - 1, k = i;

                        for (int c = j - 1; c <= j - 1 + BORDER_MINE_CHECK; c++)
                        {

                            if (c == j) continue;

                            if (c < j) if (_mainBoard[i, c].Equals("0")) number++;

                            if (c == j + 1)
                            {
                                if (_mainBoard[i, c].Equals("0")) number++;
                                k += 1;
                                continue;
                            }

                            if (c > j)
                            {
                                if (_mainBoard[k, indexCheckTB].Equals("0")) number++;

                                if (indexCheckTB == j + 1) continue;

                                indexCheckTB++;
                            }

                        }

                        _mainBoard[i, j] = number == 0 ? "-" : number.ToString();
                        Console.Write(_mainBoard[i, j]);

                    }

                    // LEFT SIDE BORDER --

                    else if (i != 0 && i != _mainBoard.GetLength(0) - 1 && j == 0)
                    {

                        number = 0;
                        int indexCheckLB = i - 1, k = j;

                        for (int c = i - 1; c <= i - 1 + BORDER_MINE_CHECK; c++)
                        {

                            if (c == i) continue;

                            if (c < i) if (_mainBoard[c, j].Equals("0")) number++;

                            if (c == i + 1)
                            {
                                k += 1;
                                if (_mainBoard[c, j].Equals("0")) number++;
                                continue;
                            }

                            if (c > i)
                            {

                                if (_mainBoard[indexCheckLB, k].Equals("0")) number++;

                                if (indexCheckLB == i + 1) continue;

                                indexCheckLB++;

                            }
                        }

                        _mainBoard[i, j] = number == 0 ? "-" : number.ToString();
                        Console.Write(_mainBoard[i, j]);

                    }

                    // BOTTOM SIDE BORDER --

                    else if (i == _mainBoard.GetLength(0) - 1 && j != 0 && j != _mainBoard.GetLength(1) - 1)
                    {

                        number = 0;
                        int indexCheckBB = j - 1, k = i;

                        for (int c = j - 1; c <= j - 1 + BORDER_MINE_CHECK; c++)
                        {

                            if (c == j) continue;

                            if (c < j) if (_mainBoard[i, c].Equals("0")) number++;

                            if (c == j + 1)
                            {
                                k -= 1;
                                if (_mainBoard[i, c].Equals("0")) number++;
                                continue;
                            }


                            if (c > j)
                            {

                                if (_mainBoard[k, indexCheckBB].Equals("0")) number++;

                                if (indexCheckBB == j + 1) continue;

                                indexCheckBB++;

                            }

                        }

                        _mainBoard[i, j] = number == 0 ? "-" : number.ToString();
                        Console.Write(_mainBoard[i, j]);

                    }

                    // RIGHT SIDE BORDER --

                    else if (i != 0 && i != _mainBoard.GetLength(0) - 1 && j == _mainBoard.GetLength(1) - 1)
                    {

                        number = 0;
                        int indexCheckRB = i - 1, k = j;

                        for (int c = i - 1; c <= i - 1 + BORDER_MINE_CHECK; c++)
                        {

                            if (c == i) continue;

                            if (c < j) if (_mainBoard[c, j].Equals("0")) number++;

                            if (c == i + 1)
                            {
                                k -= 1;
                                if (_mainBoard[c, j].Equals("0")) number++;
                                continue;
                            }

                            if (c > i)
                            {

                                if (_mainBoard[indexCheckRB, k].Equals("0")) number++;

                                if (indexCheckRB == i + 1) continue;

                                indexCheckRB++;


                            }
                        }

                        _mainBoard[i, j] = number == 0 ? "-" : number.ToString();
                        Console.Write(_mainBoard[i, j]);

                    }

                    // END: Border numbers filling

                    // START: Middle numbers filling

                    else
                    {
                        number = 0;
                        int indexCheck = j - 1, k = i - 1;

                        for (int c = indexCheck; c <= indexCheck + MIDDLE_MINE_CHECK; c++)
                        {

                            if ((c == j && k - i == 1) || (k == i + 2 || indexCheck == j + 2)) continue;

                            if (c <= j) if (_mainBoard[i - 1, c].Equals("0")) number++;

                            if (c == j + 1 || c == j + 4)
                            {
                                if (_mainBoard[k, indexCheck].Equals("0")) number++;
                                indexCheck = j - 1;
                                k += 1;
                                continue;
                            }

                            if (c > j)
                            {

                                if (_mainBoard[k, indexCheck].Equals("0")) number++;

                                if (indexCheck == j + 4) continue;

                            }

                            indexCheck++;

                        }

                        _mainBoard[i, j] = number == 0 ? "-" : number.ToString();
                        Console.Write(_mainBoard[i, j]);

                    }

                    // END: Middle numbers filling

                }

                Console.WriteLine("");
            }

            Console.ReadKey();
        }

        private void Reveal(int x, int y)
        {

            Console.WriteLine("this coordinate is core: " + x + " " + y);
            _coreIterations[x, y] = true;

            int minx = (x <= 0 ? 0 : x - 1);
            int miny = (y <= 0 ? 0 : y - 1);
            int maxx = (x >= _mainBoard.GetLength(0) - 1 ? _mainBoard.GetLength(0) - 1 : x + 1);
            int maxy = (y >= _mainBoard.GetLength(1) - 1 ? _mainBoard.GetLength(1) - 1 : y + 1);

            int mineCounter = 0;
            bool assigned = false;
            string nextCoordinates = "";

            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {

                    Console.WriteLine("cell " + i + " " + j + "visibility: " + _visibilityBoard[i, j]);

                    if (_visibilityBoard[i, j]) continue;

                    mineCounter = closeMines(i, j);

                    if (_coveredBoard[i, j].Equals("[ ]")) uncoveredCells++;

                    if (mineCounter == 0)
                    {
                        if (!_visibilityBoard[i, j] && !assigned)
                        {
                            nextCoordinates = i.ToString() + j.ToString();
                            assigned = true;
                        }
                        _coveredBoard[i, j] = "-";
                    }
                    else
                    {
                        _coveredBoard[i, j] = mineCounter.ToString();
                    }

                    _visibilityBoard[i, j] = true;               

                    if (i == maxx && j == maxy)
                    {

                        if (!nextCoordinates.Equals(""))
                        {
                            Console.WriteLine("next coordinates: " + nextCoordinates.Substring(0, 1) + " " + nextCoordinates.Substring(1));
                            Reveal(Int32.Parse(nextCoordinates.Substring(0, 1)),
                                   Int32.Parse(nextCoordinates.Substring(1)));
                        }

                    }

                    displayCovered(true); /*true -> IGNORE INPUT*/

                }

            }

            checkCoreIterations();
            displayCovered();

        }


        private int closeMines(int x, int y)
        {

            int minx = (x <= 0 ? 0 : x - 1);
            int miny = (y <= 0 ? 0 : y - 1);
            int maxx = (x >= _mainBoard.GetLength(0) - 1 ? _mainBoard.GetLength(0) - 1 : x + 1);
            int maxy = (y >= _mainBoard.GetLength(1) - 1 ? _mainBoard.GetLength(1) - 1 : y + 1);

            int result = 0;

            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {
                    if (_mainBoard[i, j].Equals("0"))
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        private void checkCoreIterations()
        {
            for (int ii = 0; ii < _coveredBoard.GetLength(0); ii++)
            {
                for (int jj = 0; jj < _coveredBoard.GetLength(1); jj++)
                {
                    if (_coveredBoard[ii, jj].Equals("-") && !_coreIterations[ii, jj])
                    {
                        Console.WriteLine("not core found, coordinates: " + ii + " " + jj);
                        Reveal(ii, jj);
                    }
                }
            }
        }

        private void RestartBoard(string input)
        {
            if (input.Equals("Y"))
            {
                Initializer.InitGame();
            }
            else
            {
                System.Environment.Exit(0);
            }
        }

    }

    static class Populator
    {
        public static void Populate<T>(this T[,] arr, T value)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = value;
                }

            }
        }
    }

}

