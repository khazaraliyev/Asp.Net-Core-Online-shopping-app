using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.WebUI.EmailService;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.ViewModels;

namespace ShopApp.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly ICardService cardService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ICardService cardService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.cardService = cardService;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl="")
        {
            return View(new LoginViewModel()
            {
                ReturnUrl=returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Username doesn't exist");
                return View(model);
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Confirm email! Confirmation link has been sent to your email");
                return View(model);
            }
            var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {   
                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl ?? "~/");
                }

                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Invalid attempt");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var token =await userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail","Account",new { 
                userid=user.Id,
                confirmationToken=token
                });
                await emailSender.SendEmailAsync(model.Email,"Confirm account",$"<h1>Go to link to confirm email</h1> <a href='https://localhost:44327{url}'>{url}</a>");
                return RedirectToAction("Login", "Account");
            }
            ModelState.AddModelError("", "Error occured");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("~/");
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId,string confirmationToken)
        {
            if (userId==null || confirmationToken == null)
            {
                TempData["message"] = "Invalid token";
                return View();
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user!=null)
            {
                var result = await userManager.ConfirmEmailAsync(user, confirmationToken);
                if (result.Succeeded)
                {
                    cardService.InitializeCard(user.Id);
                    TempData["message"] = "Email successfully confirmed";
                    return RedirectToAction("Index", "Home");
                }
               
            }
            TempData["message"] = "Email confirmation failed";
            return View();
            
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View();
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user==null)
            {
                return View();
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword", "Account", new
            {
                userid = user.Id,
                confirmationToken = token
            });
            await emailSender.SendEmailAsync(email, "Reset password link", $"<h1>Go to link to reset password</h1> <a href='https://localhost:44327{url}'>{url}</a>");

            return View();
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string confirmationToken)
        {
            if (userId==null || confirmationToken==null)
            {
                return RedirectToAction("Index","Home");
            }
            var model = new ResetPasswordViewModel { Token = confirmationToken };
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                return RedirectToAction("Index", "Home");
            }
            var result = await userManager.ResetPasswordAsync(user, model.Token,model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
