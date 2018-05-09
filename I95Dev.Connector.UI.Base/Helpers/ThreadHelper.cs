using System;
using System.Windows;
using System.Windows.Threading;

namespace I95Dev.Connector.UI.Base.Helpers
{
    internal static class ThreadHelper
    {
        /// <summary>
        /// Runs the back ground.
        /// </summary>
        /// <param name="action">The action.</param>
        internal static void RunBackGround(Action action)
        {
            RunBackGround(DispatcherPriority.Background, action);
        }

        /// <summary>
        /// Runs the back ground.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="action">The action.</param>
        internal static void RunBackGround(DispatcherPriority priority, Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(priority, (Action)delegate
             {
                 var newThread = new System.Threading.Thread(action.Invoke);
                 newThread.Start();
             });
        }
    }
}