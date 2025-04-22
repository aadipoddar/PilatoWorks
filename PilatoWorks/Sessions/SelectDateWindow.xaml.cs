using System.Windows;

namespace PilatoWorks.Sessions;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class SelectDateWindow : Window
{
	public SelectDateWindow()
	{
		InitializeComponent();
	}

	private void calendar_SelectedDatesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		SessionsWindow sessionsWindow = new(calendar.SelectedDate, this);
		sessionsWindow.Show();
		Hide();
	}
}