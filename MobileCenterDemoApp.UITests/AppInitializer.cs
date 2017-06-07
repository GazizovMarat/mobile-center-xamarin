using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MobileCenterDemoApp.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp
                    .Android
                    .ApkFile("../../../MobileCenterDemoApp/MobileCenterDemoApp.Android/bin/Debug/" +
                    "com.mobilecenterdemoapp.xamarin-Signed.apk")
                    .DeviceSerial("91b7d68a")
                    .StartApp();
            }

            if (platform == Platform.iOS)
            {
                return ConfigureApp
                    .iOS
                    .StartApp();
            }

            throw new PlatformNotSupportedException();
        }
    }
}

