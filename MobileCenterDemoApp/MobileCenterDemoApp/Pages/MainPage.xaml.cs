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

        private Page _profilePage;
        private Page _statisticsPage;

        private string _errorMessage;

        public MainPage(string errorMessage = "")
        {
            InitComponents();

            _instance = this;
            _errorMessage = errorMessage;
        }
        private void InitComponents()
        {
            Children.Add(_profilePage = new ProfilePage());
            Children.Add(_statisticsPage = new StatisticsPage());
            CurrentPage = _profilePage;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (string.IsNullOrEmpty(_errorMessage))
                return;

            ErrorPage errorPage = new ErrorPage(_errorMessage);
            await Navigation.PushModalAsync(errorPage);
            _errorMessage = string.Empty;

            if (errorPage.ShowHomePage)
                CurrentPage = _profilePage;
            else
                CurrentPage = _statisticsPage;
        }
    }



}
