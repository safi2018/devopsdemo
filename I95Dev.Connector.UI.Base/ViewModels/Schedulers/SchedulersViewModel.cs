using System;
using System.Collections.Generic;
using System.Linq;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.UI.Base.ViewModels.Base;
using Microsoft.Win32.TaskScheduler;

namespace I95Dev.Connector.UI.Base.ViewModels.Schedulers
{
    public class SchedulersViewModel : BaseViewModel
    {
        private readonly SchedulerType currentSchedulerId;
        public SchedulerViewModel PullDataScheduler { get; set; }

        public SchedulerViewModel PullResponseScheduler { get; set; }

        public SchedulerViewModel GetIdsScheduler { get; set; }

        public SchedulerViewModel PushDataScheduler { get; set; }

        public SchedulerViewModel CleanupScheduler { get; set; }

        public SchedulersViewModel(string path)
        {
            if (!string.IsNullOrEmpty(path)) Constants.BaseDirectory = path;
            CreateSchedulerObject(SchedulerType.PullData);
            CreateSchedulerObject(SchedulerType.GetIds);
            CreateSchedulerObject(SchedulerType.PushData);
            CreateSchedulerObject(SchedulerType.PullResponse);
            CreateSchedulerObject(SchedulerType.CleanUp);
        }

        public SchedulersViewModel() : this(string.Empty)
        {
        }

        public SchedulersViewModel(SchedulerType schedulerId)
        {
            currentSchedulerId = schedulerId;
            CreateSchedulerObject(schedulerId);
        }

        /// <summary>
        /// Creates the scheduler object.
        /// </summary>
        /// <param name="schedulerId">The scheduler identifier.</param>
        private void CreateSchedulerObject(SchedulerType schedulerId)
        {
            switch (schedulerId)
            {
                case SchedulerType.PullData:
                    PullDataScheduler = new SchedulerViewModel
                        (1,
                        "PullData",
                        "This scheduler is responsible to pull the data from cloud/ecommerce",
                        TimeSpan.FromMinutes(1));
                    break;

                case SchedulerType.GetIds:
                    GetIdsScheduler = new SchedulerViewModel
                        (2,
                        "GetIds",
                        "This scheduler is responsible to get the modified ids from ERP to connector queue",
                        TimeSpan.FromMinutes(1));
                    break;

                case SchedulerType.PushData:
                    PushDataScheduler = new SchedulerViewModel
                        (3,
                        "PushData",
                        "This scheduler is responsible to push the data from Connector queue to cloud/ecommerce",
                        TimeSpan.FromMinutes(1));
                    break;

                case SchedulerType.PullResponse:
                    PullResponseScheduler = new SchedulerViewModel
                       (4,
                        "PullResponse",
                        "This scheduler is responsible to pull the responses from cloud/ecommerce",
                        TimeSpan.FromMinutes(1));
                    break;

                case SchedulerType.CleanUp:
                    CleanupScheduler = new SchedulerViewModel
                        (5,
                        "Cleanup",
                        "This scheduler is responsible to clean up the data in the connector database and log files",
                        new TimeSpan(1, 0, 0, 0));
                    break;
            }
        }

        /// <summary>
        /// Runs the scheduler.
        /// </summary>
        public void RunScheduler()
        {
            switch (currentSchedulerId)
            {
                case SchedulerType.PullData:
                    PullDataScheduler.IsRunning = true;
                    break;

                case SchedulerType.GetIds:
                    GetIdsScheduler.IsRunning = true;
                    break;

                case SchedulerType.PushData:
                    PushDataScheduler.IsRunning = true;
                    break;

                case SchedulerType.PullResponse:
                    PullResponseScheduler.IsRunning = true;
                    break;

                case SchedulerType.CleanUp:
                    CleanupScheduler.IsRunning = true;
                    break;
            }
        }

        /// <summary>
        /// Creates all schedulers.
        /// </summary>
        public void CreateAllSchedulers(string exeName)
        {
            PullDataScheduler.IsUiProcess = PullResponseScheduler.IsUiProcess = false;

            GetIdsScheduler.IsUiProcess = PushDataScheduler.IsUiProcess = CleanupScheduler.IsUiProcess = false;

            if (PullDataScheduler.CreateCommand.CanExecute(null)) PullDataScheduler.CreateCommand.Execute(exeName);
            if (GetIdsScheduler.CreateCommand.CanExecute(null)) GetIdsScheduler.CreateCommand.Execute(exeName);
            if (PushDataScheduler.CreateCommand.CanExecute(null)) PushDataScheduler.CreateCommand.Execute(exeName);
            if (PullResponseScheduler.CreateCommand.CanExecute(null)) PullResponseScheduler.CreateCommand.Execute(exeName);
            if (CleanupScheduler.CreateCommand.CanExecute(null)) CleanupScheduler.CreateCommand.Execute(exeName);
        }

        /// <summary>
        /// Deletes all schedulers.
        /// </summary>
        public void DeleteAllSchedulers()
        {
            try
            {
                var taskScheduler = new TaskService();
                TaskFolder taskFolder = taskScheduler.GetFolder("\\" + PullDataScheduler.SchedulerGroupName);
                IEnumerable<TaskFolder> taskFolders = GetTaskFolder(taskScheduler, PullDataScheduler.SchedulerGroupName);

                if (taskFolders.Any())
                {
                    TaskCollection tasks = taskFolder.GetTasks();
                    if (tasks != null)
                    {
                        foreach (Task task in tasks)
                        {
                            if (task.State == TaskState.Running)
                                task.Stop();
                            taskFolder.DeleteTask(task.Name, false);
                        }
                    }
                }
                taskFolder = taskScheduler.GetFolder("\\");
                taskFolder.DeleteFolder(PullDataScheduler.SchedulerGroupName, false);
            }
            catch (Exception exception)
            {
                Logger.LogMessage(exception.Message, "DeleteSchedulers", LogType.Error, exception);
                //NotificationHelper.ShowMessage(exception.Message, ResourceHelper.GetResourceValue("CaptionError"));
            }
        }

        /// <summary>
        /// Gets the task folder.
        /// </summary>
        /// <param name="taskScheduler">The task scheduler.</param>
        /// <param name="schedulerGroupName">Name of the scheduler group.</param>
        /// <returns></returns>
        private static IEnumerable<TaskFolder> GetTaskFolder(TaskService taskScheduler, string schedulerGroupName)
        {
            TaskFolder taskFolder = taskScheduler.GetFolder("\\");
            TaskFolderCollection taskGroupCollection = taskFolder.SubFolders;
            List<TaskFolder> taskFolderCollection = taskGroupCollection.ToList();

            IEnumerable<TaskFolder> taskFolders = from c in taskFolderCollection where c.Name == schedulerGroupName select c;
            return taskFolders;
        }
    }
}