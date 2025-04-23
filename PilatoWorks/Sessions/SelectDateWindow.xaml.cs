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
	}

	private void calendar_SelectedDatesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		SessionsWindow sessionsWindow = new(_user, calendar.SelectedDate.Value, this);
		sessionsWindow.ShowDialog();
	}

	private void Window_Closed(object sender, EventArgs e) =>
		_loginWindow.Show();
}