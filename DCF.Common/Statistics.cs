using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCF.Common
{
    public static class StatisitcNames
    {
        public const string GenerationCorrectAnswers = "GenerationCorrectAnswers";
        public const string GenerationIncorrectAnswers = "GenerationIncorrectAnswers";
        public const string GenerationRestrictedTopics = "GenerationRestrictedTopics";
    }
    public class Statistics
    {
        public Statistics(string name)
        {
            m_state = StatisticsStatic.GetState(name);
        }
        public Statistics(string name, int defaultValue)
        {
            m_state = StatisticsStatic.GetState(name);
            Value = defaultValue;
        }

        public void Append()
        {
            Append(1);
        }

        public void Append(int delta)
        {
            m_state.value += delta;
        }

        public int Value
        {
            get
            {
                return m_state.value;
            }
            set
            {
                m_state.value = value;
            }
        }

        // this is a reference class so that its change in Append method will influence the static map
        internal class State
        {
            public int value=0;
        }

        private State m_state;
    }

    internal static class StatisticsStatic
    {

        internal static Statistics.State GetState(string name)
        {
            Statistics.State st;
            if (!m_name2state.TryGetValue(name, out st))
            {
                st = new Statistics.State();
                m_name2state.Add(name, st);
            }
            return st;
        }

        private static Dictionary<string, Statistics.State> m_name2state = 
            new Dictionary<string, Statistics.State>();
    }
}
