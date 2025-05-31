using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySqlConnector;
using Microsoft.Maui.Controls;
using MathMaster.Views;
using MathMaster.View;

namespace MathMaster.Views
{
    public partial class RegisterPage2 : ContentPage
    {
        public RegisterPage2()
        {
            InitializeComponent();

            string firstName = RegisterPage.UserData.FirstName;
            string lastName = RegisterPage.UserData.LastName;

            EmailEntry.Placeholder = "Enter Email";
        }

        private async void OnNextClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;

            // Basic Validation
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Missing Information", "Email is required.");
                return;
            }

            // Email format validation
            if (!IsValidEmailFormat(email))
            {
                ShowError("Invalid Email", "Please enter a valid email address.");
                return;
            }

            // If the user has manually typed an email, validate that it's not already taken
            bool emailExists = await CheckEmailExistsAsync(email);
            if (emailExists)
            {
                ShowError("Email Exists", "This email is already registered. Please choose another one.");
                return;
            }

            // If the email is valid, store it in the shared model
            RegisterPage.UserData.Email = email;

            // Navigate to RegisterPage3 (Username and Password)
            await Navigation.PushAsync(new RegisterPage3());
        }

        private bool IsValidEmailFormat(string email)
        {
            var emailRegex = new Regex(@"^[^@]+@[^@]+\.[^@]+$");
            if (!emailRegex.IsMatch(email))
            {
                return false;
            }

            string[] emailParts = email.Split('@');
            if (emailParts.Length != 2)
            {
                return false;
            }

            string domain = emailParts[1].ToLower();

            if (domain == "gmail.com")
            {
                return true;
            }

            if (domain.EndsWith("gmail.com"))
            {
                return true;
            }

            return false;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    await connection.OpenAsync();

                    string query = "SELECT COUNT(*) FROM `user` WHERE `email` = @Email";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        var result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Database Error", $"An error occurred while checking the email: {ex.Message}");
                return false;
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
    }
}