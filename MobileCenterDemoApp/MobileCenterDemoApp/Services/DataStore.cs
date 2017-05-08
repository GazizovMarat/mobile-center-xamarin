using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;

namespace MobileCenterDemoApp.Services
{
    public class DataStore
    {        
        public static SocialAccount Account { get; set; }

        public static IFacebook FacebookService { get; set; }
        public static ITwitter TwitterService { get; set; }

        public static IFitnessTracker FitnessTracker { get; set; }
    }
}
