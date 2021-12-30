using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BattleShips.Boards;
using BattleShips.Util;
using ConsoleUtils.ConsoleImagery;
using ConsoleUtils.ConsoleKeyInteractions;

namespace BattleShips.Player
{
    public class BoatsInput : IKeyHandler<IEnumerable<(Coordinates, bool)>>
    {
        public static readonly Dictionary<ConsoleKey, Vector> MoveDirectionKeys = new Dictionary<ConsoleKey, Vector>
        {
            {ConsoleKey.UpArrow, new Vector(0, -1)},
            {ConsoleKey.LeftArrow, new Vector(-1, 0)},
            {ConsoleKey.DownArrow, new Vector(0, 1)},
            {ConsoleKey.RightArrow, new Vector(1, 0)},
        };
        public static readonly HashSet<ConsoleKey> RotateKeys = new HashSet<ConsoleKey>
        {
            ConsoleKey.R,
            ConsoleKey.Tab,
        };
        public static readonly HashSet<ConsoleKey> NextKeys = new HashSet<ConsoleKey>
        {
            ConsoleKey.Enter,
            ConsoleKey.Spacebar,
        };
        public static readonly HashSet<ConsoleKey> BackKeys = new HashSet<ConsoleKey>
        {
            ConsoleKey.Backspace,
            ConsoleKey.Delete,
        };

        protected List<(Coordinates, bool)> Values;

        protected Coordinates CurrentCoordinates;
        protected bool CurrentIsVertical;

        public readonly Board Board;
        public readonly int[] BoatLengths;
        protected int BoatIndex;
        protected int CurrentBoatLength { get => BoatLengths[BoatIndex]; }

        public BoatsInput(int[] boatLengths, Board board)
        {
            Values = new List<(Coordinates, bool)>();

            CurrentCoordinates = Coordinates.Origin;
            ResetCurrent();

            Board = board;
            BoatLengths = boatLengths;
            BoatIndex = 0;
        }

        protected void ResetCurrent()
        {
            CurrentCoordinates = Coordinates.Origin;
            CurrentIsVertical = false;
        }

        protected void MoveCurrent(Vector vector)
        {
            SetCurrent(CurrentCoordinates + vector);
        }
        protected void SetCurrent(Coordinates coordinates)
        {
            SetCurrentX(coordinates.X);
            SetCurrentY(coordinates.Y);
        }
        protected void SetCurrentX(int x)
        {
            int maxX = CurrentIsVertical ? x + 1 : x + CurrentBoatLength;
            if (x < 0) x = 0;
            else if (maxX > Board.XSize) x += Board.XSize - maxX;
            CurrentCoordinates = new Coordinates(x: x, y: CurrentCoordinates.Y);
        }
        protected void SetCurrentY(int y)
        {
            int maxY = CurrentIsVertical ? y + CurrentBoatLength : y + 1;
            if (y < 0) y = 0;
            else if (maxY > Board.YSize) y += Board.YSize - maxY;
            CurrentCoordinates = new Coordinates(x: CurrentCoordinates.X, y: y);
        }

        protected void RotateCurrent()
        {
            int halfLength = CurrentBoatLength / 2;
            if (CurrentIsVertical) halfLength = -halfLength;

            Coordinates coordinates = new Coordinates(
                CurrentCoordinates.X - halfLength,
                CurrentCoordinates.Y + halfLength);
            CurrentIsVertical = !CurrentIsVertical;

            SetCurrent(coordinates);
        }

        protected HashSet<Coordinates> GetOccupied()
        {
            HashSet<Coordinates> occupied = new HashSet<Coordinates>();
            foreach ((int length, (Coordinates baseCoordinates, bool isVertical)) in BoatLengths.Zip(Values))
            {
                foreach ((int _, Coordinates coordinates) in IterCoordinates(baseCoordinates, isVertical, length))
                {
                    occupied.Add(coordinates);
                }
            }
            return occupied;
        }

        protected bool IsValid()
        {
            HashSet<Coordinates> occupied = GetOccupied();
            foreach ((int _, Coordinates coordinates) in IterCurrentCoordinates())
            {
                if (occupied.Contains(coordinates))
                    return false;
            }
            return true;
        }

