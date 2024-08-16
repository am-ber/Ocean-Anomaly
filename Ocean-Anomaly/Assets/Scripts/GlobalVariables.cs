using UnityEngine;

namespace OceanAnomaly
{
    public static class GlobalVariables
    {
        public enum GameState
        {
            gamePlay,
            gamePlayPaused,
            gamePlayMenuNoPause,
            cutscene,
            mainMenu
        }

        public static GameState currentState = GameState.gamePlay;

        // Error handeling
        public static string currentError = "";
        public static Color currentErrorColor = Color.red;
        public static bool rotateError = false;
    }
}