using MathMaster.Views;
using MySqlConnector;
using System;
using Microsoft.Maui.Controls;

namespace MathMaster.Views;

public partial class SettingsPage : ContentPage
{
    private string _currentUsername;

    public SettingsPage()
    {
        InitializeComponent();
        ValidateStudentSession();
        // Set initial volume from preferences or default
        double savedVolume = Preferences.Get("MusicVolume", 1.0);
        VolumeSlider.Value = savedVolume;
        MathMaster.Services.BackgroundMusicService.Instance.Volume = savedVolume;
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
            _currentUsername = username;
        }
    }

    private void ShowErrorDialog(string title, string message)
    {
        ErrorTitle.Text = title;
        ErrorMessage.Text = message;
        ErrorPanel.IsVisible = true;
    }

    private void ShowSuccessDialog(string title, string message)
    {
        SuccessTitle.Text = title;
        SuccessMessage.Text = message;
        SuccessDialog.IsVisible = true;
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
        await Navigation.PushAsync(new HomePage());
    }

    private async void OnChangeButtonClicked(object sender, EventArgs e)
    {
        string newPassword = NewPasswordEntry.Text;
        string currentPassword = CurrentPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(currentPassword))
        {
            DialogTitle.Text = "Error";
            DialogMessage.Text = "Please fill in all fields. Please try again!";
            DialogIcon.Source = "warning.png";
            MessageDialog.IsVisible = true;
            return;
        }

        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            string query = "SELECT password FROM user WHERE username = @username";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", _currentUsername);

            var result = await cmd.ExecuteScalarAsync();

            if (result == null || result.ToString() != currentPassword)
            {
                DialogTitle.Text = "Error";
                DialogMessage.Text = "Current password is incorrect.";
                DialogIcon.Source = "warning.png";
                MessageDialog.IsVisible = true;
                return;
            }

            query = "UPDATE user SET password = @newPassword WHERE username = @username";
            using var updateCmd = new MySqlCommand(query, connection);
            updateCmd.Parameters.AddWithValue("@newPassword", newPassword);
            updateCmd.Parameters.AddWithValue("@username", _currentUsername);

            int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                DialogTitle.Text = "Success";
                DialogMessage.Text = "Password updated successfully.";
                DialogIcon.Source = "successful.png";
                MessageDialog.IsVisible = true;
                NewPasswordEntry.Text = string.Empty;
                CurrentPasswordEntry.Text = string.Empty;
            }
            else
            {
                DialogTitle.Text = "Error";
                DialogMessage.Text = "Failed to update password.";
                DialogIcon.Source = "warning.png";
                MessageDialog.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            DialogTitle.Text = "Error";
            DialogMessage.Text = $"An error occurred: {ex.Message}";
            DialogIcon.Source = "warning.png";
            MessageDialog.IsVisible = true;
        }
    }

    private void ShowAccountDeletionDialog()
    {
        AccountDeletionDialog.IsVisible = true;
    }

    private void OnAccountDeletionCancel(object sender, EventArgs e)
    {
        AccountDeletionDialog.IsVisible = false;
        DialogTitle.Text = "Cancelled";
        DialogMessage.Text = "Account deletion was cancelled.";
        DialogIcon.Source = "warning.png";
        MessageDialog.IsVisible = true;
    }

    private async void OnAccountDeletionConfirm(object sender, EventArgs e)
    {
        AccountDeletionDialog.IsVisible = false;

        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            // Delete related records from login_history
            string deleteLoginHistoryQuery = "DELETE FROM login_history WHERE user_id = (SELECT user_id FROM user WHERE username = @username)";
            using var deleteLoginHistoryCmd = new MySqlCommand(deleteLoginHistoryQuery, connection);
            deleteLoginHistoryCmd.Parameters.AddWithValue("@username", _currentUsername);
            await deleteLoginHistoryCmd.ExecuteNonQueryAsync();

            // Delete related records from leaderboard
            string deleteLeaderboardQuery = "DELETE FROM leaderboard WHERE user_id = (SELECT user_id FROM user WHERE username = @username)";
            using var deleteLeaderboardCmd = new MySqlCommand(deleteLeaderboardQuery, connection);
            deleteLeaderboardCmd.Parameters.AddWithValue("@username", _currentUsername);
            await deleteLeaderboardCmd.ExecuteNonQueryAsync();

            // Delete the user account
            string deleteUserQuery = "DELETE FROM user WHERE username = @username";
            using var deleteUserCmd = new MySqlCommand(deleteUserQuery, connection);
            deleteUserCmd.Parameters.AddWithValue("@username", _currentUsername);

            int rowsAffected = await deleteUserCmd.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                Preferences.Clear();
                DialogTitle.Text = "Success";
                DialogMessage.Text = "Your account and all related information have been deleted.";
                DialogIcon.Source = "successful.png";
                MessageDialog.IsVisible = true;
                await Navigation.PushAsync(new LoginPage());
            }
            else
            {
                DialogTitle.Text = "Error";
                DialogMessage.Text = "Failed to delete account.";
                DialogIcon.Source = "warning.png";
                MessageDialog.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            DialogTitle.Text = "Error";
            DialogMessage.Text = $"An error occurred: {ex.Message}";
            DialogIcon.Source = "warning.png";
            MessageDialog.IsVisible = true;
        }
    }

    private async void OnDeleteAccountClicked(object sender, EventArgs e)
    {
        ShowAccountDeletionDialog();
    }

    private void OnVolumeSliderChanged(object sender, ValueChangedEventArgs e)
    {
        double newVolume = e.NewValue;
        MathMaster.Services.BackgroundMusicService.Instance.Volume = newVolume;
        Preferences.Set("MusicVolume", newVolume);
    }
}