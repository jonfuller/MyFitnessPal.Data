using System;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace MyFitnessPal.Data.Pages
{
    internal class LoginPage
    {
        private const string Url = "https://www.myfitnesspal.com/account/login";

        private readonly IBrowsingContext _context;

        public LoginPage(IBrowsingContext context)
        {
            _context = context;
        }

        public IDocument Login(string username, string password)
        {
            var loginPageTask = _context.OpenAsync(Url);
            loginPageTask.Wait(TimeSpan.FromSeconds(30));

            var form = loginPageTask.Result.QuerySelector<IHtmlFormElement>(Selectors.LoginForm);

            form.QuerySelector<IHtmlInputElement>(Selectors.Username).Value = username;
            form.QuerySelector<IHtmlInputElement>(Selectors.Password).Value = password;

            var submission = form.SubmitAsync();
            submission.Wait(TimeSpan.FromSeconds(30));

            var result = submission.Result;
            result.WaitForReadyAsync().Wait(TimeSpan.FromSeconds(30));

            return result;
        }

        private static class Selectors
        {
            public static string LoginForm = ".LoginForm";
            public static string Username = "#username";
            public static string Password = "#password";
        }
    }
}