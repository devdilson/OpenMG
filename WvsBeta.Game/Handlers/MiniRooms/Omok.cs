﻿using System;
using System.Collections.Generic;
using System.Reflection;
using WvsBeta.Common.Interfaces;
using WvsBeta.Common.Sessions;
using WvsBeta.Game.Handlers.MiniRooms;

namespace WvsBeta.Game.GameObjects.MiniRooms
{
    public class OmokStone
    {
        public int mX { get; set; }
        public int mY { get; set; }
        public bool mOpened { get; set; }
        public byte mType { get; set; }

        public OmokStone(int X, int Y, byte Type)
        {
            mX = X;
            mY = Y;
            mType = Type;
        }
    }

    public class Omok : MiniGameRoom
    {
        private const int BOARD_WIDTH = 15;
        private const int BOARD_HEIGHT = 15;

        public bool[] mLeaveBooked { get; set; }
        public bool[] mRetreat { get; set; }
        public int mGameResult { get; set; }
        public int[][] mCheckedStones { get; set; }
        public int[] mPlayerColor { get; set; }
        public int mLastStoneChecker { get; set; }
        public bool mUserReady { get; set; }
        public byte OmokType { get; set; }
        public byte[,] Board { get; set; }
        public List<OmokStone> Stones { get; set; }

        public Omok(GameCharacter owner, string title, bool @private, string password, byte pieceType) : base(owner, 2, MiniRoomType.Omok, title, password, @private, pieceType, MiniGameType.Omok)
        {
            mCurrentTurnIndex = 0;
            Board = new byte[BOARD_WIDTH, BOARD_HEIGHT];
            Stones = new List<OmokStone>();
            mWinnerIndex = 1;
            this.Initialize();
        }

        public byte GetOtherType(byte type)
        {
            if (type == 1) return 2;
            if (type == 2) return 1;
            return 0xFF;
        }
        public void RemoveStone(int stoneIdx)
        {
            var stone = Stones[stoneIdx];
            Board[stone.mX, stone.mY] = 0;
            Stones.Remove(stone);
        }
        public void AddStone(OmokStone stone, GameCharacter chr)
        {
            Stones.Add(stone);
            Board[stone.mX, stone.mY] = stone.mType;
            mCurrentTurnIndex = chr.RoomSlotId == 0 ? (byte)1 : (byte)0;
        }

        public bool CheckStone(byte Piece)
        {
            if (CheckStoneDiagonal(Piece, true) || CheckStoneDiagonal(Piece, false)
            || CheckStoneHorizontal(Piece) || CheckStoneVertical(Piece)) return true;
            else return false;
        }

        public override void StartGame(GameCharacter chr)
        {
            Packet pw = new Packet(ServerMessages.MINI_ROOM_BASE);
            pw.WriteByte((byte)MiniRoomOpServer.GameStart);
            pw.WriteByte(mWinnerIndex); //0 Would let slot 1, 1 would let slot 0
            SendStartGame(pw);
        }

        public override void EndGame(GameCharacter winner, GameResult result)
        {
            base.EndGame(winner, result);
            Board = new byte[BOARD_WIDTH, BOARD_HEIGHT];
            Stones.Clear();
        }

        private bool CheckStones(int x, int y, byte piece)
        {
            int count(int xPos, int yPos, Func<int, int> xInc, Func<int, int> yInc, int result = 0)
            {
                if (x >= Board.GetLength(0) || y >= Board.GetLength(1) || x < 0 || y < 0 || Board[x, y] != piece)
                    return result;
                else
                    return count(xInc(x), yInc(y), xInc, yInc, result + 1);
            }

            int diag1 = count(x, y, c => c + 1, c => c + 1) + count(x, y, c => c - 1, c => c - 1);
            int diag2 = count(x, y, c => c + 1, c => c - 1) + count(x, y, c => c - 1, c => c + 1);
            int horz = count(x, y, c => c + 1, c => c) + count(x, y, c => c - 1, c => c);
            int vert = count(x, y, c => c, c => c + 1) + count(x, y, c => c, c => c - 1);

            return diag1 > 5 || diag2 > 5 || horz > 5 || vert > 5; 
            //must be > 5 instead of >= because adding two count() calls overcounts by 1 (the first piece is counted twice)
        }

        //Credits to Loki for these formulas, had to tweak them a bit though.
        public bool CheckStoneDiagonal(byte Piece, bool Up)
        {
            if (Up) //from Top left to bottom right or vice versa
            {
                for (int y = 4; y < 15; y++)
                {
                    for (int x = 0; x < 11; x++)
                    {
                        if (this.Board[x, y] == Piece &&
                        this.Board[x + 1, y - 1] == Piece &&
                        this.Board[x + 2, y - 2] == Piece &&
                        this.Board[x + 3, y - 3] == Piece &&
                        this.Board[x + 4, y - 4] == Piece)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                for (int y = 0; y < 11; y++)
                {
                    for (int x = 0; x < 11; x++)
                    {
                        if (this.Board[x, y] == Piece &&
                        this.Board[x + 1, y + 1] == Piece &&
                        this.Board[x + 2, y + 2] == Piece &&
                        this.Board[x + 3, y + 3] == Piece &&
                        this.Board[x + 4, y + 4] == Piece)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CheckStoneHorizontal(byte Piece)
        {
            for (int y = 0; y < 15; y++)
            {
                for (int x = 0; x < 11; x++)
                {
                    if (this.Board[x, y] == Piece &&
                    this.Board[x + 1, y] == Piece &&
                    this.Board[x + 2, y] == Piece &&
                    this.Board[x + 3, y] == Piece &&
                    this.Board[x + 4, y] == Piece)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckStoneVertical(byte Piece)
        {
            for (int y = 0; y < 11; y++)
            {
                for (int x = 0; x < 15; x++)
                {
                    if (this.Board[x, y] == Piece &&
                    this.Board[x, y + 1] == Piece &&
                    this.Board[x, y + 2] == Piece &&
                    this.Board[x, y + 3] == Piece &&
                    this.Board[x, y + 4] == Piece)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
