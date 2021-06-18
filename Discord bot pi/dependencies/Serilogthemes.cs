using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Sinks.SystemConsole.Themes;

namespace Anti_Dox.dependencies
{
    public static class Serilogthemes
    {
        public static Serilogtheme DiscordMatrix
        {
            get;
        } = new Serilogtheme(new Dictionary<ConsoleThemeStyle, string> {
			[ConsoleThemeStyle.Text] = "\u001b[32;1m",
			[ConsoleThemeStyle.SecondaryText] = "\u001b[34;1m",
			[ConsoleThemeStyle.TertiaryText] = "\u001b[37;1m",
			[ConsoleThemeStyle.Invalid] = "\u001b[32;1m",
			[ConsoleThemeStyle.Null] = "\u001b[32;1m",
			[ConsoleThemeStyle.Name] = "\u001b[32;1m",
			[ConsoleThemeStyle.String] = "\u001bx[31;1m",
			[ConsoleThemeStyle.Number] = "\u001b[32;1m",
			[ConsoleThemeStyle.Boolean] = "\u001b[32;1m",
			[ConsoleThemeStyle.Scalar] = "\u001b[32;1m",
			[ConsoleThemeStyle.LevelVerbose] = "\u001b[32;1m",
			[ConsoleThemeStyle.LevelDebug] = "\u001b[44;1m\u001b[37;1m",
			[ConsoleThemeStyle.LevelInformation] = "\u001b[42;1m\u001b[37;1m",
			[ConsoleThemeStyle.LevelWarning] = "\u001b[43;1m\u001b[37;1m",
			[ConsoleThemeStyle.LevelError] = "\u001b[41;1m\u001b[37;1m",
			[ConsoleThemeStyle.LevelFatal] = "\u001b[46;1m\u001b[37;1m"
		});

    }
}
