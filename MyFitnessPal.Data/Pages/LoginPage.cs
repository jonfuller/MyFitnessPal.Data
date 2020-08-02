using System.Threading.Tasks;
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

        public async Task<IDocument> Login(string username, string password)
        {
            var loginPage = await _context.OpenAsync(Url);

            var form = loginPage.QuerySelector<IHtmlFormElement>(Selectors.LoginForm);

            form.QuerySelector<IHtmlInputElement>(Selectors.Username).Value = username;
            form.QuerySelector<IHtmlInputElement>(Selectors.Password).Value = password;

            var result = await form.SubmitAsync();

            await result.WaitForReadyAsync();

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