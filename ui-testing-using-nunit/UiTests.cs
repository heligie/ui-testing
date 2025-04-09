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
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
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

    [Test]
    public void AddFolderButton_TriggersModalWindow_InFileSection()
    {
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/files");

        var dropdownButton = _driver.FindElement(By.CssSelector("button[type='button']"));
        dropdownButton.Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ScrollContainer__inner']")));

        var addFolderButton = _driver.FindElement(By.CssSelector("[data-tid='CreateFolder']"));
        addFolderButton.Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='modal-content']")));

        var modalPageHeader = _driver.FindElement(By.CssSelector("[data-tid='ModalPageHeader']"));
        var modalInput = _driver.FindElement(By.CssSelector("[data-tid='ModalPageBody'] input[placeholder='Новая папка']"));

        Assert.Multiple(() => 
        {
            Assert.That(modalPageHeader.Text, Is.EqualTo("Создать"), "У модального окна нет заголовка 'Создать'");
            Assert.That(modalInput, Is.Not.Null, "Поле ввода для названия папки не найдено");
        });
    }

    [Test]
    public void ComboBox_ShouldDisplayRequestedItem() 
    {
        var query = "user1";

        var searchBar = _driver.FindElement(By.CssSelector("[data-tid='SearchBar']"));
        searchBar.Click();

        var searchInput = _driver.FindElement(By.CssSelector("input[placeholder='Поиск сотрудника, подразделения, сообщества, мероприятия']"));
        searchInput.SendKeys(query);
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ComboBoxMenu__item']")));

        var comboBoxItem = _driver.FindElement(By.CssSelector("[data-tid='ComboBoxMenu__item']"));

        Assert.That(comboBoxItem.Text, Does.Contain(query),
        "Комбобокс не отображает запрашиваемое значение");
    }

    [Test]
    public void TabUrl_ShouldPersistState_AfterPageRefresh() 
    {
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/documents");
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Tabs']")));

        var tasksTab = _driver.FindElement(By.CssSelector("[data-tid='Tabs'] a[href*='activeTab=Tasks']"));
        tasksTab.Click();
        _wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/documents?activeTab=Tasks"));

        _driver.Navigate().Refresh();

        Assert.That(_driver.Url, Is.EqualTo("https://staff-testing.testkontur.ru/documents?activeTab=Tasks"),
        "URL не соответствует ожидаемому для вкладки 'Задания'");
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
