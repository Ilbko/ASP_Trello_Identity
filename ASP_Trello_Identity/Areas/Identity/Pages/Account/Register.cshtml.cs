using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ASP_Trello_Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace ASP_Trello_Identity.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string FullName { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                Random r = new Random();
                if (await _userManager.FindByEmailAsync(Input.Email) != null)
                    return NotFound("Аккаунт с этой почтой уже был авторизирован.");

                var user = new ApplicationUser { UserName = "user" + r.Next(10000000, 99999999), Email = Input.Email, FullName = Input.FullName };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Подтверждение почты",
                    //    $"Подтвердите свою почту для аккаунта Drello по ссылке: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}</a>" +
                    //    $"<svg id=\"eR8aXFi07Kc1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink \" viewBox=\"0 0 32 32\" shape-rendering=\"geometricPrecision\" text-rendering=\"geometricPrecision\"><defs><linearGradient id=\"eR8aXFi07Kc2-fill\" x1=\"0.5\" y1=\"0\" x2=\"0.5\" y2=\"1\" spreadMethod=\"pad\" gradientUnits=\"objectBoundingBox\" gradientTransform=\"matrix(1 0 0 1 0 0)\"><stop id=\"eR8aXFi07Kc2-fill-0\" offset=\"0%\" stop-color=\"rgb(173,255,47)\"/><stop id=\"eR8aXFi07Kc2-fill-1\" offset=\"100%\" stop-color=\"rgb(47,255,255)\"/></linearGradient></defs><rect id=\"eR8aXFi07Kc2\" width=\"32\" height=\"32\" rx=\"3\" ry=\"3\" fill=\"url(#eR8aXFi07Kc2-fill)\" stroke=\"none\" stroke-width=\"0\"/><rect id=\"eR8aXFi07Kc3\" width=\"10.998895\" height=\"26.042545\" rx=\"1\" ry=\"1\" transform=\"matrix(1 0 0 1 2.982107 3.049951)\" fill=\"rgb(255,255,255)\" stroke=\"none\" stroke-width=\"0\"/><rect id=\"eR8aXFi07Kc4\" width=\"11.56658\" height=\"17.030546\" rx=\"1\" ry=\"1\" transform=\"matrix(0.92638 0 0 1 17.883837 3.049951)\" fill=\"rgb(255,255,255)\" stroke=\"none\" stroke-width=\"0\"/></svg>"
                    //    );
                    SendEmail(Input.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Redirect("~/Work/Workspace");
                }
            }

            return Page();
        }

        private void SendEmail(string to, string callbackUrl)
        {
            MailAddress from = new MailAddress(EmailCredentials.Address);

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(from.Address, EmailCredentials.Password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage()
            {
                Subject = "Подтверждение почты",
                From = from,
                Body = $"Подтвердите свою почту для аккаунта Drello по ссылке: {HtmlEncoder.Default.Encode(callbackUrl)}",
               
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception e)
            {

            }
        }
    }
}
