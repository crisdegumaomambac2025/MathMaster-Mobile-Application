using MathMaster.Views;
using MySqlConnector;
using System;
using System.Collections.ObjectModel;

namespace MathMaster;

public partial class AdminPanel : ContentPage
{
    // Data models for charting
    public class ChartDataPoint
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }

    public ObservableCollection<ChartDataPoint> RecentLoginsData { get; set; } = new();
    public ObservableCollection<ChartDataPoint> NewUsersData { get; set; } = new();

    public AdminPanel()
    {
        InitializeComponent();
        BindingContext = this;
        ValidateAdminSession();
        LoadTotalUsers();
        LoadNewestUser();
        LoadRecentLogin();
        LoadRecentLoginsChart();
        LoadNewUsersChart();
    }
    private async void ValidateAdminSession()
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
            string firstname = await GetAdminFirstname(username);
            AdminNameLabel.Text = $"Welcome, {firstname ?? username}!";
        }
    }

    private async Task<string> GetAdminFirstname(string username)
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            var query = "SELECT firstname FROM user WHERE username = @username AND role = 'Admin'";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching admin name: {ex.Message}");
            return null;
        }
    }
    private async Task LoadTotalUsers()
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM user";
            using var cmd = new MySqlCommand(query, connection);
            var result = await cmd.ExecuteScalarAsync();

            // Safely update the UI with the result
            TotalUsersLabel.Text = result?.ToString() ?? "0";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading total users: {ex.Message}");
            TotalUsersLabel.Text = "Error";
        }
    }
    private async Task LoadNewestUser()
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            var query = @"
            SELECT firstname, lastname 
            FROM user 
            WHERE role = 'Student'
            ORDER BY user_id DESC 
            LIMIT 1";

            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string firstname = reader["firstname"].ToString();
                string lastname = reader["lastname"].ToString();
                NewUsersLabel.Text = $"{firstname} {lastname}";
            }
            else
            {
                NewUsersLabel.Text = "No new users";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading new user: {ex.Message}");
            NewUsersLabel.Text = "Error loading";
        }
    }
    private async Task LoadRecentLogin()
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            var query = @"
            SELECT u.firstname, u.lastname
            FROM login_history lh
            JOIN user u ON lh.user_id = u.user_id
            WHERE u.role = 'Student'
            ORDER BY lh.login_time DESC
            LIMIT 1";

            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                string firstname = reader["firstname"].ToString();
                string lastname = reader["lastname"].ToString();

                // Set just first name and last name
                RecentLoginLabel.Text = $"{firstname} {lastname}";
            }
            else
            {
                RecentLoginLabel.Text = "No student login found.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading recent student login: {ex.Message}");
            RecentLoginLabel.Text = "Error loading data.";
        }
    }

    // --- CHARTS LOGIC ---
    private async void LoadRecentLoginsChart()
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            var query = @"
                SELECT DATE(lh.login_time) AS LoginDate, COUNT(*) AS LoginCount
                FROM login_history lh
                JOIN user u ON lh.user_id = u.user_id
                WHERE u.role = 'Student'
                GROUP BY LoginDate
                ORDER BY LoginDate DESC
                LIMIT 5
            ";
            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            var days = new List<ChartDataPoint>();
            while (await reader.ReadAsync())
            {
                var date = ((DateTime)reader["LoginDate"]).ToString("MM-dd");
                var count = Convert.ToInt32(reader["LoginCount"]);
                days.Add(new ChartDataPoint { Date = date, Count = count });
            }
            // Reverse to show oldest to newest
            days.Reverse();
            RecentLoginsData.Clear();
            foreach (var item in days)
                RecentLoginsData.Add(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading recent logins chart: {ex.Message}");
        }
    }

    private async void LoadNewUsersChart()
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            var query = @"
                SELECT DATE(created_at) AS RegisterDate, COUNT(*) AS UserCount
                FROM user
                WHERE role = 'Student'
                GROUP BY RegisterDate
                ORDER BY RegisterDate DESC
                LIMIT 5
            ";
            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();
            var days = new List<ChartDataPoint>();
            while (await reader.ReadAsync())
            {
                var date = ((DateTime)reader["RegisterDate"]).ToString("MM-dd");
                var count = Convert.ToInt32(reader["UserCount"]);
                days.Add(new ChartDataPoint { Date = date, Count = count });
            }
            // Reverse to show oldest to newest
            days.Reverse();
            NewUsersData.Clear();
            foreach (var item in days)
                NewUsersData.Add(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading new users chart: {ex.Message}");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirm)
        {
            Preferences.Clear();
            await DisplayAlert("Logged Out", "You have been logged out.", "OK");
            LoadingOverlay.IsVisible = true;
            try
            {
                await Navigation.PushAsync(new LoginPage());
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
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
    private async void OnAdminProfileClicked(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;
        try
        {
            await Navigation.PushAsync(new AdminProfilePage());
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
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