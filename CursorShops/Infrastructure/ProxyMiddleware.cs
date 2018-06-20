using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using System.Linq;
using System.Collections.Generic;

namespace CursorShops.Infrastructure
{
    public class ProxyServerMiddleware
    {
        private readonly RequestDelegate _next;
        private static string url = "";

        public ProxyServerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //фильтры - перехватывание запросов
            string[] UrlPart = context.Request.Path.Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            #region отлавливаем вызовы аудио и видео файлов
            if (context.Request.Path.Value.EndsWith(".mp3") || context.Request.Path.Value.EndsWith(".mp4"))
            {
                bool NewRequest = true;
                url = Startup.Configuration["RootWebSericeAddress"] + context.Request.Path.Value.Replace("/preview", "");
                System.Net.Http.HttpContent content = new System.Net.Http.StreamContent(context.Request.Body);
                HttpClientHandler httpClientHandler = new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    Credentials = new System.Net.NetworkCredential()
                    {
                        Domain = Startup.Configuration["Data:AdminUser:Domain"],
                        UserName = Startup.Configuration["Data:AdminUser:Login"],
                        Password = Startup.Configuration["Data:AdminUser:Password"]
                    }
                };
                using (HttpClient webRequest = new HttpClient(httpClientHandler))
                {
                    Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.FrameRequestHeaders header = (Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.FrameRequestHeaders)context.Request.Headers;
                    webRequest.BaseAddress = new Uri(Startup.Configuration["RootWebSericeAddress"]);
                    webRequest.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    int BeginByte = 0;
                    int BytesCount = 0;
                    if (string.IsNullOrEmpty(header.HeaderRange) || (!string.IsNullOrEmpty(header.HeaderRange) && header.HeaderRange == "bytes=0-"))
                    {
                        BytesCount = 0;
                    }
                    else
                    {
                        string tmp = header.HeaderRange.ToString().Replace("bytes=", "");
                        string[] ranges = tmp.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        BeginByte = Int32.Parse(ranges[0]);
                        if (ranges.Length == 2)
                        {
                            //указан конец диапазона
                            BytesCount = Int32.Parse(ranges[1]) - BeginByte;
                        }
                        else
                            BytesCount = 0;
                    }
                    HttpResponse localResponse = context.Response;
                    if (context.Request.Path.Value.EndsWith(".mp4"))
                        localResponse.ContentType = "video/mp4";
                    else
                        localResponse.ContentType = "audio/mp3";
                    localResponse.Headers.Add("Accept-Ranges", "bytes");
                    localResponse.StatusCode = 206;
                    int FileSize = 0;

                    m1:
                    //в цикле производим "досылку" данных в случае запросов "отсюда и до конца"ю Размер посылки в 1Мбайт зашит в веб-сервисе
                    try
                    {
                        //Обращаемся к дисковому кешу файлов, проверяем - есть ли там запрошенный файл
                        byte[] buffer = null;
                        #region Дисковый кэш Multimedia
                        if (Boolean.Parse(Startup.Configuration["MultimediaCachEnabled"]))
                        {
                            if (!Directory.Exists(Path.Combine("Temp", Startup.Environment.WebRootPath).Replace(".", "_")))
                                Directory.CreateDirectory(Path.Combine("Temp", Startup.Environment.WebRootPath).Replace(".", "_"));

                            string cache_path = Path.Combine("Temp", Startup.Environment.WebRootPath, System.Web.HttpUtility.UrlDecode(url).Replace(Startup.Configuration["RootWebSericeAddress"], "").Replace("/", "\\").Replace(".", "_"));
                            buffer = new byte[BytesCount == 0 ? 1048576 : BytesCount];
                            if (!File.Exists(cache_path))
                            {
                                //файл не найден, скачиваем в дисковый кеш
                                using (System.Net.WebClient cl = new System.Net.WebClient())
                                {
                                    cl.Credentials = new System.Net.NetworkCredential()
                                    {
                                        Domain = Startup.Configuration["Data:AdminUser:Domain"],
                                        UserName = Startup.Configuration["Data:AdminUser:Login"],
                                        Password = Startup.Configuration["Data:AdminUser:Password"]
                                    };
                                    byte[] buf = cl.DownloadData(url);
                                    //создаем структуру каталогов из имени файла                        
                                    Directory.CreateDirectory(Path.GetDirectoryName(cache_path));
                                    File.WriteAllBytes(cache_path, buf);
                                    FileSize = buf.Length;
                                    Buffer.BlockCopy(buf, 0, buffer, 0, BytesCount == 0 ? 1048576 : BytesCount);
                                }
                            }
                            else
                            {

                                //если исходный файл был перезаписан - нужно ручками вычистить каталог C:\Temp, чтобы снова была запись в кеш. Иначе - при каждом получении буфера данных нужно будет
                                //обращаться к исходному серверу и проверять размер исходного файла, при этом большинство смысла в кешировании пропадает.
                                //--считываем данные мегабайтными порциями

                                using (FileStream fs = new FileStream(cache_path, FileMode.Open, FileAccess.Read, FileShare.Read, 1048576, false))
                                {
                                    FileSize = (int)fs.Length;
                                    fs.Seek(BeginByte, SeekOrigin.Begin);
                                    int BytesRead = fs.Read(buffer, 0, BytesCount == 0 ? 1048576 : BytesCount);
                                    if (BytesRead < 1048576)
                                    {
                                        byte[] rez2 = new byte[BytesRead];
                                        Buffer.BlockCopy(buffer, 0, rez2, 0, BytesRead);
                                        buffer = rez2;
                                    }
                                }
                            }
                        }
                        #endregion
                        else
                        {
                            //передача данных без дискового кеширования на прокси-сервере
                            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
                            multipartContent.Add(new StringContent(url), "url");
                            multipartContent.Add(new StringContent(System.IO.Path.GetFileName(url)), "FileName");
                            multipartContent.Add(new StringContent("false"), "IsSystemIcon");
                            multipartContent.Add(new StringContent("false"), "IsPreview");
                            multipartContent.Add(new StringContent("multimedia"), "SiteID");
                            multipartContent.Add(new StringContent(BytesCount.ToString()), "BytesCount");
                            multipartContent.Add(new StringContent(BeginByte.ToString()), "BeginByte");
                            using (System.Net.Http.HttpResponseMessage response = await webRequest.PostAsync(Startup.Configuration["RootWebSericeAddress"] + "/_layouts/15/EsMVCWebUtils.asmx/GetBytesFromUrl", multipartContent))
                            {
                                response.EnsureSuccessStatusCode();
                                buffer = Convert.FromBase64String(await response.Content.ReadAsStringAsync());
                                IEnumerable<string> size;
                                if (response.Headers.TryGetValues("FileSize", out size))
                                    FileSize = Int32.Parse(size.FirstOrDefault());
                            }

                        }
                        if (NewRequest)
                        {
                            if (BytesCount == 0)
                            {
                                localResponse.Headers.Add("Content-Range", "bytes " + BeginByte.ToString() + "-" + (FileSize - 1).ToString() + "/" + FileSize.ToString());
                                localResponse.ContentLength = FileSize - BeginByte;
                            }
                            else
                            {
                                localResponse.Headers.Add("Content-Range", "bytes " + BeginByte.ToString() + "-" + (buffer.Length + BeginByte - 1).ToString() + "/" + FileSize);
                                localResponse.ContentLength = buffer.Length;
                            }
                        }
                        await localResponse.Body.WriteAsync(buffer, 0, buffer.Length);
                        BeginByte += buffer.Length;
                        NewRequest = false;
                        if (BeginByte != 0 && BeginByte < FileSize)
                        {
                            //если был другой запрос или пользователь закрыл просмотр видео - выйти
                            if (context.RequestAborted.IsCancellationRequested)
                                return;
                            goto m1;
                        }
                    }
                    catch (Exception e1)
                    {
                        return;
                    }
                }
                httpClientHandler.Dispose();
                return;
            }
            #endregion

