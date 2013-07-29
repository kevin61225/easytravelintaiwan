using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            AgilityRequest ar = new AgilityRequest();
            ar.StartSpider();
            //ar.GetThirdLayerData("花博公園", 7, "m1.aspx?sNo=0001090&id=A12-00140");
            //ar.GetThirdLayerData("總統府", 7,   "m1.aspx?sNo=0001090&id=137");
            Console.ReadLine();
        }
    }
}
