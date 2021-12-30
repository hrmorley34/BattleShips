using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUtils.ConsoleImagery;
using ConsoleUtils.ConsoleKeyInteractions;

namespace BattleShips
{
    public enum MenuOption
    {
        BeginNew = 1,
        Continue = 2,
        Instructions = 3,
        Exit = 0,
    }

    public class Menu : IKeyHandler<MenuOption>
    {
        protected bool HasFinished = false;
        protected int _SelectedOption = 0;
        protected int SelectedOption
        {
            get => _SelectedOption;
            set => _SelectedOption = ((value % OptionArray.Length) + OptionArray.Length) % OptionArray.Length;
        }
        protected MenuOption SelectedMenuOption
        {
            get => OptionArray[SelectedOption];
            set => SelectedOption = Array.IndexOf(OptionArray, value);
        }
        protected MenuOption FinalOption;

        public static readonly MenuOption[] OptionArray = { MenuOption.BeginNew, MenuOption.Continue, MenuOption.Instructions, MenuOption.Exit };
        public static readonly Dictionary<ConsoleKey, MenuOption> OptionKeys = new Dictionary<ConsoleKey, MenuOption>
        {
            {ConsoleKey.D1, MenuOption.BeginNew}, /* {ConsoleKey.NumPad1, MenuOption.BeginNew}, */
            {ConsoleKey.D2, MenuOption.Continue}, /* {ConsoleKey.NumPad2, MenuOption.Continue}, */
            {ConsoleKey.D3, MenuOption.Instructions}, /* {ConsoleKey.NumPad3, MenuOption.Instructions}, */
            {ConsoleKey.D0, MenuOption.Exit}, /* {ConsoleKey.NumPad0, MenuOption.Exit}, */
        };
        public static readonly Dictionary<char, MenuOption> OptionChars = new Dictionary<char, MenuOption>
        {
            {'1', MenuOption.BeginNew},
            {'2', MenuOption.Continue},
            {'3', MenuOption.Instructions},
            {'0', MenuOption.Exit},
        };
        public const int OptionXLocation = 3;
        public static readonly Dictionary<MenuOption, int> OptionYLocations = new Dictionary<MenuOption, int>
        {
            {MenuOption.BeginNew, 1},
            {MenuOption.Continue, 2},
            {MenuOption.Instructions, 3},
            {MenuOption.Exit, 5},
        };
        public static readonly ConsoleKey[] UpArrows = { ConsoleKey.LeftArrow, ConsoleKey.UpArrow, ConsoleKey.A, ConsoleKey.W };
        public static readonly ConsoleKey[] DownArrows = { ConsoleKey.RightArrow, ConsoleKey.DownArrow, ConsoleKey.D, ConsoleKey.S };
        public static readonly ConsoleKey[] EnterKeys = { ConsoleKey.Enter, ConsoleKey.Spacebar };

        public static readonly Dictionary<MenuOption, ConsoleColorPair> MenuColours = new Dictionary<MenuOption, ConsoleColorPair>
        {
            {MenuOption.BeginNew, new ConsoleColorPair(ConsoleColor.Green)},
            {MenuOption.Continue, new ConsoleColorPair(ConsoleColor.Yellow)},
            {MenuOption.Instructions, new ConsoleColorPair(ConsoleColor.Cyan)},
            {MenuOption.Exit, new ConsoleColorPair(ConsoleColor.Red)},
        };
        public static readonly Dictionary<MenuOption, ColoredTextImage> MenuTexts = new Dictionary<MenuOption, ColoredTextImage>
        {
            {MenuOption.BeginNew, ColoredTextImage.Text("1. New game", MenuColours[MenuOption.BeginNew])},
            {MenuOption.Continue, ColoredTextImage.Text("2. Resume game", MenuColours[MenuOption.Continue])},
            {MenuOption.Instructions, ColoredTextImage.Text("3. Instructions", MenuColours[MenuOption.Instructions])},
            {MenuOption.Exit, ColoredTextImage.Text("0. Exit", MenuColours[MenuOption.Exit])},
        };

        public ColoredTextImage GetMenu()
        {

            ColoredTextImage image = new ColoredTextImage(30, 7);
            foreach ((MenuOption m, ColoredTextImage t) in MenuTexts)
            {
                int y = OptionYLocations[m];
                image = image.Overlay(t, (OptionXLocation, y));
                if (SelectedMenuOption == m)
                    image = image.Overlay(ColoredTextImage.Text(">", MenuColours[m]), (OptionXLocation - 2, y));
            }
            return image;
        }

