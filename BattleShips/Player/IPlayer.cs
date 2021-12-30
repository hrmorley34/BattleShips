using System;
using System.Collections.Generic;
using System.Linq;
using BattleShips.Boards;
using BattleShips.Util;

namespace BattleShips.Player
{
    public interface IPlayer
    {
        string GetName();

        void SetBoards(Board boatsBoard, Board enemyBoard);
        Board GetOwnBoard();
        Board GetEnemyBoard();
        void ShowBoard(Board board);
        void ShowBoardAttack(IPlayer aggressor, Board board, Coordinates shot);

        IEnumerable<Boat> GetBoats();
        void PlaceBoats(IEnumerable<Boat> boats);

        Coordinates AskShoot();

        bool HasLost() => GetBoats().All(b => b.IsSunk());
        void ShowWinState(IPlayer loser);
    }
}