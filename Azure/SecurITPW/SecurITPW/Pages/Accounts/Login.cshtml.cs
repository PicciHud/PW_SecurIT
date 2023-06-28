using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecurITPW.Pages.Users
{
       public class LoginModel : PageModel
       {
            [BindProperty]
            public string Email { get; set; }

            [BindProperty]
            public string Password { get; set; }

            public string ErrorMessage { get; private set; }

        }
}



