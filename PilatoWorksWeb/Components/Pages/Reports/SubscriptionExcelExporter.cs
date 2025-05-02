using Syncfusion.Drawing;
using Syncfusion.XlsIO;

namespace PilatoWorksWeb.Components.Pages.Reports;

public static class SubscriptionExcelExporter
{
	public static async Task<MemoryStream> ExportSubscriptionDetailsToExcel(List<SubscriptionDetailsModel> subscriptions, DateOnly startDate, DateOnly endDate)
	{
		MemoryStream ms = new();
		using (ExcelEngine excelEngine = new())
		{
			IApplication application = excelEngine.Excel;
			application.DefaultVersion = ExcelVersion.Xlsx;

			// Create a workbook with a worksheet
			IWorkbook workbook = application.Workbooks.Create(1);
			IWorksheet worksheet = workbook.Worksheets[0];
			worksheet.Name = "Subscription Details";

			// Apply document properties
			workbook.BuiltInDocumentProperties.Title = "Subscription Detail Report";
			workbook.BuiltInDocumentProperties.Subject = "Subscription Details";
			workbook.BuiltInDocumentProperties.Author = "Reflex and Reform Pilates";

			// Set up the header section
			SetupHeader(worksheet, startDate, endDate);

			// Add summary statistics section
			int currentRow = AddSummarySection(worksheet, subscriptions);

			// Add subscription data table
			currentRow = FillSubscriptionData(worksheet, subscriptions, currentRow);

			// Apply final formatting and save
			ApplyFinalFormatting(worksheet);
			workbook.SaveAs(ms);
		}

		ms.Position = 0;
		return ms;
	}

	private static void SetupHeader(IWorksheet worksheet, DateOnly startDate, DateOnly endDate)
	{
		string dateRange = $"{startDate:dd MMM yyyy} - {endDate:dd MMM yyyy}";

		// Company Logo Placeholder (Could be added if you have a logo file)
		// worksheet.Pictures.AddPicture(1, 1, "path/to/logo.png");

		// Main header with date range
		IRange headerRange = worksheet.Range["A1:J1"];
		headerRange.Merge();
		headerRange.Text = "SUBSCRIPTION DETAIL REPORT";
		headerRange.CellStyle.Font.Bold = true;
		headerRange.CellStyle.Font.Size = 20;
		headerRange.CellStyle.Font.FontName = "Calibri";
		headerRange.CellStyle.Font.RGBColor = Color.FromArgb(63, 81, 181); // Primary color
		headerRange.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

		// Date range subtitle
		IRange dateRange1 = worksheet.Range["A2:J2"];
		dateRange1.Merge();
		dateRange1.Text = dateRange;
		dateRange1.CellStyle.Font.Size = 12;
		dateRange1.CellStyle.Font.FontName = "Calibri";
		dateRange1.CellStyle.Font.Bold = true;
		dateRange1.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
		dateRange1.CellStyle.Font.RGBColor = Color.FromArgb(96, 125, 139); // Bluish gray

		// Company Name
		IRange companyRange = worksheet.Range["A3:J3"];
		companyRange.Merge();
		companyRange.Text = "Reflex and Reform Pilates";
		companyRange.CellStyle.Font.Size = 14;
		companyRange.CellStyle.Font.FontName = "Calibri";
		companyRange.CellStyle.Font.Bold = true;
		companyRange.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
		companyRange.CellStyle.Font.RGBColor = Color.FromArgb(0, 150, 136); // Teal accent

		// Add decorative header background
		IRange headerBackgroundRange = worksheet.Range["A1:J3"];
		headerBackgroundRange.CellStyle.Color = Color.FromArgb(240, 244, 255); // Light blue background

		// Add border bottom for header section
		IRange borderRange = worksheet.Range["A3:J3"];
		borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
		borderRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(63, 81, 181);

		// Space after header
		worksheet.Range["A4:J4"].RowHeight = 10;
	}

