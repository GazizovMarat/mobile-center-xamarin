using Xamarin.Forms;

namespace MobileCenterDemoApp.Helpers
{
    public static class PlatformSizes
    {
        public static double BorderRadius { get; }
        static PlatformSizes()
        {
            BorderRadius = Device.RuntimePlatform == Device.Android ? 100 : 20;
        }


    }
}
