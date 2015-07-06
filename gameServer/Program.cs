using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace gameServer
{
    class Program
    {
        static Servidor lServidor = null;
        static Int32 nClientes = 1, giClienteAtual = 0;
        static readonly object block = new object();

        void ServidoRunLocal() 
        {
            Int32 ilClienteAtual = 0;

            lock (block)
            {
                ilClienteAtual = giClienteAtual;
            }

            while (true)
            {
                try
                {
                    lServidor.Run(ilClienteAtual);
                }
                catch
                {                    
                    Console.WriteLine("Encerrada a conexão com o cliente " + Convert.ToString(ilClienteAtual) + ".");                   
                    break;
                }
            }
        }

        static void Main(string[] args)
        {                       
            try
            {
                Console.Title = "Servidor do jogo:";
                Console.WriteLine(">> Informe a quantidade de jogadores: ");
                string stemp = Console.ReadLine();
                try
                {
                    nClientes = Convert.ToInt32(stemp);
                }
                catch
                {
                    nClientes = 1;
                }

                stemp = "";
                Console.WriteLine(">> Desejar conectar clientes localmente(1) ou na rede (2)?");
                stemp = Console.ReadLine();

                Console.WriteLine("Servidor iniciado, aguardando conexão com o(s) " + Convert.ToString(nClientes) + " cliente(s).");

                lServidor = new Servidor(nClientes, stemp);

                Console.WriteLine(">>");
                Console.WriteLine(">> Seu IP para conexão é: " + lServidor.sIpdoServidor);
                Console.WriteLine(">>");

                Console.WriteLine(">> Servidor de jogo iniciado");

                Console.WriteLine(">> Estabelecendo conexões com clientes.");

                for (int i = 0; i < nClientes; i++) {
                    lServidor.ServidorIniciar(i);
                }                

            }  //try
            catch (Exception ex)
            {
                lServidor = null;
                Console.WriteLine("Erro ao iniciar servidor: " + ex.Message);
            } //catch            

            if ((lServidor != null))
            {
               try
                  {
                     for (int i = 0; i < nClientes; i++)
                     {
                         giClienteAtual = i;

                         Program tt = new Program();
                         new Thread(tt.ServidoRunLocal).Start();

                         Thread.Sleep(2000);
                     } //for
               } //try
               catch (Exception ex)
               {
                  Console.WriteLine("Erro ao estabelecer conexão com os clientes: " + ex.Message);
               } //catch
            } //if
        }
    }
}