	private static int AddSummarySection(IWorksheet worksheet, List<SubscriptionDetailsModel> subscriptions)
	{
		int startRow = 5;

		// Summary Title
		IRange summaryTitleRange = worksheet.Range[$"A{startRow}:J{startRow}"];
		summaryTitleRange.Merge();
		summaryTitleRange.Text = "SUMMARY INFORMATION";
		summaryTitleRange.CellStyle.Font.Bold = true;
		summaryTitleRange.CellStyle.Font.Size = 12;
		summaryTitleRange.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
		summaryTitleRange.CellStyle.Color = Color.FromArgb(232, 245, 233); // Light green background
		summaryTitleRange.CellStyle.Font.RGBColor = Color.FromArgb(27, 94, 32); // Dark green text

		startRow++;

		// Calculate summary values
		int totalSubscriptions = subscriptions.Count;
		int totalSessions = subscriptions.Sum(s => s.NoOfSessions);
		int remainingSessions = subscriptions.Sum(s => s.RemainingSessions);
		decimal totalRevenue = subscriptions.Sum(s => s.TotalPaid);
		decimal totalDues = subscriptions.Sum(s => s.Dues);

		// Summary Statistics Layout - Create a 3x3 grid
		string[,] summaryLabels = new string[,] {
			{ "Total Subscriptions", "Total Sessions", "Total Remaining" },
			{ totalSubscriptions.ToString(), totalSessions.ToString(), remainingSessions.ToString() },
			{ "Total Revenue (₹)", "Outstanding Dues (₹)", "" },
			{ totalRevenue.ToString("N2"), totalDues.ToString("N2"), "" }
		};

		// Create a nice summary grid
		for (int row = 0; row < summaryLabels.GetLength(0); row++)
		{
			for (int col = 0; col < summaryLabels.GetLength(1); col++)
			{
				IRange cell = worksheet.Range[startRow + row, (col * 3) + 1, startRow + row, (col * 3) + 3];
				cell.Merge();
				cell.Text = summaryLabels[row, col];

				// Style header rows
				if (row == 0 || row == 2)
				{
					cell.CellStyle.Font.Bold = true;
					cell.CellStyle.Font.Size = 11;
					cell.CellStyle.Color = Color.FromArgb(232, 234, 246); // Light indigo
					cell.CellStyle.Font.RGBColor = Color.FromArgb(63, 81, 181); // Indigo
				}
				// Style value rows
				else
				{
					cell.CellStyle.Font.Size = 14;
					cell.CellStyle.Font.Bold = true;
					cell.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

					// Add specific styling based on values
					if (row == 3 && col == 1 && totalDues > 0) // Outstanding dues
					{
						cell.CellStyle.Font.RGBColor = Color.FromArgb(198, 40, 40); // Red for dues
					}
					else if (row == 1) // Count values
					{
						cell.CellStyle.Font.RGBColor = Color.FromArgb(21, 101, 192); // Blue for counts
					}
					else if (row == 3) // Amount values
					{
						cell.CellStyle.Font.RGBColor = Color.FromArgb(46, 125, 50); // Green for money
					}
				}
			}
		}

		// Add space after summary section
		startRow += 5;
		worksheet.Range[$"A{startRow}:J{startRow}"].RowHeight = 15;

		return startRow + 1;
	}

