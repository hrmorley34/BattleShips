using System;
using System.Collections.Generic;
using System.Linq;
using BattleShips;
using BattleShips.Player;
using ConsoleUtils.ConsoleImagery;

namespace BattleShips
{
    class Program
    {
        static readonly InstructionDataToken InstructionTokenResetColour = InstructionDataToken.ColourToken();
        static readonly InstructionPager Instructions = new InstructionPager(new IEnumerable<InstructionDataToken>[] {
            new InstructionDataToken[] {
                InstructionDataToken.ColourToken(ConsoleColor.Cyan),
                InstructionDataToken.TextToken("Instructions"),
            },
            new InstructionDataToken[] {
                InstructionDataToken.ColourToken(ConsoleColor.Cyan),
                InstructionDataToken.TextToken("============"),
            },
            new InstructionDataToken[] { InstructionTokenResetColour },
            InstructionDataToken.ParseStringLine("To play, select new game."),
            InstructionDataToken.ParseStringLine("You will be asked to enter your boats. Use the arrow keys or type in coordinates, and press enter or space to submit."),
            new InstructionDataToken[0],
            InstructionDataToken.ParseStringLine("In the main game, you will be asked where to shoot. Again, use the arrow keys or type in coordinates, and press enter or space to shoot."),
            InstructionDataToken.ParseStringLine("The game will tell you whether or not you hit anything, and whether you sunk a ship."),
            InstructionDataToken.ParseStringLine("The aim of the game is to sink all of your enemy's ships, and you lose when you have no ships left afloat."),
            new InstructionDataToken[0],
            InstructionDataToken.ParseStringLine("If you accidentally leave a game part way through, the game autosaves and you can use the Resume option on the main menu to continue where you left off."),
            new InstructionDataToken[0],
            InstructionDataToken.ParseStringLine("You can also switch between playing with 5 destroyers of length 1, or 2 destroyers (length 1), 2 submarines (length 2), and a carrier (length 3)."),
            new InstructionDataToken[0],
            new InstructionDataToken[] { InstructionDataToken.ColourToken(ConsoleColor.Cyan) }
                .Concat(InstructionDataToken.ParseStringLine("Have fun!")),
            new InstructionDataToken[] { InstructionTokenResetColour },
            new InstructionDataToken[] { InstructionDataToken.ColourToken(ConsoleColor.DarkGray) }
                .Concat(InstructionDataToken.ParseStringLine("Press any key to return to the menu")),
        });

        static void Main(string[] args)
        {
            // Clear the screen
            for (int i = 0; i < Console.WindowHeight; i++) Console.WriteLine();
            Console.SetCursorPosition(0, 0);

            Serialiser<Game> serialiser = new Serialiser<Game>("SaveData.xml");
            Settings settings = new Settings();
            MenuOption option;
            while ((option = (new Menu(serialiser: serialiser, settings: settings)).ReadKeys()) != MenuOption.Exit)
            {
                Console.Clear();
                switch (option)
                {
                    case MenuOption.BeginNew:
                        {
                            IPlayer[] players = new IPlayer[2] { new HumanPlayer(), new ComputerPlayer() };
                            Game game = new Game(players);
                            game.MainSetup(settings.CurrentLengths);

                            game.Serialiser = serialiser;
                            game.MainLoop();
                            break;
                        }
                    case MenuOption.Continue:
                        {
                            if (!serialiser.FileExists())
                            {
                                Console.Clear();
                                ColoredTextImage.Text("No past save file found!", new ConsoleColorPair(ConsoleColor.Red)).Print();
                                Console.ReadKey(true);
                                continue;
                            }

                            Game game = serialiser.LoadObject();
                            game.Serialiser = serialiser;
                            game.MainLoop();
                            break;
                        }
                    case MenuOption.Instructions:
                        {
                            Instructions.Print();
                            break;
                        }
                    case MenuOption.Settings:
                        {
                            settings.CurrentSettingId++;
                            break;
                        }
                }
            }

            Console.Clear();
        }
    }
}
