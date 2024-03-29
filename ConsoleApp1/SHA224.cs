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
        private static uint[] H = //inicializuojami registrai
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

        private static int k; //nuliu skaitiklis

        private static byte[] finalinis = new byte[28]; //inicializuojame finalini masyva, kuriame bus rezultatas

        private static void NuliuKiekis(byte[] Tekstas) //si funkcija suranda reikia nuliu skaiciu bloke.
        {
            for(; (Tekstas.Length*8 + 1 + k)%512 != 448; )
            {
                k++;// Suks cikla kol ras k su kuriuo modulis bus lygus 448. 
            }
        }

        private static void Formatavimas(byte[] Tekstas, byte[] Blokas, int k)
        {
            Array.Copy(Tekstas, Blokas, Tekstas.Length); //perkeliame pateikto teksto baitus i naujai sukurta bloka, su kuriuo atlkisime tolimesnius veiksmus

            Blokas[Tekstas.Length] = 1 << 7; //pridedame 1 po teksto bitais

            ulong tekstoIlgis = Convert.ToUInt64(Tekstas.Length * 8); //ilgis paverciamas i 64 bitu skaiciu

            byte[] l = BitConverter.GetBytes(tekstoIlgis);//teksto ilgis paverciamas i baitu masyva

            Array.Reverse(l, 0, 8); //apsukamas masyvas, kad ilgio simboliai butu gale

            Array.Copy(l, 0, Blokas, Tekstas.Length + 1 + (k/8), 8); // apskaiciuotas teksto ilgis pridedamas Bloko gale
        }

        private static void Padalinimas(int N, byte[] Blokas, byte[] chunk)
        {
            Array.Copy(Blokas, N * 64, chunk, 0, 64); //N*64 paema 64 baitus is Bloko ir perkelia i chunk masyvo nulini indexa
        }

        private static void Skaiciavimas(byte[] chunk)
        {
            uint[] w = new uint[64];
            for (int j = 0; j < 16; j++)
            {
                // i pirmus 16 w (message schedule array) (zodziu?) sudedame einamojo gabalo baitus
                byte[] _ = new byte[4];
                Array.Copy(chunk, j * 4, _, 0, 4);
                Array.Reverse(_);
                w[j] = BitConverter.ToUInt32(_, 0);
            }
           
            for (int m = 16; m < 64; m++)
            {
                // kitus w (zodziais?) padarome pagal formule:  s1(w[m-2]) + w[m - 7] + s0(m-15) + w[m - 16]
                // s0 = ROTR^7(x) xor ROTR^18(x) xor SHR^3(x)
                // s1 = ROTR^17(x) xor ROTR^19(x) xor SHR^10(x)


                uint s0 = ((w[m - 15] >> 7) | (w[m - 15] << (32 - 7))) ^ ((w[m - 15] >> 18) | (w[m - 15] << (32 - 18))) ^ (w[m - 15] >> 3);
                uint s1 = ((w[m - 2] >> 17) | (w[m - 2] << (32 - 17))) ^ ((w[m - 2] >> 19) | (w[m - 2] << (32 - 19))) ^ (w[m - 2] >> 10);
                w[m] = s1 + w[m - 7] + s0 + w[m - 16];
            }

            // inicializuojame astuonis darbinius kintamuosius ir jiems priskiariame pradines hash reiksmes, kurios deklaruotos programos pradzioje, o veliau jei reikia, naujai apskaiciuotas
            uint a = H[0], b = H[1], c = H[2], d = H[3], e = H[4], f = H[5], g = H[6], h = H[7];

            for (int n = 0; n < 64; n++)
            {
                // T1 = h + Sigma1 + choice + k[i] + w[i]
                // T2 = Sigma0 + majority
                // ch = (x & y ) ^ (~x & z); maj = (x & y) ^ (x & z) ^ (y & z);
                // Sigma0 = ROTR^2(x) ^ ROTR^13(x) ^ ROTR^22(x); Sigma1 = ROTR^6(x) ^ ROTR^11(x) ^ ROTR^25(x);

                uint Sigma1 = ((e >> 6) | (e << (32 - 6))) ^ ((e >> 11) | (e << (32 - 11))) ^ ((e >> 25) | (e << (32 - 25)));
                uint ch = (e & f) ^ (~e & g);
                uint Sigma0 = ((a >> 2) | (a << (32 - 2))) ^ ((a >> 13) | (a << (32 - 13))) ^ ((a >> 22) | (a << (32 - 22)));
                uint maj = (a & b) ^ (a & c) ^ (b & c);

                uint T1 = h + Sigma1 + ch + K[n] + w[n];
                uint T2 = Sigma0 + maj;
                //atnaujiname darbinius kintamuosius
                h = g;
                g = f;
                f = e;
                e = d + T1;
                d = c;
                c = b;
                b = a;
                a = T1 + T2;
            }
            //atnaujiname hash reiksmes
            H[0] += a;
            H[1] += b;
            H[2] += c;
            H[3] += d;
            H[4] += e;
            H[5] += f;
            H[6] += g;
            H[7] += h;
        }

        private static void Apdorojimas(byte[] finalinis)
        {
            for (int i = 0; i < 7; i++) //naudojam tik 7 H vietoj visu 8, nes 28 baitai
            {
                byte[] _ = new byte[28]; //laikinas masyvas
                _ = BitConverter.GetBytes(H[i]); // gauname H reiksme
                Array.Reverse(_); // gauta reiksme apsukam
                Array.Copy(_, 0, finalinis, i * 4, 4); // kopijuojame i finalini po 4 baitus, kas 4 indeksa
            }
        }
        
        private static void Spausdinti(byte[] Tekstas) // Si funkcija isveda atsakyma i terminala ir i faila.
        {
            foreach (byte b in Tekstas)
            {
                Console.Write(b.ToString("X2")); // atvaizduojama terminale
            }

            string Rezultatas = "rezultatas.txt"; //Deklaruojame rezultato faila.

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

        static void Main(string[] args)
        {
            #region Failo nuskaitymas
            if (args.Length == 0) //Tikrina ar paduotas failas. Jei failas paleidimo metu nenurodytas, programa issijungia.
            {
                Console.Write("Privalote nurodyti failo pavadinima ir vieta kaip parametra. PVZ.: SHA224.EXE ./Failas.txt");
                Console.ReadLine();
                Environment.Exit(0);
                return;
            }

            byte[] Tekstas = File.ReadAllBytes(args[0]); //Nuskaitomi baitai is paduoto failo.

            if(Tekstas.Length * 8 >= Math.Pow(2, 64))
            {
                Console.Write("Failo tekstas per ilgas.");
                Console.ReadLine();
                Environment.Exit(0);
            }
          
            #endregion

            #region PREPROCESSING

            NuliuKiekis(Tekstas);

            int bitai = Tekstas.Length * 8 + 1 + k + 64;
            int baitai = bitai / 8;

            #if DEBUG
            Console.WriteLine(Encoding.UTF8.GetString(Tekstas));
            Console.WriteLine("nuliu skaicius: " + k);
            Console.WriteLine("Simboliu kiekis faile: " + Tekstas.Length);
            Console.WriteLine("Bitai bloke: " + bitai);
            Console.WriteLine("Baitai bloke: " + baitai);
            Console.WriteLine();
            #endif
            
            byte[] Blokas = new byte[baitai];


            Formatavimas(Tekstas, Blokas, k);

            #if DEBUG
            foreach (byte i in Blokas) // Atvaizduoja bloka binary formatu
            {
                string KonvertuotiIBinary = Convert.ToString(i, 2).PadLeft(8, '0');
                    Console.Write(KonvertuotiIBinary + " ");
            }
            Console.WriteLine();
            Console.WriteLine();
            #endif

            #endregion

            #region Parsing
            
            int N = baitai / 64; //suzinome kiek reikes gabalu
            
            byte[] chunk = new byte[64];//inicializuojame 64baitu(512bitu) gabala

            for (int i = 0; i < N; i++)
            {
                Padalinimas(i, Blokas, chunk);
                
                Skaiciavimas(chunk);
            }

            #endregion
            
            Apdorojimas(finalinis);
            
            Spausdinti(finalinis); // iskvieciamas spausdinimas i faila
            
            Console.ReadLine(); // Readline, kad neissijungtu programam
        }
    }
}
