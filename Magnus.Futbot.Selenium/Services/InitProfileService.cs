﻿using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using OpenQA.Selenium;

namespace Magnus.Futbot.Services
{
    public class InitProfileService : BaseService
    {
        public static async Task<ProfileDTO> InitProfile(AddProfileDTO profile)
        {
            var profileDTO = new ProfileDTO()
            {
                Email = profile.Email,
                UserId = profile.UserId,
                Status = ProfileStatusType.CaptchaNeeded
            };

            return await InitProfile(profileDTO);
        }

        public static async Task<ProfileDTO> InitProfile(ProfileDTO profileDTO)
        {
            var driverInstance = GetInstance(profileDTO.Email);
            var driver = driverInstance.Driver;

            if (!driver.Url.Contains("https://www.ea.com/fifa/ultimate-team/web-app/"))
                driver.Navigate().GoToUrl("https://www.ea.com/fifa/ultimate-team/web-app/");

            IWebElement? loginBtn = driver.FindElement(By.CssSelector("#Login > div > div > button.btn-standard.call-to-action"), 6000);
            loginBtn?.Click();
            await Task.Delay(500);

            var emailInput = driver.FindElement(By.CssSelector("#email"), 1000);
            if (emailInput is not null)
            {
                profileDTO.Status = await LoginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
            }

            var transferBtn = driver.FindElement(By.CssSelector("body > main > section > nav > button.ut-tab-bar-item.icon-transfer"), 10000);
            if (transferBtn is not null) profileDTO.Status = ProfileStatusType.Logged;

            if (profileDTO.Status == ProfileStatusType.Logged)
            {
                profileDTO = DataSeleniumService.GetBasicData(profileDTO);
            }
            driver.ExecuteScript("services.User.maxAllowedAuctions = 100");
            return profileDTO;
        }
    }
}
