﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobOffer.Models;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace JobOffer.Controllers
{
    public class AuthController : Controller
    {
        #region Objects
        private readonly ModelContext _context;
        #endregion

        #region Constructors
        public AuthController(ModelContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods

        #region Login

        #region Get
        public IActionResult Login()
        {
            return View();
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Username, Password")] Useraccounth user)
        {
            #region query To Authnticate The User
            var auth = _context.Useraccounths.Where(x => x.Username == user.Username && x.Password == user.Password).FirstOrDefault();
            #endregion

            if (auth != null)
            {
                switch (auth.Roleid)
                {
                    case 1:
                        #region Session For Admin's Username and AdminId
                        HttpContext.Session.SetString("AdminUser", auth.Username);
                        HttpContext.Session.SetInt32("AdminId", (int)auth.Userid);
                        #endregion

                        return RedirectToAction("Dashboard", "Admin");
                    case 2:
                        #region Session For User's Username and UserId
                        HttpContext.Session.SetString("ActualUser", user.Username);
                        HttpContext.Session.SetInt32("UserId", Convert.ToInt32(auth.Userid));
                        #endregion

                        return RedirectToAction("Home", "ActualUser");
                }
            }
            else
            {
                Response.WriteAsync("<script>alert('Try Again Username or Password incorect')</script>");
            }
            return View();
        }
        #endregion

        #endregion

        #region Sign Up

        #region Get
        public IActionResult signUp()
        {
            return View();
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult signUp([Bind("Fullname, Username, Email, Phonenumber, Industialname, Password")] Useraccounth user)
        {
            if (user != null)
            {
                #region Any Register User Will Have The Role id >> 2 Which is User 
                user.Roleid = 2;
                #endregion
                if (user.Password.Length < 6)
                {
                    Response.WriteAsync("<script>alert('Password Must Be Greater than 6 Character')</script>");
                }
                else
                {
                    #region Add And Save
                    _context.Add(user);
                    _context.SaveChanges();
                    #endregion
                    return RedirectToAction("Login", "Auth");
                }


            }
            return View();
        }
        #endregion

        #endregion

        #region LogOut

        public IActionResult logOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        #endregion

        #region ForgotPass

        #region Get
        public IActionResult ForgotPass()
        {
            return View();
        }
        #endregion

        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPass(string ConfrirmPass, [Bind("Username, Password")] Useraccounth useraccount)
        {
            var UserInfo = _context.Useraccounths.Where(x => x.Username == useraccount.Username).FirstOrDefault();
            if (UserInfo != null)
                if (!string.IsNullOrEmpty(ConfrirmPass) && !string.IsNullOrEmpty(useraccount.Password))
                    if (useraccount.Password.Equals(ConfrirmPass))
                    {
                        UserInfo.Password = ConfrirmPass;
                        _context.SaveChanges();
                    }
                    else
                    {
                        Response.WriteAsync("<script>alert('The Password Not Equal Confirm Password')</script>");
                        return View();
                    }

                else
                {
                    Response.WriteAsync("<script>alert('Sohuld Enter Not Null Value')</script>");
                    return View();
                }
            else
                return View();

            return RedirectToAction("Login", "Auth");


        } 
        #endregion
        #endregion

        #endregion
    }
}
