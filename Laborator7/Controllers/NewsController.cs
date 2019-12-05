using Laborator7.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Laborator7.Controllers
{
    public class NewsController : Controller
    {
        
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Article
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Index()
        {
            var news = db.News.Include("Category").Include("User");

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.News = news;

            return View();
        }

        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Show(int id)
        {
            News news = db.News.Find(id);

            ViewBag.afisareButoane = false;
            if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Administrator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();


            return View(news);

        }

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New()
        {
            News news = new News();

            // preluam lista de categorii din metoda GetAllCategories()
            news.Categories = GetAllCategories();

            // Preluam ID-ul utilizatorului curent
            news.UserId = User.Identity.GetUserId();


            return View(news);   

        }

        [HttpPost]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New(News news)
        {
            news.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    db.News.Add(news);
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

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int id)
        {
            News news = db.News.Find(id);
            ViewBag.Article = news;
            news.Categories = GetAllCategories();

            if (news.UserId == User.Identity.GetUserId() || 
                User.IsInRole("Administrator"))
            {
                return View(news);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        [HttpPut]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int id, News requestNews)
        {
            requestNews.Categories = GetAllCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    News news = db.News.Find(id);
                    if (news.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(news))
                        {
                            news.Title = requestNews.Title;
                            news.Content = requestNews.Content;
                            news.Date = requestNews.Date;
                            news.CategoryId = requestNews.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "Articolul a fost modificat!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    return View(requestNews);
                }

            }
            catch (Exception e)
            {
                return View(requestNews);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Delete(int id)
        {
            News news= db.News.Find(id);
            if (news.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                db.News.Remove(news);
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
