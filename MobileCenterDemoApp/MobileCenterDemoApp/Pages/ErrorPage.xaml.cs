using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCenterDemoApp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ErrorPage : ContentPage
	{
	    public bool ShowHomePage;

	    public ErrorPage(string message)
	    {
            InitializeComponent();
	        ErrorLabel.Text = message;
	    }

	    private async void HomeClicked(object sender, EventArgs e)
	    {
	        ShowHomePage = true;
	        await Navigation.PopModalAsync();
	    }

	    private async void StatisticsClicked(object sender, EventArgs e)
	    {
	        ShowHomePage = false;
	        await Navigation.PopModalAsync();
        }
	}
}
