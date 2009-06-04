namespace MyLife.Net.Mail.IMAP
{
    public enum ConnectionState
    {
        Connecting,
        Connected,
        Authenticating,
        Open,
        Broken,
        Closed
    }
}