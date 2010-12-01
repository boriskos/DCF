using System;
using System.Collections.Generic;
using System.Text;
using DCF.Lib;
using DCF.DataLayer;

namespace DCF.DemoRules
{
    /// <summary>
    /// Returns list of two Rules for offline cleaning
    /// </summary>
    public class OfflineCleaningRuleProvider : IRuleSupplier
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
                new RepairKeySample(m_sqlUstils, this, "CountryOf"),
                new DanielConvergence(m_sqlUstils, this)
            };
        }

        #endregion
        public OfflineCleaningRuleProvider(MySqlUtils utils)
        {
            m_sqlUstils = utils;
        }
        private MySqlUtils m_sqlUstils; 
    }
}
