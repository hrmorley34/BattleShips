using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BattleShips.Boards;
using BattleShips.Player;
using BattleShips.Util;

namespace BattleShips
{
    /// <summary>The main game object</summary>
    [DataContract(IsReference = true)]
    [KnownType(typeof(ComputerPlayer))]
    [KnownType(typeof(HumanPlayer))]
    public class Game
    {
        [DataMember]
        (IPlayer, Board)[] PlayerBoards;
        [IgnoreDataMember]
        public IEnumerable<IPlayer> Players { get => PlayerBoards.Select(i => i.Item1); }
        [IgnoreDataMember]
        public IEnumerable<Board> Boards { get => PlayerBoards.Select(i => i.Item2); }
        [IgnoreDataMember]
        public int PlayerCount { get => PlayerBoards.Length; }

        [DataMember]
        protected int _CurrentPlayerIndex;
        [IgnoreDataMember]
        public int CurrentPlayerIndex
        {
            get => _CurrentPlayerIndex;
            set
            {
                _CurrentPlayerIndex = value % PlayerCount;
                if (_CurrentPlayerIndex < 0) _CurrentPlayerIndex += PlayerCount;
            }
        }
        [IgnoreDataMember]
        public IPlayer CurrentPlayerObject { get => Players.Skip(CurrentPlayerIndex).First(); }

        [IgnoreDataMember]
        public Serialiser<Game>? Serialiser = null;

        public Game(IEnumerable<IPlayer> players)
        {
            PlayerBoards = players.Select(p => (p, new Board(p))).ToArray();
            foreach (
                ((IPlayer player, Board ownBoard), (IPlayer _, Board enemyBoard))
                in PlayerBoards.Zip(PlayerBoards.Skip(1).Concat(PlayerBoards.Take(1))))
            {
                player.SetBoards(ownBoard, enemyBoard);
            }

            CurrentPlayerIndex = 0;
        }

        /// <summary>Setup the game</summary>
        public void MainSetup(IEnumerable<int> BoatLengths)
        {
            foreach (IPlayer player in Players)
            {
                player.PlaceBoats(BoatLengths.Select(i => new Boat(i)));
            }
        }

        /// <summary>Run one player turn</summary>
        public void MainStep()
        {
            IPlayer currentPlayer = CurrentPlayerObject;
            Coordinates coordinates = currentPlayer.AskShoot();
            Board dest = currentPlayer.GetEnemyBoard();

            dest.Get(coordinates).Shoot();

            CurrentPlayerIndex++;
            Save();

            foreach (IPlayer player in Players)
            {
                player.ShowBoardAttack(currentPlayer, dest, coordinates);
            }
        }

        /// <summary>Run the main game loop</summary>
        public void MainLoop()
        {
            while (!Players.Any(p => p.HasLost()))
            {
                // keep stepping until a player has lost
                MainStep();
            }

            // delete save file
            Serialiser?.Clear();

            foreach (IPlayer player in Players)
            {
                player.ShowWinState(Players.Where(p => p.HasLost()).First());
            }
        }

        /// <summary>Write the current state to the file (if available)</summary>
        public void Save()
        {
            Serialiser?.SerialiseObject(this);
        }
    }
}