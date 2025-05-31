using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MySqlConnector;
using System.Timers;
using MathMaster.Services;

namespace MathMaster.Views;

public partial class GeometricSolid : ContentPage, IResumableMusicPage
{
    private readonly List<(string QuestionText, string Answer, string Image, List<string> Options)> _questions = new();
    private readonly List<string> _userAnswers = new();
    private int _currentQuestionIndex = 0;
    private string? _selectedOption = null;
    private readonly string[] _optionLabels = { "A", "B", "C", "D" };
    private const string QuizName = "Geometric Solid";
    private TaskCompletionSource<bool>? _confirmDialogTcs;
    private System.Timers.Timer? _questionTimer;
    private const int QuestionTimeLimitSeconds = 15;
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

    public GeometricSolid()
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
        using var connection = DB_Connection.GetConnection();
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM leaderboard WHERE username = @username AND quiz_name = @quiz_name";
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@quiz_name", QuizName);
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }
    private void ShowReadyDialog()
    {
        ReadyDialog.IsVisible = true;
    }

    private void OnReadyDialogYesClicked(object sender, EventArgs e)
    {
        ReadyDialog.IsVisible = false;
        LoadQuestions();
        ShowCurrentQuestion();
    }

    private async void OnReadyDialogNoClicked(object sender, EventArgs e)
    {
        ReadyDialog.IsVisible = false;
        await Navigation.PopAsync();
    }

    private void LoadQuestions()
    {
        _questions.Clear();
        var allQuestions = new List<(string QuestionText, string Answer, string Image, List<string> Options)>
        {
            ("What is the name of this solid?", "Cube", "gs_1.png", new List<string>{ "Cube", "Cylinder", "Sphere", "Square Pyramid" }),
            ("What is the name of this 3D solid?", "Cylinder", "gs_2.png", new List<string>{ "Cone", "Hexagonal Pyramid", "Triangular Prism", "Cylinder" }),
            ("What is the name of this 3D solid?", "Rectangular Prism", "gs_3.png", new List<string>{ "Rectangular Tetrahedron", "Rectangular Pyramid", "Rectangular Prism", "Sphere" }),
            ("What is the name of this 3D solid?", "Cone", "gs_4.png", new List<string>{ "Cone", "Triangular Prism", "Square Pyramid", "Hexagonal Pyramid" }),
            ("What is the name of this 3D solid?", "Square Pyramid", "gs_5.png", new List<string>{ "Triangular Prism", "Square Prism", "Triangular Pyramid", "Square Pyramid" }),
            ("What 3D solid does this net create?", "Cone", "gs_6.png", new List<string>{ "Cube", "Cone", "Cylinder", "Triangular Pyramid" }),
            ("What 3D solid does this net create?", "Cube", "gs_7.png", new List<string>{ "Cone", "Triangular Pyramid", "Cylinder", "Cube" }),
            ("Which solid has only one vertex and one circular base?", "Cone", "gs_8.png", new List<string>{ "Cylinder", "Cone", "Sphere", "Cube" }),
            ("Which solid has no edges or vertices?", "Sphere", "gs_9.png", new List<string>{ "Cube", "Cylinder", "Sphere", "Square Pyramid" }),
            ("What is the volume of a cylinder with a radius of 4 cm and a height of 10 cm?", "160π cm³", "gs_10.png", new List<string>{ "120 cm³", "160 cm³", "120π cm³", "160π cm³" }),
        };

        // Shuffle the questions
        var random = new Random();
        var shuffledQuestions = allQuestions.OrderBy(x => random.Next()).ToList();
        _questions.AddRange(shuffledQuestions);
    }

    // Helper method to shuffle a list
    private static void Shuffle<T>(IList<T> list)
    {
        var random = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void ShowCurrentQuestion()
    {
        if (_currentQuestionIndex >= _questions.Count)
        {
            ShowResult();
            return;
        }

        var current = _questions[_currentQuestionIndex];
        QuestionLabel.Text = $"Q{_currentQuestionIndex + 1}: {current.QuestionText}";
        QuestionImage.Source = ImageSource.FromFile(current.Image);
        _selectedOption = null;

        OptionsGrid.Children.Clear();
        for (int i = 0; i < current.Options.Count; i++)
        {
            string label = _optionLabels[i];
            string value = current.Options[i];

            bool isLongOptionQuestion = current.QuestionText == "What is the name of this 3D solid?" && current.Image == "gs_3.png";

            var btn = new Button
            {
                Text = $"{label}. {value}",
                BackgroundColor = Color.FromArgb("#2E2E2E"),
                TextColor = Colors.White,
                FontSize = isLongOptionQuestion ? 14 : 16,
                HeightRequest = isLongOptionQuestion ? 70 : 60,
                Padding = new Thickness(10, 5),
                CornerRadius = 10,
                LineBreakMode = LineBreakMode.WordWrap,
                TextTransform = TextTransform.None,
                HorizontalOptions = LayoutOptions.Fill
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
        // Optionally, you can set _answeredThisQuestion = true here if you want to stop timer on selection
        // But to match Multiplication, only stop timer on Next
    }
    private async void OnNextQuestionClicked(object sender, EventArgs e)
    {
        await NextButton.ScaleTo(0.95, 80, Easing.CubicInOut);
        await NextButton.ScaleTo(1.0, 80, Easing.CubicInOut);

        if (string.IsNullOrEmpty(_selectedOption))
        {
            ShowErrorDialog("Choose an Answer", "Please select one of the options.");
            while (ErrorDialog.IsVisible) await Task.Delay(100);
            return;
        }

        _answeredThisQuestion = true;
        _questionTimer?.Stop();
        _questionTimer?.Dispose();

        string answerText = OptionsGrid.Children
            .OfType<Button>()
            .FirstOrDefault(btn => btn.Text.StartsWith(_selectedOption))?.Text
            .Split(new[] { ". " }, StringSplitOptions.None)[1];

        _userAnswers.Add(answerText);
        _currentQuestionIndex++;
        ShowCurrentQuestion();
    }

    private async void ShowReviewModal()
    {
        ReviewStack.Children.Clear();

        for (int i = 0; i < _questions.Count; i++)
        {
            var q = _questions[i];
            string userAns = i < _userAnswers.Count ? _userAnswers[i] : null;
            bool isCorrect = userAns == q.Answer;

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
                    ? $"Your answer: {userAns}"
                    : $"Your answer: {userAns}    |    Correct: {q.Answer}",
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
            if (_userAnswers[i] == _questions[i].Answer)
                correct++;
        }

        if (correct < 6)
        {
            bool retry = await ShowConfirmDialog("Quiz Finished",
                $"You scored {correct}/10. You need at least 6 to pass.\nWould you like to try again?",
                "Retry", "Exit");
            if (retry)
            {
                _userAnswers.Clear();
                _currentQuestionIndex = 0;
                ShowCurrentQuestion();
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

    private async Task UpdateStudentLevel(string username)
    {
        try
        {
            using var connection = DB_Connection.GetConnection();
            await connection.OpenAsync();
            var getLevel = new MySqlCommand("SELECT level FROM user WHERE username = @username", connection);
            getLevel.Parameters.AddWithValue("@username", username);
            int level = Convert.ToInt32(await getLevel.ExecuteScalarAsync());
            int newLevel = level < 9 ? 9 : level;

            var update = new MySqlCommand("UPDATE user SET level = @level WHERE username = @username", connection);
            update.Parameters.AddWithValue("@level", newLevel);
            update.Parameters.AddWithValue("@username", username);
            await update.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Level update error: {ex.Message}");
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
                    _userAnswers.Add(""); // Use empty string to indicate 'not answered'
                    _currentQuestionIndex++;
                    _selectedOption = null;
                    ShowCurrentQuestion();
                });
            }
        }
    }

    /*private async void OnCalculatorClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new CalculatorPage());
    }*/

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