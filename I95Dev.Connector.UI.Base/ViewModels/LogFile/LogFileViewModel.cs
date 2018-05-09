using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.Services;
using I95Dev.Connector.UI.Base.ViewModels.Base;

namespace I95Dev.Connector.UI.Base.ViewModels.LogFile
{
    public class LogFileViewModel : BaseListViewModel<LogFilesModel>
    {
        #region Properties

        private string fileName;

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get
            {
                return fileName;
            }
            set { SetProperty(ref fileName, value); }
        }

        private decimal? fileSize;

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        /// <value>
        /// The size of the file.
        /// </value>
        public decimal? FileSize
        {
            get
            {
                return fileSize;
            }
            set { SetProperty(ref fileSize, value); }
        }

        private bool createdFromChecked;

        /// <summary>
        /// Gets or sets a value indicating whether [created date checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [created date checked]; otherwise, <c>false</c>.
        /// </value>
        public bool CreatedFromChecked
        {
            get
            {
                return createdFromChecked;
            }
            set { SetProperty(ref createdFromChecked, value); }
        }

        private DateTime createdFrom;

        /// <summary>
        /// Gets or sets the created time.
        /// </summary>
        /// <value>
        /// The created time.
        /// </value>
        public DateTime CreatedFrom
        {
            get
            {
                return createdFrom;
            }
            set { SetProperty(ref createdFrom, value); }
        }

        private bool createdToChecked;

        /// <summary>
        /// Gets or sets a value indicating whether [created date checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [created date checked]; otherwise, <c>false</c>.
        /// </value>
        public bool CreatedToChecked
        {
            get
            {
                return createdToChecked;
            }
            set { SetProperty(ref createdToChecked, value); }
        }

        private DateTime createdTo;

        /// <summary>
        /// Gets or sets the created time.
        /// </summary>
        /// <value>
        /// The created time.
        /// </value>
        public DateTime CreatedTo
        {
            get
            {
                return createdTo;
            }
            set { SetProperty(ref createdTo, value); }
        }

        private DateTime updatedFrom;

        /// <summary>
        /// Gets or sets the updated time.
        /// </summary>
        /// <value>
        /// The updated time.
        /// </value>
        public DateTime UpdatedFrom
        {
            get
            {
                return updatedFrom;
            }
            set { SetProperty(ref updatedFrom, value); }
        }

        private bool updatedFromChecked;

        /// <summary>
        /// Gets or sets a value indicating whether [modified date checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [modified date checked]; otherwise, <c>false</c>.
        /// </value>
        public bool UpdateFromChecked
        {
            get
            {
                return updatedFromChecked;
            }

            set { SetProperty(ref updatedFromChecked, value); }
        }

        private DateTime updatedTo;

        /// <summary>
        /// Gets or sets the updated time.
        /// </summary>
        /// <value>
        /// The updated time.
        /// </value>
        public DateTime UpdatedTo
        {
            get
            {
                return updatedTo;
            }
            set { SetProperty(ref updatedTo, value); }
        }

        private bool updatedToChecked;

        /// <summary>
        /// Gets or sets a value indicating whether [modified date checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [modified date checked]; otherwise, <c>false</c>.
        /// </value>
        public bool UpdateToChecked
        {
            get
            {
                return updatedToChecked;
            }
            set { SetProperty(ref updatedToChecked, value); }
        }

        private Comparison comparisonType;

        /// <summary>
        /// Gets or sets the type of the comparison.
        /// </summary>
        /// <value>
        /// The type of the comparison.
        /// </value>
        public Comparison ComparisonType
        {
            get
            {
                return comparisonType;
            }
            set { SetProperty(ref comparisonType, value); }
        }

        #endregion Properties

        public LogFileViewModel()
        {
            LoadDefaultFilters();
            LoadLogFiles();
            ViewFileCommand = new BaseCommand(ViewFile);
            ResetCommand = new BaseCommand(ResetFields);
            FindCommand = new BaseCommand(LoadLogFiles);
        }

        #region Commands

        /// <summary>
        /// Gets the view file command.
        /// </summary>
        /// <value>
        /// The view file command.
        /// </value>
        public ICommand ViewFileCommand { get; private set; }

        /// <summary>
        /// Gets the reset command.
        /// </summary>
        /// <value>
        /// The reset command.
        /// </value>
        public ICommand ResetCommand { get; private set; }

        /// <summary>
        /// Gets the find command.
        /// </summary>
        /// <value>
        /// The find command.
        /// </value>
        public ICommand FindCommand { get; private set; }

