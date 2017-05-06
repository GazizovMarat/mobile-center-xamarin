using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCenterDemoApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ErrorPage : ContentPage
	{
		public ErrorPage (string message)
		{
			InitializeComponent ();

		    ErrorLabel.Text = message;
		    BackButton.Clicked += async (st, t) => await Navigation.PopModalAsync();
		}
	}
}
