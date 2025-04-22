using System.Windows;

namespace PilatoWorks.Sessions;

/// <summary>
/// Interaction logic for SessionsWindow.xaml
/// </summary>
public partial class SessionsWindow : Window
{
	private readonly DateTime _selectedDate;
	private readonly SelectDateWindow _selectDateWindow;

	public SessionsWindow(DateTime? selectedDate, SelectDateWindow selectDateWindow)
	{
		InitializeComponent();

		_selectedDate = selectedDate ?? DateTime.Now;
		_selectDateWindow = selectDateWindow;
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		datePicker.SelectedDate = _selectedDate;
	}

	private void Window_Closed(object sender, EventArgs e) =>
		_selectDateWindow.Show();

	#region Button Events

	private void sevenAMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 7);
		createSessionWindow.ShowDialog();
	}

	private void eightAMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 8);
		createSessionWindow.ShowDialog();
	}

	private void nineAMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 9);
		createSessionWindow.ShowDialog();
	}

	private void tenAMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 10);
		createSessionWindow.ShowDialog();
	}

	private void elevenAMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 11);
		createSessionWindow.ShowDialog();
	}

	private void twelvePMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 12);
		createSessionWindow.ShowDialog();
	}

	private void onePMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 13);
		createSessionWindow.ShowDialog();
	}

	private void twoPMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 14);
		createSessionWindow.ShowDialog();
	}

	private void threePMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 15);
		createSessionWindow.ShowDialog();
	}

	private void fourPMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 16);
		createSessionWindow.ShowDialog();
	}

	private void fivePMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 17);
		createSessionWindow.ShowDialog();
	}

	private void sixPMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 18);
		createSessionWindow.ShowDialog();
	}

	private void sevenPMCreateSessionButton_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, 19);
		createSessionWindow.ShowDialog();
	}

	#endregion
}
