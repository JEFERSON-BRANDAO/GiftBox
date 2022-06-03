using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using LabelManager2;
using System.Threading;
using System.Diagnostics;

namespace Foxconn_Traceability
{
    class Print
    {
        public class RawPrinterHelper
        {
            #region IMPRIMIR

            // Structure and API declarions:
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public class DOCINFOA
            {
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDocName;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pOutputFile;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDataType;
            }

            //
            [DllImport("shell32.dll", EntryPoint = "ShellExecute")]
            public static extern int ShellExecuteA(int hwnd, string lpOperation,
                  string lpFile, string lpParameters, string lpDirectory, int nShowCmd);


            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

            [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndDocPrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

            // SendBytesToPrinter()
            // When the function is given a printer name and an unmanaged array
            // of bytes, the function sends those bytes to the print queue.
            // Returns true on success, false on failure.
            public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
            {
                Int32 dwError = 0, dwWritten = 0;
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();
                bool bSuccess = false; // Assume failure unless you specifically succeed.

                di.pDocName = "Etiqueta SN Document";
                di.pDataType = "RAW";

                // Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    // Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        // Start a page.
                        if (StartPagePrinter(hPrinter))
                        {
                            // Write your bytes.
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
                // If you did not succeed, GetLastError may give more information
                // about why not.
                if (bSuccess == false)
                {
                    dwError = Marshal.GetLastWin32Error();
                }
                return bSuccess;
            }
            //
            public static bool SendFileToPrinter(string szPrinterName, string szFileName)
            {
                // Open the file.
                FileStream fs = new FileStream(szFileName, FileMode.Open);
                // Create a BinaryReader on the file.
                BinaryReader br = new BinaryReader(fs);
                // Dim an array of bytes big enough to hold the file's contents.
                Byte[] bytes = new Byte[fs.Length];
                bool bSuccess = false;
                // Your unmanaged pointer.
                IntPtr pUnmanagedBytes = new IntPtr(0);
                int nLength;

                nLength = Convert.ToInt32(fs.Length);
                // Read the contents of the file into the array.
                bytes = br.ReadBytes(nLength);
                // Allocate some unmanaged memory for those bytes.
                pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
                // Copy the managed byte array into the unmanaged array.
                Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
                // Send the unmanaged bytes to the printer.
                bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
                // Free the unmanaged memory that you allocated earlier.
                Marshal.FreeCoTaskMem(pUnmanagedBytes);

                //fs.Flush();//
                fs.Close();//Para não dar erro: The process cannot access the file 'C:\IMPRESSAO\etiqueta.prn' because it is being used by another process.
                return bSuccess;
            }
            //
            public static bool SendStringToPrinter(string szPrinterName, string szString)
            {
                IntPtr pBytes;
                Int32 dwCount;
                // How many characters are in the string?
                dwCount = szString.Length;
                // Assume that the printer is expecting ANSI text, and then convert
                // the string to ANSI text.
                pBytes = Marshal.StringToCoTaskMemAnsi(szString);
                // Send the converted ANSI string to the printer.
                SendBytesToPrinter(szPrinterName, pBytes, dwCount);
                Marshal.FreeCoTaskMem(pBytes);
                return true;
            }

            #endregion
        }

        public void Imprimir(string serial, string modelo, int quantidade, int totalImpresso, bool etiquetaDupla, bool UsarImpressoraPadrao, string Impressora)
        {
            #region GERA ETIQUETA

            string local = AppDomain.CurrentDomain.BaseDirectory + @"\IMPRESSAO\";
            string pasfilename = "etiqueta.prn";
            // 
            if (!Directory.Exists(local))//cria a pasta IMPRESSAO no diretório do programa ..\Foxconn_Traceability\bin\Debug\ caso não exista
            {
                Directory.CreateDirectory(local);
            }

            //           
            StringBuilder sb = new StringBuilder();

            if (modelo.Equals("TG1692BR") || modelo.Equals("TG1692A"))
            {
                //Impressora ZEBRA
                #region CÓDIGO  ZPL

                //
                sb.AppendLine("CT~~CD,~CC^~CT~");
                sb.AppendLine("^XA");

                //TEXTO ACIMA DO CÓDIGO DE BARRAS
                sb.AppendLine(@"^FO45,28^BY3^A0,20,0^FDSN:" + serial + "^FS");//MARGEM ESQUERDA =FO45; TOPO=28; FONT=20; 

                //CÓDIGO DE BARRAS            
                sb.AppendLine("^BY2,2,50^FT45,96^BCN,Y,N,N"); //MARGEM ESQUERDA =FT45; ALTURA CÓDIGO BARRAS=50 TOPO=96; CÓDIGO 128 = BCN
                sb.AppendLine("^FD>:" + serial + "^FS");//VALOR DO CÓDIGO DE BARRAS
                sb.AppendLine("^PQ1,0,1,Y^XZ");

                #endregion

            }
            else if (modelo.Equals("TG1692BR-SMT"))
            {
                //Impressora ZEBRA
                #region CÓDIGO  ZPL

                if (etiquetaDupla)
                {
                    #region ETIQUETA DUPLA

                    sb.AppendLine("CT~~CD,~CC^~CT~");
                    sb.AppendLine("^XA");
                    //sb.AppendLine("^XA");
                    //sb.AppendLine("^PW1200");
                    //sb.AppendLine("^LLO100");
                    //sb.AppendLine("^LS0");

                    //
                    string ladoEsquerdo = string.Empty;
                    string ladoDireito = string.Empty;
                    //
                    int proximo = 0;
                    for (int index = 0; index < serial.Length; index++)
                    {
                        if (serial[index].ToString().Equals(";"))
                            proximo++;
                        //
                        if (proximo == 0)
                        {
                            //etiqueta lado esquerdo
                            ladoEsquerdo += serial[index].ToString();
                        }
                        else
                        {
                            if (serial[index].ToString() != (";"))
                                //etiqueta lado direito
                                ladoDireito += serial[index].ToString();
                        }

                    }
                    // 
                    #region ETIQUETA LADO ESQUERDO
                    sb.AppendLine("^FT-10,0^BY1");//MARGEM ESQUERDA =FT-10;TOPO=0; LARGURA CÓDIGO BARRAS = BY1;
                    //^BCo,h,f,g,e,m (o=orientation; h=bar code height in dots; f=print interpretation line; g=print interpretation line above code; e =UCC check digit; m=mode )
                    sb.AppendLine("^BCN,30,N,N,N,N");//TIPO CÓDIGO BARRAS 128 = BCN outros BAN,B2N; ALTURA CÓDIGO BARRAS=30
                    sb.AppendLine("^FD" + ladoEsquerdo + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    //TEXTO ABAIXO DO CÓDIGO DE BARRAS
                    //ladoEsquerdo = ladoEsquerdo + "         " + ordem;//9 espaços
                    sb.AppendLine(@"^FO-10,-32^BY2^A0,22,0^FD" + ladoEsquerdo + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;


                    ////sb.AppendLine("^BY1,3.0,30^FT-10,0^BCN,N,N,N"); //LARGURA CÓDIGO BARRAS = BY1; MARGEM ESQUERDA =FT-10; ESPAÇO ENTRE BARRAS CÓDIGO BARRAS=3.0; ALTURA CÓDIGO BARRAS=30 TOPO=0; TIPO CÓDIGO BARRAS 128 = BCN outro BAN; B2N
                    //sb.AppendLine("^FD" + ladoEsquerdo + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    ////TEXTO ABAIXO DO CÓDIGO DE BARRAS
                    ////ladoEsquerdo = ladoEsquerdo + "         " + ordem;//9 espaços
                    //sb.AppendLine(@"^FO-10,-32^BY2^A0,22,0^FD" + ladoEsquerdo + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;  

                    #endregion
                    //
                    #region ETIQUETA LADO DIREITO
                    sb.AppendLine("^FT-385,0^BY1");//MARGEM ESQUERDA =FT-10;TOPO=0; LARGURA CÓDIGO BARRAS = BY1;
                    //^BCo,h,f,g,e,m (o=orientation; h=bar code height in dots; f=print interpretation line; g=print interpretation line above code; e =UCC check digit; m=mode ) 
                    sb.AppendLine("^BCN,30,N,N,N,N");//TIPO CÓDIGO BARRAS 128 = BCN outros BAN,B2N; ALTURA CÓDIGO BARRAS=30
                    sb.AppendLine("^FD" + ladoDireito + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    //TEXTO ABAIXO DO CÓDIGO DE BARRAS
                    //ladoDireito = ladoDireito + "          " + ordem;//9 espaços
                    sb.AppendLine(@"^FO-385,-32^BY2^A0,22,0^FD" + ladoDireito + "^FS");//MARGEM ESQUERDA =FO-385; TOPO=-32; FONT=22; 


                    ////sb.AppendLine("^BY1,3.0,30^FT-385,0^BCN,N,N,N"); //LARGURA CÓDIGO BARRAS = BY1; MARGEM ESQUERDA =FT-385; ESPAÇO ENTRE BARRAS CÓDIGO BARRAS=3.0; ALTURA CÓDIGO BARRAS=30 TOPO=0; TIPO CÓDIGO 128 = BCN outro BAN ;B2N
                    //sb.AppendLine("^FD" + ladoDireito + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    ////TEXTO ABAIXO DO CÓDIGO DE BARRAS
                    ////ladoDireito = ladoDireito + "          " + ordem;//9 espaços
                    //sb.AppendLine(@"^FO-385,-32^BY2^A0,22,0^FD" + ladoDireito + "^FS");//MARGEM ESQUERDA =FO-385; TOPO=-32; FONT=22; 

                    #endregion
                    //
                    sb.AppendLine("^PQ1,0,1,Y^XZ");

                    #endregion
                }
                else
                {
                    #region ETIQUETA SIMPLES

                    sb.AppendLine("CT~~CD,~CC^~CT~");
                    sb.AppendLine("^XA");
                    //sb.AppendLine("^PW350");//LARGURA IMPRESSAO 700
                    //sb.AppendLine("^LLO800");
                    //sb.AppendLine("^LS0");
                    // 
                    //sb.AppendLine("^MNY");//PAPEL NÃO CONTINUO
                    //sb.AppendLine("^PW400");//LARGURA IMPRESSAO 800
                    //sb.AppendLine("^LLO200");


                    #region ETIQUETA LADO ÚNICO

                    sb.AppendLine("^FO-10,0^BY1");//MARGEM ESQUERDA =FO-10;TOPO=0; LARGURA CÓDIGO BARRAS = BY1;
                    //^BCo,h,f,g,e,m (o=orientation; h=bar code height in dots; f=print interpretation line; g=print interpretation line above code; e =UCC check digit; m=mode )
                    sb.AppendLine("^BCN,30,N,N,N,A");//TIPO CÓDIGO BARRAS 128 = BCN outros BAN,B2N; ALTURA CÓDIGO BARRAS=30
                    sb.AppendLine("^FD" + serial + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    //TEXTO ABAIXO DO CÓDIGO DE BARRAS                    
                    sb.AppendLine(@"^FO-10,-32^BY2^A0,22,0^FD" + serial + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;  


                    #endregion
                    //
                    sb.AppendLine("^PQ1,0,1,Y^XZ");

                    #endregion
                }

                #endregion

            }
            else if (modelo.Equals("TG1692A-SMT"))
            {
                //Impressora ZEBRA
                #region CÓDIGO  ZPL

                if (etiquetaDupla)
                {
                    #region ETIQUETA DUPLA

                    sb.AppendLine("CT~~CD,~CC^~CT~");
                    sb.AppendLine("^XA");
                    //
                    string ladoEsquerdo = string.Empty;
                    string ladoDireito = string.Empty;
                    //
                    int proximo = 0;
                    for (int index = 0; index < serial.Length; index++)
                    {
                        if (serial[index].ToString().Equals(";"))
                            proximo++;
                        //
                        if (proximo == 0)
                        {
                            //etiqueta lado esquerdo
                            ladoEsquerdo += serial[index].ToString();
                        }
                        else
                        {
                            if (serial[index].ToString() != (";"))
                                //etiqueta lado direito
                                ladoDireito += serial[index].ToString();
                        }

                    }
                    //
                    //ordem = ordem.Remove(0, 8);//somente os 4 últimos digitos

                    #region ETIQUETA LADO ESQUERDO

                    sb.AppendLine("^FT-10,0^BY1");//MARGEM ESQUERDA =FT-10;TOPO=0; LARGURA CÓDIGO BARRAS = BY1;
                    //^BCo,h,f,g,e,m (o=orientation; h=bar code height in dots; f=print interpretation line; g=print interpretation line above code; e =UCC check digit; m=mode )
                    sb.AppendLine("^BCN,30,N,N,N,N");//TIPO CÓDIGO BARRAS 128 = BCN outros BAN,B2N; ALTURA CÓDIGO BARRAS=30
                    sb.AppendLine("^FD" + ladoEsquerdo + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    //TEXTO ABAIXO DO CÓDIGO DE BARRAS
                    //ladoEsquerdo = ladoEsquerdo + "         " + ordem;//9 espaços
                    sb.AppendLine(@"^FO-10,-32^BY2^A0,22,0^FD" + ladoEsquerdo + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;


                    //sb.AppendLine("^BY1,1,30^FT-10,0^B3N,N,N,N"); //LARGURA CÓDIGO BARRAS = BY1; MARGEM ESQUERDA =FT-10; ESPACO ENTRE BARRAS COD BARRAS=1; ALTURA CÓDIGO BARRAS=30 TOPO=0; TIPO CÓDIGO 128 = BCN  outro BAN ;B2N
                    //sb.AppendLine("^FD" + ladoEsquerdo + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    ////TEXTO ABAIXO DO CÓDIGO DE BARRAS
                    ////ladoEsquerdo = ladoEsquerdo + "         " + ordem;//9 espaços
                    //sb.AppendLine(@"^FO-10,-32^BY2^A0,22,0^FD" + ladoEsquerdo + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;  

                    #endregion
                    //
                    #region ETIQUETA LADO DIREITO
                    sb.AppendLine("^FT-385,0^BY1");//MARGEM ESQUERDA =FT-10;TOPO=0; LARGURA CÓDIGO BARRAS = BY1;
                    //^BCo,h,f,g,e,m (o=orientation; h=bar code height in dots; f=print interpretation line; g=print interpretation line above code; e =UCC check digit; m=mode ) 
                    sb.AppendLine("^BCN,30,N,N,N,N");//TIPO CÓDIGO BARRAS 128 = BCN outros BAN,B2N; ALTURA CÓDIGO BARRAS=30
                    sb.AppendLine("^FD" + ladoDireito + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    //TEXTO ABAIXO DO CÓDIGO DE BARRAS
                    //ladoDireito = ladoDireito + "          " + ordem;//9 espaços
                    sb.AppendLine(@"^FO-385,-32^BY2^A0,22,0^FD" + ladoDireito + "^FS");//MARGEM ESQUERDA =FO-385; TOPO=-32; FONT=22; 


                    //sb.AppendLine("^BY1,1,30^FT-385,0^B3N,N,N,N"); //LARGURA CÓDIGO BARRAS = BY1; MARGEM ESQUERDA =FT-385; ESPACO ENTRE BARRAS COD BARRAS=1; ALTURA CÓDIGO BARRAS=30 TOPO=0; TIPO CÓDIGO 128 = BCN  outro BAN ;B2N
                    //sb.AppendLine("^FD" + ladoDireito + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    ////TEXTO ABAIXO DO CÓDIGO DE BARRAS
                    ////ladoDireito = ladoDireito + "          " + ordem;//9 espaços
                    //sb.AppendLine(@"^FO-385,-32^BY2^A0,22,0^FD" + ladoDireito + "^FS");//MARGEM ESQUERDA =FO-385; TOPO=-32; FONT=22; 

                    #endregion
                    //
                    sb.AppendLine("^PQ1,0,1,Y^XZ");

                    #endregion
                }
                else
                {
                    #region ETIQUETA SIMPLES

                    sb.AppendLine("CT~~CD,~CC^~CT~");
                    sb.AppendLine("^XA");
                    // 
                    #region ETIQUETA LADO ÚNICO
                    sb.AppendLine("^FT-10,0^BY1");//MARGEM ESQUERDA =FT-10;TOPO=0; LARGURA CÓDIGO BARRAS = BY1;
                    //^BCo,h,f,g,e,m (o=orientation; h=bar code height in dots; f=print interpretation line; g=print interpretation line above code; e =UCC check digit; m=mode )
                    sb.AppendLine("^BCN,30,N,N,N,N");//TIPO CÓDIGO BARRAS 128 = BCN outros BAN,B2N; ALTURA CÓDIGO BARRAS=30
                    sb.AppendLine("^FD" + serial + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                    //TEXTO ABAIXO DO CÓDIGO DE BARRAS
                    //ladoEsquerdo = ladoEsquerdo + "         " + ordem;//9 espaços
                    sb.AppendLine(@"^FO-10,-32^BY2^A0,22,0^FD" + serial + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;
                    #endregion
                    //                    
                    sb.AppendLine("^PQ1,0,1,Y^XZ");

                    #endregion
                }

                #endregion

            }
            else if (modelo.Equals("E965"))
            {
                //Impressora ZEBRA
                #region CÓDIGO  ZPL

                sb.AppendLine("CT~~CD,~CC^~CT~");
                sb.AppendLine("^XA");

                //
                sb.AppendLine("^BY1,1,30^FT-60,-35^BCN,N,N,N,N"); //TIPO MENOE COD BARRAS = BY1; MARGEM ESQUERDA =FT-60; ESPACO ENTRE BARRAS COD BARRAS=1; ALTURA CÓDIGO BARRAS=30 TOPO=-35; CÓDIGO 128 = BCN  outro BAN ;B2N=largura codigo barras
                sb.AppendLine("^FD" + serial + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                //TEXTO ABAIXO DO CÓDIGO DE BARRAS
                sb.AppendLine(@"^FO-105,-38^BY1^A0,12,0^FD" + serial + "^FS");//MARGEM ESQUERDA =FO-105; TOPO=-38; FONT=12; 
                //
                sb.AppendLine("^XZ");

                #endregion

            }

            //
            byte[] data = new ASCIIEncoding().GetBytes(sb.ToString());
            FileStream fs = new FileStream(local + pasfilename, FileMode.OpenOrCreate);
            //
            fs.Write(data, 0, data.Length);
            //fs.Flush();
            fs.Close();

            //
            PrintDialog pd = new PrintDialog();
            pd.AllowSelection = false;
            //string nome_Impressora = pd.PrinterSettings.PrinterName;
            //
            pd.AllowPrintToFile = true;
            pd.Reset();
            pd.PrintToFile = true;

            //Envia o arquivo 'etiqueta.prn' para impressora
            RawPrinterHelper.SendFileToPrinter(Impressora, local + pasfilename);

            //
            GC.Collect();
            fs.Dispose();

            //Deleta o arquivo 'etiqueta.prn'
            if (System.IO.File.Exists(local + pasfilename))
                System.IO.File.Delete(local + pasfilename);

            #endregion
        }

        public void Reimprimir(string serial, string modelo, bool UsarImpressoraPadrao, string Impressora)
        {
            #region GERA ETIQUETA

            string local = AppDomain.CurrentDomain.BaseDirectory + @"\REIMPRESSAO\";
            string pasfilename = "etiqueta.prn";
            // 
            if (!Directory.Exists(local))//cria a pasta REIMPRESSAO no diretorio do programa ..\Foxconn_Traceability\bin\Debug\ caso não exista
            {
                Directory.CreateDirectory(local);
            }
            //           
            StringBuilder sb = new StringBuilder();

            if (modelo.Equals("TG1692BR") || modelo.Equals("TG1692A"))
            {
                //Impressora ZEBRA
                #region CÓDIGO  ZPL

                //
                sb.AppendLine("CT~~CD,~CC^~CT~");
                sb.AppendLine("^XA");

                //TEXTO ACIMA DO CÓDIGO DE BARRAS
                sb.AppendLine(@"^FO45,28^BY3^A0,20,0^FDSN:" + serial + "^FS");//MARGEM ESQUERDA =FO45; TOPO=28; FONT=20; 

                //CÓDIGO DE BARRAS            
                sb.AppendLine("^BY2,2,50^FT45,96^BCN,Y,N,N"); //MARGEM ESQUERDA =FT45; ALTURA CÓDIGO BARRAS=50 TOPO=96; CÓDIGO 128 = BCN
                sb.AppendLine("^FD>:" + serial + "^FS");//VALOR DO CÓDIGO DE BARRAS
                sb.AppendLine("^PQ1,0,1,Y^XZ");

                #endregion

            }
            else if (modelo.Equals("TG1692BR-SMT"))
            {
                //Impressora ZEBRA
                #region CÓDIGO  ZPL

                sb.AppendLine("CT~~CD,~CC^~CT~");
                sb.AppendLine("^XA");
                //
                string ladoEsquerdo = string.Empty;
                ladoEsquerdo = serial;
                //
                #region ETIQUETA LADO ESQUERDO
                sb.AppendLine("^FT-10,0^BY1");//MARGEM ESQUERDA =FT-10;TOPO=0; LARGURA CÓDIGO BARRAS = BY1;
                //^BCo,h,f,g,e,m (o=orientation; h=bar code height in dots; f=print interpretation line; g=print interpretation line above code; e =UCC check digit; m=mode )
                sb.AppendLine("^BCN,30,N,N,N,N");//TIPO CÓDIGO BARRAS 128 = BCN outros BAN,B2N; ALTURA CÓDIGO BARRAS=30
                sb.AppendLine("^FD" + ladoEsquerdo + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                //TEXTO ABAIXO DO CÓDIGO DE BARRAS
                //ladoEsquerdo = ladoEsquerdo + "         " + ordem;//9 espaços
                sb.AppendLine(@"^FO-10,-32^BY2^A0,22,0^FD" + ladoEsquerdo + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;


                //sb.AppendLine("^BY1,1,30^FT-10,0^B3N,N,N,N"); //TIPO MODELO COD BARRAS = BY1; MARGEM ESQUERDA =FT-10; ESPACO ENTRE BARRAS COD BARRAS=1; ALTURA CÓDIGO BARRAS=30 TOPO=0; CÓDIGO 128 = BCN  outro BAN ;B2N=largura codigo barras
                //sb.AppendLine("^FD" + ladoEsquerdo + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                ////TEXTO ABAIXO DO CÓDIGO DE BARRAS
                ////ladoEsquerdo = ladoEsquerdo + "         " + ordem;//9 espaços
                //sb.AppendLine(@"^FO-10,-32^BY3^A0,22,0^FD" + ladoEsquerdo + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;  

                #endregion
                //
                sb.AppendLine("^PQ1,0,1,Y^XZ");

                #endregion

            }
            else if (modelo.Equals("TG1692A-SMT"))
            {
                //Impressora ZEBRA
                #region CÓDIGO  ZPL

                sb.AppendLine("CT~~CD,~CC^~CT~");
                sb.AppendLine("^XA");

                //
                string ladoEsquerdo = string.Empty;
                ladoEsquerdo = serial;
                //
                #region ETIQUETA LADO ESQUERDO
                sb.AppendLine("^FT-10,0^BY1");//MARGEM ESQUERDA =FT-10;TOPO=0; LARGURA CÓDIGO BARRAS = BY1;
                //^BCo,h,f,g,e,m (o=orientation; h=bar code height in dots; f=print interpretation line; g=print interpretation line above code; e =UCC check digit; m=mode )
                sb.AppendLine("^BCN,30,N,N,N,N");//TIPO CÓDIGO BARRAS 128 = BCN outros BAN,B2N; ALTURA CÓDIGO BARRAS=30
                sb.AppendLine("^FD" + ladoEsquerdo + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                //TEXTO ABAIXO DO CÓDIGO DE BARRAS
                //ladoEsquerdo = ladoEsquerdo + "         " + ordem;//9 espaços
                sb.AppendLine(@"^FO-10,-32^BY2^A0,22,0^FD" + ladoEsquerdo + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;


                //sb.AppendLine("^BY1,1,30^FT-10,0^B3N,N,N,N"); //TIPO MODELO COD BARRAS = BY1; MARGEM ESQUERDA =FT-10; ESPACO ENTRE BARRAS COD BARRAS=1; ALTURA CÓDIGO BARRAS=30 TOPO=0; CÓDIGO 128 = BCN  outro BAN ;B2N=largura codigo barras
                //sb.AppendLine("^FD" + ladoEsquerdo + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                ////TEXTO ABAIXO DO CÓDIGO DE BARRAS
                ////ladoEsquerdo = ladoEsquerdo + "         " + ordem;//9 espaços
                //sb.AppendLine(@"^FO-10,-32^BY3^A0,22,0^FD" + ladoEsquerdo + "^FS");//MARGEM ESQUERDA=FO-10; TOPO=-32; FONT=22;  

                #endregion

                //
                sb.AppendLine("^PQ1,0,1,Y^XZ");

                #endregion

            }
            else if (modelo.Equals("E965"))
            {
                //Impressora ZEBRA
                #region CÓDIGO  ZPL

                //
                sb.AppendLine("CT~~CD,~CC^~CT~");
                sb.AppendLine("^XA");
                //
                sb.AppendLine("^BY1,1,30^FT-60,-35^BCN,N,N,N,N"); //TIPO MENOE COD BARRAS = BY1; MARGEM ESQUERDA =FT-60; ESPACO ENTRE BARRAS COD BARRAS=1; ALTURA CÓDIGO BARRAS=30 TOPO=-35; CÓDIGO 128 = BCN  outro BAN ;B2N=largura codigo barras
                sb.AppendLine("^FD" + serial + "^FS");//VALOR DO CÓDIGO DE BARRAS SE USAR ^FD`>` TIRA PRIMEIRA LETRA DO VALOR
                //TEXTO ABAIXO DO CÓDIGO DE BARRAS
                sb.AppendLine(@"^FO-105,-38^BY1^A0,12,0^FD" + serial + "^FS");//MARGEM ESQUERDA =FO-105; TOPO=-38; FONT=12; 
                sb.AppendLine("^PQ1,0,1,Y^XZ");

                #endregion
            }
            //
            byte[] data = new ASCIIEncoding().GetBytes(sb.ToString());
            FileStream fs = new FileStream(local + pasfilename, FileMode.OpenOrCreate);
            //
            fs.Write(data, 0, data.Length);
            //fs.Flush();// 
            fs.Close();
            //            
            PrintDialog pd = new PrintDialog();
            pd.AllowSelection = false;
            //string nome_Impressora = pd.PrinterSettings.PrinterName;
            //
            pd.AllowPrintToFile = true;
            pd.Reset();
            pd.PrintToFile = true;

            //Envia o arquivo 'etiqueta.prn' para impressora
            RawPrinterHelper.SendFileToPrinter(Impressora, local + pasfilename);
            //
            GC.Collect();
            fs.Dispose();

            //Deleta o arquivo 'etiqueta.prn'
            if (System.IO.File.Exists(local + pasfilename))
                System.IO.File.Delete(local + pasfilename);


            #endregion
        }

        public void Etiqueta_CodeSoft(List<String> Lista_Serial, string modelo, int quantidade, int totalImpresso, bool etiquetaDupla, string Impressora, string snHorizontal, string snVertical, string Wo)
        {
            string arquivoCodeSoft = string.Empty;
            string serial = string.Empty;
            int lista_row_Count = Lista_Serial.Count;
            int row_count_total_String = Lista_Serial[0].Count();
            //
            if (lista_row_Count > 0)
            {
                string ladoEsquerdo = string.Empty;
                string ladoDireito = string.Empty;
                //
                if (etiquetaDupla)
                {
                    #region ETIQUETA DUPLA

                    int proximo = 0;
                    LabelManager2.ApplicationClass lbl = null;
                    //
                    if ((modelo.Equals("TG1692A-SMT")) || (modelo.Equals("TG1692BR-SMT")) || (modelo.Equals("ROKU-MAC")) || (modelo.Equals("ROKU-PANEL")) || (modelo.Equals("ROKU-SMT")))
                    {
                        for (int row = 0; row < quantidade; row++)//quantidade vezes para imprimir
                        {
                            serial = Lista_Serial[row].ToString();//sequencia dupla
                            //extraindo dividindo lado esquerdo e direito
                            for (int index = 0; index < row_count_total_String; index++)
                            {
                                if (serial[index].ToString().Equals(";"))
                                    proximo++;
                                //
                                if (proximo == 0)
                                {
                                    //etiqueta lado esquerdo
                                    ladoEsquerdo += serial[index].ToString();
                                }
                                else
                                {
                                    if (serial[index].ToString() != (";"))
                                        //etiqueta lado direito
                                        ladoDireito += serial[index].ToString();
                                }
                                //
                                if (index == (row_count_total_String - 1))//encerra leitura da linha da lista
                                {
                                    #region NOME ARQUIVO CODESOFT

                                    if (modelo.Equals("TG1692A-SMT"))
                                    {
                                        arquivoCodeSoft = "TG_SMT_DUPLA.lab";
                                    }
                                    else if (modelo.Equals("TG1692BR-SMT"))
                                    {
                                        arquivoCodeSoft = "TGBR_SMT_DUPLA.lab";
                                    }
                                    else if (modelo.Equals("ROKU-MAC"))
                                    {
                                        arquivoCodeSoft = "RUKU_MAC.Lab";
                                    }
                                    else if (modelo.Equals("ROKU-PANEL"))
                                    {
                                        // arquivoCodeSoft = "BLANK.Lab";
                                        arquivoCodeSoft = "ROKU_PANEL.Lab";
                                    }
                                    else if (modelo.Equals("ROKU-SMT"))
                                    {
                                        arquivoCodeSoft = "SMT.Lab";
                                    }

                                    #endregion
                                    //
                                    try
                                    {
                                        string strFile = AppDomain.CurrentDomain.BaseDirectory + @"\IMPRESSAO\" + arquivoCodeSoft;
                                        lbl = new LabelManager2.ApplicationClass();
                                        lbl.Documents.Open(strFile, false);
                                        lbl.ActiveDocument.Printer.SwitchTo(Impressora, "", true);

                                        //VAREAVEL CODESOFT LABEL
                                        if (modelo.Equals("ROKU-SMT"))
                                        {
                                            string serial3 = snHorizontal;
                                            string serial4 = snVertical;
                                            snHorizontal = string.Empty;
                                            snVertical = string.Empty;

                                            lbl.ActiveDocument.Variables.FormVariables.Item("SERIAL").Value = ladoEsquerdo;
                                            lbl.ActiveDocument.Variables.FormVariables.Item("SERIAL2").Value = ladoDireito;
                                            lbl.ActiveDocument.Variables.FormVariables.Item("SERIAL3").Value = serial3;
                                            lbl.ActiveDocument.Variables.FormVariables.Item("SERIAL4").Value = serial4;
                                            lbl.ActiveDocument.Variables.FormVariables.Item("WO").Value = Wo;
                                        }
                                        else
                                        {
                                            lbl.ActiveDocument.Variables.FormVariables.Item("SERIAL_DIREITO").Value = ladoDireito;
                                            lbl.ActiveDocument.Variables.FormVariables.Item("SERIAL_ESQUERDO").Value = ladoEsquerdo;
                                        }
                                        //
                                        if ((!string.IsNullOrEmpty(snHorizontal)) && (!string.IsNullOrEmpty(snVertical)))
                                        {
                                            lbl.ActiveDocument.Variables.FormVariables.Item("HORIZONTAL_DIREITO").Value = snHorizontal;//3930BR
                                            lbl.ActiveDocument.Variables.FormVariables.Item("HORIZONTAL_ESQUERDO").Value = snHorizontal;//3930BR
                                            lbl.ActiveDocument.Variables.FormVariables.Item("VERTICAL_DIREITO").Value = snVertical;//wo
                                            lbl.ActiveDocument.Variables.FormVariables.Item("VERTICAL_ESQUERDO").Value = snVertical;//wo
                                        }

                                        lbl.ActiveDocument.PrintDocument(1);
                                        lbl.ActiveDocument.PrintLabel(1, 1, 1, 1, 1, "ETIQUETA-" + modelo);

                                        //Continuous batch print labels. FormFeed()The parameters such as the output variable must be after the execution, output to the printer. 
                                        //lbl.ActiveDocument.FormFeed();

                                        //Encerra o processo do programa CODSOFT
                                        string nomeExecutavel = "lppa"; //"LPPA.exe";
                                        foreach (Process pr in Process.GetProcessesByName(nomeExecutavel))
                                        {
                                            if (!pr.HasExited)
                                                pr.Kill();
                                        }
                                        //limpara vareaveis para próxima etiqueta
                                        proximo = 0;
                                        ladoEsquerdo = string.Empty;
                                        ladoDireito = string.Empty;
                                    }
                                    catch (Exception erro)
                                    {
                                        string e = erro.Message + " - " + erro.Source + " - " + erro.StackTrace;
                                        string nomeExecutavel = "lppa"; //"LPPA.exe";
                                        foreach (Process pr in Process.GetProcessesByName(nomeExecutavel))
                                        {
                                            if (!pr.HasExited)
                                                pr.Kill();
                                        }
                                    }
                                }

                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region ETIQUETA SIMPLES

                    #region NOME ARQUIVO CODESOFT

                    if (modelo.Equals("TG1692A-SMT"))
                    {
                        arquivoCodeSoft = "TG_SMT_SIMPLES.lab";
                    }
                    else if (modelo.Equals("TG1692BR-SMT"))
                    {
                        arquivoCodeSoft = "TGBR_SMT_SIMPLES.lab";
                    }
                    else if (modelo.Equals("E965"))
                    {
                        arquivoCodeSoft = "E965.lab";
                    }
                    else if (modelo.Equals("DSI"))
                    {
                        arquivoCodeSoft = "DSI.lab";
                    }
                    else if (modelo.Equals("ROKU-MAC"))
                    {
                        arquivoCodeSoft = "RUKU_MAC_SIMPLES.Lab";
                    }
                    else if (modelo.Equals("ROKU-PANEL"))
                    {
                        arquivoCodeSoft = "ROKU_PANEL.Lab";
                    }
                    else if (modelo.Equals("ROKU-SMT"))
                    {
                        arquivoCodeSoft = "SMT_SIMPLES.Lab";
                    }
                    else if (modelo.Equals("ROKU-GIFT_BOX"))
                    {
                        arquivoCodeSoft = "GIFT_BOX.lab";
                    }
                    else if (modelo.Equals("LASER"))
                    {
                        arquivoCodeSoft = "LASER.lab";
                    }

                    #endregion
                    //
                    string strFile = AppDomain.CurrentDomain.BaseDirectory + @"\IMPRESSAO\" + arquivoCodeSoft;
                    //LabelManager2.ApplicationClass lbl = null;
                    //Thread.CurrentThread.GetApartmentState();
                    try
                    {
                        LabelManager2.ApplicationClass lbl = new LabelManager2.ApplicationClass();
                        lbl.Documents.Open(strFile, false);
                        lbl.ActiveDocument.Printer.SwitchTo(Impressora, "", true);

                        //VAREAVEL CODESOFT LABEL
                        if (modelo.Equals("ROKU-GIFT_BOX"))
                        {
                            serial = Lista_Serial[0].ToString();
                            lbl.ActiveDocument.Variables.FormVariables.Item("SERIAL").Value = serial;                           
                        }
                        if (modelo.Equals("LASER"))
                        {
                            lbl.ActiveDocument.Variables.FormVariables.Item("SKUNO").Value = Lista_Serial[1].ToString();
                            lbl.ActiveDocument.Variables.FormVariables.Item("PALLET").Value = Lista_Serial[1].ToString();
                            lbl.ActiveDocument.Variables.FormVariables.Item("REV").Value = Lista_Serial[1].ToString();
                            lbl.ActiveDocument.Variables.FormVariables.Item("PALLETQTY").Value = Lista_Serial[1].ToString();
                        }
                        //
                        if ((!string.IsNullOrEmpty(snHorizontal)) && (!string.IsNullOrEmpty(snVertical)))
                        {
                            lbl.ActiveDocument.Variables.FormVariables.Item("HORIZONTAL_ESQUERDO").Value = snHorizontal;
                            lbl.ActiveDocument.Variables.FormVariables.Item("VERTICAL_ESQUERDO").Value = snVertical;
                        }
                        //
                        lbl.ActiveDocument.PrintDocument(1);
                        lbl.ActiveDocument.PrintLabel(1, 1, 1, 1, 1, "ETIQUETA-" + modelo);

                        //Continuous batch print labels.FormFeed()The parameters such as the output variable must be after the execution, output to the printer. 
                        //lbl.ActiveDocument.FormFeed();

                        //lbl.Documents.CloseAll(false);
                        //Marshal.ReleaseComObject(lbl);
                        //lbl.Quit();

                        //Encerra o processo do programa CODSOFT
                        string nomeExecutavel = "lppa"; //"LPPA.exe";
                        foreach (Process pr in Process.GetProcessesByName(nomeExecutavel))
                        {
                            if (!pr.HasExited)
                                pr.Kill();
                        }

                        //Process[] processes = Process.GetProcessesByName("lppa");

                        //doc.Close(false);
                        //Marshal.ReleaseComObject(doc); 

                    }
                    catch (Exception erro)
                    {
                        string e = erro.Message + " - " + erro.Source + " - " + erro.StackTrace;
                        string nomeExecutavel = "lppa"; //"LPPA.exe";
                        foreach (Process pr in Process.GetProcessesByName(nomeExecutavel))
                        {
                            if (!pr.HasExited)
                                pr.Kill();
                        }
                    }

                    #endregion
                }
            }

        }
    }
}
