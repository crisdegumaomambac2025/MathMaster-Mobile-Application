namespace MathMaster;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}
    private async void OnGetStartedClicked(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true; // Show loading spinner
        try
        {
            await Task.Delay(400); // Optional: show spinner for a short time for UX
            await Navigation.PushAsync(new LoginPage()); // Navigate to Login Page
        }
        finally
        {
            LoadingOverlay.IsVisible = false; // Hide loading spinner
        }
    }
}