using UnityEngine;
using System.Collections;

namespace Roland
{
    public class NetworkingTags
    {
        public const int Controller = 0;		//For controller ralated things (think creating new players etc)
        public const int PlayerUpdate = 1;		//For player changeing things like change of animation/position/rotation etc
        public const int Server = 2;

        public class EventSubjects
        {
            public const int KeyboardEvent = 0;		//Move the player to (Vector3)Data
            public const int MouseEvent = 1;		//Send place item events.
        }

        public class ControllerSubjects
        {
            public const int JoinMessage = 0;			//Tells everyone we've joined and need to know who's there.
            public const int SpawnPlayer = 1;			//Tell people to spawn a new player for us.
            public const int PlacePlayer = 2;
            public const int CleanNumber = 3;
        }
    }
}