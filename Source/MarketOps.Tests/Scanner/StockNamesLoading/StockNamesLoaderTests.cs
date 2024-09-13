using MarketOps.Scanner.StockNamesLoading;
using MarketOps.Tests.TestsShared;

namespace MarketOps.Tests.Scanner.StockNamesLoading;

[TestFixture]
internal class StockNamesLoaderTests
{
    private string _filePath;

    private StockNamesLoader _testObj;

    [SetUp]
    public void SetUp()
    {
        _testObj = new StockNamesLoader();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_filePath))
            Directory.Delete(Path.GetDirectoryName(_filePath)!, true);
    }

    [Test]
    public void GetStockNames_EmptyFile__ReturnsEmptyList()
    {
        _filePath = PrepareTestFile("emptyfile");

        var result = _testObj.GetStockNames(_filePath);

        result.ShouldBeEmpty();
    }

    [Test]
    public void GetStockNames_EachNameInNewLine__ReturnsAllNames()
    {
        string[] names = ["name1", "name2", "name3", "name4"];

        _filePath = PrepareTestFile("eachnameinnewline", names);

        var result = _testObj.GetStockNames(_filePath);

        result.ShouldBe(names);
    }

    [Test]
    public void GetStockNames_MultiNamesInLines__ReturnsAllNames()
    {
        string[] names = ["name1,name1.1", "name2", "name3;name3.1,name3.2", "name4"];
        string[] expected = ["name1", "name1.1", "name2", "name3", "name3.1", "name3.2", "name4"];

        _filePath = PrepareTestFile("multinamesinlines", names);

        var result = _testObj.GetStockNames(_filePath);

        result.ShouldBe(expected);
    }

    private string PrepareTestFile(string filename, params string[] lines)
    {
        var dir = Path.Combine(TestsConsts.ExecutingLocation, nameof(StockNamesLoaderTests));
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, filename);

        using var file = File.CreateText(path);
        foreach (var line in lines)
            file.WriteLine(line);

        return path;
    }
}
