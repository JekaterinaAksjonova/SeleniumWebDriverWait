using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumWaits
{
    public class SeleniumWaitsTests
    {
        WebDriver driver;
        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.selenium.dev/selenium/web/dynamic.html");
        }
        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Test, Order(1)]
        public void AddBoxWithoutWaitsFails()
        {
            driver.FindElement(By.Id("adder")).Click();

            try
            {
                var boxElemenent = driver.FindElement(By.Id("box0"));
                Assert.Fail("Expected NoSuchElementException was not thrown");
            }
            catch (NoSuchElementException)
            {
                Assert.Pass();
            }
            
        }

        [Test, Order(2)]
        public void RevealInputWithoutWaitsFail()
        {
            driver.FindElement(By.Id("reveal")).Click();

            try
            {
                var inputField = driver.FindElement(By.Id("revealed"));
                inputField.SendKeys("Displayed");

                Assert.Fail("Expected ElementNotInteractableException was not thrown");
            }
            catch (ElementNotInteractableException) 
            {
                Assert.Pass();
            }
            
        }

        [Test, Order(3)]
        public void AddBoxWithThreadSleep()
        {
            driver.FindElement(By.Id("adder")).Click();
            Thread.Sleep(3000);
            var boxElemenent = driver.FindElement(By.Id("box0"));

            Assert.IsTrue(boxElemenent.Displayed);
        }

        [Test, Order(4)]
        public void AddBoxWithImplicitWait()
        {
            driver.FindElement(By.Id("adder")).Click();
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var boxElemenent = driver.FindElement(By.Id("box0"));

            Assert.IsTrue(boxElemenent.Displayed);
        }

        [Test, Order(5)]
        public void RevealInputWithImplicitWaits()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.FindElement(By.Id("reveal")).Click();

            var inputField = driver.FindElement(By.Id("revealed"));

            inputField.SendKeys("Displayed");

            Assert.AreEqual(inputField.GetAttribute("value"), "Displayed");
        }

        [Test, Order(6)]
        public void RevealInputWithExplicitWaits()
        {
            driver.FindElement(By.Id("reveal")).Click();

            var inputField = driver.FindElement(By.Id("revealed"));

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
            wait.Until(d => inputField.Displayed);

            inputField.SendKeys("Displayed");

            Assert.AreEqual(inputField.GetAttribute("value"), "Displayed");
        }

        [Test, Order(7)]
        public void AddBoxWithFluentWaitExpectedConditionsAndIgnoredExceptions()
        {
            driver.FindElement(By.Id("adder")).Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.PollingInterval = TimeSpan.FromMilliseconds(500);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

            IWebElement newBox = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("box0")));

            Assert.IsTrue(newBox.Displayed);


        }

        [Test, Order(8)]
        public void RevealInputWithCustomFluentWait()
        {
            driver.FindElement(By.Id("reveal")).Click();
            var inputElement = driver.FindElement(By.Id("revealed"));

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10))
            {
                PollingInterval = TimeSpan.FromMilliseconds(500),

            };
            wait.IgnoreExceptionTypes(typeof(ElementNotInteractableException));

            wait.Until(d =>
            {
                inputElement.SendKeys("Displayed");
                return true;
            });

            Assert.AreEqual(inputElement.TagName, "input");
            Assert.AreEqual(inputElement.GetAttribute("value"), "Displayed");

        }
    }
}