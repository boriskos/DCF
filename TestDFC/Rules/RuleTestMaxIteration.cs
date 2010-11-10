using System;
using System.Collections.Generic;
using System.Text;
using DCF.Lib;

namespace TestDFC
{
    public class RuleTestMaxIteration : DCF.Lib.Rule
    {
        public RuleTestMaxIteration(IRuleSupplier ruleSupplier)
            : base(ruleSupplier)
        {
            m_ruleExecuter += new RuleExecuterDelegate(RuleTestMaxIteration_m_ruleExecuter);
            this.Id = "RuleTestMaxIteration";
        }

        public int CurIteration { get { return m_curIteration; } }

        void RuleTestMaxIteration_m_ruleExecuter(Dictionary<string, object> dataHashTable)
        {
            m_curIteration++;
        }

        private int m_curIteration = 0;
    }
}
