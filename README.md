This tool will help you to create (parts of) a InnoSetup Script, which will check installation files after installation for integrity, using a checksum. It makes sense using this when you have a lot of installation files.

As a bonus, it will maintain folder structure, so you don't have to determine the destination folder for every single file. In order to do this, you  have to add the main executable file (*.exe) first, whch will act as starting point. All other destination paths will be relative to this.

You can add files and folders from Windows Desktop or Windows Explorer, even mulitple at a time, via Drag and Drop onto the red area.

This will not provide 100% of safety, if you want so, you should think about getting a siganture for your software. This tool creates a snapshot of the checksum of the installation files at the time, when the tool is being run.

Also, this tool will not create an entire valid setup script, only the [Files] and [Code] section, which then can be copied and pasted into the script that InnoSetup created, respectively you wrote by yourself.

If you have any questions, don't hesitate to contact me.
