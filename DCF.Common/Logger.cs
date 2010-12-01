using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Configuration;

namespace DCF.Common
{
    public class Logger
    {
        #region Constants

        public const string RulesStr = "Rules";
        public const string CleaningDataStr = "Cleaning Data";

        #endregion Constants

        #region Static Public Properties
        public static TraceListenerCollection LoggerEventsListeners { get { return ts.Listeners; } }
        public static TraceListenerCollection LoggerTraceListeners { get { return Trace.Listeners; } }
        public static TraceListenerCollection LoggerDebugListeners { get { return Debug.Listeners; } }

        public static bool ShowSQLs
        {
            get
            {
                if (m_sShowSql == null)
                {
                    m_sShowSql = ConfigurationManager.AppSettings["ShowSQL"] != null &&
                        ConfigurationManager.AppSettings["ShowSQL"].Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                return m_sShowSql.Value;
            }
        }

        public static bool ShowTableContents
        {
            get
            {
                if (m_sShowTableContents == null)
                {
                    m_sShowTableContents = ConfigurationManager.AppSettings["ShowTableContents"] != null &&
                        ConfigurationManager.AppSettings["ShowTableContents"].Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                return m_sShowTableContents.Value;
            }
        } 
        #endregion

        #region Special methods implementation
        public static void PrintTable(DataTable table)
        {
            PrintTable(table, false);
        }

        public static void PrintTable(DataTable table, bool printCols)
        {
            if (!ShowTableContents) return;
            DebugWriteLine("Table " + table.TableName);
            DebugIndent();
            if (printCols) // output the column names
            {
                foreach (DataColumn col in table.Columns)
                {
                    int flength = 10;
                    formattingLengths.TryGetValue(col.DataType, out flength);
                    string formatStr = string.Format("{{0,-{0}}}\t", flength);
                    DebugWrite(string.Format(formatStr, col.ColumnName));
                }
                DebugWriteLine(String.Empty);
                DebugWriteLine("=====");
            }
            foreach (DataRow row in table.Rows) // prints rows
            {
                foreach (object item in row.ItemArray)
                {
                    int flength = 10;
                    formattingLengths.TryGetValue(item.GetType(), out flength);
                    string formatStr = string.Format("{{0,-{0}}}\t", flength);
                    DebugWrite(string.Format(formatStr, item));
                }
                DebugWriteLine(String.Empty);
            }
            DebugUnindent();
        }
        #endregion

        #region Standard Messages Method redirection
        public static void WriteLineIf(bool cond, string str)
        {
            Trace.WriteLineIf(cond, str);
        }

        public static void DebugWriteLine(string str)
        {
#if(DEBUG)
            Trace.WriteLine(str);
#endif
        }

        public static void TraceWriteLine(string str)
        {
            Trace.WriteLine(str);
        }
        public static void DebugWrite(string str)
        {
#if(DEBUG)
            Trace.Write(str);
#endif
        }

        public static void TraceWrite(string str)
        {
            Trace.Write(str);
        }

        public static void TraceIndent()
        {
            Trace.Indent();
        }
        public static void DebugIndent()
        {
#if(DEBUG)
            Trace.Indent();
#endif
        }
        public static void TraceUnindent()
        {
            Trace.Unindent();
        }
        public static void DebugUnindent()
        {
#if(DEBUG)
            Trace.Unindent();
#endif
        }

        public static void DebugWriteLine(string message, string category)
        {
#if(DEBUG)
            Trace.WriteLine(message, category);
#endif
        }

        public static void DebugFlush()
        {
#if(DEBUG)
            Trace.Flush();
#endif
        }

        public static void Assert(bool cond, string str)
        {
            Debug.Assert(cond, str);
        } 
        #endregion

        #region Private Static Memebers

        private static bool? m_sShowSql = null;
        private static bool? m_sShowTableContents = null;
        static Dictionary<Type, int> formattingLengths = new Dictionary<Type,int>() 
        { 
            { typeof(int), 6 }, 
            { typeof(double), 10},
            { typeof(string), 15}
        };
        private static TraceSource ts = new TraceSource("DataCleaning");
        #endregion

    }
}
