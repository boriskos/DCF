using System;
using System.Collections.Generic;

namespace DCF.Lib
{
    /// <summary>
    /// Represents the Rules provider for this cleaning session 
    /// </summary>
    /// <remarks>Produces each time new collection of new rules</remarks>
    public interface IRuleSupplier
    {

        IList<Rule> GetCleaningRules();
        IList<Rule> GetSampleRules();
    }
}
