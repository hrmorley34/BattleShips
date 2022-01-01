using System;
using System.Collections.Generic;
using ConsoleUtils.ConsoleImagery;

namespace BattleShips.Boards
{
    /// <summary>The different values to display on a grid</summary>
    [Flags]
    public enum DisplayEnum
    {
        None = 0,
        Shot = 1,
        Boat = 2,
        mHit = 3,
        Sunk = 4,
        mSunk = 7,
    }
}