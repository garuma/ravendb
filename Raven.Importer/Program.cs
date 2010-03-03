using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using Raven.Database;

namespace Raven.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: importer.exe db-dir docs-dir");
                return;
            }
            var files = Directory.GetFiles(args[1]);
            Console.WriteLine("Parsing {0:#,#} docs", files.Length);
            var array = files.Select(x => JObject.Parse(File.ReadAllText(x))).ToArray();
            Console.WriteLine("Inserting {0:#,#} docs", files.Length);
            var sw = Stopwatch.StartNew();
            int count = 0;
            using (var db = new DocumentDatabase(args[0]))
            {
                foreach (var doc in array)
                {
                    count++;
                    db.Put(null, Guid.Empty, doc, new JObject());
                }
            }
            Console.WriteLine("{0} doc inserts in {1}", count, sw.Elapsed);
        }
    }
}

