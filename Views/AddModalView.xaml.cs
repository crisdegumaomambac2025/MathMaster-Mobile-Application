using Microsoft.Maui.Storage;
using MySqlConnector;

namespace MathMaster.Views;

public partial class AddModalView : ContentPage
{
    public AddModalView()
    {
        InitializeComponent();
        ValidateAdminSession();
    }

    private async void ValidateAdminSession()
    {
        string role = Preferences.Get("Role", string.Empty);

        if (role != "Admin")
        {
            await DisplayAlert("Unauthorized", "Only admins can access this page.", "OK");
            await Navigation.PopModalAsync();
        }
    }

    private void ShowSuccessDialog(string title, string message)
    {
        SuccessTitle.Text = title;
        SuccessMessage.Text = message;
        SuccessDialog.IsVisible = true;
        SuccessDialog.FadeTo(1, 250, Easing.CubicIn);
    }

    private void ShowErrorDialog(string title, string message)
    {
        ErrorTitle.Text = title;
        ErrorMessage.Text = message;
        ErrorPanel.IsVisible = true;
        ErrorPanel.FadeTo(1, 250, Easing.CubicIn);
    }

    private async void OnSuccessDialogContinue(object sender, EventArgs e)
    {
        await SuccessDialog.FadeTo(0, 250, Easing.CubicOut);
        SuccessDialog.IsVisible = false;
        await Navigation.PopModalAsync();
    }

    private async void OnErrorPanelClose(object sender, EventArgs e)
    {
        await ErrorPanel.FadeTo(0, 250, Easing.CubicOut);
        ErrorPanel.IsVisible = false;
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        string firstName = FirstNameEntry.Text?.Trim();
        string lastName = LastNameEntry.Text?.Trim();
        string username = UsernameEntry.Text?.Trim();
        string password = PasswordEntry.Text?.Trim();
        string email = EmailEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(email))
        {
            ShowErrorDialog("Validation Error", "All fields are required.");
            return;
        }

        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            // Check if username already exists
            string checkQuery = "SELECT COUNT(*) FROM user WHERE username = @username";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@username", username);
            long count = Convert.ToInt64(await checkCmd.ExecuteScalarAsync());

            if (count > 0)
            {
                ShowErrorDialog("Error", "Username already exists. Please choose a different username.");
                return;
            }

            string insertQuery = @"INSERT INTO user (firstname, lastname, username, password, role, email)
                                   VALUES (@firstname, @lastname, @username, @password, 'Student', @email)";
            using var cmd = new MySqlCommand(insertQuery, connection);
            cmd.Parameters.AddWithValue("@firstname", firstName);
            cmd.Parameters.AddWithValue("@lastname", lastName);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password); // In production, use hashing!
            cmd.Parameters.AddWithValue("@email", email);

            await cmd.ExecuteNonQueryAsync();
            
            // Clear the form
            FirstNameEntry.Text = string.Empty;
            LastNameEntry.Text = string.Empty;
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;

            ShowSuccessDialog("Success!", "User added successfully.");
        }
        catch (Exception ex)
        {
            ShowErrorDialog("Error", $"Failed to add user: {ex.Message}");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}