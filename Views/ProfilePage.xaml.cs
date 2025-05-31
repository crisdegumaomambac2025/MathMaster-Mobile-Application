using MySqlConnector;
using System.Data;
using MathMaster.Services;

namespace MathMaster.Views
{
    public partial class ProfilePage : ContentPage
    {
        private string _username;
        private int _userId = -1;
        private int _userRank = -1;
        private string _userRankImage = string.Empty;

        public ProfilePage(string firstname, string lastname)
        {
            InitializeComponent();
            ProfileNameLabel.Text = $"{firstname} {lastname}";
            _username = Preferences.Get("Username", string.Empty);

            // Load profile data
            LoadProfileData();
        }

        public void UpdateProfileName(string firstname, string lastname)
        {
            ProfileNameLabel.Text = $"{firstname} {lastname}";
        }

        private async void LoadProfileData()
        {
            await LoadEmailAndUserId();
            if (_userId > 0)
            {
                await LoadQuizStats();
                await LoadUserRank();
                DisplayUserRank();
            }
            else
            {
                QuizzesCountLabel.Text = "0";
                PointsLabel.Text = "0";
                Console.WriteLine("User ID not found, cannot load quiz stats.");
            }
        }

        private async Task LoadEmailAndUserId()
        {
            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = "SELECT user_id, email FROM user WHERE username = @username";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", _username);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                ProfileEmailLabel.Text = reader.GetString("email");
                                _userId = reader.GetInt32("user_id");
                                Console.WriteLine($"Fetched user_id: {_userId}");
                            }
                            else
                            {
                                ProfileEmailLabel.Text = "Email not found";
                                Console.WriteLine("User not found in database.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProfileEmailLabel.Text = "Error loading email";
                Console.WriteLine($"Error fetching email: {ex.Message}");
            }
        }

        private async Task LoadQuizStats()
        {
            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    await connection.OpenAsync();
                    // Get total unique quizzes, total points, and total stars from leaderboard
                    string query = @"
                        SELECT 
                            COUNT(DISTINCT quiz_name) AS quiz_count, 
                            COALESCE(SUM(CAST(score AS SIGNED)),0) AS total_points,
                            COALESCE(SUM(star),0) AS total_stars
                        FROM leaderboard
                        WHERE user_id = @user_id";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@user_id", _userId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int quizCount = reader.IsDBNull("quiz_count") ? 0 : reader.GetInt32("quiz_count");
                                int totalPoints = reader.IsDBNull("total_points") ? 0 : reader.GetInt32("total_points");
                                int totalStars = reader.IsDBNull("total_stars") ? 0 : reader.GetInt32("total_stars");
                                QuizzesCountLabel.Text = quizCount.ToString();
                                PointsLabel.Text = totalPoints.ToString();
                                StarsLabel.Text = totalStars.ToString();
                                Console.WriteLine($"Quiz count: {quizCount}, Total points: {totalPoints}, Total stars: {totalStars}");
                            }
                            else
                            {
                                QuizzesCountLabel.Text = "0";
                                PointsLabel.Text = "0";
                                StarsLabel.Text = "0";
                                Console.WriteLine("No leaderboard results found for this user.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                QuizzesCountLabel.Text = "0";
                PointsLabel.Text = "0";
                StarsLabel.Text = "0";
                Console.WriteLine($"Error fetching quiz stats: {ex.Message}");
            }
        }

        private async Task LoadUserRank()
        {
            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = @"
                        SELECT u.user_id
                        FROM leaderboard l
                        INNER JOIN user u ON l.user_id = u.user_id
                        GROUP BY l.user_id
                        ORDER BY SUM(CAST(l.score AS SIGNED)) DESC
                    ";
                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        int rank = 1;
                        while (await reader.ReadAsync())
                        {
                            int userId = reader.GetInt32("user_id");
                            if (userId == _userId)
                            {
                                _userRank = rank;
                                if (rank == 1) _userRankImage = "first.png";
                                else if (rank == 2) _userRankImage = "second.png";
                                else if (rank == 3) _userRankImage = "third.png";
                                break;
                            }
                            rank++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user rank: {ex.Message}");
                _userRank = -1;
                _userRankImage = string.Empty;
            }
        }

        private void DisplayUserRank()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (_userRank > 0)
                {
                    if (!string.IsNullOrEmpty(_userRankImage))
                    {
                        // Show image for 1st, 2nd, 3rd
                        if (this.FindByName("UserRankImage") is Image rankImg)
                        {
                            rankImg.Source = _userRankImage;
                            rankImg.IsVisible = true;
                        }
                        if (this.FindByName("UserRankLabel") is Label rankLbl)
                        {
                            rankLbl.IsVisible = false;
                        }
                    }
                    else
                    {
                        // Show number for other ranks
                        if (this.FindByName("UserRankLabel") is Label rankLbl)
                        {
                            rankLbl.Text = $"Rank: {_userRank}";
                            rankLbl.IsVisible = true;
                        }
                        if (this.FindByName("UserRankImage") is Image rankImg)
                        {
                            rankImg.IsVisible = false;
                        }
                    }
                }
                else
                {
                    if (this.FindByName("UserRankLabel") is Label rankLbl)
                    {
                        rankLbl.Text = "Rank: N/A";
                        rankLbl.IsVisible = true;
                    }
                    if (this.FindByName("UserRankImage") is Image rankImg)
                    {
                        rankImg.IsVisible = false;
                    }
                }
            });
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            LogoutConfirmPanel.IsVisible = true;
        }

        private async void OnConfirmLogoutYes(object sender, EventArgs e)
        {
            LogoutConfirmPanel.IsVisible = false;
            Preferences.Clear(); // Clear session
            Preferences.Remove("Role"); // Explicitly remove role in case it's not cleared
            BackgroundMusicService.Instance.Stop(); // Stop music on logout
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

        private void OnConfirmLogoutCancel(object sender, EventArgs e)
        {
            LogoutConfirmPanel.IsVisible = false;
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            string username = Preferences.Get("Username", string.Empty);
            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Access Denied", "You must log in first before accessing your profile.", "OK");
                await Navigation.PushAsync(new LoginPage()); // Redirect to login page
            }
            else
            {
                // Fetch the student's firstname and lastname from the database
                var studentInfo = await GetStudentInfo(username);
                if (studentInfo != null)
                {
                    string firstname = studentInfo.Item1;
                    string lastname = studentInfo.Item2;
                    // Pass the firstname and lastname to SettingsPage
                    LoadingOverlay.IsVisible = true;
                    try
                    {
                        await Navigation.PushAsync(new SettingsPage());
                    }
                    finally
                    {
                        LoadingOverlay.IsVisible = false;
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Student data could not be fetched.", "OK");
                }
            }
        }

        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;
            try
            {
                await Navigation.PushAsync(new EditProfile());
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async Task<Tuple<string, string>> GetStudentInfo(string username)
        {
            string firstname = string.Empty;
            string lastname = string.Empty;
            try
            {
                using (var connection = DB_Connection.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = "SELECT firstname, lastname FROM user WHERE username = @username";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                firstname = reader.GetString("firstname");
                                lastname = reader.GetString("lastname");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching student info: {ex.Message}");
            }

            return string.IsNullOrEmpty(firstname) ? null : new Tuple<string, string>(firstname, lastname);
        }
    }
}