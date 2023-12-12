using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Security.Principal;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.ComponentModel;

namespace ConsoleGames.Games; // here use name of your project

public class HangMan : Game
{
    // PUBLIC PROPERTIES (Eigenschaften)S
    public override string Name => "Hang Man";
    public override string Description => "Erraten Sie das Wort bevor alle Leben weg sind";
    public override string Rules => "Errate das gesuchte Wort in möglichst wenig Versuchen.";
    public override string Credits => "Keanu, kebelode@ksr.ch";
    public override int Year => 2023;
    public override bool TheHigherTheBetter => false;
    public override int LevelMax => 4;
    public override Score HighScore { get; set; }
    // No variable declarations in this area!!
    public override Score Play(int level = 1)
    {
        // Variable declarations allowed here

        // The game repeats until finished by player 1
        // Variable declarations allowed here
        Score score = new Score();
        score.LevelCompleted = false;
        bool complete = false;
        score.Level = level;
            int lifes = 5;
            string? guessed = null; //list of guessed Characters
            string? missed = null; //list of wrong characters
            string SecretWord = ReadSecretWord(ref level);            // Player 1: Enter the secret word to be guessed by player 2
            string SecretWord_Invisible = MakeInvisible(ref SecretWord);
            int stop = 0;

            HangTheMan(ref SecretWord_Invisible, ref lifes, ref guessed, ref missed, ref SecretWord, ref level);                // Screen output for a good start
        while (true)               // Player 2: Make your guesses
        {
            string newChar = ReadOneChar();           // Handle input of one char. 
            EvaluateTheSituation(SecretWord, newChar, ref lifes, ref guessed, ref missed, ref SecretWord_Invisible, ref stop, ref complete);  // Game Logic goes here
            HangTheMan(ref SecretWord_Invisible, ref lifes, ref guessed, ref missed, ref SecretWord, ref level);

            if (stop == 1)
            {
                break;
                score.Points = lifes;
                score.Level = level;
                score.LevelCompleted = false;

            }
            
            if (stop == 2)
            {
                break;
                score.Points = lifes;
                score.Level = level;
                score.LevelCompleted = true;
            }
        }   
        return score;
    }

    private string ReadSecretWord(ref int level) // Modification of method declaration recommended: Add return value and parameters
                                   // If there are rules and constraints on allowed secrets (e.g. no digits), check them in here
    {

        string secretW = "";
;
        Random rand = new Random();
        

        string[] wordsSimple = new string[] { "Hund", "Katze", "Haus", "Auto", "Apfel", "Tisch", "Stuhl", "Buch", "Fenster", "Tuer", "Wasser", "Baum", "Blume", "Sonne", "Mond", "Tag", "Nacht", "Ball", "Kind", "Schule" };
        string[] wordsMedium = new string[] { "Bibliothek", "Abenteuer", "Geheimnis", "Symphonie", "Expedition", "Karriere", "Toleranz", "Illusion", "Revolution", "Koordination", "Vision", "Harmonie", "Konsequenz", "Inspiration", "Kreativität", "Perspektive", "Emotion", "Rhythmus", "Universum", "Innovation", "Mysterium" };
        string[] wordsAdvanced = new string[] { "Paradox", "Ambivalenz", "Konnotation", "Konstellation", "Subjektiv", "Pragmatik", "Divergenz", "Osmose", "Symbiose", "Dissonanz", "Redundanz", "Hyperbel", "Polymorph", "Metamorphose", "Obsession", "Resonanz" };

        if (level == 1)
        {
            secretW = wordsSimple[rand.Next(0, wordsSimple.Length)];
        }
        if (level == 2)
        {
            secretW = wordsMedium[rand.Next(0, wordsMedium.Length)];

        }
        if (level == 3)
        {
            secretW = wordsAdvanced[rand.Next(0, wordsAdvanced.Length)];
        }

        secretW = secretW.ToUpper();

        return secretW; 
    }
    private string MakeInvisible(ref string SecretWord)
    {
        string? SecretWord_Invisible = null;
        for (int i = 0; i < SecretWord.Length; i++)
        {
            SecretWord_Invisible = SecretWord_Invisible + "_";
        }
        return SecretWord_Invisible;
    }

    private string ReadOneChar() // Modification of method declaration recommended: Add return value and parameters
                                // If there are rules and constraints on allowed secrets (e.g. no digits), make sure the input is allowed
    {
        // Variable declarations allowed here

        Char[] alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        while (true)
        {

            Console.Write("Enter Char:");
            string USERChar = Console.ReadLine();
            USERChar = USERChar.ToUpper();
            bool isValid = true;

            if (USERChar.Length != 1)
            {
                isValid = false;
                Console.WriteLine("Please enter only one character");

            }
            else
            {
                foreach (char c in USERChar)
                {
                    if (!alphabet.Contains(c))
                    {
                        isValid = false;
                        break;
                    }
                }
                if (isValid)
                {
                    return USERChar;
                }
                else
                {
                    Console.WriteLine("Only letters allowed. Please try again");
                }
            }
        }

        // Console.Write() etc. allowed here!
    }

