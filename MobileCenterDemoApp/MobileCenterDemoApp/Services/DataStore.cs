using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using Xamarin.Auth;
using Xamarin.Social.Services;

namespace MobileCenterDemoApp.Services
{
    public class DataStore
    {        
        public static SocialAccount Account { get; set; }
        public static string UserName => Account?.UserName;

        public static IFacebook FacebookService { get; set; }
        public static ITwitter TwitterService { get; set; }

        public static IFitnessTracker FitnessTracker { get; set; }

        public static OAuth2Authenticator OAuth2;
        public static OAuth1Authenticator OAuth1;
    }
}
