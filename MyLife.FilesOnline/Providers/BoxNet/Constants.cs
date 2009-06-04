namespace MyLife.FilesOnline.Providers.BoxNet
{
    internal static class Constants
    {
        public const string BoxAPIGeneralUrl = "http://www.box.net/ping/{0}";
        public const string BoxAPILoginUrl = "https://www.box.net/ping";

        public const string BoxAPIUploadUrl = "http://www.box.net/ping/upload/{0}";
        public const string DirectLinkFormat = "http://www.box.net/public/static/{0}.{1}";

        public const string FileTreeRequest =
            "<xml><action>account_tree</action><folder_id>{0}</folder_id><one_level>{1}</one_level></xml>";

        public const string LoginRequest =
            "<xml><action>authorization</action><login>{0}</login><password>{1}</password></xml>";
    }
}