using System.IO;
using System;

namespace sqlc_gen_csharp {
 public static class UseHelloWorldGenerator
    {
        public static void Main()
        {
            // The static call below is generated at build time, and will list the syntax trees used in the compilation
            HelloWorldGenerated.HelloWorld.SayHello();
        }
    }
}
   