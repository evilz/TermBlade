namespace TermBlade.FileManager;

internal enum ConfirmationKind
{
  Delete,
  Overwrite
}

internal sealed record ConfirmationRequest(ConfirmationKind Kind, string Message, Action Confirm);
