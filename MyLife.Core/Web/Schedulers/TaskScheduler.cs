using System.Collections.Generic;

namespace MyLife.Web.Schedulers
{
    public static class TaskScheduler
    {
        public static List<Task> tasks = new List<Task>();

        public static void StartTasks()
        {
            foreach (var task in tasks)
            {
                if (!task.IsRunning)
                    task.Start();
            }
        }

        public static void StopTasks()
        {
            foreach (var task in tasks)
            {
                task.Stop();
            }
        }

        public static void AddTask(Task task)
        {
            tasks.Add(task);
        }

        public static void RemoveTask(Task task)
        {
            tasks.Remove(task);
        }
    }
}