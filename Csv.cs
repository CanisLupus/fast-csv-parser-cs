/*
zlib/libpng License (Zlib)

Copyright (c) 2021 Daniel Lobo

This software is provided 'as-is', without any express or implied warranty. In
no event will the authors be held liable for any damages arising from the use of
this software.

Permission is granted to anyone to use this software for any purpose, including
commercial applications, and to alter it and redistribute it freely, subject to
the following restrictions:

1. The origin of this software must not be misrepresented; you must not claim
that you wrote the original software. If you use this software in a product, an
acknowledgment in the product documentation would be appreciated but is not
required.

2. Altered source versions must be plainly marked as such, and must not be
misrepresented as being the original software.

3. This notice may not be removed or altered from any source distribution.
*/

using System.Collections.Generic;
using System.Text;

namespace DL.FastCsvParser
{
	/// <summary>
	/// Class representing a CSV file as a List of Lists of Strings.
	/// Use the static method Csv.Parse(string) to create the object from a CSV's contents.
	///
	/// The implementation conforms to RFC 4180 (https://tools.ietf.org/html/rfc4180)
	/// but it allows \n as line endings, not only \r\n.
	/// </summary>
	public class Csv : List<List<string>>
	{
		/// <summary>
		/// Create a Csv object from the given string contents.
		/// Returns null if the given string is an invalid CSV.
		/// </summary>
		public static Csv Parse(string csvContents)
		{
			return CsvParser.Parse(csvContents);
		}

		/// <summary>
		/// Converts the object to a valid CSV string, escaping all necessary fields.
		/// </summary>
		public override string ToString()
		{
			return CsvExporter.ConvertCsvToString(this);
		}

		// The actual CSV parser class.
		private static class CsvParser
		{
			enum ParsingState
			{
				BeforeField,		// before the first character of the following field (or at the end of the file)
				InField,			// reading the characters of a field
				InEscapedField,		// reading the characters of a field that is surrounded by quotes
				AfterEscapedField,	// right after the closing quote of an escaped field (what may follow: a separator, a newline, or the end of the file)
			}

