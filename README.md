<h1 align="center">License Generator</h1>

A simple utility program to quickly create licenses files from a specified template(selected from a sqlite database).

__SIDENOTE__: I created this project because I didn’t want to waste too much time creating license files manually(my friends might not agree on that one though, but ok) and also to get the basics of the .NET environment. Due to that, I don’t have many plans for this project. In its current stage it's almost perfect for me, the only thing missing for me is a pipeline building system using GitHub Actions.

## Placeholders

`$currentYear` will be used as a placeholder for the current year.\
`$authors` will be used as a placeholder for the name(s) of the program’s author(s).

## Configuration

The only configuration option available is the default name of the created license file. Both the configuration and database files are located in one of the following directories(it depends on your operational system):

* Linux: `$HOME/.config/LicenseGenerator/`
* MacOS: `$HOME/Library/Preferences/LicenseGenerator/`
* Windows: `%AppData%/Local/LicenseGenerator/`

If the environmental variable \$XDG_CONFIG_HOME is set, it will be used as the reference for creating the configuration directory. Also, the default directory is the same as the Linux one, meaning that by default the program configuration will be located at: `$HOME/.config/LicenseGenerator`.
