using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;
using Utilities = I95Dev.Connector.UI.Base.Helpers.Utilities;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard.Ecommerce
{
    public class EcommerceDashboardViewModel : BaseViewModel
    {
        #region Properties

        private readonly ISqlService localSql;
        private MessageSearchModel searchData;

        /// <summary>
        /// Gets or sets the search data.
        /// </summary>
        /// <value>
        /// The search data.
        /// </value>
        public MessageSearchModel SearchData
        {
            get { return searchData; }
            set { SetProperty(ref searchData, value); }
        }

        /// <summary>
        /// Gets or sets the report data.
        /// </summary>
        /// <value>
        /// The report data.
        /// </value>
        public ObservableRangeCollection<MessageSummaryModel> ReportData { get; private set; }

        #endregion Properties

        public EcommerceDashboardViewModel(ISqlService localSql) : this(localSql, -7)
        {
        }

        private EcommerceDashboardViewModel(ISqlService localSql, int dateCount) : this(localSql, DateTime.Today.AddDays(dateCount))
        {
        }

        public EcommerceDashboardViewModel(ISqlService localSql, DateTime startTime)
        {
            this.localSql = localSql;
            ReportData = new ObservableRangeCollection<MessageSummaryModel>();
            SearchData = new MessageSearchModel
            {
                CreatedFrom = startTime,
                CreatedTo = DateTime.Today.AddDays(1),
            };

            SubmitCommand = new BaseCommand(LoadReport);
            ProcessingCommand = new BaseCommand(ViewProcessing);
            ErrorCommand = new BaseCommand(ViewError);
            SuccessCommand = new BaseCommand(ViewSuccess);
            CompleteCommand = new BaseCommand(ViewComplete);
            TotalCommand = new BaseCommand(ViewTotal);

            LoadReport();
        }

        #region Commands

        /// <summary>
        /// Gets the submit command.
        /// </summary>
        /// <value>
        /// The submit command.
        /// </value>
        public ICommand SubmitCommand { get; }

        /// <summary>
        /// Loads the report.
        /// </summary>
        private void LoadReport()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");

            IList<MessageCountModel> messages = localSql.GetEcommerceMessageSummary(SearchData);
            CountMessages(messages);

            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        /// <summary>
        /// Gets the processing command.
        /// </summary>
        /// <value>
        /// The processing command.
        /// </value>
        public ICommand ProcessingCommand { get; }

        /// <summary>
        /// Views the processing.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void ViewProcessing(object args)
        {
            MessageSummaryModel model = args as MessageSummaryModel;
            ViewRecords(model.EntityId, (int)MessageStatus.Processing, model.ProcessingCount);
        }

        /// <summary>
        /// Gets the error command.
        /// </summary>
        /// <value>
        /// The error command.
        /// </value>
        public ICommand ErrorCommand { get; }

        /// <summary>
        /// Views the error.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void ViewError(object args)
        {
            MessageSummaryModel model = args as MessageSummaryModel;
            ViewRecords(model.EntityId, (int)MessageStatus.Error, model.ErrorCount);
        }

        /// <summary>
        /// Gets the success command.
        /// </summary>
        /// <value>
        /// The success command.
        /// </value>
        public ICommand SuccessCommand { get; }

        /// <summary>
        /// Views the success.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void ViewSuccess(object args)
        {
            MessageSummaryModel model = args as MessageSummaryModel;
            ViewRecords(model.EntityId, (int)MessageStatus.RequestProcessed, model.SuccessCount);
        }

        /// <summary>
        /// Gets the complete command.
        /// </summary>
        /// <value>
        /// The complete command.
        /// </value>
        public ICommand CompleteCommand { get; }

        /// <summary>
        /// Views the complete.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void ViewComplete(object args)
        {
            MessageSummaryModel model = args as MessageSummaryModel;
            ViewRecords(model.EntityId, (int)MessageStatus.Complete, model.CompleteCount);
        }

        /// <summary>
        /// Gets the total command.
        /// </summary>
        /// <value>
        /// The total command.
        /// </value>
        public ICommand TotalCommand { get; }

        /// <summary>
        /// Views the total.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void ViewTotal(object args)
        {
            MessageSummaryModel model = args as MessageSummaryModel;
            ViewRecords(model.EntityId, 0, model.TotalCount);
        }

        #endregion Commands

        #region Events

        /// <summary>
        /// Occurs when [view records clicked].
        /// </summary>
        internal event ViewRecordsEventHandler ViewRecordsClicked;

        internal delegate void ViewRecordsEventHandler(MessageSearchModel searchModel);

        #endregion Events

        #region Methods

        /// <summary>
        /// Counts the messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <returns></returns>
        private void CountMessages(IList<MessageCountModel> messages)
        {
            ReportData.Clear();
            foreach (string entityName in messages.Select(c1 => c1.EntityName).Distinct().OrderBy(c1 => c1))
            {
                int entityId = messages.First(c1 => c1.EntityName == entityName).EntityId;

                ReportData.Add(new MessageSummaryModel
                {
                    EntityName = entityName,
                    EntityId = entityId,
                    ProcessingCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.Processing).Sum(c1 => c1.Count),
                    ErrorCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.Error).Sum(c1 => c1.Count),
                    SuccessCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.RequestProcessed).Sum(c1 => c1.Count),
                    CompleteCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.Complete).Sum(c1 => c1.Count),
                    IsEnabled = true
                });
            }

            ReportData.Add(new MessageSummaryModel
            {
                EntityName = "Total by Status",
                EntityId = 0,
                ProcessingCount = ReportData.Sum(c1 => c1.ProcessingCount),
                ErrorCount = ReportData.Sum(c1 => c1.ErrorCount),
                SuccessCount = ReportData.Sum(c1 => c1.SuccessCount),
                CompleteCount = ReportData.Sum(c1 => c1.CompleteCount),
                IsEnabled = false
            });
        }

        /// <summary>
        /// Views the records.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="statusId">The status identifier.</param>
        /// <param name="recordCount">The record count.</param>
        private void ViewRecords(int entityId, int statusId, int recordCount)
        {
            if (recordCount == 0)
            {
                NotificationHelper.ShowMessage(Utilities.GetResourceValue("NoRecordsToShow"), Utilities.GetResourceValue("CaptionInfo"));
                return;
            }

            ViewRecordsEventHandler handler = ViewRecordsClicked;
            if (handler == null) return;
            SearchData.StatusId = statusId;
            SearchData.EntityId = entityId;
            handler.Invoke(SearchData);
        }

        #endregion Methods
    }
}