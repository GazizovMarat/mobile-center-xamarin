using System.Threading.Tasks;
using MobileCenterDemoApp.Models;

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
