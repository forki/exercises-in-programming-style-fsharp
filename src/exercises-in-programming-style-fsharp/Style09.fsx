﻿open System
open System.IO
open System.Text.RegularExpressions

#time

// version 1 (with simple bind)

type Result<'a> = Result of 'a

// define the standard 'bind' operator
let (>>=) (Result p) (cont : 'a -> Result<'b>) = cont p

let ``stop words`` = 
    __SOURCE_DIRECTORY__ + "../stop_words.txt"
let ``p & p`` = 
    __SOURCE_DIRECTORY__ + "../pride-and-prejudice.txt"

let readFile path = 
    File.ReadAllText path |> Result

let filterChars data = 
    (new Regex("[\W_]+")).Replace(data, " ") |> Result

let normalize (input : string) = 
    input.ToLower() |> Result

let scan (input : string) = 
    input.Split() |> Result
    
let removeStopWords (words : string[]) =
    let (Result raw) = readFile ``stop words``
    
    let stopWords = 
        raw.Split ','
        |> Array.append 
            ([| 'a'..'z' |] |> Array.map string)
        |> Set.ofArray
    
    words 
    |> Array.filter (not << stopWords.Contains)
    |> Result

let frequencies words = 
    words 
    |> Seq.groupBy id 
    |> Seq.map (fun (word, gr) -> word, Seq.length gr)
    |> Seq.toArray
    |> Result

let sort (wordFreqs : (string * int)[]) = 
    wordFreqs 
    |> Array.sortByDescending (snd)
    |> Result

let top25Freqs (wordFreqs : (string * int)[]) =
    wordFreqs
    |> Seq.take 25
    |> Seq.map (fun (word, n) -> sprintf "%s - %d\n" word n)
    |> Seq.reduce (+)
    |> Result

let printMe = printf "%s"

let lift f = f >> Result

readFile ``p & p``
>>= filterChars
>>= normalize
>>= scan
>>= removeStopWords
>>= frequencies
>>= sort
>>= top25Freqs
>>= (lift printMe)