using System;
using System.Linq;
using System.Runtime.Serialization;
using BattleShips.Boards;

namespace BattleShips
{
    [DataContract(IsReference = true)]
    public class BoatElement : IBoardElement
    {
        [DataMember]
        protected bool Shot;
        [DataMember]
        public readonly Boat Boat;

        public BoatElement(Boat boat)
        {
            Shot = false;
            Boat = boat;
        }

        public bool Shoot()
        {
            Shot = true;
            return false;
        }

        public bool HasShot() => Shot;

        public DisplayEnum GetDisplay()
        {
            var e = DisplayEnum.Boat;
            if (Shot) e |= DisplayEnum.Shot;
            if (Boat.IsSunk()) e |= DisplayEnum.Sunk;
            return e;
        }
    }

    [DataContract(IsReference = true)]
    public class Boat
    {
        [DataMember]
        public readonly BoatElement[] BoatElements;

        public Boat()
        {
            BoatElements = new BoatElement[] { new BoatElement(this) };
        }

        public Boat(int length)
        {
            BoatElements = new BoatElement[length];
            for (int i = 0; i < length; i++)
            {
                BoatElements[i] = new BoatElement(this);
            }
        }

        public bool IsSunk() => BoatElements.All(b => b.HasShot());
    }
}