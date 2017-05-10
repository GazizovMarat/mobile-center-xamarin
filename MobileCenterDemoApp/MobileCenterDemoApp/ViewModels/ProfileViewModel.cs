using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Services;
using Xamarin.Forms;

namespace MobileCenterDemoApp.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        public string Username { get; }

        public int StepsCount => DataStore.TodaySteps;

        public double Calories => DataStore.TodayCalories;

        public double Distance => DataStore.TodayDistance;

        public string Time => $"{DataStore.TodayActiveTime:%h}h {DataStore.TodayActiveTime:%m}m";


        public ImageSource AccountImageSource { get; }            

        public ProfileViewModel()
        {
            Username = DataStore.Account?.UserName;
            AccountImageSource = DataStore.Account?.ImageSource;
            DataStore.DataFill += SetProperties;
        }

        private void SetProperties()
        {
            OnPropertyChanged(nameof(StepsCount));
            OnPropertyChanged(nameof(Calories));
            OnPropertyChanged(nameof(Distance));
            OnPropertyChanged(nameof(Time));
        }
    }
}
