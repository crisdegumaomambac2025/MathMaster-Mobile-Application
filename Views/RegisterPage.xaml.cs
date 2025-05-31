using System;
using System.Data;
using MySqlConnector;
using Microsoft.Maui.Controls;
using MathMaster.Views;

namespace MathMaster.View;

public partial class RegisterPage : ContentPage
{
    public static UserRegistrationData UserData { get; set; } = new UserRegistrationData();

    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        string firstname = FirstNameEntry.Text;
        string lastname = LastNameEntry.Text;

        // Basic Validation
        if (string.IsNullOrWhiteSpace(firstname) || string.IsNullOrWhiteSpace(lastname))
        {
            ShowError("Missing Information", "First Name and Last Name are required.");
            return;
        }

        // Store user data in the shared model
        UserData.FirstName = firstname;
        UserData.LastName = lastname;

        // Navigate to RegisterPage2 (Email)
        await Navigation.PushAsync(new RegisterPage2());
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // Navigate to LoginPage
        await Navigation.PushAsync(new LoginPage());
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
}

// You should have this class somewhere in your project:
public class UserRegistrationData
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}