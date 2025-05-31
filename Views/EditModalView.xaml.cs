using static MathMaster.Views.ManageUsersPage;
using MySqlConnector;

namespace MathMaster.Views;

public partial class EditModalView : ContentPage
{
    public UserDisplayModel User { get; set; }

    public EditModalView(UserDisplayModel user)
    {
        InitializeComponent();
        User = user;
        BindingContext = this;
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

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(User.FirstName) ||
            string.IsNullOrWhiteSpace(User.LastName) ||
            string.IsNullOrWhiteSpace(User.Email))
        {
            ShowErrorDialog("Validation Error", "All fields are required.");
            return;
        }

        try
        {
            await UpdateUserInDatabase();
            ShowSuccessDialog("Success!", "User updated successfully.");
        }
        catch (Exception ex)
        {
            ShowErrorDialog("Error", $"Failed to save changes: {ex.Message}");
        }
    }

    private async Task UpdateUserInDatabase()
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();

            string query = "UPDATE user SET firstname = @firstname, lastname = @lastname, email = @email WHERE username = @username";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@firstname", User.FirstName);
            cmd.Parameters.AddWithValue("@lastname", User.LastName);
            cmd.Parameters.AddWithValue("@email", User.Email);
            cmd.Parameters.AddWithValue("@username", User.Username);

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating user.", ex);
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}