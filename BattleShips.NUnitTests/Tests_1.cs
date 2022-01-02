using NUnit.Framework;
using System;
using BattleShips;
using BattleShips.Player;
using ConsoleUtils.ConsoleImagery;

namespace BattleShips.NUnitTests
{
    public class PrintlessMenu : Menu
    {
        public PrintlessMenu(Serialiser<Game> serialiser, Settings settings) : base(serialiser: serialiser, settings: settings) { }

        public override void PrintMenu() { }
    }

    public class Tests_1
    {
        Serialiser<Game> Serialiser;

        public Tests_1()
        {
            Serialiser = new Serialiser<Game>("Tests.savegame");
        }

        [SetUp]
        public void Setup()
        {
            Serialiser.Clear();
        }

        [Test]
        // Test 1.1
        public void TestMenu_1_1()
        {
            Settings settings = new Settings();
            Menu menu = new PrintlessMenu(Serialiser, settings);

            for (int i = 0; i < 5; i++)
                menu.HandleKey(new ConsoleKeyInfo('\x00', ConsoleKey.DownArrow, false, false, false));
            menu.HandleKey(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));

            Assert.That(menu.Finished());
            Assert.That(menu.GetReturnValue(), Is.EqualTo(MenuOption.BeginNew));
        }

        [Test]
        // Test 1.2
        public void TestMenu_1_2()
        {
            Settings settings = new Settings();
            Menu menu = new PrintlessMenu(Serialiser, settings);

            menu.HandleKey(new ConsoleKeyInfo('4', ConsoleKey.D4, false, false, false));

            Assert.That(menu.Finished());
            Assert.That(menu.GetReturnValue(), Is.EqualTo(MenuOption.Settings));
        }

        [Test]
        // Test 1.3, 1.4
        public void TestMenu_1_3()
        {
            Settings settings = new Settings();
            Menu menu = new PrintlessMenu(Serialiser, settings);

            Assert.That(menu.GetMenuOptionColour(MenuOption.Continue), Is.EqualTo(new ConsoleColorPair(ConsoleColor.DarkGray)));

            Serialiser.SerialiseObject(new Game(new IPlayer[1] { new ComputerPlayer() }));

            Assert.That(menu.GetMenuOptionColour(MenuOption.Continue), Is.Not.EqualTo(new ConsoleColorPair(ConsoleColor.DarkGray)));

            Serialiser.Clear();
        }

        [Test]
        // Test 1.5
        public void TestMenu_1_5()
        {
            Settings settings = new Settings();
            Menu menu = new PrintlessMenu(Serialiser, settings);

            menu.HandleKey(new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false));

            Assert.That(menu.Finished());
            Assert.That(menu.GetReturnValue(), Is.EqualTo(MenuOption.Instructions));
        }

        [Test]
        // Test 1.6
        public void TestMenu_1_6()
        {
            Settings settings = new Settings();
            Menu menu = new PrintlessMenu(Serialiser, settings);

            menu.HandleKey(new ConsoleKeyInfo('0', ConsoleKey.D0, false, false, false));

            Assert.That(menu.Finished());
            Assert.That(menu.GetReturnValue(), Is.EqualTo(MenuOption.Exit));
        }
    }
}