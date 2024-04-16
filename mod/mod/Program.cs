using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace prueba
{
    class Codigomod
    {
        static int ejeX = 0; static int Enemigo = 0;
        static int barcos = 0; static int ejeY = 0;
        static int Player1 = 0; static string usuario;
        static int ptsobtenidos = 0; static int puntosPerdidos;
        static string tamaño = "";
        static string[] InterpretarArchivotxt()
        {
            List<string> texto = new List<string>(); using (var leer = new StreamReader(@"Barcos.txt"))
            {
                while (!leer.EndOfStream)
                {
                    var x = leer.ReadLine();
                    if (x.Contains("tamaño"))
                    {
                        tamaño = x.Substring("tamaño".Length);
                    }
                    else
                    {
                        var word = x.Split(",");
                        if (Convert.ToInt32(word[3]) >= 2 && Convert.ToInt32(word[3]) <= 4)
                        {
                            texto.Add(x);
                        }
                    }
                }
            }
            return texto.ToArray();
        }
        static void Main(string[] args)
        {
            Console.WriteLine("¡Hey, bievenido! ingresa tu alias preferido para continuar\n");
            usuario = Console.ReadLine();
            
            string door = ""; string[] archivo = InterpretarArchivotxt();
            do
            {
                
                Console.Clear();
                //Console.WriteLine("¡Vamos a empezar! Ingresa el tamaño del tablero " + usuario + ":\n");
               
                if (!tamaño.Contains(","))
                {
                    Console.Clear(); Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("El tamaño del tablero es incorrecto " + usuario + " intenta en un rango de 4-10");
                    Thread.Sleep(1900); Console.ForegroundColor = ConsoleColor.White;
                }
            } while (!tamaño.Contains(","));
            string[,] matriz = matrices(tamaño);
            var oponente = new string[ejeY, ejeX];

            for (int i = 0; i < ejeX; i++)
            {
                for (int j = 0; j < ejeY; j++)
                {
                    matriz[i, j] = "-";
                    oponente[j, i] = "-";
                }
            }
            int turnosconteo = 1;
            PosicionaBarcos(matriz, archivo); PosicionaMatriz(matriz); MatrizEnemigo(oponente);
            do
            {
                Console.BackgroundColor = ConsoleColor.Black; 
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Turno: {0}\n", turnosconteo);
                Console.ForegroundColor = ConsoleColor.White;
                if ((turnosconteo % 2) == 0)
                {
                    Random aleatorio = new Random();
                    string coordenadaX = aleatorio.Next(1, ejeX) + ""; string coordenadaY = aleatorio.Next(1, ejeY) + "";
                    Disparar(matriz, coordenadaX, coordenadaY);
                    turnosconteo++;
                    Console.Clear(); Console.ForegroundColor = ConsoleColor.DarkRed; 
                    Console.WriteLine("El enemigo tiró a la coordenada : ''{0},{1}'' 'ENTER' para continuar", coordenadaX, coordenadaY);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.ReadLine();
                }
                else
                {
                    Console.Write("Capitán, dinos cual es el objetivo a atacar: ");
                    door = Console.ReadLine();
                    if (door == "F" || door == "f")
                    {
                        break;
                    }
                    else if (door.Contains(","))
                    {
                        string[] paquete = door.Split(","); string fil = paquete[0]; string col = paquete[1];
                        disparar(matriz, oponente, fil, col); turnosconteo++;
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Ese objetivo está fuera de alcance o no es correcto");
                        Console.ForegroundColor = ConsoleColor.White;
                        Thread.Sleep(2500);
                    }

                }
                PosicionaMatriz(matriz); MatrizEnemigo(oponente);
            } while (door != "F" || door != "f");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n\nEl juego terminó en el turno : {0}", turnosconteo);
            Console.WriteLine("\nTú : {0}/{1} \tEnemigo : {2}/{3}", Player1, barcos, Enemigo, barcos);
            showB(archivo, oponente, matriz);
        }
        static string[,] matrices(string dimensiones)
        {
            var EjeXejeY = dimensiones.Split(",");
            ejeX = Convert.ToInt32(EjeXejeY[0]); ejeY = Convert.ToInt32(EjeXejeY[1]);
            while ((ejeX < 4 || ejeY < 4) || (ejeX > 10 || ejeY > 10))
            {
                Console.Clear(); 
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Capitán ese tablero esta fuera del límite, intenta nuevamente 'presiona ENTER'");
                Console.ReadLine(); 
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();
                Console.Write("¡Vamos a empezar! Ingresa el tamaño del tablero:\n");
                string auxiliar = Console.ReadLine();
                string[] contenido = auxiliar.Split(",");
                ejeX = Convert.ToInt32(contenido[0]); ejeY = Convert.ToInt32(contenido[1]);
            }
            return new string[ejeX, ejeY];
        }
        static void PosicionaMatriz(string[,] tabla)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Juego de :" + usuario + "\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" \t");
            for (int x = 0; x < ejeY; x++)
            {
                Console.Write("{0}\t", x + 1);
            }
            Console.WriteLine();
            for (int y = 0; y < ejeX; y++)
            {
                Console.Write("{0}\t", y + 1);
                for (int h = 0; h < ejeY; h++)
                {
                    Console.Write("{0}\t", tabla[y, h]);
                }
                Console.WriteLine();
            }
        }

        static void PosicionaBarcos(string[,] tabla, string[] direccionataque)
        {
            for (int x = 0; x < direccionataque.Length; x++)
            {
                string[] paquete = direccionataque[x].Split(",");
                int start = Convert.ToInt32(paquete[0]) - 1; int end = Convert.ToInt32(paquete[3]);
                int ayuda = Convert.ToInt32(paquete[1]) - 1;
                if (paquete[2] == "H")
                {
                    for (int i = ayuda; i < ayuda + end; i++)
                    {
                        tabla[start, i] = "BARCO"; barcos++;
                    }
                }
                else if (paquete[2] == "V")
                {
                    for (int i = start; i < start + end; i++)
                    {
                        tabla[i, ayuda] = "BARCO"; barcos++;
                    }
                }
            }
        }
        //cambia barco por X
        static void Disparar(string[,] tablero, string ejeY, string X)
        {
            int ejeX = Convert.ToInt32(ejeY) - 1; int accionX = Convert.ToInt32(X) - 1;
            if (tablero[ejeX, accionX] == "BARCO")
            {
                tablero[ejeX, accionX] = "X";
                Enemigo++;
            }
            else
                tablero[ejeX, accionX] = "0"; 
        }

        static void MatrizEnemigo(string[,] tablero)
        {
            Console.WriteLine(""); 
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Tablero enemigo: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" \t");
            for (int x = 0; x < ejeX; x++)
            {
                Console.Write("{0}\t", x + 1);
            }
            Console.WriteLine();
            for (int y = 0; y < ejeY; y++)
            {
                Console.Write("{0}\t", y + 1);
                for (int h = 0; h < ejeX; h++)
                {
                    Console.Write("{0}\t", tablero[y, h]);
                }
                Console.WriteLine();
            }
        }

        static void disparar(string[,] formadetablero, string[,] Enemy, string fil, string col)
        {
            int EnemigoX = Convert.ToInt32(col) - 1; 
            int EnemigoY = Convert.ToInt32(fil) - 1;
            if (ejeY < EnemigoX || ejeX < EnemigoY)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Ese objetivo esta fuera de alcance");
                Thread.Sleep(2500);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (formadetablero[EnemigoX, EnemigoY] == "BARCO")
            {
                Enemy[EnemigoY, EnemigoX] = "X"; Player1++; ptsobtenidos += 100;
            }
            else 
            { 
                Enemy[EnemigoY, EnemigoX] = "0"; 
                puntosPerdidos += 50;
            }
        }
        static void showB(string[] matrizarray, string[,] enemigo, string[,] tablerito)
        {
            int puntos = 0;
            string[] Epuntos = new string[matrizarray.Length]; string[] puntosusuario = new string[matrizarray.Length];
            int sumar = 0;
            foreach (var chain in matrizarray)
            {
                string[] paquete = chain.Split(",");
                int inicio = Convert.ToInt32(paquete[0]) - 1; int final = Convert.ToInt32(paquete[3]);
                int ayuda = Convert.ToInt32(paquete[1]) - 1;
                if (paquete[2] == "H")
                {
                    for (int i = ayuda; i < ayuda + final; i++)
                    {
                        if (tablerito[inicio, i] == "X")
                        {
                            puntos++;
                        }

                    }
                }
                else if (paquete[2] == "V")
                {
                    for (int i = inicio; i < inicio + final; i++)
                    {
                        if (tablerito[i, ayuda] == "X")
                        {
                            puntos++;
                        }
                    }
                }
                Epuntos[sumar] =
                    paquete[0] + "," + paquete[1] + "," + paquete[2] + "," + puntos + "/" + paquete[3]; puntos = 0;
                sumar++;
            }

            puntos = 0;
            sumar = 0;
            foreach (var sucesion in matrizarray)
            {
                string[] save = sucesion.Split(",");
                int principio = Convert.ToInt32(save[0]) - 1; 
                int end = Convert.ToInt32(save[3]); 
                int help = Convert.ToInt32(save[1]) - 1;
                if (save[2] == "H")
                {
                    for (int x = help; x < help + end; x++)
                    {
                        if (enemigo[x, help] == "X")
                        {
                            puntos++;
                        }
                    }
                }
                else if (save[2] == "V")
                {
                    for (int i = principio; i < principio + end; i++)
                    {
                        if (tablerito[principio, i] == "X")
                        {
                            puntos++;
                        }
                    }
                }
                string clase = "";
                if (save[2] == "V")
                {
                    clase = "H";
                }
                else
                {
                    clase = "V";
                }
                puntosusuario[sumar] =
                    save[1] + "," + save[0] + "," + clase + "," + puntos + "/" + save[3]; puntos = 0; sumar++;
                }
            Console.WriteLine("\nLos puntos obtenidos son: " + ptsobtenidos);
            Console.WriteLine("\nLos puntos perdidos son: " + puntosPerdidos);
        }
    }
}