using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FileValidation
{
    class Program
    {
        static void Main(string[] args){

            var str = File.ReadAllText(@"", Encoding.UTF8);
            var fileFormat = JsonConvert.DeserializeObject<List<FileFormmat>>(str);

            int lineCounter=0, noOfMaxOccurencesAllowed=1;
            string prevSequence = "BOF";
            string currSequence;
            bool flag = false;
            var checkSeq = new FileFormat();

            var fileStream = new FileStream(@"", FileMode.Open, FileAccess.Read);
            using( var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while((line = streamReader.ReadLine()) != null){
                    lineCounter++;
                    if(line.Length > 0)
                    {
                        currSequence = line.SubString(0, 4).Trim();
                        checkSeq = fileFormat.Where( x => x.StartingSequence == prevSequence).FirstOrDefault();
                        if(prevSequence != "BOF"){
                            if(prevSequence == currSequence){
                                noOfMaxOccurencesAllowed++;
                                if(checkSeq != null && noOfMaxOccurencesAllowed > checkSeq.NoOfTimesExpected)
                                {
                                    flag = true;
                                    Console.WriteLine("File is not in correct format at line: " + lineCounter);
                                    break;
                                }
                            }
                            else{
                                noOfMaxOccurencesAllowed = 0;
                            }
                        }
                        if(checkSeq != null && checkSeq.CanBeFollowedBySequence.Any(x=> x.Contains(currSequence)))
                        {
                            prevSequence = currSequence;
                        }
                        else{
                            flag = true;
                            Console.WriteLine("File is not in correct format at line: " + lineCounter);
                            break;
                        }
                        Console.WriteLine(line.TrimStart());
                    }
                }
                if(!flag){
                    Console.WriteLine("The file is in expected format");
                }
            }
            Console.ReadLine();

        }
    }
    public class FileFormat{
        public string StartingSequence { get; set;}
        public int NoOfTimesExpected { get; set;}
        public List<String> CanBeFollowedBySequence { get; set;}
    }

}
