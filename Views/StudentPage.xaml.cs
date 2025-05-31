using MathMaster.Views;
using MySqlConnector;
using System;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;
using MathMaster.Services;

namespace MathMaster;

public partial class StudentPage : ContentPage
{
    private int _previousLevel = 0;
    private IAudioPlayer? _backgroundMusicPlayer;
    private IAudioPlayer? _levelUpMusicPlayer;
    private double _previousBackgroundMusicVolume = 1.0;

    public StudentPage()
    {
        InitializeComponent();
        ValidateStudentSession();
    }
     protected override async void OnAppearing()
    {
        base.OnAppearing();
        await BackgroundMusicService.Instance.PlayAsync("MathMaster.mp3");

        string role = Preferences.Get("Role", string.Empty);
        string username = Preferences.Get("Username", string.Empty);

        // Check if a quiz score was set (by MentalMath)
        int quizScore = Preferences.Get("LastQuizScore", -1);
        bool showedQuizLevelUp = false;
        if (quizScore >= 0)
        {
            string stars = "";
            if (quizScore == 10)
                stars = "⭐⭐⭐";
            else if (quizScore >= 8)
                stars = "⭐⭐";
            else if (quizScore >= 6)
                stars = "⭐";
            await ShowLevelUpAnimation(stars);
            Preferences.Remove("LastQuizScore"); // Clear after showing
            showedQuizLevelUp = true;
        }

        if (!showedQuizLevelUp && role == "Student" && !string.IsNullOrEmpty(username))
        {
            int currentLevel = await GetStudentLevel(username);

            if (_previousLevel > 0 && currentLevel > _previousLevel)
            {
                //await DisplayAlert("Level Up", "Congratulations! You've leveled up!", "OK");
                await ShowLevelUpAnimation(); // 🎉 Play animation!
            }

            _previousLevel = currentLevel;
            StudentLevelLabel.Text = $"LEVEL {currentLevel}";
        }
    }

