namespace fairdao.portal
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.blazorWebView.InitBlazorWebView();
        }
    }
}