            #region  отлавливаем чек-листы
            if (context.Request.Path.Value.EndsWith(".xlsx") && context.Request.Path.Value.Contains("/owa/"))
            {

                url = Startup.Configuration["RootWebSericeAddress"] + "/sites/kadr/_layouts/15/xlviewer.aspx?id=/sites/kadr" +
                    System.Web.HttpUtility.UrlEncode(context.Request.Path.Value.Replace(UrlPart[0] + "/owa", "").Replace("//", "/"));
                await StreamAsync(context, url);
                return;
            }
            #endregion

            #region  отлавливаем вызовы видео-конференций
            if (UrlPart.Length == 3 && UrlPart[0] == "Conference")
            {
                //обращение к видео-чатам              
                if (UrlPart[1] != "Records")
                {
                    url = Startup.Configuration["RootWebSericeAddress"] + "/tasks/conference/SitePages/" + UrlPart[1] + ".aspx?user_name=" + UrlPart[2];
                    await StreamAsync(context, url);
                    return;
                }
            }
            #endregion

            #region  отлавливаем вызовы веб-сервисов
            if (context.Request.Path.Value.Contains("asmx") || context.Request.Path.Value.Contains("ashx"))
            {
                if (UrlPart.Length == 2)
                    url = Startup.Configuration["RootWebSericeAddress"] + "/_layouts/15" + context.Request.Path.Value;
                else
                {
                    //отбираем 2 последних параметра

                    url = Startup.Configuration["RootWebSericeAddress"] + "/_layouts/15";
                    url += "/" + UrlPart[UrlPart.Length - 2];
                    url += "/" + UrlPart[UrlPart.Length - 1];
                }
                await StreamAsync(context, url);
                return;
            }
            #endregion

