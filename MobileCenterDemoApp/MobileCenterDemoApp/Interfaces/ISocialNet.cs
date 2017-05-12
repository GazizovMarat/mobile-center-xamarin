using System;
using System.Threading.Tasks;
using MobileCenterDemoApp.Models;

namespace MobileCenterDemoApp.Interfaces
{
    public interface ISocialNet
    {
        Task<SocialAccount> Login();
        event Action<string> OnError;
    }

    public interface IFacebook : ISocialNet
    {
        
    }

    public interface ITwitter : ISocialNet
    {
        
    }
}
