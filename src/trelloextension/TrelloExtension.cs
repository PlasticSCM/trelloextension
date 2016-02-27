using System;
using System.Collections.Generic;

using log4net;

namespace Codice.Client.IssueTracker.Trello
{
    public class TrelloExtension : IPlasticIssueTrackerExtension
    {
        public TrelloExtension(IssueTrackerConfiguration config)
        {
            mConfig = config;
        }

        void IPlasticIssueTrackerExtension.Connect()
        {
            mLog.Debug("Logging in...");

            // TODO

            mLog.Debug("Log in successful!");
        }

        void IPlasticIssueTrackerExtension.Disconnect()
        {
            // TODO

            mLog.Debug("Logged out.");
        }

        string IPlasticIssueTrackerExtension.GetExtensionName()
        {
            return "Trello";
        }

        List<PlasticTask> IPlasticIssueTrackerExtension.GetPendingTasks()
        {
            // TODO
            return new List<PlasticTask>();
        }

        List<PlasticTask> IPlasticIssueTrackerExtension.GetPendingTasks(
            string assignee)
        {
            // TODO
            return new List<PlasticTask>();
        }

        PlasticTask IPlasticIssueTrackerExtension.GetTaskForBranch(
            string fullBranchName)
        {
            // TODO
            return null;
        }

        Dictionary<string, PlasticTask> IPlasticIssueTrackerExtension.GetTasksForBranches(
            List<string> fullBranchNames)
        {
            // TODO
            return new Dictionary<string, PlasticTask>();
        }

        List<PlasticTask> IPlasticIssueTrackerExtension.LoadTasks(
            List<string> taskIds)
        {
            // TODO
            return new List<PlasticTask>();
        }

        void IPlasticIssueTrackerExtension.LogCheckinResult(
            PlasticChangeset changeset, List<PlasticTask> tasks)
        {
            // TODO
        }

        void IPlasticIssueTrackerExtension.MarkTaskAsOpen(
            string taskId, string assignee)
        {
            // TODO
        }

        void IPlasticIssueTrackerExtension.OpenTaskExternally(
            string taskId)
        {
            // TODO
        }

        bool IPlasticIssueTrackerExtension.TestConnection(
            IssueTrackerConfiguration configuration)
        {
            // TODO
            return false;
        }

        void IPlasticIssueTrackerExtension.UpdateLinkedTasksToChangeset(
            PlasticChangeset changeset, List<string> tasks)
        {
            // TODO
        }

        IssueTrackerConfiguration mConfig;

        static readonly ILog mLog = LogManager.GetLogger("TrelloExtension");
    }
}
