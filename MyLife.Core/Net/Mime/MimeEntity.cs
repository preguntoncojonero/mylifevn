namespace MyLife.Net.Mime
{
    /// <summary>
    /// Factory for classes which implement IMimeEntity
    /// </summary>
    public static class MimeEntity
    {
        /// <summary>
        /// Returns an IMimeEntity instance by the content type parsed from the MIME data
        /// </summary>
        /// <param name="mimeParser">An IMimeParser instance which is passed to the new IMimeEntity instance</param>
        /// <param name="mimeData">A string containing the MIME data for the new IMimeEntity instance</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>contentType</term>
        ///             <description>Type of new IMimeEntity</description>
        ///         </listheader>
        ///         <item>
        ///             <term>multipart/...</term>
        ///             <description>MultipartMimeEntity</description>
        ///         </item>
        ///         <item>
        ///             <term>text/...</term>
        ///             <description>TextMimeEntity</description>
        ///         </item>
        ///         <item>
        ///             <term>all other</term>
        ///             <description>AttachmentMimeEntity</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public static IMimeEntity GetInstance(IMimeParser mimeParser, string mimeData)
        {
            IMimeEntity mimeEntity;
            var contentType = mimeParser.ParseContentType(mimeData);
            var mainType = (contentType.Contains("/"))
                               ? contentType.Substring(0, contentType.IndexOf("/"))
                               : contentType;
            switch (mainType)
            {
                case "text":
                    mimeEntity = new TextMimeEntity(mimeParser, mimeData);
                    break;
                case "multipart":
                    mimeEntity = new MultipartMimeEntity(mimeParser, mimeData);
                    break;
                default:
                    mimeEntity = new AttachmentMimeEntity(mimeParser, mimeData);
                    break;
            }
            return mimeEntity;
        }
    }
}