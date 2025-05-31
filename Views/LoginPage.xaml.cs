using MathMaster.View;
using MathMaster.Views;
using Microsoft.Maui.Controls;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace MathMaster;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    public class LoginInfo
    {
        public string IP { get; set; } = "Unknown";
        public string DeviceType { get; set; } = "Unknown";
        public string Location { get; set; } = "Unknown";
    }

    private async Task<LoginInfo> GetLoginInfo()
    {
        var loginInfo = new LoginInfo();

        try
        {
            using HttpClient client = new HttpClient();

            // Get IP
            loginInfo.IP = await client.GetStringAsync("https://api.ipify.org");

            // Get location info from IP
            var json = await client.GetStringAsync($"https://ipinfo.io/{loginInfo.IP}/json");
            var info = System.Text.Json.JsonDocument.Parse(json).RootElement;

            string city = info.TryGetProperty("city", out var cityElement) ? cityElement.GetString() : "Unknown City";
            string region = info.TryGetProperty("region", out var regionElement) ? regionElement.GetString() : "Unknown Region";
            string country = info.TryGetProperty("country", out var countryElement) ? countryElement.GetString() : "Unknown Country";

            loginInfo.Location = $"{city}, {region}, {country}";

            // Get device info
            loginInfo.DeviceType = DeviceInfo.Model + " / " + DeviceInfo.Platform;
        }
        catch
        {
            // Keep defaults
        }

        return loginInfo;
    }

    private async Task InsertLoginHistory(int userId, LoginInfo loginInfo)
    {
        try
        {
            using (var connection = DB_Connection.GetConnection())
            {
                await connection.OpenAsync();

                string query = @"INSERT INTO login_history (user_id, ip_address, device_type, location)
                             VALUES (@userId, @ip, @device, @location)";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@ip", loginInfo.IP);
                    cmd.Parameters.AddWithValue("@device", loginInfo.DeviceType);
                    cmd.Parameters.AddWithValue("@location", loginInfo.Location);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log login history: {ex.Message}");
        }
    }

    private User? _currentUser;

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorTitle.Text = "Missing Information";
            ErrorMessage.Text = "Please enter both username and password.";
            ErrorPanel.IsVisible = true;
            return;
        }

        LoadingOverlay.IsVisible = true; // Show loading spinner
        try
        {
            var user = await GetUser(username, password);

            if (user != null)
            {
                _currentUser = user;

                SessionManager.StartSession(
                    user.UserId,
                    user.Username,
                    user.FirstName + " " + user.LastName,
                    user.Username,
                    user.Role,
                    user.Email
                );

                Preferences.Set("Role", user.Role);
                Preferences.Set("Username", user.Username);

                var loginInfo = await GetLoginInfo();
                await InsertLoginHistory(user.UserId, loginInfo);

                SuccessTitle.Text = $"Welcome, {user.FirstName}!";
                SuccessMessage.Text = $"You have successfully logged in as {user.Username}.\nEnjoy using MathMaster!";
                SuccessDialog.IsVisible = true;
            }
            else
            {
                ErrorTitle.Text = "Login Failed";
                ErrorMessage.Text = "Invalid username or password.";
                ErrorPanel.IsVisible = true;
            }
        }
        finally
        {
            LoadingOverlay.IsVisible = false; // Hide loading spinner
        }
    }

    private async Task<User?> GetUser(string username, string password)
    {
        try
        {
            using (var connection = DB_Connection.GetConnection())
            {
                await connection.OpenAsync();
                string query = "SELECT user_id, firstname, lastname, username, role, email FROM user WHERE username = @username AND password = @password LIMIT 1";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserId = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                Username = reader.GetString(3),
                                Role = reader.GetString(4),
                                Email = reader.GetString(5)
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Database error: {ex.Message}", "OK");
        }
        return null;
    }

    private async void OnSignUpTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private async void OnSuccessDialogContinue(object sender, EventArgs e)
    {
        SuccessDialog.IsVisible = false;

        if (_currentUser != null)
        {
            if (_currentUser.Role == "Admin")
                await Navigation.PushAsync(new AdminPanel());
            else if (_currentUser.Role == "Student")
                await Navigation.PushAsync(new HomePage());
            else
                await Navigation.PushAsync(new MainPage());
        }
    }

    private void OnErrorPanelClose(object sender, EventArgs e)
    {
        ErrorPanel.IsVisible = false;
    }
}

public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
}