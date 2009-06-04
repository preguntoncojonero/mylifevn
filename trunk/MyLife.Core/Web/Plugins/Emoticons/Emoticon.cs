namespace MyLife.Web.Plugins.Emoticons
{
    public class Emoticon
    {
        public Emoticon()
        {
        }

        public Emoticon(int code, string asciiCode)
        {
            Code = code;
            ASCIICode = asciiCode;
        }

        public Emoticon(int code, string asciiCode, string description)
        {
            Code = code;
            ASCIICode = asciiCode;
            Description = description;
        }

        public string Description { get; set; }
        public string ASCIICode { get; set; }
        public int Code { get; set; }
    }
}