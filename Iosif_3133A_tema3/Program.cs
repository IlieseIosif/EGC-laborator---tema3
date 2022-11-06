using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iosif_3133A_tema3
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (VariatieCuloareTriunghi demo = new VariatieCuloareTriunghi())
            {
                demo.Run(30, 0);
            }
        }
    }
}
