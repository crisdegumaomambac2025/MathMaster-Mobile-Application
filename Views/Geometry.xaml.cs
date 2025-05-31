using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MySqlConnector;
using System.Timers;
using MathMaster.Services;

namespace MathMaster.Views;

public partial class Geometry : ContentPage, IResumableMusicPage
{
    private readonly List<(string QuestionText, double Answer)> _questions = new();
    private readonly List<double> _userAnswers = new();
    private int _currentQuestionIndex = 0;
    private readonly Random _random = new();
    private const string QuizName = "Geometry";

    private readonly Dictionary<string, double> _currentOptions = new();
    private string? _selectedOption = null;
    private readonly string[] _optionLabels = { "A", "B", "C", "D" };
    private TaskCompletionSource<bool>? _confirmDialogTcs;

    private System.Timers.Timer? _questionTimer;
    private const int QuestionTimeLimitSeconds = 60;
    private DateTime _questionStartTime;
    private bool _answeredThisQuestion = false;

    // --- Overlay Dialog Helpers ---
    private void ShowErrorDialog(string title, string message)
    {
        ErrorDialogTitle.Text = title;
        ErrorDialogMessage.Text = message;
        ErrorDialog.IsVisible = true;
    }

    private void ShowSuccessDialog(string title, string message)
    {
        SuccessDialogTitle.Text = title;
        SuccessDialogMessage.Text = message;
        SuccessDialog.IsVisible = true;
    }

    private void ShowInfoDialog(string title, string message)
    {
        InfoDialogTitle.Text = title;
        InfoDialogMessage.Text = message;
        InfoDialog.IsVisible = true;
    }

    private Task<bool> ShowConfirmDialog(string title, string message, string yesText = "Yes", string noText = "No")
    {
        ConfirmDialogTitle.Text = title;
        ConfirmDialogMessage.Text = message;
        ConfirmDialogYesButton.Text = yesText;
        ConfirmDialogNoButton.Text = noText;
        ConfirmDialog.IsVisible = true;
        _confirmDialogTcs = new TaskCompletionSource<bool>();
        return _confirmDialogTcs.Task;
    }

    private void OnErrorDialogClose(object sender, EventArgs e) => ErrorDialog.IsVisible = false;
    private void OnSuccessDialogClose(object sender, EventArgs e) => SuccessDialog.IsVisible = false;
    private void OnInfoDialogClose(object sender, EventArgs e) => InfoDialog.IsVisible = false;
    private void OnConfirmDialogYes(object sender, EventArgs e)
    {
        ConfirmDialog.IsVisible = false;
        _confirmDialogTcs?.TrySetResult(true);
    }
    private void OnConfirmDialogNo(object sender, EventArgs e)
    {
        ConfirmDialog.IsVisible = false;
        _confirmDialogTcs?.TrySetResult(false);
    }

