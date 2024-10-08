namespace LicenseGenerator;

public static class Program
{
    public const string ProgramName = "LicenseGenerator";
    private const int AddTemplateOption = 0;
    private const int ListTemplatesOption = 1;
    private const int SearchByIdOption = 2;
    private const int UpdateTemplateOption = 3;
    private const int DeleteTemplateOption = 4;
    private const int CreateLicenseFileOption = 5;
    private const int ShowConfigOption = 6;
    private const int ExitOption = 7;
    private static Configurator? Configurator { get; set; }
    private static TemplateManager? TemplateManager { get; set; }
    private static LicenseGenerator? LicenseGenerator { get; set; }
    
    public static int Main()
    {
        Configurator = new Configurator();
        Configurator.Configure();
        if (Configurator.DatabaseFile != null) TemplateManager = new TemplateManager(Configurator.DatabaseFile);
        if (Configurator.LicenseFilename != null) LicenseGenerator = new LicenseGenerator(Configurator.LicenseFilename);

        if (TemplateManager == null || LicenseGenerator == null)
            return 1;
        
        start:
        ShowMenu();
        int option = GetChosenOption();

        switch (option)
        {
            
            case AddTemplateOption:
                AddTemplate();
                goto start;
            
            case ListTemplatesOption:
                ListTemplates();
                goto start;
            
            case SearchByIdOption:
                SearchById();
                goto start;
            
            case UpdateTemplateOption:
                UpdateTemplate();
                goto start;
            
            case DeleteTemplateOption:
                DeleteTemplate();
                goto start;
            
            case CreateLicenseFileOption:
                CreateLicenseFile();
                goto start;
            
            case ShowConfigOption:
                Configurator.ShowConfiguration();
                goto start;
            
            case ExitOption:
                Console.WriteLine("Exiting program...");
                break;
            
            default:
                Console.WriteLine("Invalid Option. Try again...");
                goto start;
            
        }
        
        TemplateManager.CloseConnection();
        
        return 0;
    }
    
    private static void AddTemplate()
    {
        string? templateName = GetTemplateName();
        if (templateName == null) 
        {
            Console.WriteLine("Operation canceled. Returning to main menu...");
            return;
        }

        string?  body = GetTemplateBody();
        if (body == null) 
        {
            Console.WriteLine("Operation canceled. Returning to main menu...");
            return;
        }

        Template template = new()
        {
            Name = templateName,
            Body = body
        };
        
        TemplateManager!.AddTemplate(template);
        Console.WriteLine("Template saved with success.");
    }

