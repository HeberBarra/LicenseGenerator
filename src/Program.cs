using LicenseGenerator.config;
using LicenseGenerator.data.repository;
using LicenseGenerator.data.service;
using LicenseGenerator.domain.model;
using LicenseGenerator.utils;

namespace LicenseGenerator;

internal enum MainMenuOptions
{
    AddTemplate,
    ListTemplates,
    SearchById,
    UpdateTemplate,
    DeleteTemplate,
    CreateLicenseFile,
    ShowConfig,
    Exit
}

internal enum UpdateTemplateMenuOptions
{
    ChangeName,
    ChangeBody,
    SaveChanges,
    DiscardChanges
}

public static class Program
{
    public const string ProgramName = "LicenseGenerator";

    private static Configurator Configurator { get; }

    private static LicenseGenerator LicenseGenerator { get; }
    private static TemplateService TemplateService { get; }
    private static TemplateRepository TemplateRepository { get; }

    static Program()
    {
        Configurator = new Configurator(ConfigurationDirectoryPicker.PickConfigurationDirectory());
        Configurator.Configure();
        LicenseGenerator = new LicenseGenerator(Configurator.LicenseFilename);
        TemplateService = new TemplateService(Configurator.DatabaseFile);
        TemplateService.InitService();
        TemplateRepository = new TemplateRepository(TemplateService);
    }

    public static int Main()
    {
        ClearTerminalScreen.Clear();

        start:
        Console.WriteLine();
        ShowMainMenu();
        MainMenuOptions option = (MainMenuOptions)GetChosenOption();
        ClearTerminalScreen.Clear();

        switch (option)
        {
            case MainMenuOptions.AddTemplate:
                AddTemplate();
                goto start;

            case MainMenuOptions.ListTemplates:
                ListTemplates();
                goto start;

            case MainMenuOptions.SearchById:
                SearchById();
                goto start;

            case MainMenuOptions.UpdateTemplate:
                UpdateTemplate();
                goto start;

            case MainMenuOptions.DeleteTemplate:
                DeleteTemplate();
                goto start;

            case MainMenuOptions.CreateLicenseFile:
                CreateLicenseFile();
                goto start;

            case MainMenuOptions.ShowConfig:
                Configurator.ShowConfiguration();
                goto start;

            case MainMenuOptions.Exit:
                Console.WriteLine("Exiting program...");
                break;

            default:
                Console.WriteLine("Invalid Option. Try again...");
                goto start;
        }

        TemplateService.Dispose();

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

        string? body = ReadTemplateBodyFile.Read();
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

        TemplateRepository.Save(template);
        Console.WriteLine("Template saved with success.");
    }

    private static int? GetTemplateId()
    {
        while (true)
        {
            Console.Write("Template ID: \n> ");
            string? textId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(textId)) return null;

            int id;
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

    private static void ListTemplates()
    {
        List<Template> templates = TemplateRepository.List();

        if (templates.Count == 0) Console.WriteLine("There isn't any template registered");

        templates.ForEach(template => Console.WriteLine($"ID: {template.Id} - {template.Name}"));
    }

    private static void SearchById()
    {
        int? id = GetTemplateId();

        if (id == null) return;

        Template? template = TemplateRepository.SearchById((int)id);

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
        int? id = GetTemplateId();

        if (id == null) return;

        Template? template = TemplateRepository.SearchById((int)id);

        if (template == null)
        {
            Console.WriteLine("Template not found.");
            return;
        }

        start:
        Console.WriteLine($"[ {(int)UpdateTemplateMenuOptions.ChangeName} ] Change template name");
        Console.WriteLine($"[ {(int)UpdateTemplateMenuOptions.ChangeBody} ] Change template body");
        Console.WriteLine($"[ {(int)UpdateTemplateMenuOptions.SaveChanges} ] Save changes");
        Console.WriteLine($"[ {(int)UpdateTemplateMenuOptions.DiscardChanges} ] Discard changes");
        UpdateTemplateMenuOptions option = (UpdateTemplateMenuOptions)GetChosenOption();

        switch (option)
        {
            case UpdateTemplateMenuOptions.ChangeName:
                string? name = GetTemplateName();

                if (name != null)
                    template.Name = name;

                goto start;

            case UpdateTemplateMenuOptions.ChangeBody:
                string? body = ReadTemplateBodyFile.Read();

                if (body != null)
                    template.Body = body;

                goto start;

            case UpdateTemplateMenuOptions.SaveChanges:
                TemplateRepository.Update(template);
                return;

            case UpdateTemplateMenuOptions.DiscardChanges:
                return;

            default:
                Console.WriteLine("Invalid option. Try again...");
                goto start;
        }
    }

    private static void DeleteTemplate()
    {
        long? id = GetTemplateId();

        if (id == null) return;

        TemplateRepository.Delete((int)id);
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

        if (id == null) return;

        Template? template = TemplateRepository.SearchById((int)id);

        if (template == null)
        {
            Console.WriteLine("Template not found.");
            return;
        }

        List<string> authors = GetAuthorsName();

        if (template.Body == null) return;

        License license = new(Configurator.LicenseFilename, destinationPath, template.Body, authors);
        LicenseGenerator.CreateLicense(license);
    }

    private static void ShowMainMenu()
    {
        Console.WriteLine($"[ {(int)MainMenuOptions.AddTemplate} ] Add Template");
        Console.WriteLine($"[ {(int)MainMenuOptions.ListTemplates} ] List Templates");
        Console.WriteLine($"[ {(int)MainMenuOptions.SearchById} ] Search by ID");
        Console.WriteLine($"[ {(int)MainMenuOptions.UpdateTemplate} ] Update Template");
        Console.WriteLine($"[ {(int)MainMenuOptions.DeleteTemplate} ] Delete Template");
        Console.WriteLine($"[ {(int)MainMenuOptions.CreateLicenseFile} ] Create license file");
        Console.WriteLine($"[ {(int)MainMenuOptions.ShowConfig} ] Show config");
        Console.WriteLine($"[ {(int)MainMenuOptions.Exit} ] Exit Program");
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
            }
            catch (FormatException)
            {
                Console.WriteLine("Please enter a valid number. Try again...");
                continue;
            }


            return option;
        }
    }
}
