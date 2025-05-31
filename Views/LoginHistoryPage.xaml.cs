using MathMaster;
using MySqlConnector;
using System.Collections.ObjectModel;
using Microsoft.Maui.Storage;

namespace MathMaster.Views;

public partial class LoginHistoryPage : ContentPage
{
    public ObservableCollection<LoginHistoryDisplayModel> LoginHistoryList { get; set; } = new();

    public LoginHistoryPage()
    {
        InitializeComponent();
        BindingContext = this;
        ValidateAdminSessionAndLoadHistory();
    }

    private async void ValidateAdminSessionAndLoadHistory()
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
            await LoadLoginHistoryFromDatabase();
        }
    }

    private async Task LoadLoginHistoryFromDatabase()
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            string query = @"
                SELECT lh.login_time, lh.ip_address, lh.device_type, lh.location, u.username
                FROM login_history lh
                JOIN user u ON lh.user_id = u.user_id
                WHERE u.role = 'Student'
                ORDER BY lh.login_time DESC";

            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                LoginHistoryList.Add(new LoginHistoryDisplayModel
                {
                    Username = reader.GetString("username"),
                    LoginTime = reader.GetDateTime("login_time").ToString("g"),
                    IPAddress = reader.GetString("ip_address"),
                    DeviceType = reader.GetString("device_type"),
                    Location = reader.GetString("location")
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load login history: {ex.Message}", "OK");
        }
    }

    // Footer navigation
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
    private async void OnHomeClicked(object sender, EventArgs e)
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

    public class LoginHistoryDisplayModel
    {
        public string Username { get; set; }
        public string LoginTime { get; set; }
        public string IPAddress { get; set; }
        public string DeviceType { get; set; }
        public string Location { get; set; }
    }
}
