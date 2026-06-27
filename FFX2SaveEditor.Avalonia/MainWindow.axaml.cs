using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FFX2SaveEditor;
using System.IO;

namespace FFX2SaveEditor.Avalonia;

public partial class MainWindow : Window
{
    private PcSave? save;
    private string? loadedFileName;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OpenPcButton_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            StatusText.Text = "Unable to open file dialog.";
            return;
        }

        var options = new FilePickerOpenOptions
        {
            Title = "Open PC Save",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("PC Save Data")
                {
                    Patterns = new[] { "*" }
                }
            }
        };

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);
        if (files == null || files.Count == 0)
            return;

        try
        {
            loadedFileName = files[0].Name ?? "PC Save";
            await using var stream = await files[0].OpenReadAsync();
            save = new PcSave(stream);
            SavePcButton.IsEnabled = true;
            LoadedFileText.Text = loadedFileName;
            SaveInfoText.Text = $"Loaded PC save with Gil={save.Gil}, Time={save.GameTime}, Character Levels={save.Characters[0].Level},{save.Characters[1].Level},{save.Characters[2].Level}";
            StatusText.Text = "PC save loaded successfully.";
        }
        catch (System.Exception ex)
        {
            StatusText.Text = $"Error loading save: {ex.Message}";
        }
    }

    private async void SavePcButton_Click(object? sender, RoutedEventArgs e)
    {
        if (save == null)
            return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            StatusText.Text = "Unable to open save dialog.";
            return;
        }

        var options = new FilePickerSaveOptions
        {
            Title = "Save PC Save",
            SuggestedFileName = loadedFileName,
            FileTypeChoices = new[]
            {
                new FilePickerFileType("PC Save Data")
                {
                    Patterns = new[] { "*" }
                }
            }
        };

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(options);
        if (file is null)
            return;

        try
        {
            await using var writeStream = await file.OpenWriteAsync();
            save.SaveFile(writeStream);
            StatusText.Text = "Save written successfully.";
        }
        catch (System.Exception ex)
        {
            StatusText.Text = $"Error saving file: {ex.Message}";
        }
    }
}
