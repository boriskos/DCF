﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using DCF.Common;
using System.Diagnostics;
using DCF.DataLayerAwareLib;

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
                Logger.TraceWriteLine(string.Format("Starting the Rule Generation at {0}", startTime.ToLongTimeString()));
                switch (args[0])
                {
                    case "generate":
                        if (!GenerateRules(args))
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
                    default:
                        Usage();
                        break;
                }
                DateTime endTime = DateTime.Now;
                Logger.TraceWriteLine(string.Format("Finishing the Rule Generation at {0}. Total runtime is {1} sec",
                    endTime.ToLongTimeString(), (endTime - startTime).TotalSeconds));
            }
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

            Logger.TraceUnindent();
        }

    }
}
