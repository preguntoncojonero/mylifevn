namespace MyLife.Web.Plugins.Emoticons
{
    public static class Emoticons
    {
        private static readonly Emoticon[] emoticons = new[]
                                                           {
                                                               new Emoticon(109, "X_X", "I don't want to see"),
                                                               new Emoticon(102, "~X(", "at wits' end"),
                                                               new Emoticon(14, "X(", "angry"),
                                                               new Emoticon(25, "O:-)", "angel"),
                                                               new Emoticon(30, "L-)", "loser"),
                                                               new Emoticon(28, "I-)", "sleepy"),
                                                               new Emoticon(16, "B-)", "cool"),
                                                               new Emoticon(105, "8->", "day dreaming"),
                                                               new Emoticon(35, "8-}", "silly"),
                                                               new Emoticon(29, "8-|", "rolling eyes"),
                                                               new Emoticon(47, ">:P", "phbbbbt"),
                                                               new Emoticon(6, ">:D<", "big hug"),
                                                               new Emoticon(19, ">:)", "devil"),
                                                               new Emoticon(38, "=P~", "drooling"),
                                                               new Emoticon(41, "=D>", "applause"),
                                                               new Emoticon(27, "=;", "talk to the hand"),
                                                               new Emoticon(24, "=))", "rolling on the floor"),
                                                               new Emoticon(12, "=((", "broken heart"),
                                                               new Emoticon(36, "<:-P", "party"),
                                                               new Emoticon(48, "<):)", "cowboy"),
                                                               new Emoticon(114, "^#(^", "it wasn't me"),
                                                               new Emoticon(111, "\\m/", "rock on!"),
                                                               new Emoticon(33, "[-(", "no talking"),
                                                               new Emoticon(43, "@-)", "hypnotized"),
                                                               new Emoticon(5, ";;)", "batting eyelashes"),
                                                               new Emoticon(3, ";)", "winking"),
                                                               new Emoticon(8, ":x", "love struck"),
                                                               new Emoticon(45, ":-w", "waiting"),
                                                               new Emoticon(104, ":-t", "time out"),
                                                               new Emoticon(18, "#:-S", "whew!"),
                                                               new Emoticon(42, ":-SS", "nail biting"),
                                                               new Emoticon(17, ":-S", "worried"),
                                                               new Emoticon(112, ":-q", "thumbs down"),
                                                               new Emoticon(10, ":P", "tongue"),
                                                               new Emoticon(34, ":O)", "clown"),
                                                               new Emoticon(13, ":-0", "surprise"),
                                                               new Emoticon(103, ":-h", "wave"),
                                                               new Emoticon(4, ":D", "big grin"),
                                                               new Emoticon(101, ":-c", "call me"),
                                                               new Emoticon(113, ":-bd", "thumbs up"),
                                                               new Emoticon(26, ":-B", "nerd"),
                                                               new Emoticon(15, ":>", "smug"),
                                                               new Emoticon(46, ":-<", "sigh"),
                                                               new Emoticon(37, "(:|", "yawn"),
                                                               new Emoticon(22, ":|", "straight face"),
                                                               new Emoticon(44, ":^o", "liar"),
                                                               new Emoticon(39, ":-?", "thinking"),
                                                               new Emoticon(7, ":-/", "confused"),
                                                               new Emoticon(11, ":-*", "kiss"),
                                                               new Emoticon(100, ":)]", "on the phone"),
                                                               new Emoticon(21, ":))", "laughing"),
                                                               new Emoticon(23, "/:)", "raised eyebrows"),
                                                               new Emoticon(1, ":)", "happy"),
                                                               new Emoticon(20, ":((", "crying"),
                                                               new Emoticon(2, ":(", "sad"),
                                                               new Emoticon(31, ":-&", "sick"),
                                                               new Emoticon(32, ":-$", "don't tell anyone"),
                                                               new Emoticon(9, ":\">", "blushing"),
                                                               new Emoticon(110, ":!!", "hurry up!"),
                                                               new Emoticon(40, "#-o", "d'oh")
                                                           };

        public static string Replace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            foreach (var emoticon in emoticons)
            {
                input = input.Replace(emoticon.ASCIICode,
                                      string.Format("<img src=\"/emoticon/{0}\" alt=\"{1}\" />", emoticon.Code,
                                                    emoticon.Description));
            }
            return input;
        }
    }
}