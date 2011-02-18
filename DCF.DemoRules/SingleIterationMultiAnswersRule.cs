using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayer;
using DCF.Lib;

namespace DCF.DemoRules
{
    public class SingleIterationMultiAnswersRule : MultipleAnswersSample
    {
        public SingleIterationMultiAnswersRule(MySqlUtils sqlUtils, IRuleSupplier ruleSupplier, string category) :
            base(sqlUtils, ruleSupplier, category)
        { }

        public override void SampleWithJoin(Dictionary<string, object> data)
        {
            base.SampleWithJoin(data);
            OnStopCleaningProcess(); // stop iterating
        }
    }
}
