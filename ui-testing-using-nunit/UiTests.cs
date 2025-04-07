using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ui_testing_using_nunit;

public class Tests
{
    private IWebDriver _driver;

    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
