using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Vidar
{
    internal class Cartel
    {
        public int id {  get; set; }
        public string name { get; set; }
        public int created {  get; set; }
        public float reputation { get; set; }
        public string status { get; set; }
    }
}
