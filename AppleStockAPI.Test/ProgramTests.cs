using NUnit.Framework;
using AppleStockAPI.Models;

namespace AppleStockAPI.Test;

public class ProgramTests
{
    private DummyProgram _dummyProgram { get; set; } = null!;
    
    [SetUp]
    public void Setup()
    {

        _dummyProgram = new DummyProgram();

    }

    [TestCase(100)]
    [TestCase(95)]
    [TestCase(90)]
    public void GetGradesByPercentage_EqualTest(int percentage)
    {
        var grade = _dummyProgram.GetGradesByPercentage(percentage);
        Assert.That("5", Is.EqualTo(grade));
    }

    [TestCase(89)]
    [TestCase(71)]
    [TestCase(51)]
    public void GetGradesByPercentage_NotEqualTest(int percentage)
    {
        var grade = _dummyProgram.GetGradesByPercentage(percentage);
        Assert.That("5", Is.Not.EqualTo(grade));
    }

}