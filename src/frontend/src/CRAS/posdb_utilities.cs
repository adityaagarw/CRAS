using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRAS
{
    internal class posdb_utilities
    {
        

        public static string GetConnectionString(string serverName, string dbName, string uid, string password, POSDB posdb)
        {
            string connection_string = "";

            switch (posdb)
            {
                case POSDB.SQL:
                    connection_string = "Server=" + serverName +";Initial Catalog=" + dbName + ";UID=" + uid + ";PWD=" + password + ";Pooling=False;";
                    break;

                case POSDB.PostGreSQL: 
                    break;

                case POSDB.MongoDB:
                    break;

            }


            return connection_string;
        }

    }
}
