using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BattleShips.Boards;
using BattleShips.Util;

namespace BattleShips.Player
{
    [DataContract(IsReference = true)]
    public class ComputerPlayer : IPlayer
    {
        [DataMember]
        protected Board? BoatsBoard;
        [DataMember]
        protected Board? EnemyBoard;
        [DataMember]
        protected Boat[]? Boats;
        [IgnoreDataMember]
        protected Random _Random;
        [IgnoreDataMember]
        protected Random Random { get => _Random ??= new Random(); }

        public ComputerPlayer()
        {
            _Random = new Random();
        }

        public string GetName() => "Computer";

        public void SetBoards(Board boatsBoard, Board enemyBoard)
        {
            BoatsBoard = boatsBoard;
            EnemyBoard = enemyBoard;
        }

        public Board GetOwnBoard() => BoatsBoard ?? throw new NullReferenceException();
        public Board GetEnemyBoard() => EnemyBoard ?? throw new NullReferenceException();

        public void ShowBoard(Board board) { }
        public void ShowBoardAttack(IPlayer aggressor, Board board, Coordinates shot) { }

        public IEnumerable<Boat> GetBoats() => Boats ?? throw new NullReferenceException();

        public void PlaceBoats(IEnumerable<Boat> boats)
        {
            Board board = GetOwnBoard();
            Boats = boats.ToArray();

            // Find a valid placement first
            bool validPlacement = false;
            IEnumerable<(BoatElement, (int, int))> Placements;
            HashSet<(int, int)> UsedCells;
            do
            {
                UsedCells = new HashSet<(int, int)>();
                Placements = new (BoatElement, (int, int))[0];
                validPlacement = true;

                foreach (Boat b in Boats)
                {
                    bool isVertical = Random.Next(2) == 1;
                    int maxX = board.XSize, maxY = board.YSize;
                    if (isVertical) maxY -= b.BoatElements.Length - 1;
                    else maxX -= b.BoatElements.Length - 1;

                    int x = Random.Next(0, maxX), y = Random.Next(0, maxY);

                    IEnumerable<(BoatElement, (int, int))> IterElements()
                    {
                        for (int index = 0; index < b.BoatElements.Length; index++)
                            yield return (
                                b.BoatElements[index],
                                (isVertical ? x : x + index,
                                isVertical ? y + index : y));
                    }

                    foreach ((var _, (int, int) coords) in IterElements())
                    {
                        if (UsedCells.Contains(coords))
                        {
                            validPlacement = false;
                            break;
                        }
                        UsedCells.Add(coords);
                    }
                    if (!validPlacement) break;

                    Placements = Placements.Concat(IterElements());
                }
            } while (!validPlacement);

            // Write boat positions to the board
            foreach ((BoatElement element, (int, int) coords) in Placements)
            {
                board.Set(coords, element);
            }
        }

        public Coordinates AskShoot()
        {
            Board board = GetEnemyBoard();

            bool validShot = false;
            Coordinates coordinates;
            do
            {
                coordinates = new Coordinates(Random.Next(board.XSize), Random.Next(board.YSize));
                validShot = !board.Get(coordinates).HasShot();
            } while (!validShot);

            return coordinates;
        }

        public void ShowWinState(IPlayer loser) { }
    }
}