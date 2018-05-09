using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.DataAccess;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.Base.Models;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Helpers;
using Utilities = I95Dev.Connector.UI.Base.Helpers.Utilities;

namespace I95Dev.Connector.UI.Base.ViewModels.Admin
{
    public partial class ConfigurationViewModel : BaseViewModel
    {
        #region Properties

        #region Bind Data

        /// <summary>
        /// Gets or sets a value indicating whether this instance is update log configuration.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is update log configuration; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpdateLogConfiguration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is update scheduler configuration.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is update scheduler configuration; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpdateSchedulerConfiguration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is update entity configuration.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is update entity configuration; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpdateEntityConfiguration { get; set; }

        /// <summary>
        /// Gets the update configuration.
        /// </summary>
        /// <value>
        /// The update configuration.
        /// </value>
        public ICommand UpdateConfiguration { get; }

        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        /// <value>
        /// The days.
        /// </value>
        public IList<IntegerComboBoxModel> Days { get; private set; }

        /// <summary>
        /// Gets or sets the months.
        /// </summary>
        /// <value>
        /// The months.
        /// </value>
        public IList<IntegerComboBoxModel> Months { get; private set; }

        /// <summary>
        /// Gets the entity data.
        /// </summary>
        /// <value>
        /// The entity data.
        /// </value>
        public IList<EntityViewModel> EntityData { get; }

        /// <summary>
        /// Gets the scheduler data.
        /// </summary>
        /// <value>
        /// The scheduler data.
        /// </value>
        public IList<EntityViewModel> SchedulerData { get; }

        #endregion Bind Data

        #region Form Data

        /// <summary>
        /// Gets or sets a value indicating whether this instance is delete log files.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is delete log files; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleteLogFiles { get; set; }

        /// <summary>
        /// Gets or sets the log delete days.
        /// </summary>
        /// <value>
        /// The log delete days.
        /// </value>
        public string LogDeleteDays { get; set; }

        /// <summary>
        /// Gets or sets the log delete months.
        /// </summary>
        /// <value>
        /// The log delete months.
        /// </value>
        public string LogDeleteMonths { get; set; }

        #endregion Form Data

        #endregion Properties

        public ConfigurationViewModel()
        {
            EntityData = new List<EntityViewModel>();
            SchedulerData = new List<EntityViewModel>();
            UpdateConfiguration = new BaseCommand(CanUpdate, Update);
            FillDays();
            FillMonths();
            FillData();
        }

        /// <summary>
        /// Fills the data.
        /// </summary>
        private void FillData()
        {
            IsDeleteLogFiles = ConfigurationHelper.GetConfigurationValue<bool>(ConfigurationConstants.DeleteLogFiles);
            LogDeleteMonths = ConfigurationHelper.GetConfigurationValue(ConfigurationConstants.ZipLogDeleteMonths);
            LogDeleteDays = ConfigurationHelper.GetConfigurationValue(ConfigurationConstants.LogDeleteDays);

            foreach (EntityModel entityModel in EntityDataAccess.GetAllEntities())
            {
                EntityData.Add(new EntityViewModel
                {
                    EntityId = entityModel.EntityId,
                    EntityName = entityModel.EntityName,
                    IsInboundActive = entityModel.IsInboundActive,
                    IsOutboundActive = entityModel.IsOutboundActive
                });

                SchedulerData.Add(new EntityViewModel
                {
                    EntityId = entityModel.EntityId,
                    EntityName = entityModel.EntityName,
                    LastSyncTime = entityModel.LastSyncTime,
                });
            }
        }

        #region Methods

        /// <summary>
        /// Fills the days.
        /// </summary>
        private void FillDays()
        {
            Days = new List<IntegerComboBoxModel>();

            for (int i = 2; i <= 15; i++)
            {
                Days.Add(new IntegerComboBoxModel
                {
                    Value = i,
                    Name = i + " Days"
                });
            }
        }

        /// <summary>
        /// Fills the months.
        /// </summary>
        private void FillMonths()
        {
            Months = new List<IntegerComboBoxModel>();
            for (int i = 1; i <= 6; i++)
            {
                Months.Add(new IntegerComboBoxModel
                {
                    Value = i,
                    Name = i == 1 ? i + " Month" : i + " Months"
                });
            }
        }

        #endregion Methods

        #region Commands

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        private void Update()
        {
            try
            {
                bool isUpdated = false;
                if (IsUpdateLogConfiguration)
                {
                    ConfigurationHelper.SetConfigurationValue(ConfigurationConstants.DeleteLogFiles, IsDeleteLogFiles);
                    ConfigurationHelper.SetConfigurationValue(ConfigurationConstants.LogDeleteDays, LogDeleteDays);
                    ConfigurationHelper.SetConfigurationValue(ConfigurationConstants.ZipLogDeleteMonths, LogDeleteMonths);
                    isUpdated = true;
                }

                if (IsUpdateSchedulerConfiguration)
                {
                    IEnumerable<EntityViewModel> checkedRecords = SchedulerData.Where(c1 => c1.IsChecked).ToList();
                    if (checkedRecords.Any())
                    {
                        foreach (EntityViewModel model in checkedRecords)

                        {
                            if (model.LastSyncTime == null) continue;
                            var data = new EntityModel { EntityId = model.EntityId, EndTime = model.LastSyncTime.Value };
                            EntityDataAccess.UpdateLastSyncTime(data);
                        }
                        isUpdated = true;
                    }
                }

                if (IsUpdateEntityConfiguration)
                {
                    foreach (EntityViewModel model in EntityData)
                    {
                        var data = new EntityModel { EntityId = model.EntityId, IsInboundActive = model.IsInboundActive, IsOutboundActive = model.IsOutboundActive };
                        EntityDataAccess.UpdateEntityInformation(data);
                    }
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    NotificationHelper.ShowMessage(Utilities.GetResourceValue("DataUpdateSuccess"), Utilities.GetResourceValue("CaptionInfo"));
                }
                else
                {
                    NotificationHelper.ShowMessage(Utilities.GetResourceValue("NothingUpdated"), Utilities.GetResourceValue("CaptionWarning"));
                }
            }
            catch (Exception exception)
            {
                NotificationHelper.ShowMessage(exception.Message, Utilities.GetResourceValue("CaptionError"));
            }
        }

        /// <summary>
        /// Determines whether this instance can update.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can update; otherwise, <c>false</c>.
        /// </returns>
        private bool CanUpdate()
        {
            return IsUpdateLogConfiguration || IsUpdateSchedulerConfiguration || IsUpdateEntityConfiguration;
        }
    }

    #endregion Commands
}