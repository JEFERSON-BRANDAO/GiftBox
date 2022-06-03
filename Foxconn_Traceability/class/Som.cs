using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;

namespace Foxconn_Traceability
{
    class Som
    {
        #region SOM AVISO

        public void Falha()
        {
            try
            {
                string caminho = AppDomain.CurrentDomain.BaseDirectory;
                SoundPlayer som = new SoundPlayer(caminho + "/SOM/fail.wav");
                som.Play();
            }
            catch
            {
                //
            }
        }
        //
        public void Aprovado()
        {
            try
            {
                string caminho = AppDomain.CurrentDomain.BaseDirectory;
                SoundPlayer som = new SoundPlayer(caminho + "/SOM/pass.wav");
                som.Play();
            }
            catch
            {
                //
            }
        }

        #endregion
    }
}
