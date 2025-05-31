using MathMaster;
using MySqlConnector;
using System.Collections.ObjectModel;
using System;
using Microsoft.Maui.Storage;

namespace MathMaster.Views;

public partial class ManageUsersPage : ContentPage
{
    public ObservableCollection<UserDisplayModel> UsersList { get; set; } = new();

    // Store the username to delete when showing the custom dialog
    private string? _pendingDeleteUsername;

    public ManageUsersPage()
    {
        InitializeComponent();
        BindingContext = this;
        ValidateAdminSessionAndLoadUsers();
    }

    private async void ValidateAdminSessionAndLoadUsers()
    {
        string role = Preferences.Get("Role", string.Empty);
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || role != "Admin")
        {
            await DisplayAlert("Unauthorized", "Only admins can access this page.", "OK");
            await Navigation.PushAsync(new LoginPage());
        }
        else
        {
            await LoadStudentUsersFromDatabase();
        }
    }

    private async Task LoadStudentUsersFromDatabase()
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            string query = "SELECT firstname, lastname, username, email FROM user WHERE role = 'Student'";
            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            int rowIndex = 0;
            while (await reader.ReadAsync())
            {
                UsersList.Add(new UserDisplayModel
                {
                    FirstName = reader.GetString("firstname"),
                    LastName = reader.GetString("lastname"),
                    Username = reader.GetString("username"),
                    Email = reader.GetString("email"),
                    RowColor = rowIndex++ % 2 == 0 ? "#ECF0F1" : "#FFFFFF"
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load users: {ex.Message}", "OK");
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        string username = button?.CommandParameter.ToString();

        if (!string.IsNullOrEmpty(username))
        {
            var user = UsersList.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                // Navigate to the Edit Modal
                await Navigation.PushModalAsync(new EditModalView(user));
            }
        }
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        string? username = button?.CommandParameter?.ToString();

        if (!string.IsNullOrEmpty(username))
        {
            _pendingDeleteUsername = username;
            // Optionally, show username in the dialog message
            ConfirmDeleteMessage.Text = $"Are you sure you want to delete user '{username}'?";
            ConfirmDeletePanel.IsVisible = true;
        }
    }

    private async void OnConfirmDeleteYes(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_pendingDeleteUsername))
        {
            await DeleteUserFromDatabase(_pendingDeleteUsername);
            var user = UsersList.FirstOrDefault(u => u.Username == _pendingDeleteUsername);
            if (user != null)
            {
                UsersList.Remove(user);
            }
        }
        ConfirmDeletePanel.IsVisible = false;
        _pendingDeleteUsername = null;
    }

    private void OnConfirmDeleteNo(object sender, EventArgs e)
    {
        ConfirmDeletePanel.IsVisible = false;
        _pendingDeleteUsername = null;
    }

    private void OnConfirmDeleteCancel(object sender, EventArgs e)
    {
        ConfirmDeletePanel.IsVisible = false;
        _pendingDeleteUsername = null;
    }

    private async Task DeleteUserFromDatabase(string username)
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            // Get user_id from username
            int userId = -1;
            string getUserIdQuery = "SELECT user_id FROM user WHERE username = @username";
            using (var getIdCmd = new MySqlCommand(getUserIdQuery, connection))
            {
                getIdCmd.Parameters.AddWithValue("@username", username);
                var result = await getIdCmd.ExecuteScalarAsync();
                if (result == null)
                {
                    await DisplayAlert("Error", "User not found.", "OK");
                    return;
                }
                userId = Convert.ToInt32(result);
            }

            // Delete from login_history
            string deleteLoginHistory = "DELETE FROM login_history WHERE user_id = @user_id";
            using (var cmd = new MySqlCommand(deleteLoginHistory, connection))
            {
                cmd.Parameters.AddWithValue("@user_id", userId);
                await cmd.ExecuteNonQueryAsync();
            }

            // Delete from leaderboard
            string deleteLeaderboard = "DELETE FROM leaderboard WHERE user_id = @user_id";
            using (var cmd = new MySqlCommand(deleteLeaderboard, connection))
            {
                cmd.Parameters.AddWithValue("@user_id", userId);
                await cmd.ExecuteNonQueryAsync();
            }

            // Delete from user
            string deleteUser = "DELETE FROM user WHERE user_id = @user_id";
            using (var cmd = new MySqlCommand(deleteUser, connection))
            {
                cmd.Parameters.AddWithValue("@user_id", userId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to delete user: {ex.Message}", "OK");
        }
    }

    private async void OnAddUserClicked(object sender, EventArgs e)
    {
        string role = Preferences.Get("Role", string.Empty);

        if (role == "Admin")
        {
            await Navigation.PushModalAsync(new AddModalView());
        }
        else
        {
            await DisplayAlert("Unauthorized", "Only admins can add users.", "OK");
        }
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AdminPanel());
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

    private async void OnManageUsersClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ManageUsersPage());
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

    public class UserDisplayModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? RowColor { get; set; } // For alternate row shading
    }
}