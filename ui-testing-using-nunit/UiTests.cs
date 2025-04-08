using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ui_testing_using_nunit;

public class Tests
{
    private IWebDriver _driver;
    private WebDriverWait _wait;

    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

        Login("", "");
    }

    private void Login(string username, string password) 
    {
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
        _driver.FindElement(By.Id("Username")).SendKeys(username);
        _driver.FindElement(By.Id("Password")).SendKeys(password);
        _driver.FindElement(By.Name("button")).Click();
        _wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
    }

    [Test]
    public void ProfileEdit_Accessible_ThroughUserToolbar()
    {
        var userToolbar = _driver.FindElement(By.CssSelector("[data-tid='Avatar']"));
        userToolbar.Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='PopupContent']")));

        var profileEditLink = _driver.FindElement(By.CssSelector("[data-tid='ProfileEdit']"));
        profileEditLink.Click();
        _wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/profile/settings/edit"));
    
        var profileEditPageTitle = _driver.FindElement(By.CssSelector("[data-tid='Title']"));

        Assert.That(profileEditPageTitle.Text, Is.EqualTo("Редактирование профиля"),
        "На странице нет заголовка 'Редактирование профиля'");
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
