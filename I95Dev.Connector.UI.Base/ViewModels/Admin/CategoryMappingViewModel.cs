using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.Models.EventLogger;
using I95Dev.Connector.Base.Services.EventLogger;
using I95Dev.Connector.UI.Base.DataAccess;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;

namespace I95Dev.Connector.UI.Base.ViewModels.Admin
{
    public class CategoryMappingViewModel : BaseViewModel
    {
        private readonly IReportService reportService;

        #region Properties

        #region BindData

        /// <summary>
        /// Gets the categories filter.
        /// </summary>
        /// <value>
        /// The categories filter.
        /// </value>
        public IList<IntegerComboBoxModel> CategoriesFilter { get; private set; }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public IList<IntegerComboBoxModel> Categories { get; private set; }

        /// <summary>
        /// Gets the exception list.
        /// </summary>
        /// <value>
        /// The exception list.
        /// </value>
        public IList<ExceptionModel> ExceptionList { get; private set; }

        /// <summary>
        /// Gets the category mappings.
        /// </summary>
        /// <value>
        /// The category mappings.
        /// </value>
        public ObservableRangeCollection<CategoryMapping> CategoryMappings { get; }

        #endregion BindData

        private string exceptionName;

        /// <summary>
        /// Gets or sets the name of the exception.
        /// </summary>
        /// <value>
        /// The name of the exception.
        /// </value>
        public string ExceptionName
        {
            get
            {
                return exceptionName;
            }
            set
            {
                exceptionName = value;
                OnPropertyChanged("ExceptionName");
            }
        }

        /// <summary>
        /// Gets or sets the filter category.
        /// </summary>
        /// <value>
        /// The filter category.
        /// </value>
        public int FilterCategory { get; set; }

        private CategoryMappingModel selectedCategory;

        /// <summary>
        /// Gets or sets the selected category.
        /// </summary>
        /// <value>
        /// The selected category.
        /// </value>
        public CategoryMappingModel SelectedCategory
        {
            get
            {
                return selectedCategory;
            }
            set
            {
                selectedCategory = value;
                if (selectedCategory != null)
                {
                    ShowSave = selectedCategory.Id <= 0;
                    ShowUpdate = !ShowSave;
                }
                OnPropertyChanged("SelectedCategory");
            }
        }

        private bool showPopup;

        /// <summary>
        /// Gets or sets a value indicating whether [show pop-up].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show pop-up]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowPopup
        {
            get
            {
                return showPopup;
            }
            set
            {
                showPopup = value;
                ValidationMessage = null;
                OnPropertyChanged("ShowPopup");
                StatusUpdate.IsPopUpOpen = value;
            }
        }

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

        private string validationMessage;

        /// <summary>
        /// Gets or sets the validation message.
        /// </summary>
        /// <value>
        /// The validation message.
        /// </value>
        public string ValidationMessage
        {
            get
            {
                return validationMessage;
            }
            private set
            {
                validationMessage = value;
                OnPropertyChanged("ValidationMessage");
            }
        }

        #endregion Properties

        public CategoryMappingViewModel()
        {
            reportService = new ReportService();
            SelectedCategory = new CategoryMappingModel();
            CategoryMappings = new ObservableRangeCollection<CategoryMapping>();
            BindData();

            ResetCommand = new BaseCommand(ResetFields);
            FindCommand = new BaseCommand(FindData);
            AddNewCommand = new BaseCommand(AddNewRecord);
            ClosePopupCommand = new BaseCommand(ClosePopup);
            SaveCommand = new BaseCommand(CanSave, SaveData);
            RowSelectedCommand = new BaseCommand(RowSelected);
            UpdateCommand = new BaseCommand(CanUpdate, UpdateData);
        }

        #region Commands

        /// <summary>
        /// Gets the find command.
        /// </summary>
        /// <value>
        /// The find command.
        /// </value>
        public ICommand FindCommand { get; }

        /// <summary>
        /// Gets the reset command.
        /// </summary>
        /// <value>
        /// The reset command.
        /// </value>
        public ICommand ResetCommand { get; }

        /// <summary>
        /// Gets the add new command.
        /// </summary>
        /// <value>
        /// The add new command.
        /// </value>
        public ICommand AddNewCommand { get; }

        /// <summary>
        /// Gets the close pop-up command.
        /// </summary>
        /// <value>
        /// The close pop-up command.
        /// </value>
        public ICommand ClosePopupCommand { get; }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        /// <value>
        /// The save command.
        /// </value>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Gets the row selected command.
        /// </summary>
        /// <value>
        /// The row selected command.
        /// </value>
        public ICommand RowSelectedCommand { get; }

        /// <summary>
        /// Gets the update command.
        /// </summary>
        /// <value>
        /// The update command.
        /// </value>
        public ICommand UpdateCommand { get; }

