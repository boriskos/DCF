using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCF.DataLayer;
using DCF.Lib;

namespace DCF.DataLayerAwareLib
{
    public class DcfRule : MySqlRule
    {
        public DcfRule(MySqlUtils sqlUtils, IRuleSupplier ruleSupplier, string category) :
            base(sqlUtils, ruleSupplier)
        {
            Category = category;
        }

        public string Category { get; protected set; }
    }
}
