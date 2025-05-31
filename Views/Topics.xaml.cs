using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using MySqlConnector;
using System.Linq;
using Microsoft.Maui.Controls.Xaml;
using Plugin.Maui.Audio;
using MathMaster.Services;

namespace MathMaster.Views;

public partial class Topics : ContentPage
{
	private IAudioPlayer _backgroundMusicPlayer;

	public Topics()
	{
		InitializeComponent();
		ValidateStudentSession();
	}

	private async void ValidateStudentSession()
	{
		string role = Preferences.Get("Role", string.Empty);
		string username = Preferences.Get("Username", string.Empty);

		if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || role != "Student")
		{
			await ShowErrorDialog("⛔ Unauthorized", "Only students can access this quiz.");
			await Navigation.PushAsync(new LoginPage());
		}
		else if (!SessionManager.IsLoggedIn)
		{
			await ShowErrorDialog("⚠️ Session Error", "You must be logged in to take the quiz.");
			await Navigation.PopAsync();
		}
		else
		{
			bool alreadyTaken = await HasStudentTakenQuiz(SessionManager.Username);
			if (alreadyTaken)
			{
				await ShowSuccessDialog("🎉 You've Already Completed This Quiz!",
					"Awesome! You've already completed this quiz. Try another one!");
				await Navigation.PopAsync();
			}
			else
			{
				await ShowReadyDialog();
			}
		}
	}

	private async Task<bool> HasStudentTakenQuiz(string username)
	{
		// TODO: Implement actual quiz completion check logic
		return false;
	}

	private async Task ShowReadyDialog()
	{
		var template = (DataTemplate)Resources["ReadyDialogTemplate"];
		var content = (Microsoft.Maui.Controls.View)template.CreateContent();
		var dialog = new DialogPage
		{
			Content = content
		};
		await Navigation.PushModalAsync(dialog);
	}

	private async Task ShowErrorDialog(string title, string message)
	{
		var template = (DataTemplate)Resources["ErrorDialogTemplate"];
		var content = (Microsoft.Maui.Controls.View)template.CreateContent();
		var dialog = new DialogPage
		{
			Content = content
		};
		dialog.BindingContext = new { Message = message };
		await Navigation.PushModalAsync(dialog);
	}

	private async Task ShowSuccessDialog(string title, string message)
	{
		var template = (DataTemplate)Resources["SuccessDialogTemplate"];
		var content = (Microsoft.Maui.Controls.View)template.CreateContent();
		var dialog = new DialogPage
		{
			Content = content
		};
		dialog.BindingContext = new { Message = message };
		await Navigation.PushModalAsync(dialog);
	}

	private async void OnReadyDialogYesClicked(object sender, EventArgs e)
	{
        await Navigation.PopModalAsync(); // Close the dialog

        LoadingOverlay.IsVisible = true;

        try
        {
            await Task.Delay(300); // Optional: allow spinner to show
                                   // TODO: Navigate to the topic selection or learning page here
                                   // Example placeholder
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

	private async void OnReadyDialogNoClicked(object sender, EventArgs e)
	{
        await Navigation.PopModalAsync(); // Close the dialog first

        LoadingOverlay.IsVisible = true;

        try
        {
            // Wait a bit so the overlay is visible before navigating
            await Task.Delay(300); // Optional: smooth visual transition
            await Navigation.PopAsync(); // Navigate back to previous page
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

	private async void OnErrorDialogDismissed(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}

	private async void OnSuccessDialogDismissed(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();
	}

	private async void OnTopicSelected(object sender, EventArgs e)
	{
		if (sender is Button button)
		{
			string topic = button.CommandParameter.ToString();
			
			if (topic == "MentalMath")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://drillyourskill.com/"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else if (topic == "AdditionSubtraction")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://www.k5learning.com/free-math-worksheets/fifth-grade-5/addition-subtraction"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else if (topic == "Multiplication")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://math-drills.com/multiplication.php"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else if (topic == "ExponentsRoots")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://thirdspacelearning.com/gcse-maths/number/powers-and-roots/"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else if (topic == "DecimalsFractions")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://math-drills.com/fractions.php"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else if (topic == "Algebra")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://math-drills.com/algebra.php"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else if (topic == "NumbersOperations")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://www.k5learning.com/math/numbers"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else if (topic == "Geometry")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://math-drills.com/geometry.php"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else if (topic == "GeometricSolids")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://www.mathsisfun.com/geometry/solid-geometry.html"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else if (topic == "UnitConversion")
			{
				try
				{
					await Launcher.OpenAsync(new Uri("https://www.hackmath.net/en/word-math-problems/unit-conversion"));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the website. Please try again later.", "OK");
				}
			}
			else
			{
				await NavigateToTopic(topic);
			}
		}
	}

	private async Task NavigateToTopic(string topic)
	{
		// TODO: Implement navigation to specific topic study page
		await DisplayAlert("Topic Selected", $"You selected to study {topic}. This feature is coming soon!", "OK");
	}

	private async void OnLinkTapped(object sender, EventArgs e)
	{
		if (sender is Label label && label.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer tapGesture)
		{
			string url = tapGesture.CommandParameter?.ToString();
			if (!string.IsNullOrEmpty(url))
			{
				try
				{
					await Launcher.OpenAsync(new Uri(url));
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", "Could not open the link. Please try again later.", "OK");
				}
			}
		}
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await BackgroundMusicService.Instance.PlayAsync("HomeMusic.mp3");
	}
}

public class DialogPage : ContentPage
{
	public DialogPage()
	{
		BackgroundColor = Color.FromArgb("#80000000"); // Semi-transparent background
	}
}