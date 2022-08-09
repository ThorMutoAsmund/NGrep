using System;
using System.IO;

namespace NGrep
{
    internal class Program
    {
        enum ExitCode : int
        {
            Success = 0,
            Error = 1
        }

        static int Main(string[] args)
        {
            var p = new GrepParams();
            var mode = String.Empty;
            var unnamed = 0;

            foreach (var arg in args)
            {
                if (!String.IsNullOrEmpty(mode))
                {
                    if (mode == "w")
                    {
                        p.ReplaceWith = arg;
                    }
                    mode = String.Empty;
                    continue;
                }

                if (!arg.StartsWith("-"))
                {
                    if (unnamed == 0)
                    {
                        p.SearchTerm = arg;
                    }
                    else if (unnamed == 1)
                    {
                        p.SearchIn = arg;

                        if (Directory.Exists(p.SearchIn))
                        {
                            p.SearchPattern = "";
                        }
                        else
                        {
                            p.SearchPattern = Path.GetFileName(p.SearchIn);
                            p.SearchIn = Path.GetDirectoryName(p.SearchIn);
                        }
                    }
                    unnamed++;
                    continue;
                }

                if (arg == "-r")
                {
                    p.Recursive = true;
                }
                else if (arg == "-l")
                {
                    p.ListFilesOnly = true;
                }
                else if (arg == "-n")
                {
                    p.ShowLineNumbers = true;
                }
                else if (arg == "-i")
                {
                    p.IgnoreCase = true;
                }
                else if (arg == "-x")
                {
                    p.ExactMatch = true;
                }
                else if (arg == "-w")
                {
                    p.Replace = true;
                    mode = "w";
                }
            }

            if (!GrepEngine.TestParams(p, out var errorMessage))
            {
                Console.WriteLine(errorMessage);
                return (int)ExitCode.Error;
            }

            var grepEngine = new GrepEngine();
            grepEngine.Run(p);

            return (int)ExitCode.Success;
        }
    }
}
