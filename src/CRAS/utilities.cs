using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRAS
{
    internal class utilities
    {
        public static Image BytetoImage(byte[] data) 
        {
            Image image;
            var ms = new MemoryStream(data);
            image = Image.FromStream(ms);
            return image;
        }

        public static BindingList<redis_customer> OrderBy(BindingList<redis_customer> customers, string column, string order = "ASC") 
        {
            List<redis_customer> list = new List<redis_customer>();

            if(order.Equals("ASC"))
            {
                list = customers.OrderBy(x => x.entry_time).ToList();
            }

            else if(order.Equals("DESC"))
            {
                list = customers.OrderByDescending(x => x.entry_time).ToList();
            }
            BindingList<redis_customer> sorted_list = new BindingList<redis_customer>(list);
            return sorted_list;
        }

        public static redis_customer UpdateCustomerRecord(redis_customer customer, string column_name, string new_value)
        {
            if(column_name == "name") customer.name = new_value;
            if(column_name == "phone_number") customer.phone_number = new_value;
            if (column_name == "remarks") customer.remarks = new_value;
            return customer;
        }

    }
}
