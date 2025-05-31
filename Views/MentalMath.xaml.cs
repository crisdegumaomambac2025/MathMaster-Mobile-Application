using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MySqlConnector;
using System.Timers;
using MathMaster.Services;

namespace MathMaster.Views
{
    public partial class MentalMath : ContentPage, IResumableMusicPage
    {
        private readonly List<(string QuestionText, double Answer)> _questions = new();
        private int _currentQuestionIndex = 0;
        private readonly List<double> _userAnswers = new();
        private readonly Random _random = new();
        private Button? _selectedButton = null;
        private const string QuizName = "Mental Math"; // 🧠 Quiz name constant
        private TaskCompletionSource<bool>? _confirmDialogTcs;
        private System.Timers.Timer? _questionTimer;
        private const int QuestionTimeLimitSeconds = 10;
        private DateTime _questionStartTime;
        private bool _answeredThisQuestion = false;

        public MentalMath()
        {
            InitializeComponent();
            ValidateStudentSession();
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

        private async void ValidateStudentSession()
        {
            string role = Preferences.Get("Role", string.Empty);
            string username = Preferences.Get("Username", string.Empty);

            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || role != "Student")
            {
                ShowErrorDialog("⛔ Unauthorized", "Only students can access this quiz.");
                while (ErrorDialog.IsVisible) await Task.Delay(100);
                await Navigation.PushAsync(new LoginPage());
            }
            else
            {
                if (!SessionManager.IsLoggedIn)
                {
                    ShowErrorDialog("⚠️ Session Error", "You must be logged in to take the quiz.");
                    while (ErrorDialog.IsVisible) await Task.Delay(100);
                    await Navigation.PopAsync();
                    return;
                }

                // ✅ Check if student already took this specific quiz
                bool alreadyTaken = await HasStudentTakenQuiz(SessionManager.Username);

                if (alreadyTaken)
                {
                    ShowInfoDialog("🎉 You've Already Completed This Quiz!", "Great job! You've already taken the Mental Math quiz. Want to try another challenge instead?");
                    while (InfoDialog.IsVisible) await Task.Delay(100);
                    await Navigation.PopAsync(); // Go back to previous page
                    return;
                }

                ShowReadyDialog(); // Start quiz
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
                ShowErrorDialog("❌ Error", $"Error checking quiz status.\n{ex.Message}");
                while (ErrorDialog.IsVisible) await Task.Delay(100);
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
            var operations = new[] { "+", "-", "*", "/" };
            var usedQuestions = new HashSet<string>();

            while (_questions.Count < 10)
            {
                string op = operations[_random.Next(operations.Length)];
                int a = _random.Next(1, 21);
                int b = _random.Next(1, 21);

                if (op == "/") a = a * b;

                string question = $"{a} {op} {b}";
                if (usedQuestions.Contains(question)) continue;
                usedQuestions.Add(question);

                double answer = op switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => (double)a / b,
                    _ => 0
                };

                _questions.Add((question, answer));
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
            {
                await QuizCard.FadeTo(0, 250, Easing.CubicIn);
            }

            var current = _questions[_currentQuestionIndex];
            QuestionLabel.Text = $"Q{_currentQuestionIndex + 1}: {current.QuestionText} = ?";
            AnswerEntry.Text = string.Empty;

            // Reset progress bar
            QuestionProgressBar.Progress = 0;

            // Start timer for this question
            StartQuestionTimer();

            _answeredThisQuestion = false;

            if (animated)
            {
                await QuizCard.FadeTo(1, 250, Easing.CubicOut);
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
                        ShowCurrentQuestion();
                    });
                }
            }
        }

        private async void OnNextQuestionClicked(object sender, EventArgs e)
        {
            await NextButton.ScaleTo(0.95, 80, Easing.CubicInOut);
            await NextButton.ScaleTo(1.0, 80, Easing.CubicInOut);

            if (string.IsNullOrWhiteSpace(AnswerEntry.Text))
            {
                ShowErrorDialog("Enter Your Answer", "Please enter your answer before proceeding.");
                while (ErrorDialog.IsVisible) await Task.Delay(100);
                return;
            }

            if (!double.TryParse(AnswerEntry.Text, out double userAnswer))
            {
                ShowErrorDialog("Invalid Input", "Please enter a valid number.");
                while (ErrorDialog.IsVisible) await Task.Delay(100);
                return;
            }

            _answeredThisQuestion = true;
            _questionTimer?.Stop();
            _questionTimer?.Dispose();

            _userAnswers.Add(userAnswer);
            _currentQuestionIndex++;
            ShowCurrentQuestion();
        }

        private async void OnCloseWarningDialogClicked(object sender, EventArgs e)
        {
            await WarningModal.FadeTo(0, 250, Easing.CubicOut); // Fade out over 250ms
            WarningModal.IsVisible = false;
        }

        // --- Review Modal Logic ---

        private async void ShowReviewModal()
        {
            ReviewStack.Children.Clear();

            for (int i = 0; i < _questions.Count; i++)
            {
                var q = _questions[i];
                double userAns = _userAnswers[i];
                bool isAnswered = !double.IsNaN(userAns);
                bool isCorrect = isAnswered && Math.Abs(q.Answer - userAns) < 0.01;

                // Icon for correct/incorrect/unanswered
                var icon = new Image
                {
                    Source = isAnswered
                        ? (isCorrect ? "check_circle.png" : "error_circle.png")
                        : "error_circle.png",
                    HeightRequest = 28,
                    WidthRequest = 28,
                    VerticalOptions = LayoutOptions.Center
                };

                // Question text
                var questionLabel = new Label
                {
                    Text = $"Q{i + 1}: {q.QuestionText} = ?",
                    FontSize = 16,
                    TextColor = Color.FromArgb("#23243a"),
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center
                };

                // User answer and correct answer
                Label answerLabel;
                if (!isAnswered)
                {
                    answerLabel = new Label
                    {
                        Text = "You failed to answer.",
                        FontSize = 15,
                        TextColor = Color.FromArgb("#E53935"),
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Center
                    };
                }
                else
                {
                    answerLabel = new Label
                    {
                        Text = isCorrect
                            ? $"Your answer: {userAns}"
                            : $"Your answer: {userAns}    |    Correct: {q.Answer}",
                        FontSize = 15,
                        TextColor = isCorrect ? Color.FromArgb("#4CAF50") : Color.FromArgb("#E53935"),
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Center
                    };
                }

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
                    Background = new SolidColorBrush(isCorrect ? Color.FromArgb("#e3fcec") : Color.FromArgb("#ffebee")),
                    Stroke = new SolidColorBrush(isCorrect ? Color.FromArgb("#4CAF50") : Color.FromArgb("#E53935")),
                    StrokeThickness = 2,
                    Padding = new Thickness(12, 8),
                    Content = grid,
                    Margin = new Thickness(0, 0, 0, 0),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.Rectangle { RadiusX = 16, RadiusY = 16 }
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
                    _selectedButton = null;

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
                Console.WriteLine($"Error fetching level: {ex.Message}");
                return 1;
            }
        }

        private async Task UpdateStudentLevel(string username)
        {
            int currentLevel = await GetStudentLevel(username);
            int newLevel = currentLevel;

            if (currentLevel < 2)
            {
                newLevel = 2;
            }

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
    }
}