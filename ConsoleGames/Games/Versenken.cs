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
using System.Threading;
using System.Runtime.Versioning;
using static System.Formats.Asn1.AsnWriter;

namespace ConsoleGames.Games;

public class Versenken : Game
{
    // PUBLIC PROPERTIES
    public override string Name => "Versenken";
    public override string Description => "Erraten Sie die Position der gegnerischen Schiffe.";
    public override string Rules => "Du hast 5 Schüsse pro Runde, eben so dein Gegner.";
    public override string Credits => "Keanu, kebelode@ksr.ch";
    public override int Year => 2023;
    public override bool TheHigherTheBetter => true;
    public override int LevelMax => 3;
    public override Score HighScore { get; set; }
    public override Score Play(int level = 1)
    {
        int Playerlives = 6;
        int Botlives = 6;
        Score score = new Score();
        int stage = 1;
        bool Completed = false;
        Point[] PlayerBoats = new Point[] { };
        Point[] BotBoats = new Point[] { };
        //Console.SetBufferSize(310, 310); //apple funktioniert nicht??
        Display.DrawStartScreen(ref level, ref stage);
        Display.DrawPlayerBoard();
        BotBoats = placeBotBoat();
        PlayerBoats = placePlayerBoat();
        Display.DrawEnemyBoard();
        ClearConsoleArea(0, 25, 120, 80);
        Console.SetCursorPosition(0, 25);
        while (true)
        {


            while (Botlives > 0 && Playerlives > 0)
            {
                
                    for (int i = 0; i < 5; i++)
                    {
                        shoot(ref PlayerBoats, ref BotBoats, ref Botlives); //shoot does whole shoot stuff(evaluate, draw)
                        if (Botlives <= 0)
                        {
                            Completed = true;
                            WIN(ref Botlives, ref Playerlives, ref BotBoats, ref stage, ref level, ref score, ref Completed);
                            break;
                        }



                    }
                if (score.LevelCompleted == true)
                {
                    break;
                }
                
                    for (int i = 0; i < 5; i++)
                    {
                        if (Botlives > 0)
                        {
                            bot_shoot(ref PlayerBoats, ref Playerlives);
                            if (Playerlives <= 0)
                            {
                            score.Points = Playerlives;
                            score.LevelCompleted = false;
                            Display.DrawLoseScreen();
                                break;
                            }
                        }
                    }
            }
            break;
        }
        level++;
        return score;
    }
    static private Score WIN(ref int Botlives, ref int Playerlives, ref Point[] BotBoats, ref int stage, ref int level, ref Score score, ref bool Completed )
    {
         
            stage++;
            Display.DrawStage(ref stage);
            Display.DrawEnemyBoard();
            Console.SetCursorPosition(0, 0);
            BotBoats=placeBotBoat();
            Botlives = 6;
        
        switch (level)
        {
            case 1:
                if (stage == 3)// 2 wellen
                {
                   
                    Framework(ref Playerlives, ref score);
                    Display.DrawWinScreen();
                    return score;

                }
                break;

            case 2:
                if (stage == 4)// 3 wellen
                {
                   
                    Framework(ref Playerlives, ref score);
                    Display.DrawWinScreen();
                    return score;

                }
                break;
            case 3:
                if (stage == 6)// 5 wellen
                {
                    
                    Framework(ref Playerlives, ref score);
                    Display.DrawWinScreen();                    
                    return score;

                }
                break;
        }
        return score;
    } 
    static private Point[] placePlayerBoat()
    {
        int numberOfObjectives = 6; // Total squares occupied by all boats

        Console.SetCursorPosition(0, 25);
        Console.WriteLine("Place your boats. You got one galleon (3 squares), one brigantine (2 squares) and one sloop (1 square). Format: (X,Y)");
        bool valid = true;
        Point[] PlayerBoatPositions = new Point[numberOfObjectives];
        while (valid)
        {
            // Place galleon (3 squares)
            Console.WriteLine("Place your galleon: Part (1/3)");
            for (int i = 0; i < 3; i++)
            {
                PlayerBoatPositions[i] = ReadCoordinates();
                Console.WriteLine($"Part ({i+2}/3)");

            }


            // Place brigantine (2 squares)
            Console.WriteLine($"Place your brigantine: Part (1/2)");
            for (int i = 0; i < 2; i++)
            {
                PlayerBoatPositions[3 + i] = ReadCoordinates();
                Console.WriteLine($"Part ({i+2}/2)");
            }

            // Place sloop (1 square)
            Console.WriteLine("Place your sloop: Part (1/1)");
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
        int numberOfObjectives = 6; //squares occupied by all boats
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
        //nützlich falls Sie schnell gewinnen wollen...
        //foreach (Point p in botBoatPositions)
       // {
       //     Console.WriteLine($"x,y: ({p.X}, {p.Y})");
       // }
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
        Point player_input;
        bool hit;
        while (true)
        {
            hit = false;
            Console.SetCursorPosition(0, 25);
            Console.WriteLine("Shoot your shot (X,Y)");
            player_input = ReadCoordinates();
            if (player_input.X > 4 || player_input.Y > 4 || player_input.X < 0 || player_input.Y < 0)
            {
                Console.WriteLine("Your shot is outside of the battlefield! Press ENTER to continue");
                Console.ReadLine();
                ClearConsoleArea(0, 25, 120, 80);
            }
            else
            {
                break;
            }
        }

        foreach (Point p in BotBoatsPositions)
        {
            if (player_input.Equals(p)) // check botposition == input ??
            {
                Console.WriteLine("Hit! Press ENTER to continue");
                Console.ReadLine();
                hit = true;
                ClearConsoleArea(0, 25, 120, 80);
                Display.DrawHitShots_Player(player_input);
                Botlives--;


            }
        }

        if (!hit)
        {
            Console.WriteLine("Miss! Press ENTER to continue");
            Console.ReadLine();
            ClearConsoleArea(0, 25, 120, 80);
            Display.DrawMissedShots_Player(player_input);


        }
        return player_input;
    }
    static private List<Point> bot_shoot(ref Point[] PlayerBoatPositions, ref int Playerlives)
    {
        Random rand = new Random();
        List<Point> bot_input = new List<Point>();

        while (true) 
        {
            bool hit = false;
            Console.SetCursorPosition(0, 25);
            Console.WriteLine("Bot is shooting...");
            int x = rand.Next(0, 5);
            int y = rand.Next(0, 5);
            Point shot = new Point(x, y);

            // Check if it hits any player boat
            foreach (Point p in PlayerBoatPositions)
            {
                if (shot.Equals(p))
                {
                    Console.WriteLine($"Bot hit at {p.X}, {p.Y} Press ENTER to continue");
                    Console.ReadLine();
                    ClearConsoleArea(0, 25, 120, 80);
                    hit = true;
                    Playerlives--;
                    Display.DrawHitShots_Enemy(shot);
                    break;
                }
            }

            if (!hit)
            {
                Console.WriteLine($"Bot missed at {shot.X},{shot.Y} Press ENTER to continue");
                Console.ReadLine();
                ClearConsoleArea(0, 25, 120, 80);

                break;
            }

            // Add the shot to bot_input
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

        ClearConsoleArea(0, 25, 120, 80);
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
                Console.Write(" "); 
            }
        }
        Console.SetCursorPosition(0, 0);
    }
    // Point structure (can also use System.Drawing.Point)
    public struct Point
    {
        public int X { get; } //get = schreibschutz, kann nur innerhalb der konstruktoren verändert werden
        public int Y { get; }

