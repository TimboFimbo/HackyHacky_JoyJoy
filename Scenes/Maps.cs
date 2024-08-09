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
            "W.......1.E.W",
            "W.......W...W",
            "W.......W...W",
            "W.......W...W",
            "WWWWWWWWWWWWW"
        };

        static string[] mapLevel3 = 
        {
            "WWWWWWWWWWWWW",
            "W....WW.W...W",
            "W....WT.1...W",
            "W....WW.W...W",
            "W.......WWWWW",
            "W.......W...W",
            "W.......2.E.W",
            "W.......W...W",
            "WWWWWWWWWWWWW"
        };

        public static char CheckMap(int x, int y, int levelNum)
        {
            if (levelNum == 1)
            {
                return mapLevel1[y][x];
            }
            if (levelNum == 2)
            {
                // return mapLevel2[y][x];
                return ' ';
            }
            else
            {
                return mapLevel3[y][x]; // change to maplevel3
            }
        }
    }
}