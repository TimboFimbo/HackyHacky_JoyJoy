using Godot;
using System;

namespace AsciiMaps
{
    public static class Maps
    {
        static string[] mapLevel1 = 
        {
            "WWWWWWWWWWWWW",
            "W.......W...W",
            "W.......W...W",
            "W....T..W...W",
            "W.......D.E.W",
            "W.......W...W",
            "W.......W...W",
            "W.......W...W",
            "WWWWWWWWWWWWW"
        };

        public static char CheckMap(int x, int y)
        {
            return mapLevel1[y][x];
        }
    }
}