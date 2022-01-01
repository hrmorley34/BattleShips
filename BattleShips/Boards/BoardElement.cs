using System;
using System.Runtime.Serialization;

namespace BattleShips.Boards
{
    /// <summary>A generic element of a board</summary>
    public interface IBoardElement
    {
        /// <summary>Shoot at this element</summary>
        bool Shoot();
        /// <summary>Has this element yet been shot?</summary>
        bool HasShot();
        /// <summary>Get the <c>DisplayEnum</c> of this element</summary>
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