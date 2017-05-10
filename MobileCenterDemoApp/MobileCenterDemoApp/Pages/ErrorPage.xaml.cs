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
	    private bool _isInit;
	    private bool _alreadyPop;

	    public ErrorPage(string message)
	    {
	        _isInit = true;
            InitializeComponent();
	        _isInit = false;
	        ErrorLabel.Text = message;
	        if (Application.Current.MainPage == this)
	        {
	            Children[0].Title = "";
	            Children[0].Icon = null;
	            Children[1].Title = "Exit";
	            Children[1].Icon = null;
            }
	    }

        protected override async void OnCurrentPageChanged()
        {
            if (_isInit || _alreadyPop)
                return;

            if (Application.Current.MainPage == this)
            {
                
            }

            _alreadyPop = true;

            ShowHomePage = this.CurrentPage.Title == "Home";
            await this.Navigation.PopModalAsync();
            base.OnCurrentPageChanged();
        }

    }
}
