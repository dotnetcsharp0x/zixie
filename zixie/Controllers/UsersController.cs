#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using zixie.Data;
using zixie.Models;

namespace zixie.Controllers
{
    public class UsersController : Controller
    {
        private readonly zixieContext _context;

        public UsersController(zixieContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(int? id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var users = await _context.User
                .FirstOrDefaultAsync(m => m.Email == email);
            if (id == null) id = 1;
            id = id - 1;
            int id_user = 0;
            if (users != null)
            {
                id_user = users.Id;
                ViewData["UserEmail"] = email;
                ViewData["UserId"] = users.Id;
                ViewData["PortfolioNickName"] = users.Nickname;
            }
            
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        // GET: Users/Portfolio/5
        public async Task<IActionResult> Portfolio(int? id)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var users = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users != null)
            {
                ViewData["UserEmail"] = email;
                ViewData["UserId"] = users.Id;
                ViewData["PortfolioNickName"] = users.Nickname;
            }
            var query = (from u in _context.Portfolio where u.Id_User == Convert.ToInt32(users.Id) select u);
            PortfolioViewModel ivm;
            ivm = new PortfolioViewModel { pPortfolio = query };

            return View(ivm);
        }

        // GET: Users/Profile/5
        public async Task<IActionResult> Profile()
        {
            var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            if (email == null)
            {
                return Redirect("/");
            }
            var users = await _context.User
                .FirstOrDefaultAsync(m => m.Email == email);
            

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Email == users.Email);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nickname")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Create_Portfolio(string name)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var users = await _context.User
                .FirstOrDefaultAsync(m => m.Email == email);
            Portfolio p = new Portfolio();
            p.Name = name;
            p.Id_User = users.Id;
            p.Date = DateTime.Now.ToString();
            _context.Add(p);
            _context.SaveChanges();
            return Redirect($"~/Users/Portfolio/{users.Id}");
        }
        //[HttpGet("{pvm}/{portfolioid}")]
        [HttpGet]
        public async Task<IActionResult> Create_PortfolioItem(PortfolioItems pvm)
        {
            Console.WriteLine(pvm);
            //Console.WriteLine(pId);
            var identity = (ClaimsIdentity)User.Identity;
            var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var users = await _context.User
                .FirstOrDefaultAsync(m => m.Email == email);
            pvm.Date= DateTime.Now.ToString();
            //pvm.Id_Portfolio = Convert.ToInt32(ViewBag.Message);
            _context.PortfolioItems.Add(pvm);
            _context.SaveChanges();
            return Redirect($"~/Users/Portfolio/"+users.Id);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nickname")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
