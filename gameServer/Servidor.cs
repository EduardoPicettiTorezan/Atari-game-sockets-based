using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace gameServer
{
    public class Servidor
    {
        static readonly object block = new object();

        private const string posicaoPadrao = "U=2.0X=1.0|Y=1.0|R=1.0|A=0";
        //porta de comunicação do socket
        public const int PORTA = 8000;
        //tamanho máximo da mensagem recebida do cliente
        public const int TAMANHO_BUFFER = 10000;
        //número de requisições do cliente (mensagem que o cliente mandou)
        public int[] requisicoes = new int[4];
        //Socket do servidor
        public TcpListener servidor;
        public string sIpdoServidor = "";

        //Socket do cliente
        public TcpClient[] cliente = new TcpClient[4];

        public Int32 iQtdClientes = 0;
        public string[] posicoesCliente = new string[4];
        public Boolean[] clientesProntos = new Boolean[4];
        public Boolean[] clientesMortos = new Boolean[4];

        //mensagem que o cliente manda para o servidor
        public string[] mensagemCliente = new string[4];
        //mensagem que o servidor manda ao cliente
        public string[] respostaServidor = new string[4];
        public Int32[] IdLocaldoCliente = new Int32[4];

        public string GetLocalIP()
        {
            string _IP = null;

            // Resolves a host name or IP address to an IPHostEntry instance.
            // IPHostEntry - Provides a container class for Internet host address information. 
            System.Net.IPHostEntry _IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            // IPAddress class contains the address of a computer on an IP network. 
            foreach (System.Net.IPAddress _IPAddress in _IPHostEntry.AddressList)
            {
                // InterNetwork indicates that an IP version 4 address is expected 
                // when a Socket connects to an endpoint
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    _IP = _IPAddress.ToString();
                }
            }
            return _IP;
        }

        public Servidor(int iQtdClientes, string psModo)
        {
            try
            {
                this.iQtdClientes = iQtdClientes;

                if ((psModo == "2"))
                {
                    string localComputerName = Dns.GetHostName();
                    IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                    IPAddress ipTemp = null;

                    ipTemp = IPAddress.Parse(GetLocalIP());

                    if ((ipTemp != null))
                    {
                        this.servidor = new TcpListener(ipTemp, PORTA);
                        this.sIpdoServidor = Convert.ToString(ipTemp.ToString());
                    }

                    if ((this.sIpdoServidor == ""))
                    {
                        Console.WriteLine(">> ATENÇÃO! ");
                        Console.WriteLine(">> ATENÇÃO! Não foi possível identificar um IP válido na rede.");
                        Console.WriteLine(">> ATENÇÃO! ");
                        this.servidor = new TcpListener(IPAddress.Loopback, PORTA);
                        this.sIpdoServidor = Convert.ToString(IPAddress.Loopback.ToString());
                    }
                }
                else
                {
                    this.servidor = new TcpListener(IPAddress.Loopback, PORTA);
                    this.sIpdoServidor = Convert.ToString(IPAddress.Loopback.ToString());
                }
                

                for (int i = 0; i < iQtdClientes; i++)
                {
                    this.cliente[i] = default(TcpClient);
                    this.requisicoes[i] = -1;
                }

                this.servidor.Start();

            } //try
            catch
            {
            } //catch

        }

        public void ServidorIniciar(int iCliente)
        {
            try
            {                
                //Bloqueante
                this.cliente[iCliente] = servidor.AcceptTcpClient();
                this.requisicoes[iCliente] = 0;
                this.respostaServidor[iCliente] = "";
            } //try
            catch
            {
            } //catch
        } //IniciarServidor
      

        public void Run(int i)
        {
            try
            {
                if (i > -1)
                {
                    if ((this.requisicoes[i] <= 0))
                       this.requisicoes[i]++;                    

                    NetworkStream netStream = cliente[i].GetStream();

                    byte[] recebido = new byte[TAMANHO_BUFFER];
                    //recebe a mensagem do cliente
                    netStream.Read(recebido, 0, (int)cliente[i].ReceiveBufferSize);
                    //converte bytes em string
                    this.mensagemCliente[i] = Encoding.ASCII.GetString(recebido);
                    this.mensagemCliente[i] = this.mensagemCliente[i].Substring(0, this.mensagemCliente[i].IndexOf("$"));

                    TratarMensagemCliente(i);

                    Byte[] enviado = Encoding.ASCII.GetBytes(this.respostaServidor[i]);
                    //envia a resposta em bytes ao cliente
                    netStream.Write(enviado, 0, enviado.Length);
                    netStream.Flush();
                } //if
            } //try
            catch
            {

            } //catch
        }  // Run     

        private void TratarMensagemCliente(int i) {

            this.respostaServidor[i] = "";

            try
            {

                if ((this.mensagemCliente[i].Length >= 8) &&
                    (this.mensagemCliente[i].Substring(0, 8) == "INICIAR="))
                {
                    string sIdNumeroRemoto = "";

                    sIdNumeroRemoto = this.mensagemCliente[i].Replace("INICIAR=", "");

                    Console.WriteLine(">> Iniciada conexão com o cliente remoto " + Convert.ToString(i) + ", seu ID local é " + sIdNumeroRemoto);

                    try
                    {
                        lock (block)
                        {
                            this.IdLocaldoCliente[i] = Convert.ToInt32(sIdNumeroRemoto);
                        }
                    }
                    catch
                    {
                    }

                    //Devolve ao cliente sua posição na lista de clientes ativos:
                    this.respostaServidor[i] = Convert.ToString(i);
                }
                else if ((this.mensagemCliente[i].Length >= 6) &&
                         (this.mensagemCliente[i].Substring(0, 6) == "MORREU"))
                {
                    Int32 iNumCliente = -1;
                    string sGetCliente = "";
                    this.respostaServidor[i] = "OK";
                    sGetCliente = this.mensagemCliente[i].Replace("MORREU=", "");

                    if ((sGetCliente.Length > 0))
                    {
                        try
                        {
                            iNumCliente = Convert.ToInt32(sGetCliente);
                        }
                        catch
                        {
                            iNumCliente = -1;
                        }

                        if ((iNumCliente >= 0))
                        {
                            lock (block)
                            {
                                this.clientesMortos[iNumCliente] = true;
                                Console.WriteLine("Cliente " + Convert.ToString(iNumCliente) + " morreu.");
                            }
                        } //if
                    } //if

                }
                else if ((this.mensagemCliente[i].Length >= 14) &&
                         (this.mensagemCliente[i].Substring(0, 14) == "PRONTOPARAJOGO"))
                {
                    Int32 iNumCliente = -1;
                    string sGetCliente = "";
                    this.respostaServidor[i] = "NO-FIGHT";
                    sGetCliente = this.mensagemCliente[i].Replace("PRONTOPARAJOGO=", "");

                    if ((sGetCliente.Length > 0)) {
                        try
                        {
                            iNumCliente = Convert.ToInt32(sGetCliente);
                        }
                        catch
                        {
                            iNumCliente = -1;
                        }

                        if ((iNumCliente >= 0))
                        {

                            lock (block)
                            {
                                this.clientesProntos[iNumCliente] = true;
                                int iContaAtivos = 0;
                                for (int ica = 0; ica < this.iQtdClientes; ica++)
                                {
                                    if ((this.clientesProntos[ica] == true))
                                    {
                                        iContaAtivos++;
                                    }
                                } //for

                                if ((iContaAtivos >= this.iQtdClientes))
                                {
                                    this.respostaServidor[i] = "FIGHT!";
                                } //if
                            }
                        } //if
                    } //if

                }
                else if ((this.mensagemCliente[i].Length >= 10) &&
                         (this.mensagemCliente[i].Substring(0, 10) == "QTDATIVOS?"))
                {
                    //Console.WriteLine(">> Respondendo quantidade de jogadores ativos ao cliente: " + Convert.ToString(i));
                    this.respostaServidor[i] = Convert.ToString(this.iQtdClientes);
                }
                else if ((this.mensagemCliente[i].Length >= 8) &&
                         (this.mensagemCliente[i].Substring(0, 8) == "GPOSICAO"))
                {
                    string sSetPosicao = "";

                    sSetPosicao = this.mensagemCliente[i];
                    sSetPosicao = sSetPosicao.Replace("GPOSICAO=", "");
                    this.respostaServidor[i] = posicaoPadrao;

                    if ((sSetPosicao.Length > 0))
                    {
                        int iNumJogador = -1;
                        if ((sSetPosicao.Length > 0))
                        {
                            try
                            {
                                iNumJogador = Convert.ToInt32(sSetPosicao);
                            }
                            catch
                            {
                                iNumJogador = -1;
                            }
                        }
                        if ((iNumJogador > -1))
                        {
                            if ((this.clientesMortos[iNumJogador]))
                                this.respostaServidor[i] = "MORREU";
                            else
                            {
                                if ((this.posicoesCliente[iNumJogador] != null))
                                    this.respostaServidor[i] = Convert.ToString(this.posicoesCliente[iNumJogador]);
                            }
                        }
                    }

                }
                else if ((this.mensagemCliente[i].Length >= 8) &&
                         (this.mensagemCliente[i].Substring(0, 8) == "SPOSICAO"))
                {
                    string sSetPosicao = "";

                    sSetPosicao = this.mensagemCliente[i];
                    sSetPosicao = sSetPosicao.Replace("SPOSICAO=", "");

                    if ((sSetPosicao.Length >= 1))
                    {
                        int iNumJogador = -1;
                        try
                        {
                            iNumJogador = Convert.ToInt32(sSetPosicao.Substring(0, 1));
                        }
                        catch
                        {
                            iNumJogador = -1;
                        }

                        sSetPosicao = sSetPosicao.Replace((Convert.ToString(iNumJogador) + "=>"), "");
                        if ((iNumJogador > -1) &&
                            (sSetPosicao.Length > 0))
                        {
                            try
                            {
                                if ((this.posicoesCliente[iNumJogador] == null) ||
                                    (this.posicoesCliente[iNumJogador].Trim() == ""))
                                    this.posicoesCliente[iNumJogador] = posicaoPadrao;

                                if ((sSetPosicao != null))
                                    this.posicoesCliente[iNumJogador] = Convert.ToString(sSetPosicao);
                            }
                            catch { }
                        }
                    }

                    //Console.WriteLine(">> Recebendo posicoes de cliente");
                    this.respostaServidor[i] = "1";
                }


                if ((this.respostaServidor[i].Trim() == ""))
                {
                    //manda para o cliente a mensagem recebida
                    this.respostaServidor[i] = this.mensagemCliente[i];
                }
            }
            catch
            {
                this.respostaServidor[i] = "";
            }

        } //TratarMensagemCliente

        public static bool IsLocalIpAddress(string host)
        {
            try
            { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }
    }
}
