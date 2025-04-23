using System.Windows;

namespace PilatoWorks.Person;

/// <summary>
/// Interaction logic for CreatePerson.xaml
/// </summary>
public partial class ManagePersonWindow : Window
{
	private int _foundId = 0;

	public ManagePersonWindow() => InitializeComponent();

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private async void numberTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
		if (string.IsNullOrWhiteSpace(nameTextBox.Text))
		{
			MessageBox.Show("Please enter a name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (string.IsNullOrWhiteSpace(genderTextBox.Text))
		{
			MessageBox.Show("Please enter a Gender.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (string.IsNullOrWhiteSpace(numberTextBox.Text))
		{
			MessageBox.Show("Please enter a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (dateOfBirthPicker.SelectedDate == null)
		{
			MessageBox.Show("Please select a date of birth.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

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

		Close();
	}
}
