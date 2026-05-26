using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WoodAssociat.Utility
{
    public static class Loghandler
    {
        private static string GetColor(string name)
        {
            var hue = (uint)name.GetHashCode() / (float)uint.MaxValue;
            var color = Color.HSVToRGB(hue, 0.6f, 1f);
            return ColorUtility.ToHtmlStringRGB(color);
        }


        [HideInCallstack]
        public static void Log(object message, [CallerFilePath] string file = "")
        {
            var className = Path.GetFileNameWithoutExtension(file);
            var color = GetColor(className);
            Debug.Log($"<color=#{color}><b>[{className}]</b></color>----> {message}");
        }
    }
}