using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRAS
{
    internal class utilities
    {
        public static Image BytetoImage(byte[] data) 
        {
            if(data == null) return null;
            Image image;
            var ms = new MemoryStream(data);
            image = Image.FromStream(ms);
            return image;
        }

        public static string StandardiseIntToString(int number, int sizeOfString)
        {
            string standardisedPrefix = number.ToString();

            while (standardisedPrefix.Length < sizeOfString)
            {
                standardisedPrefix = "0" + standardisedPrefix;
            }

            return standardisedPrefix;
        }

        public static byte[] ImagetoByte(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "Input image cannot be null.");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                // Save the image to the memory stream in a specific format (e.g., PNG)
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                // Convert the memory stream to a byte array
                return stream.ToArray();
            }
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

        public static visit_details GetVisitDetails(string customer_id, ref string visit_source)
        {
            BindingList<visit_details> visit_list = new BindingList<visit_details>();

            visit_list = redis_utilities.GetVisitDetails(MainForm.redisConnection, customer_id);
            if (visit_list.Count > 0)
            {
                visit_source = "memory";
            }

            else
            {
                visit_list = pgsql_utilities.GetVisitDetails(pgsql_utilities.ConnectToPGSQL(), $"customer_id = '{customer_id}'");
                if (visit_list.Count > 0)
                {
                    visit_source = "local";
                }
            }

            if(visit_list.Count > 0) return visit_list[0];

            return null;
        }
        public static redis_customer GetCustomerDetails(string customer_id, ref string customer_source)
        {
            BindingList<redis_customer> customer_list = new BindingList<redis_customer>();

            customer_list = redis_utilities.GetCustomerDetails(MainForm.redisConnection, customer_id);
            if (customer_list.Count > 0)
            {
                customer_source = "memory";
            }

            else
            {
                customer_list = pgsql_utilities.GetCustomerDetails(pgsql_utilities.ConnectToPGSQL(), $"customer_id = '{customer_id}'");
                if (customer_list.Count > 0)
                {
                    customer_source = "local";
                }
            }

            if(customer_list.Count > 0) return customer_list[0];

            return null;
        }

        public static bool IsDockerDaemonRunning()
        {
            try
            {
                // Run a simple Docker command to check if the daemon is running
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = "info",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process
                {
                    StartInfo = startInfo
                };
                process.Start();

                // Wait for the process to exit
                process.WaitForExit();

                // Check the exit code
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while checking Docker daemon status: {ex.Message}");
                return false;
            }
        }



        public static string ConvertToBinary(int number)
        {
            string binary = Convert.ToString(number,2);
            return binary;
        }
        public static void StartDockerDaemon()
        {
            try
            {
                // Start the Docker daemon
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = "daemon",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process
                {
                    StartInfo = startInfo
                };
                process.Start();

                // Wait for the process to exit
                process.WaitForExit();

                // Check the exit code
                if (process.ExitCode != 0)
                {
                    string errorMessage = process.StandardError.ReadToEnd();
                    MessageBox.Show($"Error starting Docker daemon: {errorMessage}");
                }
                else
                {
                    MessageBox.Show("Docker daemon started successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while starting Docker daemon: {ex.Message}");
            }
        }

    }
}
