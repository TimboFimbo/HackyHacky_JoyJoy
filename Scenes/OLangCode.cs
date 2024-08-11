using Godot;
using System;

namespace OLangCode
{
    public static class Code
    {
        public static int varsStart = 1000;

        public static string oLangLevel1 = 
            "PRT$1000        " +
            "INP$1030,16     " +
            "PRT$1010        " +
            "END             " +
            "RUNopen_door,1  " +
            "PRT$1020        " +
            "END             " +
            "                " +
            "                " +
            "                " +
            "                ";

        public static string varsLevel1 = 
            "Enter Code      " +
            "Door Not Opened " +
            "Door Opened     " +
            "1234            " +
            "                ";

        public static string oLangLevel2 = 
            "PRT$1000        " +
            "INP$1010,16     " +
            "PRT$1030,$1010  " +
            "RUN$1020        " +
            "END             " +
            "                " +
            "                " +
            "                " +
            "                " +
            "                " +
            "                ";
            
        public static string varsLevel2 = 
            "Enter Name      " +
            "NAME            " +
            "null_func,0     " +
            "Hello,          " +
            "                ";

        public static string oLangLevel3 = 
            "PRT$1000        " +
            "RUNopen_door,0  " +
            "PRT$1010        " +
            "END             " +
            "                " +
            "                " +
            "                " +
            "                " +
            "                " +
            "                " +
            "                ";

        public static string varsLevel3 = 
            "Opening Door... " +
            "Door Opened     " +
            "                " +
            "                " +
            "                ";

        public static string oLangLevel4 = 
            "RUNgen_code,4   " +
            "PRT$1020        " +
            "INP$1010,16     " +
            "PRT$1030        " +
            "RUNcheck_code,0 " +
            "END             " +
            "                " +
            "                " +
            "                " +
            "                " +
            "                ";

        public static string varsLevel4 = 
            "GEN             " +
            "CODE            " +
            "Enter Code      " +
            "Checking Code..." +
            "                ";

        public static string emptyStack = 
            "                " +
            "                " +
            "                ";
    }
}
