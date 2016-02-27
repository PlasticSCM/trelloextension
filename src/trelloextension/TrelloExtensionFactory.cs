using System;

namespace Codice.Client.IssueTracker.Trello
{
    public class TrelloExtensionFactory : IPlasticIssueTrackerExtensionFactory
    {
        IssueTrackerConfiguration IPlasticIssueTrackerExtensionFactory.GetConfiguration(
            IssueTrackerConfiguration storedConfiguration)
        {
            // TODO
            return null;
        }

        IPlasticIssueTrackerExtension IPlasticIssueTrackerExtensionFactory.GetIssueTrackerExtension(
            IssueTrackerConfiguration configuration)
        {
            return new TrelloExtension(configuration);
        }

        string IPlasticIssueTrackerExtensionFactory.GetIssueTrackerName()
        {
            return "Trello";
        }
    }
}
