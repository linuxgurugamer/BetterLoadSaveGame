using System;
using UnityEngine;
using System.Diagnostics;

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

        [ConditionalAttribute("DEBUG")]
        public static void Info(object messageOrFormat, params object[] args)
        {
            UnityEngine.Debug.Log(GetLogMessage(messageOrFormat, args));
        }

        public static void Error(object messageOrFormat, params object[] args)
        {
            UnityEngine.Debug.LogError(GetLogMessage(messageOrFormat, args));
        }

        public static void Warn(object messageOrFormat, params object[] args)
        {
            UnityEngine.Debug.LogWarning(GetLogMessage(messageOrFormat, args));
        }
    }
}
