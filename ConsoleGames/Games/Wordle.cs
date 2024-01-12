using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml;

namespace ConsoleGames.Games; // here use name of your project

public class Wordle : Game
{
    // PUBLIC PROPERTIES (Eigenschaften)
    public override string Name => "Wordle";
    public override string Description => "Erraten Sie das Geheime Wort bevor sie keine Leben mehr haben";
    public override string Rules => "Errate das gesuchte Wort in möglichst wenig Versuchen.";
    public override string Credits => "Gian Engel, giaengel@ksr.ch";
    public override int Year => 2023;
    public override bool TheHigherTheBetter => true;
    public override int LevelMax => 3;
    public override Score HighScore { get; set; }
    // No variable declarations in this area!!




    public override Score Play(int level = 1)
    {
        Score score = new Score();
        bool Completed = false;
        bool gameover = false;
        string input = null;
        score.Level = level;
        string secretWord = readsecretword(level);
        Point point = new Point(0, 0);
        int count = 0;
        int lives = 0;

        if (level == 1)
        {
            lives = 20;
        }
        else if (level == 2)
        {
            lives = 16;
        }
        else
        {
            lives = 12;
        }
        Console.WriteLine("Versuche das Wort zu erraten!");

        // Class um zu Zählen wie oft ein Buchstabe in Wort ist
        Dictionary<char, int> letterFrequencies = new Dictionary<char, int>();
        foreach (char c in secretWord)
        {
            if (letterFrequencies.ContainsKey(c))
                letterFrequencies[c]++;
            else
                letterFrequencies[c] = 1;
        }


        while (!gameover)
        {

            Console.SetCursorPosition(100, 0);
            Console.Write("Leben:" + lives/4);
            Console.SetCursorPosition(0, point.Y+1);
            Console.WriteLine(secretWord);
            if (lives == 0)
            {
                Console.Clear();
                Thread.Sleep(50);
                GameOver(ref secretWord);
            }
            Thread.Sleep(200);
            point.X = 0;
            int YKORD = count * 10;
            count++;
            Console.ResetColor();
            input = readInput(ref level);



            if (input == secretWord)
            {
                Console.Clear();
                Thread.Sleep(50);
                WIN(ref lives, ref level, ref score, ref Completed);
                Console.ReadKey();
                gameover = true;
                break;

            }
            else
            {
                // Wie oft der Buchstabe verwendet wird in temporäre Variable
                Dictionary<char, int> tempFrequencies = new Dictionary<char, int>(letterFrequencies);
                for (int bStab = 0; bStab < secretWord.Length; bStab++)
                {
                    if (secretWord[bStab] == input[bStab]) // Richtige Positon
                    {
                        point.Y = YKORD;
                        DrawChar(input[bStab], ConsoleColor.Green, ref point);
                        tempFrequencies[input[bStab]]--; // Zähler runtermachen
                        point.X += 20;
                        lives--;
                    }
                    else
                    {
                        if (secretWord.Contains(input[bStab]) && tempFrequencies[input[bStab]] > 0) // Falsche Position aber Buchstabe im Wort
                        {
                            point.Y = YKORD;
                            DrawChar(input[bStab], ConsoleColor.DarkYellow, ref point);
                            tempFrequencies[input[bStab]]--;
                            point.X += 20;
                            lives--;

                        }
                        else // Buchstabe nicht im Wort oder Buchstabe schon zugeordnet
                        {
                            point.Y = YKORD;
                            DrawChar(input[bStab], ConsoleColor.Red, ref point);
                            point.X += 20;
                            lives--;
                        }
                    }
                }
            }
        }

        return score;

    }



