using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCF.DataLayer
{
    public enum TopicType
    {
        SingleAnswer = 0,
        MultipleAnswers = 1,
        MultipleChoiseAnswer = 2
    }
    public static class TableConstants
    {
        public const string UserScores = "userscores";
        public const string UserScoresHistory = "userscoreshistory";
        public const string Users = "users_v";
        public const string BelievedUsers = "believedusers";
        public const string UserCapitals = "usercapitals";
        public const string WikiCities = "wikicities";
        public const string CleanCities = "cleancities";
        public const string WrongCities = "wrongcities";
        public const string CorrectCities = "correctcities";
        public const string EncodedUserCapitals = "encodedusercapitals";
        public const string AbsoluteUserScoresView = "absoluteuserscores_view";
        public const string ScoredFacts = "scoredfacts";
        public const string ScoredFactsView = "scoredfacts_view";
        public const string RepKeyResults = "RepKeyResults";
        public const string CapitalsScoresView = "CapitalScores_view";
        public const string CorrectUserAnswerCountView = "CorrectUserAnswerCount_View";
        public const string AllUserAnswerCountView = "AllUserAnswerCount_View";
        public const string Parlaments = "parlaments";
        public const string CleanCitiesFactsView = "cleancitiesfacts_view";
        public const string UserCleanFactsCountView = "usercleanfactscount_view";
        public const string ScoredFactsUsers = "scoredfactsusers";
        public const string ScoredFactsUsersView = "scoredfactsusers_view";
        public const string UserFactScores = "userfactscores";
        public const string QualityCounter = "qualitycounter";
        public const string CorrectFacts = "correctfacts";
        public const string UserMayors = "UserMayors";

        public const string ItemsMentions = "ItemsMentions_v";
        public const string Items = "Items_v";
        public const string Topics = "Topics";
    }
}
