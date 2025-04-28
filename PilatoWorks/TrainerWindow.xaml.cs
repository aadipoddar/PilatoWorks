using System.Windows;
using System.Windows.Controls;

namespace PilatoWorks;

/// <summary>
/// Interaction logic for TrainerWindow.xaml
/// </summary>
public partial class TrainerWindow : Window
{
	private TrainerModel _foundTrainer = null;

	public TrainerWindow() => InitializeComponent();

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private async void numberTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		var trainers = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);
		_foundTrainer = trainers.FirstOrDefault(x => x.Number == numberTextBox.Text.Trim().RemoveSpace());

		if (_foundTrainer is null) return;

		nameTextBox.Text = _foundTrainer.Name;
		addressTextBox.Text = _foundTrainer.Address;
		salaryTextBox.Text = _foundTrainer.Salary.FormatIndianCurrency();
		commissionPercentTextBox.Text = _foundTrainer.CommissionPercent.ToString();
	}

	private async void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_foundTrainer is null || fromDatePicker.SelectedDate is null || toDatePicker.SelectedDate is null)
			return;

		var fromDate = DateOnly.FromDateTime(fromDatePicker.SelectedDate.Value);
		var toDate = DateOnly.FromDateTime(toDatePicker.SelectedDate.Value);

		var sessions = await SessionData.LoadSessionDetailsByDateTrainer(
			fromDate,
			toDate,
			_foundTrainer.Id);

		sessionTakenDataGrid.ItemsSource = sessions;

		noSessionsTextBox.Text = sessions.Count.ToString();
		totalSalesTextBox.Text = sessions.Sum(x => x.PerSessionCost).FormatIndianCurrency();
	}
}
