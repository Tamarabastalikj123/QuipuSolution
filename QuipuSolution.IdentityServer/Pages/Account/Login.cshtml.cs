using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuipuSolution.IdentityServer.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly AuthServerSettings _settings;

        public LoginModel(TestUserStore users, IIdentityServerInteractionService interaction, AuthServerSettings settings)
        {
            _users = users;
            _interaction = interaction;
            _settings = settings;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; private set; }

        public bool ShowTestUsers => _settings.EnableTestUsers;

        public void OnGet(string? returnUrl)
        {
            Input.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (!_users.ValidateCredentials(Input.Username, Input.Password))
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            var user = _users.FindByUsername(Input.Username);
            await HttpContext.SignInAsync(new IdentityServerUser(user.SubjectId)
            {
                DisplayName = user.Username
            });

            // Only redirect back to a valid authorize request or a local page (prevents open redirects)
            if (_interaction.IsValidReturnUrl(Input.ReturnUrl) || Url.IsLocalUrl(Input.ReturnUrl))
                return Redirect(Input.ReturnUrl!);

            return Redirect("~/");
        }

        public class InputModel
        {
            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;

            public string? ReturnUrl { get; set; }
        }
    }
}
