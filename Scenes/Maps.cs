using Godot;
using System;

namespace AsciiMaps
{
    public static class Maps
    {
        static string[] mapLevel1 = 
        {
            "WWWWWWWWWWWWW",
            "W...WWW..WWWW",
            "W....W....WWW",
            "W..........WW",
            "W...T...WWWWW",
            "W.......1...W",
            "WW......W.E.W",
            "WWWW....W..WW",
            "WWWWWWWWWWWWW"
        };

        static string[] mapLevel2 = 
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
            "W.........WWW",
            "W..........WW",
            "W...........W",
            "W.....T.WW1WW",
            "WW......W...W",
            "WWW....WW.E.W",
            "WWWW..WWW...W",
            "WWWWWWWWWWWWW"
        };

        static string[] mapLevel4 = 
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

        static string[] mapLevel5 = 
        {
            "WWWWWWWWWWWWW",
            "W.WWWWWWWWW.W",
            "W..WWWWWWW..W",
            "W..WW...W...W",
            "W...1...2.E.W",
            "W...W...W...W",
            "W...WWWWW...W",
            "W.T.WWWWW..WW",
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
                return mapLevel2[y][x];
            }
            if (levelNum == 3)
            {
                return mapLevel3[y][x];
            }
            if (levelNum == 4)
            {
                return mapLevel4[y][x];
            }
            else
            {
                return mapLevel5[y][x];
            }
        }
    }
}