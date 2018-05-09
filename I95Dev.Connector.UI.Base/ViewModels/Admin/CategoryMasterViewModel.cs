using System;
using System.Windows.Input;
using I95Dev.Connector.Base.Models.EventLogger;
using I95Dev.Connector.Base.Services.EventLogger;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;

namespace I95Dev.Connector.UI.Base.ViewModels.Admin
{
    public class CategoryMasterViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public ObservableRangeCollection<EventCategory> Categories { get; }

        private bool showSave;

        /// <summary>
        /// Gets or sets a value indicating whether [show save].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show save]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSave
        {
            get
            {
                return showSave;
            }
            set
            {
                showSave = value;
                OnPropertyChanged("ShowSave");
            }
        }

        private bool showUpdate;

        /// <summary>
        /// Gets or sets a value indicating whether [show update].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show update]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowUpdate
        {
            get { return showUpdate; }
            set
            {
                showUpdate = value;
                OnPropertyChanged("ShowUpdate");
            }
        }

        private EventCategory category;

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public EventCategory Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;

                if (category != null)
                {
                    ShowSave = category.Id <= 0;
                    ShowUpdate = !ShowSave;
                }
                OnPropertyChanged("Category");
            }
        }

        #endregion Properties

        public CategoryMasterViewModel()
        {
            Categories = new ObservableRangeCollection<EventCategory>();
            ShowSave = true;
            BindData();
            SaveCommand = new BaseCommand(CanSave, SaveData);
            UpdateCommand = new BaseCommand(CanUpdate, UpdateData);
            ResetCommand = new BaseCommand(ResetFilters);
        }

        #region Commands

        /// <summary>
        /// Gets or sets the save command.
        /// </summary>
        /// <value>
        /// The save command.
        /// </value>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        /// Gets or sets the update command.
        /// </summary>
        /// <value>
        /// The update command.
        /// </value>
        public ICommand UpdateCommand { get; set; }

        /// <summary>
        /// Gets or sets the reset command.
        /// </summary>
        /// <value>
        /// The reset command.
        /// </value>
        public ICommand ResetCommand { get; set; }

        /// <summary>
        /// Resets the filters.
        /// </summary>
        private void ResetFilters()
        {
            BindData();
        }

        /// <summary>
        /// Determines whether this instance can save.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can save; otherwise, <c>false</c>.
        /// </returns>
        private bool CanSave()
        {
            return Category == null || Category.Id <= 0;
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        private void SaveData()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            if (IsDataValid())
            {
                Category.CreatedTime = DateTime.Now;
                Category.ModifiedTime = DateTime.Now;

                if (Category.ParentId.HasValue)
                {
                    EventCategory parentCategory = CategoryService.GetCategory(Category.ParentId.Value);
                    Category.HierarchyLevel = parentCategory.HierarchyLevel + 1;
                }

                int result = CategoryService.AddCategory(Category);
                if (result > 0)
                {
                    NotificationHelper.ShowMessage(Utilities.GetResourceValue("DataUpdateSuccess"), Utilities.GetResourceValue("CaptionInfo"));
                    BindData();
                }
                else
                {
                    NotificationHelper.ShowMessage(Utilities.GetResourceValue("NothingUpdated"), Utilities.GetResourceValue("CaptionError"));
                }
            }
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        /// <summary>
        /// Determines whether this instance can update.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can update; otherwise, <c>false</c>.
        /// </returns>
        private bool CanUpdate()
        {
            return Category != null && Category.Id > 0;
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        private void UpdateData()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            if (IsDataValid())
            {
                if (Category.ParentId.HasValue)
                {
                    EventCategory parentCategory = CategoryService.GetCategory(Category.ParentId.Value);
                    Category.HierarchyLevel = parentCategory.HierarchyLevel + 1;
                }

                int result = CategoryService.UpdateCategory(Category);
                if (result > 0)
                {
                    NotificationHelper.ShowMessage(Utilities.GetResourceValue("DataUpdateSuccess"), Utilities.GetResourceValue("CaptionInfo"));
                    BindData();
                }
                else
                {
                    NotificationHelper.ShowMessage(Utilities.GetResourceValue("NothingUpdated"), Utilities.GetResourceValue("CaptionError"));
                }
            }
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Determines whether [is data valid].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is data valid]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDataValid()
        {
            if (string.IsNullOrEmpty(Category.CategoryName))
            {
                NotificationHelper.ShowMessage(Utilities.GetResourceValue("Admin_CategoryMaster_Validation_CategoryName"), Utilities.GetResourceValue("CaptionWarning"));
                return false;
            }

            if (!Category.ParentId.HasValue) return true;
            if (Category.ParentId != Category.Id || Category.ParentId == 0) return true;
            NotificationHelper.ShowMessage(Utilities.GetResourceValue("Admin_CategoryMaster_Validation_ParentMatch"), Utilities.GetResourceValue("CaptionWarning"));
            return false;
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            Categories.Clear();
            Categories.AddRange(CategoryService.GetAllCategories());
            Category = new EventCategory();
        }

        #endregion Methods
    }
}