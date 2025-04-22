using System.Windows;

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
}
