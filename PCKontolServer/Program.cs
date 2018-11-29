using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCKontolServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.StartServer(5678);
            Server.Listen();
        }
    }
}
