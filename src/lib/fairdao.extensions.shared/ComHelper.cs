using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fairdao.extensions.shared
{
    public static class ComHelper
    {

        /// <summary>
        /// 移除指定类型的注入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection Remove<T>(this IServiceCollection serviceCollection)
        {
            var serviceDescriptor = serviceCollection.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
            if (serviceDescriptor != null)
            {
                serviceCollection.Remove(serviceDescriptor);
            }

            return serviceCollection;
        }

        /// <summary>
        /// guid数据排序
        /// </summary>
        /// <param name="guids"></param>
        public static void Sort(Guid[] guids)
        {
            if (guids.Length < 2) return;
            if (guids[0].ToString().CompareTo(guids[1].ToString()) > 0)
            {
                Guid a = guids[0];
                guids[0] = guids[1];
                guids[1] = a;
            }

        }


        /// <summary>
        /// 生成一个随机的账号
        /// </summary>
        /// <param name="number"></param>
        /// <param name="userOther"></param>
        /// <returns></returns>
        public static string GetRand(int number, bool userOther = false)
        {
            char[] idWords = new char[]{'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n'
            , 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z','0','1','2','3','4','5','6','7','8','9'};
            char[] idOthers = new char[]
            {
            '-','~','$','^','|','@','`','!','*','.'
            };
            StringBuilder sb = new StringBuilder();
            Random r = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < number; i++)
            {
                if (userOther && r.Next(3) == 0) sb.Append(idOthers[r.Next(idOthers.Length)]);
                else sb.Append(idWords[r.Next(idWords.Length)]);
            }
            return sb.ToString();

        }


        /// <summary>
        /// 生成一个字符串（大写、小写、数字）
        /// </summary>
        /// <param name="number"></param>
        /// <param name="userOther"></param>
        /// <returns></returns>
        public static string GetRand(int minlen, int maxlen)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int len = minlen + random.Next(maxlen - minlen + 1);
            char[] chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                int k = random.Next(62);
                if (k >= 52)
                {
                    k -= 52;
                    chars[i] = (char)('0' + k);
                }
                else if (k >= 26)
                {
                    k -= 26;
                    chars[i] = (char)('A' + k);
                }
                else
                {
                    chars[i] = (char)('a' + k);
                }
            }
            return new string(chars);
        }





        /// <summary>
        /// 构造一个忽略SSL证书的httpClient
        /// </summary>
        /// <returns></returns>
        public static HttpClient BuildNOValidationSSLClient()
        {
            //为使httpClient能访问https协议网站必须添加这一项，忽略证书验证
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
            };
            return new HttpClient(httpHandler);
        }





        /// <summary>
        /// 是否正确手机号码(格式为+国家代码 手机号码）
        /// </summary>
        /// <param name="phoneNo">输入字符串</param>
        public static bool IsMobile(this string phoneNo)
        {
            long a = 0;
            if (phoneNo.Length < 18 && Regex.IsMatch(phoneNo, @"^[\d][\d]{8,16}$"))
                return true;
            else return false;
        }

        /// <summary>
        /// 是否为中国手机号码
        /// </summary>
        /// <param name="phoneNo">输入字符串</param>
        public static bool IsMobileCN(this string phoneNo)
        {
            long a = 0;
            if (phoneNo.Length < 18 && Regex.IsMatch(phoneNo, @"^1[\d]{10}$"))
                return true;
            else return false;
        }



        /// <summary>
        /// 获取当前程序集中的文件资源
        /// </summary>
        /// <param name="resFileName"></param>
        /// <returns></returns>
       public static byte[] GetFileResource(this System.Reflection.Assembly asm, string resFileName)
        {
            string strName = $"{asm.GetName().Name}.{resFileName}";
            Console.WriteLine(strName);
            byte[] data = null;
            try
            {
                System.IO.Stream ManifestStream = asm.GetManifestResourceStream(strName);

                data = new byte[ManifestStream.Length];
                ManifestStream.Read(data, 0, (int)ManifestStream.Length);
                ManifestStream.Close();
            }
            catch
            {
                throw new Exception($"资源{resFileName}获取失败");
            }
            return data;

        }





        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="url"></param>
        /// <param name="data">可以为字符串，HttContnent或object对象</param>
        /// <param name="method"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>


        public static Task<T> SubmitResult<T>(this IAPIHttpClient c, string url, object? data=null, HttpMethod? method = null)
        {
            data = data ?? string.Empty;
            HttpContent content;
            method= method ?? HttpMethod.Post;
            if (data is HttpContent)
            {
                content = data as HttpContent;
            }
            else if (data is string)
            {
                content = new StringContent(data as string);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            }
            else
            {
                content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);
            }


            return c.SendResult<T>(url, content, method);

        }


    


        /// <summary>
        /// 提交并返回数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static Task<T> SendResult<T>(this IAPIHttpClient c, string url, HttpContent content = null, HttpMethod? method = null)
        {
          

            return c.ToSend<Result<T>>(url, content, method).ContinueWith<T>(r =>
            {
                Exception e = null;
                Result<T> o = null;

                if (r.Exception == null)
                {
                    o = r.Result;
                    if (o != null)
                    {

                        c.ResultHander?.Invoke(o, o);
                        if (o.state == Result.STATE_OK)
                        {
                            return o.data;
                        }


                        if (o.state == "notlogin") //未登录
                        {
                            e = new fairdao.extensions.shared.exs.NoLoginException(o.msg);
                        }
                        else if (o.state == Result.STATE_ERR_NOPOWER) //无权访问
                        {
                            e = new fairdao.extensions.shared.exs.NoPowerException(o.msg);
                        }
                        else if (!string.IsNullOrEmpty(o.msg))
                        {
                            e = new Exception(o.msg);
                        }
                        else e = new Exception($"数据获取失败{url}");
                    }
                    e = e ?? new Exception("调用失败");
                    throw e;
                }
                else
                {
                    throw r.Exception;
                }
            });
        }



        /// <summary>
        /// 获取用户头像 
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static string GetUserFace(string face)
        {

            if (string.IsNullOrEmpty(face)) face = "/fairdao/images/noface.png";
            return face;

        }


        /// <summary>
        /// 格式化数据
        /// </summary>
        /// <param name="obj">数值</param>
        /// <param name="zeroHide">true:数值为0时不显示,false:数值为0时显示</param>
        /// <returns></returns>
        public static string FormatData(object obj, int dots = 0, bool zeroHide = false)
        {
            decimal d = (Decimal)obj;

            d = Math.Round(d, dots);
            if (d == 0 && zeroHide) return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("<span style=\"color:");
            if (d > 0)
            {
                sb.Append("blue");

            }
            else
            {
                if (d == 0)
                {
                    sb.Append("green");
                }
                else
                {
                    sb.Append("red");
                }
            }
            sb.Append(";\">");
            sb.Append(d);
            sb.Append("</span>");
            return sb.ToString();

        }


   

    }
}
