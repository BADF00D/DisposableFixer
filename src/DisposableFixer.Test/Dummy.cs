using System.IO;
using System.Text;

namespace Demo
{
    internal class Program
    {
        public Program()
        {
            var y = 0;
            var z = new MemoryStream();
            using (var reader = new StreamReader(z, Encoding.ASCII, true, 1024, true))
            {
                var x = 1;
            }
        }
    }
}