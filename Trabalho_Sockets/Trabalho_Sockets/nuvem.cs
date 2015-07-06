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
    public class nuvem
    {
        public Texture2D modelo;
        //Vetores com as posições dos objetos.        
        public Vector2 posicaoatual;
        public Boolean ativo = false;
        static private Random gVendoRnd = new Random(DateTime.Now.Millisecond);
        static private float fMovimentoNuvem = 0.09f;
        static private float fDirecaoVento;

        static public void Iniciar(ref nuvem[] pNuvens)
        {

            gVendoRnd.Next();
            fDirecaoVento = gVendoRnd.Next(1, 5);

            for (int i = 0; i < pNuvens.Count(); i++)
            {
                pNuvens[i] = new nuvem();                
                pNuvens[i].ativo = true;
            }
        }

        static public void PosicionarNuvens(ref nuvem[] pNuvens)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < pNuvens.Count(); i++)
            {
                if ((pNuvens[i].ativo == true))
                {
                    rnd.Next();
                    pNuvens[i].posicaoatual.X = rnd.Next(20, Principal.gciLimiteLargura - 20);
                    pNuvens[i].posicaoatual.Y = rnd.Next(20, Principal.gciLimiteAltura - 20);
                }
            }            
        }

        static public void Movimentar(ref nuvem[] pNuvens)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < pNuvens.Count(); i++)
            {
                if ((pNuvens[i].ativo == true))
                {
                    if ((fDirecaoVento == 1))
                        pNuvens[i].posicaoatual.X -= fMovimentoNuvem;

                    if ((fDirecaoVento == 2))
                        pNuvens[i].posicaoatual.X += fMovimentoNuvem;

                    if ((fDirecaoVento == 3))
                        pNuvens[i].posicaoatual.Y -= fMovimentoNuvem;

                    if ((fDirecaoVento == 4))
                        pNuvens[i].posicaoatual.Y += fMovimentoNuvem;

                    if ((pNuvens[i].posicaoatual.X >= Principal.gciLimiteLargura))
                        pNuvens[i].posicaoatual.X = 0 - pNuvens[i].modelo.Width;
                    else if ((pNuvens[i].posicaoatual.X + pNuvens[i].modelo.Width <= 0))
                        pNuvens[i].posicaoatual.X = Principal.gciLimiteLargura;

                    if ((pNuvens[i].posicaoatual.Y >= Principal.gciLimiteAltura))
                        pNuvens[i].posicaoatual.Y = 0 - pNuvens[i].modelo.Height;
                    else if ((pNuvens[i].posicaoatual.Y + pNuvens[i].modelo.Height <= 0))
                        pNuvens[i].posicaoatual.Y = Principal.gciLimiteAltura;                                        
                }
            }
        }

        static public void CarregarModelo(ref nuvem[] pNuvens, ContentManager pContent)
        {
            for (int i = 0; i < pNuvens.Count(); i++)
            {
                if ((pNuvens[i].ativo == true))
                {
                    pNuvens[i].modelo = pContent.Load<Texture2D>("nuvem");
                }
            }
        }

        static public void DesenharNuvem(ref nuvem[] pNuvens, SpriteBatch pSpriteBatch)
        {
            for (int i = 0; i < pNuvens.Count(); i++)
            {
                if ((pNuvens[i].ativo == true))
                {
                    pSpriteBatch.Draw(pNuvens[i].modelo, pNuvens[i].posicaoatual, Color.White);
                }
            } //for
        }

        
    }
}
