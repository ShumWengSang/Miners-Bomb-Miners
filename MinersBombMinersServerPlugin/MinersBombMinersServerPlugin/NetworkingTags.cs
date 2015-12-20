﻿using System.Collections;

namespace Roland
{
    public class NetworkingTags
    {
        public const int Controller = 0;		//For controller ralated things (think creating new players etc)
        public const int PlayerUpdate = 1;		//For player changeing things like change of animation/position/rotation etc
        public const int Server = 2;
        public const int Room = 3;
        public const int Events = 4;
        public const int Player = 5;

        public class EventSubjects
        {
            public const int KeyboardEvent = 0;		//Move the player to (Vector3)Data
            public const int leftMouseButton = 1;
            public const int rightMouseButton = 2;

        }

        public class ControllerSubjects
        {
            public const int JoinMessage = 0;			//Tells everyone we've joined and need to know who's there.
            public const int SpawnPlayer = 1;
            public const int PlacePlayer = 2;
            public const int CleanNumber = 3;
            public const int ReadyToStartGame = 4;
            public const int ReplyToJoin = 5;
            public const int YouWin = 6;
        }

        public class RoomSubjects
        {
            public const int JoinRoom = 0;
            public const int ExitRoom = 1;
            public const int Message = 2;
            public const int ReplayToJoin = 3;
            public const int ReadyToMoveToGameScene = 4;
            public const int GiveItemDic = 5;

        }

        public class ServerSubjects
        {
            public const int GetRandomSpawn = 0;
            public const int ChangeStateToRoom = 1;
            public const int ChangeStateToGame = 2;
            public const int ILose = 3;
        }

        public class PlayerSubjects
        {
            public const int GiveItemDic = 0;
            public const int UpdatePostion = 1;
        }
    }
}