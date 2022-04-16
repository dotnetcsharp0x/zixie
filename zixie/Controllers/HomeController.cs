#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tinkoff.InvestApi;
using zixie.Data;
using Microsoft.Extensions.Logging;
using zixie.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace zixie.Controllers
{
    public class HomeController : Controller
    {
        #region Vars
        private readonly zixieContext _context;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly int items_per_page = 10;
        #endregion
        #region HomeController
        public HomeController(zixieContext context
            , IHostApplicationLifetime lifetime
            , ILogger<HomeController> logger
            )
        {
            _context = context;
            _lifetime = lifetime;
        }
        #endregion
        #region Index
        public async Task<IActionResult> Index(int? id)
        {
            var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var users = await _context.User
                .FirstOrDefaultAsync(m => m.Email == email);
            if (id == null) id = 1;
            id = id - 1;
            int id_user = 0;
            if (users != null)
            {
                id_user = users.Id;
            }
            var first_query = (from p in _context.Shares
                                   //join i in _context.Watchlists on p equals i.Id into ps// p.Id equals i.Id_Instrument
                               join i in _context.Watchlists.Where(w=>w.Id_User==id_user) on p.Id equals i.Id_Instrument into i_group
                               from d in i_group.DefaultIfEmpty() 
                                   //where p.Currency == "rub"
                               //where a.Id_User == id_user
                               select new SharesTable()
                               {
                                   Name = p.Name,
                                   Currency = p.Currency,
                                   Ticker = p.Ticker,
                                   Watchlist = d// == null ? null : d
                               })
                              //.OrderBy(p => p.Name);
                              .Skip((int)id * items_per_page)
                              .Take(items_per_page);
            InstrumentsViewModel ivm = new InstrumentsViewModel { SharesTable = first_query};

            //var second_query = (from p in first_query select p).OrderBy(p=>p.Name);
            //var third_query = (from p in second_query select p).Take(10);

            //System.Diagnostics.Debug.WriteLine("test4: " + user.AuthenticationType + " " + user.Name + " " + user.IsAuthenticated);
            ViewBag.pages = new List<int> { (int)id - 1, (int)id - 0, (int)id+1, (int)id + 2, (int)id + 3 };
            ViewData["Page"] = (int)id+1;
            if (users != null)
            {
                ViewData["User_Id"] = users.Id;
            }
            else
            {
                ViewData["User_Id"] = 0;
            }
            return View(ivm);
        }
        #endregion
        #region Watchlist
        [Authorize]
        public async Task<IActionResult> Watchlist(int? id)
        {
            var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var users = await _context.User
                .FirstOrDefaultAsync(m => m.Email == email);
            if (id == null) id = 1;
            id = id - 1;
            int id_user = 0;
            if (users != null)
            {
                id_user = users.Id;
            }
            var first_query = (from p in _context.Shares
                                   //join i in _context.Watchlists on p equals i.Id into ps// p.Id equals i.Id_Instrument
                               join i in _context.Watchlists.Where(w => w.Id_User == id_user) on p.Id equals i.Id_Instrument into i_group
                               from d in i_group.DefaultIfEmpty()
                                   //where p.Currency == "rub"
                               //where d.Id_User == id_user
                               select new SharesTable()
                               {
                                   Name = p.Name,
                                   Currency = p.Currency,
                                   Ticker = p.Ticker,
                                   Watchlist = d// == null ? null : d
                               })
                              //.OrderBy(p => p.Name);
                              .Where(w=>w.Watchlist.Id_User==id_user)
                              .Skip((int)id * items_per_page)
                              .Take(items_per_page);
            InstrumentsViewModel ivm = new InstrumentsViewModel { SharesTable = first_query };

            //var second_query = (from p in first_query select p).OrderBy(p=>p.Name);
            //var third_query = (from p in second_query select p).Take(10);

            //System.Diagnostics.Debug.WriteLine("test4: " + user.AuthenticationType + " " + user.Name + " " + user.IsAuthenticated);
            ViewBag.pages = new List<int> { (int)id - 1, (int)id - 0, (int)id + 1, (int)id + 2, (int)id + 3 };
            ViewData["Page"] = (int)id+1;
            if (users != null)
            {
                ViewData["User_Id"] = users.Id;
            }
            else
            {
                ViewData["User_Id"] = 0;
            }
            return View(ivm);
        }
        #endregion
        #region Details
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
        #endregion
        #region Detail
        public async Task<IActionResult> Detail(string? id)
        {
            var back_link = Request.Headers;
            System.Diagnostics.Debug.WriteLine("Back link: " + back_link);
            if (id == null)
            {
                return NotFound();
            }
            var instruments = await _context.Shares
                .FirstOrDefaultAsync(m => m.Ticker == id);
            int id_insturment = instruments.Id;
            var someClaim = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var email = someClaim;
            var users = await _context.User
                .FirstOrDefaultAsync(m => m.Email == email);
            int id_user = 0;
            if (users != null)
            {
                id_user = users.Id;
            }
            List<Shares> shareModels = _context.Shares
                .Select(c => new Shares { Ticker = c.Ticker, Name = c.Name, Figi = c.Figi, Isin = c.Isin})
                .Where(m => m.Ticker == id)
                .ToList();
            if (id_user > 0)
            {
                List<Watchlist> watchlistModels = _context.Watchlists
                    .Select(c => new Watchlist { Id_Instrument = c.Id_Instrument, Id_User = c.Id_User })
                    .Where(m => m.Id_Instrument == id_insturment).Where(m => m.Id_User == id_user)
                    .ToList();

                InstrumentsViewModel ivm = new InstrumentsViewModel { Shares = shareModels, Watchlists = watchlistModels };
                return View(ivm);
            }
            else
            {
                InstrumentsViewModel ivm = new InstrumentsViewModel { Shares = shareModels, Watchlists = null };
                return View(ivm);
            }
        }
        #endregion
        #region Create
        public IActionResult Create()
        {
            var user_auth = HttpContext.User.Identity;
            if (!user_auth.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("/");
            }
        }
        #endregion
        #region AddToWatchList
        public async Task<int> AddToWatchlist(string? id)
        {
            var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var users = await _context.User.FirstOrDefaultAsync(m => m.Email == email);
            var instrument = await _context.Shares.FirstOrDefaultAsync(m => m.Ticker == id);
            Watchlist wl = new Watchlist();
            wl.Id_User = users.Id;
            wl.Id_Instrument = instrument.Id;
            var wl_find = from p in _context.Watchlists where p.Id_User == users.Id where p.Id_Instrument == instrument.Id select p;
            if (wl_find.Count() == 0)
            {
                _context.Add(wl);
                await _context.SaveChangesAsync();
            }
            return 1;
        }
        #endregion
        #region DeleteFromWatchlist
        public async Task<int> DeleteFromWatchlist(string? id)
        {
            var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var users = await _context.User.FirstOrDefaultAsync(m => m.Email == email);
            var instrument = await _context.Shares.FirstOrDefaultAsync(m => m.Ticker == id);
            var wl = from p in _context.Watchlists where p.Id_User == users.Id where p.Id_Instrument == instrument.Id select p;
            Watchlist wl_resp = new Watchlist();
            wl_resp.Id = wl.Select(i => i.Id).FirstOrDefault();
            wl_resp.Id_User = wl.Select(i => i.Id_User).FirstOrDefault();
            wl_resp.Id_Instrument = wl.Select(i => i.Id_Instrument).FirstOrDefault();
            _context.Remove(wl_resp);
            await _context.SaveChangesAsync();
            return 1;
        }
        #endregion
        #region Login
        public IActionResult Login()
        {
            var user_auth = HttpContext.User.Identity;
            if (!user_auth.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("/");
            }
        }
        #endregion
        #region Create_Action
        [HttpGet]
        public async Task<IActionResult> Create_Action([Bind("Name,Email,Password,ConfirmPassword")] RegisterModel Login)
        {
            var user_auth = HttpContext.User.Identity;
            if (!user_auth.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.User
                    .FirstOrDefaultAsync(m => m.Email == Login.Email);
                    if (user == null)
                    {
                        user = await _context.User
                    .FirstOrDefaultAsync(m => m.Nickname == Login.Name);
                    }
                    if (user != null)
                    {
                        return Redirect("/");
                    }
                    else
                    {
                        User person = new User();
                        person.Nickname = Login.Name;
                        person.Password = Login.Password;
                        person.Email = Login.Email;
                        _context.Add(person);
                        await _context.SaveChangesAsync();
                        var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
                        // создаем объект ClaimsIdentity
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    }
                }
                return Redirect("/");
            }
            else
            {
                return Redirect("/");
            }

        }
        #endregion
        #region Logout_Action
        public async Task<IActionResult> Logout_Action([Bind("Id,Nickname,Password")] User user)
        {
            var user_auth = HttpContext.User.Identity;
            if (!user_auth.IsAuthenticated)
            {
                return View();
            }
            else
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/");
            }
        }
        #endregion
        #region Login_Action
        public async Task<IActionResult> Login_Action([Bind("Email,Password")] LoginModel Login)
        {
            var user_auth = HttpContext.User.Identity;
            if (!user_auth.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.User
                    .FirstOrDefaultAsync(m => m.Email == Login.Email && m.Password == Login.Password);
                    if (user == null)
                    {
                        return Redirect("/");
                    }
                    else
                    {
                        User person = new User();
                        person.Email = Login.Email;
                        var claims = new List<Claim> { new Claim(ClaimTypes.Email, person.Email) };
                        // создаем объект ClaimsIdentity
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    }
                }
                return Redirect("/");
            }
            else
            {
                return Redirect("/");
            }
        }
        #endregion
        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            var user_auth = HttpContext.User.Identity;
            ViewData["Auth"] = user_auth.IsAuthenticated;
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
        #endregion
        #region Edit_action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nickname")] User user)
        {
            var user_auth = HttpContext.User.Identity;
            ViewData["Auth"] = user_auth.IsAuthenticated;
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

        #endregion
        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            var user_auth = HttpContext.User.Identity;
            ViewData["Auth"] = user_auth.IsAuthenticated;
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
        #endregion
        #region Delete_action
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user_auth = HttpContext.User.Identity;
            ViewData["Auth"] = user_auth.IsAuthenticated;
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion
        #region UserExists
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
        #endregion
    }
}
