using System;
using NUnit.Framework;
using Xamarin.UITest;
using System.Linq;

namespace MobileCenterDemoApp.UITests
{
    [TestFixture(Platform.Android)]    
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp _app;
        readonly Platform _platform;

        readonly string _platformButtonName;

        public Tests(Platform platform)
        {
            _platform = platform;
            _platformButtonName = "Button";
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _app = AppInitializer.StartApp(_platform);            
        }

        [Test]
        public void CheckFacebookButton()
        {
            var facebookButtons = _app.Query("Login via Facebook");
            Assert.IsNotEmpty(facebookButtons);

            var facebookbutton = facebookButtons.FirstOrDefault(x => x.Class.Contains(_platformButtonName));
            Assert.NotNull(facebookbutton);
        }

        [Test]
        public void CheckTwitterButton()
        {
            var twitterButtons = _app.Query("Login via Twitter");
            Assert.IsNotEmpty(twitterButtons);

            var twitterButton = twitterButtons.FirstOrDefault(x => x.Class.Contains(_platformButtonName));
            Assert.NotNull(twitterButton);                                            
        }

        [Test]
        public void CheckLoginPage()
        {
            _app.Tap("Login via Twitter");
            _app.WaitForElement(x => x.WebView(), timeout: TimeSpan.FromSeconds(20));
            Assert.IsNotEmpty(_app.Query(x => x.WebView()));
        }
    }
}