namespace Constants
{
    public static class Errors
    {
        public static string ERR_NO_PRINT = "Nothing to Print!";
        public static string ERR_NO_ADDRESS = "No Valid Address!";
        public static string ERR_NO_RUN = "Nothing to Run!";
        public static string ERR_NO_FUNC = "No Valid Function!";
        public static string ERR_RET_LINE = "Return Line Error!";
        public static string ERR_ARGS_LINE = "Args Line Error!";
        public static string ERR_STACK_PARSE = "Stack Parsing Error!";
    }

    public static class Hints
    {
        public static string[] hints = 
        {
            "What is being generated in address $1000?..",
            "There's an 'open_door' function, but it can't be reached. Or can it?...",
            "That 'null_func' doesn't do anything. If only it opened a door instead...",
            "Don't forget you can still move when the oLang is paused...",
            "Have you considered using multiple exploits?.. (don't forget - 'r' to restart the level)"
        };
    }
}