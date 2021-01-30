using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace VBEditor
    {
    public class MyWeb
        {


        public  StringBuilder GetWebPage(string page)
            {
            StringBuilder sb = new StringBuilder();
            Stream resStream = null;
            
            HttpWebRequest request = request = (HttpWebRequest)
                      WebRequest.Create(page);
            HttpWebResponse response = response = (HttpWebResponse)
                    request.GetResponse();

            byte[] buf = new byte[8192];
            int count = 0;
            string tempString = null;

            try
                {
                resStream = response.GetResponseStream();
                do
                    {
                    count = resStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                        {
                        tempString = Encoding.ASCII.GetString(buf, 0, count);
                        sb.Append(tempString);
                        }
                    }
                while (count > 0); // any more data to read?
                }
            catch (Exception ex)
                {
                MessageBox.Show(ex.ToString());
                sb.Append("");
                }
            finally
                {
                resStream.Dispose();
                response.Close();
                request = null;
                
                }
            return sb;
            }
        }
    }