    private bool EvaluateTheSituation(string SecretWord, string newChar, ref int lifes, ref string guessed, ref string missed, ref string SecretWord_Invisible, ref int stop, ref bool complete) // Modification of method declaration recommended: Add return value and parameters
                                                                                                                                                                             // In here, evaluate the char entered: Is it part of the secret word?
                                                                                                                                                                             // Calculate and return the game status (Hit or missed? Where? List and number of missed chars?...)
    {

        char[] secretWordArray = SecretWord.ToCharArray();
        char[] secretWordInvisibleArray = SecretWord_Invisible.ToCharArray();

        // Variable declarations allowed here

        if (!SecretWord.Contains(newChar))
        {
            lifes--;
            Console.WriteLine("not contained");
            missed = missed+newChar;
            missed = String.Concat(missed.OrderBy(c => c));
        }
        else
        {
            Console.WriteLine("contained");
            guessed = guessed+newChar;
            for (int i = 0; i < secretWordArray.Length; i++)
            {
                if (secretWordArray[i].ToString() == newChar)
                {
                    secretWordInvisibleArray[i] = secretWordArray[i];
                }
            }
            SecretWord_Invisible = new string(secretWordInvisibleArray);
            Console.WriteLine(SecretWord_Invisible); //test
        }

        if (lifes <= 0)
        {
            stop = 1;
            complete = false;
            
        }
        if (SecretWord == SecretWord_Invisible)
        {
            stop = 2;
            Console.WriteLine("Good Job!");
            complete = true;
            

        }
        return complete;
        // NO Console.Write() etc. in here!
    }

    private void HangTheMan(ref string SecretWord_Invisible, ref int lifes, ref string guessed, ref string missed, ref string SecretWord, ref int level) // Modification of method declaration recommended: Add return value and parameters
                                                                                                                                         // In here, clear the screen and redraw everything reflecting the actual game status
    {
        string? heart_1 = @"
    ,d88b.d88b,    XXXXXXXXX    XXXXXXXXX   XXXXXXXXX   XXXXXXXXX   XXXXXXXXX
   888888888888    XXXXXXXXX    XXXXXXXXX   XXXXXXXXX   XXXXXXXXX   XXXXXXXXX
    `Y8888888Y'    XXXXXXXXX    XXXXXXXXX   XXXXXXXXX   XXXXXXXXX   XXXXXXXXX
      `Y888Y'      XXXXXXXXX    XXXXXXXXX   XXXXXXXXX   XXXXXXXXX   XXXXXXXXX
        `Y'        XXXXXXXXX    XXXXXXXXX   XXXXXXXXX   XXXXXXXXX   XXXXXXXXX
";
        string? heart_2 = @"
    ,d88b.d88b,     ,d88b.d88b,    XXXXXXXXX    XXXXXXXXX   XXXXXXXXX   
   88888888888     888888888888    XXXXXXXXX    XXXXXXXXX   XXXXXXXXX
    `Y8888888Y'     `Y8888888Y'    XXXXXXXXX    XXXXXXXXX   XXXXXXXXX
      `Y888Y'         `Y888Y'      XXXXXXXXX    XXXXXXXXX   XXXXXXXXX
        `Y'             `Y'        XXXXXXXXX    XXXXXXXXX   XXXXXXXXX
";
        string? heart_3 = @"
    ,d88b.d88b,     ,d88b.d88b,     ,d88b.d88b,     XXXXXXXXX   XXXXXXXXX
   88888888888     888888888888    888888888888     XXXXXXXXX   XXXXXXXXX
    `Y8888888Y'     `Y8888888Y'     `Y8888888Y'     XXXXXXXXX   XXXXXXXXX
      `Y888Y'         `Y888Y'         `Y888Y'       XXXXXXXXX   XXXXXXXXX
        `Y'             `Y'             `Y'         XXXXXXXXX   XXXXXXXXX
";
        string? heart_4 = @"
    ,d88b.d88b,     ,d88b.d88b,     ,d88b.d88b,     ,d88b.d88b,     XXXXXXXXX
   88888888888     888888888888    888888888888     888888888888    XXXXXXXXX
    `Y8888888Y'     `Y8888888Y'     `Y8888888Y'     `Y8888888Y'     XXXXXXXXX
      `Y888Y'         `Y888Y'         `Y888Y'         `Y888Y'       XXXXXXXXX
        `Y'             `Y'             `Y'             `Y'         XXXXXXXXX
";
        string? heart_5 = @"
    ,d88b.d88b,     ,d88b.d88b,     ,d88b.d88b,     ,d88b.d88b,       ,d88b.d88b,
   88888888888     888888888888    888888888888     888888888888     888888888888
    `Y8888888Y'     `Y8888888Y'     `Y8888888Y'     `Y8888888Y'       `Y8888888Y'
      `Y888Y'         `Y888Y'         `Y888Y'         `Y888Y'           `Y888Y'
        `Y'             `Y'             `Y'             `Y'               `Y'
";
        string? heart_0 = @"
    G A M E   O V E R
";
        Console.Clear();
        Console.WriteLine("level:" + level);
        Console.WriteLine("secret word:");
        Console.WriteLine(SecretWord_Invisible);
        Console.WriteLine("missed:");
        Console.WriteLine(missed);
        if (lifes == 5)
        {
            Console.WriteLine(heart_5);
        }
        else if (lifes ==4)
        {
            Console.WriteLine(heart_4);
        }
        else if (lifes == 3)
        {
            Console.WriteLine(heart_3);
        }
        else if (lifes == 2)
        {
            Console.WriteLine(heart_2);
        }
        else if (lifes == 1)
        {
            Console.WriteLine(heart_1);
        }
        else if (lifes == 0)
        {
            Console.Clear();
            Console.WriteLine("missed:");
            Console.WriteLine(missed);
            Console.WriteLine("secret word:");
            Console.WriteLine(SecretWord);

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(heart_0);
            }
        }
    }
}