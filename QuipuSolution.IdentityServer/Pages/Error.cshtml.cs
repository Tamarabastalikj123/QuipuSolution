using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuipuSolution.IdentityServer.Pages
{
    public class ErrorModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;

        public ErrorModel(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        public string? Error { get; private set; }

        public string? Description { get; private set; }

        public async Task OnGetAsync(string? errorId)
        {
            var message = await _interaction.GetErrorContextAsync(errorId);
            Error = message?.Error;
            Description = message?.ErrorDescription;
        }
    }
}
