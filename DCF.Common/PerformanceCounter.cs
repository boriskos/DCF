using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DCF.Common
{
    public struct PerformanceCounter: IDisposable
    {
        /// <summary>
        /// Constructor of new Performance Counter
        /// </summary>
        /// <param name="name">name of performance counter (may be already existing)</param>
        public PerformanceCounter(string name)
        {
            m_timerName = name;
            PerformanceCounterStatic.StartTimer(name);
        }

        /// <summary>
        /// Starts the stop watch
        /// </summary>
        public void Start()
        {
            PerformanceCounterStatic.StartTimer(m_timerName);
        }
        /// <summary>
        /// Stops the stop watch
        /// </summary>
        public void Stop()
        {
            PerformanceCounterStatic.StopTimer(m_timerName);
        }

        #region IDisposable Members
        /// <summary>
        /// Stops the stop watch
        /// </summary>
        public void Dispose()
        {
            PerformanceCounterStatic.StopTimer(m_timerName);
        }

        #endregion

        private string m_timerName;
    }

    public static class PerformanceCounterStatic
    {
        /// <summary>
        /// Map of all the watches per name
        /// </summary>
        private static Dictionary<string, Stopwatch> m_stopWatches = 
            new Dictionary<string, Stopwatch>();

        /// <summary>
        /// Starts the timer accessed by name
        /// </summary>
        /// <remarks>
        /// If timer does not exist this function creates it
        /// </remarks>
        /// <param name="timerName">timer name to start</param>
        public static void StartTimer(string timerName)
        {
            Stopwatch sw = GetStopwatchTimer(timerName);
            if (!sw.IsRunning)
            {
                sw.Start();
            }
        }

        /// <summary>
        /// Stops the timer by name
        /// </summary>
        /// <remarks>
        /// If timer does not exists or it is not running the function throws exception
        /// </remarks>
        /// <param name="timerName">name of the timer to stop</param>
        public static void StopTimer(string timerName)
        {
            Stopwatch sw = m_stopWatches[timerName];
            if (sw != null && sw.IsRunning)
            {
                sw.Stop();
            }
        }

        /// <summary>
        /// Returns elapsed time for a specific timer
        /// </summary>
        /// <remarks>
        /// The function creates the timer if it is unknown
        /// </remarks>
        /// <param name="timerName">timer name to be queried</param>
        /// <returns>Period in <see cref="TimeSpan"/> when the timer was on</returns>
        public static TimeSpan Elapsed(string timerName)
        {
            Stopwatch sw = GetStopwatchTimer(timerName);
            return sw.Elapsed;
        }

        /// <summary>
        /// Generates report for all existing timers
        /// </summary>
        /// <returns>report string</returns>
        public static string ReportAllTimers()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string name in m_stopWatches.Keys.OrderBy(a => a))
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                }
                sb.Append(ReportTimer(name));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Generates report for given timer.
        /// </summary>
        /// <remarks>
        /// if the timer does not exist, the report will be of form: *&lt;timer name&gt; 0 ms
        /// </remarks>
        /// <param name="timerName">timer name</param>
        /// <returns>report string</returns>
        public static string ReportTimer(string timerName)
        {
            Stopwatch sw = null;
            if (m_stopWatches.TryGetValue(timerName, out sw))
            {
                return string.Format("{0}:\t{1} ms", timerName, sw.ElapsedMilliseconds);
            }
            else
            {
                return string.Format("*{0}:\t0 ms", timerName);
            }
        }
        /// <summary>
        /// Returns <see cref="Stopwatch"/> timer by name
        /// </summary>
        /// <param name="timerName">name of timer</param>
        /// <returns>new timer if does not exit in the internal map</returns>
        public static Stopwatch GetStopwatchTimer(string timerName)
        {
            Stopwatch sw = null;
            if (!m_stopWatches.TryGetValue(timerName, out sw))
            {
                sw = new Stopwatch();
                m_stopWatches.Add(timerName, sw);
            }
            return sw;
        }

    }
}
