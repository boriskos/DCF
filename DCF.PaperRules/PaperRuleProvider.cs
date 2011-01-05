using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.Lib;
using DCF.DataLayer;
using DCF.DemoRules;
using DCF.Common;

namespace DCF.PaperRules
{
    public class PaperRuleProvider : IRuleSupplier
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
            CleaningConfiguration.ExperimentTypeEnum experiment = 
                (CleaningConfiguration.ExperimentTypeEnum)CleaningConfiguration.Instance.ExperimentType;
            Logger.TraceWriteLine(string.Format("Currently running cleaning with {0}", experiment));

            List<DCF.Lib.Rule> list = new List<Rule>(2);
            switch(experiment)
            {
                case CleaningConfiguration.ExperimentTypeEnum.Cosine:
                    list.Add( new Cosine(MySqlUtils, this, "CountryOf") );
                    list.Add(new DanielConvergence(m_sqlUstils, this));
                    break;
                case CleaningConfiguration.ExperimentTypeEnum.Majority:
                    list.Add( new Majority(MySqlUtils, this, "CountryOf") );
                    break;
                case CleaningConfiguration.ExperimentTypeEnum.RepeirPrimaryKey:
                    list.Add( new RepairKeySample(m_sqlUstils, this, "CountryOf") );
                    list.Add(new DanielConvergence(m_sqlUstils, this));
                    break;
                case CleaningConfiguration.ExperimentTypeEnum.TwoEstimates:
                    list.Add( new TwoEstimate(MySqlUtils, this, "CountryOf") );
                    list.Add(new DanielConvergence(m_sqlUstils, this));
                    break;
                default:
                    throw new System.Configuration.ConfigurationException("Invalid Experiment value " +
                        CleaningConfiguration.Instance.ExperimentType.ToString());
            }
            return list;
        }

        #endregion
        public PaperRuleProvider(MySqlUtils utils)
        {
            m_sqlUstils = utils;
        }
        private MySqlUtils m_sqlUstils; 

    }
}
