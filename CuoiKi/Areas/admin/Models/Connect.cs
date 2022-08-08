using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CuoiKi.Areas.admin.Models
{

    public class Connect
    {
        public static SqlConnection connect()
        {
            string strCon = "server=.\\SQLEXPRESS; database=dataGiayXml;integrated security=true";
            SqlConnection con = new SqlConnection(strCon);
            return con;
        }
        
    }
}