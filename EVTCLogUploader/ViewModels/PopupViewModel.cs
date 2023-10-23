namespace EVTCLogUploader.ViewModels
{
    public class PopupViewModel : ViewModelBase
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public PopupViewModel()
        {
            Title = "Error: Popup failed to get message";
            Body = string.Empty;
        }
    }
}
