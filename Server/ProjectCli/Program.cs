
using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
//תמר אלישיב
//פעולה המכילה מספר אפשרויות
var bundleCommand = new Command("bundle", "Bundle code files to a single file");
var bundleOption = new Option<FileInfo>("--output", "File path and name");
var Language = new Option<List<string>>("--language", "check language")
{
    //אפשרות חובה
    IsRequired = true,
    //אפשרות של כמה ארגומנטים לאחר כתיבת אפשרות
    AllowMultipleArgumentsPerToken = true,
};
var note = new Option<Boolean>("--note", "Brings routing and name");
var sort = new Option<Boolean>("--sort", "give files sort");
var removeLine = new Option<Boolean>("--remove", "remove empty line");
var author = new Option<string>("--author", "write author name");

//פעולה נוספת
var creatersp = new Command("creatersp", "create short");
//יצירת קצורים לאפשרויות-alias
bundleOption.AddAlias("-o");
Language.AddAlias("-l");
note.AddAlias("-n");
sort.AddAlias("-s");
removeLine.AddAlias("-r");
author.AddAlias("-a");
//bundle-הוספת האפשרויות לפעולה הראשית
bundleCommand.AddOption(bundleOption);
bundleCommand.AddOption(Language);
bundleCommand.AddOption(note);
bundleCommand.AddOption(sort);
bundleCommand.AddOption(removeLine);
bundleCommand.AddOption(author);
//מערך השפות
string[] language = { "java", "css", "js", "txt", "html" };
//רשימת הקבצים-נתובים וכו'
List<string> trueFile = new List<String>();

bundleCommand.SetHandler((output, ending, note, sort, remove, author) =>
{
    //output-יצירת הקובץ הרצוי
    try
    {
        //יצירת קובץ
        var f = File.Create(output.FullName);
        f.Close();
        //הדפסה הכותבת על יצירת הקובץ
        Console.WriteLine("File was created");
    }
    catch
    {
        Console.WriteLine("Error: File path is invalid or an error occurred.");
    }
    //הספריה הנוכחית
    string directory = Directory.GetCurrentDirectory();
    //language
    //אם רוצים את כל הקבצים-שקיימים גם במערך של השפות
    if (ending.Contains("all"))
    {//מעבר על המערך
        foreach (var lang in language)
        {//נכנס גם לתתי התקיות
            string[] filesWithLang = Directory.GetFiles(directory, "*." + lang, SearchOption.AllDirectories);
            //עובר על תתי התקיות
            foreach (var file in filesWithLang)
            {
                //אני לא רוצה להוסיף לרשימה מהתקיה הזו
                if (!file.Contains("Debug"))
                {
                    trueFile.Add(file);
                }
            }
        }
    }

    else
    {
        //עובר על מערך הסיומות
        foreach (string lang in ending)
        {
            //בודק האם מערך השפות מכיל את הסיומת
            if (language.Contains(lang))
            {
                //תתי תקיות

                string[] l = Directory.GetFiles(directory, "*." + lang, SearchOption.AllDirectories);
                Console.WriteLine(l.Length);
                // מוסיף את הנתובים לרשימה
                foreach (var item in l)
                {
                    //לא רוצה להכנס לתקיה זו
                    if (!item.Contains("Debug"))
                    {
                        //מוסיף לתוך הרשימה את הנתוב של הקובץ

                        trueFile.Add(item);
                    }
                }
            }
            else
            {
                Console.WriteLine("Error: The specified language is not valid.");
            }
        }
    }


    Console.WriteLine("-----------------------------------");
    // אפשרות המיון:
    if (sort)
        //ממין לפי השפה
        trueFile = trueFile.OrderBy(fn => Path.GetExtension(fn)).ToList();

    else
        //ממיין לפי הא"ב
        trueFile = trueFile.OrderBy(f => Path.GetFileName(f)).ToList();
    //המרת תוכן הקבצים לstring והוספה לstring את התוכן
    string str = "";
    //מביא את שם היוצר בראש הדף
    if (!string.IsNullOrEmpty(author))
    {
        str += author;
        str += "\n";
    }

    foreach (var item in trueFile)
    {
        //מחיקת שורה 
        if (remove)
        {
            // קורא את השורות  על כל קובץ ובודק האם השורה ריקה ואם כן מוחק
            var lines = File.ReadAllLines(item).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToArray();
            File.WriteAllLines(item, lines);
        }
        Console.WriteLine(item);

        //כותב לכל קובץ את הנתוב שלו
        if (note)
        {
            str += "//" +
                item;

            str += "\n";

        }
        //מוסיף את תוכן הקבצים
        str += File.ReadAllText(item);
        str += "\n";
    }


    File.WriteAllText(output.FullName, str);
}, bundleOption, Language, note, sort, removeLine, author);

creatersp.SetHandler(() =>
{
    //create- rsp
    string commandStr = "";
    try
    {
        //המשתמש מכניס נתונים ואת הנתונים שהזין מציב בתוך מחרוזת
        Console.WriteLine("Enter a file name and file type: ");
        commandStr += " " + "--output" + " " + Console.ReadLine();

        Console.WriteLine("Enter desired language or all languages-all:");
        if (Console.ReadLine == null)
        {
            Console.WriteLine("Eror:is required ");
        }
        else
        {
            commandStr += " " + "--language" + " " + Console.ReadLine();
        }
        Console.WriteLine("(y/n) Enter Do you want to show paths and file name?");
        if (Console.ReadLine() == "y")
            commandStr += " " + "--note";
        Console.WriteLine("Do you want to sort by file name? (y/n)");
        if (Console.ReadLine() == "y")
            commandStr += " " + "--sort";
        Console.WriteLine("Do you want to delete blank lines? (y/n)");
        if (Console.ReadLine() == "y")
            commandStr += " " + "--remove";
        Console.WriteLine("If you want to write the creator's name at the top of the page, write it, and if you want me, write no");
        string a = Console.ReadLine();
        if (a != "no")
            commandStr += " " + "--author " + " " + a;
        if (a == null)
        {
            Console.WriteLine("Eror");
        }
        //יוצר קובץ בסיומת rsp
        File.WriteAllText("creatersp.rsp", commandStr);
    }
    catch
    {
        Console.WriteLine("Error: File path is invalid or an error occurred.");
    }

});

var rootCommand = new RootCommand("root command for file bundler CLI");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(creatersp);
rootCommand.InvokeAsync(args);