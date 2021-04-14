using NUnit.Framework;

namespace DL.FastCsvParser.Tests
{
	// NOTE: Although this class tests the CSV conversion to string, it relies on the parsing being correct
	// because the tests create the Csv object with a string instead of hand-coding lists of lists.
	public class CsvExporterTests
	{
		[Test]
		public void Empty()
		{
			ValidateExport("");
		}

		[Test]
		public void EOF()
		{
			ValidateExport("Year");
		}

		[Test]
		public void SeparatorEOF()
		{
			ValidateExport("Year,");
		}

		[Test]
		public void SeparatorMultiple()
		{
			ValidateExport("Year,Stuff,Something");
		}

		[Test]
		public void QuotesWithCommaInside()
		{
			ValidateExport("\"Year,Stuff\"");
		}

		[Test]
		public void QuotesWithQuotesInside()
		{
			ValidateExport("\"Year \"\"The BEST\"\" Stuff\"");
		}

		[Test]
		public void QuotesEndingInQuotesInside()
		{
			ValidateExport("\"Year Stuff\"\"\"");
		}

		[Test]
		public void QuotesWithLFInside()
		{
			ValidateExport("\"Year\nStuff\"");
		}

		[Test]
		public void QuotesWithCRLFInside()
		{
			ValidateExport("\"Year\r\nStuff\"");
		}

		[Test]
		public void QuotesWithCRLFAndCommasAndStuffInside()
		{
			ValidateExport("\"Year\r\n,\nasdasd,\"\"asdasd\"\"\nStuff\"\"\n\"\"\"");
		}

		[Test]
		public void EmptyStrings1()
		{
			ValidateExport(",");
		}

		[Test]
		public void EmptyStrings2()
		{
			ValidateExport(",,");
		}

		[Test]
		public void EmptyStrings3()
		{
			ValidateExport(",,,,");
		}

		[Test]
		public void QuotedFieldAfterSeparator()
		{
			ValidateExport("Year,\"asd,\"");
		}

		[Test]
		public void SeparatorAndFieldAfterQuotedField()
		{
			ValidateExport("\"Year,\",something");
		}

		[Test]
		public void AbruptSeparatorCRLF()
		{
			ValidateExport(string.Join("\r\n", new string[] { "Year,", "1997," }));
		}

		[Test]
		public void QuotedFieldAfterCRLF()
		{
			ValidateExport(string.Join("\r\n", new string[] { "Year", "\",asd\"" }));
		}

		[Test]
		public void CRLFAndFieldAfterQuotedField()
		{
			ValidateExport(string.Join("\r\n", new string[] { "\",Year\"", "something" }));
		}

		[Test]
		public void SpacesInLines()
		{
			ValidateExport(string.Join("\r\n", new string[] { " ", " ", " " }));
		}

		[Test]
		public void EmptyLineAtStartOfText()
		{
			ValidateExport(string.Join("\r\n", new string[] { "", "Year" }));
		}

		[Test]
		public void LastColumnIsAllEmptyFields1()
		{
			ValidateExport(string.Join("\r\n", new string[] { ",", ",", "," }));
		}

		[Test]
		public void LastColumnIsAllEmptyFields2()
		{
			ValidateExport(string.Join("\r\n", new string[] { ",,,", ",,,", ",,," }));
		}

		[Test]
		public void WikipediaTest()
		{
			ValidateExport(string.Join("\r\n", new string[] {
				"Year,Make,Model,Description,Price",
				"1997,Ford,E350,\"ac, abs, moon\",3000.00",
				"1999,Chevy,\"Venture \"\"Extended Edition\"\"\",,4900.00",	// removed empty quote for 4th element
				"1999,Chevy,\"Venture \"\"Extended Edition, Very Large\"\"\",,5000.00",
				"1996,Jeep,Grand Cherokee,\"MUST SELL!\nair, moon roof, loaded\",4799.00"
			}));
		}

		[Test]
		public void UnicodeChars()
		{
			ValidateExport(string.Join("\r\n", new string[] {
				"日本語が読めない,日本語が読めない",
				"日本語が読めない,日本語が読めない",
			}));
		}

		private static void ValidateExport(string csvContents)
		{
			Csv csv = Csv.Parse(csvContents);
			string exportedContents = csv.ToString();
			Assert.That(exportedContents == csvContents, () => $"Exported string is different!\n\nExpected:\n\n-->{exportedContents}<--\n\nActual:\n\n-->{csvContents}<--");
		}
	}
}