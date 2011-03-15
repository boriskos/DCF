using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.Lib;
using DCF.DataLayer;

namespace DCF.DemoRules
{
    public class OnlineCleaningRuleProvider: IRuleSupplier
    {
        public DCF.DataLayer.MySqlUtils MySqlUtils
        {
            get
            {
                return m_sqlUstils;
            }
        }

        #region IRuleSupplier Members

        public IList<DCF.Lib.Rule> GetCleaningRules()
        {
            return null;
        }

        public IList<DCF.Lib.Rule> GetSampleRules()
        {
            return new List<DCF.Lib.Rule>() 
            { 
                new SingleIterationMultiAnswersRule(m_sqlUstils, this, "Demo"),
                new SingleIterationSingleAnswersRule(m_sqlUstils, this, "Demo")
            };
        }

        #endregion
        public OnlineCleaningRuleProvider(MySqlUtils utils)
        {
            m_sqlUstils = utils;
        }
        private MySqlUtils m_sqlUstils; 
    }
}
