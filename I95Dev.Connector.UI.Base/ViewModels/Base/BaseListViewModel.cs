using System.Collections.Generic;
using System.Windows.Data;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Models;

namespace I95Dev.Connector.UI.Base.ViewModels.Base
{
    public abstract class BaseListViewModel<T> : BaseNavigationViewModel
    {
        #region Properties

        private int itemPerPage = 20;
        private int itemcount;
        private int currentPageIndex;
        private int totalPages = 1;

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage
        {
            get { return currentPageIndex; }
            set
            {
                if (SetProperty(ref currentPageIndex, value))
                {
                    SetPages(value);
                    ViewList.View.Refresh();
                }
                else
                {
                    OnPropertyChanged("CurrentPage");
                }
            }
        }

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        /// <value>
        /// The total pages.
        /// </value>
        public ObservableRangeCollection<IntegerComboBoxModel> PageNumbers { get; }

        /// <summary>
        /// Gets or sets the total records.
        /// </summary>
        /// <value>
        /// The total records.
        /// </value>
        public int TotalRecords
        {
            get { return itemcount; }
            set
            {
                itemcount = value;
                OnPropertyChanged("TotalRecords");
            }
        }

        /// <summary>
        /// Gets or sets the records per page.
        /// </summary>
        /// <value>
        /// The records per page.
        /// </value>
        public int RecordsPerPage
        {
            get { return itemPerPage; }
            set
            {
                if (itemPerPage == value) return;
                itemPerPage = value;
                if (CurrentPage == 1) ViewList.View.Refresh();
                else
                    CurrentPage = 1;
                SetPages(1);
                OnPropertyChanged("RecordsPerPage");
            }
        }

        /// <summary>
        /// Gets or sets the view list.
        /// </summary>
        /// <value>
        /// The view list.
        /// </value>
        public CollectionViewSource ViewList { get; set; }

        /// <summary>
        /// Gets the people list.
        /// </summary>
        /// <value>
        /// The people list.
        /// </value>
        public ObservableRangeCollection<T> RecordList
        {
            get;
        }

        #endregion Properties

        protected BaseListViewModel()
        {
            RecordList = new ObservableRangeCollection<T>();
            PageNumbers = new ObservableRangeCollection<IntegerComboBoxModel>();
            ViewList = new CollectionViewSource
            {
                Source = RecordList
            };
            ViewList.Filter += View_Filter;
            CurrentPage = 1;

            NextCommand = new BaseCommand(CanShowNextPage, ShowNextPage);
            PreviousCommand = new BaseCommand(CanShowPreviousPage, ShowPreviousPage);
            FirstCommand = new BaseCommand(CanShowFirstPage, ShowFirstPage);
            LastCommand = new BaseCommand(CanShowLastPage, ShowLastPage);
        }

        #region Events

        /// <summary>
        /// Handles the Filter event of the view control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FilterEventArgs"/> instance containing the event data.</param>
        private void View_Filter(object sender, FilterEventArgs e)
        {
            long index = ((BaseListModel)e.Item).Index;
            if (index > itemPerPage * (currentPageIndex - 1) && index <= itemPerPage * currentPageIndex)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        #endregion Events

        #region Commands

        /// <summary>
        /// Determines whether this instance [can show next page].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can show next page]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanShowNextPage()
        {
            return totalPages > CurrentPage;
        }

        /// <summary>
        /// Determines whether this instance [can show previous page].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can show previous page]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanShowPreviousPage()
        {
            return CurrentPage != 1;
        }

        /// <summary>
        /// Determines whether this instance [can show first page].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can show first page]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanShowFirstPage()
        {
            return CurrentPage != 1;
        }

        /// <summary>
        /// Determines whether this instance [can show last page].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can show last page]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanShowLastPage()
        {
            return CurrentPage != totalPages;
        }

        #endregion Commands

        #region Pagination Methods

        /// <summary>
        /// Sets the pages.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        private void SetPages(int currentPage)
        {
            TotalRecords = RecordList.Count;
            CalculateTotalPages(currentPage);
        }

        /// <summary>
        /// Calculates the total pages.
        /// </summary>
        private void CalculateTotalPages(int currentPage)
        {
            if (itemcount == 0)
            {
                totalPages = 1;
            }
            else if (itemcount % itemPerPage == 0)
            {
                totalPages = (itemcount / itemPerPage);
            }
            else
            {
                totalPages = (itemcount / itemPerPage) + 1;
            }

            PageNumbers.Clear();

            int startPage = currentPage - 5;
            if (startPage <= 0) startPage = 1;
            int page = startPage;

            addPages:
            PageNumbers.Add(new IntegerComboBoxModel { Name = page.ToString(Constants.DefaultCulture), Value = page });
            if (totalPages > page & page < (startPage + 9))
            {
                page++;
                goto addPages;
            }
            OnPropertyChanged("CurrentPage");
        }

        /// <summary>
        /// Shows the first page.
        /// </summary>
        private void ShowFirstPage()
        {
            CurrentPage = 1;
        }

        /// <summary>
        /// Shows the last page.
        /// </summary>
        private void ShowLastPage()
        {
            CurrentPage = totalPages;
        }

        /// <summary>
        /// Shows the next page.
        /// </summary>
        private void ShowNextPage()
        {
            CurrentPage++;
        }

        /// <summary>
        /// Shows the previous page.
        /// </summary>
        private void ShowPreviousPage()
        {
            CurrentPage--;
        }

        #endregion Pagination Methods

        /// <summary>
        /// Sets the record list.
        /// </summary>
        /// <param name="list">The list.</param>
        internal void SetRecordList(IList<T> list)
        {
            RecordList.SuppressNotification = true;
            RecordList.Clear();
            CurrentPage = 1;
            RecordList.AddRange(list);
            SetPages(1);
        }
    }
}