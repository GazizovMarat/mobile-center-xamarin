using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;
using MobileCenterDemoApp.Interfaces;
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

        public static IFitnessTracker FitnessTracker { get; set; }
    }
}
