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

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard.Erp
{
    public class ErpDashboardViewModel : BaseViewModel
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ErpDashboardViewModel"/> class.
        /// </summary>
        /// <param name="localSql">The local SQL.</param>
        public ErpDashboardViewModel(ISqlService localSql) : this(localSql, -7)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErpDashboardViewModel"/> class.
        /// </summary>
        /// <param name="localSql">The local SQL.</param>
        /// <param name="previousDays">The previous days.</param>
        private ErpDashboardViewModel(ISqlService localSql, int previousDays) : this(localSql, DateTime.Today.AddDays(previousDays))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErpDashboardViewModel"/> class.
        /// </summary>
        /// <param name="localSql">The local SQL.</param>
        /// <param name="startTime">The start time.</param>
        public ErpDashboardViewModel(ISqlService localSql, DateTime startTime)
        {
            this.localSql = localSql;

            ReportData = new ObservableRangeCollection<MessageSummaryModel>();
            SearchData = new MessageSearchModel
            {
                CreatedFrom = startTime,
                CreatedTo = DateTime.Today.AddDays(1),
            };

            LoadCommand = new BaseCommand(LoadReport);
            TransferredCommand = new BaseCommand(ViewTransferred);
            ProcessingCommand = new BaseCommand(ViewProcessing);
            ErrorCommand = new BaseCommand(ViewError);
            SuccessCommand = new BaseCommand(ViewSuccess);
            CompleteCommand = new BaseCommand(ViewComplete);
            TotalCommand = new BaseCommand(ViewTotal);
            TransferPendingCommand = new BaseCommand(ViewTransferPending);

            LoadReport();
        }

        #region Commands

        /// <summary>
        /// Gets the load command.
        /// </summary>
        /// <value>
        /// The load command.
        /// </value>
        public ICommand LoadCommand { get; }

        /// <summary>
        /// Loads the summary.
        /// </summary>
        private void LoadReport()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");

            IList<MessageCountModel> messages = localSql.GetErpMessageSummary(SearchData);
            CountMessages(messages);

            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        /// <summary>
        /// Gets the pending command.
        /// </summary>
        /// <value>
        /// The pending command.
        /// </value>
        public ICommand TransferredCommand { get; }

        /// <summary>
        /// Views the pending.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void ViewTransferred(object args)
        {
            MessageSummaryModel model = args as MessageSummaryModel;
            if (model != null) ViewRecords(model.EntityId, (int)MessageStatus.RequestTransferred, model.SuccessCount);
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
            if (model != null) ViewRecords(model.EntityId, (int)MessageStatus.Processing, model.ProcessingCount);
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
            if (model != null) ViewRecords(model.EntityId, (int)MessageStatus.Error, model.ErrorCount);
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
            if (model != null) ViewRecords(model.EntityId, (int)MessageStatus.ResponseReceived, model.ResponseReceivedCount);
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
            if (model != null) ViewRecords(model.EntityId, (int)MessageStatus.Complete, model.CompleteCount);
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
            if (model != null) ViewRecords(model.EntityId, 0, model.TotalCount);
        }

        /// <summary>
        /// Gets the transfer command.
        /// </summary>
        /// <value>
        /// The transfer command.
        /// </value>
        public ICommand TransferPendingCommand { get; }

        /// <summary>
        /// Views the transfer.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void ViewTransferPending(object args)
        {
            MessageSummaryModel model = args as MessageSummaryModel;
            if (model != null) ViewRecords(model.EntityId, (int)MessageStatus.TransferPending, model.PendingCount);
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
                    PendingCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.TransferPending).Sum(c1 => c1.Count),
                    ProcessingCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.Processing).Sum(c1 => c1.Count),
                    SuccessCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.RequestTransferred).Sum(c1 => c1.Count),
                    ErrorCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.Error).Sum(c1 => c1.Count),
                    CompleteCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.Complete).Sum(c1 => c1.Count),
                    ResponseReceivedCount = messages.Where(c1 => c1.EntityId == entityId && c1.StatusId == (int)MessageStatus.ResponseReceived).Sum(c1 => c1.Count),
                    IsEnabled = true
                });
            }

            ReportData.Add(new MessageSummaryModel
            {
                EntityName = Utilities.TotalStatusName,
                EntityId = 0,
                PendingCount = ReportData.Sum(c1 => c1.PendingCount),
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