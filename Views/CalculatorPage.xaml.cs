using Microsoft.Maui.Controls;
using System;

namespace MathMaster.Views;

public partial class CalculatorPage : ContentPage
{
    public CalculatorPage()
    {
        InitializeComponent();
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        Display.Text += button.Text;
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        Display.Text = string.Empty;
    }

    private void OnCalculateClicked(object sender, EventArgs e)
    {
        try
        {
            var result = new System.Data.DataTable().Compute(Display.Text, null);
            Display.Text = result.ToString();
        }
        catch
        {
            Display.Text = "Error";
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
