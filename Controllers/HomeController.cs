﻿using FaturaYönetimSistemleri.Models.DB;
using FaturaYönetimSistemleri.Models.Entities;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;

namespace FaturaYönetimSistemleri.Controllers
{
    public class HomeController : Controller
    {
        Context c = new Context();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult UserLogin(User user)
        {
            var login = c.Users.FirstOrDefault(x => x.Email == user.Email && x.Password == user.Password);
            if (login != null)
            {
                FormsAuthentication.SetAuthCookie(login.Email, false);

                Session["UserEmail"] = login.Email.ToString();

                return RedirectToAction("Index", "UserPanel");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult AdminLogin(Admin admin)
        {
            var login = c.Admins.FirstOrDefault(x => x.UserName == admin.UserName && x.Password == admin.Password);

            if (login != null)
            {
                FormsAuthentication.SetAuthCookie(login.UserName, false);

                Session["UserName"] = login.UserName.ToString();

                return RedirectToAction("Index", "istatistik");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult LogOut() 
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult PasswordForgot()
        {
            return View();
        }


        [HttpPost]
        public ActionResult PasswordForgot(string Email) // Parametreden Emali alıyoruz.
        {
            var user = c.Users.Where(x => x.Email == Email).SingleOrDefault(); // Kullanıcıyı aldık

            if (user != null) // Veri tabanında email varsa if calısacak
            {
                Guid randomkey = Guid.NewGuid(); // 32 karakterli kodu ürettik

                user.Password = randomkey.ToString().Substring(0, 5); /// Keyi ekleyip veritabanına ekledik

                c.SaveChanges(); // Veritabanına kaydettik


                MailAddress from = new MailAddress("SMTcoder@gmail.com"); // Maili atacak mail adresi
                MailAddress to = new MailAddress(user.Email); // Kullanıcının mailini yazdık
                MailMessage msg = new MailMessage(from, to); 
                msg.IsBodyHtml = true;  
                msg.Subject = "Şifre Sıfırlama";
                msg.Body += "<h2>  Merhaba " + user.Email
                    + " Şifre Degiştirme İsteğiniz Alınmıştır.  Kodunuz : "
                    + randomkey.ToString().Substring(0, 5)  // Substring ile random keyimizi 5 karatere düşdük
                    + "Sitemize girerek profilden şifrenizi güncelleyiniz ! </h2>  </br>  ";


                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("SMTcoder@gmail.com", "MailSifreniz");
                client.Send(msg); // Maili gönderdik.

                return RedirectToAction("Index", "Home"); // Index sayfasına atacak
            }
            else // Email kayıtlı değilse else calısacak 
            {
                ViewBag.EmailNotFind = "Email kayıtlı değil ! ";
                return RedirectToAction("PasswordForgot", "Home"); //  PasswordForgot sayfasına atacak
            }
        }


    }

}