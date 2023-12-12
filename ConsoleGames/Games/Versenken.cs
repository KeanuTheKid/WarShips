using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Security.Principal;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace ConsoleGames.Games;

public class Versenken : Game
{
    // PUBLIC PROPERTIES
    public override string Name => "Versenken";
    public override string Description => "Erraten Sie die Position der gegnerischen Schiffe.";
    public override string Rules => "Jedes Schiff hat ein Schuss, bei einem Treffer oder nach der Runde läd es nach.";
    public override string Credits => "Keanu, kebelode@ksr.ch";
    public override int Year => 2023;
    public override bool TheHigherTheBetter => true;
    public override int LevelMax => 1;
    public override Score HighScore { get; set; }
    public override Score Play(int level = 1)
    {
        int Playerlives = 6; //placeholder
        int Botlives = 6; //placeholder
        bool WIN = true;
        int stage = 1;
        bool hit = false;
        Point[] PlayerBoats = new Point[] { };
        Point[] BotBoats = new Point[] { };
        //Console.SetBufferSize(310, 310); //apple funktioniert nicht??
        Display.DrawStartScreen(ref level);
        Display.DrawPlayerBoard();
        BotBoats = placeBotBoat();
        PlayerBoats = placePlayerBoat();
        Display.DrawEnemyBoard();
        ClearConsoleArea(0, 25, 120, 10);
        Console.SetCursorPosition(0, 25);
        while (WIN)
        {
            for (int i = 0; i < 1; i++)
            {
                Point already_shot = shoot(ref PlayerBoats, ref BotBoats, ref Botlives); //shoot does whole shoot stuff(evaluate, draw)
            }

            for (int i = 0; i < 5; i++)
            {
                bot_shoot(ref PlayerBoats, ref Playerlives); //shoot does whole shoot stuff(evaluate, draw)
            }
                
            

            if (Playerlives == 0)
            {
                WIN = false;

            }
            else if (Botlives == 0)
            {
                stage++;
                Display.DrawEnemyBoard();
                Console.SetCursorPosition(0, 0);
                BotBoats=placeBotBoat(); //next


            }
            
            switch (level)
            {
                case 1:
                    if (stage == 3)
                    {
                        break;
                    }
                    
                case 2:
                    if (stage == 8)
                    {
                        break;
                    }
                case 3:
                    if (stage == 12)
                    {
                        break;
                    }

            }

        }

        return new Score();
    }
    static private Point[] placePlayerBoat()
    {
        int numberOfObjectives = 6; // Total squares occupied by all boats

        Console.SetCursorPosition(0, 25);
        Console.WriteLine("Place your boats. You got one galleon (3 squares), one brigantine (2 squares) and one sloop (1 square)");
        bool valid = true;
        Point[] PlayerBoatPositions = new Point[numberOfObjectives];
        while (valid)
        {
            // Place galleon (3 squares)
            Console.WriteLine("Place your galleon:");
            for (int i = 0; i < 3; i++)
            {
                PlayerBoatPositions[i] = ReadCoordinates();
            }


            // Place brigantine (2 squares)
            Console.WriteLine("Place your brigantine:");
            for (int i = 0; i < 2; i++)
            {
                PlayerBoatPositions[3 + i] = ReadCoordinates();
            }

            // Place sloop (1 square)
            Console.WriteLine("Place your sloop:");
            PlayerBoatPositions[5] = ReadCoordinates();
            if (CheckObjectDistanceANDDoubleCords(PlayerBoatPositions))
            {
                Display.DrawPlayerBoats(PlayerBoatPositions);
                valid = false;
            }
            else
            {
                Console.WriteLine("You are drunk, Captain!");
                Array.Clear(PlayerBoatPositions, 0, numberOfObjectives);
            }
        }


        //foreach (Point p in PlayerBoatPositions)
        //{
        //    Console.WriteLine($"x,y: ({p.X}, {p.Y})");
        //}

        return PlayerBoatPositions;
    }
    private static double CalculateDistance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }
    static public bool CheckObjectDistanceANDDoubleCords(Point[] Coordinates)
    {
        bool valid = false;

        if (CalculateDistance(Coordinates[0], Coordinates[1])<=1 && CalculateDistance(Coordinates[1], Coordinates[2])<=1 && CalculateDistance(Coordinates[3], Coordinates[4])<=1) // galleon
        {
            valid = true;
        }

        for (int i = 0; i < Coordinates.Length; i++)
        {
            for (int j = i + 1; j < Coordinates.Length; j++)
            {
                if (Coordinates[i].Equals(Coordinates[j]))
                {
                    return false; // erst wenn alles platziert ist, deshalb erst am ende
                }
            }
        }
        return valid;
    }

    static private Point[] placeBotBoat()
    {
        Random random = new Random();
        int numberOfObjectives = 6; // Total squares occupied by all boats
        Point[] botBoatPositions = new Point[numberOfObjectives];
        bool valid = false;

        while (!valid)
        {
            // Place galleon (3 squares)
            PlaceBoatRandomly(botBoatPositions, 0, 3, random);

            // Place brigantine (2 squares)
            PlaceBoatRandomly(botBoatPositions, 3, 2, random);

            // Place sloop (1 square)
            PlaceBoatRandomly(botBoatPositions, 5, 1, random);

            if (CheckObjectDistanceANDDoubleCords(botBoatPositions))
            {
                valid = true;
            }
            else
            {
                Array.Clear(botBoatPositions, 0, numberOfObjectives);
            }
        }
        foreach (Point p in botBoatPositions)
        {
            Console.WriteLine($"x,y: ({p.X}, {p.Y})");
        }
        return botBoatPositions;
    }

    static private void PlaceBoatRandomly(Point[] boatPositions, int startIndex, int boatSize, Random random)
    {
        bool placed = false;
        while (!placed)
        {
            int x = random.Next(0, 5);
            int y = random.Next(0, 5);
            bool horizontal = random.Next(2) == 0;

            bool canPlace = true;
            for (int i = 0; i < boatSize; i++)
            {
                int checkX = horizontal ? x + i : x;
                int checkY = horizontal ? y : y + i;

                if (checkX >= 5 || checkY >= 5 || boatPositions.Contains(new Point(checkX, checkY)))
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                for (int i = 0; i < boatSize; i++)
                {
                    boatPositions[startIndex + i] = new Point(horizontal ? x + i : x, horizontal ? y : y + i);
                }
                placed = true;
            }
        }
    }
    static private Point shoot(ref Point[] PlayerBoatPositions, ref Point[] BotBoatsPositions, ref int Botlives)
    {
        bool hit = false;
        Console.SetCursorPosition(0, 25);
        Console.WriteLine("Shoot your shot");
        Point player_input = ReadCoordinates();

        foreach (Point p in BotBoatsPositions)
        {
            if (player_input.Equals(p)) // Check if the player's input matches any point in BotBoatsPositions
            {
                Console.WriteLine("Hit!");
                Console.ReadLine();
                hit = true;
                ClearConsoleArea(0, 25, 120, 30);
                Display.DrawHitShots_Player(player_input);
                Botlives--;


            }
        }

        if (!hit)
        {
            Console.WriteLine("Miss!");
            Console.ReadLine();
            ClearConsoleArea(0, 25, 120, 30);
            Display.DrawMissedShots_Player(player_input);


        }
        return player_input;
    }

    static private List<Point> bot_shoot(ref Point[] PlayerBoatPositions, ref int Playerlives)
    {
        Random rand = new Random();
        List<Point> bot_input = new List<Point>();

        while (true) // Keep shooting until a miss occurs
        {
            bool hit = false;
            Console.SetCursorPosition(0, 25);
            Console.WriteLine("Bot is shooting");

            // Get a random point
            int x = rand.Next(0, 5);
            int y = rand.Next(0, 5);
            Point shot = new Point(x, y);
            Point last_shot;

            // Check if it hits any player boat
            foreach (Point p in PlayerBoatPositions)
            {
                if (shot.Equals(p))
                {
                    Console.WriteLine($"Bot hit at {p.X}, {p.Y}");                    
                    Console.ReadLine();
                    ClearConsoleArea(0, 25, 120, 30);
                    hit = true;
                    Playerlives--;
                    Display.DrawHitShots_Enemy(shot);
                    break;
                }                                                
            }

            if (!hit)
            {
                Console.WriteLine($"Bot missed{shot.X},{shot.Y} ");
                Console.ReadLine();
                ClearConsoleArea(0, 25, 120, 30);

                break;
            }

            // Add the shot (hit or miss) to bot_input
            bot_input.Add(shot);

            if (hit)
            {
                // Choose a direction for the next shot
                int direction = rand.Next(0, 4);
                switch (direction)
                {
                    case 0: y = Math.Max(y - 1, 0); break; // Up
                    case 1: y = Math.Min(y + 1, 4); break; // Down
                    case 2: x = Math.Max(x - 1, 0); break; // Left
                    case 3: x = Math.Min(x + 1, 4); break; // Right
                }
            }
        }

        ClearConsoleArea(0, 25, 120, 30);
        return bot_input;
    }

    public static System.Drawing.Point ConvertToPoint(Versenken.Point versenkenPoint)
    {
        return new System.Drawing.Point(versenkenPoint.X, versenkenPoint.Y);
    }
    public static Point ReadCoordinates()
    {
        while (true) // Keep asking until valid input is provided
        {
            string input = Console.ReadLine();
            string[] parts = input.Split(',');

            if (parts.Length == 2
                && int.TryParse(parts[0], out int x)
                && int.TryParse(parts[1], out int y))
            {
                return new Point(x, y); // Assuming you have a Point structure defined
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter the coordinates in the format 'x,y'.");
            }
        }
    }
    public static void ClearConsoleArea(int startX, int startY, int width, int height)
    {
        Console.SetCursorPosition(startX, startY);
        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(" "); // Overwrite the current position with a space
            }
        }
        Console.SetCursorPosition(0, 0);
    }

    // Point structure (can also use System.Drawing.Point)
    public struct Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)      //chatgpt
        {
            X = x;
            Y = y;
        }
    }
}

