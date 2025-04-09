using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ui_testing_using_nunit;

public class Tests
{
    const string BASE_URL = "https://staff-testing.testkontur.ru/";
    const string NEWS_URL = $"{BASE_URL}news";
    const string PROFILE_EDIT_URL = $"{BASE_URL}profile/settings/edit";
    const string COMMUNITIES_URL = $"{BASE_URL}communities";
    const string FILES_URL = $"{BASE_URL}files";
    const string DOCUMENTS_URL = $"{BASE_URL}documents";
    const string TASKS_TAB_URL = $"{DOCUMENTS_URL}?activeTab=Tasks";
    
    private IWebDriver _driver;
    private WebDriverWait _wait;

    private void Login(string username, string password) 
    {
        _driver.Navigate().GoToUrl(BASE_URL);
        _driver.FindElement(By.Id("Username")).SendKeys(username);
        _driver.FindElement(By.Id("Password")).SendKeys(password);
        _driver.FindElement(By.Name("button")).Click();
        _wait.Until(ExpectedConditions.UrlToBe(NEWS_URL));
    }

    private void MobileEmulation(int width, int height)
    {
        _driver.Manage().Window.Size = new System.Drawing.Size(width, height);
    }

    private void CreateCommunity(string name)
    {
        _driver.Navigate().GoToUrl(COMMUNITIES_URL);

        var createButton = _driver.FindElement(By.CssSelector("[data-tid='PageHeader']"))
                                    .FindElement(By.XPath("//button[contains(text(),'СОЗДАТЬ')]"));
        createButton.Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Name']")));

        var nameInput = _driver.FindElement(By.CssSelector("[data-tid='Name'] textarea[placeholder='Название сообщества']"));
        nameInput.SendKeys(name);

        var submitButton = _driver.FindElement(By.CssSelector("[data-tid='CreateButton'] button[type='button']"));
        submitButton.Click();
    }

    private void DeleteCommunity() 
    {
        var deleteButton = _driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        deleteButton.Click();

        var modalDeleteButton = _driver.FindElement(By.CssSelector("[data-tid='ModalPageFooter'] [data-tid='DeleteButton']"));
        modalDeleteButton.Click();
    }

    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

        Login("", "");
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }

    [Test]
    public void ProfileEdit_Accessible_ThroughUserToolbar()
    {
        var userToolbar = _driver.FindElement(By.CssSelector("[data-tid='Avatar']"));
        userToolbar.Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='PopupContent']")));

        var profileEditLink = _driver.FindElement(By.CssSelector("[data-tid='ProfileEdit']"));
        profileEditLink.Click();
        _wait.Until(ExpectedConditions.UrlToBe(PROFILE_EDIT_URL));
    
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
        _wait.Until(ExpectedConditions.UrlToBe(COMMUNITIES_URL));

        var communitiesPageTitle = _driver.FindElement(By.CssSelector("[data-tid='Title']"));

        Assert.That(communitiesPageTitle.Text, Is.EqualTo("Сообщества"),
        "На странице нет заголовка 'Сообщества'");
    }

    [Test]
    public void AddFolderButton_TriggersModalWindow_InFileSection()
    {
        _driver.Navigate().GoToUrl(FILES_URL);

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
        _driver.Navigate().GoToUrl(DOCUMENTS_URL);
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Tabs']")));

        var tasksTab = _driver.FindElement(By.CssSelector("[data-tid='Tabs'] a[href*='activeTab=Tasks']"));
        tasksTab.Click();
        _wait.Until(ExpectedConditions.UrlToBe(TASKS_TAB_URL));

        _driver.Navigate().Refresh();

        Assert.That(_driver.Url, Is.EqualTo(TASKS_TAB_URL),
        "URL не соответствует ожидаемому для вкладки 'Задания'");
    }

    [Test]
    public void CreateCommunity_ShouldSucceed_WhenDataIsValid() 
    {
        var expectedName = $"AT_Новое_Сообщество_{DateTime.Now:yyyyMMdd_HHmmss}";

        CreateCommunity(expectedName);

        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='PageHeader'] [data-tid='Title']")));

        var actualName = _driver.FindElement(By.CssSelector("[data-tid='Title'] a"));

        DeleteCommunity();

        Assert.That(actualName.Text, Is.EqualTo(expectedName), 
        "В заголовке 'Управление сообществом «_Name»' expectedName и actualName не совпадают");
    }   
}
