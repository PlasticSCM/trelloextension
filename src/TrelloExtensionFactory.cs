using System.Collections.Generic;
using TrelloNet;

namespace Codice.Client.IssueTracker.Trello
{
    public class TrelloExtensionFactory : IPlasticIssueTrackerExtensionFactory
    {
        IssueTrackerConfiguration IPlasticIssueTrackerExtensionFactory.GetConfiguration(
            IssueTrackerConfiguration storedConfiguration)
        {
            return new IssueTrackerConfiguration(
                GetWorkingMode(storedConfiguration),
                GetValidParameterList(storedConfiguration));
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

        ExtensionWorkingMode GetWorkingMode(IssueTrackerConfiguration configuration)
        {
            if (configuration == null
                || configuration.WorkingMode == ExtensionWorkingMode.None)
            {
                return ExtensionWorkingMode.TaskOnBranch;
            }

            return configuration.WorkingMode;
        }

        List<IssueTrackerConfigurationParameter> GetValidParameterList(
            IssueTrackerConfiguration config)
        {
            List<IssueTrackerConfigurationParameter> parameters =
                new List<IssueTrackerConfigurationParameter>();

            parameters.Add(new IssueTrackerConfigurationParameter
            {
                Name = TrelloExtension.BRANCH_PREFIX_KEY,
                Value = GetValidParameterValue(
                    config, TrelloExtension.BRANCH_PREFIX_KEY, "scm"),
                Type = IssueTrackerConfigurationParameterType.BranchPrefix,
                IsGlobal = true
            });

            parameters.Add(new IssueTrackerConfigurationParameter
            {
                Name = "TOKEN_LABEL",
                Value = string.Format(
                    "Open the URL below to log in to Trello and retrieve the {0}.",
                    TrelloExtension.TOKEN_KEY),
                Type = IssueTrackerConfigurationParameterType.Label,
                IsGlobal = true
            });

            ITrello trello = new TrelloNet.Trello(TrelloExtension.API_KEY);
            var url = trello.GetAuthorizationUrl(
                "Plastic SCM Trello Extension", Scope.ReadWrite, Expiration.Never);

            parameters.Add(new IssueTrackerConfigurationParameter
            {
                Name = TrelloExtension.LOGIN_URL,
                Value = url.ToString(),
                Type = IssueTrackerConfigurationParameterType.Text,
                IsGlobal = true
            });

            parameters.Add(new IssueTrackerConfigurationParameter
            {
                Name = TrelloExtension.TOKEN_KEY,
                Value = GetValidParameterValue(
                    config, TrelloExtension.TOKEN_KEY, string.Empty),
                Type = IssueTrackerConfigurationParameterType.Text,
                IsGlobal = false
            });

            return parameters;
        }

        string GetValidParameterValue(
            IssueTrackerConfiguration config, string paramName, string defaultValue)
        {
            string configValue =
                (config != null) ? config.GetValue(paramName) : null;

            if (string.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }
    }
}