        /// <summary>
        /// Resets the fields.
        /// </summary>
        private void ResetFields()
        {
            FilterCategory = 0;
            ExceptionName = null;
            BindMappings();
        }

        /// <summary>
        /// Finds the data.
        /// </summary>
        private void FindData()
        {
            BindMappings();
        }

        /// <summary>
        /// Determines whether this instance can save.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can save; otherwise, <c>false</c>.
        /// </returns>
        private bool CanSave()
        {
            return SelectedCategory.Id <= 0;
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        private void SaveData()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            try
            {
                if (IsDataValid())
                {
                    int result = CategoryService.AddCategoryMapping(SelectedCategory.EventCategory, SelectedCategory.ExceptionType);
                    if (result > 0)
                    {
                        ShowPopup = false;
                        NotificationHelper.ShowMessage(Utilities.GetResourceValue("DataUpdateSuccess"), Utilities.GetResourceValue("CaptionInfo"));
                        BindMappings();
                    }
                    else
                    {
                        NotificationHelper.ShowMessage(Utilities.GetResourceValue("NothingUpdated"), Utilities.GetResourceValue("CaptionError"));
                    }
                }
            }
            catch (Exception exception)
            {
                ShowPopup = false;
                NotificationHelper.ShowMessage(exception.Message, Utilities.GetResourceValue("CaptionError"));
                ShowPopup = true;
            }
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        /// <summary>
        /// Rows the selected.
        /// </summary>
        /// <param name="selectedRow">The selected row.</param>
        private void RowSelected(object selectedRow)
        {
            CategoryMapping selectedCategoryRow = selectedRow as CategoryMapping;
            if (selectedCategoryRow == null) return;
            SelectedCategory = new CategoryMappingModel
            {
                EventCategory = selectedCategoryRow.EventCategoryId,
                ExceptionType = selectedCategoryRow.ExceptionType,
                Id = selectedCategoryRow.Id
            };
            ShowPopup = true;
        }

        /// <summary>
        /// Determines whether this instance can update.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can update; otherwise, <c>false</c>.
        /// </returns>
        private bool CanUpdate()
        {
            return SelectedCategory.Id > 0;
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        private void UpdateData()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            try
            {
                if (IsDataValid())
                {
                    var categoryMapping = new CategoryMapping
                    {
                        Id = SelectedCategory.Id,
                        EventCategoryId = SelectedCategory.EventCategory,
                        ExceptionType = SelectedCategory.ExceptionType,
                        ModifiedTime = Constants.CurrentTime
                    };
                    int result = CategoryService.UpdateCategoryMapping(categoryMapping);
                    if (result > 0)
                    {
                        ShowPopup = false;
                        NotificationHelper.ShowMessage(Utilities.GetResourceValue("DataUpdateSuccess"), Utilities.GetResourceValue("CaptionInfo"));
                        BindMappings();
                    }
                    else
                    {
                        NotificationHelper.ShowMessage(Utilities.GetResourceValue("NothingUpdated"), Utilities.GetResourceValue("CaptionError"));
                    }
                }
            }
            catch (Exception exception)
            {
                ShowPopup = false;
                NotificationHelper.ShowMessage(exception.Message, Utilities.GetResourceValue("CaptionError"));
                ShowPopup = true;
            }
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        /// <summary>
        /// Closes the pop-up.
        /// </summary>
        private void ClosePopup()
        {
            ShowPopup = false;
        }

        /// <summary>
        /// Adds the new record.
        /// </summary>
        private void AddNewRecord()
        {
            SelectedCategory = new CategoryMappingModel();
            ShowPopup = true;
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            Categories = reportService.GetCategories();

            CategoriesFilter = new List<IntegerComboBoxModel>(Categories);
            //CategoriesFilter.AddRange(Categories);
            CategoriesFilter.Insert(0, new IntegerComboBoxModel { Name = "All", Value = 0 });
            BindMappings();
            BindSuggestions();
        }

        /// <summary>
        /// Binds the mappings.
        /// </summary>
        private void BindMappings()
        {
            IList<CategoryMapping> categoryMappings = CategoryService.GetCategoryMappings(FilterCategory, ExceptionName).ToList();

            CategoryMappings.Clear();
            CategoryMappings.AddRange(categoryMappings);
        }

        /// <summary>
        /// Binds the suggestions.
        /// </summary>
        private void BindSuggestions()
        {
            ExceptionList = ExceptionListService.LoadExceptions();
        }

        /// <summary>
        /// Determines whether [is data valid].
        /// </summary>
        /// <returns></returns>
        private bool IsDataValid()
        {
            ValidationMessage = Utilities.GetResourceValue("AllFieldsMandatory");
            if (SelectedCategory.EventCategory <= 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(SelectedCategory.ExceptionType))
            {
                return false;
            }
            ValidationMessage = null;
            return true;
        }

        #endregion Methods
    }
}