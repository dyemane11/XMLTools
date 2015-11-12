using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

namespace ConsoleApplication1
{
    class Program
    {
        public static List<string> flatpaths = new List<string>();
        public static StringBuilder sb = new StringBuilder();

        private static void Main(string[] args)
        {
            const string startPath = ""; //"//driver";
            const string filename = "ADC-10-29";

            const bool ignoreAttributes = false;

            //var isdn = "1843369283";
            //var counter = 1;
            //Console.WriteLine("Counter:" + counter);
            //var isdnsum = isdn.Select(n =>
            //{
            //    var j = Int32.Parse(n.ToString(CultureInfo.InvariantCulture))*counter++;
            //    return j;
            //});

            //Console.WriteLine(isdnsum.Sum()%11 == 0 ? "Valid" : "Not Valid");

            var doc = XElement.Load(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\ADC\" + filename + ".xml");


            Flatten(!String.IsNullOrWhiteSpace(startPath) ? doc.XPathSelectElement(startPath) : doc, ignoreAttributes);
            //var filename = !String.IsNullOrWhiteSpace(startPath) ? startPath.Replace("/", ".") : "ADC-6-29";

            File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\ADC\" + filename + ".txt", flatpaths);
            flatpaths.ForEach(x =>
            {
                Console.WriteLine(x);
                Console.WriteLine();
            });
        
            Console.ReadLine();

            //SortElements(doc);
            //doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\XMLFile1-sorted.xml");

            //var tables = new List<string>(File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\ROPD\Oasis_Tables.txt"));
            //var newTables = new List<string>(File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\ROPD\Oasis_New_Tables.txt"));
            //foreach (var s in newTables)
            //{
            //    if (tables.Contains(s))
            //    {
            //        Console.WriteLine("***Affected: " + s);
            //    }
            //    else
            //    {
            //        Console.WriteLine("No dependency: " + s);
            //    }
            //}

            //var regex = new Regex(@"\<xs:restriction(.|\n)*?xs:restriction\>");

            //var regexin = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\regex_in.txt");

            //var matches = regex.Matches(string.Join(Environment.NewLine, regexin));
            
            //File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\regex_out.txt", 
            //    matches.Cast<Match>().Select(m => m.Value.Replace("\t","")).ToArray());

            //Console.WriteLine("done!");
            //Console.ReadLine();

            //Console.WriteLine("Sleeping!");
            //Thread.Sleep(5000);
            //Console.WriteLine("Waking up!");

            //CaptureApplication("communicator");
            //using (var bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
            //                                Screen.PrimaryScreen.Bounds.Height))
            //{
            //    using (var g = Graphics.FromImage(bmpScreenCapture))
            //    {
            //        g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
            //                         Screen.PrimaryScreen.Bounds.Y,
            //                         0, 0,
            //                         bmpScreenCapture.Size,
            //                         CopyPixelOperation.SourceCopy);

            //        bmpScreenCapture.Save(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\" +
            //                              Guid.NewGuid() + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //    }
            //}

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        //static void ValidationCallback(object sender, System.Xml.Schema.ValidationEventArgs args)
        //{
        //    if (args.Severity == System.Xml.Schema.XmlSeverityType.Warning)
        //    {
        //        Console.Write("WARNING: ");
        //        Console.WriteLine(args.Message);
        //    }
        //    else if (args.Severity == System.Xml.Schema.XmlSeverityType.Error)
        //    {
        //        Console.Write("ERROR: ");
        //        Console.WriteLine(args.Message);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public static Stack<string> stack = new Stack<string>();

        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            Console.WriteLine(args.Message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ignoreAttributes"></param>
        public static void Flatten(XElement node, bool ignoreAttributes)
        {
            var path = stack.Count == 0 ? string.Empty : stack.Reverse().Aggregate((current, next) => current + "/" + next);

            if (!node.HasElements)
            {
                var flatPath = path + "/" + node.Name;
                if (flatpaths.All(x => x != flatPath))
                {
                    flatpaths.Add(flatPath);
                    //GenCSharpPropertyCode(flatPath);
                }
            }

            if (!ignoreAttributes && node.Attributes().Any())
            {
                foreach (var attribute in node.Attributes().OrderBy(a => a.Name.LocalName))
                {
                    var attrbutePath = path + "/" + node.Name + "/@" + attribute.Name;
                    if (flatpaths.All(x => x != attrbutePath))
                    {
                        flatpaths.Add(attrbutePath);
                    }
                }
            }

            stack.Push(node.Name.LocalName);
            foreach (XElement n in node.Elements().OrderBy(e => e.Name.LocalName))
            {
                Flatten(n, ignoreAttributes);
            }

            stack.Pop();
        }

        public static void GenCSharpPropertyCode(string path)
        {
            var regex = new Regex("request.*/(.*)$");
            var match = regex.Match(path);

            var varName = match.Groups[1].Value.Substring(0,1).ToUpper() + match.Groups[1].Value.Substring(1);
            sb.Append("/// <summary>\n/// XPath for " + match.Groups[0] + "\n/// </summary>\npublic const string " + varName + " = \"" + match.Groups[0].Value + "\";\n\n");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public static void SortElements(XElement node)
        {
            if (!node.Elements().Any())
            {
                return;
            }

            foreach (var n in node.Elements())
            {
                SortElements(n);
            }
            node.ReplaceNodes(node.Elements().OrderBy(x => x.Name.LocalName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procName"></param>
        public static void CaptureApplication(string procName)
        {
            var proc = Process.GetProcessesByName(procName)[0];
            var rect = new User32.Rect();
            User32.GetWindowRect(proc.MainWindowHandle, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            bmp.Save(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\" +
                                          Guid.NewGuid() + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    public class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
    }
}
