using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using DCF.Common;
using System.Diagnostics;
using DCF.DataLayerAwareLib;
using System.Threading;
using DCF.XmlRules;

namespace DCF.DemoRules.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleTraceListener ctl = new ConsoleTraceListener();
            Logger.LoggerTraceListeners.Add(ctl);
#if DEBUG
            string debugFileName = string.Format("Debug {0}.txt", DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"));
            TextWriterTraceListener trlFile = new TextWriterTraceListener(
                System.IO.File.CreateText(debugFileName));
            Debug.Listeners.Add(trlFile);
#endif
            if (args.Length == 0)
            {
                Usage();
            }
            else
            {
                DateTime startTime = DateTime.Now;
                Logger.TraceWriteLine(string.Format("Starting the process at {0}", startTime.ToLongTimeString()));
                switch (args[0])
                {
                    case "generate":
                        if (!GenerateRules(args))
                        {
                            Usage();
                        }
                        break;
                    case "generate_files":
                        if (!GenerateFiles(args))
                        {
                            Usage();
                        }
                        break;
                    case "clean":
                        if (!CleanDatabase(args))
                        {
                            Usage();
                        }
                        break;
                    case "continous-cleaning":
                        if (!ContinousCleaning(args))
                        {
                            Usage();
                        }
                        break;
                    case "read-XML":
                        if (!ReadXml(args))
                        {
                            Usage();
                        }
                        break;
                    default:
                        Usage();
                        break;
                }
                DateTime endTime = DateTime.Now;
                Logger.TraceWriteLine(string.Format("Finishing the process at {0}. Total runtime is {1} sec",
                    endTime.ToLongTimeString(), (endTime - startTime).TotalSeconds));
                Logger.TraceWriteLine(PerformanceCounterStatic.ReportAllTimers());
                Logger.DebugFlush();
            }
        }

        private static bool GenerateFiles(string[] args)
        {
            TestTextFilesGenerator gen = new TestTextFilesGenerator();
            gen.ParseArgs(args.Skip(1).ToArray());
            Logger.TraceWriteLine("Creating a state");
            //gen.GenerateState();
            gen.GenerateUsersConcurrently();
            //Logger.TraceWriteLine("Generating files");
            //gen.CreateBasisTablesFiles();
            return true;
        }

        private static bool ReadXml(string[] args)
        {
            Console.WriteLine("Creating test");
            TestXmlReading test = new TestXmlReading(args[1]);
            test.init();
            Console.WriteLine("Running test");
            test.DoTestFlow();
            Console.WriteLine("done");
            return true;
        }

        private static bool ContinousCleaning(string[] args)
        {
            CountinousThreadState cleaner_state = new CountinousThreadState(args);

            Thread t = new Thread(new ThreadStart(cleaner_state.thread_main));
            t.Start();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
            Console.Write("Exinting thread...");
            cleaner_state.StopThread();
            t.Join();
            Console.WriteLine("done");
            return true;
        }

        private static bool CleanDatabase(string[] args)
        {
            bool res = true;
            try
            {
                // parse arguments and App.config file
                DatabaseCleaningManager dcm = new DatabaseCleaningManager();
                dcm.ParseArgs(args.Skip(1).ToArray() );
                dcm.InitFlow();
                // call the flow manager according to parameters
                dcm.DoTestFlow();
                // done
                dcm.FinishFlow();
            }
            catch (Exception ex)
            {
                Logger.TraceWriteLine(string.Format("Unhandled exception {0}; Trace {1}", ex.Message, ex.StackTrace));
                res = false;
            }
            return res;
        }

        private static bool GenerateRules(string[] args)
        {
            bool res = true;
            try
            {
                // parse arguments and App.config file
                TestDataGenerationManager tdgm = new TestDataGenerationManager();
                tdgm.ParseArgs(args.Skip(1).ToArray());
                tdgm.InitFlow();
                // call the flow manager according to parameters
                tdgm.DoTestFlow();
                // done
                tdgm.FinishFlow();
            }
            catch (Exception ex)
            {
                Logger.TraceWriteLine(string.Format("Unhandled exception {0}; Trace {1}", ex.Message, ex.StackTrace));
                res = false;
            }
            return res;
        }

        public static void Usage()
        {
            Logger.TraceWriteLine(string.Format("Usage: {0} <operation> <options>", Process.GetCurrentProcess().MainModule.ModuleName));
            Logger.TraceWriteLine("Where <operation> can be:");
            Logger.TraceIndent();
            Logger.TraceWriteLine("generate_files - generates the synthetic database in files");
            Logger.TraceIndent();
            Logger.TraceWriteLine("Use LOAD DATA FROM command to load thess files");
            Logger.TraceWriteLine("");
            Logger.TraceUnindent();
            Logger.TraceWriteLine("generate - generates the synthetic database");
            Logger.TraceWriteLine("<option> can be the following:");
            Logger.TraceIndent();
            Logger.TraceWriteLine("/NumberOfFacts=<number of vacts to generate>");
            Logger.TraceWriteLine("/GenerateBasisTables=<true - recreate Topics and Items tables; otherwise false>");
            Logger.TraceWriteLine("/UserProfiles=\"(<portion of users>,<their confidence>)[(...)*]\"");
            Logger.TraceWriteLine("/ItemsDefinitionFile=<full path to items template XML file>");
            Logger.TraceWriteLine("/TopicsDefinitionFile=<full path to tipics template XML file>");
            Logger.TraceUnindent();
            Logger.TraceWriteLine("");
            Logger.TraceWriteLine("clean - cleans the database by running set of algorithms");
            Logger.TraceWriteLine("continous-cleaning - cleans the database continously");
            Logger.TraceWriteLine("read_xml - loads the provided XML file with rules and runs them");
            Logger.TraceIndent();
            Logger.TraceWriteLine("Provide full path to rules XML file");
            Logger.TraceWriteLine("");

            Logger.TraceUnindent();
        }

    }
}
