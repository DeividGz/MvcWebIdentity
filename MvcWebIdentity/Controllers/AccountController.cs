using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcWebIdentity.Models;

namespace MvcWebIdentity.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    //Método GET que apresenta o formulário de cadastro
    public IActionResult Register()
    {
        return View();
    }

    //Método POST onde a lógica do cadastro é definida
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            //Copia os dados do RegisterViewModel para o IdentityUser
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
            };

            //Armazena os dados do usuário na tabela AspNetUsers
            var result = await userManager.CreateAsync(user, model.Password);

            //Se o usuário foi criado com sucess, faz o login do usuário
            //usando serviço SignInManager e redireciona para o método Action Index
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "home");
            }

            //Se houver erros, então inclui no ModelState
            //que será exibido pela tag helper summary na validação
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    //Método GET que apresenta o formulário de login
    public IActionResult Login()
    {
        return View();
    }

    //Método POST onde a lógica do login é definida
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "home");
            }

            ModelState.AddModelError(string.Empty, "Login inválido.");
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "home");
    }

    [Route("/Account/AccessDenied")]
    public ActionResult AccessDenied()
    {
        return View();
    }
}
