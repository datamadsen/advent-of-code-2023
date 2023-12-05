// See https://aka.ms/new-console-template for more information
using System.Globalization;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines("./input.txt");

var colorMax = new Dictionary<string, int>
{
    ["blue"] = 14,
    ["green"] = 13,
    ["red"] = 12
};

// A line looks like this:
// Game 1: 1 green, 2 blue; 13 red, 2 blue, 3 green; 4 green, 14 red
var validGamesSum = 0;
var powerSum = 0;
foreach (var line in lines)
{
    var gameNumber = line
        .Split(':')[0]
        .Replace("Game ", string.Empty);

    Console.WriteLine($"Game: {gameNumber}");

    var gameSets = line
        .Split(':')
        .Last()
        .Split(';');

    Console.WriteLine($"Game sets: {string.Join(',', gameSets)}");

    var setCubes = gameSets
        .SelectMany(set => set.Split(", "))
        .Select(cubes => cubes.Trim())
        .Select(x => x.Split(' '))
        .Select(x => new KeyValuePair<string, int>(x[1], Convert.ToInt32(x[0], CultureInfo.InvariantCulture)));

    Console.WriteLine($"Set cubes : {string.Join(',', setCubes)}");

    var allSetsAreValid = true;

    foreach (var (color, number) in setCubes)
    {
        allSetsAreValid = allSetsAreValid && (colorMax[color] >= number);

        if (!allSetsAreValid)
        {
            break;
        }
    }

    if (allSetsAreValid)
        validGamesSum += Convert.ToInt32(gameNumber, CultureInfo.InvariantCulture);

    // Find the minimum number of cubes that could have been used
    var minimumRequiredCubes = setCubes
        .GroupBy(x => x.Key)
        .Select(group => new KeyValuePair<string, int>(group.Key, group.Max(x => x.Value)));

    Console.WriteLine($"Minimum required cubes : {string.Join(',', minimumRequiredCubes)}");

    powerSum += minimumRequiredCubes.Select(x => x.Value).Aggregate(1, (x, y) => x * y);
}

Console.WriteLine($"Valid games sum: {validGamesSum}");
Console.WriteLine($"Power sum: {powerSum}");
