using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace InfiniteRoleplay.Helpers
{
    internal class HTTPData
    {
        public static bool RequestIsOkay(WebRequest request)
        {
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    Console.WriteLine("Response");
                    bool containsImage = response.ContentType.Contains("image/");
                    long containsContent = response.ContentLength;
                    if (containsImage == true && containsContent > 0)
                    {
                        Console.WriteLine("it's an image");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                using (WebResponse response = ex.Response)
                using (Stream data = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(data))
                {
                    string errorMessage = reader.ReadToEnd();
                    return false;
                }
            }
        }
    }
}
