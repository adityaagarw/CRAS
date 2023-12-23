using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRAS
{
    public class redis_employee
    {
        public string key {  get; set; }
        public string employee_id {  get; set; }
        public string name { get; set; }
        
        public string phone_number { get; set; }
        public byte[] image { get; set; }
        public string entry_time { get; set; }
        public string exit_time { get; set; }
    
        public int num_exits { get; set; }

        public int in_store { get; set; }

        public static int IndexOf(BindingList<redis_employee> employeeList, redis_employee employee)
        {
            int index = -1;

            foreach (var x in employeeList)
            {
                index++;
                if (employee.employee_id == x.employee_id)
                    return index;
            }
            return -1;
        }
    }

    
}
