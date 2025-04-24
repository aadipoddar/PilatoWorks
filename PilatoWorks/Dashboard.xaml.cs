using System.Windows;

using PilatoWorks.Sessions;
using PilatoWorks.Subscription;

namespace PilatoWorks;

/// <summary>
/// Interaction logic for Dashboard.xaml
/// </summary>
public partial class Dashboard : Window
{
	private readonly UserModel _user;
	private readonly LoginWindow _loginWindow;

	public Dashboard(UserModel user, LoginWindow loginWindow)
	{
		InitializeComponent();

		_user = user;
		_loginWindow = loginWindow;
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		if (!_user.Session) sessionButton.Visibility = Visibility.Collapsed;
		if (!_user.Trainer) trainerButton.Visibility = Visibility.Collapsed;
		if (!_user.Admin) adminButton.Visibility = Visibility.Collapsed;
	}

	private void sessionButton_Click(object sender, RoutedEventArgs e)
	{
		SelectDateWindow selectDateWindow = new(_user, _loginWindow);
		selectDateWindow.Show();
		Close();
		_loginWindow.Hide();
	}

	private void trainerButton_Click(object sender, RoutedEventArgs e)
	{

	}

	private void subscriptionButton_Click(object sender, RoutedEventArgs e)
	{
		ManageSubscriptionsPanel manageSubscriptions = new(_loginWindow);
		manageSubscriptions.Show();
		Close();
		_loginWindow.Hide();
	}

	private void clearDuesButton_Click(object sender, RoutedEventArgs e)
	{

	}

	private void adminButton_Click(object sender, RoutedEventArgs e)
	{

	}

	private void Window_Closed(object sender, EventArgs e) =>
		_loginWindow.Show();
}
