using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUtils.ConsoleImagery;
using ConsoleUtils.ConsoleKeyInteractions;

namespace BattleShips
{
    /// <summary>Represents a possible boat lengths setting</summary>
    public enum BoatLengthSetting
    {
        Normal,
        Extension,
    }

    /// <summary>An object for holding/changing a <c>BoatLengthSetting</c></summary>
    public class Settings
    {
        public static BoatLengthSetting[] SettingsOrder = { BoatLengthSetting.Normal, BoatLengthSetting.Extension };
        public static Dictionary<BoatLengthSetting, int[]> BoatLengthsArrays = new Dictionary<BoatLengthSetting, int[]>
        {
            {BoatLengthSetting.Normal, new int[5] { 1, 1, 1, 1, 1 }},
            {BoatLengthSetting.Extension, new int[5] { 1, 1, 2, 2, 3 }},
        };
        public static Dictionary<BoatLengthSetting, string> LengthNames = new Dictionary<BoatLengthSetting, string>
        {
            {BoatLengthSetting.Normal, "Normal (5x1)"},
            {BoatLengthSetting.Extension, "Extension (2x1 2x2 1x3)"},
        };

        protected int _CurrentSettingId = 0;
        public int CurrentSettingId
        {
            get => _CurrentSettingId;
            // if goes out of range, wrap around
            set => _CurrentSettingId = ((value % SettingsOrder.Length) + SettingsOrder.Length) % SettingsOrder.Length;
        }
        public BoatLengthSetting CurrentSetting
        {
            get => SettingsOrder[_CurrentSettingId];
            set => _CurrentSettingId = Array.IndexOf(SettingsOrder, value);
        }

        public string CurrentLengthName { get => LengthNames[CurrentSetting]; }
        public int[] CurrentLengths { get => BoatLengthsArrays[CurrentSetting]; }
    }

    /// <summary>Represents possible menu options</summary>
    public enum MenuOption
    {
        BeginNew = 1,
        Continue = 2,
        Instructions = 3,
        Settings = 4,
        Exit = 0,
    }

    /// <summary>Handler for displaying the menu, allowing the user to switch between options</summary>
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

        public readonly Serialiser<Game> Serialiser;
        public readonly Settings Settings;

        /// <summary>The menu options, in order</summary>
        public static readonly MenuOption[] OptionArray = { MenuOption.BeginNew, MenuOption.Continue, MenuOption.Instructions, MenuOption.Settings, MenuOption.Exit };
        /// <summary>Number keys to select an option</summary>
        public static readonly Dictionary<ConsoleKey, MenuOption> OptionKeys = new Dictionary<ConsoleKey, MenuOption>
        {
            {ConsoleKey.D1, MenuOption.BeginNew}, /* {ConsoleKey.NumPad1, MenuOption.BeginNew}, */
            {ConsoleKey.D2, MenuOption.Continue}, /* {ConsoleKey.NumPad2, MenuOption.Continue}, */
            {ConsoleKey.D3, MenuOption.Instructions}, /* {ConsoleKey.NumPad3, MenuOption.Instructions}, */
            {ConsoleKey.D4, MenuOption.Settings}, /* {ConsoleKey.NumPad4, MenuOption.Config}, */
            {ConsoleKey.D0, MenuOption.Exit}, /* {ConsoleKey.NumPad0, MenuOption.Exit}, */
        };
        /// <summary>Number characters to select an option</summary>
        public static readonly Dictionary<char, MenuOption> OptionChars = new Dictionary<char, MenuOption>
        {
            {'1', MenuOption.BeginNew},
            {'2', MenuOption.Continue},
            {'3', MenuOption.Instructions},
            {'4', MenuOption.Settings},
            {'0', MenuOption.Exit},
        };
        /// <summary>X position of menu options</summary>
        public const int OptionXLocation = 3;
        /// <summary>Y positions of menu options</summary>
        public static readonly Dictionary<MenuOption, int> OptionYLocations = new Dictionary<MenuOption, int>
        {
            {MenuOption.BeginNew, 1},
            {MenuOption.Continue, 2},
            {MenuOption.Instructions, 3},
            {MenuOption.Settings, 4},
            {MenuOption.Exit, 6},
        };
        /// <summary>Keys to move up the menu</summary>
        public static readonly ConsoleKey[] UpArrows = { ConsoleKey.LeftArrow, ConsoleKey.UpArrow, ConsoleKey.A, ConsoleKey.W };
        /// <summary>Keys to move down the menu</summary>
        public static readonly ConsoleKey[] DownArrows = { ConsoleKey.RightArrow, ConsoleKey.DownArrow, ConsoleKey.D, ConsoleKey.S };
        /// <summary>Keys to finish selecting (by arrows)</summary>
        public static readonly ConsoleKey[] EnterKeys = { ConsoleKey.Enter, ConsoleKey.Spacebar };

