using System;
using System.Collections.Generic;
using System.Linq;
using BattleShips.Boards;
using BattleShips.Util;

namespace BattleShips.Player
{
    /// <summary>Generic interface for a player</summary>
    public interface IPlayer
    {
        /// <summary>Get the name of this player</summary>
        string GetName();

        /// <summary>Set the boards for this player to place boats on and to attack</summary>
        void SetBoards(Board boatsBoard, Board enemyBoard);
        /// <summary>Get the board of this player</summary>
        Board GetOwnBoard();
        /// <summary>Get the board that this player attacks</summary>
        Board GetEnemyBoard();
        /// <summary>Show a board (to a human player)</summary>
        void ShowBoard(Board board);
        /// <summary>Show a board being attacked</summary>
        void ShowBoardAttack(IPlayer aggressor, Board board, Coordinates shot);

        /// <summary>Get this player's boats</summary>
        IEnumerable<Boat> GetBoats();
        /// <summary>Ask this player to place these boats</summary>
        void PlaceBoats(IEnumerable<Boat> boats);

        /// <summary>Ask this player for a location to shoot</summary>
        Coordinates AskShoot();

        /// <summary>Has this player lost?</summary>
        bool HasLost() => GetBoats().All(b => b.IsSunk());
        /// <summary>Show this player who won</summary>
        void ShowWinState(IPlayer loser);
    }
}