using System;
using System.Collections.Generic;
using ConsoleUtils.ConsoleImagery;
using BattleShips.Boards;

namespace BattleShips.Util
{
    public static class Colours
    {
        public const ConsoleColor EvenColour = ConsoleColor.Green;
        public static readonly ConsoleColorPair EvenPair = new ConsoleColorPair(EvenColour);
        public const ConsoleColor OddColour = ConsoleColor.Yellow;
        public static readonly ConsoleColorPair OddPair = new ConsoleColorPair(OddColour);

        public static ConsoleColorPair EvenOddPair(int number)
            => number % 2 == 0 ? EvenPair : OddPair;


        public static ConsoleColor BadColour = ConsoleColor.Red;
        public static ConsoleColorPair BadPair = new ConsoleColorPair(BadColour);
        public static ConsoleColor GoodColour = ConsoleColor.Green;
        public static ConsoleColorPair GoodPair = new ConsoleColorPair(GoodColour);
        public static ConsoleColor IndifferentColour = ConsoleColor.DarkGray;
        public static ConsoleColorPair IndifferentPair = new ConsoleColorPair(IndifferentColour);


        public static Dictionary<DisplayEnum, ColoredTextImage> OwnDisplayEnumTexts = new Dictionary<DisplayEnum, ColoredTextImage> {
            {DisplayEnum.None, ColoredTextImage.Text(" ")},
            {DisplayEnum.Shot, ColoredTextImage.Text("M", IndifferentPair)},
            {DisplayEnum.Boat, ColoredTextImage.Text("B", GoodPair)},
            {DisplayEnum.mHit, ColoredTextImage.Text("H", BadPair)},
            {DisplayEnum.mSunk, ColoredTextImage.Text("S", BadPair)},
        };
        public static Dictionary<DisplayEnum, ColoredTextImage> EnemyDisplayEnumTexts = new Dictionary<DisplayEnum, ColoredTextImage> {
            {DisplayEnum.None, ColoredTextImage.Text(" ")},
            {DisplayEnum.Shot, ColoredTextImage.Text("M", BadPair)},
            {DisplayEnum.Boat, ColoredTextImage.Text(" ")},
            {DisplayEnum.mHit, ColoredTextImage.Text("H", GoodPair)},
            {DisplayEnum.mSunk, ColoredTextImage.Text("S", GoodPair)},
        };

        public static ColoredTextImage RenderElement(DisplayEnum d, bool enemy)
            => enemy
            ? EnemyDisplayEnumTexts[d]
            : OwnDisplayEnumTexts[d];
        public static ColoredTextImage RenderElement(IBoardElement element, bool enemy)
            => RenderElement(element.GetDisplay(), enemy);

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

        public static ColoredTextImage RenderHitMissText(DisplayEnum d, bool enemy)
            => enemy
            ? GetEnemyHitMissText(d)
            : GetOwnHitMissText(d);

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