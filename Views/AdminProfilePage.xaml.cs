using MySqlConnector;
using System.Data;

namespace MathMaster.Views
{
    public partial class AdminProfilePage : ContentPage
    {
        private string _username;

        public AdminProfilePage()
        {
            InitializeComponent();
            _username = Preferences.Get("Username", string.Empty);

            // Load admin profile data
            LoadAdminProfileData();
        }

        private async void LoadAdminProfileData()
        {
            await LoadAdminNameAndEmail();
        }

        private async Task LoadAdminNameAndEmail()
        {
            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = "SELECT firstname, lastname, email FROM user WHERE username = @username";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", _username);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string firstname = reader.GetString("firstname");
                                string lastname = reader.GetString("lastname");
                                string email = reader.GetString("email");

                                ProfileNameLabel.Text = $"{firstname} {lastname}";
                                ProfileEmailLabel.Text = email;
                            }
                            else
                            {
                                ProfileNameLabel.Text = "Admin User";
                                ProfileEmailLabel.Text = "Email not found";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProfileNameLabel.Text = "Admin User";
                ProfileEmailLabel.Text = "Error loading email";
                Console.WriteLine($"Error fetching admin info: {ex.Message}");
            }
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            LogoutConfirmPanel.IsVisible = true;
        }

        private async void OnLogoutClickedAsync(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "No");
            if (confirm)
            {
                Preferences.Clear(); // Clear session
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

        private async void OnAdminSettingsClicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;
            try
            {
                await Navigation.PushAsync(new AdminSettings());
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void OnManageUsersClicked(object sender, EventArgs e)
        {
            string username = Preferences.Get("Username", string.Empty);

            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Access Denied", "You must log in first before managing users.", "OK");
                await Navigation.PushAsync(new LoginPage()); // Redirect to login page
            }
            else
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

        private void OnConfirmLogoutCancel(object sender, EventArgs e)
        {
            LogoutConfirmPanel.IsVisible = false;
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent back navigation if the user is not logged in
            string username = Preferences.Get("Username", string.Empty);
            string role = Preferences.Get("Role", string.Empty);
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                return true; // Disable back button
            }
            return base.OnBackButtonPressed();
        }
    }
}