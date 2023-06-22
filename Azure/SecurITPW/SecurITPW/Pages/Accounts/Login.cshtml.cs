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

            public void OnGet()
            {
                // Logic for handling GET request
            }

            public IActionResult OnPost()
            {
                if (Email == "alessandro.bonaldo@stud.itsaltoadriatico.it" && Password == "Poiuytre1!")
                {
                    // Successful login logic
                    return RedirectToPage("/Shared/SecurITPW");
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }
            }
        }
}



