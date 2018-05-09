using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;
using System.Windows.Input;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers;
using I95Dev.Connector.UI.Base.Helpers.Commands;
using I95Dev.Connector.UI.Base.Helpers.Controls;
using I95Dev.Connector.UI.Base.Helpers.Mvvm;
using I95Dev.Connector.UI.Base.Models;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using I95Dev.Connector.UI.Base.ViewModels.Share;
using Microsoft.Win32.TaskScheduler;
using MvvmDialogs;
using Utilities = I95Dev.Connector.UI.Base.Helpers.Utilities;

namespace I95Dev.Connector.UI.Base.ViewModels.Schedulers
{
    public class SchedulerViewModel : BaseViewModel
    {
        #region Private Properties

        private readonly bool initialLoading;
        private readonly IDialogService dialogService;
        private const string CompanyName = "i95DevGPConnect";
        internal readonly string SchedulerGroupName;
        private static string currentUserPassword = "";

        #endregion Private Properties

        #region Properties

        private int schedulerId;

        public int SchedulerId
        {
            get { return schedulerId; }
            private set
            {
                SetProperty(ref schedulerId, value);
            }
        }

        public string SchedulerName { get; set; }

        public string Description { get; set; }
        public TimeSpan IntervalTime { get; private set; }

        private TaskState status;

        public TaskState Status
        {
            get { return status; }
            private set
            {
                SetProperty(ref status, value);
                OnPropertyChanged("IsCreated");
            }
        }

        private DateTime? lastRuntime;

        public DateTime? LastRuntime
        {
            get { return lastRuntime; }
            private set
            {
                SetProperty(ref lastRuntime, value);
            }
        }

        private DateTime? nextRuntime;

        public DateTime? NextRuntime
        {
            get { return nextRuntime; }
            private set
            {
                SetProperty(ref nextRuntime, value);
            }
        }

