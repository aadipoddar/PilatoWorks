using System.Windows;

namespace PilatoWorks.Sessions;

/// <summary>
/// Interaction logic for CreateSessionWindow.xaml
/// </summary>
public partial class CreateSessionWindow : Window
{
	private readonly DateTime _sessionDay;
	private readonly int _slot;

	public CreateSessionWindow(DateTime sessionDay, int slot)
	{
		InitializeComponent();

		_sessionDay = sessionDay;
		_slot = slot;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		sessionDayComboBox.SelectedIndex = (int)_sessionDay.DayOfWeek - 1;
		sessionSlotComboBox.SelectedIndex = _slot - 7;

		trainer1ComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);
		trainer1ComboBox.DisplayMemberPath = nameof(TrainerModel.Name);
		trainer1ComboBox.SelectedValuePath = nameof(TrainerModel.Id);
		trainer1ComboBox.SelectedIndex = -1;

		trainer2ComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<TrainerModel>(TableNames.Trainer);
		trainer2ComboBox.DisplayMemberPath = nameof(TrainerModel.Name);
		trainer2ComboBox.SelectedValuePath = nameof(TrainerModel.Id);
		trainer2ComboBox.SelectedIndex = -1;

		sessionTypeComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<SessionTypeModel>(TableNames.SessionType);
		sessionTypeComboBox.DisplayMemberPath = nameof(SessionTypeModel.Name);
		sessionTypeComboBox.SelectedValuePath = nameof(SessionTypeModel.Id);
		sessionTypeComboBox.SelectedIndex = 0;
	}

	private void saveButton_Click(object sender, RoutedEventArgs e)
	{

	}
}
