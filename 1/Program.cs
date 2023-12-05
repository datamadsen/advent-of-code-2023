using System.Globalization;
using System.Text.RegularExpressions;

Dictionary<string, string> _translations = new Dictionary<string, string>
{
    ["one"] = "1",
    ["two"] = "2",
    ["three"] = "3",
    ["four"] = "4",
    ["five"] = "5",
    ["six"] = "6",
    ["seven"] = "7",
    ["eight"] = "8",
    ["nine"] = "9",
};

var input = File.ReadAllLines("./input.txt");

long result = 0;
foreach (var line in input)
{
    var regex = new Regex("(?=(\\d|one|two|three|four|five|six|seven|eight|nine))");

    var matches = regex.Matches(line);

    var firstMatch = matches.First().Groups[1].Value;
    var lastMatch = matches.Last().Groups[1].Value;

    var first = GetNumberString(firstMatch);
    var last = GetNumberString(lastMatch);

    var number = Convert.ToInt64($"{first}{last}", CultureInfo.InvariantCulture);
    result += number;
}

Console.WriteLine($"RESULT: {result}");

string GetNumberString(string value)
{
    if (_translations.TryGetValue(value, out string? translation))
        return translation;

    return value;
}
