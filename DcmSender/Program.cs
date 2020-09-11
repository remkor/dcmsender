using CommandLine;
using Dicom.Network;
using System;
using System.IO;

namespace DcmSender
{
    public class Options
    {
        [Option("calledAE", HelpText = "Set called AE title.", Required = true)]
        public string CalledAE { get; set; }

        [Option("callingAE", HelpText = "Set calling AE title.", Required = true)]
        public string CallingAE { get; set; }

        [Option("input", HelpText = "Input file or directory to read.", Required = true)]
        public string Input { get; set; }

        [Option("pacsHost", HelpText = "Address of PACS server.", Required = true)]
        public string PacsHost { get; set; }

        [Option("pacsPort", HelpText = "Port of PACS server.", Required = true)]
        public int PacsPort { get; set; }

        [Option("verbose", HelpText = "Set output to verbose messages.", Required = false)]
        public bool Verbose { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(options =>
            {
                try
                {
                    if (File.Exists(options.Input))
                    {
                        DicomClient dicomClient = new DicomClient();
                        DicomRequest request = new DicomCStoreRequest(options.Input);

                        dicomClient.AddRequest(request);
                        dicomClient.Send(options.PacsHost, options.PacsPort, false, options.CallingAE, options.CalledAE);
                    }
                    else if (Directory.Exists(options.Input))
                    {
                        string[] files = Directory.GetFiles(options.Input, "*", SearchOption.AllDirectories);

                        foreach (var file in files)
                        {
                            if (options.Verbose) Console.WriteLine(file.ToString());

                            DicomClient dicomClient = new DicomClient();
                            DicomRequest request = new DicomCStoreRequest(file);

                            dicomClient.AddRequest(request);
                            dicomClient.Send(options.PacsHost, options.PacsPort, false, options.CallingAE, options.CalledAE);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid file or directory.");
                    }
                }
                catch (Exception exc)
                {
                    if (options.Verbose) Console.WriteLine(exc.Message);
                }
            });
        }
    }
}
