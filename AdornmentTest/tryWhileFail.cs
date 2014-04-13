﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycles
{
    class tryWhileFail
    {
        public delegate void Del();
        const int MAXTRIES = 4;

        public static void execute(Del function,bool thrw = true)
        {
            bool failed = true;
            int tries = 0;
            while (failed)
            {
                try
                {
                    function();
                    failed = false;
                    tries++;
                }
                catch (System.Exception e)
                {
                    if (tries >= MAXTRIES)
                    {
                        if (thrw)
                            throw;
                        else
                            return;
                    }
                    System.Diagnostics.Debug.WriteLine("EXCEPTION ROOF");
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
        }
    }
}
