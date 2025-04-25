using System.Windows;

namespace PilatoWorks.Admin;

/// <summary>
/// Interaction logic for AdminPanel.xaml
/// </summary>
public partial class AdminPanel : Window
{
	public AdminPanel() =>
		InitializeComponent();

	private void Window_Loaded(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new UserPage();

	private void manageUserButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new UserPage();

	private void manageSlotsButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new SlotPage();

	private void manageTrainerButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new TrainerPage();

	private void sqlEditorButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new SqlEditorPage();

	private void settingsButton_Click(object sender, RoutedEventArgs e) =>
		mainFrame.Content = new SettingsPage();
}