        public void PrintMenu()
        {
            Console.SetCursorPosition(0, 0);
            GetMenu().Print();
        }

        public bool HandleKey(ConsoleKeyInfo key)
        {
            if (Finished()) throw new FinishedException();

            if (OptionKeys.ContainsKey(key.Key))
            {
                FinalOption = OptionKeys[key.Key];
                HasFinished = true;
            }
            else if (UpArrows.Contains(key.Key))
            {
                SelectedOption--;
                PrintMenu();
            }
            else if (DownArrows.Contains(key.Key))
            {
                SelectedOption++;
                PrintMenu();
            }
            else if (EnterKeys.Contains(key.Key))
            {
                FinalOption = SelectedMenuOption;
                HasFinished = true;
            }

            return Finished();
        }

        public bool HandleKey(char c)
        {
            if (Finished()) throw new FinishedException();

            if (OptionChars.ContainsKey(c))
            {
                FinalOption = OptionChars[c];
                HasFinished = true;
            }
            SelectedOption++;
            return Finished();
        }

        public bool Finished() => HasFinished;

        public MenuOption GetReturnValue()
        {
            if (!Finished()) throw new NoValueException();
            return FinalOption;
        }

        public MenuOption ReadKeys()
        {
            Console.Clear();
            PrintMenu();
            return ReadKeysMethod.ReadKeys<MenuOption>(this);
        }
    }

    public class InstructionDataToken
    {
        public ConsoleColorPair? Colour = null;
        public string? Text = null;
        public bool Breakable = false;

        public InstructionDataToken() { }
        public static InstructionDataToken ColourToken()
            => new InstructionDataToken { Colour = ConsoleColorPair.Reset };
        public static InstructionDataToken ColourToken(ConsoleColorPair? colour)
            => new InstructionDataToken { Colour = colour ?? ConsoleColorPair.Reset };
        public static InstructionDataToken ColourToken(ConsoleColor? fg, ConsoleColor? bg = null)
            => new InstructionDataToken { Colour = new ConsoleColorPair(fg, bg) };

        public static InstructionDataToken TextToken(string text)
            => new InstructionDataToken { Text = text };

        public static InstructionDataToken WhitespaceToken(string whitespace)
            => new InstructionDataToken { Text = whitespace, Breakable = true };

        public ConsoleColorPair? GetColour() => Colour;
        public string? GetText() => Text;
        public bool GetBreakable() => Breakable;

        public static IEnumerable<InstructionDataToken> ParseStringLine(string line)
        {
            List<char> chars = new List<char>();
            bool LastWhitespace = char.IsWhiteSpace(line[0]);
            string data;
            foreach (char c in line)
            {
                if (char.IsWhiteSpace(c) != LastWhitespace)
                {
                    data = new string(chars.ToArray());
                    if (LastWhitespace)
                        yield return WhitespaceToken(data);
                    else
                        yield return TextToken(data);

                    chars.Clear();
                    LastWhitespace = char.IsWhiteSpace(c);
                }
                chars.Add(c);
            }
            data = new string(chars.ToArray());
            if (LastWhitespace)
                yield return WhitespaceToken(data);
            else
                yield return TextToken(data);
        }

        public static IEnumerable<IEnumerable<InstructionDataToken>> ParseString(string text)
            => text.Split('\n').Select(ParseStringLine);

        public static IEnumerable<IEnumerable<InstructionDataToken>> ParseColoredTextImage(ColoredTextImage image)
        {
            // image of coloured chars => alternating ColourToken, TextToken
            return image.IterRows().Select(
                r => r.Aggregate(
                    (IEnumerable<InstructionDataToken>)new List<InstructionDataToken>(),
                    (l, c) =>
                    {
                        c ??= new ColoredChar();
                        return l.Append(ColourToken(c.Color)).Append(TextToken(c.Char.ToString()));
                    }));
        }
    }

    public class InstructionPager
    {
        public List<List<InstructionDataToken>> Lines;

        public InstructionPager()
        {
            Lines = new List<List<InstructionDataToken>>();
        }

        public InstructionPager(IEnumerable<IEnumerable<InstructionDataToken>> lines)
        {
            Lines = lines.Select(l => l.ToList()).ToList();
        }

        public InstructionPager(string text)
        {
            Lines = InstructionDataToken.ParseString(text).Select(l => l.ToList()).ToList();
        }

        public void Add(IEnumerable<InstructionDataToken> line)
        {
            Lines.Add(line.ToList());
        }

        public void Add(IEnumerable<IEnumerable<InstructionDataToken>> lines)
        {
            Lines.AddRange(lines.Select(l => l.ToList()));
        }

