/**
* I95Dev.com
*
* NOTICE OF LICENSE
*
* This source file is subject to the EULA
* that is bundled with this package in the file LICENSE.txt.
* It is also available through the world-wide-web at this URL:
* http://store.i95dev.com/LICENSE-M1.txt
* If you did not receive a copy of the license and are unable to
* obtain it through the world-wide-web, please send an email
* to sub@i95dev.com so we can send you a copy immediately.
*
*
* @category       GP
* @package        I95DevGPConnect
* @Description
* @author         I95Dev
* @copyright      Copyright (c) 2014 I95Dev
* @license        http://store.i95dev.com/LICENSE-M1.txt
*/

using System;
using System.Net;
using System.Runtime.InteropServices;
using I95Dev.Connector.Base.Common;
using I95Dev.Connector.Base.Helpers;
using I95Dev.Connector.Base.Services;
using I95Dev.Connector.Base.Services.EventLogger;
using I95Dev.Connector.Base.Services.Inbound;
using I95Dev.Connector.Base.Services.Outbound;
using I95Dev.Connector.UI.Base.Helpers.Mvvm;
using I95Dev.Connector.UI.Base.ViewModels.Schedulers;
using log4net;

namespace I95Dev.Connector.UI.Base.Services
{
    public static class ManageSchedulers
    {
        private static class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            /// <summary>
            /// Hides the service execution in users list of processes
            /// </summary>
            /// <param name="hide"></param>
            /// <param name="windowTitle"></param>
            public static void HideWindow(bool hide, string windowTitle)
            {
                try
                {
                    //Sometimes System.Windows.Forms.Application.ExecutablePath works for the caption depending on the system you are running under.
                    IntPtr hWnd = FindWindow(null, windowTitle); //put your console window caption here
                    if (hide)
                    {
                        if (hWnd != IntPtr.Zero)
                        {  //Hide the window
                            ShowWindow(hWnd, 0); // 0 = SW_HIDE
                        }
                    }
                    else
                    {
                        if (hWnd != IntPtr.Zero)
                        {  //Show window again
                            ShowWindow(hWnd, 1); //1 = SW_SHOWNORMA
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogMessage(exception.Message, "HideWindow", LogType.Error);
                }
            }
        }

        /// <summary>
        /// This method is used to start the scheduler based on given argument number
        /// </summary>
        /// <param name="argument"></param>
        public static void StartScheduler(string[] argument)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            NativeMethods.HideWindow(true, argument[0]);
            if (!int.TryParse(argument[0], out int result)) return;

            SetLogFileName(result);
            DateTime startTime = DateTime.Now;
            Logger.LogMessage(string.Format(Constants.DefaultCulture, "SchedulerStarted \n {0:s}", DateTime.Now), "StartScheduler", LogType.Debug);
            switch (result)
            {
                case 1:
                    new MessageReceiveService().ReceiveRecords();
                    break;

                case 2:
                    new MessageSendService().GetIds();
                    new SchedulersViewModel(SchedulerType.PushData).RunScheduler();
                    break;

                case 3:
                    new MessageSendService().SyncRecords();
                    break;

                case 4:
                    new ResponseReceiveService().PullResponse();
                    break;

                case 5:// Cleanup log files and message  queue database
                    CleanUpService.CleanUpData();
                    break;

                case 6: // Creating Database
                    new InitializeService().InitializeModule(true);
                    break;

                case 7:
                    new SchedulersViewModel().CreateAllSchedulers(DialogLocator.MainAssemblyPath + ".exe");
                    break;

                case 8:
                    InitializeService.PrintScript();
                    break;

                case 9:
                    if (argument.Length < 2)
                    {
                        string filePath = CategoryService.ExportCategories(null);
                        Console.WriteLine(string.Format(Constants.DefaultCulture, @"File exported to {0}", filePath));
                    }
                    else
                    {
                        string filePath = CategoryService.ExportCategories(argument[1]);
                        Console.WriteLine(string.Format(Constants.DefaultCulture, @"File exported to {0}", filePath));
                    }
                    break;

                case 10:
                    if (argument.Length < 2)
                    {
                        bool status = CategoryService.ImportCategories("");
                        Console.WriteLine(string.Format(Constants.DefaultCulture, @"File import status : {0}", status));
                    }
                    else
                    {
                        bool status = CategoryService.ImportCategories(argument[1]);
                        Console.WriteLine(string.Format(Constants.DefaultCulture, @"File import status : {0}", status));
                    }
                    break;

                case 11:
                    new ImportService().ImportData();
                    break;

                case 12:
                    new ImportService().ExportData();
                    break;

                default:
                    Logger.LogMessage("Invalid command Argument" + argument, "StartScheduler", LogType.Warning);
                    break;
            }
            Logger.LogMessage(string.Format(Constants.DefaultCulture, "SchedulerCompleted \n {0:s} \n RunTime : {1:dd\\-hh\\:mm\\:ss}", DateTime.Now, DateTime.Now.Subtract(startTime)), "StartScheduler", LogType.Debug);
        }

        /// <summary>
        /// Sets the name of the logger file.
        /// </summary>
        /// <param name="schedulerType">Type of the scheduler.</param>
        internal static void SetLogFileName(int schedulerType)
        {
            string fileName = Logger.GeneralFileName;

            switch (schedulerType)
            {
                case 1:
                    Constants.CurrentScheduler = SchedulerType.PullData;
                    fileName = Logger.ReceiveFileName;
                    break;

                case 2:
                    Constants.CurrentScheduler = SchedulerType.GetIds;
                    fileName = Logger.GetIdsFileName;
                    break;

                case 3:
                    Constants.CurrentScheduler = SchedulerType.PushData;
                    fileName = Logger.SendFileName;
                    break;

                case 4:
                    Constants.CurrentScheduler = SchedulerType.PullResponse;
                    fileName = Logger.ResponseFileName;
                    break;

                case 5:
                    Constants.CurrentScheduler = SchedulerType.CleanUp;
                    fileName = Logger.CleanUpFileName;
                    break;

                case 0:
                    Constants.CurrentScheduler = SchedulerType.Ui;
                    fileName = Logger.UiFileName;
                    break;
            }
            GlobalContext.Properties["LoggerName"] = fileName;
        }
    }
}