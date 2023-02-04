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
using System.Transactions;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using NuGet.Protocol;

namespace zixie.Controllers
{
    public class HomeController : Controller
    {
        #region Vars
        private readonly zixieContext _context;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly int items_per_page = 10;
        private readonly int items_per_page_crypto = 50;

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
            InstrumentsViewModel ivm;
            var identity = (ClaimsIdentity)User.Identity;
                var email = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
                var users = await _context.User
                    .FirstOrDefaultAsync(m => m.Email == email);
                if (id == null) id = 1;
                id = id - 1;
                int id_user = 0;
            ViewData["UserEmail"] = "";
            ViewData["UserId"] = 0;
            ViewData["PortfolioNickName"] = "";
            if (users != null)
                {
                    id_user = users.Id;
                ViewData["UserEmail"] = email;
                ViewData["UserId"] = users.Id;
                ViewData["PortfolioNickName"] = users.Nickname;
            }
                
            if (id_user>0)
            {
                User person = new User();
                person.Email = users.Email;
                var claims = new List<Claim> { new Claim(ClaimTypes.Email, person.Email) };
                // создаем объект ClaimsIdentity
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                });
            }
                int to_skip = (int)id * items_per_page;
                var first_query = (from p in _context.Shares
                                   join i in _context.Watchlists.Where(w => w.Id_User == id_user) on p.Id equals i.Id_Instrument into i_group
                                   from d in i_group.DefaultIfEmpty()
                                   select new SharesTable()
                                   {
                                       Name = p.Name,
                                       Currency = p.Currency,
                                       Ticker = p.Ticker,
                                       Figi = p.Figi,
                                       Watchlist = d,
                                       Price = (from u in _context.Prices
                                                orderby u.Id descending
                                                where u.Figi == p.Figi
                                                select u.Price).AsParallel().FirstOrDefault()
                                   })
                                  .OrderBy(p => p.Name)
                                  .Skip(to_skip)
                                  .Take(items_per_page);
                string sql = first_query.ToQueryString();
                Console.WriteLine("START:");
                Console.WriteLine(sql);
                Console.WriteLine("END:");
                ivm = new InstrumentsViewModel { SharesTable = first_query };
                ViewBag.pages = new List<int> { (int)id - 1, (int)id - 0, (int)id + 1, (int)id + 2, (int)id + 3 };
                ViewData["Page"] = (int)id + 1;
                if (users != null)
                {
                    ViewData["User_Id"] = users.Id;
                }
                else
                {
                    ViewData["User_Id"] = 0;
                }
            //}
            return View(ivm);
        }
        #endregion
        #region SearchBox
        public async Task<IActionResult> Search(string? id)
        {
            var searchStocks = (from s in _context.Shares where s.Ticker.Contains(@"/"+id+"/") 
                                select new SharesTable()
                                {
                                    Name = s.Name,
                                    Currency = s.Currency,
                                    Ticker = s.Ticker,
                                    Figi = s.Figi,
                                    Price = (from u in _context.Prices
                                             orderby u.Id descending
                                             where u.Figi == s.Figi
                                             select u.Price).AsParallel().First()
                                });
            InstrumentsViewModel ivm = new InstrumentsViewModel { SharesTable = searchStocks };
            
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
            Console.WriteLine(users.Id);
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
                                   Watchlist = d,// == null ? null : d
                                   Price = (from u in _context.Prices
                                            orderby u.Id descending
                                            where u.Figi == p.Figi
                                            select u.Price).AsParallel().First()
                               })
                              
                              .Where(w=>w.Watchlist.Id_User==id_user)
                              .OrderBy(p => p.Name)
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
                .Select(c => new Shares { Ticker = c.Ticker, Name = c.Name, Figi = c.Figi, Isin = c.Isin,Currency=c.Currency,Sector=c.Sector })
                .Where(m => m.Ticker == id)
                .ToList();
            List<Prices> prices = new List<Prices>();
            var pric = (from u in _context.Prices
                       orderby u.Id descending
                       where u.Figi == shareModels.First().Figi
                       select u).AsParallel().First();
            var lstModel = new List<SimpleReportViewModel>();
            ViewData["date_now"] = DateTime.Now.AddDays(-365);
            ViewData["min"] = 99999999;
            string figi = shareModels.First().Figi;
            var crypto_price = (from u in _context.Prices where u.Figi == figi orderby u.Id select u).Reverse().Take(200).Reverse();
            foreach (var a in crypto_price)
            {
                if (Convert.ToDateTime(a.Date) > Convert.ToDateTime(ViewData["date_now"]))
                {
                    if (Convert.ToDecimal(a.Price) < Convert.ToDecimal(ViewData["min"]))
                    {
                        ViewData["min"] = a.Price;
                    }
                    lstModel.Add(new SimpleReportViewModel
                    {
                        DimensionOne = a.Date,
                        Quantity = a.Price
                    });
                }
            }
            prices.Add(pric);
            if (id_user > 0)
            {
                List<Watchlist> watchlistModels = _context.Watchlists
                    .Select(c => new Watchlist { Id_Instrument = c.Id_Instrument, Id_User = c.Id_User })
                    .Where(m => m.Id_Instrument == id_insturment).Where(m => m.Id_User == id_user)
                    .ToList();

                InstrumentsViewModel ivm = new InstrumentsViewModel { Shares = shareModels, Watchlists = watchlistModels,Prices = prices, SimpleReportViewModel = lstModel };
                return View(ivm);
            }
            else
            {
                InstrumentsViewModel ivm = new InstrumentsViewModel { Shares = shareModels, Watchlists = null, Prices = prices, SimpleReportViewModel = lstModel };
                return View(ivm);
            }
        }
        #endregion
        #region Crypto
        public async Task<IActionResult> Crypto(string? id)
        {
            Random rnd = new Random();
            var lstModel = new List<SimpleReportViewModel>();
            ViewData["date_now"] = DateTime.Now.AddHours(-24);
            ViewData["min"] = 99999999;
            var crypto_price = (from u in _context.Crypto where u.Symbol == id orderby u.Id select u).Reverse().Take(2200).Reverse();
                foreach (var a in crypto_price)
            {
                if (Convert.ToDateTime(a.Time) > Convert.ToDateTime(ViewData["date_now"]))
                {
                    if (Convert.ToDecimal(a.Price) < Convert.ToDecimal(ViewData["min"]))
                    {
                        ViewData["min"] = a.Price;
                    }
                    lstModel.Add(new SimpleReportViewModel
                    {
                        DimensionOne = a.Time,
                        Quantity = a.Price
                    });
                }
            }
            var back_link = Request.Headers;
            System.Diagnostics.Debug.WriteLine("Back link: " + back_link);
            if (id == null)
            {
                return NotFound();
            }
            var someClaim = HttpContext.User.Claims.Select(i => i.Value).FirstOrDefault();
            var email = someClaim;
            var users = await _context.User
                .FirstOrDefaultAsync(m => m.Email == email);
            int id_user = 0;
            if (users != null)
            {
                id_user = users.Id;
            }
            List<Shares> prices1 = new List<Shares>();
            List<Crypto> prices = new List<Crypto>();
            var pric = ((from u in _context.Crypto
                         orderby u.Id descending
                         where u.Symbol == id
                         select u).AsParallel().First());
            prices.Add(pric);
            if (id_user > 0)
            {
                //List<Watchlist> watchlistModels = _context.Watchlists
                //    .Select(c => new Watchlist { Id_Instrument = c.Id_Instrument, Id_User = c.Id_User })
                //    .Where(m => m.Id_Instrument == id_insturment).Where(m => m.Id_User == id_user)
                //    .ToList();

                InstrumentsViewModel ivm = new InstrumentsViewModel { Shares = prices1, Crypto = prices, SimpleReportViewModel = lstModel };
                return View(ivm);
            }
            else
            {
                InstrumentsViewModel ivm = new InstrumentsViewModel { Shares = prices1, Crypto = prices, SimpleReportViewModel = lstModel };
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
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                        });
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
        #region PortfolioItems
        public async Task<IQueryable<PortfolioItemsmodel>> PortfolioItems(int? id)
        {
            PortfolioViewModel pvm;
            var resp = (from u in _context.PortfolioItems where u.Id_Portfolio == id select new PortfolioItemsmodel()
            { Id=1,
            Instrument_Type=u.Instrument_Type,
            Price= (float?)Convert.ToDecimal((from s in _context.PortfolioItems where u.Id_Portfolio == s.Id_Portfolio && u.Id_Instrument == s.Id_Instrument select s.Price).Average())
            ,Date="!"
            ,Id_Instrument=u.Id_Instrument
            ,Id_Portfolio=u.Id_Portfolio,
            NameInstrument=u.NameInstrument,
            Ticker=u.Ticker,
            CuurentPrice= (float?)Convert.ToDecimal((from s in _context.Prices
                           orderby s.Id descending
                           where s.Figi == u.Figi
                           select s.Price).FirstOrDefault().ToString()),
            Currency =u.Currency,
            Count=Convert.ToInt32((from s in _context.PortfolioItems where s.Id_Instrument == u.Id_Instrument select s.Count).Sum(g=>g.Value))
            });
            //var resp1 = (from s in resp select s).GroupBy(s => s.Id_Instrument).Distinct();
            //var resp2 = (from d in resp1 select new PortfolioItems
            //{
            //    Id=1,
            //    Instrument_Type = Convert.ToInt32((from y in resp where y.Id_Instrument == d.Value select y.Instrument_Type)),
            //    Price = (float?)Convert.ToDecimal((from y in resp where y.Id_Instrument == d.Value select y.Price)),
            //    Date = Convert.ToString((from y in resp where y.Id_Instrument == d.Value select y.Date)),
            //    Id_Instrument=d.Value,
            //    Count = Convert.ToInt32((from y in resp where y.Id_Instrument == d.Value select y.Count)),
            //    Id_Portfolio = Convert.ToInt32((from y in resp where y.Id_Instrument == d.Value select y.Id_Portfolio)),
            //});
            ViewBag.Message = id;
            return resp.Distinct();
        }
        #endregion
        #region Load_Crypto
        public async Task<IQueryable<CryptosTable>> Load_Crypto(int? id)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            InstrumentsViewModel ivm;
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
            }
            ViewData["UserEmail"] = id_user;
            int to_skip = (int)id * items_per_page_crypto;

            var prepare = (from p in _context.Crypto select p).GroupBy(x => new { x.Name, x.Address,x.Symbol }).Select(y => new Crypto()
            {
                Name = y.Key.Name,
                Symbol = y.Key.Symbol,
                Address = y.Key.Address,
                VolumeYesterdayUSD =(from p in _context.Crypto where p.Symbol == y.Key.Symbol select p).FirstOrDefault().VolumeYesterdayUSD
            }
            );

            var first_query = (from p in prepare
                               select new CryptosTable()
                               {
                                   Symbol = p.Symbol,
                                   Name = p.Name,
                                   VolumeYesterdayUSD = p.VolumeYesterdayUSD,
                                   Address = p.Address,
                                   Price = (float)(from u in _context.Crypto
                                                   orderby u.Id descending
                                                   where u.Address == p.Address && u.Name == p.Name
                                                   select u.Price).AsParallel().FirstOrDefault()
                               });
                               //.GroupBy(g => new CryptosTable { p.Symbol, Name, Address, Price });
                               //.GroupBy(g=>g.Name)
                               //.OrderBy(x => x.Symbol)
                              
            //first_query = (from u in first_query orderby u.Address select u);
            //Console.WriteLine($"||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||{stopwatch.ElapsedMilliseconds}");
            //string path = "Logs\\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + ".log";
            //Console.WriteLine(stopwatch);
            //float ticks = stopwatch.ElapsedTicks;
            //float time = stopwatch.ElapsedMilliseconds;
            //using (StreamWriter writer = new StreamWriter(path, true))
            //{
                
            //    await writer.WriteLineAsync($"CR | {DateTime.Now.ToString()} | {ticks} | {time}");
            //}
            //stopwatch.Stop();
            //string sql = first_query.ToQueryString();
            Console.WriteLine("START:");
            //Console.WriteLine(sql);
            Console.WriteLine("END:");
            //ivm = new InstrumentsViewModel { CryptosTable = first_query };
            ViewBag.pages = new List<int> { (int)id - 1, (int)id - 0, (int)id + 1, (int)id + 2, (int)id + 3 };
            ViewData["Page"] = (int)id + 1;
            if (users != null)
            {
                ViewData["User_Id"] = users.Id;
            }
            else
            {
                ViewData["User_Id"] = 0;
            }
            first_query = first_query.OrderByDescending(x => x.VolumeYesterdayUSD);
            first_query = first_query.Skip(to_skip).Take(items_per_page_crypto);
            
            return first_query;
        }
        #endregion
        public IActionResult Test()
        {
            ViewData["Message"] = "Hello!";
            return View("Test");
        }
    }
}
