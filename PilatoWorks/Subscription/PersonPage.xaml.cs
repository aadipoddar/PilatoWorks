using System.Windows;
using System.Windows.Controls;

namespace PilatoWorks.Subscription;

/// <summary>
/// Interaction logic for PersonPage.xaml
/// </summary>
public partial class PersonPage : Page
{
	private int _foundId = 0;

	public PersonPage() => InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private async void numberTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		var foundPerson = await PersonData.LoadPersonByNumber(numberTextBox.Text.Trim().RemoveSpace());

		if (foundPerson is not null)
		{
			_foundId = foundPerson.Id;
			nameTextBox.Text = foundPerson.Name;
			dateOfBirthPicker.SelectedDate = foundPerson.DateOfBirth.ToDateTime(new TimeOnly(0, 0, 0));
			genderTextBox.Text = foundPerson.Gender;
			emailTextBox.Text = foundPerson.Email;
			addressTextBox.Text = foundPerson.Address;
			emergencyNameTextBox.Text = foundPerson.EmergencyName;
			emergencyRelationshipTextBox.Text = foundPerson.EmergencyRelationship;
			emergencyNumberTextBox.Text = foundPerson.EmergencyNumber;
			existingMedicalConditionTextBox.Text = foundPerson.ExistingMedicalCondition;
			currentMedicationTextBox.Text = foundPerson.CurrentlyOnMedication;
			pastSurgeriesTextBox.Text = foundPerson.SurgeriesTwoYears;
			fitnessGoalsTextBox.Text = foundPerson.FitnessGoals;
			fitnessLevelTextBox.Text = foundPerson.CurrentFitnessLevel;
			exerciseTextBox.Text = foundPerson.OftenExercise;
			dietTextBox.Text = foundPerson.Diet;
		}

		else _foundId = 0;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		await PersonData.InsertPerson(new PersonModel
		{
			Id = _foundId,
			Name = nameTextBox.Text.Trim(),
			DateOfBirth = DateOnly.FromDateTime(dateOfBirthPicker.SelectedDate.Value),
			Gender = genderTextBox.Text.Trim(),
			Number = numberTextBox.Text.Trim().RemoveSpace(),
			Email = emailTextBox.Text.Trim().RemoveSpace(),
			Address = addressTextBox.Text.Trim(),
			EmergencyName = emergencyNameTextBox.Text.Trim(),
			EmergencyRelationship = emergencyRelationshipTextBox.Text.Trim(),
			EmergencyNumber = emergencyNumberTextBox.Text.Trim().RemoveSpace(),
			ExistingMedicalCondition = existingMedicalConditionTextBox.Text.Trim(),
			CurrentlyOnMedication = currentMedicationTextBox.Text.Trim(),
			SurgeriesTwoYears = pastSurgeriesTextBox.Text.Trim(),
			FitnessGoals = fitnessGoalsTextBox.Text.Trim(),
			CurrentFitnessLevel = fitnessLevelTextBox.Text.Trim(),
			OftenExercise = exerciseTextBox.Text.Trim(),
			Diet = dietTextBox.Text.Trim()
		});

		_foundId = 0;
		numberTextBox.Clear();
		await ApplySearchFilter();
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrWhiteSpace(nameTextBox.Text))
		{
			MessageBox.Show("Please enter a name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrWhiteSpace(genderTextBox.Text))
		{
			MessageBox.Show("Please enter a Gender.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrWhiteSpace(numberTextBox.Text))
		{
			MessageBox.Show("Please enter a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (dateOfBirthPicker.SelectedDate == null)
		{
			MessageBox.Show("Please select a date of birth.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		await ApplySearchFilter();

	private async Task ApplySearchFilter() =>
		personDataGrid.ItemsSource = await PersonData.LoadPersonByNameNumber(
			searchNameTextBox.Text.Trim().RemoveSpace(),
			searchNumberTextBox.Text.Trim().RemoveSpace());

	private void personDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (personDataGrid.SelectedItem is null) return;

		numberTextBox.Text = ((PersonModel)personDataGrid.SelectedItem).Number;
	}
}
