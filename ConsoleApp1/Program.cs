﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
    private static uint[] h =
    {
     0xc1059ed8, 0x367cd507, 0x3070dd17, 0xf70e5939, 0xffc00b31, 0x68581511, 0x64f98fa7, 0xbefa4fa4
    };

    private static uint[] k =
    {
    0x428a2f98,0x71374491,0xb5c0fbcf,0xe9b5dba5,0x3956c25b,0x59f111f1,0x923f82a4,0xab1c5ed5,
    0xd807aa98,0x12835b01,0x243185be,0x550c7dc3,0x72be5d74,0x80deb1fe,0x9bdc06a7,0xc19bf174,
    0xe49b69c1,0xefbe4786,0x0fc19dc6,0x240ca1cc,0x2de92c6f,0x4a7484aa,0x5cb0a9dc,0x76f988da,
    0x983e5152,0xa831c66d,0xb00327c8,0xbf597fc7,0xc6e00bf3,0xd5a79147,0x06ca6351,0x14292967,
    0x27b70a85,0x2e1b2138,0x4d2c6dfc,0x53380d13,0x650a7354,0x766a0abb,0x81c2c92e,0x92722c85,
    0xa2bfe8a1,0xa81a664b,0xc24b8b70,0xc76c51a3,0xd192e819,0xd6990624,0xf40e3585,0x106aa070,
    0x19a4c116,0x1e376c08,0x2748774c,0x34b0bcb5,0x391c0cb3,0x4ed8aa4a,0x5b9cca4f,0x682e6ff3,
    0x748f82ee,0x78a5636f,0x84c87814,0x8cc70208,0x90befffa,0xa4506ceb,0xbef9a3f7,0xc67178f2
    };
        static void Main(string[] args)
        {

            if (args.Length == 0) //tikrina ar paduotas failas
            {
                Console.WriteLine("Nurodykite failą.");
                return;
            }

            string failas = args[0]; //paduotas failo pavadinimas

            byte[] inputBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(failas);
            foreach (byte b in inputBytes)
            {
                Console.WriteLine(b);
            }

            byte[] SkaitytiTeksta = File.ReadAllBytes(failas); //nuskaitomi baitai
            foreach (byte i in SkaitytiTeksta)
            {
                string KonvertuotiIBinary = Convert.ToString(i, 2).PadLeft(8, '0'); //paverciama i binary
                Console.Write(KonvertuotiIBinary + " ");
            }
            //Console.WriteLine("programos pabaiga, spauskite norint isjungti");
            Console.ReadLine();
        }
    }
}