class Display
{
    static public void DrawPlayerBoard()
    {
        int CellHeight = 2;
        int LineHeight = 3;
        int counter_y = 0;
        int counter_x = 0;

        for (int i = 0; i < 15; i=i+LineHeight)
        {
            Console.SetCursorPosition(11, 10+i);
            Console.WriteLine(counter_y);
            counter_y++;
        }

        for (int i = 0; i < 30; i=i+LineHeight*2)
        {
            Console.SetCursorPosition(14+i, 8);
            Console.WriteLine(counter_x);
            counter_x++;
        }

        for (int y = 0; y< 15; y=y+LineHeight)
        {
            for (int x = 0; x < 30; x=x+2*LineHeight)
            {
                DrawRectangle(CellHeight, 14+x, 10+y, ConsoleColor.DarkBlue);
            }
        }
        Console.SetCursorPosition(0, 0);
        Console.ResetColor();



    }
    static public void DrawEnemyBoard()
    {
        int CellHeight = 2;
        int LineHeight = 3;
        int counter_y = 0;
        int counter_x = 0;

        for (int i = 0; i < 15; i=i+LineHeight)
        {
            Console.SetCursorPosition(48, 10+i);
            Console.WriteLine(counter_y);
            counter_y++;
        }

        for (int i = 0; i < 30; i=i+LineHeight*2)
        {
            Console.SetCursorPosition(52+i, 8);
            Console.WriteLine(counter_x);
            counter_x++;
        }


        for (int y = 0; y< 15; y=y+LineHeight)
        {
            for (int x = 0; x < 30; x=x+2*LineHeight)
            {
                DrawRectangle(CellHeight, 50+x, 10+y, ConsoleColor.DarkBlue);
            }
        }
        Console.SetCursorPosition(0, 0);
        Console.ResetColor();

    }



