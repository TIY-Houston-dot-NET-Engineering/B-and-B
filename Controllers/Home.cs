using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

[Route("/")]
[Authorize]
public class HomeController : Controller
{
    private DB db;
    private IRepository<BnB> bnb;
    private IAuthService auth;
    public HomeController(DB db, IRepository<BnB> bnb, IAuthService auth){
        this.db = db;
        this.bnb = bnb;
        this.auth = auth;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Root(){
        // ViewData["User"] = await auth.GetUser(HttpContext);
        // (await auth.GetUser(HttpContext)).Log();
        return View("Index", db.BnBs.ToList());
    }

    [HttpGet("bnb/new")]
    public IActionResult CreateBnB() => View("CreateBnB");

    [HttpPost("bnb/new")]
    [ValidateAntiForgeryToken]
    public IActionResult CreateBnB([FromForm] BnB bnb)
    {
        if(!ModelState.IsValid)
            return View("CreateBnB", bnb);

        db.BnBs.Add(bnb);
        db.SaveChanges();
        return Redirect("/");
    }

    [HttpGet("bnb/{id}")]
    public async Task<IActionResult> BnB(int id)
    {
        BnB item = bnb.Read(id);
        if(item == null) return NotFound();
        return View("BnB", item);
    }

    [HttpPost("bnb/{id}/messages")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMessage([FromForm] Message m, int id){
        m.BnB = null;
        string name = (await auth.GetUser(HttpContext))?.Email ?? "NOT PROVIDED";
        m.Visitor = new Visitor { Name = name };
        
        TryValidateModel(m);
        
        if(ModelState.IsValid){
            db.Messages.Add(m);
            db.SaveChanges();
        }

        return Redirect($"/bnb/{id}");
    }

    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login() => View("Login");

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromForm] LoginVM user){
        if(await auth.Login(user.Email, user.Password)){
            // HttpContext.User
            // HttpContext.Session["SessionID"]
            return Redirect("/");
        } else {
            ModelState.AddModelError("", "That email/password combo does not exist");
            return View("Login", user);
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromForm] LoginVM user){
        if(await auth.Register(user.Email, user.Password)){
            return Redirect("/");
        } else {
            ModelState.AddModelError("", "That email already exists in the database");
            return View("Login", user);
        }
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(){
        await auth.Logout();
        return Redirect("/");
    }
    
}

public class LoginVM {
    [Required]
    [EmailAddress]
    public string Email {get;set;}
    [Required]
    [DataType(DataType.Password)]
    public string Password {get;set;}
}