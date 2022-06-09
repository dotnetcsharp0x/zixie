using Microsoft.AspNetCore.Mvc;
using zixie.Data;
using zixie.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace zixie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly zixieContext _context;
        public ValuesController(zixieContext context
            )
        {
            _context = context;
        }

        [HttpGet]
        [Route("Search/{id}")]
        public InstrumentsViewModel SearchByName(string id)
        {
            var searchStocks = (from s in _context.Shares
                                where s.Ticker.Contains(@"" + id + "") ||  s.Name.Contains(@"" + id + "")
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
                                }).Take(5);

            //if (searchStocks.ToList().Count == 0)
            //{
            //    searchStocks = (from s in _context.Shares
            //                    where s.Name.Contains(@"" + id + "")

            //                    select new SharesTable()
            //                    {
            //                        Name = s.Name,
            //                        Currency = s.Currency,
            //                        Ticker = s.Ticker,
            //                        Figi = s.Figi,
            //                        Price = (from u in _context.Prices
            //                                 orderby u.Id descending
            //                                 where u.Figi == s.Figi
            //                                 select u.Price).AsParallel().First()
            //                    }).Take(5);
            //}
            InstrumentsViewModel ivm = new InstrumentsViewModel { SharesTable = searchStocks };

            return ivm;
        }
    }
}
