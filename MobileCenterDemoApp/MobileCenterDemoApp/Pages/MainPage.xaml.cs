using Android.Text.Method;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCenterDemoApp.Views
{
#if __ANDROID__
    using BottomBar.XamarinForms;
#endif
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class MainPage
#if __ANDROID__
        : BottomBarPage
#else
        : TabbedPage
#endif
    {
        private static MainPage _instance;

        public static void SwitchHome()
            => _instance.CurrentPage = _instance.Children[0];
        public static void SwitchStatistics()
            => _instance.CurrentPage = _instance.Children[1];

        private string _errorMessage;

        public MainPage(string errorMessage = "")
        {
            Children.Add(new ProfilePage());
            Children.Add(new StatisticsPage());
            _instance = this;

            _errorMessage = errorMessage;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (string.IsNullOrEmpty(_errorMessage))
                return;

            var errorPage = new ErrorPage(_errorMessage);
            await Navigation.PushModalAsync(errorPage);
            _errorMessage = string.Empty;

            CurrentPage =  Children[errorPage.ShowHomePage ? 0 : 1];
        }
    }



}
