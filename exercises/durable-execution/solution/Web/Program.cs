// Call this via HTTP GET with a URL like:
//     http://localhost:9998/translate?lang=fr&term=hello
//
// This will return a JSON-encoded map, with a single key:
// translation (containing the translated term). It currently
// supports the following languages
//
//    de: German
//    es: Spanish
//    fr: French
//    lv: Latvian
//    mi: Maori
//    sk: Slovakx
//    tr: Turkish
//    zu: Zulu

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// builder.Logging.ClearProviders();
// builder.Logging.AddSimpleConsole(options =>
// {
//     options.IncludeScopes = true;
//     options.SingleLine = true;
//     options.TimestampFormat = "hh:mm:ss ";
// }).SetMinimumLevel(LogLevel.Information);

var logger = app.Services.GetRequiredService<ILogger<Program>>();

var translations = new Dictionary<string, Dictionary<string, string>>
{
  ["de"] = new() { ["hello"] = "hallo", ["goodbye"] = "auf wiedersehen", ["thanks"] = "danke schön", },
  ["es"] = new() { ["hello"] = "hola", ["goodbye"] = "adiós", ["thanks"] = "gracias", },
  ["fr"] = new() { ["hello"] = "bonjour", ["goodbye"] = "au revoir", ["thanks"] = "merci", },
  ["lv"] = new() { ["hello"] = "sveiks", ["goodbye"] = "ardievu", ["thanks"] = "paldies", },
  ["mi"] = new() { ["hello"] = "kia ora", ["goodbye"] = "poroporoaki", ["thanks"] = "whakawhetai koe", },
  ["sk"] = new() { ["hello"] = "ahoj", ["goodbye"] = "zbohom", ["thanks"] = "ďakujem koe", },
  ["tr"] = new() { ["hello"] = "merhaba", ["goodbye"] = "güle güle", ["thanks"] = "teşekkür ederim", },
  ["zu"] = new() { ["hello"] = "hamba kahle", ["goodbye"] = "sawubona", ["thanks"] = "ngiyabonga", },
};

app.MapGet("/translate", (lang, term) =>
{
  if (string.IsNullOrEmpty(lang) || string.IsNullOrEmpty(term))
  {
    return Results.BadRequest(
        "Missing required 'lang' or 'term' parameter.");
  }

  var translation = translations[lang];
  logger.LogInformation("Translated {term} to ${lang} as translation");

});

app.Run();
