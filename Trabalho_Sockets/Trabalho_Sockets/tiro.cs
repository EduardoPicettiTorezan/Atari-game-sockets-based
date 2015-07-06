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
    public class tiro
    {
        public Texture2D modelo;
        public Vector2 posicaoatual = new Vector2(0, 0);
        public Vector2 posicaooriginal = new Vector2(0, 0);
        //mesma rotação do avião
        public float Rotacao = 1.56f;
        public Boolean ativo = false;
        public Boolean carregoumodelo = false;
        public Boolean desenhar = false;
        public Boolean movimentar = false;
        public Boolean disparou = false;
        public float ValorVelocidadeTiro = 6.0f;
        public Rectangle EspacoTiro;

        static public void Iniciar(ref tiro[] pTiro)
        {
            for (int i = 0; i < pTiro.Count(); i++)
            {
                if ((pTiro[i] == null))
                {
                    pTiro[i] = new tiro();
                    pTiro[i].ativo = true;
                }
            }
        }

        static public void CarregarModelo(aviao pAvioes, ref tiro[] pTiros, ContentManager pContent, Viewport pViewport)
        {
            for (int i = 0; i < pTiros.Count(); i++)
            {
                if ((pTiros[i].ativo == true) &&
                    (pTiros[i].carregoumodelo == false))
                {
                    pTiros[i].modelo = pContent.Load<Texture2D>("tiro");

                    pTiros[i].posicaooriginal.X = pTiros[i].modelo.Width / 2;
                    pTiros[i].posicaooriginal.Y = pTiros[i].modelo.Height / 2;
                    pTiros[i].posicaoatual.X = pViewport.Width / 2;
                    pTiros[i].posicaoatual.Y = pViewport.Height / 2;

                    pTiros[i].carregoumodelo = true;
                }
            }
        }

        static public void Movimentar(ref aviao[] pAvioes, Int32 pIndice, string sDirecao)
        {

            for (int i = pAvioes[pIndice].iQtdeTiros; i < pAvioes[pIndice].AviaoTiros.Count(); i++)
            {
                if ((pAvioes[pIndice].AviaoTiros[i].ativo == true) &&
                    (pAvioes[pIndice].AviaoTiros[i].movimentar == true))
                {
                    //Aplicar ângulo sobre a movimentação
                    if ((sDirecao == "F"))
                    {
                        pAvioes[pIndice].AviaoTiros[i].posicaoatual.Y = (pAvioes[pIndice].AviaoTiros[i].posicaoatual.Y -
                            (pAvioes[pIndice].AviaoTiros[i].ValorVelocidadeTiro * (float)Math.Cos(pAvioes[pIndice].AviaoTiros[i].Rotacao)));

                        pAvioes[pIndice].AviaoTiros[i].EspacoTiro.Y = (pAvioes[pIndice].AviaoTiros[i].EspacoTiro.Y -
                            Convert.ToInt32(pAvioes[pIndice].AviaoTiros[i].ValorVelocidadeTiro * (float)Math.Cos(pAvioes[pIndice].AviaoTiros[i].Rotacao)));

                        pAvioes[pIndice].AviaoTiros[i].posicaoatual.X = (pAvioes[pIndice].AviaoTiros[i].posicaoatual.X -
                            (pAvioes[pIndice].AviaoTiros[i].ValorVelocidadeTiro * (float)-Math.Sin(pAvioes[pIndice].AviaoTiros[i].Rotacao)));

                        pAvioes[pIndice].AviaoTiros[i].EspacoTiro.X = (pAvioes[pIndice].AviaoTiros[i].EspacoTiro.X -
                            Convert.ToInt32(pAvioes[pIndice].AviaoTiros[i].ValorVelocidadeTiro * (float)-Math.Sin(pAvioes[pIndice].AviaoTiros[i].Rotacao)));
                    }
                    else if ((sDirecao == "T"))
                    {
                        pAvioes[pIndice].AviaoTiros[i].posicaoatual.Y = (pAvioes[pIndice].AviaoTiros[i].posicaoatual.Y +
                            (pAvioes[pIndice].AviaoTiros[i].ValorVelocidadeTiro * (float)Math.Cos(pAvioes[pIndice].AviaoTiros[i].Rotacao)));

                        pAvioes[pIndice].AviaoTiros[i].EspacoTiro.Y = (pAvioes[pIndice].AviaoTiros[i].EspacoTiro.Y +
                            Convert.ToInt32(pAvioes[pIndice].AviaoTiros[i].ValorVelocidadeTiro * (float)Math.Cos(pAvioes[pIndice].AviaoTiros[i].Rotacao)));

                        pAvioes[pIndice].AviaoTiros[i].posicaoatual.X = (pAvioes[pIndice].AviaoTiros[i].posicaoatual.X +
                            (pAvioes[pIndice].AviaoTiros[i].ValorVelocidadeTiro * (float)-Math.Sin(pAvioes[pIndice].AviaoTiros[i].Rotacao)));

                        pAvioes[pIndice].AviaoTiros[i].EspacoTiro.X = (pAvioes[pIndice].AviaoTiros[i].EspacoTiro.X +
                            Convert.ToInt32(pAvioes[pIndice].AviaoTiros[i].ValorVelocidadeTiro * (float)-Math.Sin(pAvioes[pIndice].AviaoTiros[i].Rotacao)));
                    }

                    if ((pAvioes[pIndice].AviaoTiros[i].posicaoatual.X >= Principal.gciLimiteLargura) ||
                        (pAvioes[pIndice].AviaoTiros[i].posicaoatual.X + pAvioes[pIndice].AviaoTiros[i].modelo.Width <= 0) ||
                        (pAvioes[pIndice].AviaoTiros[i].posicaoatual.Y >= Principal.gciLimiteAltura) ||
                        (pAvioes[pIndice].AviaoTiros[i].posicaoatual.Y + pAvioes[pIndice].AviaoTiros[i].modelo.Height <= 0))
                        AtivarDesativarTiro(ref pAvioes[pIndice].AviaoTiros[i], false);
                    else
                    {
                        //Detectando se o tiro atingiu algum avião.
                        for (int iContaAviao = 0; iContaAviao < pAvioes.Count(); iContaAviao++)
                        {
                            if ((iContaAviao != pIndice) &&
                                (pAvioes[iContaAviao].morto == false)
                                )
                            {
                                if ((pAvioes[iContaAviao].EspacoAviao.Intersects(pAvioes[pIndice].AviaoTiros[i].EspacoTiro)))
                                {
                                    AtivarDesativarTiro(ref pAvioes[pIndice].AviaoTiros[i], false);
                                    aviao.ExplodirAviao(ref pAvioes[iContaAviao]);
                                }
                            }
                        }
                    }
                }
            }
        }

        static public void DesenharTiro(aviao pAviao, ref tiro[] pTiro, SpriteBatch pSpriteBatch)
        {
            for (int i = pAviao.iQtdeTiros; i < pTiro.Count(); i++)
            {
                if ((pTiro[i].ativo == true) &&
                    (pTiro[i].desenhar == true))
                {
                    pSpriteBatch.Draw(
                       pTiro[i].modelo,
                       pTiro[i].posicaoatual,
                       null,
                       Color.White,
                       pTiro[i].Rotacao,
                       pTiro[i].posicaooriginal,
                       1.0f,
                       SpriteEffects.None,
                       0f);
                }
            } //for
        }

        static public void Atirar(ref aviao pAviao) {
            TimeSpan Dif;

            Dif = DateTime.Now - pAviao.DataHoraUltimoTiro;

            if ((pAviao.morto == false))
            {
                if ((pAviao.iQtdeTiros > 0) &&
                   (Dif.Milliseconds >= 300.0f))
                {
                    AtivarDesativarTiro(ref pAviao.AviaoTiros[pAviao.iQtdeTiros - 1], true);

                    pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].posicaoatual = pAviao.posicaoatual;
                    //pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].posicaoatual.Y += (pAviao.modelo.Width / 2) - 5.0f;

                    pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].posicaooriginal = pAviao.posicaooriginal;
                    pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].posicaooriginal.X = pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].posicaooriginal.X / 4 - 2.0f;

                    pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].EspacoTiro = new Rectangle
                            (Convert.ToInt32(pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].posicaoatual.X),
                             Convert.ToInt32(pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].posicaoatual.Y),
                             Convert.ToInt32(pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].modelo.Width),
                             Convert.ToInt32(pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].modelo.Height));

                    pAviao.AviaoTiros[pAviao.iQtdeTiros - 1].Rotacao = pAviao.Rotacao;

                    pAviao.iQtdeTiros -= 1;
                    pAviao.DataHoraUltimoTiro = DateTime.Now;
                }
            }
        }

        static public void AtivarDesativarTiro(ref tiro pTiro, Boolean status)
        {
            pTiro.desenhar = status;
            pTiro.movimentar = status;
            pTiro.disparou = status;
        }
    }


}
