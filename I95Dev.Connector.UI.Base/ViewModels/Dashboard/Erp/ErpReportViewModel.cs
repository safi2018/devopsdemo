using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.Base.Models.MessageQueue;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;
using Utilities = I95Dev.Connector.UI.Base.Helpers.Utilities;

namespace I95Dev.Connector.UI.Base.ViewModels.Dashboard.Erp
{
    public class ErpReportViewModel : BaseListViewModel<MessageReportModel>
    {
        #region Properties

        private ISqlService localSql;

        #region BindData

        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>
        /// The entities.
        /// </value>
        public IList<IntegerComboBoxModel> Entities { get; private set; }

        /// <summary>
        /// Gets or sets the statuses.
        /// </summary>
        /// <value>
        /// The statuses.
        /// </value>
        public IList<IntegerComboBoxModel> Statuses { get; private set; }

        private MessageSearchModel searchModel;

        /// <summary>
        /// Gets or sets the search model.
        /// </summary>
        /// <value>
        /// The search model.
        /// </value>
        public MessageSearchModel SearchModel
        {
            get { return searchModel; }
            set { SetProperty(ref searchModel, value); }
        }

        private bool selectAll;

        /// <summary>
        /// Gets or sets a value indicating whether [select all].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [select all]; otherwise, <c>false</c>.
        /// </value>
        public bool SelectAll
        {
            get
            {
                return selectAll;
            }
            set
            {
                if (!SetProperty(ref selectAll, value)) return;
                foreach (MessageReportModel item in ViewList.View)
                {
                    if (item.IsCheckBoxEnabled)
                        item.IsCheckBoxChecked = value;
                }
                ViewList.View.Refresh();
            }
        }

        #endregion BindData

        #endregion Properties

        public ErpReportViewModel(ISqlService localSql)
        {
            Initialize(localSql);
            LoadReport();
        }

        public ErpReportViewModel(ISqlService localSql, MessageSearchModel searchModel)
        {
            Initialize(localSql);
            LoadDataWithFilters(searchModel);
        }

        #region Commands

        /// <summary>
        /// Gets or sets the reset command.
        /// </summary>
        /// <value>
        /// The reset command.
        /// </value>
        public ICommand ResetCommand { get; private set; }

        /// <summary>
        /// Resets the fields.
        /// </summary>
        private void ResetFields()
        {
            int entityId = SearchModel.EntityId;
            SetDefaultFilters();
            SearchModel.EntityId = entityId;
            LoadReport();
        }

        /// <summary>
        /// Gets or sets the find command.
        /// </summary>
        /// <value>
        /// The find command.
        /// </value>
        public ICommand FindCommand { get; private set; }

        /// <summary>
        /// Gets the view command.
        /// </summary>
        /// <value>
        /// The view command.
        /// </value>
        public ICommand ViewCommand { get; private set; }

        /// <summary>
        /// Views the error.
        /// </summary>
        /// <param name="arg">The argument.</param>
        private void ViewError(object arg)
        {
            MessageReportModel model = arg as MessageReportModel;
            if (model != null)
            {
                NotificationHelper.ShowMessage(model.ErrorMessage, Utilities.GetResourceValue("CaptionError"));
            }
        }

        /// <summary>
        /// Gets or sets the submit command.
        /// </summary>
        /// <value>
        /// The submit command.
        /// </value>
        public ICommand SubmitCommand { get; private set; }

        private void SyncRecords()
        {
            if (!RecordList.Any(c1 => c1.IsCheckBoxChecked))
            {
                NotificationHelper.ShowMessage("Please select records to sync", Utilities.GetResourceValue("CaptionWarning"));
                return;
            }

            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");

            ThreadHelper.RunBackGround(delegate
            {
                int? entityId = RecordList.FirstOrDefault(c1 => c1.IsCheckBoxChecked)?.EntityId;
                if (!entityId.HasValue)
                {
                    NotificationHelper.ShowMessage("Entity not found", Utilities.GetResourceValue("CaptionWarning"));
                    return;
                }

                IList<long> processRecords = RecordList.Where(c1 => c1.IsCheckBoxChecked & (c1.StatusId == (int)MessageStatus.Error || c1.StatusId == (int)MessageStatus.TransferPending))
                                                .Select(c1 => c1.MessageId).ToList();

                if (processRecords.Any())
                {
                    Connector.Base.Services.Outbound.MessageSendService.SyncRecords(processRecords, entityId.Value);
                }

                IList<MessageModel> getResponse = RecordList.Where(c1 => c1.IsCheckBoxChecked & (c1.StatusId == (int)MessageStatus.RequestTransferred))
                                                    .Select(c1 => new MessageModel
                                                    {
                                                        CloudMessageId = c1.CloudMessageId,
                                                        LocalMessageId = c1.MessageId,
                                                        SyncCounter = c1.SyncCounter + 1,
                                                        MessageStatus = (MessageStatus)c1.StatusId
                                                    }).ToList();
                if (getResponse.Any())
                {
                    Connector.Base.Services.Outbound.ResponseReceiveService.PullResponse(getResponse, entityId.Value);
                }
                Application.Current.Dispatcher.Invoke(LoadReport);
                StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
            });
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize(ISqlService sqlService)
        {
            localSql = sqlService;
            BindData();
            SetDefaultFilters();
            if (Entities.Any(c1 => c1.Value == (int)Entity.SalesOrder))
                SearchModel.EntityId = (int)Entity.SalesOrder;
            else
            {
                SearchModel.EntityId = Entities.OrderBy(c1 => c1.Value).First().Value;
            }
            FindCommand = new BaseCommand(LoadReport);
            ResetCommand = new BaseCommand(ResetFields);
            ViewCommand = new BaseCommand(ViewError);
            SubmitCommand = new BaseCommand(SyncRecords);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            Entities = localSql.GetReverseEntities();
            Statuses = localSql.GetReverseStatuses();
            Statuses.Insert(0, new IntegerComboBoxModel { Name = "All", Value = 0 });
        }

        /// <summary>
        /// Sets the default filters.
        /// </summary>
        private void SetDefaultFilters()
        {
            SearchModel = new MessageSearchModel
            {
                StatusId = 0,
                CreatedFrom = DateTime.Today,
                CreatedTo = DateTime.Today.AddDays(1),
                ModifiedFrom = DateTime.Today,
                ModifiedTo = DateTime.Today.AddDays(1),
                CreatedFromHasValue = true,
                CreatedToHasValue = true,
            };
        }

        /// <summary>
        /// Loads the report.
        /// </summary>
        private void LoadReport()
        {
            SetRecordList(localSql.GetErpMessages(SearchModel));
        }

        /// <summary>
        /// Loads the filters.
        /// </summary>
        /// <param name="model">The search model.</param>
        internal void LoadDataWithFilters(MessageSearchModel model)
        {
            SearchModel.CreatedFrom = model.CreatedFrom;
            SearchModel.CreatedTo = model.CreatedTo;
            SearchModel.EntityId = model.EntityId;
            SearchModel.StatusId = model.StatusId;
            LoadReport();
        }

        #endregion Methods
    }
}