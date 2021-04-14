using System.Collections.Generic;
using NUnit.Framework;

namespace DL.FastCsvParser.Tests
{
	public class CsvParserTests
	{
		[Test]
		public void Empty()
		{
			const string testCsvContents = "";
			var expectedCsv = new Csv { new List<string> { "" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void EOF()
		{
			const string testCsvContents = "Year";
			var expectedCsv = new Csv { new List<string> { "Year" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void LF()
		{
			const string testCsvContents = "Year\n";
			var expectedCsv = new Csv { new List<string> { "Year" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void CRLF()
		{
			const string testCsvContents = "Year\r\n";
			var expectedCsv = new Csv { new List<string> { "Year" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void SeparatorEOF()
		{
			const string testCsvContents = "Year,";
			var expectedCsv = new Csv { new List<string> { "Year", "" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void SeparatorLF()
		{
			const string testCsvContents = "Year,\n";
			var expectedCsv = new Csv { new List<string> { "Year", "" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void SeparatorCRLF()
		{
			const string testCsvContents = "Year,\r\n";
			var expectedCsv = new Csv { new List<string> { "Year", "" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void SeparatorBeforeField()
		{
			const string testCsvContents = ",Year";
			var expectedCsv = new Csv { new List<string> { "", "Year" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void Quotes()
		{
			const string testCsvContents = "\"Year\"";
			var expectedCsv = new Csv { new List<string> { "Year" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotesWithCommaInside()
		{
			const string testCsvContents = "\"Year,Stuff\"";
			var expectedCsv = new Csv { new List<string> { "Year,Stuff" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotesWithQuotesInside()
		{
			const string testCsvContents = "\"Year \"\"The BEST\"\" Stuff\"";
			var expectedCsv = new Csv { new List<string> { "Year \"The BEST\" Stuff" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotesEndingInQuotesInside()
		{
			const string testCsvContents = "\"Year Stuff\"\"\"";
			var expectedCsv = new Csv { new List<string> { "Year Stuff\"" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotesWithLFInside()
		{
			const string testCsvContents = "\"Year\nStuff\"";
			var expectedCsv = new Csv { new List<string> { "Year\nStuff" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotesWithCRLFInside()
		{
			const string testCsvContents = "\"Year\r\nStuff\"";
			var expectedCsv = new Csv { new List<string> { "Year\r\nStuff" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotesWithCRLFAndCommasAndStuffInside()
		{
			const string testCsvContents = "\"Year\r\n,\nasdasd,\"\"asdasd\"\"\nStuff\"\"\n\"\"\"";
			var expectedCsv = new Csv { new List<string> { "Year\r\n,\nasdasd,\"asdasd\"\nStuff\"\n\"" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void EmptyQuotedField()
		{
			const string testCsvContents = "\"\"";
			var expectedCsv = new Csv { new List<string> { "" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void EscapedQuotesInMultipleFields()
		{
			const string testCsvContents = "\"Ye\"\"ar\",\"St\"\"uff\""
								  + "\n" + "\"St\"\"uff\",\"Ye\"\"ar\"";

			var expectedCsv = new Csv {
				new List<string> { "Ye\"ar", "St\"uff" },
				new List<string> { "St\"uff", "Ye\"ar" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotesInsideNonQuotedField()
		{
			const string testCsvContents = "Ye\"ar";
			ValidateInvalidParse(testCsvContents);
		}

		[Test]
		public void QuotesInsideNonQuotedField2()
		{
			const string testCsvContents = "Year\"";
			ValidateInvalidParse(testCsvContents);
		}

		[Test]
		public void QuotesAtStartOfFieldButNoEndQuotes()
		{
			const string testCsvContents = "\"Year";
			ValidateInvalidParse(testCsvContents);
		}

		[Test]
		public void TextBeforeQuotedField()
		{
			const string testCsvContents = "Year, \"Stuff\"";
			ValidateInvalidParse(testCsvContents);
		}

		[Test]
		public void TextBeforeQuotedField2()
		{
			const string testCsvContents = "Year,asd\"Stuff\"";
			ValidateInvalidParse(testCsvContents);
		}

		[Test]
		public void TextAfterQuotedField()
		{
			const string testCsvContents = "\"Year\"  ,";
			ValidateInvalidParse(testCsvContents);
		}

		[Test]
		public void TextAfterQuotedField2()
		{
			const string testCsvContents = "\"Year\"asd,";
			ValidateInvalidParse(testCsvContents);
		}

		[Test]
		public void EmptyStrings1()
		{
			const string testCsvContents = ",";
			var expectedCsv = new Csv { new List<string> { "", "" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void EmptyStrings2()
		{
			const string testCsvContents = ",,";
			var expectedCsv = new Csv { new List<string> { "", "", "" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void EmptyStrings3()
		{
			const string testCsvContents = ",,,,";
			var expectedCsv = new Csv { new List<string> { "", "", "", "", "" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void EmptyStringsBetweenFields()
		{
			const string testCsvContents = "Some,,Thing";
			var expectedCsv = new Csv { new List<string> { "Some", "", "Thing" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void LineWithEOF()
		{
			const string testCsvContents = "Year,Make,Model,Description,Price";
			var expectedCsv = new Csv { new List<string> { "Year", "Make", "Model", "Description", "Price" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void LineWithLF()
		{
			const string testCsvContents = "Year,Make,Model,Description,Price\n";
			var expectedCsv = new Csv { new List<string> { "Year", "Make", "Model", "Description", "Price" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void LineWithCRLF()
		{
			const string testCsvContents = "Year,Make,Model,Description,Price\r\n";
			var expectedCsv = new Csv { new List<string> { "Year", "Make", "Model", "Description", "Price" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void CRInsideQuotedField()
		{
			const string testCsvContents = "\"Ye\rar\"";
			var expectedCsv = new Csv { new List<string> { "Ye\rar" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotedFieldAfterSeparator()
		{
			const string testCsvContents = "Year,\"asd\"";
			var expectedCsv = new Csv { new List<string> { "Year", "asd" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotedFieldAfterEmptyFieldAndSeparator()
		{
			const string testCsvContents = ",\"asd\"";

			var expectedCsv = new Csv {
				new List<string> { "", "asd" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotedFieldAfterEmptyFieldAndSeparator2()
		{
			const string testCsvContents = ",\"asd\""
								  + "\n" + ",\"1997\"";

			var expectedCsv = new Csv {
				new List<string> { "", "asd" },
				new List<string> { "", "1997" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void SeparatorAndFieldAfterQuotedField()
		{
			const string testCsvContents = "\"Year\",something";
			var expectedCsv = new Csv { new List<string> { "Year", "something" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void AbruptSeparatorLF()
		{
			const string testCsvContents = "Year,"
								  + "\n" + "1997,";

			var expectedCsv = new Csv {
				new List<string> { "Year", "" },
				new List<string> { "1997", "" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void AbruptSeparatorCRLF()
		{
			const string testCsvContents = "Year,"
								  + "\r\n" + "1997,";

			var expectedCsv = new Csv {
				new List<string> { "Year", "" },
				new List<string> { "1997", "" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotedFieldAfterLF()
		{
			const string testCsvContents = "Year\n\"asd\"";

			var expectedCsv = new Csv {
				new List<string> { "Year" },
				new List<string> { "asd" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void QuotedFieldAfterCRLF()
		{
			const string testCsvContents = "Year\r\n\"asd\"";

			var expectedCsv = new Csv {
				new List<string> { "Year" },
				new List<string> { "asd" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void LFAndFieldAfterQuotedField()
		{
			const string testCsvContents = "\"Year\"\nsomething";

			var expectedCsv = new Csv {
				new List<string> { "Year" },
				new List<string> { "something" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void EmptyFirstLineCRLF()
		{
			const string testCsvContents = ""
								  + "\r\n" + "Year";

			var expectedCsv = new Csv {
				new List<string> { "" },
				new List<string> { "Year" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void EmptyFirstLineLF()
		{
			const string testCsvContents = ""
								  + "\n" + "Year";

			var expectedCsv = new Csv {
				new List<string> { "" },
				new List<string> { "Year" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void CRLFAndFieldAfterQuotedField()
		{
			const string testCsvContents = "\"Year\"\r\nsomething";

			var expectedCsv = new Csv {
				new List<string> { "Year" },
				new List<string> { "something" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void CRInMiddleOfField()
		{
			const string testCsvContents = "Ye\rar";

			var expectedCsv = new Csv {
				new List<string> { "Ye" },
				new List<string> { "ar" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void CRAtEndOfText()
		{
			const string testCsvContents = "Year\r";
			var expectedCsv = new Csv { new List<string> { "Year" } };
			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void CRAtStartOfText()
		{
			const string testCsvContents = "\rYear";

			var expectedCsv = new Csv {
				new List<string> { "" },
				new List<string> { "Year" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void DifferentNumberOfCellsPerRecord()
		{
			const string testCsvContents = "Year,Stuff"
								  + "\n" + "1997";
			ValidateInvalidParse(testCsvContents);
		}

		[Test]
		public void DifferentNumberOfCellsPerRecord2()
		{
			const string testCsvContents = ",,,"
								+ "\r\n" + ",,"
								+ "\r\n" + ",";
			ValidateInvalidParse(testCsvContents);
		}

		[Test]
		public void SpacesInLines()
		{
			const string testCsvContents = " "
								+ "\r\n" + " "
								+ "\r\n" + " ";

			var expectedCsv = new Csv {
				new List<string> { " " },
				new List<string> { " " },
				new List<string> { " " },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void LastColumnIsAllEmptyFields1()
		{
			const string testCsvContents = ""
								+ "\r\n" + ""
								+ "\r\n" + "";

			var expectedCsv = new Csv {
				new List<string> { "" },
				new List<string> { "" },
				// NOTE: the last line is actually interpreted as the final optional empty line
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void LastColumnIsAllEmptyFields2()
		{
			const string testCsvContents = ","
								+ "\r\n" + ","
								+ "\r\n" + ",";

			var expectedCsv = new Csv {
				new List<string> { "", "" },
				new List<string> { "", "" },
				new List<string> { "", "" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void LastColumnIsAllEmptyFields3()
		{
			const string testCsvContents = ",,,"
								  + "\n" + ",,,"
								  + "\n" + ",,,";

			var expectedCsv = new Csv {
				new List<string> { "", "", "", "" },
				new List<string> { "", "", "", "" },
				new List<string> { "", "", "", "" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void WikipediaTest()
		{
			const string testCsvContents = "Year,Make,Model,Description,Price"
								  + "\n" + "1997,Ford,E350,\"ac, abs, moon\",3000.00"
								  + "\n" + "1999,Chevy,\"Venture \"\"Extended Edition\"\"\",\"\",4900.00"
								  + "\n" + "1999,Chevy,\"Venture \"\"Extended Edition, Very Large\"\"\",,5000.00"
								  + "\n" + "1996,Jeep,Grand Cherokee,\"MUST SELL!\nair, moon roof, loaded\",4799.00";

			var expectedCsv = new Csv {
				new List<string> { "Year", "Make", "Model", "Description", "Price" },
				new List<string> { "1997", "Ford", "E350", "ac, abs, moon", "3000.00" },
				new List<string> { "1999", "Chevy", "Venture \"Extended Edition\"", "", "4900.00" },
				new List<string> { "1999", "Chevy", "Venture \"Extended Edition, Very Large\"", "", "5000.00" },
				new List<string> { "1996", "Jeep", "Grand Cherokee", "MUST SELL!\nair, moon roof, loaded", "4799.00" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		[Test]
		public void UnicodeChars()
		{
			const string testCsvContents = "日本語が読めない,日本語が読めない"
								  + "\n" + "日本語が読めない,日本語が読めない";

			var expectedCsv = new Csv {
				new List<string> { "日本語が読めない", "日本語が読めない" },
				new List<string> { "日本語が読めない", "日本語が読めない" },
			};

			ValidateParse(testCsvContents, expectedCsv);
		}

		private static void ValidateParse(string csvContents, Csv expectedCsv)
		{
			Csv csv = Csv.Parse(csvContents);
			Assert.That(csv != null, () => "Parse was unsuccessful!");
			// NOTE: This assertion message relies on Csv.ToString() working nicely (that's tested in another set of tests).
			Assert.That(AreCsvsEqual(expectedCsv, csv), () => $"Expected:\n\n-->{expectedCsv}<--\n\nActual:\n\n-->{csv}<--");
		}

		private static void ValidateInvalidParse(string csvContents)
		{
			Csv csv = Csv.Parse(csvContents);
			// NOTE: This assertion message relies on Csv.ToString() working nicely (that's tested in another set of tests).
			Assert.That(csv == null, () => $"Parse was successful but it shouldn't have been!\n\nActual was:\n\n-->{csv}<--");
		}

		private static bool AreCsvsEqual(Csv expected, Csv actual)
		{
			if (expected is null && actual is null) return true;	// if both are null: equal
			if (expected is null != actual is null) return false;	// if one is null and the other isn't: not equal

			// otherwise compare

			if (expected.Count != actual.Count) return false;

			for (int i = 0; i < actual.Count; i++)
			{
				List<string> actualLine = actual[i];
				List<string> expectedLine = expected[i];

				if (expectedLine.Count != actualLine.Count) return false;

				for (int j = 0; j < expectedLine.Count; j++) {
					if (expectedLine[j] != actualLine[j]) return false;
				}
			}

			return true;
		}
	}
}