using System.Windows;

namespace PilatoWorks.Sessions;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class SelectDateWindow : Window
{
	private readonly UserModel _user;
	private readonly LoginWindow _loginWindow;

	public SelectDateWindow(UserModel user, LoginWindow loginWindow)
	{
		InitializeComponent();

		_user = user;
		_loginWindow = loginWindow;

		_loginWindow.Hide();

		calendar.DisplayDateStart = DateTime.Now;
	}

	private void calendar_SelectedDatesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		if (calendar.SelectedDate is null) return;

		SessionsWindow sessionsWindow = new(_user, calendar.SelectedDate.Value, this);
		sessionsWindow.ShowDialog();

		calendar.SelectedDate = null;
	}

	private void Window_Closed(object sender, EventArgs e) =>
		_loginWindow.Show();
}