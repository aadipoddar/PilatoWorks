using System.Globalization;
using System.Windows.Controls;

namespace PilatoWorks.Common;

public static class Helper
{
	public static string RemoveSpace(this string str) => str.Replace(" ", "");

	public static string FormatIndianCurrency(this decimal rate)
	{
		var hindi = new CultureInfo("hi-IN");
		return string.Format(hindi, "{0:C}", rate);
	}

	public static string FormatIndianCurrency(this decimal? rate)
	{
		rate ??= 0;

		var hindi = new CultureInfo("hi-IN");
		return string.Format(hindi, "{0:C}", rate);
	}

	public static string FormatIndianCurrency(this int rate)
	{
		var hindi = new CultureInfo("hi-IN");
		return string.Format(hindi, "{0:C}", rate);
	}

	public static void ValidateIntegerInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		e.Handled = !int.TryParse(e.Text, out _);

	public static void ValidateDecimalInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		bool approvedDecimalPoint = false;

		if (e.Text == ".")
		{
			if (!((TextBox)sender).Text.Contains('.'))
				approvedDecimalPoint = true;
		}

		if (!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint))
			e.Handled = true;
	}
}

