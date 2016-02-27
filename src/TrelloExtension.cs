using System;
using System.Collections.Generic;

using log4net;
using TrelloNet;

namespace Codice.Client.IssueTracker.Trello
{
    public class TrelloExtension : IPlasticIssueTrackerExtension
    {
        public TrelloExtension(IssueTrackerConfiguration config)
        {
            mConfig = config;

            mTrello = new TrelloNet.Trello(API_KEY);
        }

        public void Connect()
        {
            mLog.Debug("Logging in...");

            try
            {
                mTrello.Authorize(mConfig.GetValue(TOKEN_KEY));
            }
            catch (Exception e)
            {
                mLog.ErrorFormat("Unable to log in to trello: {0}", e.Message);
                return;
            }

            mLog.Debug("Log in successful!");
        }

        public void Disconnect()
        {
            mTrello.Deauthorize();
            mLog.Debug("Logged out.");
        }

        public string GetExtensionName()
        {
            return "Trello";
        }

        public List<PlasticTask> GetPendingTasks()
        {
            List<PlasticTask> tasks = new List<PlasticTask>();
            foreach (Card card in mTrello.Cards.Search("is:open", MAX_RESULTS))
            {
                tasks.Add(GetPlasticTaskFromCard(card));
            }
            return tasks;
        }

        public List<PlasticTask> GetPendingTasks(string assignee)
        {
            return GetPendingTasks();
        }

        public PlasticTask GetTaskForBranch(string fullBranchName)
        {
            // TODO
            return null;
        }

        public Dictionary<string, PlasticTask> GetTasksForBranches(
            List<string> fullBranchNames)
        {
            // TODO
            return new Dictionary<string, PlasticTask>();
        }

        public List<PlasticTask> LoadTasks(List<string> taskIds)
        {
            // TODO
            return new List<PlasticTask>();
        }

        public void LogCheckinResult(
            PlasticChangeset changeset, List<PlasticTask> tasks)
        {
            // TODO
        }

        public void MarkTaskAsOpen(string taskId, string assignee)
        {
            // TODO
        }

        public void OpenTaskExternally(string taskId)
        {
            // TODO
        }

        public bool TestConnection(IssueTrackerConfiguration configuration)
        {
            // TODO
            return false;
        }

        public void UpdateLinkedTasksToChangeset(
            PlasticChangeset changeset, List<string> tasks)
        {
            // TODO
        }

        PlasticTask GetPlasticTaskFromCard(Card card)
        {
            return new PlasticTask
            {
                Id = card.Id,
                Owner = GetMembersString(card),
                Status = "Open",
                Title = card.Name,
                Description = card.Desc
            };
        }

        string GetMembersString(Card card)
        {
            List<string> memberNames = new List<string>();

            foreach (Member member in mTrello.Members.ForCard(card))
            {
                memberNames.Add(member.FullName);
            }
            return string.Join(", ", memberNames);
        }

        IssueTrackerConfiguration mConfig;

        ITrello mTrello;

        static readonly ILog mLog = LogManager.GetLogger("TrelloExtension");

        internal const string EMAIL_KEY = "E-mail";
        internal const string PASSWORD_KEY = "Password";
        internal const string BRANCH_PREFIX_KEY = "Branch prefix";
        internal const string LOGIN_URL = "Login URL";
        internal const string TOKEN_KEY = "API token";

        internal const string API_KEY = "fe72b23308f2a49cb5591615fc99aa1d";

        const int MAX_RESULTS = 50;
    }
}