			public static Csv Parse(string csvContents)
			{
				const char SEPARATOR = ',';
				const char NEWLINE = '\n';
				const char CARRIAGE_RETURN = '\r';
				const char QUOTE = '"';

				var csv = new Csv();
				var row = new List<string>();

				ParsingState state = ParsingState.BeforeField;
				int totalNumChars = csvContents.Length;
				int fieldStartIndex = -1;	// marks the start index for the current field being read (BeforeField state code updates this)
				bool escapedFieldHasQuotes = false;

				// for each char, act on it based on the current parsing state
				for (int i = 0; i < totalNumChars; i++)
				{
					char c = csvContents[i];

					// NOTE: if-statements were reordered to match the most common cases first

					if (state == ParsingState.InField)
					{
						if (c == SEPARATOR) {
							row.Add(csvContents.Substring(fieldStartIndex, i-fieldStartIndex));
							state = ParsingState.BeforeField;
						}
						else if (c == CARRIAGE_RETURN || c == NEWLINE) {
							row.Add(csvContents.Substring(fieldStartIndex, i-fieldStartIndex));
							csv.Add(row);

							row = new List<string>(row.Count);
							state = ParsingState.BeforeField;

							// if we found \r then it's a newline, so don't process the \n if it exists
							if (c == CARRIAGE_RETURN && i+1 < totalNumChars && csvContents[i+1] == NEWLINE) {
								i++;
							}
						}
						else if (c == QUOTE) {
							return null;	// can't have quotes in a non-escaped field
						}
					}
					else if (state == ParsingState.InEscapedField)
					{
						if (c == QUOTE) {
							// quotes can be escaped by repeating them, so check if next char is also a quote
							if (i+1 < totalNumChars && csvContents[i+1] == QUOTE) {
								i++;
								escapedFieldHasQuotes = true;
							}
							// if not an escaped quote, this is the closing quote
							else {
								string field = csvContents.Substring(fieldStartIndex, i-fieldStartIndex);
								if (escapedFieldHasQuotes) {
									field = field.Replace("\"\"", "\"");
									escapedFieldHasQuotes = false;
								}
								row.Add(field);

								state = ParsingState.AfterEscapedField;
							}
						}
					}
					else if (state == ParsingState.BeforeField)
					{
						if (c == SEPARATOR) {
							row.Add("");
							// state is still "BeforeField"
						}
						else if (c == CARRIAGE_RETURN || c == NEWLINE) {
							row.Add("");
							csv.Add(row);

							row = new List<string>(row.Count);
							// state is still "BeforeField"

							// if we found \r then it's a newline, so don't process the \n if it exists
							if (c == CARRIAGE_RETURN && i+1 < totalNumChars && csvContents[i+1] == NEWLINE) {
								i++;
							}
						}
						else if (c == QUOTE) {
							state = ParsingState.InEscapedField;
							fieldStartIndex = i+1;
						}
						else {
							state = ParsingState.InField;
							fieldStartIndex = i;
						}
					}
					else if (state == ParsingState.AfterEscapedField)
					{
						if (c == SEPARATOR) {
							state = ParsingState.BeforeField;
						}
						else if (c == CARRIAGE_RETURN || c == NEWLINE) {
							csv.Add(row);

							row = new List<string>(row.Count);
							state = ParsingState.BeforeField;

							// if we found \r then it's a newline, so don't process the \n if it exists
							if (c == CARRIAGE_RETURN && i+1 < totalNumChars && csvContents[i+1] == NEWLINE) {
								i++;
							}
						}
						else {
							return null;	// can't have extra characters after closing quotes
						}
					}
				}

				// when we reach this point, all characters have been processed but the last field/row might not have been completed yet

				if (state == ParsingState.InEscapedField) {
					return null;	// can't have no closing quote in an escaped field
				}

				// if the last field is an empty field, add it
				// (NOTE: an empty CSV file is also valid and generates a single empty field in a single row)
				if (totalNumChars == 0 || csvContents[totalNumChars-1] == SEPARATOR) {
					row.Add("");
				}
				// if we didn't finish the last field yet, add it
				else if (state == ParsingState.InField) {
					row.Add(csvContents.Substring(fieldStartIndex, totalNumChars-fieldStartIndex));
				}

				// likewise, if we didn't finish the last row yet, add it
				if (row.Count > 0) {
					csv.Add(row);
				}

				// confirm that all rows have the same number of fields
				int nFieldsInRow = csv[0].Count;

				for (int i = 1, nRows = csv.Count; i < nRows; i++)
				{
					if (csv[i].Count != nFieldsInRow) {
						return null;	// can't have a different number of fields in each row
					}
				}

				return csv;
			}
		}

		// The actual CSV exporter class.
		private static class CsvExporter
		{
			public static string ConvertCsvToString(Csv csv)
			{
				const char SEPARATOR = ',';
				const string NEWLINE = "\r\n";	// according to RFC 4180 it should be \r\n, not only \n

				int nRows = csv.Count;
				int nCols = csv[0].Count;

				var sb = new StringBuilder(nRows * nCols * 10);	// allocate 10 chars per field as the default

				for (int i = 0; i < nRows; i++)
				{
					List<string> line = csv[i];

					for (int j = 0; j < nCols; j++)
					{
						string field = line[j];
						bool needsEscape;

						// NOTE: In testing, IndexOf was faster than a for-cycle with a check for each char and also faster than Contains with StringComparison.Ordinal.

						if (field.IndexOf('"') > -1) {
							field = field.Replace("\"", "\"\"");	// escape quotes
							needsEscape = true;
						} else {
							needsEscape = field.IndexOf(',') > -1
									|| field.IndexOf('\n') > -1
									|| field.IndexOf('\r') > -1;
						}

						if (needsEscape) {
							field = "\"" + field + "\"";
						}

						sb.Append(field);
						if (j < nCols-1) {
							sb.Append(SEPARATOR);
						}
					}

					if (i < nRows-1) {
						sb.Append(NEWLINE);
					}
				}

				return sb.ToString();
			}
		}
	}
}
