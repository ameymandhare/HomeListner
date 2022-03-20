using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace HomeListner
{
    public sealed class UpdateIPAgent : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // run the background task code
        }

        // call it when your application will start.
        // it will register the task if not already done
        private static IBackgroundTaskRegistration Register()
        {
            // get the entry point of the task. I'm reusing this as the task name in order to get an unique name
            var taskEntryPoint = typeof(UpdateIPAgent).FullName;
            var taskName = taskEntryPoint;

            // if the task is already registered, there is no need to register it again
            var registration = BackgroundTaskRegistration.AllTasks.Select(x => x.Value).FirstOrDefault(x => x.Name == taskName);
            if (registration != null) return registration;

            // register the task to run every 1 minutes if an internet connection is available
            var taskBuilder = new BackgroundTaskBuilder();
            taskBuilder.Name = taskName;
            taskBuilder.TaskEntryPoint = taskEntryPoint;
            taskBuilder.SetTrigger(new TimeTrigger(1, false));
            taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));

            return taskBuilder.Register();
        }
    }
}