    private async void ValidateStudentSession()
    {
        string role = Preferences.Get("Role", string.Empty);
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || role != "Student")
        {
            await DisplayAlert("Session Expired", "You must login first to access this page.", "OK");
            await Navigation.PushAsync(new LoginPage());
        }
        else
        {
            string firstname = await GetStudentFirstname(username);
            int level = await GetStudentLevel(username);

            _previousLevel = level; // Save the current level
            StudentLevelLabel.Text = $"LEVEL {level}";
        }
    }

    private async Task<string> GetStudentFirstname(string username)
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            string query = "SELECT firstname FROM user WHERE username = @username";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching student firstname: {ex.Message}");
            return string.Empty;
        }
    }
    private async Task<int> GetStudentLevel(string username)
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            string query = "SELECT level FROM user WHERE username = @username";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching student level: {ex.Message}");
            return 1;
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "No");
        if (confirm)
        {
            Preferences.Clear(); // Clear session
            await DisplayAlert("Logged Out", "You have been logged out.", "OK");
            await Navigation.PushAsync(new LoginPage()); // Redirect to login
        }
    }
    private async void OnLeaderboardTapped(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new LeaderboardPage());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }
    private async void OnSettingsTapped(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new SettingsPage());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }
    private async void OnHomeTapped(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new HomePage());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }
    //QUIZE...
    private async void OnMentalMathTapped(object sender, EventArgs e)
    {
        // Check if the user is logged in by validating the session
        if (!SessionManager.IsLoggedIn)
        {
            await DisplayAlert("Access Denied", "You must log in first to access the Mental Math section.", "OK");
            await Navigation.PushAsync(new LoginPage()); // Redirect to login page
        }
        else
        {
            // Proceed to Mental Math page as the session is valid
            await Navigation.PushAsync(new MentalMath());
        }
    }
    private async void OnAdditionSubtractionTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            ShowLevelLockedPanel("Please log in first.", "Access Denied");
            return;
        }

        int level = await GetStudentLevel(username);

        if (level < 2)
        {
            ShowLevelLockedPanel("You must reach Level 2 to unlock Addition and Subtraction.");
            return;
        }

        await Navigation.PushAsync(new AdditionSubtraction());
    }

    private async void OnMultiplicationTableTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            ShowLevelLockedPanel("Please log in first.", "Access Denied");
            return;
        }

        int level = await GetStudentLevel(username);

        if (level < 3)
        {
            ShowLevelLockedPanel("You must reach Level 3 to unlock Multiplication Table.");
            return;
        }

        await Navigation.PushAsync(new Multiplication());
    }

    private async void OnExponentsAndRootsTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            ShowLevelLockedPanel("Please log in first.", "Access Denied");
            return;
        }

        int level = await GetStudentLevel(username);

        if (level < 4)
        {
            ShowLevelLockedPanel("You must reach Level 4 to unlock Exponents and Roots.");
            return;
        }

        await Navigation.PushAsync(new ExponentsAndRoots());
    }

    private async void OnDecimalsAndFractionsTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            ShowLevelLockedPanel("Please log in first.", "Access Denied");
            return;
        }

        int level = await GetStudentLevel(username);

        if (level < 5)
        {
            ShowLevelLockedPanel("You must reach Level 5 to unlock Decimals and Fractions.");
            return;
        }

        await Navigation.PushAsync(new DecimalsAndFractions());
    }

    private async void OnAlgebraTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            ShowLevelLockedPanel("Please log in first.", "Access Denied");
            return;
        }

        int level = await GetStudentLevel(username);

        if (level < 6)
        {
            ShowLevelLockedPanel("You must reach Level 6 to unlock Algebra.");
            return;
        }

        await Navigation.PushAsync(new Algebra());
    }

    private async void OnNumbersOperationsTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            ShowLevelLockedPanel("Please log in first.", "Access Denied");
            return;
        }

        int level = await GetStudentLevel(username);

        if (level < 7)
        {
            ShowLevelLockedPanel("You must reach Level 7 to unlock Numbers, Operations.");
            return;
        }

        await Navigation.PushAsync(new NumbersOperations());
    }

    private async void OnGeometryTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            ShowLevelLockedPanel("Please log in first.", "Access Denied");
            return;
        }

        int level = await GetStudentLevel(username);

        if (level < 8)
        {
            ShowLevelLockedPanel("You must reach Level 8 to unlock Geometry.");
            return;
        }

        await Navigation.PushAsync(new Geometry());
    }

    private async void OnGeometricSolidTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            ShowLevelLockedPanel("Please log in first.", "Access Denied");
            return;
        }

        int level = await GetStudentLevel(username);

        if (level < 9)
        {
            ShowLevelLockedPanel("You must reach Level 9 to unlock Geometric Solid.");
            return;
        }

        await Navigation.PushAsync(new GeometricSolid());
    }

    private async void OnConversionOfUnitsTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            ShowLevelLockedPanel("Please log in first.", "Access Denied");
            return;
        }

        int level = await GetStudentLevel(username);

        if (level < 10)
        {
            ShowLevelLockedPanel("You must reach Level 10 to unlock Conversion of Units.");
            return;
        }

        await Navigation.PushAsync(new ConversionOfUnits());
    }
    private async void OnQuizTapped(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new StudentPage());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }
    private async void OnTopicsTapped(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new Topics());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }
    private async Task ShowLevelUpAnimation(string stars = "")
    {
        // Hide the scrollable content
        MainScrollView.IsVisible = false;

        // Show the modal
        LevelUpModal.IsVisible = true;
        LevelUpModal.Opacity = 0;
        LevelUpImage.Scale = 0.5;

        // Set the stars label
        LevelUpStarsLabel.Text = stars;

        // Minimize background music volume
        _previousBackgroundMusicVolume = BackgroundMusicService.Instance.Volume;
        BackgroundMusicService.Instance.Volume = 0.2;

        // Play level up music
        try
        {
            var audioManager = Plugin.Maui.Audio.AudioManager.Current;
            var stream = await FileSystem.OpenAppPackageFileAsync("Level_Up.mp3");
            _levelUpMusicPlayer = audioManager.CreatePlayer(stream);
            _levelUpMusicPlayer.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing level up music: {ex.Message}");
        }

        // Animate the appearance
        await Task.WhenAll(
            LevelUpModal.FadeTo(1, 500, Easing.CubicIn),
            LevelUpImage.ScaleTo(1.2, 200, Easing.CubicOut)
        );

        // Final scale adjustment
        await LevelUpImage.ScaleTo(1, 200, Easing.CubicIn);
    }

    private async void OnLevelUpClose(object sender, EventArgs e)
    {
        // Animate the disappearance
        await Task.WhenAll(
            LevelUpModal.FadeTo(0, 300, Easing.CubicOut),
            LevelUpImage.ScaleTo(0.5, 300, Easing.CubicIn)
        );

        // Hide the modal
        LevelUpModal.IsVisible = false;

        // Stop level up music and restore background music volume
        if (_levelUpMusicPlayer != null)
        {
            _levelUpMusicPlayer.Stop();
            _levelUpMusicPlayer.Dispose();
            _levelUpMusicPlayer = null;
        }
        BackgroundMusicService.Instance.Volume = _previousBackgroundMusicVolume;

        // Refresh the level label immediately
        string username = Preferences.Get("Username", string.Empty);
        if (!string.IsNullOrEmpty(username))
        {
            int currentLevel = await GetStudentLevel(username);
            StudentLevelLabel.Text = $"LEVEL {currentLevel}";
            _previousLevel = currentLevel;
        }

        // Show the scrollable content again
        MainScrollView.IsVisible = true;
    }

    private void ShowLevelLockedPanel(string message, string title = "Level Locked")
    {
        MainScrollView.IsVisible = false;
        LevelLockedPanel.IsVisible = true;
        LevelLockedPanel.Opacity = 1;
        LevelLockedTitle.Text = title;
        LevelLockedMessage.Text = message;
    }

    private void OnLevelLockedClose(object sender, EventArgs e)
    {
        LevelLockedPanel.IsVisible = false;
        MainScrollView.IsVisible = true;
    }
}