    private string readsecretword(int level)
    {
        string[] wordsSimple = new string[] { "Hund", "Elfe", "Haus", "Auto", "Edel", "Fett", "Bube", "Buch", "Chef", "Brav", "Mond", "Baum", "Zeit", "Wind", "Brot", "Boot", "Lied", "Ball", "Kind", "Leid", "Wolf", "Gold", "Faul", "Kamm", "Tuch" };
        string[] wordsMedium = new string[] { "Aachs", "Aarau", "uboot", "haben", "seine", "geben", "Abgas", "Armut", "Blume", "Hafen", "Welle", "Nacht", "Stein", "Faden", "Apfel", "Tisch" };
        string[] wordsAdvanced = new string[] { "Blumen", "Sorgen", "Träume", "Frucht", "Muster", "Pflege", "Reifen", "Winter", "Sommer", "Farben", "Kuchen", "Lachen", "Stille", "Wunder", "Zirkus", "Schnee" };

        Random rand = new Random();
        string secretWord = "";
        if (level == 1)
        {
            secretWord = wordsSimple[rand.Next(0, wordsSimple.Length)];
        }
        if (level == 2)
        {
            secretWord = wordsMedium[rand.Next(0, wordsMedium.Length)];

        }
        if (level == 3)
        {
            secretWord = wordsAdvanced[rand.Next(0, wordsAdvanced.Length)];
        }



        secretWord = secretWord.ToUpper();
        return secretWord;


    }


    private static string readInput(ref int level)
    {

        string input = Console.ReadLine();
        input = input.ToUpper();
        while (true)
        {


            switch (level)
            {
                case 1:
                    if (input.Length == 4)
                    {
                        return input;
                    }
                    else
                    {
                        Console.WriteLine("Eingabe ungültig");
                    }
                    break;
                case 2:
                    if (input.Length == 5)
                    {
                        return input;
                    }
                    else
                    {
                        Console.WriteLine("Eingabe ungültig");
                    }
                    break;
                case 3:
                    if (input.Length == 6)
                    {
                        return input;
                    }
                    else
                    {
                        Console.WriteLine("Eingabe ungültig");

                    }
                    break;
            }
            input = Console.ReadLine();
            input= input.ToUpper();
        }

    }



    static private void Framework(ref int lives, ref Score score)
    {
        score.Points = lives;
        score.LevelCompleted = true;

    }
    static private Score WIN(ref int lives, ref int level, ref Score score, ref bool Completed)
    {

        Framework(ref lives, ref score);
        GameWon();
        return score;


    }


