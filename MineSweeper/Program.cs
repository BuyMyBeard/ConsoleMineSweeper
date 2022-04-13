using System;
using System.Diagnostics;

class Program
{
    const char BlankSpace = ' ';
    const char Undiscovered = '-';
    const char Mine = '*';
    const char Flag = '!';
    const char Unknown = '?';
    const char DownArrow = '▼';
    const char UpArrow = '▲';
    const char LeftArrow = '◄';
    const char RightArrow = '►';
    enum Direction { top, bottom, left, right };
    static bool firstTurnCompleted = false;

    static void GameLoop(ref int selectedX, ref int selectedY, char[,]game, ref int mineCount, int minesLeft, Stopwatch time, bool[,] mines, bool[,] visitedMap, int width, int height, Random rng)
    {
        bool isReading = true;
        bool gameLost = false;
        while (isReading)
        {
            ConsoleKey input = Console.ReadKey(true).Key;
            switch (input)
            {
                case ConsoleKey.Enter:
                    SwitchInfo(selectedX, selectedY, game, ref minesLeft);
                    break;

                case ConsoleKey.Spacebar:
                    gameLost = UncoverSelected(selectedX, selectedY, mines, game, visitedMap, rng);
                    break;

                case ConsoleKey.UpArrow:
                    moveCursors(ref selectedX, ref selectedY, Direction.top, game);
                    break;

                case ConsoleKey.DownArrow:
                    moveCursors(ref selectedX, ref selectedY, Direction.bottom, game);
                    break;

                case ConsoleKey.LeftArrow:
                    moveCursors(ref selectedX, ref selectedY, Direction.left, game);
                    break;

                case ConsoleKey.RightArrow:
                    moveCursors(ref selectedX, ref selectedY, Direction.right, game);
                    break;

                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;

                default:
                    break;
            }
            DisplayGame(game, minesLeft, time);
            if (gameLost)
            {               
                Console.WriteLine("BOOOOM!!!!!!");
                Environment.Exit(0);
            }
            if (CheckIfWon(visitedMap, mineCount))
            {
                Console.WriteLine("YOU WON!!!");
                Environment.Exit(0);
            }
        }
    }
    static bool[,] InitializeVisitedMap(int width, int height)
    {
        bool[,] visitedMap = new bool[height + 2, width + 2];
        for(int i = 0; i < width + 2; i++)
        {
            visitedMap[0, i] = true;
            visitedMap[height + 1, i] = true;
        }
        for (int i = 0; i < height + 2; i++)
        {
            visitedMap[i, 0] = true;
            visitedMap[i, width + 1] = true;
        }
        return visitedMap;
    }
    static bool[,] GenerateMines(int width, int height, int mineCount, Random rng)
    {
        bool[,] mines = new bool[height + 2, width + 2];
        for (int i = 1; i <= mineCount; i++)
        {
            bool valid = false;
            while (!valid)
            {
                int num1 = rng.Next(1, height + 1);
                int num2 = rng.Next(1, width + 1);
                if (!mines[num1, num2])
                {
                    mines[num1, num2] = true;
                    valid = true;
                }
            }
        }
        return mines;
    }
    static void MoveMines(int selectedX, int selectedY, bool[,] mines, Random rng)
    {
        int mineCount = 0;
        for (int i = selectedY - 1; i <= selectedY + 1; i++)
        {
            for(int j = selectedX - 1; j <= selectedX + 1; j++)
            {
                if (mines[i, j])
                {
                    mines[i, j] = false;
                    mineCount++;
                }
            }
        }
        for (int i = 1; i <= mineCount; i++)
        {
            bool valid = false;
            while (!valid)
            {
                int num1 = rng.Next(1, mines.GetLength(0) - 1);
                int num2 = rng.Next(1, mines.GetLength(1) - 1);
                bool possibleY = (num1 < selectedY - 1) || (num1 > selectedY + 1);
                bool possibleX = (num2 < selectedX - 1) || (num2 > selectedX + 1);
                if (!mines[num1, num2] && (possibleX || possibleY))
                {         
                    mines[num1, num2] = true;
                    valid = true;
                }
            }
        }
    }
    static char[,] InitializeGame(int width, int height)
    {
        char[,] game = new char[height + 2, width + 2];
        for (int i = 0; i < game.GetLength(0); i++)
        {
            for (int j = 0; j < game.GetLength(1); j++)
            {
                if (i == 0 || j == 0 || i == game.GetLength(0) - 1 || j == game.GetLength(1) - 1)
                {
                    game[i, j] = BlankSpace;
                }
                else
                {
                    game[i, j] = Undiscovered;
                }
            }
        }
        game[0, 1] = DownArrow;
        game[1, 0] = RightArrow;
        game[game.GetLength(0) - 1, 1] = UpArrow;
        game[1, game.GetLength(1) - 1] = LeftArrow;
        return game;
    }
    static void moveCursors(ref int selectedX, ref int selectedY, Direction movement, char[,]jeu)
    {
        switch (movement)
        {
            case Direction.top:
                if (selectedY - 1 > 0)
                {
                    jeu[selectedY, 0] = BlankSpace;
                    jeu[selectedY, jeu.GetLength(1) - 1] = BlankSpace;
                    selectedY--;
                    jeu[selectedY, 0] = RightArrow;
                    jeu[selectedY, jeu.GetLength(1) - 1] = LeftArrow;
                }
                break;
            case Direction.bottom:
                if (selectedY + 1 < jeu.GetLength(0) - 1)
                {
                    jeu[selectedY, 0] = BlankSpace;
                    jeu[selectedY, jeu.GetLength(1) - 1] = BlankSpace;
                    selectedY++;
                    jeu[selectedY, 0] = RightArrow;
                    jeu[selectedY, jeu.GetLength(1) - 1] = LeftArrow;
                }
                break;

            case Direction.left:
                if (selectedX - 1 > 0)
                {
                    jeu[0, selectedX] = BlankSpace;
                    jeu[jeu.GetLength(0) - 1, selectedX] = BlankSpace;
                    selectedX--;
                    jeu[0, selectedX] = DownArrow;
                    jeu[jeu.GetLength(0) - 1, selectedX] = UpArrow;
                }
                break;

            case Direction.right:
                if (selectedX + 1 < jeu.GetLength(1) - 1)
                {
                    jeu[0, selectedX] = BlankSpace;
                    jeu[jeu.GetLength(0) - 1, selectedX] = BlankSpace;
                    selectedX++;
                    jeu[0, selectedX] = DownArrow;
                    jeu[jeu.GetLength(0) - 1, selectedX] = UpArrow;
                }
                break;
        }
    }
    static void DisplayGame(char[,]game, int mineCount, Stopwatch time)
    {
        Console.Clear();
        for (int i = 0; i < game.GetLength(0); i++)
        {
            for (int j = 0; j < game.GetLength(1); j++)
            {
                char item = game[i, j];
                ApplyColor(item);
                Console.Write(item);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.WriteLine("Mine count : " + mineCount);
        Console.WriteLine("Time elapsed : " + time.Elapsed.ToString("mm\\:ss"));
    }
    static void ApplyColor(char item)
    {
        switch (item)
        {
            case '1':
                Console.ForegroundColor = ConsoleColor.Blue;
                break;

            case '2':
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                break;

            case '3':
            case Flag:
                Console.ForegroundColor = ConsoleColor.Red;
                break;

            case '4':
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                break;

            case '5':
                Console.ForegroundColor = ConsoleColor.DarkRed;
                break;

            case '6':
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;

            case '7':
                Console.ForegroundColor = ConsoleColor.Black;
                break;

            case '8':
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;

            case Mine:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;

            default:
                Console.ResetColor();
                break;
        }
    }
    static bool UncoverSelected(int selectedX, int selectedY, bool[,] mines, char[,]game, bool[,] visitedMap, Random rng)
    {
        if (!firstTurnCompleted)
        {
            MoveMines(selectedX, selectedY, mines, rng);
            firstTurnCompleted = true;
        }
        int countSurroundingMines = 0;
        if (game[selectedY, selectedX] == Flag)
        {
            return false;
        }
        if (mines[selectedY, selectedX])
        {
            
            game[selectedY, selectedX] = Mine;
            return true;
        }
        if (mines[selectedY - 1, selectedX])
        {
            countSurroundingMines++;
        }
        if (mines[selectedY - 1, selectedX + 1])
        {
            countSurroundingMines++;
        }
        if (mines[selectedY, selectedX + 1])
        {
            countSurroundingMines++;
        }
        if (mines[selectedY + 1, selectedX + 1])
        {
            countSurroundingMines++;
        }
        if (mines[selectedY + 1, selectedX])
        {
            countSurroundingMines++;
        }
        if (mines[selectedY + 1, selectedX - 1])
        {
            countSurroundingMines++;
        }
        if (mines[selectedY , selectedX - 1])
        {
            countSurroundingMines++;
        }
        if (mines[selectedY - 1, selectedX - 1])
        {
            countSurroundingMines++;
        }
        visitedMap[selectedY, selectedX] = true;
        if (countSurroundingMines == 0)
        {
            game[selectedY, selectedX] = BlankSpace;
            if (!visitedMap[selectedY - 1, selectedX])
            {
                UncoverSelected(selectedX, selectedY - 1, mines, game, visitedMap, rng);
            }
            if (!visitedMap[selectedY + 1, selectedX])
            {
                UncoverSelected(selectedX, selectedY + 1, mines, game, visitedMap, rng);
            }
            if (!visitedMap[selectedY, selectedX - 1])
            {
                UncoverSelected(selectedX - 1, selectedY, mines, game, visitedMap, rng);
            }
            if (!visitedMap[selectedY, selectedX + 1])
            {
                UncoverSelected(selectedX + 1, selectedY, mines, game, visitedMap, rng);
            }
            if (!visitedMap[selectedY - 1, selectedX - 1])
            {
                UncoverSelected(selectedX - 1, selectedY - 1, mines, game, visitedMap, rng);
            }
            if (!visitedMap[selectedY - 1, selectedX + 1])
            {
                UncoverSelected(selectedX + 1, selectedY - 1, mines, game, visitedMap, rng);
            }
            if (!visitedMap[selectedY + 1, selectedX - 1])
            {
                UncoverSelected(selectedX - 1, selectedY + 1, mines, game, visitedMap, rng);
            }
            if (!visitedMap[selectedY + 1, selectedX + 1])
            {
                UncoverSelected(selectedX + 1, selectedY + 1, mines, game, visitedMap, rng);
            }
        }
        else
        {
            game[selectedY, selectedX] = countSurroundingMines.ToString()[0];
        }
        return false;
    }   
    static void SwitchInfo(int selectedX, int selectedY, char[,] game, ref int mineCount)
    {
        string informationTypes = Undiscovered.ToString() + Flag.ToString() + Unknown.ToString();
        char information = game[selectedY, selectedX];
        if (informationTypes.Contains(information))
        {          
            int i = informationTypes.IndexOf(information);
            i = (i + 1) % 3;         
            game[selectedY, selectedX] = informationTypes[i];
            if (informationTypes[i] == Flag)
            {
                mineCount--;
            }
            else if (informationTypes[i] == Unknown)
            {
                mineCount++;
            }
        }
    }
    static bool CheckIfWon(bool[,] visitedMap, int mineCount)
    {
        int unvisitedCount = 0;
        for (int i = 0; i < visitedMap.GetLength(0); i++)
        {
            for (int j = 0; j < visitedMap.GetLength(1); j++)
            {
                if (!visitedMap[i, j])
                {
                    unvisitedCount++;
                }              
            }
        }
        return unvisitedCount <= mineCount;      
    }
    static void DisplayMenu()
    {
        Console.WriteLine("[1] Beginner");
        Console.WriteLine("[2] Intermediate");
        Console.WriteLine("[3] Expert");
        Console.WriteLine("[4] Custom");
        Console.WriteLine("[5] How to play");
    }
    static void DisplayInstructions()
    {
        Console.WriteLine("Arrow keys to move");
        Console.WriteLine("Spacebar to uncover a cell");
        Console.WriteLine("Enter to flag a cell");
        Console.WriteLine("Escape to stop the game");
    }
    static int TakeValidInput(int min, int max)
    {
        while (true)
        {
            int input;
            string rawInput = Console.ReadLine();
            if (int.TryParse(rawInput, out input))
            {
                if (input >= min && input <= max)
                {
                    return input;
                }
            }
            Console.WriteLine("Enter a number between {0} and {1}", min, max);
        }
    }
    static void DefineGameAttributes(out int width, out int height, out int mineCount)
    {
        
        switch (TakeValidInput(1, 5))
        {
            case 1:
                width = 9;
                height = 9;
                mineCount = 10;
                break;

            case 2:
                width = 16;
                height = 16;
                mineCount = 40;
                break;

            case 3:
                width = 30;
                height = 16;
                mineCount = 99;
                break;

            case 4:
                Console.Write("Width : ");
                width = TakeValidInput(5, 99);
                Console.Write("Height : ");
                height = TakeValidInput(5, 99);
                int maxMines = width * height - 9;
                Console.Write("Mines : ");
                mineCount = TakeValidInput(1,maxMines);
                break;

            case 5:
                Console.Clear();
                DisplayInstructions();
                Console.ReadLine();
                Console.Clear();
                DisplayMenu();
                DefineGameAttributes(out width, out height, out mineCount);
                break;

            default:
                width = 9;
                height = 9;
                mineCount = 10;
                break;
        }
    }
    static void Main(string[] args)
    {
        int width;
        int height;
        int mineCount;
        Random rng = new Random();
        DisplayMenu();
        DefineGameAttributes(out width, out height, out mineCount);
        Stopwatch time = new Stopwatch();
        time.Start();
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        int minesLeft = mineCount;
        int selectedX = 1;
        int selectedY = 1;
        // Each of the 3 2d arrays has a width and height of width + 2 and height + 2. This creates a margin for the display of selector cursor
        // and easier management of looking what is around a square, without going out of boundaries
        char[,] game = InitializeGame(width, height); 
        bool[,] mines = GenerateMines(width, height, mineCount, rng);
        bool[,] visitedMap = InitializeVisitedMap(width, height);        
        DisplayGame(game, minesLeft, time);
        GameLoop(ref selectedX, ref selectedY, game, ref mineCount, minesLeft, time, mines, visitedMap, width, height, rng);       
    }
}

