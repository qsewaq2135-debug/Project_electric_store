using EBIKES09.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBIKES09
{
    public static class DatabaseControl
    {
        public static List<User> GetUser()
        {
            using (Bdebikes09Context ctx = new Bdebikes09Context())
            {
                return ctx.Users.ToList();
            }
        }
        public static List<Product> GetProduct()
        {
            using (Bdebikes09Context ctx = new Bdebikes09Context())
            {
                return ctx.Products.ToList();
            }
        }
        public static List<string> GetPayMethod()
        {
            using (var context = new Bdebikes09Context())
            {
                return context.Payments.Select(p => p.PaymentName).Distinct().ToList();
            }
        }

    }
}
