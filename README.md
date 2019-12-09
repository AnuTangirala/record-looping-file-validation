# record-looping-file-validation

## Assumptions
white space at the start of each line can be ignored : Eg : "      10" is as good as line "10"

Blank lines in between can be ignored

sequence to be verified is string and case insensitive

after Trimming in the start, first 4 characters are considered for Sequence validation. 

## How to run the program
FileValidation "D:\Anuradha\FileToBeValidated.txt" "D:\Anuradha\FileFormat.json"

## Overview of the solution
The approach I have used for validating the file is as follows :

The Record looping diagram is converted to Json format, which in turn is used as basis for validating the required file.

The Json file is deserialized to FileFormat object , which is a class which consists of the following properties : 

StartingSequence, //StartingSequence denotes the sequence with which the line starts
NoOfTimesExpected , //NoOfTimesExpected denotes the maximum number of times the StartingSequence can be expected in a file
NeedsFollowUpRecord, // Indicated whether a record has to be followed up by a different record : Eg 10 should be followed by 12
CanBeFollowedBySequence  //CanBeFollowedBySequence denotes the valid sequences that can follow the StartingSequence

The file to be verfied is accessed and read line by line.
Beginning of the file is referred to "BOF". "BOF" is taken as first previous sequence.
on each line read, the first 4 characters, post trimming , are assigned to current sequence.

for the previous sequence, fetch from FileFormat, the maximum no of times it is allowed and list of valid sequences that can follow it. This is fetched by matching the previous sequence with StartingSequence in the FileFormat data.

if the current sequence is matching the previous sequence, check whether it is less than or equal to the maximum no of times it is allowed limit. 
If it exceeds, set a flag to true, print that the file is not in valid format at so and so line  and break from the loop
this flag is to check whether we are exiting the loop because we reached end of file or because there were some unexpected sequences midway

if the current sequence is not matching the previous sequence, verify whether the current sequence is present in the list of allowed sequences that can follow it.
If it is not present, set a flag to true,, print that the file is not in valid format at so and so line and break from the loop to read lines of file..
if it is present, assign current sequence to previous sequence, reset the counter tracking the sequence occurence count and fetch the next line
repeat the steps from  ##29

if the loop is exited and the flag is fale, that means the file is a valid file.

## JSON Config
Json file needs to be prepared based on the given record looping diagram. The reason for using JSON config file is because it is simpler and readable.

## Test Files used
The following are the files used for testing. All these are attached in the repository
FileToBeValidated.txt
FileToBeValidated1.txt
FileToBeValidated2.txt
FileToBeValidated3.txt
FileToBeValidated4.txt
FileToBeValidated5.txt
FileToBeValidated6.txt