        private bool isEnabled;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (!(SetProperty(ref isEnabled, value) & !initialLoading)) return;
                if (value)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
                OnPropertyChanged("CanRun");
                OnPropertyChanged("IsRunning");
            }
        }

        private bool isRunning;

        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                if (!(SetProperty(ref isRunning, value) & !initialLoading)) return;
                if (value)
                {
                    RunScheduler();
                }
                else
                {
                    Stop();
                }
            }
        }

        public bool IsCreated { get { return Status != TaskState.Unknown; } }

        public bool CanRun { get { return Status != TaskState.Unknown & Status != TaskState.Disabled; } }

        internal bool IsUiProcess { get; set; }

        #endregion Properties

        public SchedulerViewModel(int schedulerId, string name, string description, TimeSpan timeSpan)
        {
            initialLoading = true;
            dialogService = Utilities.CreateDialogServiceInstance();
            string instancePostfix = ConfigurationHelper.GetConfigurationValue(ConfigurationConstants.InstanceType);
            SchedulerGroupName = string.Format(CultureInfo.CurrentCulture, "{0}_{1}", CompanyName, instancePostfix);
            SchedulerId = schedulerId;
            SchedulerName = name;
            Description = description;
            IntervalTime = timeSpan;
            CreateCommand = new BaseCommand(CanCreate, Create);
            RefreshCommand = new BaseCommand(Refresh);

            using (var taskScheduler = new TaskService())
            {
                TaskFolder taskFolder = taskScheduler.GetFolder("\\");
                CheckFolder(taskFolder);
            }
            LoadSchedulerStatus();
            initialLoading = false;
        }

        #region Commands

        public ICommand CreateCommand { get; private set; }

        private bool CanCreate()
        {
            return Status == TaskState.Unknown;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        private void Create(object exeName)
        {
            var schedulerData = new SchedulerModel
            {
                Author = "i95Dev",
                Id = SchedulerName,
                Description = Description,
                TaskFolder = SchedulerGroupName,
                Arguments = schedulerId.ToString(CultureInfo.CurrentCulture),
                Path = Constants.BaseDirectory + (string.IsNullOrEmpty(DialogLocator.MainAssemblyPath) & exeName != null
                                    ? Convert.ToString(exeName, Constants.DefaultCulture) : DialogLocator.MainAssemblyPath) + @".exe",
                WorkingDirectory = Constants.BaseDirectory,
                DaysInterval = 1,
                RepetitionInterval = IntervalTime
            };
            bool result = CreateScheduler(schedulerData, true);
            if (result)
            {
                LoadSchedulerStatus();
            }
        }

        public ICommand RefreshCommand { get; private set; }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        private void Refresh()
        {
            LoadSchedulerStatus();
        }

        #endregion Commands

        #region Methods

        private void NotifyChanges()
        {
            OnPropertyChanged("IsCreated");
            OnPropertyChanged("IsEnabled");
            OnPropertyChanged("CanRun");
            OnPropertyChanged("IsRunning");
        }

        /// <summary>
        /// This method is used to get the task scheduler which have the given name
        /// </summary>
        /// <returns></returns>
        private Task GetTask()
        {
            using (var taskScheduler = new TaskService())
            {
                Task task = taskScheduler.GetTask("\\" + SchedulerGroupName + "\\" + SchedulerName);
                return task;
            }
        }

        /// <summary>
        /// Creates the scheduler.
        /// </summary>
        /// <param name="schedulerData">The scheduler data.</param>
        /// <param name="showStatus">if set to <c>true</c> [status].</param>
        /// <returns></returns>
        private bool CreateScheduler(SchedulerModel schedulerData, bool showStatus)
        {
            bool result = false;
            try
            {
                using (var taskScheduler = new TaskService())
                {
                    TaskDefinition newTaskDefinition = taskScheduler.NewTask();
                    newTaskDefinition.RegistrationInfo.Author = schedulerData.Author;
                    newTaskDefinition.RegistrationInfo.Description = schedulerData.Description;
                    newTaskDefinition.Settings.Compatibility = TaskCompatibility.V2;

                    var action = (ExecAction)newTaskDefinition.Actions.AddNew(TaskActionType.Execute);
                    action.Id = schedulerData.Id;
                    action.Path = schedulerData.Path;
                    action.Arguments = schedulerData.Arguments;
                    action.WorkingDirectory = schedulerData.WorkingDirectory;
                    newTaskDefinition.Principal.UserId = Environment.UserName;
                    newTaskDefinition.Principal.LogonType = TaskLogonType.Password;

                    DailyTrigger trigger = newTaskDefinition.Triggers.AddNew(TaskTriggerType.Daily) as DailyTrigger;
                    if (trigger != null)
                    {
                        trigger.StartBoundary = DateTime.Now.Date;
                        trigger.DaysInterval = schedulerData.DaysInterval;
                        trigger.Repetition.Interval = schedulerData.RepetitionInterval;
                        trigger.Enabled = true;
                    }

                    TaskFolder taskFolder = taskScheduler.GetFolder("\\");
                    CheckFolder(taskFolder);

                    if (string.IsNullOrEmpty(currentUserPassword))
                    {
                        var viewModel = new PasswordViewModel();
                        System.Windows.Window window = null;

                        if (System.Windows.Application.Current != null)
                        {
                            window = System.Windows.Application.Current.MainWindow;
                        }
                        if (window == null)
                        {
                            CheckPassword:
                            string password = null;
                            if (InputBox.Show("Authentication", string.Format(Constants.DefaultCulture, "Enter {0} Password:", viewModel.UserName), ref password) == DialogResult.OK)
                            {
                                viewModel.Password = password;
                                viewModel.CheckCredentials(IsUiProcess);
                                if (!viewModel.DialogResult.HasValue || !viewModel.DialogResult.Value)
                                    goto CheckPassword;
                            }
                        }
                        else
                        {
                            var dataContext = window.DataContext as HomeViewModel;
                            dialogService.ShowDialog(dataContext, viewModel);
                        }
                        if (!string.IsNullOrEmpty(viewModel.Password))
                        {
                            currentUserPassword = viewModel.Password;
                        }
                        else
                        {
                            if (IsUiProcess)
                                NotificationHelper.ShowMessage(string.Format(CultureInfo.CurrentCulture, Utilities.GetResourceValue("UnableToCreateScheduler"), schedulerData.Description), Utilities.GetResourceValue("CaptionError"));
                            return false;
                        }
                    }

                    taskFolder = taskScheduler.GetFolder("\\" + schedulerData.TaskFolder);
                    string userId = string.Concat(Environment.UserDomainName, "\\", Environment.UserName);
                    taskFolder.RegisterTaskDefinition(schedulerData.Id, newTaskDefinition, TaskCreation.CreateOrUpdate, userId, currentUserPassword, TaskLogonType.InteractiveTokenOrPassword);
                    result = true;
                    if (showStatus)
                    {
                        if (IsUiProcess)
                            NotificationHelper.ShowMessage(string.Format(CultureInfo.CurrentCulture, Utilities.GetResourceValue("SchedulerCreated"), schedulerData.Id), Utilities.GetResourceValue("CaptionInfo"));
                        SetFilePermission(schedulerData.Id);
                    }
                }
            }
            catch (Exception exception)
            {
                if (IsUiProcess)
                    NotificationHelper.ShowMessage(string.Format(CultureInfo.CurrentCulture, Utilities.GetResourceValue("UnableToCreateScheduler"), schedulerData.Description, exception.Message), Utilities.GetResourceValue("CaptionError"));
                else
                    Logger.LogMessage(exception.Message, "CreateScheduler", LogType.Error, exception);
            }
            return result;
        }

        /// <summary>
        /// Sets the file permission.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void SetFilePermission(string fileName)
        {
            var directory = new DirectoryInfo(Path.Combine("C:\\Windows\\System32\\Tasks", SchedulerGroupName));
            if (!directory.Exists) return;
            string filePath = Path.Combine(directory.FullName, fileName);
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists) return;
            FileSecurity security = fileInfo.GetAccessControl();
            var allUsers = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            security.AddAccessRule(new FileSystemAccessRule(allUsers, FileSystemRights.ReadAndExecute, AccessControlType.Allow));

            var admins = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            security.AddAccessRule(new FileSystemAccessRule(admins, FileSystemRights.FullControl, AccessControlType.Allow));

            var owner = new SecurityIdentifier(WellKnownSidType.CreatorOwnerSid, null);
            security.AddAccessRule(new FileSystemAccessRule(owner, FileSystemRights.FullControl, AccessControlType.Allow));

            // Set the new access settings.
            var ntAccount = new NTAccount(Environment.UserDomainName, Environment.UserName);

            try
            {
                security.SetOwner(ntAccount);
                fileInfo.SetAccessControl(security);
            }
            catch (Exception exception)
            {
                Logger.LogMessage(exception.Message, "SetSchedulerPermission", LogType.Error, exception);
            }
        }

        /// <summary>
        /// Loads the scheduler status.
        /// </summary>
        private void LoadSchedulerStatus()
        {
            Task currentTask = GetTask();
            if (currentTask != null)
            {
                Status = currentTask.State;
                SchedulerName = currentTask.Name;
                LastRuntime = currentTask.LastRunTime;
                NextRuntime = currentTask.NextRunTime;
            }
            else
            {
                LastRuntime = DateTime.MinValue;
                NextRuntime = DateTime.MinValue;
                Status = TaskState.Unknown;
            }
            IsEnabled = Status != TaskState.Unknown & Status != TaskState.Disabled;
            isRunning = Status == TaskState.Running;
            NotifyChanges();
        }

        /// <summary>
        /// Checks the folder.
        /// </summary>
        /// <param name="taskFolder">The task folder.</param>
        private void CheckFolder(TaskFolder taskFolder)
        {
            TaskFolderCollection taskGroupCollection = taskFolder.SubFolders;
            List<TaskFolder> taskFolderCollection = taskGroupCollection.ToList();

            IEnumerable<TaskFolder> taskFolders = from c in taskFolderCollection where c.Name == SchedulerGroupName select c;
            if (taskFolders.Any()) return;
            taskFolder.CreateFolder(SchedulerGroupName, taskFolder.GetAccessControl(AccessControlSections.Access));
            var directory = new DirectoryInfo(Path.Combine("C:\\Windows\\System32\\Tasks", SchedulerGroupName));
            if (!directory.Exists) return;
            DirectorySecurity dSecurity = directory.GetAccessControl();
            var allUsers = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            dSecurity.AddAccessRule(new FileSystemAccessRule(allUsers, FileSystemRights.ListDirectory, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));

            var admins = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            dSecurity.AddAccessRule(new FileSystemAccessRule(admins, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));

            var owner = new SecurityIdentifier(WellKnownSidType.CreatorOwnerSid, null);
            dSecurity.AddAccessRule(new FileSystemAccessRule(owner, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));

            // Set the new access settings.
            var ntAccount = new NTAccount(Environment.UserDomainName, Environment.UserName);
            try
            {
                dSecurity.SetOwner(ntAccount);
                directory.SetAccessControl(dSecurity);
            }
            catch (Exception exception)
            {
                Logger.LogMessage(exception.Message, "SetSchedulerPermission", LogType.Error, exception);
            }
        }

        /// <summary>
        /// Enables this instance.
        /// </summary>
        private void Enable()
        {
            Task task = GetTask();
            if (task != null)
            {
                task.Enabled = true;
                //NotificationHelper.ShowMessage(string.Format(CultureInfo.CurrentCulture, ResourceHelper.GetResourceValue("SchedulerEnableDone"), SchedulerName), ResourceHelper.GetResourceValue("CaptionInfo"));
            }
            LoadSchedulerStatus();
        }

        /// <summary>
        /// Disables this instance.
        /// </summary>
        private void Disable()
        {
            Task task = GetTask();
            if (task != null)
            {
                task.Enabled = false;
                //NotificationHelper.ShowMessage(string.Format(CultureInfo.CurrentCulture, ResourceHelper.GetResourceValue("SchedulerDisableDone"), SchedulerName), ResourceHelper.GetResourceValue("CaptionInfo"));
            }
            LoadSchedulerStatus();
        }

        /// <summary>
        /// Runs the scheduler.
        /// </summary>
        private void RunScheduler()
        {
            Task task = GetTask();
            if (task?.State == TaskState.Ready)
            {
                task.Run();
            }
            LoadSchedulerStatus();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        private void Stop()
        {
            Task task = GetTask();
            if (task?.State == TaskState.Running)
            {
                task.Stop();
                //NotificationHelper.ShowMessage(string.Format(CultureInfo.CurrentCulture, ResourceHelper.GetResourceValue("SchedulerIsStopped"), SchedulerName), ResourceHelper.GetResourceValue("CaptionInfo"));
            }
            LoadSchedulerStatus();
        }

        #endregion Methods
    }
}