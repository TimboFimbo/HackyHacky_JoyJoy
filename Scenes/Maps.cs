using Godot;
using System;

namespace AsciiMaps
{
    public static class Maps
    {
        static string[] mapLevel1 = 
        {
            ".......W...",
            ".......W...",
            "....T..W...",
            ".......D.E.",
            ".......W...",
            ".......W...",
            ".......W..."
        };

        public static char CheckMap(int y, int x)
        {
            return mapLevel1[y][x];
        }
    }
}