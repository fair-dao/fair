namespace fairdao.maui.demo
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
