using System;
using System.Collections.Generic;
using System.IO;

namespace NGrep
{
    public class GrepEngine
    {
        public GrepParams Params { get; private set; }

        public static bool TestParams(GrepParams p, out string errorMessage)
        {
            if (String.IsNullOrEmpty(p.SearchIn))
            {
                errorMessage = "No folder to search in specified";
                return false;
            }

            if (String.IsNullOrEmpty(p.SearchTerm))
            {
                errorMessage = "No search term specified";
                return false;
            }

            if (!Directory.Exists(p.SearchIn))
            {
                errorMessage = $"Specified search path not found: {p.SearchIn}";
                return false;
            }

            errorMessage = String.Empty;
            return true;
        }

        public void Run(GrepParams p)
        {
            SearchOption searchOption = p.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            this.Params = p;

            var files = Directory.EnumerateFiles(p.SearchIn, String.IsNullOrEmpty(p.SearchPattern) ? "*" : p.SearchPattern, searchOption);

            // List files only
            if (p.ListFilesOnly)
            {
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                }

                return;
            }

            foreach (var file in files)
            {
                ProcessFile(file);
            }
        }

        private void ProcessFile(string path)
        {
            var compareOptions =
                this.Params.IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
            
            var term = this.Params.SearchTerm;
            var replaceWith = this.Params.ReplaceWith;
            var lines = File.ReadAllLines(path);
            var changed = new Dictionary<int, string>();

            int lineNo = -1;
            foreach (var _line in lines)
            {
                lineNo++;
                var line = _line;
                var startIndex = 0;
                int pos = -1;
                bool match = false;
                do
                {
                    pos = line.IndexOf(term, startIndex, compareOptions);

                    match = this.Params.ExactMatch ?
                        pos == 0 && line.Length == term.Length :
                        pos > -1;

                    if (match)
                    {
                        if (!this.Params.Replace)
                        {
                            DisplayResult(lineNo, pos, path, line, term);

                            startIndex = pos + term.Length;
                        }
                        else
                        {
                            line =
                                (pos > 0 ? line.Substring(0, pos) : string.Empty) +
                                (!String.IsNullOrEmpty(replaceWith) ? replaceWith : String.Empty) +
                                (pos + term.Length < line.Length ? line.Substring(pos + term.Length) : string.Empty);

                            pos = pos + (!String.IsNullOrEmpty(replaceWith) ? replaceWith.Length : 0);

                            changed[lineNo] = line;
                        }
                    }
                }
                while (!this.Params.ExactMatch && match);
            }

            // Update
            if (changed.Count > 0)
            {
                foreach (var pair in changed)
                {
                    lines[pair.Key] = pair.Value;
                }

                try
                {
                    File.WriteAllLines(path, lines);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}: {path}");
                }
            }
        }

        private void DisplayResult(int lineNumber, int pos, string path, string line, string term)
        {
            if (this.Params.ShowLineNumbers)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{lineNumber} ");
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(path);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(":");
            if (pos > 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(line.Substring(0, pos));
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(line.Substring(pos, term.Length));
            if (pos + term.Length < line.Length)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(line.Substring(pos + term.Length));
            }

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine();
        }
    }
}
