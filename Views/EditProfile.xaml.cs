using Microsoft.Maui.Controls;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace MathMaster.Views;

public partial class EditProfile : ContentPage
{
	private string _currentUsername;
	private string _currentPassword;

	public EditProfile()
	{
		InitializeComponent();
		LoadUserData();
	}

	private void OnErrorPanelClose(object sender, EventArgs e)
	{
		if (ErrorPanel != null)
		{
			ErrorPanel.IsVisible = false;
		}
	}

	private void OnSuccessDialogClose(object sender, EventArgs e)
	{
		if (SuccessDialog != null)
		{
			SuccessDialog.IsVisible = false;
			Navigation.PopAsync();
		}
	}

	private void ShowErrorDialog(string title, string message)
	{
		if (ErrorPanel != null && ErrorTitle != null && ErrorMessage != null)
		{
			ErrorTitle.Text = title;
			ErrorMessage.Text = message;
			ErrorPanel.IsVisible = true;
			ErrorPanel.FadeTo(1, 250, Easing.CubicIn);
		}
	}

	private void ShowSuccessDialog(string title, string message)
	{
		if (SuccessDialog != null && SuccessTitle != null && SuccessMessage != null)
		{
			SuccessTitle.Text = title;
			SuccessMessage.Text = message;
			SuccessDialog.IsVisible = true;
			SuccessDialog.FadeTo(1, 250, Easing.CubicIn);
		}
	}

	private async void LoadUserData()
	{
		_currentUsername = Preferences.Get("Username", string.Empty);
		if (string.IsNullOrEmpty(_currentUsername))
		{
			ShowErrorDialog("Error", "Please login first");
			await Navigation.PopAsync();
			return;
		}

		try
		{
			using var connection = DB_Connection.GetConnection();
			await connection.OpenAsync();
			string query = "SELECT firstname, lastname, password FROM user WHERE username = @username";
			using var cmd = new MySqlCommand(query, connection);
			cmd.Parameters.AddWithValue("@username", _currentUsername);
			
			using var reader = await cmd.ExecuteReaderAsync();
			if (await reader.ReadAsync())
			{
				FirstNameEntry.Text = reader.GetString("firstname");
				LastNameEntry.Text = reader.GetString("lastname");
				_currentPassword = reader.GetString("password");
			}
		}
		catch (Exception ex)
		{
			ShowErrorDialog("Error", "Failed to load user data: " + ex.Message);
		}
	}

	private async void OnSaveClicked(object sender, EventArgs e)
	{
		await SaveButton.ScaleTo(0.95, 80, Easing.CubicInOut);
		await SaveButton.ScaleTo(1.0, 80, Easing.CubicInOut);

		if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) || 
			string.IsNullOrWhiteSpace(LastNameEntry.Text) || 
			string.IsNullOrWhiteSpace(PasswordEntry.Text))
		{
			ShowErrorDialog("Required Fields", "Please fill in all fields before saving.");
			while (ErrorPanel.IsVisible) await Task.Delay(100);
			return;
		}

		if (PasswordEntry.Text != _currentPassword)
		{
			ShowErrorDialog("Invalid Password", "The password you entered is incorrect. Please try again.");
			while (ErrorPanel.IsVisible) await Task.Delay(100);
			return;
		}

		try
		{
			using var connection = DB_Connection.GetConnection();
			await connection.OpenAsync();
			string query = "UPDATE user SET firstname = @firstname, lastname = @lastname WHERE username = @username";
			using var cmd = new MySqlCommand(query, connection);
			cmd.Parameters.AddWithValue("@firstname", FirstNameEntry.Text.Trim());
			cmd.Parameters.AddWithValue("@lastname", LastNameEntry.Text.Trim());
			cmd.Parameters.AddWithValue("@username", _currentUsername);

			int rowsAffected = await cmd.ExecuteNonQueryAsync();
			if (rowsAffected > 0)
			{
				ShowSuccessDialog("Profile Updated", "Your profile has been updated successfully!");
				while (SuccessDialog.IsVisible) await Task.Delay(100);
				await Navigation.PopAsync();
			}
			else
			{
				ShowErrorDialog("Update Failed", "Failed to update your profile. Please try again.");
				while (ErrorPanel.IsVisible) await Task.Delay(100);
			}
		}
		catch (Exception ex)
		{
			ShowErrorDialog("Error", $"Failed to update profile.\n{ex.Message}");
			while (ErrorPanel.IsVisible) await Task.Delay(100);
		}
	}
}