//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Monolith.Web.Models;
//using OneTrueError.Core;

//namespace Monolith.Web.Infrastructure
//{
//    public class CustomerDbProvider : ICustomerDbProvider
//    {
        

//        public string CustomerDbName
//        {
//            get
//            {
//                var dbName = HttpContext.Current.Items["CustomerDbName"] as string;
//                if (dbName != null)
//                {
//                    return dbName;
//                }

//                if (HttpContext.Current.User is ICustomerDbProvider)
//                {
//                    dbName = ((ICustomerDbProvider) HttpContext.Current.User).CustomerDbName;
//                    if (dbName != null)
//                        return dbName;
//                }

//                if (SessionUser.IsAuthenticated)
//                {
//                    return SessionUser.Current.DbName;
//                }

//                return null;
//            }
//        }
//    }
//}