        public void Add(string text) => Add(InstructionDataToken.ParseString(text));

        public static IEnumerable<(bool, ColoredTextImage)> GroupTokens(IEnumerable<InstructionDataToken> tokenRow)
        {
            if (tokenRow.Count() <= 0) yield break;

            ConsoleColorPair? lastColour = null;
            bool lastBreakable = tokenRow.First().GetBreakable();
            ColoredTextImage image = new ColoredTextImage(0, 1);
            foreach (InstructionDataToken token in tokenRow)
            {
                if (token.GetColour() != null)
                    lastColour = token.GetColour();
                string? text = token.GetText();
                if (text != null)
                {
                    if (token.GetBreakable() != lastBreakable)
                    {
                        yield return (lastBreakable, image);

                        image = new ColoredTextImage(0, 1);
                        lastBreakable = token.GetBreakable();
                    }
                    image += ColoredTextImage.Text(text, lastColour);
                }
            }
            if (image.XSize > 0)
                yield return (lastBreakable, image);
        }

        public static IEnumerable<(ColoredTextImage?, ColoredTextImage?)> GroupPairTokens(IEnumerable<InstructionDataToken> tokenRow)
            => GroupPairTokens(GroupTokens(tokenRow));
        public static IEnumerable<(ColoredTextImage?, ColoredTextImage?)> GroupPairTokens(IEnumerable<(bool, ColoredTextImage)> groupedTokens)
        {
            ColoredTextImage? breakableImage = null, nonBreakableImage = null;
            foreach ((bool breakable, ColoredTextImage image) in groupedTokens)
            {
                if (breakable)
                {
                    if (breakableImage != null)
                    {
                        yield return (breakableImage, nonBreakableImage);
                        breakableImage = nonBreakableImage = null;
                    }
                    breakableImage = image;
                }
                else
                {
                    nonBreakableImage = image;
                    yield return (breakableImage, nonBreakableImage);
                    breakableImage = nonBreakableImage = null;
                }
            }
            if (breakableImage != null || nonBreakableImage != null)
                yield return (breakableImage, nonBreakableImage);
        }

        public ColoredTextImage WrapLines(int width)
        {
            int currentColumn;
            List<ColoredChar?[]> image = new List<ColoredChar?[]>();

            void NewLine()
            {
                currentColumn = 0;
                image.Add(new ColoredChar?[width]);
            }
            ColoredChar?[] CurrentLine() => image.Last();
            void AppendArray(ColoredChar?[] array)
            {
                // if (array.Length == 0) return;
                array.CopyTo(CurrentLine(), currentColumn);
                currentColumn += array.Length;
            }

            foreach (List<InstructionDataToken> line in Lines)
            {
                NewLine();
                foreach ((ColoredTextImage? breakableImage, ColoredTextImage? nonBreakableImage) in GroupPairTokens(line))
                {
                    ColoredChar?[] breakableArray =
                        breakableImage == null
                        ? new ColoredChar?[0] { }
                        : breakableImage.IterRows().Single().ToArray();
                    ColoredChar?[] nonBreakableArray =
                        nonBreakableImage == null
                        ? new ColoredChar?[0] { }
                        : nonBreakableImage.IterRows().Single().ToArray();

                    int preCurrentColumn = currentColumn;
                    if (currentColumn + breakableArray.Length + nonBreakableArray.Length <= width)
                    {
                        AppendArray(breakableArray);
                        AppendArray(nonBreakableArray);
                    }
                    else if (nonBreakableArray.Length > width)
                    {
                        if (currentColumn + breakableArray.Length <= width)
                        {
                            AppendArray(breakableArray);
                        }
                        foreach (ColoredChar? c in nonBreakableArray)
                        {
                            if (currentColumn >= width)
                            {
                                NewLine();
                            }
                            CurrentLine()[currentColumn] = c;
                            currentColumn++;
                        }
                    }
                    else
                    {
                        if (currentColumn + breakableArray.Length <= width)
                        {
                            AppendArray(breakableArray);
                        }
                        NewLine();
                        AppendArray(nonBreakableArray);
                    }
                }
            }
            return new ColoredTextImage(image);
        }

        public void Print(int xBorder = 1, int yBorder = 0)
        {
            int wrap = Console.WindowWidth - 2 * xBorder;
            ColoredTextImage image = WrapLines(wrap);
            image = new ColoredTextImage(image.XSize + 2 * xBorder, image.YSize + 2 * yBorder).Overlay(image, (xBorder, yBorder));
            image.Print();
            Console.ReadKey(true);
        }
    }
}