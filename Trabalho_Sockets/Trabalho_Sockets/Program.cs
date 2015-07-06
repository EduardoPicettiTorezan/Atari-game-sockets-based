using System;
using System.Windows.Forms;

namespace Trabalho_Sockets
{
#if WINDOWS || XBOX
    static class Program
    {
        public static string sProgramIpDoServidor = "";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Menu_Inicial serve = new Menu_Inicial())
            {
                if (serve.ShowDialog() == DialogResult.OK)
                {
                    sProgramIpDoServidor = Menu_Inicial.sIpdoServidor;
                    using (Principal game = new Principal())
                    {
                        game.Run();
                    }
                }
            }
        }
    }
#endif
}

