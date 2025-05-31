using System;
using Microsoft.Maui.Controls;

namespace MathMaster.View;

public partial class ProblemSolverPage : ContentPage
{
    private int correctAnswer;
    public ProblemSolverPage()
	{
		InitializeComponent();
        GenerateNewProblem();
    }

    private void GenerateNewProblem()
    {
        Random rand = new Random();
        int num1 = rand.Next(1, 20);
        int num2 = rand.Next(1, 20);
        string[] operators = { "+", "-", "*", "/" };
        string selectedOperator = operators[rand.Next(operators.Length)];

        if (selectedOperator == "/")
        {
            num1 = num2 * rand.Next(1, 10);
        }

        correctAnswer = selectedOperator switch
        {
            "+" => num1 + num2,
            "-" => num1 - num2,
            "*" => num1 * num2,
            "/" => num1 / num2,
            _ => 0
        };

        ProblemLabel.Text = $"{num1} {selectedOperator} {num2} = ?";
        ResultLabel.Text = "";
        UserAnswerEntry.Text = "";
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserAnswerEntry.Text))
        {
            ResultLabel.Text = "⚠️ Please enter a number!";
            ResultLabel.TextColor = Colors.Orange;
            return;
        }

        if (int.TryParse(UserAnswerEntry.Text, out int userAnswer))
        {
            if (userAnswer == correctAnswer)
            {
                await ShowCorrectAnswerDialog();
                GenerateNewProblem();
            }
            else
            {
                ResultLabel.Text = "❌ Incorrect. Try again!";
                ResultLabel.TextColor = Colors.Red;
            }
        }
        else
        {
            ResultLabel.Text = "⚠️ Enter a valid number!";
            ResultLabel.TextColor = Colors.Orange;
        }
    }

    private void OnNewProblemClicked(object sender, EventArgs e)
    {
        GenerateNewProblem();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async Task ShowCorrectAnswerDialog()
    {
        await DisplayAlert("✅ Correct!", "Great job! You got the right answer.", "OK");
    }
}