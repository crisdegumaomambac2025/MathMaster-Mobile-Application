using MySqlConnector;
using System.Collections.ObjectModel;
using System;
using MathMaster.Views;

namespace MathMaster.Views;

public partial class AdminRankingPage : ContentPage
{
    public class LeaderboardEntry
    {
        public string FullName { get; set; }
        public string Username { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public string RankImage { get; set; }
        public string RankNumber { get; set; }
        public bool IsImageRank { get; set; }
        public bool IsNumberRank { get; set; }
        public int RankImageSize { get; set; }
    }

    private string? _pendingDeleteUsername; // Nullable to fix warning

    public AdminRankingPage()
	{
		InitializeComponent();
        ValidateAdminSession();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadLeaderboard();
    }
    private async void ValidateAdminSession()
    {
        string role = Preferences.Get("Role", string.Empty);
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || role != "Admin")
        {
            await DisplayAlert("Access Denied", "Admin access only.", "OK");
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
                SELECT u.firstname, u.lastname, l.username, u.user_id, SUM(CAST(l.score AS SIGNED)) AS total_score
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

            AdminLeaderboardCollectionView.ItemsSource = leaderboard;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading leaderboard: {ex.Message}");
            await DisplayAlert("Error", "Failed to load leaderboard data.", "OK");
        }
    }
    private async void OnDeleteUserClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        string? usernameToDelete = button?.CommandParameter?.ToString();

        if (string.IsNullOrEmpty(usernameToDelete))
            return;

        // Show custom confirm dialog instead of DisplayAlert
        _pendingDeleteUsername = usernameToDelete;
        ConfirmDeleteMessage.Text = $"Are you sure you want to delete leaderboard info for user '{usernameToDelete}'?";
        ConfirmDeletePanel.IsVisible = true;
    }

    private async void OnConfirmDeleteYes(object sender, EventArgs e)
    {
        ConfirmDeletePanel.IsVisible = false;
        if (string.IsNullOrEmpty(_pendingDeleteUsername))
            return;
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            // Get user_id from username
            string getUserIdQuery = "SELECT user_id FROM user WHERE username = @username";
            int userId = -1;

            using (var cmd = new MySqlCommand(getUserIdQuery, connection))
            {
                cmd.Parameters.AddWithValue("@username", _pendingDeleteUsername);
                var result = await cmd.ExecuteScalarAsync();
                if (result == null)
                {
                    await DisplayAlert("Error", "User not found.", "OK");
                    return;
                }

                userId = Convert.ToInt32(result);
            }

            // Disable foreign key checks
            using (var cmd = new MySqlCommand("SET FOREIGN_KEY_CHECKS=0;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Delete leaderboard entries only
            using (var cmd = new MySqlCommand("DELETE FROM leaderboard WHERE user_id = @user_id", connection))
            {
                cmd.Parameters.AddWithValue("@user_id", userId);
                await cmd.ExecuteNonQueryAsync();
            }

            // Re-enable foreign key checks
            using (var cmd = new MySqlCommand("SET FOREIGN_KEY_CHECKS=1;", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            ShowSuccessDialog("Deleted", $"Leaderboard info for user '{_pendingDeleteUsername}' has been deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting leaderboard info: {ex.Message}");
            await DisplayAlert("Error", "Failed to delete leaderboard info.", "OK");
        }
        finally
        {
            _pendingDeleteUsername = null;
        }
    }

    private void OnConfirmDeleteCancel(object sender, EventArgs e)
    {
        ConfirmDeletePanel.IsVisible = false;
        _pendingDeleteUsername = null;
    }

    private void ShowSuccessDialog(string title, string message)
    {
        SuccessTitle.Text = title;
        SuccessMessage.Text = message;
        SuccessDialog.IsVisible = true;
    }

    private async void OnSuccessDialogClose(object sender, EventArgs e)
    {
        SuccessDialog.IsVisible = false;
        await LoadLeaderboard();
    }

    private async void OnDashboardClicked(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new AdminPanel());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }
    private async void OnManageUsersClicked(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new ManageUsersPage());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

    private async void OnLoginHistoryClicked(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new LoginHistoryPage());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

    private async void OnRankingClicked(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new AdminRankingPage());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }
}