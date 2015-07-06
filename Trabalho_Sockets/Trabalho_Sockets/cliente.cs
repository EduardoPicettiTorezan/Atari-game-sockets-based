using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Trabalho_Sockets
{
    public class cliente
    {
        public int iPORTA = 8000;
        public int iTAMANHO_BUFFER = 10000;
        public TcpClient tcp_cliente;
        public string mensagem;
        public string respostaServidor;

        public cliente(string hostname)
        {            
                try
                {
                    this.tcp_cliente = new TcpClient();
                    this.tcp_cliente.Connect(hostname, iPORTA);
                }
                catch
                {
                }
            
        }

        
        public void EnviarMensagem(string mensagem)
        {
            this.mensagem = mensagem;
            NetworkStream servidorStream = this.tcp_cliente.GetStream();
            //converte a mensagem para array de bytes
            byte[] saida = Encoding.ASCII.GetBytes(mensagem + "$");

            //envia a mensagem para o servidor
            servidorStream.Write(saida, 0, saida.Length); 
            servidorStream.Flush();
            byte[] entrada = new byte[iTAMANHO_BUFFER];


            //recebe o retorno da mensagem do servidor
            servidorStream.Read(entrada, 0, (int)this.tcp_cliente.ReceiveBufferSize);
            //converte a mensagem do servidor em uma string
            this.respostaServidor = Encoding.ASCII.GetString(entrada);
        }

        public void EnviarMensagemSemAguardarResposa(string mensagem)
        {
            this.mensagem = mensagem;
            NetworkStream servidorStream = this.tcp_cliente.GetStream();
            //converte a mensagem para array de bytes
            byte[] saida = Encoding.ASCII.GetBytes(mensagem + "$");

            //envia a mensagem para o servidor
            servidorStream.Write(saida, 0, saida.Length);
            servidorStream.Flush();            
        }
    }
}
