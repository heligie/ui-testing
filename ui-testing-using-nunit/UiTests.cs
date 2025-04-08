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

    private void MobileEmulation(int width, int height)
    {
        _driver.Manage().Window.Size = new System.Drawing.Size(width, height);
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

    [Test]
    public void Communities_Accessible_ThroughBurgerMenu()
    {
        MobileEmulation(375, 812);

        var burgerMenu = _driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        burgerMenu.Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SidePage__root']")));

        var communitiesLink = _driver.FindElement(By.CssSelector("[data-tid='SidePageBody'] [data-tid='Community']"));
        communitiesLink.Click();
        _wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/communities"));

        var communitiesPageTitle = _driver.FindElement(By.CssSelector("[data-tid='Title']"));

        Assert.That(communitiesPageTitle.Text, Is.EqualTo("Сообщества"),
        "На странице нет заголовка 'Сообщества'");
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
