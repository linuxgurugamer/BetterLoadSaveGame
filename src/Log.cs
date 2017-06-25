using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BetterLoadSaveGame
{
    public static class Log
    {
        private static bool _enableLogging = true;

        private static string GetLogMessage(object messageOrFormat, object[] args)
        {
            string message = messageOrFormat.ToString();
            if (args != null && args.Length > 0)
            {
                message = String.Format(message, args);
            }
            return String.Format("[BLSG] {0}", message);
        }

        public static void Info(object messageOrFormat, params object[] args)
        {
            if (_enableLogging)
            {
                Debug.Log(GetLogMessage(messageOrFormat, args));
            }
        }

        public static void Error(object messageOrFormat, params object[] args)
        {
            if (_enableLogging)
            {
                Debug.LogError(GetLogMessage(messageOrFormat, args));
            }
        }

        public static void Warn(object messageOrFormat, params object[] args)
        {
            if (_enableLogging)
            {
                Debug.LogWarning(GetLogMessage(messageOrFormat, args));
            }
        }
    }
}
