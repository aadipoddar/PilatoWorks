
namespace PilatoWorksWeb.Components.Pages.Panels;

public partial class PeoplePage
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private PersonModel _person = new()
	{
		Id = 0,
		DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
		Number = "",
		Name = "",
		Gender = "",
		Email = "",
		Address = "",
		EmergencyName = "",
		EmergencyRelationship = "",
		EmergencyNumber = "",
		ExistingMedicalCondition = "",
		CurrentlyOnMedication = "",
		SurgeriesTwoYears = "",
		FitnessGoals = "",
		CurrentFitnessLevel = "",
		OftenExercise = "",
		Diet = ""
	};

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");
	}

	private async Task<bool> ValidatePassword()
	{
		var userId = await JS.InvokeAsync<string>("getCookie", "UserId");
		var password = await JS.InvokeAsync<string>("getCookie", "Password");

		if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
			return false;

		var user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, int.Parse(userId));
		if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(user.Password.ToString(), password))
			return false;

		return true;
	}

	private async Task OnNumberChanged(string args)
	{
		_person.Number = args;

		var foundPerson = await PersonData.LoadPersonByNumber(_person.Number.Trim().RemoveSpace());

		if (foundPerson is not null)
			_person = foundPerson;

		else
			_person = new()
			{
				Id = 0,
				DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
				Number = "",
				Name = "",
				Gender = "",
				Email = "",
				Address = "",
				EmergencyName = "",
				EmergencyRelationship = "",
				EmergencyNumber = "",
				ExistingMedicalCondition = "",
				CurrentlyOnMedication = "",
				SurgeriesTwoYears = "",
				FitnessGoals = "",
				CurrentFitnessLevel = "",
				OftenExercise = "",
				Diet = ""
			};

		_person.Number = args;
		StateHasChanged();
	}

	private async Task OnSaveClick()
	{
		if (!await ValidateForm())
			return;

		await PersonData.InsertPerson(_person);

		NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
	}

	private async Task<bool> ValidateForm()
	{
		if (string.IsNullOrWhiteSpace(_person.Name))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a name.");
			return false;
		}

		if (string.IsNullOrWhiteSpace(_person.Gender))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a Gender.");
			return false;
		}

		if (string.IsNullOrWhiteSpace(_person.Number))
		{
			await JS.InvokeVoidAsync("alert", "Please enter a number.");
			return false;
		}

		if (string.IsNullOrWhiteSpace(_person.Email)) _person.Email = "";
		if (string.IsNullOrWhiteSpace(_person.Address)) _person.Address = "";
		if (string.IsNullOrWhiteSpace(_person.EmergencyName)) _person.EmergencyName = "";
		if (string.IsNullOrWhiteSpace(_person.EmergencyRelationship)) _person.EmergencyRelationship = "";
		if (string.IsNullOrWhiteSpace(_person.EmergencyNumber)) _person.EmergencyNumber = "";
		if (string.IsNullOrWhiteSpace(_person.ExistingMedicalCondition)) _person.ExistingMedicalCondition = "";
		if (string.IsNullOrWhiteSpace(_person.CurrentlyOnMedication)) _person.CurrentlyOnMedication = "";
		if (string.IsNullOrWhiteSpace(_person.SurgeriesTwoYears)) _person.SurgeriesTwoYears = "";
		if (string.IsNullOrWhiteSpace(_person.FitnessGoals)) _person.FitnessGoals = "";
		if (string.IsNullOrWhiteSpace(_person.CurrentFitnessLevel)) _person.CurrentFitnessLevel = "";
		if (string.IsNullOrWhiteSpace(_person.OftenExercise)) _person.OftenExercise = "";
		if (string.IsNullOrWhiteSpace(_person.Diet)) _person.Diet = "";

		return true;
	}

	private void OnCancelClick() =>
		NavManager.NavigateTo("/");
}