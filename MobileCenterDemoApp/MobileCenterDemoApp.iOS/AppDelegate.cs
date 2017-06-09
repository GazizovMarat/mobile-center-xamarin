﻿using Foundation;
using System;
using System.Diagnostics;
using System.Linq;
using UIKit;

namespace MobileCenterDemoApp.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//

        public static UIWindow UiXamarinWindow { get; private set; }

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            // Uncomment next line for using Xamarin.UITests 
            //  but Do not forget comment line for Release version
            // Xamarin.Calabash.Start();

            global::Xamarin.Forms.Forms.Init ();

            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();

            LoadApplication(new App());

            return base.FinishedLaunching (app, options);
		}
	}
}
