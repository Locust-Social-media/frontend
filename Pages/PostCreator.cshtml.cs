using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Security.Claims;

namespace Locust.Pages
{
    public class PostCreatorModel : PageModel
    {
        private readonly IConfiguration _config;

        public PostCreatorModel(IConfiguration config)
        {
            _config = config;
        }


        public async Task<IActionResult> OnPostAsync(string title, string bodyText)
        {
            if (((title != "") || (bodyText != "")) && ((title != null) && (bodyText != null)))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cs = _config.GetConnectionString("MySqlConnection");
                await using var conn = new MySqlConnection(cs);
                await conn.OpenAsync();
                await using var cmd = new MySqlCommand(
                    @"INSERT INTO post (title, bodyText, likes, userID, comments)
                    VALUES (@title, @bodyText, 0, @userId, 0)",
                    conn
                );
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@bodyText", bodyText);
                cmd.Parameters.AddWithValue("@userId", userId);
                await cmd.ExecuteNonQueryAsync();
                return RedirectToPage("/index");
            }
            else
            {
                Console.WriteLine("haha loser");
                return Page();
            }
        }
    }
}
