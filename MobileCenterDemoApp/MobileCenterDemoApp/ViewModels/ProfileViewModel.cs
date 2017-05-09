using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Services;
using Xamarin.Forms;

namespace MobileCenterDemoApp.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        public string Username => DataStore.Account?.UserName;

        public int StepsCount 
            => DataStore.TodaySteps;
        public double Calories 
            => DataStore.TodayCalories;
        public double Distance 
            => DataStore.TodayDistance;
        public string Time 
            => $"{DataStore.TodayActiveTime:%h}h {DataStore.TodayActiveTime:%m}m";

        public ImageSource AccountImageSource 
            => DataStore.Account.ImageSource;
    }
}
