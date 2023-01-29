using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Xunit;
using zixie.Controllers;
using zixie.Data;
using zixie.Models;

namespace UnitTestApp.Tests
{
    public class HomeControllerTests
    {
        private readonly zixieContext _context;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<HomeController> _logger;
        public HomeControllerTests()
        {
            
        }
        [Fact]
        public void IndexViewDataMessage()
        {
            // Arrange
            HomeController controller = new HomeController(_context,_lifetime, _logger);

            // Act
            ViewResult result = controller.Test() as ViewResult;

            // Assert
            Assert.Equal("Hello!", result?.ViewData["Message"]);
        }

        [Fact]
        public void IndexViewResultNotNull()
        {
            // Arrange
            HomeController controller = new HomeController(_context, _lifetime, _logger);
            // Act
            ViewResult result = controller.Test() as ViewResult;
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void IndexViewNameEqualIndex()
        {
            var options = new DbContextOptionsBuilder<zixieContext>()
            .UseInMemoryDatabase(databaseName: "asd")
            .Options;
            var find = "AAPL";
            var context = new zixieContext(options);
            //_context.Add(new SharesTable { Ticker = "AAPL", Figi = "123" });
            // Arrange
            ValuesController controller = new ValuesController(context);
            context.Add(new Shares { Ticker = "AAPL", Figi = "123", Currency = "usd", DivYieldFlag = 1, Isin = "123", Exchange = "SPB", Name = "Apple", Nominal = "usd", Sector = "IT", BuyAvailableFlag = 1});
            
            // Act
            InstrumentsViewModel ivm = controller.SearchByName(find) as InstrumentsViewModel;
            Shares sh = new Shares();
            sh.Ticker = "AAPL";
            sh.Figi = "123";
            sh.Currency = "usd";
            sh.DivYieldFlag=1;
            sh.Isin= "123";
            sh.Exchange = "SPB";
            sh.Name = "Apple";
            sh.Nominal = "usd";
            sh.Sector = "IT";
            sh.BuyAvailableFlag = 1;
            Prices pr = new Prices();
            pr.Figi = "123";
            pr.Price = 123;
            pr.Date = "2023.01.18 10:34:56";
            context.Add(pr);
            context.SaveChanges();
            var searchStocks = (from s in context.Shares
                                where s.Ticker.Contains(@"" + find + "") || s.Name.Contains(@"" + find + "")
                                select new SharesTable()
                                {
                                    Name = s.Name,
                                    Currency = s.Currency,
                                    Ticker = s.Ticker,
                                    Figi = s.Figi,
                                    Price = (from u in context.Prices
                                             orderby u.Id descending
                                             where u.Figi == s.Figi
                                             select u.Price).AsParallel().First()
                                }).Take(5);
            InstrumentsViewModel ivm_Test = new InstrumentsViewModel { SharesTable = searchStocks };
            Assert.Equal(sh.Name, context.Shares.FirstOrDefault().Name);
        }
    }
}