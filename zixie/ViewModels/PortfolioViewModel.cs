using zixie.Models;
using System.Collections.Generic;

public class PortfolioViewModel
{
    public IEnumerable<User> pUsers { get; set; }
    public IEnumerable<Shares> pShares { get; set; }
    public IEnumerable<Crypto> pCrypto { get; set; }
    public IEnumerable<SharesTable> pSharesTable { get; set; }
    public IEnumerable<Prices> pPrices { get; set; }
    public IEnumerable<CryptosTable> pCryptosTable { get; set; }
    public IEnumerable<Portfolio>? pPortfolio { get; set; }
    public IEnumerable<PortfolioItems> pPortfolioItems { get; set; }
}
