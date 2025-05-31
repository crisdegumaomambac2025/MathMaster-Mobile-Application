using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MySqlConnector;
using System.Timers;
using MathMaster.Services;

namespace MathMaster.Views;

public partial class NumbersOperations : ContentPage, IResumableMusicPage
{
    private readonly List<(string QuestionText, double Answer)> _questions = new();
    private readonly List<double> _userAnswers = new();
    private int _currentQuestionIndex = 0;
    private readonly Random _random = new();
    private const string QuizName = "NumbersOperations";

    private readonly Dictionary<string, double> _currentOptions = new();
    private string? _selectedOption = null;
    private readonly string[] _optionLabels = { "A", "B", "C", "D" };
    private TaskCompletionSource<bool>? _confirmDialogTcs;

    private System.Timers.Timer? _questionTimer;
    private const int QuestionTimeLimitSeconds = 30;
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

    public NumbersOperations()
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

        var allQuestions = new List<(string QuestionText, double CorrectAnswer, List<double> Options)>
    {
        ("What is the smallest positive integer divisible by 3, 4, 5, and 6 but not divisible by 7?",
            60, new List<double>{ 60, 120, 180, 210 }),

        ("If a ∗ b = a² + b² - ab, what is the value of 3 ∗ 4?",
            13, new List<double>{ 13, 12, 15, 10 }),

        ("A number is decreased by 40%, then increased by 50%. What is the net percent change?",
            10, new List<double>{ -10, 0, 10, 20 }),

        ("If x + 1/x = 3, find the value of x² + 1/x².",
            7, new List<double>{ 7, 8, 9, 6 }),

        ("What is the remainder when 7^103 is divided by 6?",
            1, new List<double>{ 1, 5, 3, 0 }),

        ("The sum of five consecutive odd numbers is 145. What is the smallest of these numbers?",
            25, new List<double>{ 25, 27, 29, 23 }),

        ("Simplify: (3^10 × 9^2) / 27^3",
            27, new List<double>{ 27, 81, 9, 3 }),

        ("If √(x + 9) = x - 3, find all real solutions for x.",
            4, new List<double>{ 4, 3, 5, 6 }),

        ("A three-digit number has digits that sum to 18 and is divisible by 9. What is the probability it is also divisible by 11?",
            0.18, new List<double>{ 0.18, 0.5, 0.25, 0.33 }),

        ("Find the largest prime factor of 2⁶ × 3⁴ × 5² × 7³ × 11.",
            11, new List<double>{ 11, 7, 5, 3 }),

        ("If the average of three numbers is 60, and two are 45 and 90, what is the third?",
            45, new List<double>{ 45, 60, 30, 75 }),

        ("What is the smallest excellent number (square and cube) greater than 64?",
            729, new List<double>{ 729, 1000, 512, 216 }),

        ("Evaluate: (2 + √3)² + (2 − √3)²",
            14, new List<double>{ 14, 12, 16, 10 }),

        ("If x² ≡ 1 mod 8, which of the following can be a value of x?",
            3, new List<double>{ 3, 4, 2, 6 }),

        ("A number leaves remainder 3 mod 5 and 2 mod 4. What is the smallest such number?",
            7, new List<double>{ 7, 17, 23, 27 }),

        ("Reversing digits increases number by 27. Number is between 10 and 100. What is the number?",
            41, new List<double>{ 41, 52, 63, 32 }),

        ("If x = 3 + 2√2, find x + 1/x.",
            6, new List<double>{ 6, 7, 5, 8 }),

        ("What is the smallest positive integer with exactly 6 positive divisors?",
            12, new List<double>{ 12, 18, 30, 16 }),

        ("How many trailing zeros are there in 100!?",
            24, new List<double>{ 24, 20, 22, 25 }),

        ("If n(n+1)/2 = 136, find n.",
            16, new List<double>{ 16, 17, 15, 18 }),
    };

        var selected = allQuestions.OrderBy(q => _random.Next()).Take(10).ToList();

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

        _answeredThisQuestion = false;
        // Reset progress bar
        QuestionProgressBar.Progress = 0;

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

        // Start timer for this question
        StartQuestionTimer();

        if (animated)
            await QuizCard.FadeTo(1, 250, Easing.CubicOut);
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

        double selectedAnswer = _currentOptions[_selectedOption];
        _userAnswers.Add(selectedAnswer);
        _currentQuestionIndex++;
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
            }

            // Store score for LevelUp dialog
            Preferences.Set("LastQuizScore", correct);
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
        int newLevel = level < 7 ? 7 : level; // Upgrade to level 7 for Numbers quiz

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
            Console.WriteLine($"Level update error: {ex.Message}");
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