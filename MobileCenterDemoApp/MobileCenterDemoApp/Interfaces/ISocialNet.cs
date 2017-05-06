using System;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Social.Services;

namespace MobileCenterDemoApp.Interfaces
{
    public interface ISocialNet
    {
        event Action<Account> OnConnect;
        Task<Account> Login();        
    }

    public interface IFacebook : ISocialNet
    {
        FacebookService Service { get; }
    }

    public interface ITwitter : ISocialNet
    {
        TwitterService Service { get; }
    }
}
