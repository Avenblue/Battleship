using System;
using System.Threading;
using System.Collections.Generic;
class Program
{
    static char[,] CreateBoard()
    {
        char[,] myArray = new char[10, 10];

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                myArray[i, j] = '-';
            }
        }

        return myArray;
    }

    static void PrintBoard(char[,] array, string player)
    {
        Console.WriteLine(player);
        Console.WriteLine("    1 2 3 4 5 6 7 8 9 10");
        Console.WriteLine("  +---------------------+");
        char letter = '@';
        for (int i = 0; i < array.GetLength(0); i++)
        {
            char nextLetter = (char)(letter + 1);
            Console.Write(nextLetter + " | ");
            letter = nextLetter;
            for (int j = 0; j < array.GetLength(1); j++)
            {
                Console.Write(array[i, j] + " ");
            }
            Console.Write("|");
            Console.WriteLine();
        }
        Console.WriteLine("  +---------------------+");
        Console.WriteLine();
    }

    static (int, int) RandomCoordinate()
    {
        Random random = new Random();
        int ycoordinate = random.Next(10);
        int xcoordinate = random.Next(10);
        return (xcoordinate, ycoordinate);
    }

    static void ShuffleArray(int[] array)
    {
        Random random = new Random();
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int j = i + random.Next(n - i);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    static void PlaceShip(char[,] array, int shipSize)
    {
        int xcoordinate,
            ycoordinate;

        while (true)
        {
            // Generate random coordinates for each attempt
            (xcoordinate, ycoordinate) = RandomCoordinate();

            // Generate an array of directions (0=left, 1=up, 2=right, 3=down) in random order
            int[] directions = { 0, 1, 2, 3 };
            ShuffleArray(directions);

            // Initialize a flag to track whether a valid placement is found for this attempt
            bool validPlacementFound = false;

            // Iterate through the shuffled directions
            foreach (int direction in directions)
            {
                // Check if the ship can fit in the chosen direction
                bool canFit = true;

                switch (direction)
                {
                    case 0: // Left
                        if (xcoordinate - shipSize + 1 < 0)
                            canFit = false;
                        else
                        {
                            for (int x = xcoordinate; x > xcoordinate - shipSize; x--)
                            {
                                if (array[x, ycoordinate] == 'O' || array[x, ycoordinate] == '*')
                                {
                                    canFit = false;
                                    break;
                                }
                            }
                        }
                        break;

                    case 1: // Up
                        if (ycoordinate - shipSize + 1 < 0)
                            canFit = false;
                        else
                        {
                            for (int y = ycoordinate; y > ycoordinate - shipSize; y--)
                            {
                                if (array[xcoordinate, y] == 'O' || array[xcoordinate, y] == '*')
                                {
                                    canFit = false;
                                    break;
                                }
                            }
                        }
                        break;

                    case 2: // Right
                        if (xcoordinate + shipSize - 1 >= array.GetLength(0))
                            canFit = false;
                        else
                        {
                            for (int x = xcoordinate; x < xcoordinate + shipSize; x++)
                            {
                                if (array[x, ycoordinate] == 'O' || array[x, ycoordinate] == '*')
                                {
                                    canFit = false;
                                    break;
                                }
                            }
                        }
                        break;

                    case 3: // Down
                        if (ycoordinate + shipSize - 1 >= array.GetLength(1))
                            canFit = false;
                        else
                        {
                            for (int y = ycoordinate; y < ycoordinate + shipSize; y++)
                            {
                                if (array[xcoordinate, y] == 'O' || array[xcoordinate, y] == '*')
                                {
                                    canFit = false;
                                    break;
                                }
                            }
                        }
                        break;
                }

                if (canFit)
                {
                    // Place the ship on the board
                    switch (direction)
                    {
                        case 0: // Left
                            for (int x = xcoordinate; x > xcoordinate - shipSize; x--)
                            {
                                array[x, ycoordinate] = 'O';
                            }
                            break;

                        case 1: // Up
                            for (int y = ycoordinate; y > ycoordinate - shipSize; y--)
                            {
                                array[xcoordinate, y] = 'O';
                            }
                            break;

                        case 2: // Right
                            for (int x = xcoordinate; x < xcoordinate + shipSize; x++)
                            {
                                array[x, ycoordinate] = 'O';
                            }
                            break;

                        case 3: // Down
                            for (int y = ycoordinate; y < ycoordinate + shipSize; y++)
                            {
                                array[xcoordinate, y] = 'O';
                            }
                            break;
                    }
                    validPlacementFound = true;
                    break;
                }
            }

            if (validPlacementFound)
                break;
        }
    }

    static void SpaceCushion(ref char[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        char[,] result = new char[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (array[row, col] == 'O')
                {
                    // Mark the current 'O' cell
                    result[row, col] = 'O';

                    // Check and mark adjacent cells with '*'
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int newRow = row + i;
                            int newCol = col + j;

                            // Ensure the new coordinates are within bounds
                            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                            {
                                // Mark adjacent cells with '*'
                                if (result[newRow, newCol] != 'O')
                                {
                                    result[newRow, newCol] = '*';
                                }
                            }
                        }
                    }
                }
                else if (result[row, col] != '*')
                {
                    // Mark empty cells with '-'
                    result[row, col] = '-';
                }
            }
        }

        // Override the original array with the updated one
        array = result;
    }

    static bool AreAllShipsSunk(char[,] playerBoard)
    {
        int xcount = 0;
        foreach (char cell in playerBoard)
        {
            if (cell == 'X')
            {
                xcount++;
            }
        }
        if (xcount == 17)
        {
            return true; // There's still an 'O' on the board, so not all ships are sunk.
        }
        else
        {
            return false;
        }
    }

    static bool TakeTurn(
        char[,] OpponentBoard,
        char[,] HiddenOpponentBoard,
        string playerName,
        ref (int, int)? HitCoordinate,
        ref int DirectionofShots,
        ref (int, int)? FirstHitCoordinate
    )
    {
        bool missed = false;
        int x,
            y;

        if (HitCoordinate == null)
        {
            do
            {
                (x, y) = RandomCoordinate();
            } while (HiddenOpponentBoard[x, y] == '*' || HiddenOpponentBoard[x, y] == 'X');

            Console.WriteLine("It's " + playerName + "'s turn!");
            Thread.Sleep(500);
            Console.WriteLine($"Attacking {(char)('A' + x)}{y + 1}");
            Thread.Sleep(500);

            if (OpponentBoard[x, y] == 'O')
            {
                Console.WriteLine("It's a HIT!");
                HiddenOpponentBoard[x, y] = 'X';
                FirstHitCoordinate = (x, y);
                HitCoordinate = (x, y);

                // Check if a ship is sunk
                if (CheckSunkShip(OpponentBoard, HiddenOpponentBoard, x, y))
                {
                    FirstHitCoordinate = null;
                    HitCoordinate = null; // Reset the hit coordinate
                    DirectionofShots = -1;
                }
            }
            else
            {
                Console.WriteLine("It's a MISS!");
                HiddenOpponentBoard[x, y] = '*';
                missed = true;
            }
        }
        else
        {
            (x, y) = HitCoordinate.Value;
            int newX = x,
                newY = y;
            bool canFit = false;

            // Attempt to shoot in the current direction
            switch (DirectionofShots)
            {
                case 0: // Left
                    if (
                        x - 1 >= 0
                        && HiddenOpponentBoard[x - 1, y] != '*'
                        && HiddenOpponentBoard[x - 1, y] != 'X'
                    )
                    {
                        newX = x - 1;
                        canFit = true;
                    }
                    break;

                case 1: // Up
                    if (
                        y - 1 >= 0
                        && HiddenOpponentBoard[x, y - 1] != '*'
                        && HiddenOpponentBoard[x, y - 1] != 'X'
                    )
                    {
                        newY = y - 1;
                        canFit = true;
                    }
                    break;

                case 2: // Right
                    if (
                        x + 1 <= 9
                        && HiddenOpponentBoard[x + 1, y] != '*'
                        && HiddenOpponentBoard[x + 1, y] != 'X'
                    )
                    {
                        newX = x + 1;
                        canFit = true;
                    }
                    break;

                case 3: // Down
                    if (
                        y + 1 <= 9
                        && HiddenOpponentBoard[x, y + 1] != '*'
                        && HiddenOpponentBoard[x, y + 1] != 'X'
                    )
                    {
                        newY = y + 1;
                        canFit = true;
                    }
                    break;
            }

            if (canFit)
            {
                x = newX;
                y = newY;

                Console.WriteLine("It's " + playerName + "'s turn!");
                Thread.Sleep(500);
                Console.WriteLine($"Attacking {(char)('A' + x)}{y + 1}");
                Thread.Sleep(500);

                if (OpponentBoard[x, y] == 'O')
                {
                    Console.WriteLine("It's a HIT!");
                    HiddenOpponentBoard[x, y] = 'X';
                    HitCoordinate = (x, y);

                    // Check if a ship is sunk
                    if (CheckSunkShip(OpponentBoard, HiddenOpponentBoard, x, y))
                    {
                        FirstHitCoordinate = null;
                        HitCoordinate = null; // Reset the hit coordinate
                        DirectionofShots = -1;
                    }
                }
                else
                {
                    Console.WriteLine("It's a MISS!");
                    HiddenOpponentBoard[x, y] = '*';
                    missed = true;
                }
            }
            else
            {
                // Change direction
                DirectionofShots = (DirectionofShots + 2) % 4;
                if (HitCoordinate != FirstHitCoordinate)
                {
                    HitCoordinate = FirstHitCoordinate;
                }
                else
                    DirectionofShots = (DirectionofShots + 1) % 4;
            }
        }

        Thread.Sleep(500);
        ; // Delay for one second to allow the player to see the result
        Console.Clear(); // Clear the console after each turn

        return missed;
    }

    static bool CheckSunkShip(char[,] OpponentBoard, char[,] HiddenOpponentBoard, int x, int y)
    {
        char shipSymbol = OpponentBoard[x, y];
        List<(int, int)> shipCoordinates = new List<(int, int)>(); // Store ship coordinates

        // Check horizontally to the right
        int i = x;
        while (i < OpponentBoard.GetLength(0) && OpponentBoard[i, y] == shipSymbol)
        {
            shipCoordinates.Add((i, y));
            i++;
        }

        // Check horizontally to the left
        i = x - 1;
        while (i >= 0 && OpponentBoard[i, y] == shipSymbol)
        {
            shipCoordinates.Add((i, y));
            i--;
        }

        // Check vertically down
        int j = y;
        while (j < OpponentBoard.GetLength(1) && OpponentBoard[x, j] == shipSymbol)
        {
            shipCoordinates.Add((x, j));
            j++;
        }

        // Check vertically up
        j = y - 1;
        while (j >= 0 && OpponentBoard[x, j] == shipSymbol)
        {
            shipCoordinates.Add((x, j));
            j--;
        }

        // Check if the ship is sunk
        bool shipSunk = true;
        foreach (var coord in shipCoordinates)
        {
            if (HiddenOpponentBoard[coord.Item1, coord.Item2] != 'X')
            {
                shipSunk = false;
                break;
            }
        }

        if (shipSunk)
        {
            Console.WriteLine("A ship has been sunk!");

            // Add '*' around the sunken ship
            foreach (var coord in shipCoordinates)
            {
                int row = coord.Item1;
                int col = coord.Item2;

                // Check and mark adjacent cells with '*'
                for (int m = -1; m <= 1; m++)
                {
                    for (int n = -1; n <= 1; n++)
                    {
                        int newRow = row + m;
                        int newCol = col + n;

                        // Ensure the new coordinates are within bounds
                        if (
                            newRow >= 0
                            && newRow < HiddenOpponentBoard.GetLength(0)
                            && newCol >= 0
                            && newCol < HiddenOpponentBoard.GetLength(1)
                            && HiddenOpponentBoard[newRow, newCol] != 'X'
                        )
                        {
                            HiddenOpponentBoard[newRow, newCol] = '*';
                        }
                    }
                }
            }
        }
        return shipSunk;
    }

    static void ClearCushion(char[,] board)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == '*')
                {
                    board[i, j] = '-';
                }
            }
        }
    }

    static void Main()
    {
        Random random = new Random();
        char[,] Player1 = CreateBoard();
        char[,] Player2 = CreateBoard();
        char[,] Player1Hidden = CreateBoard();
        char[,] Player2Hidden = CreateBoard();
        (int, int)? player1FirstHitCoordinate = null;
        (int, int)? player2FirstHitCoordinate = null;
        (int, int)? player1HitCoordinate = null;
        (int, int)? player2HitCoordinate = null;
        int DirectionofShots1 = -1;
        int DirectionofShots2 = -1;

        Console.WriteLine("Welcome to the Battleship Simulator");
        Console.WriteLine("Press any key to simulate the boards");
        Console.WriteLine();
        Console.ReadKey();

        int[] shipSizes = { 5, 4, 3, 3, 2 };
        foreach (int shipSize in shipSizes)
        {
            PlaceShip(Player1, shipSize);
            SpaceCushion(ref Player1);
        }
        ClearCushion(Player1);
        PrintBoard(Player1, "Player 1");

        foreach (int shipSize in shipSizes)
        {
            PlaceShip(Player2, shipSize);
            SpaceCushion(ref Player2);
        }
        ClearCushion(Player2);
        PrintBoard(Player2, "Player 2");

        Console.WriteLine("Looks good, press any key to start the battle");
        Console.ReadKey();
        Console.Clear();
        PrintBoard(Player1Hidden, "Player 1");
        PrintBoard(Player2Hidden, "Player 1");

        Console.WriteLine("Determining the first player's turn...");
        Thread.Sleep(500);

        int currentTurn = random.Next(2);
        bool switchTurn = false; // Indicates whether to switch the turn

        while (true)
        {
            if (currentTurn == 0)
            {
                switchTurn = TakeTurn(
                    Player2,
                    Player2Hidden,
                    "Player 1",
                    ref player1HitCoordinate,
                    ref DirectionofShots1,
                    ref player1FirstHitCoordinate
                );
                PrintBoard(Player1Hidden, "Player 1");
                PrintBoard(Player2Hidden, "Player 2");
            }
            else
            {
                switchTurn = TakeTurn(
                    Player1,
                    Player1Hidden,
                    "Player 2",
                    ref player2HitCoordinate,
                    ref DirectionofShots2,
                    ref player2FirstHitCoordinate
                );
                PrintBoard(Player1Hidden, "Player 1");
                PrintBoard(Player2Hidden, "Player 2");
            }

            // Check if the game is over
            if (AreAllShipsSunk(Player1Hidden))
            {
                Console.WriteLine("Player 2 wins!");
                break;
            }
            else if (AreAllShipsSunk(Player2Hidden))
            {
                Console.WriteLine("Player 1 wins!");
                break;
            }

            // Switch to the other player's turn only if there was a miss
            if (switchTurn)
                currentTurn = 1 - currentTurn;

            Thread.Sleep(500); // Sleep for 1 second
        }

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}
