using UnityEngine;

namespace Age
{
    public static class GameWindow
    {
        static Vector3 invalidPosition = new Vector3(-1, -1, -1);
        public static Vector3 InvalidPosition { get { return invalidPosition; } }

        public static int BottomBarHeight { get { return 80; } }

        public static int BottomBorder { get { return BottomBarHeight; } }
        public static int TopBorder { get { return Screen.height; } }
        public static int LeftBorder { get { return 0; } }
        public static int RightBorder { get { return Screen.width; } }
        

    }
    public static class Globals
    {
        public enum ResourceType { Food, Wood, Coin }
    }
}