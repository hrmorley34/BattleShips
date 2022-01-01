using System;
using System.Collections.Generic;
using ConsoleUtils.ConsoleImagery;
using BattleShips.Boards;

namespace BattleShips.Util
{
    /// <summary>Static/constant colours</summary>
    public static class Colours
    {
        /// <summary>Colour for even rows/columns</summary>
        public const ConsoleColor EvenColour = ConsoleColor.Green;
        /// <summary>Colour pair for even rows/columns</summary>
        public static readonly ConsoleColorPair EvenPair = new ConsoleColorPair(EvenColour);
        /// <summary>Colour for odd rows/columns</summary>
        public const ConsoleColor OddColour = ConsoleColor.Yellow;
        /// <summary>Colour pair for odd rows/columns</summary>
        public static readonly ConsoleColorPair OddPair = new ConsoleColorPair(OddColour);

        /// <summary>Get even/odd colour pair for a number</summary>
        public static ConsoleColorPair EvenOddPair(int number)
            => number % 2 == 0 ? EvenPair : OddPair;


        /// <summary>Colour for bad cells</summary>
        public const ConsoleColor BadColour = ConsoleColor.Red;
        /// <summary>Colour pair for bad cells</summary>
        public static readonly ConsoleColorPair BadPair = new ConsoleColorPair(BadColour);
        /// <summary>Colour for good cells</summary>
        public const ConsoleColor GoodColour = ConsoleColor.Green;
        /// <summary>Colour pair for good cells</summary>
        public static readonly ConsoleColorPair GoodPair = new ConsoleColorPair(GoodColour);
        /// <summary>Colour for unimportant cells</summary>
        public const ConsoleColor IndifferentColour = ConsoleColor.DarkGray;
        /// <summary>Colour pair for unimportant cells</summary>
        public static readonly ConsoleColorPair IndifferentPair = new ConsoleColorPair(IndifferentColour);


        /// <summary>Mapping of <c>DisplayEnum</c>s to one's own cell text</summary>
        public static Dictionary<DisplayEnum, ColoredTextImage> OwnDisplayEnumTexts = new Dictionary<DisplayEnum, ColoredTextImage> {
            {DisplayEnum.None, ColoredTextImage.Text(" ")},
            {DisplayEnum.Shot, ColoredTextImage.Text("M", IndifferentPair)},
            {DisplayEnum.Boat, ColoredTextImage.Text("B", GoodPair)},
            {DisplayEnum.mHit, ColoredTextImage.Text("H", BadPair)},
            {DisplayEnum.mSunk, ColoredTextImage.Text("S", BadPair)},
        };
        /// <summary>Mapping of <c>DisplayEnum</c>s to an enemy's cell text</summary>
        public static Dictionary<DisplayEnum, ColoredTextImage> EnemyDisplayEnumTexts = new Dictionary<DisplayEnum, ColoredTextImage> {
            {DisplayEnum.None, ColoredTextImage.Text(" ")},
            {DisplayEnum.Shot, ColoredTextImage.Text("M", BadPair)},
            {DisplayEnum.Boat, ColoredTextImage.Text(" ")},
            {DisplayEnum.mHit, ColoredTextImage.Text("H", GoodPair)},
            {DisplayEnum.mSunk, ColoredTextImage.Text("S", GoodPair)},
        };

        /// <summary>Render a <c>DisplayEnum</c> for oneself or an enemy</summary>
        public static ColoredTextImage RenderElement(DisplayEnum d, bool enemy)
            => enemy
            ? EnemyDisplayEnumTexts[d]
            : OwnDisplayEnumTexts[d];
        /// <summary>Render an <c>IBoardElement</c> for oneself or an enemy</summary>
        public static ColoredTextImage RenderElement(IBoardElement element, bool enemy)
            => RenderElement(element.GetDisplay(), enemy);


        /// <summary>Get text for hitting or missing a cell</summary>
        public static ColoredTextImage GetOwnHitMissText(DisplayEnum d)
        {
            // `d & DisplayEnum.Shot` is implied
            if ((d & DisplayEnum.Sunk) == DisplayEnum.Sunk)
            {
                return
                    ColoredTextImage.Text("hitting", GoodPair)
                    + " a boat and "
                    + ColoredTextImage.Text("sinking", GoodPair)
                    + " it.";
            }
            else if ((d & DisplayEnum.Boat) == DisplayEnum.Boat)
            {
                return
                    ColoredTextImage.Text("hitting", GoodPair)
                    + " a boat but "
                    + ColoredTextImage.Text("failing", BadPair)
                    + " to sink it.";
            }
            else
            {
                return
                    "hitting "
                    + ColoredTextImage.Text("water", BadPair)
                    + ".";
            }
        }

        /// <summary>Get text for an enemy hitting or missing a cell</summary>
        public static ColoredTextImage GetEnemyHitMissText(DisplayEnum d)
        {
            // `d & DisplayEnum.Shot` is implied
            if ((d & DisplayEnum.Sunk) == DisplayEnum.Sunk)
            {
                return
                    ColoredTextImage.Text("sinking", BadPair)
                    + " one of your ships.";
            }
            else if ((d & DisplayEnum.Boat) == DisplayEnum.Boat)
            {
                return
                    ColoredTextImage.Text("hitting", BadPair)
                    + " a boat but "
                    + ColoredTextImage.Text("failing", GoodPair)
                    + " to sink it.";
            }
            else
            {
                return
                    "hitting "
                    + ColoredTextImage.Text("water", GoodPair)
                    + ".";
            }
        }

        /// <summary>Get text for you or an enemy hitting or missing a cell</summary>
        public static ColoredTextImage RenderHitMissText(DisplayEnum d, bool enemy)
            => enemy
            ? GetEnemyHitMissText(d)
            : GetOwnHitMissText(d);


        /// <summary>Get text for winning or losing the game</summary>
        public static ColoredTextImage RenderWinText(bool won)
        {
            const int width = 40;
            ColoredTextImage image = new ColoredTextImage(width, 9);
            if (won)
                image = image
                    .Overlay(ColoredTextImage.Text("Congratulations!", GoodPair), (width / 2 - 8, 2))
                    .Overlay(ColoredTextImage.Text("You win!", GoodPair), (width / 2 - 4, 4));
            else
                image = image
                    .Overlay(ColoredTextImage.Text("You lose!", BadPair), (width / 2 - 4, 4));
            image = image
                .Overlay(ColoredTextImage.Text(
                        "Press any key to continue",
                        new ConsoleColorPair(ConsoleColor.DarkGray)),
                    (width / 2 - 12, 7));
            return image;
        }
    }
}