    public Geometry()
    {
        InitializeComponent();
        ValidateStudentSession();
    }
    private async void ValidateStudentSession()
    {
        string role = Preferences.Get("Role", string.Empty);
        string username = Preferences.Get("Username", string.Empty);

        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || role != "Student")
        {
            await DisplayAlert("⛔ Unauthorized", "Only students can access this quiz.", "OK");
            await Navigation.PushAsync(new LoginPage());
        }
        else if (!SessionManager.IsLoggedIn)
        {
            await DisplayAlert("⚠️ Session Error", "You must be logged in to take the quiz.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            bool alreadyTaken = await HasStudentTakenQuiz(SessionManager.Username);
            if (alreadyTaken)
            {
                await DisplayAlert("🎉 Completed", "You've already taken this quiz.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                ShowReadyDialog();
            }
        }
    }
    private async Task<bool> HasStudentTakenQuiz(string username)
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM leaderboard 
                WHERE username = @username AND quiz_name = @quiz_name";
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@quiz_name", QuizName);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    private void ShowReadyDialog()
    {
        ReadyDialog.IsVisible = true;
    }

    private void OnReadyDialogYesClicked(object sender, EventArgs e)
    {
        ReadyDialog.IsVisible = false;
        GenerateQuestions();
        ShowCurrentQuestion(animated: false);
    }

    private async void OnReadyDialogNoClicked(object sender, EventArgs e)
    {
        ReadyDialog.IsVisible = false;
        await Navigation.PopAsync();
    }
    private void GenerateQuestions()
    {
        _questions.Clear();

        var allQuestions = new List<(string QuestionText, double CorrectAnswer, List<string> Options)>
    {
        // Keeping the original first 10 questions
        ("In an isosceles triangle, the equal sides are 10 cm each, and the base is 12 cm. Find the area of the triangle.",
            24, new List<string>{ "24", "30", "35", "20" }),

        ("Find the sum of the interior angles of a 14-sided polygon.",
            2160, new List<string>{ "2160", "2000", "2200", "2300" }),

        ("In triangle ABC, the angle bisector of angle A divides side BC into segments of 4 cm and 6 cm. If the area of triangle ABC is 48 square cm, find the height from vertex A.",
            8, new List<string>{ "8", "10", "9", "7" }),

        ("In circle O, m∠ABO = 61º. Find m∠ACB.",
            61, new List<string>{ "61", "58", "30.5", "29" }),

        ("ΔABC is similar to ΔDEF. m∠BAC = (x² - 5x)º, m∠BCA = (4x - 5)º and m∠EDF = (4x + 36)º. Find m∠F.",
            43, new List<string>{ "43", "36", "30", "12" }),

        ("In ΔABC, AB = AD = DC and m∠BAD = 46º. Find m∠BCA.",
            67, new List<string>{ "33.5", "48.5", "46", "67" }),

        ("In triangle ABC, the sides are in the ratio 3:4:5, and the area is 120 square units. Find the length of the shortest side.",
            24, new List<string>{ "24", "30", "36", "15" }),

        ("ΔABC is similar to ΔDEF. Find BC.",
            7, new List<string>{ "6", "7", "18", "21" }),

        ("Given ΔABC, midpoints M and N, MN = 5x - 6 and CB = 48, find x.",
            6, new List<string>{ "3.6", "8.6", "6", "10.8" }),

        ("In right ΔABC, the right angle is located at vertex C. If sin(A) = 3x - 0.5 and cos(B) = 2x - 0.1, find x.",
            0.5, new List<string>{ "0.5", "0.4", "9.06", "18.12" })
    };

        // Randomly shuffle the 20 questions and take 10 random ones to add
        var selected = allQuestions.OrderBy(q => _random.Next()).Take(20).ToList();

        foreach (var (questionText, correctAnswer, options) in selected)
        {
            _questions.Add((questionText, correctAnswer));
        }
    }
    private async void ShowCurrentQuestion(bool animated = true)
    {
        if (_currentQuestionIndex >= _questions.Count)
        {
            ShowResult();
            return;
        }

        if (animated)
            await QuizCard.FadeTo(0, 250, Easing.CubicIn);

        var current = _questions[_currentQuestionIndex];
        QuestionLabel.Text = $"Q{_currentQuestionIndex + 1}: {current.QuestionText}";
        _selectedOption = null;

        _currentOptions.Clear();
        List<double> optionValues = GenerateOptions(current.Answer);
        optionValues.Shuffle();

        OptionsGrid.Children.Clear();
        for (int i = 0; i < 4; i++)
        {
            string label = _optionLabels[i];
            double value = optionValues[i];
            _currentOptions[label] = value;

            var btn = new Button
            {
                Text = $"{label}. {Math.Round(value, 2)}",
                BackgroundColor = Color.FromArgb("#2E2E2E"),
                TextColor = Colors.White,
                FontSize = 16,
                HeightRequest = 45,
                CornerRadius = 10
            };

            string capturedLabel = label;
            btn.Clicked += (s, e) => OnOptionSelected(capturedLabel);

            int row = i % 2;
            int col = i / 2;
            OptionsGrid.Add(btn, col, row);
        }

        // Reset progress bar
        QuestionProgressBar.Progress = 0;
        // Start timer for this question
        StartQuestionTimer();
        _answeredThisQuestion = false;

        // Set the appropriate image for the current question
        SetQuestionImage(current.QuestionText);

        if (animated)
            await QuizCard.FadeTo(1, 250, Easing.CubicOut);
    }

    private void SetQuestionImage(string questionText)
    {
        string imageUrl = string.Empty;

        // Check for each specific question and assign the corresponding image URL
        if (questionText == "ΔABC is similar to ΔDEF. Find BC.")
            imageUrl = "geometry_8.jpg"; // Image for "ΔABC is similar to ΔDEF. Find BC."
        else if (questionText == "In ΔABC, AB = AD = DC and m∠BAD = 46º. Find m∠BCA.")
            imageUrl = "geometry_6.jpg"; // Image for "In ΔABC, AB = AD = DC and m∠BAD = 46º. Find m∠BCA."
        else if (questionText == "ΔABC is similar to ΔDEF. m∠BAC = (x² - 5x)º, m∠BCA = (4x - 5)º and m∠EDF = (4x + 36)º. Find m∠F.")
            imageUrl = "geometry_5.jpg"; // Image for "ΔABC is similar to ΔDEF. m∠BAC = (x² - 5x)º, m∠BCA = (4x - 5)º and m∠EDF = (4x + 36)º. Find m∠F."
        else if (questionText == "In circle O, m∠ABO = 61º. Find m∠ACB.")
            imageUrl = "geometry_4.jpg"; // Image for "In circle O, m∠ABO = 61º. Find m∠ACB."

        // If an image URL is found, set it as the source
        if (!string.IsNullOrEmpty(imageUrl))
        {
            QuestionImage.Source = ImageSource.FromFile(imageUrl);
        }
        else
        {
            // If no image is found for the question, clear the image
            QuestionImage.Source = null;
        }
    }
    private List<double> GenerateOptions(double correctAnswer)
    {
        HashSet<double> options = new() { Math.Round(correctAnswer, 2) };
        while (options.Count < 4)
        {
            double distractor = correctAnswer + _random.Next(-5, 6);
            if (Math.Abs(distractor - correctAnswer) < 0.1 || options.Contains(distractor))
                continue;

            options.Add(distractor);
        }
        return options.ToList();
    }
    private void OnOptionSelected(string label)
    {
        _selectedOption = label;

        foreach (var child in OptionsGrid.Children)
        {
            if (child is Button btn)
            {
                btn.BackgroundColor = btn.Text.StartsWith($"{label}.")
                    ? Color.FromArgb("#FFA000")
                    : Color.FromArgb("#2E2E2E");
            }
        }
    }
    private void StartQuestionTimer()
    {
        _questionTimer?.Stop();
        _questionTimer?.Dispose();

        _questionStartTime = DateTime.Now;
        _questionTimer = new System.Timers.Timer(100); // 100ms interval
        _questionTimer.Elapsed += OnQuestionTimerElapsed;
        _questionTimer.Start();
    }

    private void OnQuestionTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        var elapsed = (DateTime.Now - _questionStartTime).TotalSeconds;
        double progress = Math.Min(elapsed / QuestionTimeLimitSeconds, 1.0);

        // Update progress bar on UI thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            QuestionProgressBar.Progress = progress;
        });

        if (elapsed >= QuestionTimeLimitSeconds)
        {
            _questionTimer?.Stop();
            _questionTimer?.Dispose();

            // If not answered, auto-move to next question
            if (!_answeredThisQuestion)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _userAnswers.Add(double.NaN); // Use NaN to indicate 'not answered'
                    _currentQuestionIndex++;
                    _selectedOption = null;
                    ShowCurrentQuestion();
                });
            }
        }
    }

    private async void OnNextQuestionClicked(object sender, EventArgs e)
    {
        await NextButton.ScaleTo(0.95, 80, Easing.CubicInOut);
        await NextButton.ScaleTo(1.0, 80, Easing.CubicInOut);

        if (string.IsNullOrEmpty(_selectedOption) || !_currentOptions.ContainsKey(_selectedOption))
        {
            ShowErrorDialog("Choose an Answer", "Please select one of the options.");
            while (ErrorDialog.IsVisible) await Task.Delay(100);
            return;
        }

        _answeredThisQuestion = true;
        _questionTimer?.Stop();
        _questionTimer?.Dispose();

        double selectedValue = _currentOptions[_selectedOption];
        if (_userAnswers.Count > _currentQuestionIndex)
            _userAnswers[_currentQuestionIndex] = selectedValue;
        else
            _userAnswers.Add(selectedValue);

        _currentQuestionIndex++;
        _selectedOption = null;
        ShowCurrentQuestion();
    }

    private async void ShowReviewModal()
    {
        ReviewStack.Children.Clear();

        for (int i = 0; i < _questions.Count; i++)
        {
            var q = _questions[i];
            double userAns = i < _userAnswers.Count ? _userAnswers[i] : double.NaN;
            bool isCorrect = Math.Abs(q.Answer - userAns) < 0.01;

            // Icon for correct/incorrect
            var icon = new Image
            {
                Source = isCorrect ? "check_circle.png" : "error_circle.png",
                HeightRequest = 28,
                WidthRequest = 28,
                VerticalOptions = LayoutOptions.Center
            };

            // Question text
            var questionLabel = new Label
            {
                Text = $"Q{i + 1}: {q.QuestionText}",
                FontSize = 16,
                TextColor = Color.FromArgb("#23243a"),
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };

            // User answer and correct answer
            var answerLabel = new Label
            {
                Text = isCorrect
                    ? $"Your answer: {Math.Round(userAns, 2)}"
                    : $"Your answer: {Math.Round(userAns, 2)}    |    Correct: {Math.Round(q.Answer, 2)}",
                FontSize = 15,
                TextColor = isCorrect ? Color.FromArgb("#4CAF50") : Color.FromArgb("#E53935"),
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(36) },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };

            grid.Children.Add(icon);
            Grid.SetColumn(icon, 0);
            Grid.SetRow(icon, 0);
            Grid.SetRowSpan(icon, 2);

            grid.Children.Add(questionLabel);
            Grid.SetColumn(questionLabel, 1);
            Grid.SetRow(questionLabel, 0);

            grid.Children.Add(answerLabel);
            Grid.SetColumn(answerLabel, 1);
            Grid.SetRow(answerLabel, 1);

            var card = new Border
            {
                BackgroundColor = isCorrect ? Color.FromArgb("#e3fcec") : Color.FromArgb("#ffebee"),
                Stroke = isCorrect ? Color.FromArgb("#4CAF50") : Color.FromArgb("#E53935"),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(16) },
                Padding = new Thickness(12, 8),
                Content = grid
            };

            ReviewStack.Children.Add(card);
        }

        ReviewModal.IsVisible = true;
        await ReviewModal.FadeTo(1, 250, Easing.CubicIn);
    }

    private async void OnCloseReviewModalClicked(object sender, EventArgs e)
    {
        await ReviewModal.FadeTo(0, 250, Easing.CubicOut);
        ReviewModal.IsVisible = false;
    }

    private async void ShowResult()
    {
        ShowReviewModal();
        await Task.Delay(100); // Let the modal appear before continuing

        // Wait for the user to close the review modal before proceeding
        while (ReviewModal.IsVisible)
            await Task.Delay(100);

        int correct = 0;
        for (int i = 0; i < _questions.Count; i++)
        {
            if (Math.Abs(_questions[i].Answer - _userAnswers[i]) < 0.01)
                correct++;
        }

        if (correct < 6)
        {
            bool retry = await ShowConfirmDialog("Quiz Finished",
                $"You scored {correct}/10. You need at least 6 to pass.\nWould you like to try again?",
                "Retry", "Exit");
            if (retry)
            {
                _questions.Clear();
                _userAnswers.Clear();
                _currentQuestionIndex = 0;
                GenerateQuestions();
                ShowCurrentQuestion(false);
            }
            else
            {
                await Navigation.PopAsync();
            }
        }
        else
        {
            bool submit = await ShowConfirmDialog("Quiz Finished",
                $"You answered {correct}/10 correctly.\nSubmit score?",
                "Submit", "Cancel");
            if (submit)
            {
                await SubmitToLeaderboard(correct);
                await UpdateStudentLevel(SessionManager.Username);

                // Store score for LevelUp dialog
                Preferences.Set("LastQuizScore", correct);
            }
            await Navigation.PopAsync();
        }
    }

    private async Task SubmitToLeaderboard(int score)
    {
        if (!SessionManager.IsLoggedIn)
        {
            ShowErrorDialog("Session Error", "User session is not active. Please log in again.");
            while (ErrorDialog.IsVisible) await Task.Delay(100);
            return;
        }
        int star = 0;
        if (score == 6 || score == 7) star = 1;
        else if (score == 8 || score == 9) star = 2;
        else if (score == 10) star = 3;
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO leaderboard (user_id, username, score, star, quiz_name)
                VALUES (@user_id, @username, @score, @star, @quiz_name)";
            command.Parameters.AddWithValue("@user_id", SessionManager.UserId);
            command.Parameters.AddWithValue("@username", SessionManager.Username);
            command.Parameters.AddWithValue("@score", $"{score}/10");
            command.Parameters.AddWithValue("@star", star);
            command.Parameters.AddWithValue("@quiz_name", QuizName);
            await command.ExecuteNonQueryAsync();

            ShowSuccessDialog("Score Submitted!", "Your score has been added to the leaderboard.");
            while (SuccessDialog.IsVisible) await Task.Delay(100);
        }
        catch (Exception ex)
        {
            ShowErrorDialog("Error", $"Failed to submit score.\n{ex.Message}");
            while (ErrorDialog.IsVisible) await Task.Delay(100);
        }
    }

    private async Task<int> GetStudentLevel(string username)
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            string query = "SELECT level FROM user WHERE username = @username";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Level fetch error: {ex.Message}");
            return 1;
        }
    }

    private async Task UpdateStudentLevel(string username)
    {
        int level = await GetStudentLevel(username);
        int newLevel = level < 8 ? 8 : level; // Upgrade to level 8 for Geometry quiz

        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            string query = "UPDATE user SET level = @level WHERE username = @username";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@level", newLevel);
            cmd.Parameters.AddWithValue("@username", username);
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating level: {ex.Message}");
        }
    }
    private async void OnCalculatorClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new CalculatorPage());
    }

    public async Task ResumeMusicAsync()
    {
        await BackgroundMusicService.Instance.PlayAsync("QuizMusic.mp3");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ResumeMusicAsync();
    }
}