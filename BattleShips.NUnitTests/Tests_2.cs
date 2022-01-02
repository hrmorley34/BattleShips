using NUnit.Framework;
using System;
using System.Linq;
using BattleShips;
using BattleShips.Boards;
using BattleShips.Player;
using BattleShips.Util;

namespace BattleShips.NUnitTests
{
    public class PrintlessBoatsInput : BoatsInput
    {
        public PrintlessBoatsInput(int[] boatLengths, Board board) : base(boatLengths, board) { }

        public override void PrintCurrent(int xPad = Board.RenderXPad, int yPad = Board.RenderYPad) { }

        public void Test_2_1_2()
        {
            for (int i = 0; i < 9; i++)
                HandleKey(new ConsoleKeyInfo('\x00', ConsoleKey.RightArrow, false, false, false));

            Assert.That(CurrentCoordinates, Is.EqualTo(new Coordinates(7, 0)));

            for (int i = 0; i < 10; i++)
                HandleKey(new ConsoleKeyInfo('\x00', ConsoleKey.DownArrow, false, false, false));

            Assert.That(CurrentCoordinates, Is.EqualTo(new Coordinates(7, 7)));

            for (int i = 0; i < 6; i++)
                HandleKey(new ConsoleKeyInfo('\x00', ConsoleKey.LeftArrow, false, false, false));

            Assert.That(CurrentCoordinates, Is.EqualTo(new Coordinates(1, 7)));

            for (int i = 0; i < 5; i++)
                HandleKey(new ConsoleKeyInfo('\x00', ConsoleKey.UpArrow, false, false, false));

            Assert.That(CurrentCoordinates, Is.EqualTo(new Coordinates(1, 2)));
        }

        public void Test_2_1_4()
        {
            Assert.That(Values, Has.Count.EqualTo(0));

            HandleKey(new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false));
            HandleKey(new ConsoleKeyInfo('6', ConsoleKey.D6, false, false, false));
            Coordinates position = CurrentCoordinates;
            HandleKey(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));

            Assert.That(Values, Has.Count.EqualTo(1));

            HandleKey(new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false));

            Assert.That(Values, Has.Count.EqualTo(0));
            Assert.That(CurrentCoordinates, Is.EqualTo(position));
        }

        public void Test_2_2()
        {
            HandleKey(new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false));
            HandleKey(new ConsoleKeyInfo('6', ConsoleKey.D6, false, false, false));
            HandleKey(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));

            Assert.That(Values, Has.Count.EqualTo(1));

            HandleKey(new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false));
            HandleKey(new ConsoleKeyInfo('6', ConsoleKey.D6, false, false, false));

            Assert.That(IsValid(), Is.False);

            HandleKey(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));

            Assert.That(Values, Has.Count.EqualTo(1));
        }

        public void Test_2_4()
        {
            for (int offset = 0; offset < 5; offset++)
            {
                HandleKey(new ConsoleKeyInfo('D', ConsoleKey.D, false, false, false));
                HandleKey(new ConsoleKeyInfo((char)('1' + offset), ConsoleKey.D1 + offset, false, false, false));
                HandleKey(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));
            }

            Assert.That(Values, Has.Count.EqualTo(5));
            Assert.That(Finished());
            Assert.That(GetReturnValue(), Has.Count.EqualTo(5));
        }
    }

    public class Tests_2
    {
        [Test]
        // Test 2.1.2
        public void TestBoatsInput_2_1_2()
        {
            Game game = new Game(new IPlayer[1] { new ComputerPlayer() });
            Board board = game.Boards.First();
            PrintlessBoatsInput boatsInput = new PrintlessBoatsInput(new int[1] { 1 }, board);

            boatsInput.Test_2_1_2();
        }

        [Test]
        // Test 2.1.4
        public void TestBoatsInput_2_1_4()
        {
            Game game = new Game(new IPlayer[1] { new ComputerPlayer() });
            Board board = game.Boards.First();
            PrintlessBoatsInput boatsInput = new PrintlessBoatsInput(new int[2] { 1, 1 }, board);

            boatsInput.Test_2_1_4();
        }

        [Test]
        // Test 2.2
        public void TestBoatsInput_2_2()
        {
            Game game = new Game(new IPlayer[1] { new ComputerPlayer() });
            Board board = game.Boards.First();
            PrintlessBoatsInput boatsInput = new PrintlessBoatsInput(new int[2] { 1, 1 }, board);

            boatsInput.Test_2_2();
        }

        [Test]
        // Test 2.4
        public void TestBoatsInput_2_4()
        {
            Game game = new Game(new IPlayer[1] { new ComputerPlayer() });
            Board board = game.Boards.First();
            PrintlessBoatsInput boatsInput = new PrintlessBoatsInput(new int[5] { 1, 1, 1, 1, 1 }, board);

            boatsInput.Test_2_4();
        }
    }
}