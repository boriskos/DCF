using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCF.Lib
{
    public enum WellKnownRules
    {
        InitializeCleaningSession,
        SelectTrustedUsers,
        CopyTrustedUsersInformationToCleanTable,
        CreateCorrectAndWrongCitiesTable,
        UpdateUserScores,
        RepairKeyOnCleanTable,
        RepairCountryKeyOnCleanTable,
        RepairCityKeyOnCleanTable,

        CheckCleanCitiesConsistency
    }

}
