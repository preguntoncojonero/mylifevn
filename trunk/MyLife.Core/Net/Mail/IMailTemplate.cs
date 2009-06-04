namespace MyLife.Net.Mail
{
    public interface IMailTemplate
    {
        string Subject { get; set; }
        string Body { get; set; }
        object Data { get; set; }
        void Process();
    }
}