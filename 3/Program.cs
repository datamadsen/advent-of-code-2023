
using System.Globalization;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var inputLines = File.ReadAllLines("input.txt");
var lines = new List<Line>();

for (var i = 0; i < inputLines.Length; i++)
{
    var line = inputLines[i];
    lines.Add(new Line
    {
        Index = i,
        NumberSpans = Regex.Matches(line, @"\d+").Select(x => new NumberSpan
        {
            Start = x.Index,
            End = x.Index + x.Length - 1,
            Number = Convert.ToInt32(line.Substring(x.Index, x.Length), CultureInfo.InvariantCulture)
        }),
        SymbolPositions = Regex.Matches(line, @"\W").Where(x => x.Value != ".").Select(x => x.Index),
        GearPositions = Regex.Matches(line, @"\*").Select(x => x.Index),
        LineString = line
    });
}

var lineManager = new LineManager
{
    Lines = lines
};

var partNumbers = lineManager.GetPartNumbers();
var gearRatios = lineManager.GetGearRatiosPerLine();

Console.WriteLine($"Part numbers sum: {partNumbers.Sum()}");
Console.WriteLine($"Gear reations sum: {gearRatios.Sum()}");

public class NumberSpan
{
    public Guid Id { get; init; }
    private static int _adjacencyBuffer = 1;

    private int _start;
    public int Start
    {
        get => _start - _adjacencyBuffer;
        init => _start = value;
    }

    private int _end;
    public int End
    {
        get => _end + _adjacencyBuffer;
        init => _end = value;
    }

    public int Number { get; init; }

    public NumberSpan()
    {
        Id = Guid.NewGuid();
    }

    // 465...114.
    // ....*.....
    // ....43....
    // ..592.....
    // ......755.
    // ...$.*....
    public bool OverlapsWithPosition(int position)
    {
        var result = Start <= position && position <= End;
        return result;
    }

    public bool OverlapsWithPositions(IEnumerable<int> positions)
    {
        positions = positions.ToList();
        return positions.Select(OverlapsWithPosition).Any(x => x == true);
    }
}

public class Line
{
    public int Index { get; set; }
    public IEnumerable<NumberSpan> NumberSpans { get; init; } = [];
    public IEnumerable<int> SymbolPositions { get; set; } = [];
    public IEnumerable<int> GearPositions { get; set; } = [];
    public string LineString { get; set; } = string.Empty;
}

public class LineManager
{
    public List<Line> Lines { get; set; } = [];

    public IEnumerable<int> GetPartNumbers()
    {
        return Lines.SelectMany(line =>
        {
            var surroundingLines = SurroundingLines(line).ToArray();
            return GetNumbersNextToASymbol(line, surroundingLines);
        });
    }

    public IEnumerable<int> GetGearRatiosPerLine()
    {
        return Lines.Select(line => {
            var surroundingLines = SurroundingLines(line).ToArray();
            var gearRatios = GetGearRatiosForLine(line, surroundingLines).ToList();

            return gearRatios.Sum();
        });
    }

    private static IEnumerable<int> GetNumbersNextToASymbol(Line line, params Line[] otherLines)
    {
        var symbolsPositions = line.SymbolPositions.Union(otherLines.SelectMany(x => x.SymbolPositions));
        var numberSpans = line.NumberSpans.Where(x => x.OverlapsWithPositions(symbolsPositions));
        return numberSpans.DistinctBy(x => x.Id).Select(x => x.Number);
    }

    private static IEnumerable<int> GetGearRatiosForLine(Line line, params Line[] otherLines)
    {
        var gearPositions = line.GearPositions;

        foreach (var position in gearPositions)
        {
            var numberSpans = line
                .NumberSpans.Union(otherLines.SelectMany(x => x.NumberSpans))
                .Where(x => x.OverlapsWithPosition(position));

            yield return numberSpans.Count() == 2
                ? numberSpans.Select(x => x.Number).Aggregate(1, (x, y) => x * y)
                : 0;
        }

    }

    /// <summary>
    /// Get this line and the surrounding lines (top and bottom).
    /// </summary>
    public IEnumerable<Line> SurroundingLines(Line line)
    {
        if (line.Index > 0)
            yield return Lines[line.Index - 1];

        if (line.Index < Lines.Count - 1)
            yield return Lines[line.Index + 1];
    }
}
