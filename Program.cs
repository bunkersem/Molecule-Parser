using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using MoleculeParse;

namespace moleculeparser
{
    class Program
    {
        // test
        static void Main(string[] args)
        {
            Console.WriteLine("Example: ");
            var resourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream("moleculeparser.Molecules.json");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Molecule[]));
            var molecules = ser.ReadObject(resourceStream) as Molecule[];
            
            for(int i = 0; i < molecules.Length; i++) {
                System.Console.WriteLine("{0} ({1}) : [ {2} ]", 
                    molecules[i].name, 
                    molecules[i].structure,
                    String.Join(", ", MoleculeParser.Parse(molecules[i].structure)
                        .Select(x => String.Format("{0} : {1}", x.Key, x.Value))));
                if ((i+1) % 10 == 0) {
                    System.Console.WriteLine("---More---");
                    System.Console.ReadKey(true);
                }
            }
        }
    }

    [DataContract]  
    internal class Molecule  
    {  
        #pragma warning disable 0649
        [DataMember]  
        internal string name;  

        [DataMember]  
        internal string structure;  
        #pragma warning restore 0649
    } 
}
