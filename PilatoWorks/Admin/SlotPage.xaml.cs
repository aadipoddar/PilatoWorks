using System.Windows;
using System.Windows.Controls;

namespace PilatoWorks.Admin;

/// <summary>
/// Interaction logic for SlotPage.xaml
/// </summary>
public partial class SlotPage : Page
{
	public SlotPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private void textBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void textBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private void slotDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		UpdateFields();

	private async Task LoadData()
	{
		if (slotDataGrid is null) return;

		slotDataGrid.ItemsSource = await CommonData.LoadTableData<SlotModel>(TableNames.Slot);
		slotDataGrid.Items.Refresh();

		UpdateFields();
	}

	private void UpdateFields()
	{
		if (slotDataGrid.SelectedItem is SlotModel selectedSlot)
		{
			hourTextBox.Text = selectedSlot.Hour.ToString();
			maxTextBox.Text = selectedSlot.Max.ToString();
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;
		}

		else
		{
			hourTextBox.Clear();
			maxTextBox.Clear();
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (slotDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(hourTextBox.Text.Trim()) && !string.IsNullOrEmpty(maxTextBox.Text.Trim())) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(hourTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter Slot Hour", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(maxTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Slot Max", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		await SlotData.InsertSlot(new SlotModel
		{
			Id = slotDataGrid.SelectedItem is SlotModel selectedSlot ? selectedSlot.Id : 0,
			Hour = short.Parse(hourTextBox.Text),
			Max = short.Parse(maxTextBox.Text)
		});

		await LoadData();
	}
}
