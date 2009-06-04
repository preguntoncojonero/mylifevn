namespace MyLife
{
    public static class Constants
    {
        #region Regulars Pattern

        public static class Regulars
        {
            public const string Date =
                @"^(?=\d)(?:(?!(?:(?:0?[5-9]|1[0-4])(?:\.|-|\/)10(?:\.|-|\/)(?:1582))|(?:(?:0?[3-9]|1[0-3])(?:\.|-|\/)0?9(?:\.|-|\/)(?:1752)))(31(?!(?:\.|-|\/)(?:0?[2469]|11))|30(?!(?:\.|-|\/)0?2)|(?:29(?:(?!(?:\.|-|\/)0?2(?:\.|-|\/))|(?=\D0?2\D(?:(?!000[04]|(?:(?:1[^0-6]|[2468][^048]|[3579][^26])00))(?:(?:(?:\d\d)(?:[02468][048]|[13579][26])(?!\x20BC))|(?:00(?:42|3[0369]|2[147]|1[258]|09)\x20BC))))))|2[0-8]|1\d|0?[1-9])([-.\/])(1[012]|(?:0?[1-9]))\2((?=(?:00(?:4[0-5]|[0-3]?\d)\x20BC)|(?:\d{4}(?:$|(?=\x20\d)\x20)))\d{4}(?:\x20BC)?)(?:$|(?=\x20\d)\x20))?((?:(?:0?[1-9]|1[012])(?::[0-5]\d){0,2}(?:\x20[aApP][mM]))|(?:[01]\d|2[0-3])(?::[0-5]\d){1,2})?$";

            public const string Email = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

            public const string Guid =
                @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$";

            public const string Id = "^[1-9]([0-9]+$)?";
            public const string IlegalCharacters = "\\W";
            public const string IndexOfPage = "^[1-9]([0-9]+$)?";
            public const string Slug = "^[a-zA-Z0-9-]+$";
            public const string User = "^[a-zA-Z0-9_]+$";
            public const string Website = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        }

        #endregion

        #region Nested type: DateTime

        public static class DateTime
        {
            public static System.DateTime MinSqlDate = new System.DateTime(1753, 1, 1, 12, 0, 0);
        }

        #endregion

        #region Nested type: Roles

        public static class Roles
        {
            public const string Administrators = "Administrators";
        }

        #endregion

        #region Nested type: RouteData

        public static class RouteData
        {
            public const string Action = "action";
            public const string IndexOfPage = "indexOfPage";
            public const string Slug = "slug";
            public const string User = "user";
        }

        #endregion

        #region Nested type: ViewData

        public static class ViewData
        {
            public const string FeedLink = "FeedLink";
            public const string RsdLink = "RsdLink";
            public const string Title = "Title";

            #region Nested type: Blogs

            public static class Blogs
            {
                public const string Blogrolls = "Blogrolls";
                public const string Categories = "Categories";
                public const string Comment = "Comment";
                public const string Description = "Description";
                public const string Header = "Header";
                public const string IsPostList = "IsPostList";
                public const string Name = "Name";
                public const string Posts = "Posts";
                public const string RecentComments = "RecentComments";
                public const string RecentPosts = "RecentPosts";
                public const string Slogan = "Slogan";
                public const string Themes = "Themes";
            }

            #endregion

            #region Nested type: PageNavigator

            public static class PageNavigator
            {
                public const string BaseUrl = "BaseUrl";
                public const string HasNextPage = "HasNextPage";
                public const string HasPrevPage = "HasPrevPage";
                public const string IndexOfPage = "IndexOfPage";
                public const string TotalPages = "TotalPages";
            }

            #endregion
        }

        #endregion
    }
}