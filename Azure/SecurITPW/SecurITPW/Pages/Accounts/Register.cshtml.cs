using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecurITPW.Pages.Accounts
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Surname { get; set; }

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
            // Perform registration logic here
            // You can access the values of Name, Surname, Email, and Password properties
            // and store them in your database or perform other actions

            // Redirect to a success page or return an appropriate response
            return RedirectToPage("/Home/Index");
        }
    }
}

