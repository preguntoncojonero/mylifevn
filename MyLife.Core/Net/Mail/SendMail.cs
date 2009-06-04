using System;
using System.Collections;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace MyLife.Net.Mail
{
    public static class SendMail
    {
        private static readonly Queue tasks = new Queue();

        public static void Send(string to, string subject, string body)
        {
            Send(null, new[] { to }, null, null, subject, body);
        }

        public static void Send(string from, string to, string subject, string body)
        {
            Send(from, new[] {to}, null, null, subject, body);
        }

        public static void Send(string from, string[] to, string[] cc, string[] bcc, string subject, string body)
        {
            var task = new SendMailTask {From = from, To = to, Cc = cc, Bcc = bcc, Subject = subject, Body = body};
            lock (tasks)
            {
                tasks.Enqueue(task);
            }
            ThreadPool.QueueUserWorkItem(DoSendMail);
        }

        private static void DoSendMail(object state)
        {
            SendMailTask task;
            lock (tasks)
            {
                task = tasks.Dequeue() as SendMailTask;
            }

            if (task != null)
            {
                var message = new MailMessage();

                if (!string.IsNullOrEmpty(task.From))
                {
                    message.From = new MailAddress(task.From);
                }
                else
                {
                    message.Headers.Add("Reply-To", "nguyen.dainghia@gmail.com");
                    message.Headers.Add("Return-Path", "nguyen.dainghia@gmail.com");
                }

                if (task.To != null && task.To.Length > 0)
                {
                    foreach (var address in task.To)
                    {
                        message.To.Add(address);
                    }
                }

                if (task.Cc != null && task.Cc.Length > 0)
                {
                    foreach (var address in task.Cc)
                    {
                        message.CC.Add(address);
                    }
                }

                if (task.Bcc != null && task.Bcc.Length > 0)
                {
                    foreach (var address in task.Bcc)
                    {
                        message.Bcc.Add(address);
                    }
                }

                message.SubjectEncoding = Encoding.UTF8;
                message.Subject = task.Subject;

                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Body = task.Body;
                message.Priority = MailPriority.High;
                
                var smtpClient = new SmtpClient();

                try
                {
                    smtpClient.Send(message);
                }
                catch (Exception ex)
                {
                    var log = new LogEntry {Message = ex.Message, Severity = TraceEventType.Error};
                    Logger.Write(log);
                }
            }
        }

        #region Nested type: SendMailTask

        private class SendMailTask
        {
            public string From { get; set; }

            public string[] To { get; set; }

            public string[] Cc { get; set; }

            public string[] Bcc { get; set; }

            public string Subject { get; set; }

            public string Body { get; set; }
        }

        #endregion
    }
}