        /// <summary>
        /// Resets the fields.
        /// </summary>
        private void ResetFields()
        {
            FileName = null;
            FileSize = null;
            CreatedFromChecked = false;
            UpdateFromChecked = false;
            CreatedToChecked = false;
            UpdateToChecked = false;
            LoadDefaultFilters();
            LoadLogFiles();
        }

        /// <summary>
        /// Views the file.
        /// </summary>
        /// <param name="arg">The argument.</param>
        private void ViewFile(object arg)
        {
            string filePath = arg as string;
            if (!string.IsNullOrEmpty(filePath))
            {
                OpenFile(filePath);
            }
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Loads the default filters.
        /// </summary>
        private void LoadDefaultFilters()
        {
            ComparisonType = Comparison.GreaterThan;
            CreatedFrom = UpdatedFrom = DateTime.Today.Date;
            createdTo = updatedTo = DateTime.Today.Date.AddDays(1);
        }

        /// <summary>
        /// Loads the log files.
        /// </summary>
        private void LoadLogFiles()
        {
            StatusUpdate.StatusMessage = Utilities.GetResourceValue("Loading");
            try
            {
                var directoryInfo = new DirectoryInfo(Constants.LogFilesDirectory);
                if (directoryInfo.Exists)
                {
                    IEnumerable<FileInfo> query = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).Where(s => s.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) || s.Name.ToLower(CultureInfo.CurrentCulture).Contains(".xml."));

                    if (!string.IsNullOrEmpty(FileName))
                    {
                        query = query.Where(c1 => c1.Name.ToLower(Constants.DefaultCulture).Contains(FileName.ToLower(Constants.DefaultCulture)));
                    }

                    if (CreatedFromChecked)
                    {
                        query = query.Where(c1 => c1.CreationTime >= CreatedFrom);
                    }

                    if (CreatedToChecked)
                    {
                        query = query.Where(c1 => c1.CreationTime <= CreatedTo);
                    }

                    if (UpdateFromChecked)
                    {
                        query = query.Where(c1 => c1.LastWriteTime >= UpdatedFrom);
                    }

                    if (UpdateToChecked)
                    {
                        query = query.Where(c1 => c1.LastWriteTime <= UpdatedTo);
                    }

                    if (FileSize.HasValue)
                    {
                        switch (ComparisonType)
                        {
                            case Comparison.Equal:
                                query = query.Where(c1 => Math.Round(c1.Length / 1024.00, 2) == (double)FileSize);
                                break;

                            case Comparison.GreaterThan:
                                query = query.Where(c1 => Math.Round(c1.Length / 1024.00, 2) > (double)FileSize);
                                break;

                            case Comparison.GreaterThanOrEqualTo:
                                query = query.Where(c1 => Math.Round(c1.Length / 1024.00, 2) >= (double)FileSize);
                                break;

                            case Comparison.LessThan:
                                query = query.Where(c1 => Math.Round(c1.Length / 1024.00, 2) < (double)FileSize);
                                break;

                            case Comparison.LessThanOrEqualTo:
                                query = query.Where(c1 => Math.Round(c1.Length / 1024.00, 2) <= (double)FileSize);
                                break;
                        }
                    }

                    int i = 1;

                    LogFilesModel[] filesList = (from file in query.OrderByDescending(c1 => c1.LastWriteTime).OrderBy(c1 => c1.Extension)
                                                 select new LogFilesModel
                                                 {
                                                     Index = i++,
                                                     FileName = file.Name,
                                                     FilePath = file.FullName,
                                                     LastUpdatedTime = file.LastWriteTime,
                                                     CreatedTime = file.CreationTime,
                                                     FileSize = Math.Round(file.Length / 1024.00, 2),
                                                     FileType = file.Extension
                                                 }).ToArray();

                    SetRecordList(filesList);
                }
            }
            catch (Exception exception)
            {
                NotificationHelper.ShowMessage(exception.Message, Utilities.GetResourceValue("CaptionError"));
            }

            StatusUpdate.StatusMessage = Utilities.GetResourceValue("DefaultStatus");
        }

        /// <summary>
        /// Opens the file in log viewer.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private static void OpenFile(string filePath)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = Constants.BaseDirectory + @"lib\I95DevLogViewer.exe";
                process.StartInfo.Arguments = string.Format(CultureInfo.CurrentCulture, "\"{0}\"", Convert.ToString(filePath, CultureInfo.CurrentCulture));
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.Start();
            }
        }

        #endregion Methods
    }
}