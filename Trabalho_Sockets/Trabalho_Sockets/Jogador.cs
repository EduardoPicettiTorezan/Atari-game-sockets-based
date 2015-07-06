using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Trabalho_Sockets
{
    public class jogador
    {
        public Boolean bJogadorCliente = false;
        public Boolean bConexaoRemotaLiberada = false;
        public Boolean bPartidaIniciada = false;
        public rede redeClienteJogador;
        public cliente clienteJogador;

        public Int32 IdLocalJogador = -1;
        public Int32 IdRemotoJogador = -1;

        public aviao[] lAviao = new aviao[4];
        public nuvem[] lNuvem = new nuvem[10];
        public int NumeroDoAviaoDoJogador = -1;
        public int iQtdAvAtivos = -1;
        public string sJogIpDoServidor = "";

        static public void Iniciar(ref jogador[] pJogadores, string psIp)
        {
            for (int i = 0; i < pJogadores.Count(); i++)
            {
                pJogadores[i] = new jogador();
                pJogadores[i].sJogIpDoServidor = psIp;
                pJogadores[i].NumeroDoAviaoDoJogador = 1;

                pJogadores[i].bJogadorCliente = true;
                pJogadores[i].redeClienteJogador = new rede();                

                aviao.Iniciar(ref pJogadores[i].lAviao, false);
                nuvem.Iniciar(ref pJogadores[i].lNuvem);
               
            } //for

        } //Inicar

        static public void ConectarServidor(ref jogador[] pJogadores)
        {
            for (int i = 0; i < pJogadores.Count(); i++)
            {
                try
                {
                    Random rnd = new Random(DateTime.Now.Millisecond);

                    rnd.Next();

                    pJogadores[i].IdLocalJogador = rnd.Next(1, 50000);

                    pJogadores[i].IdRemotoJogador =
                        pJogadores[i].redeClienteJogador.IniciarCliente(
                            ref pJogadores[i].clienteJogador,
                            pJogadores[i].IdLocalJogador,
                            pJogadores[i].sJogIpDoServidor);

                    //Ativando apenas o avião o local, os demais serão ativados pela confirmação de sua conexão
                    //pela rede
                    for (int jk = 0; jk < pJogadores[i].lAviao.Count(); jk++)
                    {
                        if ((jk == pJogadores[i].IdRemotoJogador) &&
                            (pJogadores[i].IdRemotoJogador >= 0))
                            pJogadores[i].lAviao[jk].ativo = true;
                        else
                            pJogadores[i].lAviao[jk].ativo = false;
                    } //for

                    pJogadores[i].bConexaoRemotaLiberada = true;
                    pJogadores[i].bPartidaIniciada = false;
                } //try
                catch
                {
                    pJogadores[i].bConexaoRemotaLiberada = false;
                    pJogadores[i].bPartidaIniciada = false;
                } //catch

            } //for
        } //ConectarServidor

        public static void SetarCliente(ref jogador[] pJogadores)
        {
            for (int i = 0; i < pJogadores.Count(); i++)
            {
                pJogadores[i].bJogadorCliente = true;
            }
        }        

        public static void jogadorLoadContent(ref jogador[] pJogadores, Viewport pViewport, ContentManager pContent)
        {
            for (int i = 0; i < pJogadores.Count(); i++)
            {
                nuvem.PosicionarNuvens(ref pJogadores[i].lNuvem);

                aviao.PosicionarAvioes(ref pJogadores[i].lAviao, pViewport);
                SetarPosicaoAviaoAtual(ref pJogadores, i);

                nuvem.CarregarModelo(ref pJogadores[i].lNuvem, pContent);
                aviao.CarregarModelo(ref pJogadores[i].lAviao, pContent, pViewport);

                for (Int32 it = 0; it < pJogadores[i].lAviao.Count(); it++)
                {
                    tiro.CarregarModelo(pJogadores[i].lAviao[it], ref pJogadores[i].lAviao[it].AviaoTiros, pContent, pViewport);
                }
            }
        }

        public static void GetPosOutrosAvioes(ref jogador[] pJogadores,int i)
        {
            try
            {

                if ((pJogadores[i].clienteJogador != null))
                {
                    //Pega do servidor a posição atual de todos os clientes, exceto ele próprio
                    if ((pJogadores[i].iQtdAvAtivos < 0))
                    {
                        pJogadores[i].iQtdAvAtivos = Convert.ToInt32(
                            pJogadores[i].redeClienteJogador.TrocaDeMensagens(
                               ref pJogadores[i].clienteJogador, "QTDATIVOS?"));
                    }

                    if ((pJogadores[i].clienteJogador != null))
                    {
                        for (int j = 0; j < pJogadores[i].iQtdAvAtivos; j++)
                        {

                            if ((j != pJogadores[i].IdRemotoJogador))
                            {

                                TimeSpan Dif;
                                Dif = DateTime.Now - pJogadores[i].lAviao[j].DataHoraUltRecebimeno;

                                if ((pJogadores[i].lAviao[j].DataHoraUltRecebimeno == null) ||
                                    (Dif.Milliseconds > pJogadores[i].lAviao[j].IntervaloAtualRede))
                                {
                                    pJogadores[i].lAviao[j].DataHoraUltRecebimeno = DateTime.Now;

                                    string sRetPos = "", su = "", sx = "", sy = "", sr = "", sa = "";
                                    Double u = 0, x = 0, y = 0, r = 0;
                                    Int32 a = 0;

                                    sRetPos = pJogadores[i].redeClienteJogador.TrocaDeMensagens(
                                       ref pJogadores[i].clienteJogador, "GPOSICAO=" + Convert.ToString(j));
                                    if ((pJogadores[i].clienteJogador != null))
                                    {
                                        if ((sRetPos.Length >= 6) &&
                                            (sRetPos.Substring(0, 6) == "MORREU"))
                                        {
                                            pJogadores[i].lAviao[j].morto = true;
                                            pJogadores[i].lAviao[j].iQtdeVidas = -1;
                                        }
                                        else
                                        {
                                            if ((sRetPos.Length > 0))
                                            {
                                                for (int k = 0; k < sRetPos.Length; k++)
                                                {
                                                    su = su + sRetPos[k];
                                                    if ((sRetPos[k] == '|'))
                                                    {
                                                        sRetPos = sRetPos.Replace(su, "");
                                                        su = su.Replace("U=", "");
                                                        su = su.Replace("|", "");
                                                        break;
                                                    }
                                                } //for

                                                try
                                                {
                                                    u = Convert.ToDouble(su);
                                                } //try
                                                catch
                                                {
                                                    u = -1;
                                                } //catch

                                                if ((u > 0) &&
                                                    (u != pJogadores[i].lAviao[j].UltimaAtRemota))
                                                {
                                                    pJogadores[i].lAviao[j].UltimaAtRemota = (float)u;

                                                    for (int k = 0; k < sRetPos.Length; k++)
                                                    {
                                                        sx = sx + sRetPos[k];
                                                        if ((sRetPos[k] == '|'))
                                                        {
                                                            sRetPos = sRetPos.Replace(sx, "");
                                                            sx = sx.Replace("X=", "");
                                                            sx = sx.Replace("|", "");
                                                            break;
                                                        }
                                                    } //for

                                                    for (int k = 0; k < sRetPos.Length; k++)
                                                    {
                                                        sy = sy + sRetPos[k];
                                                        if ((sRetPos[k] == '|'))
                                                        {
                                                            sRetPos = sRetPos.Replace(sy, "");
                                                            sy = sy.Replace("Y=", "");
                                                            sy = sy.Replace("|", "");
                                                            break;
                                                        }
                                                    } //for

                                                    for (int k = 0; k < sRetPos.Length; k++)
                                                    {
                                                        sr = sr + sRetPos[k];
                                                        if ((sRetPos[k] == '|'))
                                                        {
                                                            sRetPos = sRetPos.Replace(sr, "");
                                                            sr = sr.Replace("R=", "");
                                                            sr = sr.Replace("|", "");
                                                            break;
                                                        }
                                                    } //for

                                                    for (int k = 0; k < sRetPos.Length; k++)
                                                    {
                                                        sa = sa + sRetPos[k];
                                                        if ((sRetPos[k] == '|'))
                                                        {
                                                            sRetPos = sRetPos.Replace(sa, "");
                                                            sa = sa.Replace("A=", "");
                                                            sa = sa.Replace("|", "");
                                                            break;
                                                        }
                                                    } //for

                                                    try
                                                    {
                                                        x = Convert.ToDouble(sx);
                                                        y = Convert.ToDouble(sy);
                                                        r = Convert.ToDouble(sr);
                                                        a = Convert.ToInt32(sa);

                                                        pJogadores[i].lAviao[j].ativo = true;
                                                        pJogadores[i].lAviao[j].Rotacao = (float)r;
                                                        pJogadores[i].lAviao[j].fDirecaoRotacao = (float)r;
                                                        pJogadores[i].lAviao[j].posicaoatual.Y = (float)y;
                                                        pJogadores[i].lAviao[j].posicaoatual.X = (float)x;
                                                        pJogadores[i].lAviao[j].EspacoAviao.Y = Convert.ToInt32(y);
                                                        pJogadores[i].lAviao[j].EspacoAviao.X = Convert.ToInt32(x);

                                                        if ((a > 0))
                                                        {
                                                            //pJogadores[i].lAviao[j].ForcarTiro = 1;
                                                            tiro.Atirar(ref pJogadores[i].lAviao[j]);
                                                        }
                                                    } //try
                                                    catch
                                                    {
                                                    } //catch
                                                } //if                   
                                            } //if
                                        } //else
                                    } //if
                                } //if
                            }
                        } //for
                    } //if != null
                } //if != null
            } //try
            catch
            {
            }
        }

        public static void jogadorUpdate(ref jogador[] pJogadores, KeyboardState teclado_estado)
        {
            Boolean bConectarServidor = false;

                

                for (int i = 0; i < pJogadores.Count(); i++)
                {
                    if ((pJogadores[i].bConexaoRemotaLiberada == true))
                    {
                        if ((pJogadores[i].bPartidaIniciada == false))
                        {
                            for (int ixad = 0; ixad < pJogadores.Count(); ixad++)
                            {
                                if ((pJogadores[ixad].clienteJogador != null))
                                {
                                    string sTemp = "";

                                    sTemp = pJogadores[ixad].redeClienteJogador.TrocaDeMensagens(
                                            ref pJogadores[ixad].clienteJogador,
                                            @"PRONTOPARAJOGO=" + Convert.ToString(pJogadores[ixad].IdRemotoJogador));
                                    if ((sTemp.Length >= 6) &&
                                        (sTemp.Substring(0, 6) == "FIGHT!"))
                                    {
                                        pJogadores[ixad].bPartidaIniciada = true;
                                    } //if
                                } //if
                            } //for
                        }


                        if ((pJogadores[i].bPartidaIniciada == true))
                        {
                            TimeSpan DifTela;
                            DifTela = DateTime.Now - pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].DataHoraUltAtualizacaoTela;

                            if ((DifTela.Milliseconds >= pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].IntervaloAtualTela))
                            {
                                pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].DataHoraUltAtualizacaoTela = DateTime.Now;

                                Boolean bMovimentar = false;
                                Boolean bAlterouPos = false;

                                if ((teclado_estado.IsKeyDown(Keys.Right)))
                                {
                                    bAlterouPos = true;
                                    bMovimentar = aviao.GirarParaDireita(ref pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador]);
                                }

                                if ((teclado_estado.IsKeyDown(Keys.Left)))
                                {
                                    bAlterouPos = true;
                                    bMovimentar = aviao.GirarParaEsquerda(ref pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador]);
                                }

                                if ((teclado_estado.IsKeyDown(Keys.Right)))
                                {
                                    bAlterouPos = true;
                                    bMovimentar = aviao.GirarParaDireita(ref pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador]);
                                }

                                if ((teclado_estado.IsKeyDown(Keys.Up)))
                                {
                                    bAlterouPos = true;
                                    bMovimentar = aviao.MoverParaFrente(ref pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador]);
                                }

                                if ((teclado_estado.IsKeyDown(Keys.Down)))
                                {
                                    bAlterouPos = true;
                                    bMovimentar = aviao.MoverParaTraz(ref pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador]);
                                }

                                if ((teclado_estado.IsKeyDown(Keys.Space)))
                                {
                                    bAlterouPos = true;
                                    //Força a atualização.
                                    pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].DataHoraUltEnvio = pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].DataHoraInicial;
                                    pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].EnviarTiro = 1;
                                    tiro.Atirar(ref pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador]);
                                }

                                if ((bMovimentar))
                                {
                                    aviao.Movimentar(ref pJogadores[i].lAviao, "F");
                                }

                                for (Int32 it = 0; it < pJogadores[i].lAviao.Count(); it++)
                                {
                                    tiro.Movimentar(ref pJogadores[i].lAviao, it, "F");
                                }
                                nuvem.Movimentar(ref pJogadores[i].lNuvem);

                                //Mwahahhahhaha
                                if ((bAlterouPos == true) ||
                                    (pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].UltimaAtRemota <= 1.0f))
                                {
                                    SetarPosicaoAviaoAtual(ref pJogadores, i); //Este SEMPRE antes.
                                }
                                GetPosOutrosAvioes(ref pJogadores, i);
                            }
                        } //else
                    } //if
                    else
                    {
                        TimeSpan Dif;
                        Dif = DateTime.Now - pJogadores[i].lAviao[0].DataHoraUltRecebimeno;

                        if ((Dif.Seconds > 5))
                        {
                            bConectarServidor = true;
                            aviao.Iniciar(ref pJogadores[i].lAviao, true);

                        }
                    } //else
                } //for

                if ((bConectarServidor))
                {
                    ConectarServidor(ref pJogadores);
                } //if
      
        }

        public static void SetarPosicaoAviaoAtual(ref jogador[] pJogadores, int i)
        {
            string sTemp = "";
            TimeSpan Dif;

            try
            {
                if ((pJogadores[i].clienteJogador != null))
                {
                    Dif = DateTime.Now - pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].DataHoraUltEnvio;

                    if ((pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].DataHoraUltEnvio == null) ||
                        (Dif.Milliseconds > pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].IntervaloAtualRede))
                    {
                        pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].DataHoraUltEnvio = DateTime.Now;

                        Random rnd = new Random(DateTime.Now.Millisecond);
                        rnd.Next();
                        pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].UltimaAtRemota = rnd.Next(1, 500000);

                        //Avisa ao servidor qual é a posicao atual do avião do cliente.
                        sTemp = pJogadores[i].redeClienteJogador.TrocaDeMensagens(
                                ref pJogadores[i].clienteJogador,
                                @"SPOSICAO="
                                       + Convert.ToString(pJogadores[i].IdRemotoJogador) +
                                  "=>" +
                                  "U=" + Convert.ToString(pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].UltimaAtRemota) + "|" +
                                  "X=" + Convert.ToString(pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].posicaoatual.X) + "|" +
                                  "Y=" + Convert.ToString(pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].posicaoatual.Y) + "|" +
                                  "R=" + Convert.ToString(pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].Rotacao) + "|" +
                                  "A=" + Convert.ToString(pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].EnviarTiro) + "|"
                                  );
                        pJogadores[i].lAviao[pJogadores[i].IdRemotoJogador].EnviarTiro = 0;
                    } //if
                } //if
            } //try
            catch
            {
            } //

        }


        public static void jogadorDraw(ref jogador[] pJogadores, SpriteBatch pSpriteBatch) {

            for (int i = 0; i < pJogadores.Count(); i++)
            {
                nuvem.DesenharNuvem(ref pJogadores[i].lNuvem, pSpriteBatch);


                if ((pJogadores[i].bPartidaIniciada) &&
                    (pJogadores[i].bConexaoRemotaLiberada))
                {
                    for (Int32 ia = 0; ia < pJogadores[i].lAviao.Count(); ia++)
                    {
                        tiro.DesenharTiro(pJogadores[i].lAviao[ia], ref pJogadores[i].lAviao[ia].AviaoTiros, pSpriteBatch);
                    }
                }
                aviao.DesenharAviao(ref pJogadores[i], pSpriteBatch);
                
            }
        }
    }
}
