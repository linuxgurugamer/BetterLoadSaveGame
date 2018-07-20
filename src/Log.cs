using System;
using UnityEngine;

namespace BetterLoadSaveGame
{
    public static class Log
    {
        private static string GetLogMessage(object messageOrFormat, object[] args)
        {
            string message = messageOrFormat.ToString();
            if (args != null && args.Length > 0)
            {
                message = String.Format(message, args);
            }
            return String.Format("[BetterLoadSaveGame] {0}", message);
        }

        public static void Info(object messageOrFormat, params object[] args)
        {
            Debug.Log(GetLogMessage(messageOrFormat, args));
        }

        public static void Error(object messageOrFormat, params object[] args)
        {
            Debug.LogError(GetLogMessage(messageOrFormat, args));
        }

        public static void Warn(object messageOrFormat, params object[] args)
        {
            Debug.LogWarning(GetLogMessage(messageOrFormat, args));
        }
    }
}
