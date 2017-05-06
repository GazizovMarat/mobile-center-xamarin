using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;
using Xamarin.Social.Services;

namespace MobileCenterDemoApp.Services
{
    public class DataStore
    {        
        public static Account Account { get; set; }
        public static string UserName => Account?.Username;

        public static FacebookService FacebookService { get; set; }
        public static TwitterService TwitterService { get; set; }
    }
}
