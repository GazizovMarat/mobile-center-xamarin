using NUnit.Framework;
using Xamarin.UITest;
using System.Linq;
using System;

namespace MobileCenterDemoApp.UITests
{
    [TestFixture(Platform.Android)]    
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        string _platformButtonName;

        public Tests(Platform platform)
        {
            this.platform = platform;
            _platformButtonName = "Button";
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);            
        }

        [Test]
        public void CheckFacebookButton()
        {
            var facebookButtons = app.Query("Login via Facebook");
            Assert.IsNotEmpty(facebookButtons);

            var facebookbutton = facebookButtons.FirstOrDefault(x => x.Class.Contains(_platformButtonName));
            Assert.NotNull(facebookbutton);
        }

        [Test]
        public void CheckTwitterButton()
        {
            var twitterButtons = app.Query("Login via Twitter");
            Assert.IsNotEmpty(twitterButtons);

            var twitterButton = twitterButtons.FirstOrDefault(x => x.Class.Contains(_platformButtonName));
            Assert.NotNull(twitterButton);                                            
        }
    }
}

