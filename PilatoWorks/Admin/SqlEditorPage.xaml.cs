using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Data.SqlClient;

namespace PilatoWorks.Admin;

/// <summary>
/// Interaction logic for SqlEditorPage.xaml
/// </summary>
public partial class SqlEditorPage : Page
{
	public SqlEditorPage() => InitializeComponent();

	private async void sqlEditControl_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.F5)
			await RunQuery();
	}

	private async void executeButton_Click(object sender, RoutedEventArgs e) => await RunQuery();

	private async Task RunQuery()
	{
		tabControl1.Items.Clear();
		var selectedText = sqlEditControl.SelectedText;
		if (string.IsNullOrWhiteSpace(selectedText))
			selectedText = sqlEditControl.Text;
		var queries = selectedText.Split([";"], StringSplitOptions.RemoveEmptyEntries);
		await ExecuteQueries(queries);
	}

	private async Task ExecuteQueries(string[] queries)
	{
		SqlConnection connection = new(ConnectionStrings.Azure);
		await connection.OpenAsync();

		foreach (var query in queries)
		{
			await using SqlCommand command = new(query.Trim(), connection);
			try
			{
				await using var reader = await command.ExecuteReaderAsync();
				if (reader.HasRows)
				{
					DataTable dt = new();

					for (var i = 0; i < reader.FieldCount; i++)
						dt.Columns.Add(reader.GetName(i));

					while (reader.Read())
					{
						var row = dt.NewRow();
						for (var i = 0; i < reader.FieldCount; i++)
							row[i] = reader[i];
						dt.Rows.Add(row);
					}

					TabItem tabItem = new() { Header = $"Query {queries.ToList().IndexOf(query) + 1}" };
					DataGrid dataGrid = new()
					{
						ItemsSource = dt.DefaultView,
						IsReadOnly = true,
						AutoGenerateColumns = true,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Stretch
					};
					tabItem.Content = dataGrid;
					tabControl1.Items.Add(tabItem);
				}
			}
			catch (SqlException ex)
			{
				TabItem tabItem = new() { Header = $"Query {queries.ToList().IndexOf(query) + 1}" };
				TextBox textBox = new()
				{
					Text = $"""
                            Error executing query:
                            {query}

                            {ex.Message}
                            """,
					IsReadOnly = true,
					TextWrapping = TextWrapping.Wrap,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch
				};
				tabItem.Content = textBox;
				tabControl1.Items.Add(tabItem);
			}
		}
	}
}
