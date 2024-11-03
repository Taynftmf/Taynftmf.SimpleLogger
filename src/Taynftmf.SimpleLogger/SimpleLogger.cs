﻿namespace Taynftmf
{

    using System;
    using System.IO;
    using System.Linq;

    public static class SimpleLogger
    {
        private const string configFile = "log-setup.txt";

        public static void Log(string message, params string[] keywords)
        {
            string setupFile = Path.Combine(Path.GetTempPath(), configFile);
            if (!File.Exists(setupFile)) return;

            // Read all lines from the setup file
            string[] lines = File.ReadAllLines(setupFile);
            
            // Ensure there is at least one line for the log file name
            if (lines.Length == 0) return;

            // The first line is the log file name
            string logFile = lines[0].Trim();

            // Check if there are more lines for keywords and validate the log keyword
            if (lines.Length > 1 && keywords.Any(item => lines.Contains(item)))
            {
                using (StreamWriter writer = new StreamWriter(logFile, true))
                {
                    string allKeywords = string.Join(", ", keywords.OrderBy(keyword => keyword));
                    writer.WriteLine($"{DateTime.Now} ['{allKeywords}']: {message}");
                }
            }
        }
    }

}