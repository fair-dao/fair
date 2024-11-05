using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared
{
    /// <summary>
    /// 加密
    /// </summary>
    public static class Encrypt
    {
        public static string EncodeNum(string data)
        {
            byte[] btds=Encoding.UTF8.GetBytes(data+data.Length);
            byte[] bts = new byte[8];
            int i = 0;
            int sum = 0;
            foreach (byte b in btds)
            {
                sum+= b;
                if(bts[7-i]==0) bts[7-i]= (byte)(((Int32)b+sum)>>2);

                bts[i] = (byte)(((int) b) ^ (int)bts[i]);
                i++;
                if(i == 8)
                {
                    i = 0;
                }
            }
            for (int k= 0;k<8;k++)
            {
                if (bts[k] == 0) bts[k] = (byte)(k+sum);
                bts[k] =(byte) (0x30+(int)bts[k]%10);
            }
            return Encoding.UTF8.GetString(bts);
            
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptText">明文</param>
        /// <param name="key">密钥(长度32个字节)</param>
        /// <returns></returns>
        public static string AES256Encrypt(string encryptText, string key)
        {
            if (key?.Length >= 31)
            {
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptText);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = UTF8Encoding.UTF8.GetBytes(key,0,32) ;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.IV  = UTF8Encoding.UTF8.GetBytes(key.Substring(2, 16));
            
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            else throw new Exception("key长度必须大于31");
        }




        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptText">密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string AE256SDecrypt(string decryptText, string key)
        {
            if (key?.Length >= 31)
            {
                byte[] bts = Convert.FromBase64String(decryptText);
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = UTF8Encoding.UTF8.GetBytes(key, 0, 32);
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                rDel.IV = UTF8Encoding.UTF8.GetBytes(key.Substring(2, 16));

                ICryptoTransform form = rDel.CreateDecryptor();
                byte[] resultArray = form.TransformFinalBlock(bts, 0, bts.Length);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            else throw new Exception("key长度必须大于31");
        }



        /// <summary>  
        /// SHA1 加密，返回大写字符串  
        /// </summary>  
        /// <param name="data">需要加密字符串</param>  
        /// <returns>返回40位大写字符串</returns>  
        public static string SHA1(string data)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = Encoding.UTF8.GetBytes(data);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "").ToLower();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }

        public static string GetMD5(string txt)
        {
            try
            {
                MD5 md5 = MD5.Create();
                byte[] bytValue, bytHash;
                bytValue = System.Text.Encoding.UTF8.GetBytes(txt);
                bytHash = md5.ComputeHash(bytValue);
                md5.Clear();
                string sTemp = "";
                for (int i = 0; i < bytHash.Length; i++)
                {
                    sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
                }
                txt = sTemp.ToLower();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return txt;
        }






    }
}
