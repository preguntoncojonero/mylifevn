using System;
using System.Collections.ObjectModel;
using System.Net.Mail;

namespace MyLife.Net.Mail.IMAP
{
    public class MailMessageCollection : Collection<MailMessage>
    {
        protected override void InsertItem(int index, MailMessage item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, MailMessage item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            base.SetItem(index, item);
        }
    }
}