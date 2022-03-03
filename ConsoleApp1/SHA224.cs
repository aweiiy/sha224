using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SHA224
{
    internal class Program
    {
    private static uint[] H =
    {
     0xc1059ed8, 0x367cd507, 0x3070dd17, 0xf70e5939, 0xffc00b31, 0x68581511, 0x64f98fa7, 0xbefa4fa4
    };

    private static uint[] K =
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
            #region Failo nuskaitymas
            if (args.Length == 0) //tikrina ar paduotas failas
            {
                Console.WriteLine("Nurodykite failą.");
                Console.ReadLine();
                Environment.Exit(0);
                return;
            }

            string failas = args[0]; //paduotas failo pavadinimas

            byte[] Tekstas = File.ReadAllBytes(failas); //nuskaitomi baitai

            #endregion

            #region PREPROCESSING
            //Konstruojame bloka

            int k; //nuliu skaitiklis

            for (k = 0; (Tekstas.Length * 8 + 8 + k + 64) % 512 != 0; k += 8) ; // suks cikla kol ras i su kurio modulis bus 0, tada lygybe bus neteisinga, o mes zinosim kiek reikes nuliu.

            byte[] Blokas = new byte[(64 + k + 8 + Tekstas.Length*8)/8];
            
            Tekstas.CopyTo(Blokas,0);

            Blokas[Tekstas.Length] = 128; // (128 DEC = 10000000 BIN) pridedame 1 po teksto bitais

            // for (int i = 1; i < (1 + (k / 8)); i++) // k + 1, nes yra pridetas vienetas gale. GALIMA PAKEISTI I WHILE
            // Blokas[Tekstas.Length + i] = 0; 
            
            byte[] l = BitConverter.GetBytes(Convert.ToUInt64(Tekstas.Length * 8)).Reverse().ToArray();

            l.CopyTo(Blokas, Tekstas.Length + 1 + (k / 8));

            foreach (byte i in Blokas)
            {
                string KonvertuotiIBinary = Convert.ToString(i, 2).PadLeft(8, '0'); //paverciama i binary
                    Console.Write(KonvertuotiIBinary + " ");
            }

            #endregion

            #region Parsing
            int N = Blokas.Length / 64; //padalinam visa i 64baitu(512bitu) gabalus
            byte[,] chunk = new byte[N,64];
            Console.WriteLine();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    chunk[i, j] = Blokas[(i * 64) + j];
                }         

            }

            int[] w = new int[64];

            for (int i = 0; i < N; i++)
            {
                int x = 0;
                for (int j = 0; j < 16; j++)
                {
                  w[j] = chunk[i,x] << 24 | (chunk[i,x + 1] << 16) | (chunk[i,x + 2] << 8) | (chunk[i,x + 3]);
                    x += 4;
                }
            }
            #endregion
            Console.ReadLine();
        }
    }
}