    private static long? GetTemplateId()
    {
        while (true)
        {
            Console.Write("Template ID: \n> ");
            string? textId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(textId))
            {
                return null;
            }

            long id;
            try
            {
                id = int.Parse(textId);
            }
            catch (FormatException)
            {
                Console.WriteLine("Please insert a valid number. Try again...");
                continue;
            }

            return id;
        }
    }

    private static string? GetTemplateName()
    {
        while (true)
        {
            Console.Write("Template name: \n> ");
            string? name = Console.ReadLine();
            
            return name == string.Empty ? null : name;
        }
    }
    
    private static string? GetTemplateBody() 
    {
        while (true)
        {
            Console.Write("Template body filepath: \n> ");
            string? file = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(file))
            {
                return null;
            }
            
            if (!File.Exists(file)) 
            {
                Console.WriteLine($"Error! File {file} doesn't exists. Try again...");
                continue;
            }

            if (File.ReadAllLines(file).Length != 0) return string.Join("\n", File.ReadAllLines(file));
            
            Console.WriteLine($"Error! File {file} is empty. Try again...");
        }
    }
    
    private static void ListTemplates()
    {
        List<Template> templates = TemplateManager!.GetEntries();
        
        if (templates.Count == 0) 
        {
            Console.WriteLine("There isn't any template registered");
        }
        
        templates.ForEach(template => Console.WriteLine($"ID: {template.Id} - {template.Name}"));
    }
    
    private static void SearchById()
    {
        long? id = GetTemplateId();
        
        if (id == null)
            return;

        Template? template = TemplateManager!.SearchById(id);

        if (template == null) 
        {
            Console.WriteLine("Template not found.");
            return;
        }
        
        Console.WriteLine($"ID: {template.Id}");
        Console.WriteLine($"Name: {template.Name}");
        Console.WriteLine($"Body: \n{template.Body}");
    }
    
    private static void UpdateTemplate()
    {
        const int changeName = 0;
        const int changeBody = 1;
        const int saveChanges = 2;
        const int discardChanges = 3;
        long? id = GetTemplateId();
        
        if (id == null)
            return;

        Template? template = TemplateManager!.SearchById(id);
        
        if (template == null) 
        {
            Console.WriteLine("Template not found.");
            return;
        }
        
        start:
        Console.WriteLine($"[ {changeName} ] Change template name");
        Console.WriteLine($"[ {changeBody} ] Change template body");
        Console.WriteLine($"[ {saveChanges} ] Save changes");
        Console.WriteLine($"[ {discardChanges} ] Discard changes");
        int option = GetChosenOption();
        
        switch (option)
        {
            case changeName:
                string? name = GetTemplateName();

                if (name != null)
                    template.Name = name;
                
                goto start;
            
            case changeBody:
                string? body = GetTemplateBody();

                if (body != null)
                    template.Body = body;

                goto start;
            
            case saveChanges:
                TemplateManager.UpdateEntry(template);
                return;
            
            case discardChanges:
                return;
            
            default:
                Console.WriteLine("Invalid option. Try again...");
                goto start;
        }
        
    }
    
    private static void DeleteTemplate()
    {
        long? id = GetTemplateId();
        
        if (id == null)
            return;
        
        TemplateManager!.DeleteEntry(id);
    }
    
    private static List<string> GetAuthorsName()
    {
        List<string> authors = [];
        
        Console.Write("Add author(s)? [y/N]\n> ");
        string? option = Console.ReadLine();

        while (option != null && option.ToLower().StartsWith('y'))
        {
            Console.Write("Author's name: \n> ");
            string? author = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(author))
                return authors;
            
            authors.Add(author);
            Console.Write("Add another author? [y/N]\n> ");
            option = Console.ReadLine();
        }
        
        return authors;
    }
    
    private static void CreateLicenseFile()
    {
        Console.Write("License file destination path: \n> ");
        string? destinationPath = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(destinationPath)) 
            return;

        long? id = GetTemplateId();
        
        if (id == null)
            return;

        Template? template = TemplateManager!.SearchById(id);
        
        if (template == null) 
        {
            Console.WriteLine("Template not found.");
            return;
        }

        List<string> authors = GetAuthorsName();
        if (template.Body != null) LicenseGenerator!.CreateLicenseFile(template.Body, destinationPath, authors);
    }
    
    private static void ShowMenu() 
    {
        Console.WriteLine($"[ {AddTemplateOption} ] Add Template");
        Console.WriteLine($"[ {ListTemplatesOption} ] List Templates");
        Console.WriteLine($"[ {SearchByIdOption} ] Search by ID");
        Console.WriteLine($"[ {UpdateTemplateOption} ] Update Template");
        Console.WriteLine($"[ {DeleteTemplateOption} ] Delete Template");
        Console.WriteLine($"[ {CreateLicenseFileOption} ] Create license file");
        Console.WriteLine($"[ {ShowConfigOption} ] Show config");
        Console.WriteLine($"[ {ExitOption} ] Exit Program");
    }
    
    private static int GetChosenOption()
    {
        while (true)
        {
            Console.Write("Choose an option: \n> ");
            string? input = Console.ReadLine();
            int option;
            
            if (input == null)
            {
                Console.WriteLine("Input cannot be null. Try again...");
                continue;
            }
            
            try 
            {
                option = int.Parse(input);
            } catch (FormatException) {
                Console.WriteLine("Please enter a valid number. Try again...");
                continue;
            }


            return option;
        }
    } 
}