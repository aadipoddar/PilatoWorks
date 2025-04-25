using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PilatoWorks.Admin;

/// <summary>
/// Interaction logic for TrainerPage.xaml
/// </summary>
public partial class TrainerPage : Page
{
	public TrainerPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private void numberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void decimalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private void trainerDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		UpdateFields();

	private void textBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private async Task LoadData()
	{
		if (trainerDataGrid is null) return;

		trainerDataGrid.ItemsSource = await CommonData.LoadTableData<TrainerModel>(TableNames.Trainer);
		trainerDataGrid.Items.Refresh();

		UpdateFields();
	}

	private void UpdateFields()
	{
		if (trainerDataGrid.SelectedItem is TrainerModel selectedTrainer)
		{
			nameTextBox.Text = selectedTrainer.Name;
			numberTextBox.Text = selectedTrainer.Number;
			addressTextBox.Text = selectedTrainer.Address;
			salaryTextBox.Text = selectedTrainer.Salary.ToString();
			commissionTextBox.Text = selectedTrainer.CommissionPercent.ToString();
			statusCheckBox.IsChecked = selectedTrainer.Status;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;
		}

		else
		{
			nameTextBox.Clear();
			numberTextBox.Clear();
			addressTextBox.Clear();
			salaryTextBox.Clear();
			commissionTextBox.Clear();
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (trainerDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text.Trim()) &&
			!string.IsNullOrEmpty(numberTextBox.Text.Trim()) &&
			!string.IsNullOrEmpty(addressTextBox.Text.Trim()) &&
			!string.IsNullOrEmpty(salaryTextBox.Text.Trim()) &&
			!string.IsNullOrEmpty(commissionTextBox.Text.Trim())) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Trainer Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(numberTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Trainer Number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(addressTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Trainer Address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(salaryTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Trainer Salary", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(commissionTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Trainer Commission Percent", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		await TrainerData.InsertTrainer(new TrainerModel
		{
			Id = trainerDataGrid.SelectedItem is TrainerModel selectedTrainer ? selectedTrainer.Id : 0,
			Name = nameTextBox.Text.Trim(),
			Number = numberTextBox.Text.Trim().RemoveSpace(),
			Address = addressTextBox.Text.Trim(),
			Salary = int.Parse(salaryTextBox.Text.Trim()),
			CommissionPercent = decimal.Parse(commissionTextBox.Text.Trim()),
			Status = statusCheckBox.IsChecked ?? false
		});

		await LoadData();
	}
}
