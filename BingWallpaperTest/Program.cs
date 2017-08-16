using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BingWallpaperTest
{
    class Program
    {
        static void Main(string[] args)
        {
            setWallpaper();
        }

        /**
*获取壁纸网络地址 别的不多说了直接扔出代码
*/
        public static string getURL()
        {
            string InfoUrl = "http://cn.bing.com/HPImageArchive.aspx?idx=0&n=1";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(InfoUrl);
            request.Method = "GET"; request.ContentType = "text/html;charset=UTF-8";
            string xmlDoc;
            //使用using自动注销HttpWebResponse
            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            {
                Stream stream = webResponse.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    xmlDoc = reader.ReadToEnd();
                }
            }
            // 使用正则表达式解析标签（字符串），当然你也可以使用XmlDocument类或XDocument类
            Regex regex = new Regex("<Url>(?<MyUrl>.*?)</Url>", RegexOptions.IgnoreCase);
            MatchCollection collection = regex.Matches(xmlDoc);
            // 取得匹配项列表
            string ImageUrl = "http://www.bing.com" + collection[0].Groups["MyUrl"].Value;
            if (true)
            {
                ImageUrl = ImageUrl.Replace("1366x768", "1920x1080");
            }
            return ImageUrl;
        }

        public static void setWallpaper()
        {
            string ImageSavePath = @"D:\Program Files\BingWallpaper";
            //设置墙纸
            Bitmap bmpWallpaper;
            WebRequest webreq = WebRequest.Create(getURL());
            //Console.WriteLine(getURL());
            //Console.ReadLine();
            WebResponse webres = webreq.GetResponse();
            using (Stream stream = webres.GetResponseStream())
            {
                bmpWallpaper = (Bitmap)Image.FromStream(stream);
                //stream.Close();
                if (!Directory.Exists(ImageSavePath))
                {
                    Directory.CreateDirectory(ImageSavePath);
                }
                //设置文件名为例：bing2017816.jpg
                bmpWallpaper.Save(ImageSavePath + "\\bing" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".jpg", ImageFormat.Jpeg); //图片保存路径为相对路径，保存在程序的目录下
            }
            //保存图片代码到此为止，下面就是
            string strSavePath = ImageSavePath + "\\bing" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".jpg";
            setWallpaperApi(strSavePath);
        }

        //利用系统的用户接口设置壁纸
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                string lpvParam,
                int fuWinIni
                );
        public static void setWallpaperApi(string strSavePath)
        {
            SystemParametersInfo(20, 1, strSavePath, 1);
        }
    }
}
