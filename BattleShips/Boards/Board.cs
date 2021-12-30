using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BattleShips.Player;
using BattleShips.Util;
using ConsoleUtils.ConsoleImagery;

namespace BattleShips.Boards
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(EmptyBoardElement))]
    [KnownType(typeof(BoatElement))]
    [KnownType(typeof(ComputerPlayer))]
    [KnownType(typeof(HumanPlayer))]
    public class Board
    {
        [IgnoreDataMember]
        public const int DEFAULT_SIZE = 8;

        [IgnoreDataMember]
        protected IBoardElement[,] Elements;

        [IgnoreDataMember]
        public int XSize { get => Elements.GetLength(0); }
        [IgnoreDataMember]
        public int YSize { get => Elements.GetLength(1); }

        [DataMember(Name = "Data")]
        protected (IBoardElement[], int) SerialisableData
        {
            get
            {
                IBoardElement[] outArray = new IBoardElement[XSize * YSize];
                for (int y = 0; y < YSize; y++)
                    for (int x = 0; x < XSize; x++)
                        outArray[y * XSize + x] = Elements[x, y];
                return (outArray, XSize);
            }
            set
            {
                (IBoardElement[] inArray, int xSize) = value;
                if (inArray.Length % xSize != 0) throw new ArgumentException();
                Elements = new IBoardElement[xSize, inArray.Length / xSize];
                for (int y = 0; y < YSize; y++)
                    for (int x = 0; x < XSize; x++)
                        Elements[x, y] = inArray[y * XSize + x];
            }
        }

        [DataMember]
        public IPlayer Owner;

        public Board(IPlayer owner, int size = DEFAULT_SIZE)
        {
            Owner = owner;
            Elements = new IBoardElement[size, size];
            FillWithEmpty();
        }

        public Board(IPlayer owner, int xsize, int ysize)
        {
            Owner = owner;
            Elements = new IBoardElement[xsize, ysize];
            FillWithEmpty();
        }

        protected void FillWithEmpty()
        {
            for (int x = 0; x < XSize; x++)
            {
                for (int y = 0; y < YSize; y++)
                {
                    Elements[x, y] = new EmptyBoardElement();
                }
            }
        }

        public IBoardElement Get(int x, int y)
            => Elements[x, y];
        public IBoardElement Get(Coordinates coords)
            => Get(coords.X, coords.Y);
        public IBoardElement Get((int, int) coords)
            => Get(coords.Item1, coords.Item2);

        public IBoardElement Set(int x, int y, IBoardElement value)
            => Elements[x, y] = value;
        public IBoardElement Set(Coordinates coords, IBoardElement value)
            => Set(coords.X, coords.Y, value);
        public IBoardElement Set((int, int) coords, IBoardElement value)
            => Set(coords.Item1, coords.Item2, value);

        protected IEnumerable<IBoardElement> IterRow(int y)
        {
            for (int x = 0; x < XSize; x++)
            {
                yield return Get(x, y);
            }
        }

        public IEnumerable<IEnumerable<IBoardElement>> IterRows()
        {
            for (int y = 0; y < YSize; y++)
            {
                yield return IterRow(y);
            }
        }

        public IEnumerable<Coordinates> IterCoords()
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    yield return new Coordinates(x, y);
                }
            }
        }

        public const int RenderXPad = 3;
        public const int RenderYPad = 1;

        public (int, int) GetRenderCoordinates(Coordinates coords, int xPad = RenderXPad, int yPad = RenderYPad)
            => (xPad + 1 + coords.X * 2, yPad + coords.Y);

        public ColoredTextImage Render(bool enemy, int xPad = RenderXPad, int yPad = RenderYPad)
        {
            // ---_A_B_C ..._H-
            //   1_x_x_x ..._x-
            //   2_x_x_x ..._x-
            // ... . . . ... ..

            ColoredTextImage image = new ColoredTextImage(xPad + XSize * 2 + 1, yPad + YSize);

            for (int x = 0; x < XSize; x++)
            {
                ColoredTextImage text = ColoredTextImage.Text(CoordinatesLetterNumber.IntToAlphas(x), Colours.EvenOddPair(x));
                image = image.Overlay(text, (xPad + 2 + x * 2 - text.XSize, 0));
            }

            for (int y = 0; y < YSize; y++)
            {
                ColoredTextImage text = ColoredTextImage.Text((y + 1).ToString(), Colours.EvenOddPair(y));
                image = image.Overlay(text, (xPad - text.YSize, yPad + y));
            }

            foreach (Coordinates coords in IterCoords())
            {
                ColoredTextImage text = Colours.RenderElement(Get(coords), enemy);
                image = image.Overlay(text, GetRenderCoordinates(coords, xPad: xPad, yPad: yPad));
            }

            return image;
        }

        public ColoredTextImage RenderSelectedCoordinates(Coordinates coords, bool enemy, ConsoleColorPair? color = null, int xPad = RenderXPad, int yPad = RenderYPad)
        {
            (int x, int y) = GetRenderCoordinates(coords, xPad: xPad, yPad: yPad);
            return Render(enemy: enemy, xPad: xPad, yPad: yPad)
                .Overlay(ColoredTextImage.Text("(", color), (x - 1, y))
                .Overlay(ColoredTextImage.Text(")", color), (x + 1, y));
        }
    }
}