namespace MyLife.Web.DynamicData
{
    public class DynamicTextFieldAttribute : DynamicFieldAttribute
    {
        public bool Multiple { get; set; }

        public int MaxLength { get; set; }
    }
}