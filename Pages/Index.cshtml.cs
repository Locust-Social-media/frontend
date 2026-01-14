using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize]
public class IndexModel : PageModel
{
    public string Email { get; private set; } = "";
    public string RoleLabel { get; private set; } = "";

    public void OnGet()
    {
        Email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.Identity?.Name
            ?? "onbekend";

        // Role uit AUTH (claims), niet uit DB
        RoleLabel = User.IsInRole("beheerder") ? "beheerder" : "gebruiker";
    }
}