        public static IEnumerable<(int, Coordinates)> IterCoordinates(Coordinates coordinates, bool isVertical, int length)
        {
            for (int index = 0; index < length; index++)
            {
                yield return (index, new Coordinates(
                    isVertical ? coordinates.X : coordinates.X + index,
                    isVertical ? coordinates.Y + index : coordinates.Y));
            }
        }
        public static IEnumerable<(Coordinates, BoatElement)> IterCoordinates(Coordinates coordinates, bool isVertical, Boat boat)
        {
            foreach ((int index, Coordinates partCoordinates) in BoatsInput.IterCoordinates(coordinates, isVertical, boat.BoatElements.Length))
            {
                yield return (partCoordinates, boat.BoatElements[index]);
            }
        }
        protected IEnumerable<(int, Coordinates)> IterCurrentCoordinates()
        {
            if (BoatIndex < BoatLengths.Length)
                return IterCoordinates(CurrentCoordinates, CurrentIsVertical, CurrentBoatLength);
            return new (int, Coordinates)[0];
        }

        public ColoredTextImage RenderCurrent(int xPad = Board.RenderXPad, int yPad = Board.RenderYPad)
        {
            ColoredTextImage boardImage = Board.Render(enemy: false, xPad: xPad, yPad: yPad);
            var occupied = GetOccupied();
            var current = IterCurrentCoordinates();

            foreach (Coordinates coords in occupied)
            {
                boardImage = boardImage.Overlay(
                    ColoredTextImage.Text("B", new ConsoleColorPair(ConsoleColor.Cyan)),
                    Board.GetRenderCoordinates(coords, xPad: xPad, yPad: yPad));
            }

            foreach ((int _, Coordinates coords) in current)
            {
                ConsoleColorPair colour = new ConsoleColorPair(
                    occupied.Contains(coords) ? ConsoleColor.Red : ConsoleColor.Green);
                boardImage = boardImage.Overlay(
                   ColoredTextImage.Text("B", colour),
                   Board.GetRenderCoordinates(coords, xPad: xPad, yPad: yPad));
            }

            ColoredTextImage coordsText
                = "Placing boat at ("
                + ColoredTextImage.Text(CoordinatesLetterNumber.IntToAlphas(CurrentCoordinates.X), Colours.EvenOddPair(CurrentCoordinates.X))
                + ", "
                + ColoredTextImage.Text(CoordinatesLetterNumber.IntToDigits(CurrentCoordinates.Y), Colours.EvenOddPair(CurrentCoordinates.Y))
                + ")";

            return new ColoredTextImage(Math.Max(boardImage.XSize, coordsText.XSize), boardImage.YSize + coordsText.YSize)
                .Overlay(boardImage, (0, 0))
                .Overlay(coordsText, (0, boardImage.YSize));
        }
        public void PrintCurrent(int xPad = Board.RenderXPad, int yPad = Board.RenderYPad)
        {
            Console.Clear();
            RenderCurrent(xPad: xPad, yPad: yPad).Print();
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (Finished()) throw new FinishedException();

            if (MoveDirectionKeys.ContainsKey(key.Key))
            {
                MoveCurrent(MoveDirectionKeys[key.Key]);
            }
            else if (RotateKeys.Contains(key.Key))
            {
                RotateCurrent();
            }
            else if (NextKeys.Contains(key.Key))
            {
                if (IsValid())
                {
                    BoatIndex++;
                    Values.Add((CurrentCoordinates, CurrentIsVertical));
                    ResetCurrent();
                }
            }
            else if (BackKeys.Contains(key.Key))
            {
                if (BoatIndex > 0)
                {
                    BoatIndex--;
                    (CurrentCoordinates, CurrentIsVertical) = Values.Last();
                    Values.RemoveAt(Values.Count() - 1);
                }
                else
                {
                    Values.Clear();  // shouldn't do anything
                    ResetCurrent();
                }
            }
            else if (CoordinatesLetterNumber.ValidAlphaKey(key.Key))
            {
                int x = CoordinatesLetterNumber.AlphaToInt(key.Key);
                if (0 <= x && x < Board.XSize)
                    SetCurrentX(x);
            }
            else if (CoordinatesLetterNumber.ValidDigitKey(key.Key))
            {
                int y = CoordinatesLetterNumber.DigitToInt(key.Key);
                if (0 <= y && y < Board.YSize)
                    SetCurrentY(y);
            }

            PrintCurrent();

            return Finished();
        }

