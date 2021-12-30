using System;
using System.Runtime.Serialization;

namespace BattleShips.Boards
{
    public interface IBoardElement
    {
        bool Shoot();
        bool HasShot();
        DisplayEnum GetDisplay();
    }

    [DataContract(IsReference = true)]
    public class EmptyBoardElement : IBoardElement
    {
        [DataMember]
        protected bool Shot;

        public EmptyBoardElement()
        {
            Shot = false;
        }

        public bool Shoot()
        {
            Shot = true;
            return false;
        }

        public bool HasShot() => Shot;

        public DisplayEnum GetDisplay() => Shot ? DisplayEnum.Shot : DisplayEnum.None;
    }
}