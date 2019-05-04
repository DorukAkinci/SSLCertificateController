using System;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography.X509Certificates;

using System.Configuration;
namespace SSLCertificateController
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var _initializations = ConfigurationManager.GetSection("Initializations") as Initializations;

                foreach (WebsiteElement _website in _initializations.ConnectionManagerEndpoints)
                {

                    Console.WriteLine("*********************************************");
                    Console.WriteLine("Address: " + _website.Address + " | Type: " + _website.Type + " | Thumbprint: " + _website.Thumbprint);
                    Console.WriteLine("*********************************************");

                    switch (_website.Type)
                    {
                        case "AddressCheck":
                            {
                                try
                                {
                                    var _cert = getSSLCertificate(_website.Address);
                                    if (_cert.Thumbprint == _website.Thumbprint)
                                    {
                                        Console.WriteLine("Thumbprint is matched.");
                                        if (_cert.NotAfter < DateTime.Now)
                                        {
                                            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!! Certificate is expired !!!!!!!!!!!!!!!!!!!! | Expired On: " + _cert.NotAfter.ToString());
                                            Console.WriteLine("Press any button to continue.");
                                            Console.ReadKey();
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!! Thumbprint is different than configuration file !!!!!!!!!!!!!!!!!!!! | Thumbprint: " + _cert.Thumbprint);
                                        Console.WriteLine("Press any button to continue.");
                                        Console.ReadKey();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            }
                        default:
                            {
                                var _Servers = ConfigurationManager.GetSection(_website.Type) as System.Collections.Specialized.NameValueCollection;
                                foreach (var _server in _Servers)
                                {
                                    string _ipAddress = _Servers[_server.ToString()];
                                    Console.WriteLine(" + " + _server as string + '(' + _ipAddress + ") is checking...");
                                    try
                                    {
                                        var _cert = getSSLCertificate(_ipAddress, _website.Address);
                                        if (_cert.Thumbprint == _website.Thumbprint)
                                        {
                                            Console.WriteLine("Thumbprint is matched.");
                                            if (_cert.NotAfter < DateTime.Now)
                                            {
                                                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!! Certificate is expired !!!!!!!!!!!!!!!!!!!! | Expired On: " + _cert.NotAfter.ToString());
                                                Console.WriteLine("Press any button to continue.");
                                                Console.ReadKey();
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!! Thumbprint is different than configuration file !!!!!!!!!!!!!!!!!!!! | Thumbprint: " + _cert.Thumbprint);
                                            Console.WriteLine("Press any button to continue.");
                                            Console.ReadKey();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Program is finished.");
        }

        public static X509Certificate2 getSSLCertificate(string _webSiteAddress)
        {
            try
            {
                return DoHttpsWebRequest("https://" + _webSiteAddress);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static X509Certificate2 getSSLCertificate(string _ipAddress, string _webSiteAddress)
        {
            try
            {
                string _host = _webSiteAddress;
                string _additional = "";
                int _additionalPos = _webSiteAddress.IndexOf('/');
                if (_additionalPos > -1)
                {
                    _host = _webSiteAddress.Substring(0, _additionalPos);
                    _additional = _webSiteAddress.Substring(_additionalPos);
                }

                return DoHttpsWebRequest("https://" + _ipAddress + _additional, _host);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static X509Certificate2 DoHttpsWebRequest(string URL, string _hostHeader = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            if (_hostHeader != null)
                request.Host = _hostHeader;
            request.AllowAutoRedirect = false;
            request.Timeout = 30 * 1000;
            request.Headers.Add("Cache-Control", "no-cache");
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.KeepAlive = false;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                //retrieve the ssl cert and assign it to an X509Certificate object
                X509Certificate cert = request.ServicePoint.Certificate;
                //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
                return new X509Certificate2(cert);
            }
        }
    }
}
