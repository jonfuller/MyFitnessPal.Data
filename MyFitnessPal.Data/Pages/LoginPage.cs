using System;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using LanguageExt;
using Microsoft.Extensions.Logging;
using MyFitnessPal.Data.Utility;
using static LanguageExt.Prelude;

namespace MyFitnessPal.Data.Pages
{
    internal class LoginPage
    {
        private const string Url = "https://www.myfitnesspal.com/account/login";

        private readonly IBrowsingContext _context;
        private readonly ILogger<LoginPage> _logger;

        public LoginPage(IBrowsingContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<LoginPage>();
        }

        public Option<IDocument> Login(string username, string password)
        {
            _logger.LogDebug("opening login form");
            _logger.LogDebug($"username: {username}");
            _logger.LogDebug($"password length: {password.Length}");

            var loginPageTask = _context.OpenAsync(Url);
            loginPageTask.Wait(TimeSpan.FromSeconds(30));

            var form = loginPageTask.Result.QuerySelector<IHtmlFormElement>(Selectors.LoginForm);

            form.QuerySelector<IHtmlInputElement>(Selectors.Username).Value = username;
            form.QuerySelector<IHtmlInputElement>(Selectors.Password).Value = password;

            _logger.LogDebug("submitting login form");
            var submission = form.SubmitAsync();
            submission.Wait(TimeSpan.FromSeconds(30));

            var result = submission.Result;
            result.WaitForReadyAsync().Wait(TimeSpan.FromSeconds(30));

            _logger.LogTrace($"result of login page {result.ToHtml()}");

            return LoginPageParser.IsLoggedIn(result)
                ? Some(result)
                : None;
        }

        private static class Selectors
        {
            public static string LoginForm = ".LoginForm";
            public static string Username = "#username";
            public static string Password = "#password";
        }
    }
}