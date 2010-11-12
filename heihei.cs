using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Threading;

using System.Net;
using System.Net.Sockets;


namespace xmlserialization
{
    public class NetTest
    {
    	static List<StreamWriter> all = new List<StreamWriter>();
			
        public static void Main()
        {
            Thread t = new Thread(new ThreadStart(Listener));
            t.Start();
        }
        

        public static void Listener()
        {
            TcpListener tl = new TcpListener(9999);
            tl.Start();

            while (true)
            {
                try
                {
                    TcpClient tc = tl.AcceptTcpClient();
                    Thread c = new Thread(new ParameterizedThreadStart(ServerInstans));
                    c.Start(tc);
                }
                catch
                {
                    break;
                }
            }

            tl.Stop();

        }

        public static void ServerInstans(object c)
        {
            TcpClient t = (TcpClient)c;
            Console.WriteLine("[S] Tilkobling mottatt.");
            NetworkStream ns = t.GetStream();
            
			StreamReader sr = new StreamReader (ns);
			StreamWriter sw = new StreamWriter (ns);
			
			FileStream ft = new FileStream ("tilbake.txt", FileMode.Open);
			Dictionary<string, string> resp = new Dictionary<string, string>() ;
			StreamReader st = new StreamReader (ft);
			while (!st.EndOfStream) {
				string l = st.ReadLine ();
				string[] d = l.Split (':');
				if (d.Length == 2) {
				resp.Add (d[0], d[1]);
				Console.WriteLine (d[0] + d[1]);
				}
			}
			
			st.Close ();
			ft.Close ();
			
			Console.WriteLine (resp.ContainsKey ("Hei").ToString ());
			
			sw.WriteLine ("HeiHei! Prøv ?");
			sw.Flush ();
			all.Add (sw);
			

			while (true)
			{
				try {
					String s = sr.ReadLine ();
					
					Console.WriteLine ("Inn: \"" + s + "\"");
					s = s.Trim ();
					
					if (s == "Hade") break;
					
					if (resp.ContainsKey (s)) {
						sw.WriteLine (resp[s]);
						sw.Flush ();
						Console.WriteLine (resp[s]);
					} else if (s == "?") {
						sw.WriteLine ("Woha! vetsj ka da e!");
						FileStream fh = new FileStream ("help.txt", FileMode.Open);
						StreamReader sh = new StreamReader (fh);
						string rh = sh.ReadToEnd ();
						sw.Write (rh);
						
						foreach (string k in resp.Keys) {
							sw.WriteLine (k);	
						}
						
						sw.Flush ();
						ns.Flush ();
						sh.Close ();
						fh.Close ();
					} else {
						wall (sw, t.Client.Handle.ToString () + ": " + s);	
					}
					
					
				} catch (Exception e) {
					Console.WriteLine ("Brutt.! " + e.ToString ());
					break;	
				}
					
			}
		 
            t.Close();
            all.Remove (sw);

        }
        
        public static void wall (StreamWriter self, string s) {
			foreach (StreamWriter sw in all) {
				try {
					if (sw != self) {
						sw.WriteLine (s);
						sw.Flush ();
					}
				} catch {
					continue;
				}
			}
		}

    }


}
