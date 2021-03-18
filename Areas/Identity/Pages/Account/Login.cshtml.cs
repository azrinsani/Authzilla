using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Authzilla;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using AzUtil.Core;

namespace Authzilla.Areas.Identity.Pages.Account
{
    //public class UsernameRequiredAttribute : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        var model = (LoginModel)validationContext.ObjectInstance;
    //        if (model.LoginSettings.MustLoginWithEmail) return ValidationResult.Success;
    //        if (model.Input.Username.IsNullOrEmpty()) return new ValidationResult(model.LoginSettings.UsernameEmpty);
    //        return ValidationResult.Success;
    //    }
    //}

    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        public Settings Settings { get; set; }
        
        public LoginModel(SignInManager<AppUser> signInManager, ILogger<LoginModel> logger, UserManager<AppUser> userManager, IOptions<Settings> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            this.Settings = settings.Value;
            this.LoginSettings = this.Settings.Login;            
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public LoginSettings LoginSettings { get; set; }
        public string ReturnUrl { get; set; }


        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            public string Username { get; set; }

            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            //if (ModelState.IsValid)
            bool emailNotEmpty = !this.LoginSettings.MustLoginWithEmail || !Input.Email.IsEmptyOrWhiteSpace();
            if (!emailNotEmpty) ModelState.AddModelError(string.Empty, this.LoginSettings.EmailEmpty);

            bool emailValid = !this.LoginSettings.MustLoginWithEmail || Input.Email.IsValidEmail();
            if (!emailValid) ModelState.AddModelError(string.Empty, this.LoginSettings.EmailInvalid);

            bool usernameValid = this.LoginSettings.MustLoginWithEmail || !Input.Username.IsNullOrEmpty();
            if (!usernameValid) ModelState.AddModelError(string.Empty, this.LoginSettings.UsernameEmpty);


            //if ((emailValid || (!this.LoginSettings.MustLoginWithEmail && !Input.Username.IsEmptyOrWhiteSpace())) && !Input.Password.IsNullOrEmpty())
            if (ModelState.IsValid)
            {                
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                Microsoft.AspNetCore.Identity.SignInResult result;
                if (this.LoginSettings.MustLoginWithEmail) result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                else result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, this.LoginSettings.LoginFailed);
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
