using System;

namespace ProjectPsi
{
    class Program
    {
#if NETFX_CORE
        [MTAThread]
#else
        [STAThread]
#endif
        static void Main()
        {
            using (var program = new PsiGame())
                program.Run();
        }
    }
}
