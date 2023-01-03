using Clean.Core.Domain.IdentityEntities;
using Clean.Core.DTO.IdentityDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Clean.UI.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AccountController(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        var registerDTO = new RegisterDTO();

        return View(registerDTO);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDTO model)
    {
        if (ModelState.IsValid == false)
            return View(model);

        var user = new ApplicationUser()
        {
            UserName = model.Name,
            Email = model.Email,
            PhoneNumber = model.Phone,
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded == false)
        {
            result.Errors.ToList().ForEach(x => ModelState.AddModelError("", x.Description));
        }

        //await signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction(nameof(PersonController.Index), "Person");
    }

    [HttpGet]
    public IActionResult Login()
    {
        var loginDTO = new LoginDTO();

        return View(loginDTO);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        if (ModelState.IsValid == false)
            return View(model);

        var user = await userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return NotFound();

        var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (result.Succeeded == false)
        {
            ModelState.AddModelError("", "Invalid Login Attempt");
            return View(model);
        }

        return RedirectToAction(nameof(PersonController.Index), "Person");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return RedirectToAction(nameof(PersonController.Index), "Person");
    }


    public async Task<IActionResult> IsEmailInUse(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Email {email} is already in use");
        }
    }
}