namespace MobileCenterDemoApp.UITests
{
    using NUnit.Framework;
    using Xamarin.UITest;

    [TestFixture(Platform.Android)]    
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        private IApp _app;
        private readonly Platform _platform;

        public Tests(Platform platform)
        {
            _platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _app = AppInitializer.StartApp(_platform);            
        }

        [Test]
        public void CheckFacebookButton()
        {
            const string buttonName = "Login via Facebook";

            _app.WaitForElement(x => x.Button());

            var button = _app.Query(x => x.Button(buttonName));
            
            Assert.IsNotEmpty(button);
        }

        [Test]
        public void CheckTwitterButton()
        {
            const string buttonName = "Login via Twitter";

            _app.WaitForElement(x => x.Button());

            var button = _app.Query(x => x.Button(buttonName));

            Assert.IsNotEmpty(button);
        }

        [Test]
        public void CheckLoginPage()
        {
            const string buttonName = "Login via Facebook";

            _app.WaitForElement(x => x.Button());

            _app.Tap(x => x.Button(buttonName));

            var webViews = _app.Query(x => x.WebView());

            Assert.IsNotEmpty(webViews);
        }
    }
}