using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FileValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("File path and Record Looping Path are required");
                return;
            }
           
            var filePath = args[0];
            var recordLoopingFilePath = args[1];

           if (File.Exists(filePath) && File.Exists(recordLoopingFilePath))
            {               
                var fileFormatStr = File.ReadAllText(recordLoopingFilePath, Encoding.UTF8);
                var fileFormat = JsonConvert.DeserializeObject<List<FileFormat>>(fileFormatStr);

                int lineCounter = 0; // to track on which line number the format is not as expected
                int noOfMaxOccurencesAllowed = 1; // no of times the record type can occur in a sequence
                string prevSequence = "BOF"; // To represent the beginning of file
                string currSequence;
                bool flag = false;
                bool needsFollowUpRecord = true; // to check if a rcord type needs to be followed up by another record type
                var checkSeq = new FileFormat();

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                try
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            lineCounter++;
                            if (line.Trim().Length > 0)
                            {
                                currSequence = line.TrimStart().Substring(0, 4).Trim();

                                checkSeq = fileFormat.Where(x => x.StartingSequence == prevSequence).FirstOrDefault();

                                if (prevSequence == currSequence)
                                    {
                                        noOfMaxOccurencesAllowed++;
                                        if (checkSeq != null && noOfMaxOccurencesAllowed > checkSeq.NoOfTimesExpected)
                                        {
                                            flag = true;
                                            Console.WriteLine("File is not in correct format at line: " + lineCounter);
                                            break;
                                        }

                                    }
                                    else
                                    {
                                        noOfMaxOccurencesAllowed = 0;

                                        if (checkSeq != null && checkSeq.CanBeFollowedBySequence.Any(x => x.Contains(currSequence)))
                                        {
                                            prevSequence = currSequence;
                                            needsFollowUpRecord = fileFormat.Where(x => x.StartingSequence == prevSequence).Select(x => x.NeedsFollowUpRecord).First();
                                        }
                                        else
                                        {
                                            flag = true;
                                            Console.WriteLine("File is not in correct format at line: " + lineCounter);
                                            break;
                                        }
                                    }
                            }                              

                        }
                        if (!flag && !needsFollowUpRecord)
                        {
                            Console.WriteLine("The file is in expected format");
                        }
                        else
                        {
                            Console.WriteLine("The file is not in expected format at line: " + lineCounter);
                        }
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine("An exception occured: " + ex.Message);
                }               
                
            }
           else
            {
                var message = File.Exists(filePath) ? "Record Looping file is not present" : "File is not present";
                Console.WriteLine(message);
            }
            Console.ReadLine(); 
        }
    }

    public class FileFormat
    {
        public string StartingSequence { get; set; }
        public int NoOfTimesExpected { get; set; }
        public bool NeedsFollowUpRecord { get; set; }
        public List<string> CanBeFollowedBySequence { get; set; }
    }
}
