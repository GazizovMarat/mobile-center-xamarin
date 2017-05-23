namespace MobileCenterDemoApp.Helpers
{
    using Xamarin.Forms;

    /// <summary>
    /// Constants which depend on Running Platform
    /// </summary>
    public static class PlatformSizes
    {
        /// <summary>
        /// Radius for round buttons
        /// </summary>
        public static double BorderRadius { get; }

        static PlatformSizes()
        {
            if(Device.RuntimePlatform == Device.Android)
            {
                BorderRadius = 100;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                BorderRadius = 20;
            }
        }
    }
}
