using System.Windows;

namespace PilatoWorks.Sessions;

/// <summary>
/// Interaction logic for SessionsWindow.xaml
/// </summary>
public partial class SessionsWindow : Window
{
	private readonly UserModel _user;
	private readonly DateTime _selectedDate;
	private readonly SelectDateWindow _selectDateWindow;

	public SessionsWindow(UserModel user, DateTime selectedDate, SelectDateWindow selectDateWindow)
	{
		InitializeComponent();

		_user = user;
		_selectedDate = selectedDate;
		_selectDateWindow = selectDateWindow;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		datePicker.SelectedDate = _selectedDate;

		slotComboBox.ItemsSource = await CommonData.LoadTableData<SlotModel>(TableNames.Slot);
		slotComboBox.DisplayMemberPath = nameof(SlotModel.Hour);
		slotComboBox.SelectedValuePath = nameof(SlotModel.Id);
		slotComboBox.SelectedIndex = 0;
	}

	private void Window_Closed(object sender, EventArgs e) =>
		_selectDateWindow.Show();

	private void slotComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{

	}

	private void createSessionWindow_Click(object sender, RoutedEventArgs e)
	{
		CreateSessionWindow createSessionWindow = new(datePicker.SelectedDate.Value, (int)slotComboBox.SelectedValue);
		createSessionWindow.ShowDialog();
	}
}
