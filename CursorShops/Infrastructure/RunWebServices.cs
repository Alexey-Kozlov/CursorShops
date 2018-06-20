using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System.IO;


namespace CursorShops.Infrastructure
{
    public class RunWebServices
    {
        public static async Task<string> RunWebServise(string WebServiceMethod, Microsoft.AspNetCore.Http.HttpContext context, MethodType MethodType, Dictionary<string,string> Params)
        {
            string url = Startup.Configuration["RootWebSericeAddress"] + "/_layouts/15/" + WebServiceMethod;
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                Credentials = new System.Net.NetworkCredential() { Domain = Startup.Configuration["Data:AdminUser:Domain"], UserName = Startup.Configuration["Data:AdminUser:Login"],
                    Password = Startup.Configuration["Data:AdminUser:Password"]
                }
            };
            List<byte> rez_byte = new List<byte>();
            using (HttpClient webRequest = new HttpClient(httpClientHandler))
            {
                switch(MethodType)
                {
                    case MethodType.POST:
                        webRequest.BaseAddress = new Uri(Startup.Configuration["RootWebSericeAddress"]);
                        webRequest.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        //заполняем заголовки                        
                        foreach (var hd in context.Request.Headers)
                        {
                            System.Collections.Generic.IEnumerable<string> vl = hd.Value;
                            if (hd.Key != "Content-Type" && hd.Key != "Content-Length")
                                webRequest.DefaultRequestHeaders.Add(hd.Key, vl);
                        } 
                        
                        MultipartFormDataContent multipartContent = new MultipartFormDataContent();                        
                        foreach (KeyValuePair<string,string> par in Params)
                        {
                            multipartContent.Add(new StringContent(par.Value??""), par.Key);
                        }
                        
                        using (System.Net.Http.HttpResponseMessage response = await webRequest.PostAsync(url, multipartContent))
                        {
                            response.EnsureSuccessStatusCode();
                            IEnumerable<string> headers = new List<string>();
                            if (response.Content.Headers.TryGetValues("Content-Type", out headers))
                            {
                                if (headers.SingleOrDefault() == "text; charset=utf-8")
                                {
                                    using (System.IO.Stream ms = await response.Content.ReadAsStreamAsync())
                                    {
                                        ms.Seek(0, SeekOrigin.Begin);
                                        int bufferSize = 1048576;
                                        byte[] buffer = new byte[bufferSize];
                                        int totalSize = (int)ms.Length;
                                        while (totalSize > 0)
                                        {
                                            int count = totalSize > buffer.Length ? buffer.Length : totalSize;
                                            int sizeOfReadedBuffer = ms.Read(buffer, 0, count);
                                            byte[] tmp = new byte[count];
                                            Buffer.BlockCopy(buffer, 0, tmp, 0, count);
                                            rez_byte.AddRange(tmp);
                                            totalSize -= sizeOfReadedBuffer;
                                        }
                                    }
                                }
                                else
                                {                                 
                                    using (System.IO.Compression.GZipStream gz_stream = new System.IO.Compression.GZipStream(await response.Content.ReadAsStreamAsync(), System.IO.Compression.CompressionMode.Decompress))
                                    {
                                        using (System.IO.MemoryStream ms = new MemoryStream())
                                        {
                                            gz_stream.CopyTo(ms);
                                            rez_byte.AddRange(ms.ToArray());
                                        }
                                    }
                                    
                                }
                            }
                        }
                   
                        break;
                    case MethodType.GET:
                        break;
                }
            }
            httpClientHandler.Dispose();
            return System.Text.Encoding.UTF8.GetString(rez_byte.ToArray());
        }
    }

    public enum MethodType
    {
        GET,POST
    }
}
