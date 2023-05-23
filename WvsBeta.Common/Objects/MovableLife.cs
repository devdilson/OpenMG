﻿using System.Collections.Generic;
using System.Linq;

namespace WvsBeta.Common.Objects
{
    public class MovableLife
    {
        public byte Stance { get; set; }
        public short Foothold { get; set; }
        public Pos Position { get; set; }
        public Pos Wobble { get; set; }
        public byte Jumps { get; set; }
        public long LastMove { get; set; }

        public long MovePathTimeSumLastCheck { get; set; }
        public long MovePathTimeSum { get; set; }
        public long MovePathTimeHackCountLastReset { get; set; }
        public int MovePathTimeHackCount { get; set; }

        public MovableLife()
        {
            Stance = 0;
            Foothold = 0;
            Position = new Pos();
            Wobble = new Pos(0, 0);
        }

        public MovableLife(short pFH, Pos pPosition, byte pStance)
        {
            Stance = pStance;
            Position = new Pos(pPosition);
            Foothold = pFH;
            Wobble = new Pos(0, 0);
            MovePathTimeHackCountLastReset =
                MovePathTimeSumLastCheck =
                    LastMove = MasterThread.CurrentTime;
        }

        public bool IsFacingRight()
        {
            return Stance % 2 == 0;
        }

        public static bool MovableInRange(MovableLife mob, Pos pAround, Pos pLeftTop, Pos pRightBottom)
        {
            return (
                (mob.Position.Y >= pAround.Y + pLeftTop.Y) && (mob.Position.Y <= pAround.Y + pRightBottom.Y) &&
                (mob.Position.X >= pAround.X + pLeftTop.X) && (mob.Position.X <= pAround.X + pRightBottom.X)
            );
        }

        public static IEnumerable<T> InRange<T>(IEnumerable<T> elements, Pos pAround, Pos pLeftTop, Pos pRightBottom)
            where T : MovableLife
        {
            return elements.Where(mob => MovableInRange(mob, pAround, pLeftTop, pRightBottom));
        }
    }
}
