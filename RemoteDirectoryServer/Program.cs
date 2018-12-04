using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using PirateRPC;
using System.Threading;
using System.Xml;
using System.IO;
using Verse;
using System.Reflection;
/*
namespace UPnP
{
public class NAT
{
static TimeSpan _timeout = new TimeSpan(0, 0, 0, 3);
public static TimeSpan TimeOut
{
get { return _timeout; }
set { _timeout = value; }
}
static string _descUrl, _serviceUrl, _eventUrl;
public static bool Discover()
{
Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
string req = "M-SEARCH * HTTP/1.1\r\n" +
"HOST: 239.255.255.250:1900\r\n" +
"ST:upnp:rootdevice\r\n" +
"MAN:\"ssdp:discover\"\r\n" +
"MX:3\r\n\r\n";
byte[] data = Encoding.ASCII.GetBytes(req);
IPEndPoint ipe = new IPEndPoint(IPAddress.Broadcast, 1900);
byte[] buffer = new byte[0x1000];

DateTime start = DateTime.Now;

do
{
s.SendTo(data, ipe);
s.SendTo(data, ipe);
s.SendTo(data, ipe);

int length = 0;
do
{
length = s.Receive(buffer);

string resp = Encoding.ASCII.GetString(buffer, 0, length).ToLower();
if (resp.Contains("upnp:rootdevice"))
{
resp = resp.Substring(resp.ToLower().IndexOf("location:") + 9);
resp = resp.Substring(0, resp.IndexOf("\r")).Trim();
if (!string.IsNullOrEmpty(_serviceUrl = GetServiceUrl(resp)))
{
_descUrl = resp;
return true;
}
}
} while (length > 0);
} while (start.Subtract(DateTime.Now) < _timeout);
return false;
}

private static string GetServiceUrl(string resp)
{
#if !DEBUG
try
{
#endif
XmlDocument desc = new XmlDocument();
desc.Load(WebRequest.Create(resp).GetResponse().GetResponseStream());
XmlNamespaceManager nsMgr = new XmlNamespaceManager(desc.NameTable);
nsMgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
XmlNode typen = desc.SelectSingleNode("//tns:device/tns:deviceType/text()", nsMgr);
if (!typen.Value.Contains("InternetGatewayDevice"))
return null;
XmlNode node = desc.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:controlURL/text()", nsMgr);
if (node == null)
return null;
XmlNode eventnode = desc.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:eventSubURL/text()", nsMgr);
_eventUrl = CombineUrls(resp, eventnode.Value);
return CombineUrls(resp, node.Value);
#if !DEBUG
}
catch { return null; }
#endif
}

private static string CombineUrls(string resp, string p)
{
int n = resp.IndexOf("://");
n = resp.IndexOf('/', n + 3);
return resp.Substring(0, n) + p;
}

public static void ForwardPort(int port, ProtocolType protocol, string description)
{
if (string.IsNullOrEmpty(_serviceUrl))
throw new Exception("No UPnP service available or Discover() has not been called");
XmlDocument xdoc = SOAPRequest(_serviceUrl, "<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">" +
"<NewRemoteHost></NewRemoteHost><NewExternalPort>" + port.ToString() + "</NewExternalPort><NewProtocol>" + protocol.ToString().ToUpper() + "</NewProtocol>" +
"<NewInternalPort>" + port.ToString() + "</NewInternalPort><NewInternalClient>" + Dns.GetHostAddresses(Dns.GetHostName())[0].ToString() +
"</NewInternalClient><NewEnabled>1</NewEnabled><NewPortMappingDescription>" + description +
"</NewPortMappingDescription><NewLeaseDuration>0</NewLeaseDuration></u:AddPortMapping>", "AddPortMapping");
}

public static void DeleteForwardingRule(int port, ProtocolType protocol)
{
if (string.IsNullOrEmpty(_serviceUrl))
throw new Exception("No UPnP service available or Discover() has not been called");
XmlDocument xdoc = SOAPRequest(_serviceUrl,
"<u:DeletePortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">" +
"<NewRemoteHost>" +
"</NewRemoteHost>" +
"<NewExternalPort>" + port + "</NewExternalPort>" +
"<NewProtocol>" + protocol.ToString().ToUpper() + "</NewProtocol>" +
"</u:DeletePortMapping>", "DeletePortMapping");
}

public static IPAddress GetExternalIP()
{
if (string.IsNullOrEmpty(_serviceUrl))
throw new Exception("No UPnP service available or Discover() has not been called");
XmlDocument xdoc = SOAPRequest(_serviceUrl, "<u:GetExternalIPAddress xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">" +
"</u:GetExternalIPAddress>", "GetExternalIPAddress");
XmlNamespaceManager nsMgr = new XmlNamespaceManager(xdoc.NameTable);
nsMgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
string IP = xdoc.SelectSingleNode("//NewExternalIPAddress/text()", nsMgr).Value;
return IPAddress.Parse(IP);
}

private static XmlDocument SOAPRequest(string url, string soap, string function)
{
string req = "<?xml version=\"1.0\"?>" +
"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
"<s:Body>" +
soap +
"</s:Body>" +
"</s:Envelope>";
WebRequest r = HttpWebRequest.Create(url);
r.Method = "POST";
byte[] b = Encoding.UTF8.GetBytes(req);
r.Headers.Add("SOAPACTION", "\"urn:schemas-upnp-org:service:WANIPConnection:1#" + function + "\"");
r.ContentType = "text/xml; charset=\"utf-8\"";
r.ContentLength = b.Length;
r.GetRequestStream().Write(b, 0, b.Length);
XmlDocument resp = new XmlDocument();
WebResponse wres = r.GetResponse();
Stream ress = wres.GetResponseStream();
resp.Load(ress);
return resp;
}
}
}
*/
//NEVER run server with debugger attached!
namespace RemoteDirectoryServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int listenPort = 12345;// int.Parse(System.Configuration.ConfigurationManager.AppSettings["listen_port"]);
            int playercount = 2;
            bool useNatTraversal = true;
            try
            {
                CooperateRim.TickManagerPatch.syncRoundLength = int.Parse(System.Configuration.ConfigurationManager.AppSettings["round_length_ticks"]);
                playercount = int.Parse(System.Configuration.ConfigurationManager.AppSettings["player_count"]);
                useNatTraversal = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["sockopt_nat_traversal"]);
                NetDemo.SetDesiredPlayerCount(playercount);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
                Console.WriteLine("Error reading config file. Press any key to exit.");
                Console.ReadKey();
                return;
            }
            //UPnP.NAT.Discover();
            //UPnP.NAT.ForwardPort(12345, ProtocolType.Tcp, "MyApp (TCP)");
            //CRand.set_state(900 << 32);
            int workers;
            int completions;
            ThreadPool.GetMinThreads(out workers, out completions);
            bool res = ThreadPool.SetMinThreads(4, 4);
            TcpListener lst = new TcpListener(IPAddress.Any, listenPort);
            lst.Server.NoDelay = true;
            lst.AllowNatTraversal(useNatTraversal);
            lst.Server.ExclusiveAddressUse = true;
            lst.Start();
            AsyncCallback acceptor = null;
            Console.WriteLine("Player count set to " + playercount);
            Console.WriteLine("Listen port is " + listenPort);

            AssemblyName updatedAssemblyName = AssemblyName.GetAssemblyName("CooperateRim.dll");
            Console.WriteLine("RimAlong mod version is " + updatedAssemblyName.Version );
            Console.WriteLine("RimAlong server version 0.0.0.2, codename \"mumblebee\", ready to rumble!");
            CooperateRim.SyncTickData.cliendID = 0;
            

            lst.BeginAcceptTcpClient(acceptor = u =>
            {
                Console.WriteLine("new client");
                TcpClient tcc = (u.AsyncState as TcpListener).EndAcceptTcpClient(u);
                NetworkStream ns = tcc.GetStream();
                int clid = CooperateRim.SyncTickData.cliendID;

                (u.AsyncState as TcpListener).BeginAcceptTcpClient(acceptor, u.AsyncState);

                if (clid < NetDemo.desiredPlayerCount)
                {
                    NetDemo.allClients.AddLast(ns);


                    for (; tcc.Connected;)
                    {
                        Thread.Sleep(10);
                        if (ns.DataAvailable)
                        {
                            PirateRPC.PirateRPC.ReceiveInvocation(ns);
                        }
                    };
                }
                else
                {
                    for (; !ns.DataAvailable;)
                    {
                        Thread.Sleep(10);
                        if (ns.DataAvailable)
                        {
                            PirateRPC.PirateRPC.ReceiveInvocation(ns);
                            break;
                        }
                    };
                }

                Console.WriteLine("client lost");
            }, lst);
            
            for (; ; )
            {
                Thread.Sleep(100);
            }
        }
    }
}
