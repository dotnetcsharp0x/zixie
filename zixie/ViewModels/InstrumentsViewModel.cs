using zixie.Models;
using System.Collections.Generic;

public class InstrumentsViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Shares> Shares { get; set; }
        public IEnumerable<Watchlist> Watchlists { get; set; }
        public IEnumerable<SharesTable> SharesTable { get; set; }
    }