    public static void DrawMissedShots_Player(Versenken.Point missedshots)
    {

        // Convert Versenken.Point to System.Drawing.Point
        System.Drawing.Point drawingPoint = Versenken.ConvertToPoint(missedshots);
        int xOffset = 14;
        int yOffset = 10;
        int xStep = 6;
        int yStep = 3;



        int consoleX = xOffset + (drawingPoint.X * xStep);
        int consoleY = yOffset + (drawingPoint.Y * yStep);


        DrawRectangle(2, consoleX+36, consoleY, ConsoleColor.Cyan);


    }
    public static void DrawHitShots_Enemy(Versenken.Point hitshots)
    {
        // Convert Versenken.Point to System.Drawing.Point
        System.Drawing.Point drawingPoint = Versenken.ConvertToPoint(hitshots);
        int xOffset = 14;
        int yOffset = 10;
        int xStep = 6;
        int yStep = 3;



        int consoleX = xOffset + (drawingPoint.X * xStep);
        int consoleY = yOffset + (drawingPoint.Y * yStep);


        DrawRectangle(2, consoleX, consoleY, ConsoleColor.Magenta);


    }
    public static void DrawHitShots_Player(Versenken.Point hitshots)
    {

        // Convert Versenken.Point to System.Drawing.Point
        System.Drawing.Point drawingPoint = Versenken.ConvertToPoint(hitshots);
        int xOffset = 14;
        int yOffset = 10;
        int xStep = 6;
        int yStep = 3;



        int consoleX = xOffset + (drawingPoint.X * xStep);
        int consoleY = yOffset + (drawingPoint.Y * yStep);


        DrawRectangle(2, consoleX+36, consoleY, ConsoleColor.Magenta);


    }

    public static void DrawPlayerBoats(Versenken.Point[] coordinates)
    {
        int xOffset = 14;
        int yOffset = 10;
        int xStep = 6;
        int yStep = 3;

        foreach (Versenken.Point p in coordinates)
        {
            int consoleX = xOffset + (p.X * xStep);
            int consoleY = yOffset + (p.Y * yStep);


            DrawRectangle(2, consoleX, consoleY, ConsoleColor.DarkGreen);
        }
    }
    public static void DrawRectangle(int cellHeight, int currentPos_x, int currentPos_y, ConsoleColor COLOR)
    {
        int cellWidth = 2 * cellHeight;

        Console.BackgroundColor = COLOR;

        for (int y = 0; y < cellHeight; y++)
        {
            for (int x = 0; x < cellWidth; x++)
            {
                Console.SetCursorPosition(currentPos_x+x, currentPos_y+y);
                Console.Write(" ");
            }
        }
        Console.ResetColor();
    }
    public static void DrawStartScreen(ref int level)
    {
        String Title = @"
                    ____        _   _   _      ____  _     _           
                   | __ )  __ _| |_| |_| | ___/ ___|| |__ (_)_ __  ___ 
                   |  _ \ / _` | __| __| |/ _ \___ \| '_ \| | '_ \/ __|
                   | |_) | (_| | |_| |_| |  __/___) | | | | | |_) \__ \
                   |____/ \__,_|\__|\__|_|\___|____/|_| |_|_| .__/|___/
                                                            |_|        
";
        Console.Write(Title);
        Console.SetCursorPosition(0, 0);
    }
}
class Cursor //falls genug zeit
{

}