using System.Windows;

namespace PilatoWorks.Subscription;

/// <summary>
/// Interaction logic for Manage_Subscriptions.xaml
/// </summary>
public partial class ManageSubscriptionsPanel : Window
{
	private readonly UserModel _user;
	private readonly LoginWindow _loginWindow;

	public ManageSubscriptionsPanel(UserModel user, LoginWindow loginWindow)
	{
		InitializeComponent();

		_user = user;
		_loginWindow = loginWindow;
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		_loginWindow.Hide();
		mainFrame.Content = new SubscriptionPage(_user);
	}

	private void managePersonButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new PersonPage();

	private void manageSubscriptionsButton_Click(object sender, RoutedEventArgs e) => mainFrame.Content = new SubscriptionPage(_user);

	private void Window_Closed(object sender, EventArgs e) =>
		_loginWindow.Show();
}
