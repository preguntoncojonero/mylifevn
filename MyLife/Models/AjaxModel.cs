namespace MyLife.Models
{
    public class AjaxModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
        public object Data { get; set; }
    }
}