            #region  отлавливаем вызовы изображений для предварительного просмотра
            if (context.Request.Path.Value.Contains("/preview/"))
            {
                //получаем уменьшенные (превьюшки) изображения из веб-сервиса
                System.Net.Http.HttpContent content = new System.Net.Http.StreamContent(context.Request.Body);
                url = Startup.Configuration["RootWebSericeAddress"] + context.Request.Path.Value.Replace("/preview", "");
                HttpClientHandler httpClientHandler = new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                    Credentials = new System.Net.NetworkCredential()
                    {
                        Domain = Startup.Configuration["Data:AdminUser:Domain"],
                        UserName = Startup.Configuration["Data:AdminUser:Login"],
                        Password = Startup.Configuration["Data:AdminUser:Password"]
                    }
                };
                using (HttpClient webRequest = new HttpClient(httpClientHandler))
                {
                    HttpResponse localResponse = context.Response;
                    webRequest.BaseAddress = new Uri(Startup.Configuration["RootWebSericeAddress"]);
                    webRequest.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    MultipartFormDataContent multipartContent = new MultipartFormDataContent();
                    multipartContent.Add(new StringContent(url), "url");
                    multipartContent.Add(new StringContent(System.IO.Path.GetFileName(url)), "FileName");
                    multipartContent.Add(new StringContent("false"), "IsSystemIcon");
                    multipartContent.Add(new StringContent("true"), "IsPreview");
                    using (System.Net.Http.HttpResponseMessage response = await webRequest.PostAsync(Startup.Configuration["RootWebSericeAddress"] + "/_layouts/15/EsMVCWebUtils.asmx/GetBytesFromUrl", multipartContent))
                    {
                        response.EnsureSuccessStatusCode();
                        string str_tmp = await response.Content.ReadAsStringAsync();
                        byte[] buffer = Convert.FromBase64String(str_tmp);
                        await localResponse.Body.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
                return;
            }
            #endregion


            #region отлавливаем обращения к системным скриптам
            if (context.Request.Path.Value.Contains("/_layouts/15/") || context.Request.Path.Value.Contains("/_vti_bin/"))
            {
                url = context.Request.Path.Value.Replace(Startup.Configuration["AzureWebSiteAddress"], Startup.Configuration["RootWebSericeAddress"]);
                if (url.Substring(0, 4) != "http")
                    url = Startup.Configuration["RootWebSericeAddress"] + url;
                if (!string.IsNullOrEmpty(context.Request.QueryString.Value))
                    url += context.Request.QueryString.Value;
                await StreamAsync(context, url);
                return;
            }
            #endregion

            //Обычный функционал
            await _next(context);
        }

