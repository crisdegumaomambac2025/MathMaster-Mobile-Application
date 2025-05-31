using MathMaster.Views;
using MySqlConnector;
using System;
using Microsoft.Maui.Controls;

namespace MathMaster.Views;

public partial class AdminSettings : ContentPage
{
    private string _currentUsername;

    public AdminSettings()
    {
        InitializeComponent();
        ValidateAdminSession();
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
            _currentUsername = username;
        }
    }

    private async Task<string> GetAdminFirstname(string username)
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            string query = "SELECT firstname FROM user WHERE username = @username AND role = 'Admin'";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void ShowErrorDialog(string title, string message)
    {
        ErrorTitle.Text = title;
        ErrorMessage.Text = message;
        ErrorPanel.IsVisible = true;
        ErrorPanel.FadeTo(1, 250, Easing.CubicIn);
    }

    private void ShowSuccessDialog(string title, string message)
    {
        SuccessTitle.Text = title;
        SuccessMessage.Text = message;
        SuccessDialog.IsVisible = true;
        SuccessDialog.FadeTo(1, 250, Easing.CubicIn);
    }

    private void ShowMessageDialog(string title, string message, bool isError = false)
    {
        DialogTitle.Text = title;
        DialogMessage.Text = message;
        DialogIcon.Source = isError ? "warning.png" : "successful.png";
        
        // Update colors based on message type
        if (isError)
        {
            MessageDialogBorder.Background = (LinearGradientBrush)Resources["ErrorDialogGradient"];
            DialogTitle.TextColor = Color.FromArgb("#D7263D");
            DialogButton.BackgroundColor = Color.FromArgb("#D7263D");
        }
        else
        {
            MessageDialogBorder.Background = (LinearGradientBrush)Resources["SuccessDialogGradient"];
            DialogTitle.TextColor = Color.FromArgb("#1CB142");
            DialogButton.BackgroundColor = Color.FromArgb("#1CB142");
        }
        
        MessageDialog.IsVisible = true;
        MessageDialog.FadeTo(1, 250, Easing.CubicIn);
    }

    private void OnDialogClose(object sender, EventArgs e)
    {
        MessageDialog.IsVisible = false;
    }

    private void OnErrorPanelClose(object sender, EventArgs e)
    {
        ErrorPanel.IsVisible = false;
    }

    private async void OnSuccessDialogContinue(object sender, EventArgs e)
    {
        SuccessDialog.IsVisible = false;
        await Navigation.PushAsync(new AdminPanel());
    }

    private async void OnChangeButtonClicked(object sender, EventArgs e)
    {
        string newPassword = NewPasswordEntry.Text;
        string currentPassword = CurrentPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(currentPassword))
        {
            ShowMessageDialog("Error", "Please fill in all fields. Please try again!", true);
            return;
        }

        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            string query = "SELECT password FROM user WHERE username = @username AND role = 'Admin'";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", _currentUsername);

            var result = await cmd.ExecuteScalarAsync();

            if (result == null || result.ToString() != currentPassword)
            {
                ShowMessageDialog("Error", "Current password is incorrect.", true);
                return;
            }

            query = "UPDATE user SET password = @newPassword WHERE username = @username AND role = 'Admin'";
            using var updateCmd = new MySqlCommand(query, connection);
            updateCmd.Parameters.AddWithValue("@newPassword", newPassword);
            updateCmd.Parameters.AddWithValue("@username", _currentUsername);

            int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                ShowMessageDialog("Success", "Password updated successfully.");
                NewPasswordEntry.Text = string.Empty;
                CurrentPasswordEntry.Text = string.Empty;
            }
            else
            {
                ShowMessageDialog("Error", "Failed to update password.", true);
            }
        }
        catch (Exception ex)
        {
            ShowMessageDialog("Error", $"An error occurred: {ex.Message}", true);
        }
    }
}