        public bool HandleKey(char c)
        {
            // if (Finished()) throw new FinishedException();

            throw new NotImplementedException();

            // return Finished();
        }

        public bool Finished() => BoatIndex >= BoatLengths.Length;
        public IEnumerable<(Coordinates, bool)> GetReturnValue() => Values;

        public IEnumerable<(Coordinates, bool)> ReadKeys() => ReadKeysMethod.ReadKeys(this);
    }

    public class ShotInput : IKeyHandler<Coordinates>
    {
        public static readonly Dictionary<ConsoleKey, Vector> MoveDirectionKeys = new Dictionary<ConsoleKey, Vector>
        {
            {ConsoleKey.UpArrow, new Vector(0, -1)},
            {ConsoleKey.LeftArrow, new Vector(-1, 0)},
            {ConsoleKey.DownArrow, new Vector(0, 1)},
            {ConsoleKey.RightArrow, new Vector(1, 0)},
        };
        public static readonly HashSet<ConsoleKey> ConfirmKeys = new HashSet<ConsoleKey>
        {
            ConsoleKey.Enter,
            ConsoleKey.Spacebar,
        };

        protected Coordinates Coordinates;
        protected bool HasFinished;

        public readonly Board Board;

        public ShotInput(Board board)
        {
            Coordinates = new Coordinates(0, 0);
            Board = board;
        }

        protected void MoveCurrent(Vector vector)
        {
            SetCurrent(Coordinates + vector);
        }
        protected void SetCurrent(Coordinates coordinates)
        {
            SetCurrentX(coordinates.X);
            SetCurrentY(coordinates.Y);
        }
        protected void SetCurrentX(int x)
        {
            if (x < 0) x = 0;
            else if (x >= Board.XSize) x = Board.XSize - 1;
            Coordinates = new Coordinates(x: x, y: Coordinates.Y);
        }
        protected void SetCurrentY(int y)
        {
            if (y < 0) y = 0;
            else if (y >= Board.YSize) y = Board.YSize - 1;
            Coordinates = new Coordinates(x: Coordinates.X, y: y);
        }

        protected bool IsValid()
            => !Board.Get(Coordinates).HasShot();

        public ColoredTextImage RenderCurrent(int xPad = Board.RenderXPad, int yPad = Board.RenderYPad)
        {
            ConsoleColorPair colour = new ConsoleColorPair(IsValid() ? ConsoleColor.Green : ConsoleColor.Red);
            ColoredTextImage boardImage = Board.RenderSelectedCoordinates(
                Coordinates, enemy: true, color: colour, xPad: xPad, yPad: yPad);

            int gridMaxY = boardImage.YSize;
            ColoredTextImage coordsText
                = "Shooting ("
                + ColoredTextImage.Text(CoordinatesLetterNumber.IntToAlphas(Coordinates.X), Colours.EvenOddPair(Coordinates.X))
                + ", "
                + ColoredTextImage.Text(CoordinatesLetterNumber.IntToDigits(Coordinates.Y), Colours.EvenOddPair(Coordinates.Y))
                + ")";

            return new ColoredTextImage(Math.Max(boardImage.XSize, coordsText.XSize), gridMaxY + coordsText.YSize)
                .Overlay(boardImage, (0, 0))
                .Overlay(coordsText, (0, gridMaxY));
        }
        public void PrintCurrent(int xPad = Board.RenderXPad, int yPad = Board.RenderYPad)
        {
            Console.Clear();
            RenderCurrent(xPad: xPad, yPad: yPad).Print();
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (Finished()) throw new FinishedException();

            if (MoveDirectionKeys.ContainsKey(key.Key))
            {
                MoveCurrent(MoveDirectionKeys[key.Key]);
            }
            else if (ConfirmKeys.Contains(key.Key))
            {
                if (IsValid())
                {
                    HasFinished = true;
                }
            }
            else if (CoordinatesLetterNumber.ValidAlphaKey(key.Key))
            {
                int x = CoordinatesLetterNumber.AlphaToInt(key.Key);
                if (0 <= x && x < Board.XSize)
                    SetCurrentX(x);
            }
            else if (CoordinatesLetterNumber.ValidDigitKey(key.Key))
            {
                int y = CoordinatesLetterNumber.DigitToInt(key.Key);
                if (0 <= y && y < Board.YSize)
                    SetCurrentY(y);
            }

            PrintCurrent();

            return Finished();
        }

