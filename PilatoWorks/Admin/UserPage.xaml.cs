using System.Windows;
using System.Windows.Controls;

namespace PilatoWorks.Admin;

/// <summary>
/// Interaction logic for UsersPage.xaml
/// </summary>
public partial class UserPage : Page
{
	public UserPage() => InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private void adminCheckBox_Checked(object sender, RoutedEventArgs e)
	{
		sessionCheckBox.IsChecked = true;
		trainerCheckBox.IsChecked = true;
	}

	private void adminCheckBox_Unchecked(object sender, RoutedEventArgs e)
	{
		sessionCheckBox.IsChecked = true;
		trainerCheckBox.IsChecked = false;
	}

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private async Task ApplySearchFilter()
	{
		if (userDataGrid is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;
		bool showAdmin = showAdminCheckBox?.IsChecked ?? false;
		bool showNonAdmin = showNonAdminCheckBox?.IsChecked ?? false;
		bool showSession = showSessionCheckBox?.IsChecked ?? false;
		bool showNonSession = true;
		bool showTrainer = showTrainerCheckBox?.IsChecked ?? false;
		bool showNonTrainer = true;

		userDataGrid.ItemsSource = (await CommonData.LoadTableData<UserModel>(TableNames.User))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.Where(item => (showAdmin && item.Admin) || (showNonAdmin && !item.Admin))
			.Where(item => (showSession && item.Session) || (showNonSession && !item.Session))
			.Where(item => (showTrainer && item.Trainer) || (showNonTrainer && !item.Trainer))
			.OrderBy(item => !item.Status)
			.ToList();

		UpdateFields();
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void searchLocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private void userDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private void passwordTextBox_TextChanged(object sender, RoutedEventArgs e) =>
		UpdateButtonField();

	private void UpdateFields()
	{
		if (userDataGrid.SelectedItem is UserModel selectedUser)
		{
			nameTextBox.Text = selectedUser.Name;
			passwordBox.Password = selectedUser.Password.ToString();
			statusCheckBox.IsChecked = selectedUser.Status;
			sessionCheckBox.IsChecked = selectedUser.Session;
			trainerCheckBox.IsChecked = selectedUser.Trainer;
			adminCheckBox.IsChecked = selectedUser.Admin;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;
		}

		else
		{
			nameTextBox.Clear();
			passwordBox.Clear();
			statusCheckBox.IsChecked = true;
			sessionCheckBox.IsChecked = false;
			trainerCheckBox.IsChecked = false;
			adminCheckBox.IsChecked = false;
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (userDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text) && !string.IsNullOrEmpty(passwordBox.Password)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a User Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(passwordBox.Password.Trim()))
		{
			MessageBox.Show("Please enter a Password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (sessionCheckBox.IsChecked == false && trainerCheckBox.IsChecked == false)
		{
			MessageBox.Show("Please select at least one Permission", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (adminCheckBox.IsChecked == true && (sessionCheckBox.IsChecked == false || trainerCheckBox.IsChecked == false))
		{
			MessageBox.Show("Admin must have all permissions", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		UserModel userModel = new()
		{
			Id = userDataGrid.SelectedItem is UserModel selectedUser ? selectedUser.Id : 0,
			Name = nameTextBox.Text,
			Password = short.Parse(passwordBox.Password),
			Session = (bool)sessionCheckBox.IsChecked,
			Trainer = (bool)trainerCheckBox.IsChecked,
			Admin = (bool)adminCheckBox.IsChecked,
			Status = (bool)statusCheckBox.IsChecked
		};

		await UserData.InsertUser(userModel);

		await ApplySearchFilter();
	}
}
