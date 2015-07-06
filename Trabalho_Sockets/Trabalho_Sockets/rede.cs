using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trabalho_Sockets
{
    public class rede
    {

        public int IniciarCliente(ref cliente PCliente, int pIdCliente, string psIp)
        {
            int iNumCliente = 0;
            string sRespostaServidor = "";

            try
            {
                PCliente = new cliente(psIp);

                sRespostaServidor = TrocaDeMensagens(ref PCliente, "INICIAR=" + Convert.ToString(pIdCliente));

                iNumCliente = Convert.ToInt32(sRespostaServidor);

                return iNumCliente;
            }
            catch
            {
                PCliente = null; //encerra a conexão com o servidor.
                return 0;
            }

            
        } //IniciarCliente

        public string TrocaDeMensagens(ref cliente pCliente, string psMsg)
        {
            try
            {
                if ((pCliente != null))
                {
                    pCliente.EnviarMensagem(psMsg);
                    return pCliente.respostaServidor;
                }
                else
                    return "";
            }
            catch 
            {
                pCliente = null; //encerra a conexão com o servidor.
                return "";
            }
        } //TrocaDeMensagens

        public string TrocaDeMensagensNaoBloq(ref cliente pCliente, string psMsg)
        {
            try
            {
                if ((pCliente != null))
                {
                    pCliente.EnviarMensagemSemAguardarResposa(psMsg);
                    return "";
                }
                else
                    return "";
            }
            catch
            {
                pCliente = null; //encerra a conexão com o servidor.
                return "";
            }
        } //TrocaDeMensagens
    }
}
