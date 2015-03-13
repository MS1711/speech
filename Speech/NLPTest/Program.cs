using NLPCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPTest
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] s = {
                             "我想吃四川麻辣烫，oh,yeah",
                             "请把客厅的灯打开",
                             "这首歌真好听",
                         };
            //CWSTagger tag = new CWSTagger("");
            ////Console.WriteLine(tag.tag(s));
            //POSTagger tag2 = new POSTagger(tag, "");

            CNFactory f = CNFactory.GetInstance("");
            CNFactory.loadDict(@"C:\dict\mydict2.txt");

            foreach (var item in s)
            {
                string ss = f.tag2String(item);
                Console.WriteLine(ss);
            }
            
            Console.Read();
        }
    }
}
