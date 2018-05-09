using System;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.ViewModels.Helpers
{
    public class SchedulersSearchModel : BaseModel
    {
        public SchedulersSearchModel()
        {
            SchedulerCreatedFrom = new DateModel { Date = DateTime.Today };
            SchedulerCreatedTo = new DateModel { Date = DateTime.Today };
            SchedulerStartFrom = new DateModel { Date = DateTime.Today };
            SchedulerStartTo = new DateModel { Date = DateTime.Today };
            SchedulerEndFrom = new DateModel { Date = DateTime.Today };
            SchedulerEndTo = new DateModel { Date = DateTime.Today };
        }

        private int entityId;

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public int EntityId
        {
            get { return entityId; }
            set { SetProperty(ref entityId, value); }
        }

        private Comparison countComparision;

        /// <summary>
        /// Gets or sets the count comparison.
        /// </summary>
        /// <value>
        /// The count comparison.
        /// </value>
        public Comparison CountComparison
        {
            get { return countComparision; }
            set { SetProperty(ref countComparision, value); }
        }

        private int recordCount;

        /// <summary>
        /// Gets or sets the record count.
        /// </summary>
        /// <value>
        /// The record count.
        /// </value>
        public int RecordCount
        {
            get { return recordCount; }
            set { SetProperty(ref recordCount, value); }
        }

        private DateModel schedulerCreatedFrom;

        /// <summary>
        /// Gets or sets the scheduler created from.
        /// </summary>
        /// <value>
        /// The scheduler created from.
        /// </value>
        public DateModel SchedulerCreatedFrom
        {
            get { return schedulerCreatedFrom; }
            set { SetProperty(ref schedulerCreatedFrom, value); }
        }

        private DateModel schedulerCreatedTo;

        /// <summary>
        /// Gets or sets the scheduler created to.
        /// </summary>
        /// <value>
        /// The scheduler created to.
        /// </value>
        public DateModel SchedulerCreatedTo
        {
            get { return schedulerCreatedTo; }
            set { SetProperty(ref schedulerCreatedTo, value); }
        }

        private DateModel schedulerStartFrom;

        /// <summary>
        /// Gets or sets the scheduler started from.
        /// </summary>
        /// <value>
        /// The scheduler started from.
        /// </value>
        public DateModel SchedulerStartFrom
        {
            get { return schedulerStartFrom; }
            set { SetProperty(ref schedulerStartFrom, value); }
        }

        private DateModel schedulerStartTo;

        /// <summary>
        /// Gets or sets the scheduler started to.
        /// </summary>
        /// <value>
        /// The scheduler started to.
        /// </value>
        public DateModel SchedulerStartTo
        {
            get { return schedulerStartTo; }
            set { SetProperty(ref schedulerStartTo, value); }
        }

        private DateModel schedulerEndFrom;

        /// <summary>
        /// Gets or sets the scheduler End from.
        /// </summary>
        /// <value>
        /// The scheduler End from.
        /// </value>
        public DateModel SchedulerEndFrom
        {
            get { return schedulerEndFrom; }
            set { SetProperty(ref schedulerEndFrom, value); }
        }

        private DateModel schedulerEndTo;

        /// <summary>
        /// Gets or sets the scheduler End to.
        /// </summary>
        /// <value>
        /// The scheduler End to.
        /// </value>
        public DateModel SchedulerEndTo
        {
            get { return schedulerEndTo; }
            set { SetProperty(ref schedulerEndTo, value); }
        }
    }
}