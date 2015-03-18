using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        public static List<string> flatpaths = new List<string>();
        static void Main(string[] args)
        {
            var doc = XElement.Load(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\XMLFile1.xml");
            SortElements(doc);
            doc.Save(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\XMLFile1-sorted.xml");

            Console.WriteLine("done!");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public static void Flatten(XElement node)
        {
            var path = stack.Count == 0 ? string.Empty : stack.Reverse().Aggregate((current, next) => 
                current + "/" + next
                );

            //Console.WriteLine(path + "/" + node.Name);
            flatpaths.Add(path + "/" + node.Name);

            stack.Push(node.Name.LocalName);
            foreach (XElement n in node.Descendants())
                Flatten(n);

            stack.Pop();
        }

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
    }
}
