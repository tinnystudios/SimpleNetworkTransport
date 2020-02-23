using UnityEngine;

namespace SimpleTransport
{
    public static class Logger
    {
        public static bool ShowLog = true;

        public static void Log(string text)
        {
            if(ShowLog)
                Debug.Log(text);
        }
    }
}