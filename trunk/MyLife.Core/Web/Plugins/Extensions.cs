namespace MyLife.Web.Plugins
{
    public static class Extensions
    {
        public static string Emoticons(this string input)
        {
            return Plugins.Emoticons.Emoticons.Replace(input);
        }
    }
}