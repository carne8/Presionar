open System
open FSharp.Configuration

let [<Literal>] templatePath = (__SOURCE_DIRECTORY__ + "/template-config.yml")
type Config = YamlConfig<templatePath>

let loadConfig (path: string) =
    let config = Config()
    config.Load path
    config

let KnuthShuffle (lst : #seq<'a>) =
    let lst = lst |> Seq.toArray
    let Swap i j =
        let item = lst.[i]
        lst.[i] <- lst.[j]
        lst.[j] <- item
    let rnd = new Random()
    let ln = lst.Length
    [0..(ln - 2)]
    |> Seq.iter (fun i -> Swap i (rnd.Next(i, ln)))
    lst |> Array.toList


[<EntryPoint>]
let main args =
    let configPath =
        args
        |> Array.tryHead
        |> Option.defaultValue (__SOURCE_DIRECTORY__ + "/template-config.yml")

    let totalQuestions = args |> Array.tryItem 1 |> Option.defaultValue "5" |> int

    let config = configPath |> loadConfig

    let all =
        config.verbs
        |> List.ofSeq
        |> List.map (fun verb -> [ verb.infinitive, "1s", verb.``1s``; verb.infinitive, "2s", verb.``2s``; verb.infinitive, "3s", verb.``3s``; verb.infinitive, "1p", verb.``1p``; verb.infinitive, "2p", verb.``2p``; verb.infinitive, "3p", verb.``3p``; ])
        |> List.concat

    all
    |> KnuthShuffle
    |> List.truncate totalQuestions
    |> List.map (fun (verb, person, correctAnswer) ->
        Console.Write (sprintf "%s %s: " verb person)
        let answer = Console.ReadLine().Trim().ToLower()

        if answer = correctAnswer.ToLower() then
            Console.ForegroundColor <- ConsoleColor.Green
            Console.WriteLine "Correct"
            Console.ResetColor()
            Console.WriteLine()
            true
        else
            Console.ForegroundColor <- ConsoleColor.Red
            Console.WriteLine ("False, it was " + correctAnswer.ToLower())
            Console.ResetColor()
            Console.WriteLine()
            false
    )
    |> fun list ->
        let correct = list |> List.filter ((=) true) |> List.length
        let incorrect = list |> List.filter ((=) false) |> List.length

        printfn ""
        printfn "Finished !"
        printfn "✅ Correct answers: %i" correct
        printfn "❌ Incorrect answers: %i" incorrect

    0