        public Point(int x, int y)   
        {
            X = x;
            Y = y;
        }
    }
    static private void Framework(ref int Playerlives, ref Score score)
    {
        score.Points = Playerlives;
        score.LevelCompleted = true;
        score.Level++;
    }
}

class Display
{
    static internal void DrawPlayerBoard()
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
    static internal void DrawEnemyBoard()
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
    internal static void DrawMissedShots_Player(Versenken.Point missedshots)
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
    internal static void DrawHitShots_Enemy(Versenken.Point hitshots)
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
    internal static void DrawHitShots_Player(Versenken.Point hitshots)
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
    internal static void DrawPlayerBoats(Versenken.Point[] coordinates)
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
    internal static void DrawRectangle(int cellHeight, int currentPos_x, int currentPos_y, ConsoleColor COLOR)
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
    internal static void DrawStartScreen(ref int level, ref int stage)
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
        DrawStage(ref stage);

    }
    internal static void DrawStage(ref int stage)
    {
        Console.SetCursorPosition(80, 2);
        Console.Write("Stage:");
        Console.Write(stage);
    }
    internal static void DrawWinScreen()
    {
        String Title = @"
               __        ___                        
               \ \      / (_)_ __  _ __   ___ _ __  
                \ \ /\ / /| | '_ \| '_ \ / _ \ '__|
                 \ V  V / | | | | | | | |  __/ |     
                  \_/\_/  |_|_| |_|_| |_|\___|_|      
               __        ___                        
               \ \      / (_)_ __  _ __   ___ _ __  
                \ \ /\ / /| | '_ \| '_ \ / _ \ '__|
                 \ V  V / | | | | | | | |  __/ |     
                  \_/\_/  |_|_| |_|_| |_|\___|_|                                                                                                                                                                                                                                                                                     
  ____ _     _      _                  _ _                       
 / ___| |__ (_) ___| | _____ _ __   __| (_)_ __  _ __   ___ _ __ 
| |   | '_ \| |/ __| |/ / _ \ '_ \ / _` | | '_ \| '_ \ / _ \ '__|
| |___| | | | | (__|   <  __/ | | | (_| | | | | | | | |  __/ |   
 \____|_| |_|_|\___|_|\_\___|_| |_|\__,_|_|_| |_|_| |_|\___|_|   
                                                                                                                                            
";
        int x = 0;
        bool button = true;

        while (button)
        {
            Console.Clear();
            Console.SetCursorPosition(x, 4);
            Console.Write(Title);

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;//taste unsichtbar
                if (key == ConsoleKey.Q)
                {
                    button = false;
                }
            }

            Thread.Sleep(100); 
        }
    }
    internal static void DrawLoseScreen()
    {
        String Title = @"
   ____    _    __  __ _____    _____     _______ ____  
  / ___|  / \  |  \/  | ____|  / _ \ \   / / ____|  _ \ 
 | |  _  / _ \ | |\/| |  _|   | | | \ \ / /|  _| | |_) |
 | |_| |/ ___ \| |  | | |___  | |_| |\ V / | |___|  _ < 
  \____/_/ _ \_\_|  |_|_____|  \___/_ \_/ _|_____|_|_\_\
  / ___|  / \  |  \/  | ____|  / _ \ \   / / ____|  _ \ 
 | |  _  / _ \ | |\/| |  _|   | | | \ \ / /|  _| | |_) |
 | |_| |/ ___ \| |  | | |___  | |_| |\ V / | |___|  _ < 
  \____/_/ _ \_\_|  |_|_____|  \___/_ \_/ _|_____|_|_\_\
  / ___|  / \  |  \/  | ____|  / _ \ \   / / ____|  _ \ 
 | |  _  / _ \ | |\/| |  _|   | | | \ \ / /|  _| | |_) |
 | |_| |/ ___ \| |  | | |___  | |_| |\ V / | |___|  _ < 
  \____/_/   \_\_|  |_|_____|  \___/  \_/  |_____|_| \_\
                                                                                                                                                                          
";
        int x = 0;
        bool button = true;

        while (button)
        {
            Console.Clear();
            Console.SetCursorPosition(x, 4);
            Console.Write(Title);

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;//taste unsichtbar
                if (key == ConsoleKey.Q)
                {
                    button = false;
                }
            }

            Thread.Sleep(100); 
        }
    }
}
