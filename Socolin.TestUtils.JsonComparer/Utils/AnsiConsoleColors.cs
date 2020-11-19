namespace Socolin.TestUtils.JsonComparer.Utils
{
    internal static class AnsiConsoleColors
    {
        private const string EscapeCode = "\u001b";
        public static readonly string RESET = $"{EscapeCode}[0m";
        public static readonly string RED = $"{EscapeCode}[31m";
        public static readonly string GREEN = $"{EscapeCode}[32m";
        // ReSharper disable UnusedMember.Global
        public static readonly string YELLOW = $"{EscapeCode}[33m";
        public static readonly string BLUE = $"{EscapeCode}[34m";
        public static readonly string MAGENTA = $"{EscapeCode}[35m";
        public static readonly string CYAN = $"{EscapeCode}[36m";
        public static readonly string WHITE = $"{EscapeCode}[37m";
        // ReSharper restore UnusedMember.Global
    }
}
