using Syncfusion.Blazor.Grids;

namespace PilatoWorks.Components.Pages.Reports;

public partial class PeopleReport
{
	[Inject] public NavigationManager NavManager { get; set; }
	[Inject] public IJSRuntime JS { get; set; }

	private readonly string[] _pageSizes = ["5", "10", "15", "20"];

	private List<PersonModel> _personModels = [];
	private SfGrid<PersonModel> _sfPersonGrid;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && !await ValidatePassword()) NavManager.NavigateTo("/");

		if (firstRender)
			await LoadData();
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

	private async Task LoadData()
	{
		_personModels = await CommonData.LoadTableData<PersonModel>(TableNames.Person);
		await _sfPersonGrid.Refresh();
		StateHasChanged();
	}

	private void OnBackClick() =>
		NavManager.NavigateTo("/Reports");

	private async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
	{
		if (args.Item.Id == "_sfPersonGrid_excelexport")
		{
			var summaryItems = new Dictionary<string, object>
			{
				{ "Registered Clients", _personModels.Count },
				{ "Clients With Email", CountClientsWithEmail() },
				{ "Clients With Emergency Contact", CountClientsWithEmergencyContact() },
				{ "Clients With Address Details Contact", CountClientsWithAddress() }
			};

			// Use the generalized Excel exporter
			var stream = ExcelExportUtil.ExportToExcel(
				data: _personModels,
				reportTitle: "Client DETAIL REPORT",
				worksheetName: "Client Details",
				summaryItems: summaryItems);

			// Save the file with a descriptive name
			var fileName = $"Client_Report.xlsx";
			await JS.InvokeVoidAsync("saveAs", Convert.ToBase64String(stream.ToArray()), fileName);
		}
	}

	private void OpenPersonReport(string phoneNumber)
	{
		if (!string.IsNullOrEmpty(phoneNumber))
			NavManager.NavigateTo($"/PersonReport?phoneNumber={phoneNumber}");
	}

	private int CountClientsWithEmail()
	{
		if (_personModels == null) return 0;
		return _personModels.Count(p => !string.IsNullOrEmpty(p.Email));
	}

	private int CountClientsWithEmergencyContact()
	{
		if (_personModels == null) return 0;
		return _personModels.Count(p => !string.IsNullOrEmpty(p.EmergencyName) && !string.IsNullOrEmpty(p.EmergencyNumber));
	}

	private int CountClientsWithAddress()
	{
		if (_personModels == null) return 0;
		return _personModels.Count(p => !string.IsNullOrEmpty(p.Address));
	}
}