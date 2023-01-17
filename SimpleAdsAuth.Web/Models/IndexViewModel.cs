using SimpleAdsAuth.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Web.Models
{
    public class IndexViewModel
    {
        public List<Ad> Ads { get; set; }
        public User User { get; set; }
    }
}
