﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Console {
    class Program {
        static void Main(string[] args) {
            EventBus.Run();
            System.Console.ReadKey();
        }
    }
}