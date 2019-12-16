using Laborator4.Models;
using Laborator7.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Laborator7.Controllers
{
    public class ProposalController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Index(string searchName)
        {
            var news = db.Proposal.Include("Category").Include("User");

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            if (!String.IsNullOrEmpty(searchName))
            {
                var news2 = news.Where(n => n.Title.Contains(searchName) || n.Content.Contains(searchName)).ToList();

                if (news2 != null)
                    ViewBag.News = news2;
                else ViewBag.News = news;

            }
            else
            {
                ViewBag.News = news;
            }
            return View();
        }

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Show(int id)
        {
            Proposal proposal = db.Proposal.Find(id);

            ViewBag.afisareButoane = true;
            //if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
            //{
            //    ViewBag.afisareButoane = true;
            //}

            ViewBag.esteAdmin = User.IsInRole("Administrator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();


            return View(proposal);

        }

        [Authorize(Roles = "User")]
        public ActionResult New()
        {
            Proposal proposal = new Proposal(); 

            // preluam lista de categorii din metoda GetAllCategories()
            proposal.Categories = GetAllCategories();

            // Preluam ID-ul utilizatorului curent
            proposal.UserId = User.Identity.GetUserId();


            return View(proposal);

        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult New(Proposal proposal)
        {
            proposal.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Proposal.Add(proposal);
                    db.SaveChanges();
                    TempData["message"] = "Articolul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(proposal);
                }
            }
            catch (Exception e)
            {
                return View(proposal);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult NewNews(string content, string title, string userId, int categoryId, int proposalId)
        {
            News news = new News();
            try
            {
                if (ModelState.IsValid)
                {
                  
                    news.Title = title;
                    news.Content = content;
                    news.UserId = userId;
                    news.Date = DateTime.Now;
                    news.CategoryId = categoryId;
                    db.News.Add(news);
                    db.SaveChanges();

                    Proposal proposal = new Proposal();
                    proposal = db.Proposal.Find(proposalId);
                    db.Proposal.Remove(proposal);
                    db.SaveChanges();
                    TempData["message"] = "Articolul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(news);
                }
            }
            catch (Exception e)
            {
                return View(news);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Delete(int id)
        {
            Proposal proposal = db.Proposal.Find(id);
            if (proposal.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                db.Proposal.Remove(proposal);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un articol care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // Extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            // returnam lista de categorii
            return selectList;
        }
    }
}
