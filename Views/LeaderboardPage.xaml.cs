using MySqlConnector;
using System.Collections.ObjectModel;
using MathMaster.Services;

namespace MathMaster.Views;

public partial class LeaderboardPage : ContentPage
{
    public class LeaderboardEntry
    {
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public int TotalScore { get; set; }

        public string? RankImage { get; set; } // Path to image (e.g., first.png)
        public string? RankNumber { get; set; } // For 4th to 10th place
        public bool IsImageRank { get; set; }
        public bool IsNumberRank { get; set; }

        public int RankImageSize { get; set; } // New property for image size
    }

    private int _userRank = -1;
    private string _userRankImage = string.Empty;

    public LeaderboardPage()
	{
		InitializeComponent();
        ValidateStudentSession();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadLeaderboard();
        await LoadUserRank();
        DisplayUserRank();
        await BackgroundMusicService.Instance.PlayAsync("HomeMusic.mp3");
    }
    private async void ValidateStudentSession()
    {
        string role = Preferences.Get("Role", string.Empty);
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || role != "Student")
        {
            await DisplayAlert("Session Expired", "You must log in first to access the leaderboard.", "OK");
            await Navigation.PushAsync(new LoginPage());
        }
    }
    private async Task LoadLeaderboard()
    {
        var leaderboard = new ObservableCollection<LeaderboardEntry>();

        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            string query = @"
            SELECT u.firstname, u.lastname, l.username, SUM(CAST(l.score AS SIGNED)) AS total_score
            FROM leaderboard l
            INNER JOIN user u ON l.user_id = u.user_id
            GROUP BY l.user_id
            ORDER BY total_score DESC
            LIMIT 10;";

            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            int rank = 1;

            while (await reader.ReadAsync())
            {
                string rankImage = string.Empty;
                string rankNumber = string.Empty;
                bool isImageRank = false;
                bool isNumberRank = false;
                int imageSize = 0;

                switch (rank)
                {
                    case 1:
                        rankImage = "first.png";
                        isImageRank = true;
                        imageSize = 30;
                        break;
                    case 2:
                    case 3:
                        rankImage = rank == 2 ? "second.png" : "third.png";
                        isImageRank = true;
                        imageSize = 30;
                        break;
                    default:
                        rankNumber = rank.ToString();
                        isNumberRank = true;
                        break;
                }

                leaderboard.Add(new LeaderboardEntry
                {
                    FullName = $"{reader["firstname"]} {reader["lastname"]}",
                    Username = reader["username"].ToString(),
                    TotalScore = Convert.ToInt32(reader["total_score"]),
                    RankImage = rankImage,
                    RankNumber = rankNumber,
                    IsImageRank = isImageRank,
                    IsNumberRank = isNumberRank,
                    RankImageSize = imageSize
                });

                rank++;
            }

            LeaderboardCollectionView.ItemsSource = leaderboard;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading leaderboard: {ex.Message}");
            await DisplayAlert("Error", "Failed to load leaderboard data.", "OK");
        }
    }
    private async Task LoadUserRank()
    {
        string username = Preferences.Get("Username", string.Empty);
        if (string.IsNullOrEmpty(username))
        {
            _userRank = -1;
            _userRankImage = string.Empty;
            return;
        }
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            string query = @"
                SELECT u.username
                FROM leaderboard l
                INNER JOIN user u ON l.user_id = u.user_id
                GROUP BY l.user_id
                ORDER BY SUM(CAST(l.score AS SIGNED)) DESC
            ";
            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            int rank = 1;
            while (await reader.ReadAsync())
            {
                string uname = reader["username"].ToString();
                if (uname == username)
                {
                    _userRank = rank;
                    if (rank == 1) _userRankImage = "first.png";
                    else if (rank == 2) _userRankImage = "second.png";
                    else if (rank == 3) _userRankImage = "third.png";
                    break;
                }
                rank++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user rank: {ex.Message}");
            _userRank = -1;
            _userRankImage = string.Empty;
        }
    }

    private void DisplayUserRank()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_userRank > 0)
            {
                if (!string.IsNullOrEmpty(_userRankImage))
                {
                    if (this.FindByName("UserRankImage") is Image rankImg)
                    {
                        rankImg.Source = _userRankImage;
                        rankImg.IsVisible = true;
                    }
                    if (this.FindByName("UserRankLabel") is Label rankLbl)
                    {
                        rankLbl.IsVisible = false;
                    }
                }
                else
                {
                    if (this.FindByName("UserRankLabel") is Label rankLbl)
                    {
                        rankLbl.Text = $"Rank: {_userRank}";
                        rankLbl.IsVisible = true;
                    }
                    if (this.FindByName("UserRankImage") is Image rankImg)
                    {
                        rankImg.IsVisible = false;
                    }
                }
            }
            else
            {
                if (this.FindByName("UserRankLabel") is Label rankLbl)
                {
                    rankLbl.Text = "Rank: N/A";
                    rankLbl.IsVisible = true;
                }
                if (this.FindByName("UserRankImage") is Image rankImg)
                {
                    rankImg.IsVisible = false;
                }
            }
        });
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
}