        private static async Task StreamAsync(HttpContext context, string url)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                Credentials = new System.Net.NetworkCredential()
                {
                    Domain = Startup.Configuration["Data:AdminUser:Domain"],
                    UserName = Startup.Configuration["Data:AdminUser:Login"],
                    Password = Startup.Configuration["Data:AdminUser:Password"]
                }
            };

            using (HttpClient webRequest = new HttpClient(httpClientHandler))
            {
                HttpResponse localResponse = context.Response;
                if (context.Request.Method == "GET")
                {
                    try
                    {
                        //localResponse.OnStarting(SetHeaders, state: context);
                        // здесь - если нужно с авторизацией получить контент через GET.
                        webRequest.BaseAddress = new Uri(url);
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url);
                        req.Content = new StringContent("", System.Text.Encoding.UTF8, "application/json");
                        using (System.Net.Http.HttpResponseMessage response = await webRequest.SendAsync(req, HttpCompletionOption.ResponseContentRead))
                        {
                            string tmp = await response.Content.ReadAsStringAsync();
                            tmp = tmp.Replace(Startup.Configuration["RootWebSericeAddress"], Startup.Configuration["AzureWebSiteAddress"])
                                .Replace(Startup.Configuration["RootWebSericeAddress"].Substring(7), Startup.Configuration["AzureWebSiteAddress"].Substring(7))
                                .Replace("src=\"/_layouts/15", "src=\"" + Startup.Configuration["AzureWebSiteAddress"] + "/_layouts/15");
                            localResponse.StatusCode = (int)response.StatusCode;
                            localResponse.ContentType = response.Content.Headers.ContentType.ToString();
                            foreach (var pars in response.Headers)
                            {
                                System.Collections.Generic.IEnumerable<string> vl = pars.Value;
                                localResponse.Headers.Add(pars.Key, vl.FirstOrDefault()
                                    .Replace(Startup.Configuration["RootWebSericeAddress"].Substring(7), Startup.Configuration["AzureWebSiteAddress"].Substring(7)));
                            }
                            await localResponse.WriteAsync(tmp);
                        }
                        return;

                    }
                    catch (Exception e1)
                    {
                        string error = e1.Message;
                    }
                }
                else
                {
                    //здесь обработка POST-запросов
                    try
                    {

                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);
                        webRequest.BaseAddress = new Uri(Startup.Configuration["RootWebSericeAddress"]);
                        //webRequest.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        foreach (var hd in context.Request.Headers)
                        {
                            System.Collections.Generic.IEnumerable<string> vl = hd.Value;
                            if (hd.Key != "Content-Type" && hd.Key != "Content-Length")
                            {
                                webRequest.DefaultRequestHeaders.Add(hd.Key, vl);
                                continue;
                            }
                            
                            if (hd.Key == "Content-Type")
                            {
                                if(string.Join(";", vl).Contains("json"))
                                    req.Content = new StringContent("", System.Text.Encoding.UTF8, "application/json");
                                if (string.Join(";", vl).Contains("urlencoded"))
                                    req.Content = new StringContent("", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                                continue;
                            }
                            
                        }


                        MultipartFormDataContent multipartContent = new MultipartFormDataContent();
                        try
                        {
                            foreach (var hd in context.Request.Form)
                            {
                                if (hd.Key == "url" && hd.Value.ToString().Contains("/preview/"))
                                    multipartContent.Add(new StringContent(Startup.Configuration["RootWebSericeAddress"] + hd.Value.ToString().Replace("/preview/", "/")), "url");
                                else
                                    multipartContent.Add(new StringContent(hd.Value.ToString()), hd.Key);
                            }
                            req.Content = multipartContent;
                        }
                        catch (Exception e1) { }
                        

                        //using (System.Net.Http.HttpResponseMessage response = await webRequest.PostAsync(url, multipartContent))
                        using (System.Net.Http.HttpResponseMessage response = await webRequest.SendAsync(req))
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
                                            await localResponse.Body.WriteAsync(buffer, 0, sizeOfReadedBuffer);
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
                                            byte[] buffer = ms.ToArray();
                                            await localResponse.Body.WriteAsync(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception e1)
                    {
                        string sss = e1.Message;
                    }
                }

            }
        }
        public static Task SetHeaders(object context)
        {
            var httpContext = (HttpContext)context;
            httpContext.Response.Redirect(url, false);
            httpContext.Response.Headers.Add("Authorization", String.Format("Basic {0}", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(@"easystep\spadmin:Gbgb$rf0Ktyz"))));
            return Task.FromResult(0);
        }
    }

    public static class ProxyServerMiddlewareExtension
    {
        public static IApplicationBuilder UseProxyServer(this IApplicationBuilder builder)
        {
            return builder.Use(next => new ProxyServerMiddleware(next).Invoke);
        }
    }
}