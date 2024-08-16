using UnityEngine;

namespace OceanAnomaly
{
    public static class GlobalVariables
    {
        public enum GameState
        {
            GamePlay,
            GamePlayPaused,
            GamePlayMenuNoPause,
            Cutscene,
            MainMenu
        }

        public static GameState CurrentState = GameState.GamePlay;

        // Error handeling
        public static string CurrentError = "";
        public static Color CurrentErrorColor = Color.red;
    }
}