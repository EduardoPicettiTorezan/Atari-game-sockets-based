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
    public class aviao
    {
        private const Int32 ciQtdTiros = 200;
        private const Int32 ciQtdVidas = 3;
        private const float gcfTempoExplosao = 1.5f;

        //Texturas a serem desenhadas.
        public Texture2D modelo;
        public Texture2D modeloExplosao;
        public SpriteFont arialFont;
        public SpriteFont arialFont2;

        //Vetores com as posições dos objetos.
        public Vector2 posicaoatual = new Vector2(0, 0);
        public Vector2 posicaooriginal = new Vector2(0, 0);
        public Vector2 posicaoTexto = new Vector2(0, 0);
        public Vector2 posicaoTextoTop = new Vector2(0, 0);
        public Vector2 posicaoTextoMsg = new Vector2(0, 0);

        //mesma rotação dos tiros;
        public float Rotacao = 1.56f;
        public float fDirecaoRotacao = 0;
        public int Pontuacao = 0;
        public Boolean ativo = false;
        public Boolean morto = false;
        public float ValorRotacaoPadrao = 0.03f;
        public float ValorVelocidadePadrao = 2.0f;
        public tiro[] AviaoTiros = new tiro[ciQtdTiros];
        public Int16 iQtdeTiros = ciQtdTiros;
        public Int16 iQtdeVidas = ciQtdVidas;

        public DateTime DataHoraUltimoTiro;
        public DateTime DataHoraInicial;
        public DateTime DataHoraUltEnvio;
        public DateTime DataHoraUltRecebimeno;
        public DateTime DataHoraUltAtualizacaoTela;

        public Int32 EnviarTiro = 0;
        public Int32 ForcarTiro = 0;

        public Boolean EmMovimento = false;
        public Boolean Explodir = false;
        public Boolean Vencedor = false;
        public float fTempoExplosao = gcfTempoExplosao;
        public Rectangle EspacoAviao;
        public String NomeJogador = "";
        public Color CorAviao;
        public float UltimaAtRemota = 1.0f;
        public int IntervaloAtualRede = 0;
        public int IntervaloAtualTela = 0;

        static public void Iniciar(ref aviao[] pAvioes, Boolean pbForceAtivo)
        {
            DateTime Data = DateTime.Now;

            for (int i = 0; i < pAvioes.Count(); i++)
            {
                if ((pAvioes[i] == null))
                   pAvioes[i] = new aviao();
                pAvioes[i].ativo = ((pbForceAtivo));
                pAvioes[i].morto = false;
                pAvioes[i].EmMovimento = false;
                pAvioes[i].DataHoraUltimoTiro = Data;
                pAvioes[i].DataHoraInicial = Data;
                pAvioes[i].DataHoraUltEnvio = Data;
                pAvioes[i].DataHoraUltRecebimeno = Data;
                pAvioes[i].DataHoraUltAtualizacaoTela = Data;
                pAvioes[i].NomeJogador = ("Jogador " + Convert.ToString(i + 1));

                if ((i == 3))
                    pAvioes[i].CorAviao = Color.Pink;
                else if ((i == 2))
                    pAvioes[i].CorAviao = Color.Red;
                else if ((i == 1))
                    pAvioes[i].CorAviao = Color.Green;
                else
                    pAvioes[i].CorAviao = Color.Black;

                tiro.Iniciar(ref pAvioes[i].AviaoTiros);
                for (int j = 0; j < pAvioes[i].AviaoTiros.Count(); j ++) {
                    tiro.Iniciar(ref pAvioes[i].AviaoTiros);
                }
            }
        }

        static public void ZerarTempoTiros(ref aviao[] pAvioes)
        {
            DateTime Data = DateTime.Now;

            for (int i = 0; i < pAvioes.Count(); i++)
            {
                pAvioes[i].DataHoraUltimoTiro = pAvioes[i].DataHoraInicial;
            }
        }

        static public void PosicionarAvioes(ref aviao[] pAvioes, Viewport pViewport)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < pAvioes.Count(); i++)
            {
                Boolean bProximo = false;

                //if ((pAvioes[i].ativo == true))
                //{
                    //pAvioes[i].posicaoatual.X = 200;
                    //pAvioes[i].posicaoatual.Y = 200;
                    //pAvioes[i].posicaooriginal.X = pAvioes[i].modelo.Width / 2;
                    //pAvioes[i].posicaooriginal.Y = pAvioes[i].modelo.Height / 2;
                    bProximo = true;
                    while (bProximo)
                    {
                        rnd.Next();
                        pAvioes[i].posicaoatual.X = rnd.Next(70, Principal.gciLimiteLargura - 70);
                        rnd.Next();
                        pAvioes[i].posicaoatual.Y = rnd.Next(70, Principal.gciLimiteAltura - 70);
                        rnd.Next();
                        pAvioes[i].Rotacao = rnd.Next(1, 4) * pAvioes[i].Rotacao;

                        if ((i <= 0))
                            bProximo = false;
                        else //Não permite que aviões comecem muito pertos uns dos outros!
                        {
                            if (!(pAvioes[i].EspacoAviao.Intersects(pAvioes[i - 1].EspacoAviao)))
                                bProximo = false;
                        }
                    }
                //}
            }
        }

        static public void CarregarModelo(ref aviao[] pAvioes, ContentManager pContent, Viewport pViewport)
        {
            for (int i = 0; i < pAvioes.Count(); i++)
            {
                //if ((pAvioes[i].ativo == true))
                //{
                    pAvioes[i].modelo = pContent.Load<Texture2D>("aviao_p" + Convert.ToString(i + 1));
                    pAvioes[i].modeloExplosao = pContent.Load<Texture2D>("Explosion");
                    pAvioes[i].arialFont = pContent.Load<SpriteFont>("SpriteFont1");
                    pAvioes[i].arialFont2 = pContent.Load<SpriteFont>("SpriteFont2");

                    pAvioes[i].posicaooriginal.X = pAvioes[i].modelo.Width / 2;
                    pAvioes[i].posicaooriginal.Y = pAvioes[i].modelo.Height / 2;

                    //pAvioes[i].posicaoatual.X = pViewport.Width / 2;
                    //pAvioes[i].posicaoatual.Y = pViewport.Height / 2;

                    pAvioes[i].EspacoAviao = new Rectangle
                        (Convert.ToInt32(pAvioes[i].posicaoatual.X),
                         Convert.ToInt32(pAvioes[i].posicaoatual.Y),
                         Convert.ToInt32(pAvioes[i].modelo.Width),
                         Convert.ToInt32(pAvioes[i].modelo.Height));

                    pAvioes[i].EmMovimento = true;
                    MoverParaFrente(ref pAvioes[i]);
                //}
            }
        }

        static public Boolean GirarParaDireita(ref aviao pAviao)
        {
            float fTemp = 0;

            if ((pAviao.ativo == true))
            {
                pAviao.Rotacao = pAviao.Rotacao + pAviao.ValorRotacaoPadrao;
                pAviao.EmMovimento = true;
                fTemp = (MathHelper.Pi * 2) * -1.0f;
                pAviao.Rotacao = pAviao.Rotacao % fTemp;

                return true;
            }
            else
            {
                return false;
            }
        }

        static public Boolean GirarParaEsquerda(ref aviao pAviao)
        {
            float fTemp = 0;

            if ((pAviao.ativo == true))
            {
                pAviao.Rotacao = pAviao.Rotacao - pAviao.ValorRotacaoPadrao;
                pAviao.EmMovimento = true;
                fTemp = MathHelper.Pi * 2;
                pAviao.Rotacao = pAviao.Rotacao % fTemp;

                return true;
            }
            else
            {
                return false;
            }
        }

        static public Boolean MoverParaFrente(ref aviao pAviao)
        {
            aviao[] tmpAviao = new aviao[1];

            pAviao.EmMovimento = true;
            tmpAviao[0] = pAviao;

            aviao.Movimentar(ref tmpAviao, "F");

            pAviao = tmpAviao[0];

            return false;
        }

        static public Boolean MoverParaTraz(ref aviao pAviao)
        {
            aviao[] tmpAviao = new aviao[1];

            pAviao.EmMovimento = true;
            tmpAviao[0] = pAviao;

            aviao.Movimentar(ref tmpAviao, "T");

            pAviao = tmpAviao[0];

            return false;
        }

        static public void Movimentar(ref aviao[] pAvioes, string sDirecao)
        {

            for (int i = 0; i < pAvioes.Count(); i++)
            {
                if ((pAvioes[i].ativo == true) &&
                    (pAvioes[i].EmMovimento == true))
                {
                    try
                    {
                        //Aplicar ângulo sobre a movimentação
                        if ((sDirecao == "F"))
                        {
                            pAvioes[i].posicaoatual.Y = (pAvioes[i].posicaoatual.Y -
                                (pAvioes[i].ValorVelocidadePadrao * (float)Math.Cos(pAvioes[i].Rotacao)));

                            pAvioes[i].EspacoAviao.Y = (pAvioes[i].EspacoAviao.Y -
                                Convert.ToInt32(pAvioes[i].ValorVelocidadePadrao * (float)Math.Cos(pAvioes[i].Rotacao)));

                            pAvioes[i].posicaoatual.X = (pAvioes[i].posicaoatual.X -
                                (pAvioes[i].ValorVelocidadePadrao * (float)-Math.Sin(pAvioes[i].Rotacao)));

                            pAvioes[i].EspacoAviao.X = (pAvioes[i].EspacoAviao.X -
                                Convert.ToInt32(pAvioes[i].ValorVelocidadePadrao * (float)-Math.Sin(pAvioes[i].Rotacao)));
                        }
                        else if ((sDirecao == "T"))
                        {
                            pAvioes[i].posicaoatual.Y = (pAvioes[i].posicaoatual.Y +
                                (pAvioes[i].ValorVelocidadePadrao * (float)Math.Cos(pAvioes[i].Rotacao)));

                            pAvioes[i].EspacoAviao.Y = (pAvioes[i].EspacoAviao.Y +
                                Convert.ToInt32(pAvioes[i].ValorVelocidadePadrao * (float)Math.Cos(pAvioes[i].Rotacao)));

                            pAvioes[i].posicaoatual.X = (pAvioes[i].posicaoatual.X +
                                (pAvioes[i].ValorVelocidadePadrao * (float)-Math.Sin(pAvioes[i].Rotacao)));

                            pAvioes[i].EspacoAviao.X = (pAvioes[i].EspacoAviao.X +
                                Convert.ToInt32(pAvioes[i].ValorVelocidadePadrao * (float)-Math.Sin(pAvioes[i].Rotacao)));
                        }

                        pAvioes[i].fDirecaoRotacao = pAvioes[i].Rotacao;

                        if ((pAvioes[i].posicaoatual.X >= Principal.gciLimiteLargura))
                        {
                            pAvioes[i].posicaoatual.X = 0 - pAvioes[i].modelo.Width;
                            pAvioes[i].EspacoAviao.X = Convert.ToInt32(pAvioes[i].posicaoatual.X);
                        }
                        else if ((pAvioes[i].posicaoatual.X + pAvioes[i].modelo.Width <= 0))
                        {
                            pAvioes[i].posicaoatual.X = Principal.gciLimiteLargura;
                            pAvioes[i].EspacoAviao.X = Convert.ToInt32(pAvioes[i].posicaoatual.X);
                        }

                        if ((pAvioes[i].posicaoatual.Y >= Principal.gciLimiteAltura))
                        {
                            pAvioes[i].posicaoatual.Y = 0 - pAvioes[i].modelo.Height;
                            pAvioes[i].EspacoAviao.Y = Convert.ToInt32(pAvioes[i].posicaoatual.Y);
                        }
                        else if ((pAvioes[i].posicaoatual.Y + pAvioes[i].modelo.Height <= 0))
                        {
                            pAvioes[i].posicaoatual.Y = Principal.gciLimiteAltura;
                            pAvioes[i].EspacoAviao.Y = Convert.ToInt32(pAvioes[i].posicaoatual.Y);
                        }
                    }
                    finally
                    {
                        pAvioes[i].EmMovimento = false;
                    }
                }
            }
        }


        static public void DesenharAviao(ref jogador pJogador, SpriteBatch pSpriteBatch)
        {
            string sDadosJogador = "";

            if ((pJogador.bConexaoRemotaLiberada == true))
               VerifVencedor(ref pJogador.lAviao);

            for (int i = 0; i < pJogador.lAviao.Count(); i++)
                {
                    //if ((pJogador.lAviao[i].ativo == true))
                    //{
                        pJogador.lAviao[i].posicaoTextoMsg.X = (10);
                        pJogador.lAviao[i].posicaoTextoMsg.Y = (Principal.gciLimiteAltura - 60); ;
                        
                        if ((pJogador.bConexaoRemotaLiberada == false))
                        {
                            pSpriteBatch.DrawString(
                                       pJogador.lAviao[i].arialFont2,
                                       "    Iniciando conexao com servidor. Aguarde...",
                                       pJogador.lAviao[i].posicaoTextoMsg,
                                       pJogador.lAviao[i].CorAviao);
                            break;
                        }
                        else if ((pJogador.bPartidaIniciada == false))
                        {
                            pSpriteBatch.DrawString(
                               pJogador.lAviao[i].arialFont2,
                               "Conectado ao servidor. Aguardando os outros jogadores.",
                               pJogador.lAviao[i].posicaoTextoMsg,
                               pJogador.lAviao[i].CorAviao);
                            break;
                        }
                        else
                        {
                            if ((pJogador.lAviao[i].Explodir))
                            {
                                pSpriteBatch.Draw(
                                   pJogador.lAviao[i].modeloExplosao,
                                   pJogador.lAviao[i].posicaoatual,
                                   null,
                                   Color.White,
                                   pJogador.lAviao[i].Rotacao,
                                   pJogador.lAviao[i].posicaooriginal,
                                   1.0f,
                                   SpriteEffects.None,
                                   0f);

                                pJogador.lAviao[i].fTempoExplosao -= 0.1f;

                                if ((pJogador.lAviao[i].fTempoExplosao <= 0.0f))
                                {
                                    pJogador.lAviao[i].Explodir = false;
                                    pJogador.lAviao[i].fTempoExplosao = gcfTempoExplosao;

                                    if ((pJogador.lAviao[i].iQtdeVidas <= 0))
                                    {
                                        pJogador.lAviao[i].morto = true;
                                        if ((pJogador.redeClienteJogador != null)) {
                                            pJogador.redeClienteJogador.TrocaDeMensagens(
                                            ref pJogador.clienteJogador, "MORREU=" + Convert.ToString(i));
                                        }
                                    }
                                }
                            } //if
                            else
                            {
                                if ((pJogador.lAviao[i].morto == false) &&
                                    (pJogador.lAviao[i].ativo))
                                {
                                    pSpriteBatch.Draw(
                                       pJogador.lAviao[i].modelo,
                                       pJogador.lAviao[i].posicaoatual,
                                       null,
                                       Color.White,
                                       pJogador.lAviao[i].Rotacao,
                                       pJogador.lAviao[i].posicaooriginal,
                                       1.0f,
                                       SpriteEffects.None,
                                       0f);
                                } //if
                            } //else
                        } //else

                        if ((pJogador.bPartidaIniciada) &&
                            (pJogador.lAviao[i].ativo))
                        {
                            //Escrevendo dados do avião
                            pJogador.lAviao[i].posicaoTexto = pJogador.lAviao[i].posicaoatual;
                            pJogador.lAviao[i].posicaoTextoTop.X = (Convert.ToInt32(Principal.gciLimiteLargura / pJogador.lAviao.Count()) * (i));
                            pJogador.lAviao[i].posicaoTextoTop.Y = 0;

                            pJogador.lAviao[i].posicaoTexto.Y -= pJogador.lAviao[i].modelo.Height;
                            pJogador.lAviao[i].posicaoTexto.X -= pJogador.lAviao[i].modelo.Width - 20.0f;

                            if ((pJogador.lAviao[i].morto))
                                sDadosJogador = ("MORTO!");
                            else
                                sDadosJogador = ("Vidas: " + Convert.ToString(pJogador.lAviao[i].iQtdeVidas));

                            sDadosJogador = pJogador.lAviao[i].NomeJogador + " - " + sDadosJogador;
                        }

                        //texto do avião
                        if ((pJogador.clienteJogador == null))
                        {
                            pSpriteBatch.DrawString(
                                pJogador.lAviao[i].arialFont2,
                                "      Nao foi possivel conectar ao servidor.",
                                pJogador.lAviao[i].posicaoTextoMsg,
                                pJogador.lAviao[i].CorAviao);
                            break;
                        } //if
                        else if ((pJogador.bPartidaIniciada == true))
                        {

                            if ((pJogador.lAviao[i].morto == false) &&
                                (pJogador.lAviao[i].ativo))
                            {
                                pSpriteBatch.DrawString(
                                   pJogador.lAviao[i].arialFont,
                                   (pJogador.lAviao[i].NomeJogador),
                                   pJogador.lAviao[i].posicaoTexto,
                                   pJogador.lAviao[i].CorAviao);
                            }

                            if ((i == pJogador.IdRemotoJogador))
                            {

                                if ((pJogador.lAviao[i].Vencedor))
                                {
                                    pSpriteBatch.DrawString(
                                       pJogador.lAviao[i].arialFont2,
                                       "                Jogador " + Convert.ToString(i + 1) + " ganhou!",
                                       pJogador.lAviao[i].posicaoTextoMsg,
                                       pJogador.lAviao[i].CorAviao);
                                    break;
                                }

                                if ((pJogador.lAviao[i].morto == true))
                                {
                                    pSpriteBatch.DrawString(
                                            pJogador.lAviao[i].arialFont2,
                                            "                Jogador " + Convert.ToString(i + 1) + " perdeu!",
                                            pJogador.lAviao[i].posicaoTextoMsg,
                                            pJogador.lAviao[i].CorAviao);
                                    break;
                                } //if
                            } //if
                        } //else if

                        //texto superior
                        if ((pJogador.lAviao[i].ativo))
                        {
                            pSpriteBatch.DrawString(
                                pJogador.lAviao[i].arialFont,
                                sDadosJogador,
                                pJogador.lAviao[i].posicaoTextoTop,
                                pJogador.lAviao[i].CorAviao);
                        }

                    //} //if
                
                } //for

        }

        static public void ExplodirAviao(ref aviao pAviao)
        {            
           if ((pAviao.ativo == true))
            {
                if ((pAviao.iQtdeVidas > 0))
                    pAviao.iQtdeVidas -= 1;

                pAviao.Explodir = true;
            } //for
        }

        static public void VerifVencedor(ref aviao[] pAvioes)
        {

            Int32 icv = 0, icm = 0, iv = 0, icativs = 0;
            for (int i = 0; i < pAvioes.Count(); i++)
            {
                if ((pAvioes[i].ativo == true))
                {
                    icativs++;
                    if ((pAvioes[i].morto == true))                    
                        icm = icm + 1;                   
                    else
                    {
                        icv = icv + 1;
                        iv = i;
                    }
                }
            }

            if ((icv == 1) && (icativs != 1))           
                pAvioes[iv].Vencedor = true;           
        }

    }
}