    private static void GameWon()
    {
        ;
        Thread.Sleep(50);
        Console.Write(@"
___  _ ____  _       _      ____  _     
\  \///  _ \/ \ /\  / \  /|/  _ \/ \  /|
 \  / | / \|| | ||  | |  ||| / \|| |\ ||
 / /  | \_/|| \_/|  | |/\||| \_/|| | \||
/_/   \____/\____/  \_/  \|\____/\_/  \|
                                        
");

        Thread.Sleep(200);

    }



    private void GameOver(ref string secretWord)
    {
        Console.Write(@"
 _____ ____  _      _____ ____  _     _____ ____ 
/  __//  _ \/ \__/|/  __//  _ \/ \ |\/  __//  __\
| |  _| / \|| |\/|||  \  | / \|| | //|  \  |  \/|
| |_//| |-||| |  |||  /_ | \_/|| \// |  /_ |    /
\____\\_/ \|\_/  \|\____\\____/\__/  \____\\_/\_\
");
        Console.SetCursorPosition(0, 8);
        Console.Write("The secret word was " + secretWord);

    }






    private Point DrawChar(char c, ConsoleColor col, ref Point point)
    {

        string letter = (font[(int)c - 65]);
        string[] array1 = letter.Split('\n');
        for (int i = 0; i < array1.Length; i++)
        {

            Console.SetCursorPosition(point.X, point.Y);
            Console.ForegroundColor = col;
            Console.WriteLine(array1[i]);
            Console.ResetColor();
            point.Y++;


        }
        return point;
    }



    static string[] font =
    {
@"
.----------------.   
| .--------------. |  
| |      __      | |  
| |     /  \     | |  
| |    / /\ \    | |  
| |   / ____ \   | |  
| | _/ /    \ \_ | |  
| ||____|  |____|| |  
| |              | |  
| '--------------' |  
 '----------------'  ",
 @"
.----------------.
| .--------------. |
| |   ______     | |
| |  |_   _ \    | |
| |    | |_) |   | |
| |    |  __'.   | |
| |   _| |__) |  | |
| |  |_______/   | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------.
| .--------------. |
| |     ______   | |
| |   .' ___  |  | |
| |  / .'   \_|  | |
| |  | |         | |
| |  \ `.___.'\  | |
| |   `._____.'  | |
| |              | |
| '--------------' |
 '----------------'",
@"
 .----------------. 
| .--------------. |
| |  ________    | |
| | |_   ___ `.  | |
| |   | |   `. \ | |
| |   | |    | | | |
| |  _| |___.' / | |
| | |________.'  | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |  _________   | |
| | |_   ___  |  | |
| |   | |_  \_|  | |
| |   |  _|  _   | |
| |  _| |___/ |  | |
| | |_________|  | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |  _________   | |
| | |_   ___  |  | |
| |   | |_  \_|  | |
| |   |  _|      | |
| |  _| |_       | |
| | |_____|      | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |    ______    | |
| |  .' ___  |   | |
| | / .'   \_|   | |
| | | |    ____  | |
| | \ `.___]  _| | |
| |  `._____.'   | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |  ____  ____  | |
| | |_   ||   _| | |
| |   | |__| |   | |
| |   |  __  |   | |
| |  _| |  | |_  | |
| | |____||____| | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |     _____    | |
| |    |_   _|   | |
| |      | |     | |
| |      | |     | |
| |     _| |_    | |
| |    |_____|   | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |     _____    | |
| |    |_   _|   | |
| |      | |     | |
| |   _  | |     | |
| |  | |_' |     | |
| |  `.___.'     | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |  ___  ____   | |
| | |_  ||_  _|  | |
| |   | |_/ /    | |
| |   |  __'.    | |
| |  _| |  \ \_  | |
| | |____||____| | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |   _____      | |
| |  |_   _|     | |
| |    | |       | |
| |    | |   _   | |
| |   _| |__/ |  | |
| |  |________|  | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| | ____    ____ | |
| ||_   \  /   _|| |
| |  |   \/   |  | |
| |  | |\  /| |  | |
| | _| |_\/_| |_ | |
| ||_____||_____|| |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .-----------------.
| .--------------. |
| | ____  _____  | |
| ||_   \|_   _| | |
| |  |   \ | |   | |
| |  | |\ \| |   | |
| | _| |_\   |_  | |
| ||_____|\____| | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |     ____     | |
| |   .'    `.   | |
| |  /  .--.  \  | |
| |  | |    | |  | |
| |  \  `--'  /  | |
| |   `.____.'   | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |   ______     | |
| |  |_   __ \   | |
| |    | |__) |  | |
| |    |  ___/   | |
| |   _| |_      | |
| |  |_____|     | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |    ___       | |
| |  .'   '.     | |
| | /  .-.  \    | |
| | | |   | |    | |
| | \  `-'  \_   | |
| |  `.___.\__|  | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |  _______     | |
| | |_   __ \    | |
| |   | |__) |   | |
| |   |  __ /    | |
| |  _| |  \ \_  | |
| | |____| |___| | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |    _______   | |
| |   /  ___  |  | |
| |  |  (__ \_|  | |
| |   '.___`-.   | |
| |  |`\____) |  | |
| |  |_______.'  | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |  _________   | |
| | |  _   _  |  | |
| | |_/ | | \_|  | |
| |     | |      | |
| |    _| |_     | |
| |   |_____|    | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| | _____  _____ | |
| ||_   _||_   _|| |
| |  | |    | |  | |
| |  | '    ' |  | |
| |   \ `--' /   | |
| |    `.__.'    | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| | ____   ____  | |
| ||_  _| |_  _| | |
| |  \ \   / /   | |
| |   \ \ / /    | |
| |    \ ' /     | |
| |     \_/      | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| | _____  _____ | |
| ||_   _||_   _|| |
| |  | | /\ | |  | |
| |  | |/  \| |  | |
| |  |   /\   |  | |
| |  |__/  \__|  | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |  ____  ____  | |
| | |_  _||_  _| | |
| |   \ \  / /   | |
| |    > `' <    | |
| |  _/ /'`\ \_  | |
| | |____||____| | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |  ____  ____  | |
| | |_  _||_  _| | |
| |   \ \  / /   | |
| |    \ \/ /    | |
| |    _|  |_    | |
| |   |______|   | |
| |              | |
| '--------------' |
 '----------------' ",
@"
 .----------------. 
| .--------------. |
| |   ________   | |
| |  |  __   _|  | |
| |  |_/  / /    | |
| |     .'.' _   | |
| |   _/ /__/ |  | |
| |  |________|  | |
| |              | |
| '--------------' |
 '----------------' ",















    };





}






