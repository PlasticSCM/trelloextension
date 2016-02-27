using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

            mBoard = GetBoardFromConfig();

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
            return mTrello.Cards.Search(
                ONLY_OPEN_CARDS_QUERY, MAX_RESULTS, GetSearchFilter())
                .Select(card => GetPlasticTaskFromCard(card)).ToList();
        }

        public List<PlasticTask> GetPendingTasks(string assignee)
        {
            return mTrello.Cards.ForMe(CardFilter.Open)
                .Where(card => mBoard == null || card.IdBoard == mBoard.GetBoardId())
                .Select(card => GetPlasticTaskFromCard(card))
                .ToList();
        }

        public PlasticTask GetTaskForBranch(string fullBranchName)
        {
            var card = GetCardFromId(ExtractCardIdFromBranch(fullBranchName));

            if (card == null)
                return null;

            return GetPlasticTaskFromCard(card);
        }

        public Dictionary<string, PlasticTask> GetTasksForBranches(
            List<string> fullBranchNames)
        {
            var result = new Dictionary<string, PlasticTask>();

            foreach (string fullBranchName in fullBranchNames)
                result.Add(fullBranchName, GetTaskForBranch(fullBranchName));

            return result;
        }

        public List<PlasticTask> LoadTasks(List<string> taskIds)
        {
            var cards = taskIds.Select(cardId => GetCardFromId(cardId));

            return cards.Select(card => GetPlasticTaskFromCard(card)).ToList();
        }

        public void LogCheckinResult(
            PlasticChangeset changeset, List<PlasticTask> tasks)
        {
            var cards = tasks.Select(task => GetCardFromId(task.Id));

            foreach (Card card in cards)
            {
                if (card == null)
                    continue;

                mTrello.Cards.AddComment(card, GetPrintableChangeset(changeset));
            }
        }

        public void MarkTaskAsOpen(string taskId, string assignee)
        {
            Card card = GetCardFromId(taskId);

            if (card == null)
                return;

            mTrello.Cards.AddMember(card, mTrello.Members.Me());
        }

        public void OpenTaskExternally(string taskId)
        {
            Card card = GetCardFromId(taskId);

            if (card == null)
                return;

            Process.Start(card.Url);
        }

        public bool TestConnection(IssueTrackerConfiguration configuration)
        {
            mTrello.Authorize(configuration.GetValue(
                configuration.GetValue(TOKEN_KEY)));
            mTrello.Deauthorize();
            return true;
        }

        public void UpdateLinkedTasksToChangeset(
            PlasticChangeset changeset, List<string> tasks)
        {
            // TODO
        }

        Card GetCardFromId(string cardId)
        {
            if (string.IsNullOrEmpty(cardId))
                return null;

            if (mBoard == null)
            {
                try
                {
                    return mTrello.Cards.WithId(cardId);
                }
                catch (Exception e)
                {
                    mLog.ErrorFormat(
                        "Unable to retrieve card with ID '{0}': {1}", cardId, e.Message);
                    return null;
                }
            }

            int shortId;
            if (!int.TryParse(cardId, out shortId))
            {
                mLog.ErrorFormat(
                    "Unable to parse card short ID '{0}'", cardId);
                return null;
            }

            return mTrello.Cards.WithShortId(shortId, mBoard);
        }

        Board GetBoardFromConfig()
        {
            var boardUrl = mConfig.GetValue(BOARD_URL_KEY);

            if (string.IsNullOrEmpty(boardUrl))
                return null;

            try
            {
                return mTrello.Boards.ForMe()
                    .Where(board => board.Url == boardUrl).Single();
            }
            catch(Exception e)
            {
                mLog.ErrorFormat(
                    "Unable to retrieve board '{0}': {1}", boardUrl, e.Message);
                return null;
            }
        }

        PlasticTask GetPlasticTaskFromCard(Card card)
        {
            if (card == null)
                return null;

            return new PlasticTask
            {
                Id = (mBoard != null) ? card.IdShort.ToString() : card.Id,
                Owner = GetMembersString(card),
                Status = "Open",
                Title = card.Name,
                Description = card.Desc
            };
        }

        string GetMembersString(Card card)
        {
            if (card.IdMembers.Count == 0)
                return string.Empty;

            return mTrello.Members.ForCard(card)
                .Select(member => member.FullName)
                .Aggregate((current, next) => current + ", " + next);
        }

        SearchFilter GetSearchFilter()
        {
            if (mBoard == null)
                return null;

            return new SearchFilter { Boards = new IBoardId[] { mBoard } };
        }

        string ExtractCardIdFromBranch(string fullBranchName)
        {
            var branchName = GetBranchName(fullBranchName);

            if (string.IsNullOrEmpty(branchName))
                return string.Empty;

            var prefix = mConfig.GetValue(BRANCH_PREFIX_KEY) ?? string.Empty;
            if (prefix == string.Empty)
                return branchName;

            if (!branchName.StartsWith(prefix) || prefix == branchName)
                return string.Empty;

            return branchName.Substring(prefix.Length);
        }

        string GetBranchName(string fullBranchName)
        {
            var lastSeparator = fullBranchName.LastIndexOf('/');

            if (lastSeparator < 0)
                return fullBranchName;

            if (lastSeparator == fullBranchName.Length - 1)
                return string.Empty;

            return fullBranchName.Substring(lastSeparator + 1);
        }

        string GetPrintableChangeset(PlasticChangeset changeset)
        {
            // TODO
            return string.Empty;
        }

        IssueTrackerConfiguration mConfig;

        ITrello mTrello;
        Board mBoard;

        static readonly ILog mLog = LogManager.GetLogger("TrelloExtension");

        internal const string EMAIL_KEY = "E-mail";
        internal const string PASSWORD_KEY = "Password";
        internal const string BRANCH_PREFIX_KEY = "Branch prefix";
        internal const string LOGIN_URL = "Login URL";
        internal const string TOKEN_KEY = "API token";
        internal const string BOARD_URL_KEY = "Board URL";

        internal const string API_KEY = "fe72b23308f2a49cb5591615fc99aa1d";

        const int MAX_RESULTS = 50;
        const string ONLY_OPEN_CARDS_QUERY = "is:open";
    }
}
