Fast CSV Parser (C#)
=================

## Description

A single-file C# library to quickly parse/export a CSV file's contents. It conforms to [RFC 4180](https://tools.ietf.org/html/rfc4180) but also allows `\n` as line endings, not only `\r\n`.

## Usage

Simply copy the `Csv.cs` file to your project and use it. You need to import the `DL.FastCsvParser` namespace.

#### Reading a CSV file:

```C#
string csvContents = System.IO.File.ReadAllText("somefile.csv");
Csv csv = Csv.Parse(csvContents);
```

The `Csv` class is just a `List<List<string>>` with a custom `ToString` method for exporting/printing a valid CSV. You can easily create a `Csv` object as you would a list.

#### Writing a CSV file:

```C#
var csv = new Csv {
	new List<string> { "A", "B", "C" },
	new List<string> { "D", "E", "F" },
	new List<string> { "G", "H", "I" },
};

System.IO.File.WriteAllText("somefile.csv", csv.ToString());
```

## Credits / License / Notes

Fast CSV Parser (C#) was made by Daniel Lobo and is published under the zlib license.

Feel free to create an issue if you find a problem or bug.
