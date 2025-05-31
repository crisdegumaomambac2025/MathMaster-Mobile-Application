using MathMaster.Views;
using MySqlConnector;
using System.Collections.ObjectModel;
using Plugin.Maui.Audio;
using MathMaster.Services;

namespace MathMaster.Views;

public partial class HomePage : ContentPage
{
    private IAudioPlayer _backgroundMusicPlayer;
    public ObservableCollection<QuizHistoryItem> QuizHistory { get; set; } = new();

    public HomePage()
    {
        InitializeComponent();
        ValidateStudentSession();
        LoadQuizHistory();
        QuizHistoryCollection.ItemsSource = QuizHistory;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await BackgroundMusicService.Instance.PlayAsync("HomeMusic.mp3");
    }

    private async void ValidateStudentSession()
    {
        string role = Preferences.Get("Role", string.Empty);
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || role != "Student")
        {
            await DisplayAlert("Unauthorized", "Only students can access this page.", "OK");
            await Navigation.PushAsync(new LoginPage());
        }
        else
        {
            string firstname = await GetStudentFirstname(username);
            StudentNameLabel.Text = $"Welcome, {firstname ?? username}!";
        }
    }

    private async Task<string> GetStudentFirstname(string username)
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            var query = "SELECT firstname FROM user WHERE username = @username AND role = 'Student'";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching student name: {ex.Message}");
            return null;
        }
    }

    private async void LoadQuizHistory()
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            string username = Preferences.Get("Username", string.Empty);
            var query = "SELECT quiz_name, score, date FROM leaderboard WHERE username = @username ORDER BY date DESC";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            using var reader = await cmd.ExecuteReaderAsync();

            QuizHistory.Clear();

            while (await reader.ReadAsync())
            {
                QuizHistory.Add(new QuizHistoryItem
                {
                    QuizName = $"Quiz: {reader["quiz_name"]}",
                    Score = $"Score: {reader["score"]}",
                    Date = $"Date: {Convert.ToDateTime(reader["date"]).ToString("yyyy-MM-dd HH:mm")}"
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading quiz history: {ex.Message}");
            await DisplayAlert("Error", "Could not load quiz history.", "OK");
        }
    }

    private async void OnLeaderboardTapped(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new LeaderboardPage()); // Replace with your actual page
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

    private async void OnQuizTapped(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new StudentPage()); // Replace with your actual page
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
            await Navigation.PushAsync(new Topics()); // Replace with your actual page
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(username))
        {
            await DisplayAlert("Access Denied", "You must log in first before accessing your profile.", "OK");
            await Navigation.PushAsync(new LoginPage()); // Redirect to login page
        }
        else
        {
            // Fetch the student's firstname and lastname from the database
            var studentInfo = await GetStudentInfo(username);
            if (studentInfo != null)
            {
                string firstname = studentInfo.Item1;
                string lastname = studentInfo.Item2;
                // Pass the firstname and lastname to ProfilePage
                LoadingOverlay.IsVisible = true;
                try
                {
                    await Navigation.PushAsync(new ProfilePage(firstname, lastname)); // Replace with your actual page
                }
                finally
                {
                    LoadingOverlay.IsVisible = false;
                }
            }
            else
            {
                await DisplayAlert("Error", "Student data could not be fetched.", "OK");
            }
        }
    }

    private async Task<Tuple<string, string>> GetStudentInfo(string username)
    {
        string firstname = string.Empty;
        string lastname = string.Empty;

        try
        {
            using (var connection = DB_Connection.GetConnection()) // Correct usage of DB Connection
            {
                await connection.OpenAsync();
                string query = "SELECT firstname, lastname FROM user WHERE username = @username";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            firstname = reader.GetString("firstname");
                            lastname = reader.GetString("lastname");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log or handle the error as needed
            Console.WriteLine($"Error fetching student info: {ex.Message}");
        }

        return string.IsNullOrEmpty(firstname) ? null : new Tuple<string, string>(firstname, lastname);
    }

    public class QuizHistoryItem
    {
        public string QuizName { get; set; }
        public string Score { get; set; }
        public string Date { get; set; }
    }

    protected override bool OnBackButtonPressed()
    {
        // Show logout confirmation dialog instead of navigating back
        LogoutConfirmPanel.IsVisible = true;
        return true; // Prevent default back navigation
    }

    private void OnConfirmLogoutCancel(object sender, EventArgs e)
    {
        LogoutConfirmPanel.IsVisible = false;
    }

    private async void OnConfirmLogoutYes(object sender, EventArgs e)
    {
        LogoutConfirmPanel.IsVisible = false;
        Preferences.Clear(); // Clear session
        Preferences.Remove("Role"); // Explicitly remove role in case it's not cleared
        BackgroundMusicService.Instance.Stop(); // Stop music on logout
        LoadingOverlay.IsVisible = true;
        try
        {
            // Replace the navigation stack with LoginPage to prevent back navigation
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }
}
