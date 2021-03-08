using IdentityServer.Models;
using IdentityServer.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        public AuthController(
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            IIdentityServerInteractionService identityServerInteractionService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _identityServerInteractionService = identityServerInteractionService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel {
                returnUrl = returnUrl
            });
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            var logoutRequest = await _identityServerInteractionService.GetLogoutContextAsync(logoutId);

            if(string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            UcabUserAuth userAuth = await BannerRestAPI.AuthenticateUser(loginViewModel.username, loginViewModel.password);
            if(userAuth == null)
            {
                return Redirect(loginViewModel.returnUrl);
            }
            if (userAuth.success)
            {
                var user = new IdentityUser(loginViewModel.username);

                var resultUser = await _userManager.CreateAsync(user, loginViewModel.password);

                if (resultUser.Succeeded)
                {
                    var resultClaim = await _userManager.AddClaimAsync(user, new Claim("uid",userAuth.uid));

                    if(resultClaim.Succeeded)
                    {
                        var resultSignIn = await _signInManager.PasswordSignInAsync(user, loginViewModel.password, false, false);

                        if(resultSignIn.Succeeded)
                        {
                            return Redirect(loginViewModel.returnUrl);
                        }
                    }
                }
                else
                {
                    var resultSignIn = await _signInManager.PasswordSignInAsync(user, loginViewModel.password, false, false);

                    if (resultSignIn.Succeeded)
                    {
                        return View(loginViewModel);
                    }
                }

            }
            else
            {
                return Redirect(loginViewModel.returnUrl);
            }

            return View(loginViewModel);
        }
    }
}
