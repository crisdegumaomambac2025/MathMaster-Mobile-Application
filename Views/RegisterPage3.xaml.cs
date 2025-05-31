using System;
using System.Data;
using MySqlConnector;
using Microsoft.Maui.Controls;
using MathMaster.Views;
using MathMaster.View;

namespace MathMaster.Views;

public partial class RegisterPage3 : ContentPage
{
    public RegisterPage3()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        // Basic Validation
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Missing Information", "Username and Password are required.");
            return;
        }

        // Username Validation (Length and Existence)
        if (username.Length < 6)
        {
            ShowError("Invalid Username", "Username must be at least 6 characters long.");
            return;
        }

        // Check if the username already exists in the database
        try
        {
            using (var connection = DB_Connection.GetConnection())
            {
                await connection.OpenAsync();

                string checkQuery = "SELECT COUNT(*) FROM user WHERE username = @username";
                using (var checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@username", username);

                    int userCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                    if (userCount > 0)
                    {
                        ShowError("Username Exists", "Username already exists. Please choose a different one.");
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("Database Error", "Error checking username: " + ex.Message);
            return;
        }

        // Password Validation (Length)
        if (password.Length < 6)
        {
            ShowError("Invalid Password", "Password must be at least 6 characters long.");
            return;
        }

        // Store username and password in the shared model
        RegisterPage.UserData.Username = username;
        RegisterPage.UserData.Password = password;

        // Save to database
        try
        {
            using (var connection = DB_Connection.GetConnection())
            {
                await connection.OpenAsync();

                string query = "INSERT INTO user (firstname, lastname, username, password, email, role) " +
                               "VALUES (@firstname, @lastname, @username, @password, @email, 'Student')";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@firstname", RegisterPage.UserData.FirstName);
                    command.Parameters.AddWithValue("@lastname", RegisterPage.UserData.LastName);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@email", RegisterPage.UserData.Email);

                    int result = await command.ExecuteNonQueryAsync();
                    if (result > 0)
                    {
                        // Show custom success dialog instead of DisplayAlert
                        SuccessTitle.Text = "Registration Successful!";
                        SuccessMessage.Text = "Your account has been created. You can now log in!";
                        SuccessDialog.IsVisible = true;
                    }
                    else
                    {
                        ShowError("Registration Failed", "Registration failed. Please try again.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowError("Database Error", "Error: " + ex.Message);
        }
    }

    private void ShowError(string title, string message)
    {
        ErrorTitle.Text = title;
        ErrorMessage.Text = message;
        ErrorPanel.IsVisible = true;
    }

    private void OnErrorPanelClose(object sender, EventArgs e)
    {
        ErrorPanel.IsVisible = false;
    }

    private async void OnSuccessDialogContinue(object sender, EventArgs e)
    {
        SuccessDialog.IsVisible = false;
        await Navigation.PushAsync(new LoginPage());
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage2());
    }
}