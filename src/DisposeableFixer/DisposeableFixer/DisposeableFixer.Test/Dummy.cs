//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DisFixerTest
//{
//    public class Class1 : IDisposable
//    {
//        private IDisposable MemoryStreamProperty { get;} = new MemoryStream();
//        private IDisposable MemoryStreamField = new MemoryStream();
//        public Class1()
//        {
//            var memStream = new MemoryStream();
//            //var reader = new StreamWriter(memStream);
//            using (new Newtonsoft.Json.JsonTextWriter(new StreamWriter(memStream)))
//            {
//            }

//            var reader = new StreamWriter(memStream);
//            using (new Newtonsoft.Json.JsonTextWriter(reader))
//            {
//            }
//        }
//        public void Dispose()
//        {
//            MemoryStreamField.Dispose();
//        }
//    }
//}
