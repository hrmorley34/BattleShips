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
        static readonly InstructionPager Instructions = new InstructionPager(new IEnumerable<InstructionDataToken>[] {
            new InstructionDataToken[] {
                InstructionDataToken.ColourToken(ConsoleColor.Cyan),
                InstructionDataToken.TextToken("Instructions"),
            },
            new InstructionDataToken[] {
                InstructionDataToken.ColourToken(ConsoleColor.Cyan),
                InstructionDataToken.TextToken("============"),
            },
            new InstructionDataToken[] { InstructionDataToken.ColourToken() },
            InstructionDataToken.ParseStringLine("Example text goes here. I should put something here."),
            InstructionDataToken.ParseStringLine("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."),
        });

        static readonly int[] BoatLengths = { 1, 1, 1, 1, 1 };

        static void Main(string[] args)
        {
            // Clear the screen
            for (int i = 0; i < Console.WindowHeight; i++) Console.WriteLine();
            Console.SetCursorPosition(0, 0);

            MenuOption option;
            while ((option = (new Menu()).ReadKeys()) != MenuOption.Exit)
            {
                Console.Clear();
                switch (option)
                {
                    case MenuOption.BeginNew:
                        {
                            IPlayer[] players = new IPlayer[2] { new HumanPlayer(), new ComputerPlayer() };
                            Game game = new Game(players);
                            game.MainSetup(BoatLengths);

                            Serialiser<Game> serialiser = new Serialiser<Game>("SaveData.xml");
                            game.Serialiser = serialiser;
                            game.MainLoop();
                            break;
                        }
                    case MenuOption.Continue:
                        {
                            Serialiser<Game> serialiser = new Serialiser<Game>("SaveData.xml");
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
                }
            }

            Console.Clear();
        }
    }
}
