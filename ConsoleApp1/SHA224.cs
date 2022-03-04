﻿using System;
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
            if (args.Length == 0) //Tikrina ar paduotas failas. Jei failas paleidimo metu nenurodytas, programa issijungia.
            {
                Console.Write("Privalote nurodyti failo pavadinima kaip parametra. PVZ.: SHA224.EXE Failas.txt");
                Console.ReadLine();
                Environment.Exit(0);
                return;
            }

            byte[] Tekstas = File.ReadAllBytes(args[0]); //Nuskaitomi baitai is paduoto failo.

            #endregion

            #region PREPROCESSING
            //Konstruojame bloka
            /* Kad tekstas butu suskirstytas po 512bitu:
             * 1. apskaiciuojame kiek reikes 0;
             * 2. apskaiciuojame kokio dydzio reikes bloko
             * 3. prideti 1 teksto gale.
             * 4. surandame kokio ilgio yra pradinis tekstas, ji paverciame kovertuojame i bitus ir paverciame i 64bitu skaiciu, tada idedame i bloko gala.
             */

            int k; //nuliu skaitiklis

            for (k = 0; (Tekstas.Length * 8 + 8 + k + 64) % 512 != 0; k += 8) ; // Suks cikla kol ras k su kurio modulis bus 0, tada lygybe bus neteisinga, o mes zinosim kiek reikes nuliu, kad uzpildyti bloka.

            #if DEBUG
            Console.WriteLine("Nuliu kiekis: " + k);
            Console.WriteLine("Raidziu kiekis faile: " + Tekstas.Length);
            Console.WriteLine("Bitai bloke: " + (64 + k + 8 + (Tekstas.Length * 8)));
            Console.WriteLine("Baitai bloke: " + (64 + k + 8 + Tekstas.Length * 8) / 8);
            Console.WriteLine();
            #endif

            // 64 - galiniai bitai teksto ilgiui, k - nuliu baitai, 8 - tai vienetas iterptas teksto gale, Tekstas.Length*8 pavercia i bitus.
            byte[] Blokas = new byte[(64 + k + 8 + (Tekstas.Length*8))/8]; //sukuriame bloka, kurio dydis yra apskaicuojamas pagal paduota teksto ilgi.
            
            Tekstas.CopyTo(Blokas,0); //perkeliame baitus i naujai sukurta suformatuota bloka

            Blokas[Tekstas.Length] = 128; // (128 DEC = 10000000 BIN) pridedame 1 po teksto bitais
            
            byte[] l = BitConverter.GetBytes(Convert.ToUInt64(Tekstas.Length * 8)).Reverse().ToArray(); //apsakicuojamas teksto ilgis baitais

            l.CopyTo(Blokas, Tekstas.Length + 1 + (k / 8)); // apskaiciuotas teksto ilgis pridedamas Bloko gale

            #if DEBUG
            foreach (byte i in Blokas)
            {
                string KonvertuotiIBinary = Convert.ToString(i, 2).PadLeft(8, '0'); //paverciama i binary
                    Console.Write(KonvertuotiIBinary + " ");
            }
            Console.WriteLine();
            Console.WriteLine();
            #endif

            #endregion

            #region Parsing
            int N = Blokas.Length / 64; //suzinome kiek reikes gabalu
            byte[,] chunk = new byte[N,64];//padalinam i 64baitu(512bitu) gabalus
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    chunk[i, j] = Blokas[(i * 64) + j]; //i*64 paema 512bitus is Bloko, o +j nurodo indexa pvz.: (0*64)+0=0, (1*64)+0=64 <- Jau antras chunk
                }         

            }


            for (int i = 0; i < N; i++) //Suks cikla tiek kartu, kiek yra gabalu
            {
                uint[] w = new uint[64];
                int x = 0;
                for (int j = 0; j < 16; j++)
                {
                    
                    w[j] = ((uint)(chunk[i,x] << 24 | (chunk[i,x + 1] << 16) | (chunk[i,x + 2] << 8) | (chunk[i,x + 3]))); //i 32bitu skaiciu sutalpinami po 4 8bitu skaicius.
                    // 01000100 00000000 00000000 00000000 -> 01000100 00011100 00000000 00000000  -> 01000100 00011100 01000001 00000000 ...
                    x += 4;
                }
                for(int m = 16; m < 64; m++)
                {
                    //

                    uint s0 = ((w[m - 15] >> 7) | (w[m - 15] << (32 - 7))) ^ ((w[m - 15] >> 18) | (w[m - 15] << (32 - 18))) ^ (w[m - 15] >> 3);
                    uint s1 = ((w[m - 2] >> 17) | (w[m - 2] << (32 - 17))) ^ ((w[m - 2] >> 19) | (w[m - 2] << (32 - 19))) ^ (w[m - 2] >> 10);
                    w[m] = (w[m - 16] + s0 + w[m - 7] + s1);
                }
                uint a = H[0];
                uint b = H[1];
                uint c = H[2];
                uint d = H[3];
                uint e = H[4];
                uint f = H[5];
                uint g = H[6];
                uint h = H[7];

                for(int n = 0; n < 64; n++)
                {
                    uint S1 = ((e >> 6) | (e << (32 - 6))) ^ ((e >> 11) | (e << (32 - 11))) ^ ((e >> 25) | (e << (32 - 25)));
                    uint ch = (e & f) ^ (~e & g);
                    uint temp1 = h + S1 + ch + K[n] + w[n];
                    uint S0 = ((a >> 2) | (a << (32 - 2))) ^ ((a >> 13) | (a << (32 - 13))) ^ ((a >> 22) | (a << (32 - 22)));
                    uint maj = (a & b) ^ (a & c) ^ (b & c);
                    uint temp2 = S0 + maj;

                    h = g;
                    g = f;
                    f = e;
                    e = d + temp1;
                    d = c;
                    c = b;
                    b = a;
                    a = temp1 + temp2;
                }

                H[0] += a;
                H[1] += b;
                H[2] += c;
                H[3] += d;
                H[4] += e;
                H[5] += f;
                H[6] += g;
                H[7] += h;

            }
            #endregion
            byte[] finalinis = new byte[28];

            for(int i = 0; i < 7; i++)
            {
                byte[] _ = new byte[28];
                _ = BitConverter.GetBytes(H[i]);
                Array.Reverse(_);
                Array.Copy(_, 0, finalinis, i*4, 4);
            }
            foreach (byte b in finalinis)
            {
                Console.Write(b.ToString("X2")); // atvaizduojamas 
            }
            Spausdinti(finalinis); // iskvieciamas spausdinimas i faila
            
            Console.ReadLine(); // Readline, kad neissijungtu programam
        }
        private static void Spausdinti(byte[] Tekstas) // Si funkcija isveda atsakyma i faila.
        {
            string Rezultatas = @"rezultatas.txt"; //Deklaruojame rezultato faila.

            try
            {
                if (File.Exists(Rezultatas)) // Tikrina ar yra failas, jei taip, ji istrina
                {
                    File.Delete(Rezultatas);
                }

                using (StreamWriter writer = File.CreateText(Rezultatas))
                {
                    foreach (byte b in Tekstas) 
                    {
                        // Kiekviena baita spausdiname i faila
                        writer.Write(b.ToString("X2"));
                    }
                }
            }
            catch (Exception ex) //Gaudome klaida
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