        /// <summary>Get colours of menu options</summary>
        public ConsoleColorPair? GetMenuOptionColour(MenuOption option)
        {
            switch (option)
            {
                case MenuOption.BeginNew:
                    return new ConsoleColorPair(ConsoleColor.Green);
                case MenuOption.Continue:
                    return new ConsoleColorPair(Serialiser.FileExists() ? ConsoleColor.Yellow : ConsoleColor.DarkGray);
                case MenuOption.Instructions:
                    return new ConsoleColorPair(ConsoleColor.Cyan);
                case MenuOption.Settings:
                    return new ConsoleColorPair(ConsoleColor.Magenta);
                case MenuOption.Exit:
                    return new ConsoleColorPair(ConsoleColor.Red);
            }
            return null;
        }

        /// <summary>Get coloured text of menu options</summary>
        public ColoredTextImage GetMenuOptionText(MenuOption option)
        {
            string text = "UNKNOWN OPTION";
            switch (option)
            {
                case MenuOption.BeginNew:
                    text = "1. New game"; break;
                case MenuOption.Continue:
                    text = "2. Resume game"; break;
                case MenuOption.Instructions:
                    text = "3. Instructions"; break;
                case MenuOption.Settings:
                    text = "4. Mode: " + Settings.CurrentLengthName; break;
                case MenuOption.Exit:
                    text = "0. Exit"; break;
            }
            return ColoredTextImage.Text(text, GetMenuOptionColour(option));
        }

        public Menu(Serialiser<Game> serialiser, Settings settings)
        {
            Serialiser = serialiser;
            Settings = settings;
        }

        /// <summary>Put together all parts of the menu as one block of text</summary>
        public ColoredTextImage GetMenu()
        {
            ColoredTextImage image = new ColoredTextImage(40, 8);
            foreach ((MenuOption m, int y) in OptionYLocations)
            {
                image = image.Overlay(GetMenuOptionText(m), (OptionXLocation, y));
                if (SelectedMenuOption == m)
                    image = image.Overlay(ColoredTextImage.Text(">", GetMenuOptionColour(m)), (OptionXLocation - 2, y));
            }
            return image;
        }

        /// <summary>Print the menu out</summary>
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
                // select an option and immediately finish
                FinalOption = OptionKeys[key.Key];
                HasFinished = true;
            }
            else if (UpArrows.Contains(key.Key))
            {
                // go up an option, and re-print the menu
                SelectedOption--;
                PrintMenu();
            }
            else if (DownArrows.Contains(key.Key))
            {
                // go down an option and re-print the menu
                SelectedOption++;
                PrintMenu();
            }
            else if (EnterKeys.Contains(key.Key))
            {
                // finish with the current option
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

    /// <summary>Represents a part of a set of instructions</summary>
    public class InstructionDataToken
    {
        /// <summary>The colour to switch to</summary>
        public ConsoleColorPair? Colour = null;
        /// <summary>The text to represent</summary>
        public string? Text = null;
        /// <summary>Whether or not this text needs to be included, e.g. at a newline</summary>
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

        /// <summary>Split a line of text into alternating non-breakable and breakable tokens</summary>
        public static IEnumerable<InstructionDataToken> ParseStringLine(string line)
        {
            List<char> chars = new List<char>();
            bool LastWhitespace = char.IsWhiteSpace(line[0]);
            string data;
            foreach (char c in line)
            {
                if (char.IsWhiteSpace(c) != LastWhitespace)
                {
                    // if the last one was whitespace and this one isn't (or vice versa)
                    // return the last block of text, and start a new list of characters
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

    /// <summary>Object for printing out the instructions</summary>
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

        /// <summary>Group tokens by colour</summary>
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

        /// <summary>Group tokens into breakable and non-breakable images</summary>
        public static IEnumerable<(ColoredTextImage?, ColoredTextImage?)> GroupPairTokens(IEnumerable<InstructionDataToken> tokenRow)
            => GroupPairTokens(GroupTokens(tokenRow));
        /// <summary>Group tokens into breakable and non-breakable images</summary>
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

        /// <summary>Wrap the lines of this object to a limited length</summary>
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
                        // if both elements will fit, just add them
                        AppendArray(breakableArray);
                        AppendArray(nonBreakableArray);
                    }
                    else if (nonBreakableArray.Length > width)
                    {
                        // if the non-breakable is too long, split it by length
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
                        // if the non-breakable text doesn't fit, put it on a newline
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

        /// <summary>Print the instructions out</summary>
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