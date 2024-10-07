using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class RefreshTokenModel
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
    }
}
