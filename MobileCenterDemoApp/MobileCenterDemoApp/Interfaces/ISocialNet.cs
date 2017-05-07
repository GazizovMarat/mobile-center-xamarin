using System;
using System.Threading.Tasks;
using MobileCenterDemoApp.Models;
using Xamarin.Auth;
using Xamarin.Social.Services;

namespace MobileCenterDemoApp.Interfaces
{
    public interface ISocialNet
    {
        Task<SocialAccount> Login();        
    }

    public interface IFacebook : ISocialNet
    {
        
    }

    public interface ITwitter : ISocialNet
    {
        
    }
}