	private static int FillSubscriptionData(IWorksheet worksheet, List<SubscriptionDetailsModel> subscriptions, int startRow)
	{
		// Create table title
		IRange tableTitleRange = worksheet.Range[$"A{startRow}:J{startRow}"];
		tableTitleRange.Merge();
		tableTitleRange.Text = "DETAILED SUBSCRIPTION DATA";
		tableTitleRange.CellStyle.Font.Bold = true;
		tableTitleRange.CellStyle.Font.Size = 12;
		tableTitleRange.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
		tableTitleRange.CellStyle.Color = Color.FromArgb(225, 245, 254); // Light blue background
		tableTitleRange.CellStyle.Font.RGBColor = Color.FromArgb(13, 71, 161); // Dark blue text

		startRow++;

		// Define column headers
		string[] headers = new string[]
		{
			"Client Name", "Phone Number", "Subscription ID", "Valid From", "Valid To",
			"Session Type", "Total Sessions", "Remaining", "Amount Paid (₹)", "Dues (₹)"
		};

		// Apply header styles
		for (int i = 0; i < headers.Length; i++)
		{
			IRange headerCell = worksheet.Range[startRow, i + 1];
			headerCell.Text = headers[i];
			headerCell.CellStyle.Font.Bold = true;
			headerCell.CellStyle.Color = Color.FromArgb(63, 81, 181); // Primary color background
			headerCell.CellStyle.Font.RGBColor = Color.White;
			headerCell.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
			headerCell.CellStyle.Borders.ColorRGB = Color.FromArgb(48, 63, 159);
		}

		startRow++;

		// Fill data rows
		int rowIndex = startRow;
		foreach (var subscription in subscriptions)
		{
			// Set data in each cell
			worksheet.Range[$"A{rowIndex}"].Text = subscription.PersonName;
			worksheet.Range[$"B{rowIndex}"].Text = subscription.PersonNumber;
			worksheet.Range[$"C{rowIndex}"].Number = subscription.SubscriptionId;
			worksheet.Range[$"D{rowIndex}"].DateTime = subscription.SubscriptionValidFrom.ToDateTime(TimeOnly.MinValue);
			worksheet.Range[$"D{rowIndex}"].CellStyle.NumberFormat = "dd-MMM-yyyy";
			worksheet.Range[$"E{rowIndex}"].DateTime = subscription.SubscriptionValidTo.ToDateTime(TimeOnly.MinValue);
			worksheet.Range[$"E{rowIndex}"].CellStyle.NumberFormat = "dd-MMM-yyyy";
			worksheet.Range[$"F{rowIndex}"].Text = subscription.SessionType;
			worksheet.Range[$"G{rowIndex}"].Number = subscription.NoOfSessions;
			worksheet.Range[$"H{rowIndex}"].Number = subscription.RemainingSessions;
			worksheet.Range[$"I{rowIndex}"].Number = subscription.TotalPaid;
			worksheet.Range[$"J{rowIndex}"].Number = subscription.Dues;

			// Style data row
			IRange dataRow = worksheet.Range[$"A{rowIndex}:J{rowIndex}"];
			dataRow.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
			dataRow.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.LightGray;

			// Alternate row colors
			if (rowIndex % 2 == 0)
			{
				dataRow.CellStyle.Color = Color.FromArgb(248, 249, 250);
			}

			// Style remaining sessions with conditional formatting
			if (subscription.RemainingSessions == 0)
			{
				worksheet.Range[$"H{rowIndex}"].CellStyle.Font.RGBColor = Color.FromArgb(198, 40, 40); // Red
				worksheet.Range[$"H{rowIndex}"].CellStyle.Font.Bold = true;
			}
			else if (subscription.RemainingSessions <= 3)
			{
				worksheet.Range[$"H{rowIndex}"].CellStyle.Font.RGBColor = Color.FromArgb(239, 108, 0); // Orange
				worksheet.Range[$"H{rowIndex}"].CellStyle.Font.Bold = true;
			}
			else
			{
				worksheet.Range[$"H{rowIndex}"].CellStyle.Font.RGBColor = Color.FromArgb(56, 142, 60); // Green
			}

			// Style dues with conditional formatting
			if (subscription.Dues > 0)
			{
				worksheet.Range[$"J{rowIndex}"].CellStyle.Font.RGBColor = Color.FromArgb(198, 40, 40); // Red
				worksheet.Range[$"J{rowIndex}"].CellStyle.Font.Bold = true;
			}

			rowIndex++;
		}

		// Create table with borders
		IRange tableRange = worksheet.Range[$"A{startRow}:J{rowIndex - 1}"];
		tableRange.CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
		tableRange.CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
		tableRange.CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
		tableRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

		// Apply number format for currency columns
		worksheet.Range[$"I{startRow}:J{rowIndex - 1}"].CellStyle.NumberFormat = "₹#,##0.00";

		// Set alignment for specific columns
		worksheet.Range[$"A{startRow}:A{rowIndex - 1}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
		worksheet.Range[$"B{startRow}:C{rowIndex - 1}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
		worksheet.Range[$"D{startRow}:E{rowIndex - 1}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
		worksheet.Range[$"F{startRow}:F{rowIndex - 1}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
		worksheet.Range[$"G{startRow}:H{rowIndex - 1}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
		worksheet.Range[$"I{startRow}:J{rowIndex - 1}"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;

		// Add grand total row
		rowIndex += 1;
		IRange totalLabelRange = worksheet.Range[$"A{rowIndex}:F{rowIndex}"];
		totalLabelRange.Merge();
		totalLabelRange.Text = "GRAND TOTAL";
		totalLabelRange.CellStyle.Font.Bold = true;
		totalLabelRange.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
		totalLabelRange.CellStyle.Color = Color.FromArgb(232, 234, 246); // Light indigo
		totalLabelRange.CellStyle.Font.RGBColor = Color.FromArgb(40, 53, 147); // Dark indigo

		// Total formulas
		worksheet.Range[$"G{rowIndex}"].Formula = $"=SUM(G{startRow}:G{rowIndex - 1})";
		worksheet.Range[$"H{rowIndex}"].Formula = $"=SUM(H{startRow}:H{rowIndex - 1})";
		worksheet.Range[$"I{rowIndex}"].Formula = $"=SUM(I{startRow}:I{rowIndex - 1})";
		worksheet.Range[$"J{rowIndex}"].Formula = $"=SUM(J{startRow}:J{rowIndex - 1})";

		// Style total row
		IRange totalRow = worksheet.Range[$"G{rowIndex}:J{rowIndex}"];
		totalRow.CellStyle.Font.Bold = true;
		totalRow.CellStyle.Color = Color.FromArgb(232, 234, 246); // Light indigo
		totalRow.CellStyle.Font.RGBColor = Color.FromArgb(40, 53, 147); // Dark indigo
		totalRow.CellStyle.NumberFormat = "₹#,##0.00";

		return rowIndex + 2;
	}

	private static void ApplyFinalFormatting(IWorksheet worksheet)
	{
		try
		{
			// AutoFit columns for better readability (this might be causing issues if there's no data)
			if (worksheet.UsedRange != null)
			{
				worksheet.UsedRange.AutofitColumns();
			}

			// Get the actual number of columns in use
			int columnCount = 0;
			try
			{
				columnCount = worksheet.UsedRange.LastColumn;
			}
			catch
			{
				// Default to 10 columns if we can't determine the count
				columnCount = 10;
			}

			// Set minimum column widths to avoid too narrow columns
			// Columns collection might be 0-indexed or 1-indexed depending on the library version
			for (int i = 1; i <= Math.Min(columnCount, 10); i++)
			{
				try
				{
					double width = worksheet.Columns[i].ColumnWidth;

					if (width < 8)
					{
						worksheet.Columns[i].ColumnWidth = 8;
					}
					else if (width > 30)
					{
						worksheet.Columns[i].ColumnWidth = 30;
					}
				}
				catch
				{
					// Skip this column if there's an issue
					continue;
				}
			}

			// Ensure the client name column has enough space
			try
			{
				if (columnCount >= 1)
				{
					worksheet.Columns[1].ColumnWidth = 25;
				}
			}
			catch
			{
				// If setting column width fails, continue with the method
			}

			// Add footer with date and page numbers
			worksheet.PageSetup.CenterFooter = "&D &T";
			worksheet.PageSetup.RightFooter = "Page &P of &N";

			// Set print options
			worksheet.PageSetup.Orientation = ExcelPageOrientation.Landscape;
			worksheet.PageSetup.FitToPagesTall = 0;
			worksheet.PageSetup.FitToPagesWide = 1;
			worksheet.PageSetup.LeftMargin = 0.25;
			worksheet.PageSetup.RightMargin = 0.25;
			worksheet.PageSetup.TopMargin = 0.5;
			worksheet.PageSetup.BottomMargin = 0.5;
			worksheet.PageSetup.HeaderMargin = 0.3;
			worksheet.PageSetup.FooterMargin = 0.3;
		}
		catch (Exception ex)
		{
			// If there's an error in formatting, log it or handle it, but don't let it prevent the Excel file from being created
			Console.WriteLine($"Error in ApplyFinalFormatting: {ex.Message}");
		}
	}

}
