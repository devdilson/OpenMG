﻿using System.Collections.Generic;

namespace WvsBeta.Game.Scripting
{
    public interface INpcScript : IGameScript
    {
        void Run(INpcHost self, GameCharacter target, byte state, byte nRet, string stringAnswer, int nRetNum);
    }

    public interface INpcHost
    {
        int mID { get; }
        void Say(int offset, bool stopOnEnd, params string[] messages);
        void Say(string message);
        void SendNext(string Message);
        void SendBackNext(string Message);
        void SendBackOK(string Message);
        void SendOK(string Message);
        void AskMenu(string Message);
        void AskYesNo(string Message);
        void AskText(string Message, string Default, short MinLength, short MaxLength);
        void AskNumber(string Message, int Default, int MinValue, int MaxValue);
        void AskStyle(string Message, List<int> Values);
        void AskPet(string message);
        void AskPetAllExcept(string message, string petcashid);
        void Stop();

        object GetSessionValue(string pName);
        void SetSessionValue(string pName, object pValue);
    }
}