using System;
using System.Collections.Generic;
using System.Text;

namespace UI
{
    class Application
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.Run(new UI2());
        }
    }
}