        public bool HandleKey(char c)
        {
            // if (Finished()) throw new FinishedException();

            throw new NotImplementedException();

            // return Finished();
        }

        public bool Finished() => HasFinished;
        public Coordinates GetReturnValue() => Coordinates;

        public Coordinates ReadKeys() => ReadKeysMethod.ReadKeys(this);
    }

    [DataContract(IsReference = true)]
    public class HumanPlayer : IPlayer
    {
        [DataMember]
        protected Board? BoatsBoard;
        [DataMember]
        protected Board? EnemyBoard;
        [DataMember]
        protected Boat[]? Boats;

        public HumanPlayer() { }

        public string GetName() => "Player";

        public void SetBoards(Board boatsBoard, Board enemyBoard)
        {
            BoatsBoard = boatsBoard;
            EnemyBoard = enemyBoard;
        }

        public Board GetOwnBoard() => BoatsBoard ?? throw new NullReferenceException();
        public Board GetEnemyBoard() => EnemyBoard ?? throw new NullReferenceException();

        protected bool? IsBoardEnemy(Board board)
            => board == GetOwnBoard() ? false : board == GetEnemyBoard() ? true : null;

        public void ShowBoard(Board board)
        {
            bool? enemy = IsBoardEnemy(board);
            if (enemy == null) return;
            board.Render(enemy.Value).Print();

            Console.ReadKey(true);
        }
        public void ShowBoardAttack(IPlayer aggressor, Board board, Coordinates shot)
        {
            bool? boardIsEnemy = IsBoardEnemy(board);
            if (boardIsEnemy == null) return;
            Console.Clear();

            board.RenderSelectedCoordinates(shot, boardIsEnemy.Value).Print();

            DisplayEnum d = board.Get(shot).GetDisplay();
            string name = (aggressor == this) ? "You" : aggressor.GetName();
            bool attackByEnemy = !boardIsEnemy.Value;
            (name
             + " shot ("
             + ColoredTextImage.Text(CoordinatesLetterNumber.IntToAlphas(shot.X), Colours.EvenOddPair(shot.X))
             + ", "
             + ColoredTextImage.Text(CoordinatesLetterNumber.IntToDigits(shot.Y), Colours.EvenOddPair(shot.Y))
             + "), "
             + Colours.RenderHitMissText(d, attackByEnemy)).Print();

            Console.ReadKey(true);
        }

        public IEnumerable<Boat> GetBoats() => Boats ?? throw new NullReferenceException();

        public void PlaceBoats(IEnumerable<Boat> boats)
        {
            Board board = GetOwnBoard();
            Boats = boats.ToArray();

            BoatsInput boatsInput = new BoatsInput(Boats.Select(b => b.BoatElements.Length).ToArray(), board);
            boatsInput.PrintCurrent();
            var positions = boatsInput.ReadKeys();

            foreach ((Boat b, (Coordinates coordinates, bool isVertical)) in Boats.Zip(positions))
            {
                foreach ((Coordinates partCoordinates, BoatElement boatElement) in BoatsInput.IterCoordinates(coordinates, isVertical, b))
                {
                    board.Set(partCoordinates, boatElement);
                }
            }
        }

        public Coordinates AskShoot()
        {
            Board board = GetEnemyBoard();
            ShotInput input = new ShotInput(board);
            input.PrintCurrent();
            return input.ReadKeys();
        }

        public void ShowWinState(IPlayer loser)
        {
            bool won = loser != this;
            Console.Clear();
            Colours.RenderWinText(won).Print();
        }
    }
}