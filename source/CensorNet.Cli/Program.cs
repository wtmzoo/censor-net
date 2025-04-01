using System.Text;
using System.Text.RegularExpressions;

namespace CensorNet.Cli;

public static class Program
{
    private static readonly string[] Patterns =
    [
        @"\b[А-ЯЁA-Z][а-яёa-z]+\s[А-ЯЁA-Z][а-яёa-z]+(?:-[А-ЯЁA-Z][а-яёa-z]+)?\b", // Имена и фамилии (русские и английские, начинающиеся с заглавной буквы)
        @"(?:\+?\d{1,3}[- ]?)?\(?\d{3}\)?[- ]?\d{3}[- ]?\d{2}[- ]?\d{2}", // Номера телефонов (разные форматы)
        @"\b(?:ул\.|улица|пр\.|проспект|пер\.|переулок|бульвар|б-р|наб\.|набережная)\s[А-ЯЁA-Z][а-яёa-z]+\b.*?\d+[а-я]?", // Адреса (улицы, дома, квартиры)
        @"\b\d{6}\b", // Почтовые индексы
        @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b", // Email адреса
        @"\b\d{1,3}\.\d+\s*[°]?\s*[NSns],?\s*\d{1,3}\.\d+\s*[°]?\s*[EWew]\b", // Координаты (широта/долгота)
        @"\b(?:\d[ -]*?){13,16}\b" // Номера банковских карт

    ];
    
    public static void Main()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "letter.txt");
        
        if (!File.Exists(filePath)) throw new FileNotFoundException("File not found", filePath);

        var fileDirectoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(fileDirectoryPath)) throw new DirectoryNotFoundException($"Directory {fileDirectoryPath} not found");
        
        var outputPath = Path.Combine(
            fileDirectoryPath,
            Path.GetFileNameWithoutExtension(filePath) + "_censored" + Path.GetExtension(filePath));
        
        try
        {
            var text = File.ReadAllText(filePath, Encoding.UTF8);
            var censoredText = Patterns.Aggregate(text, (current, pattern) => 
                Regex.Replace(current, pattern, "[censored]", RegexOptions.IgnoreCase));
            
            File.WriteAllText(outputPath, censoredText, Encoding.UTF8);
            Console.WriteLine($"Файл сохранен по пути: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}