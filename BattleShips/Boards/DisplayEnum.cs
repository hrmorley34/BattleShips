using System;
using System.Collections.Generic;
using ConsoleUtils.ConsoleImagery;

namespace BattleShips.Boards
{
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