using NUnit.Framework;
using System;
using System.Linq;
using BattleShips;
using BattleShips.Boards;
using BattleShips.Player;
using BattleShips.Util;
using ConsoleUtils.ConsoleImagery;

namespace BattleShips.NUnitTests
{
    public class PrintlessShotInput : ShotInput
    {
        public PrintlessShotInput(Board board) : base(board) { }

        public override void PrintCurrent(int xPad = 3, int yPad = 1) { }

        public void Test_5_2()
        {
            HandleKey(new ConsoleKeyInfo('B', ConsoleKey.B, false, false, false));
            HandleKey(new ConsoleKeyInfo('5', ConsoleKey.D5, false, false, false));
            HandleKey(new ConsoleKeyInfo('\x00', ConsoleKey.DownArrow, false, false, false));
            HandleKey(new ConsoleKeyInfo('\x00', ConsoleKey.RightArrow, false, false, false));
            HandleKey(new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false));

            Assert.That(Finished());
            Assert.That(GetReturnValue(), Is.EqualTo(new Coordinates(2, 5)));
        }
    }

    public class Tests_5
    {
        [Test]
        // Test 5.2
        public void TestBoatsInput_5_2()
        {
            Game game = new Game(new IPlayer[1] { new ComputerPlayer() });
            Board board = game.Boards.First();
            PrintlessShotInput shotInput = new PrintlessShotInput(board);

            shotInput.Test_5_2();
        }
    }
}