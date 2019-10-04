using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace SamsungMDCSerial
{
    public abstract class Options
    {
        [Option('i', "id", Required = true, HelpText = "The id of the display to control")]
        public byte DisplayId { get; set; }

        [Option('c', "com", Required = true, HelpText = "Com port to use to communicate with the display")]
        public string ComPort { get; set; }
    }

    [Verb("on")]
    public class On : Options { }

    [Verb("off")]
    public class Off : Options { }

    [Verb("list")]
    public class List
    {

    }
}
