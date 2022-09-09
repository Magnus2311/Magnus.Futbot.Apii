﻿using Magnus.Futbot.Common.Models.Selenium.Trading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Magnus.Futbot.Services
{
    public static class SeleniumExtensions
    {
        public static IWebElement? FindElement(this IWebDriver driver, By by, int milliseconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(milliseconds));
            try
            {
                var isElementActive = wait.Until(drv =>
                {
                    try
                    {
                        var element = drv.FindElement(by);
                        return element.Displayed && element.Enabled;
                    }
                    catch
                    {
                        return false;
                    }
                });
                return isElementActive ? driver.FindElement(by) : null;
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<IWebElement> FindElements(this IWebDriver driver, By by, int milliseconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(milliseconds));
            var isElementActive = wait.Until(drv =>
            {
                try
                {
                    var elements = drv.FindElements(by);
                    return elements.All(e => e.Displayed) && elements.All(e => e.Enabled);
                }
                catch
                {
                    return false;
                }
            });
            return driver.FindElements(by);
        }

        public static void OpenTransfer(this IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("body > main > section > nav > button.ut-tab-bar-item.icon-transfer")).Click();
            Thread.Sleep(500);
        }

        public static void OpenUnassignedItems(this IWebDriver driver)
        {
            driver.OpenHomePage();
            driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.ut-unassigned-tile-view.tile.col-1-1")).Click();
            Thread.Sleep(500);
        }

        public static void OpenTransferTargets(this IWebDriver driver)
        {
            driver.OpenTransfer();
            driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-targets")).Click();
            Thread.Sleep(500);
        }

        public static void OpenTransferList(this IWebDriver driver)
        {
            driver.OpenTransfer();
            driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-list")).Click();
            Thread.Sleep(500);
        }

        public static void OpenHomePage(this IWebDriver driver)
        {
            var homeBtn = driver.FindElement(By.CssSelector("body > main > section > nav > button.ut-tab-bar-item.icon-home"), 10000);
            homeBtn?.Click();
            Thread.Sleep(500);
        }

        public static void OpenSearchTransfer(this IWebDriver driver)
        {
            driver.OpenTransfer();
            var searchDiv = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-1.ut-tile-transfer-market"), 2000);
            if (searchDiv is not null) searchDiv.Click();
            Thread.Sleep(1000);
        }

        public static PlayerCard ConvertPlayerElementToPlayerCard(this IWebElement player)
            => new()
            {
                Name = player.FindElement(By.ClassName("name")).Text,
                Rating = int.Parse(player.FindElement(By.ClassName("rating")).Text)
            };

        public static int GetCoins(this IWebDriver driver)
        {
            var coinsDiv = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-bar-view.navbar-style-landscape.currency-purchase > div.view-navbar-currency > div.view-navbar-currency-coins"), 1000);
            if (coinsDiv is not null && int.TryParse(coinsDiv.Text.Replace(",", ""), out var coins)) return coins;

            return 0;
        }
    }
}