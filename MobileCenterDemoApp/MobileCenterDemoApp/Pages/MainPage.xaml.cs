using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCenterDemoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage
    {
        private static MainPage _instance;

        public static void SwitchHome()
            => _instance.CurrentPage = _instance.Children[0];
        public static void SwitchStatistics()
            => _instance.CurrentPage = _instance.Children[1];

        public MainPage()
        {
            InitializeComponent();

            _instance = this;

            DataStore.FitnessTracker.Connect();
        }
    }
}
