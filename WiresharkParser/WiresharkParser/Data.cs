using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiresharkParser
{
    public class Data
    {

        public string dateTime { get; set; }
        public string source { get; set; }
        public string sourcePort { get; set; }
        public string destination { get; set; }
        public string destinationPort { get; set; }
        public string protocol { get; set; }
        public string data { get; set; }
        public string information { get; set; }
        public Data(string dateTime,string source, string sourcePort, string destination, string destinationPort, string protocol, string data, string information)
        {
            this.dateTime = dateTime;
            this.source = source;
            this.sourcePort = sourcePort;
            this.destination = destination;
            this.destinationPort = destinationPort;
            this.protocol = protocol;
            this.data = data;
            this.information = information;
        }

        //public Data(string dateTime, string srcIP, string srcPort, string dstIP, string dstPort, string protokol, string v)
        //{
        //    this.dateTime = dateTime;
        //    this.srcIP = srcIP;
        //    this.srcPort = srcPort;
        //    this.dstIP = dstIP;
        //    this.dstPort = dstPort;
        //    this.protokol = protokol;
        //    this.v = v;
        //}
    }
}
