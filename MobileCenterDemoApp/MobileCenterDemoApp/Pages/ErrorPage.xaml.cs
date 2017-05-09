using System.Collections.Specialized;
using BottomBar.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCenterDemoApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ErrorPage : BottomBarPage
	{
	    public static bool ShowHomePage;
	    private bool _isInit = true;
	    private bool _alreadyPop = false;

	    public ErrorPage(string message)
	    {
	        _isInit = true;

            InitializeComponent();
	        _isInit = false;
	        ErrorLabel.Text = message;
	    }

        protected override async void OnCurrentPageChanged()
        {
            if (_isInit || _alreadyPop)
                return;

            _alreadyPop = true;

            ShowHomePage = this.CurrentPage.Title == "Home";
            await this.Navigation.PopModalAsync();
            base.OnCurrentPageChanged();
        }

    }
}
