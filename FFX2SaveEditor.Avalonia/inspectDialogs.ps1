$path = 'C:\Users\terry\.nuget\packages\avalonia.desktop\12.0.5\lib\net8.0\Avalonia.Dialogs.dll'
if (-Not (Test-Path $path)) {
    Write-Host "Missing assembly: $path"
    exit 1
}
$asm = [System.Reflection.Assembly]::LoadFrom($path)
$asm.GetExportedTypes() | Where-Object { $_.Name -like '*FileDialog*' } | Select-Object FullName, Namespace | Format-Table -AutoSize
