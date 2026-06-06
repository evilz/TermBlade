namespace TermBlade.FileManager;

internal interface IFileSystemOperations
{
  bool DirectoryExists(string path);
  bool FileExists(string path);
  IReadOnlyList<FileManagerEntry> ListEntries(string path);
  string GetParentPath(string path);
  string Combine(string path, string name);
  string GetFileName(string path);
  void OpenFile(string path);
  void CreateDirectory(string path);
  void CreateFile(string path);
  void Rename(string path, string newName);
  void Copy(string sourcePath, string destinationPath, bool overwrite);
  void Move(string sourcePath, string destinationPath, bool overwrite);
  void Delete(string path, bool recursive);
  string? ReadTextPreview(string path, int maxChars);
}
