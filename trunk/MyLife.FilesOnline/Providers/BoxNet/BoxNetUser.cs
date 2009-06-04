namespace MyLife.FilesOnline.Providers.BoxNet
{
    public class BoxNetUser : User
    {
        public int AccessId { get; set; }
        public string Sid { get; set; }

        public string Email
        {
            get { return Name; }
            set { Name = value; }
        }

        public bool Free { get; set; }

        public string Login { get; set; }

        public long SpaceAmount { get; set; }

        public long SpaceUsed { get; set; }

        public int UserId { get; set; }

        public bool IsAuthenticated
        {
            get { return (!string.IsNullOrEmpty(Sid)); }
        }
    }
}