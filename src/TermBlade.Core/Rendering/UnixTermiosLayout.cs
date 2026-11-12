using System.Runtime.InteropServices;

namespace TermBlade.Core.Rendering;

internal readonly record struct UnixTermiosLayout(
  int Size,
  int InputFlagsOffset,
  int LocalFlagsOffset,
  int FlagsWidth,
  int ControlCharactersOffset,
  int InputSpeedOffset,
  int OutputSpeedOffset,
  int VminIndex,
  int VtimeIndex)
{
  internal static UnixTermiosLayout Linux { get; } = new(
    Marshal.SizeOf<LinuxTermios>(),
    OffsetOf<LinuxTermios>(nameof(LinuxTermios.InputFlags)),
    OffsetOf<LinuxTermios>(nameof(LinuxTermios.LocalFlags)),
    sizeof(uint),
    OffsetOf<LinuxTermios>(nameof(LinuxTermios.ControlCharacters)),
    OffsetOf<LinuxTermios>(nameof(LinuxTermios.InputSpeed)),
    OffsetOf<LinuxTermios>(nameof(LinuxTermios.OutputSpeed)),
    6,
    5);

  internal static UnixTermiosLayout Darwin { get; } = new(
    Marshal.SizeOf<DarwinTermios>(),
    OffsetOf<DarwinTermios>(nameof(DarwinTermios.InputFlags)),
    OffsetOf<DarwinTermios>(nameof(DarwinTermios.LocalFlags)),
    sizeof(ulong),
    OffsetOf<DarwinTermios>(nameof(DarwinTermios.ControlCharacters)),
    OffsetOf<DarwinTermios>(nameof(DarwinTermios.InputSpeed)),
    OffsetOf<DarwinTermios>(nameof(DarwinTermios.OutputSpeed)),
    16,
    17);

  internal static UnixTermiosLayout ForPlatform(bool isDarwin)
    => isDarwin ? Darwin : Linux;

  private static int OffsetOf<T>(string fieldName) where T : struct
    => Marshal.OffsetOf<T>(fieldName).ToInt32();

  [StructLayout(LayoutKind.Sequential)]
  internal struct LinuxTermios
  {
    public uint InputFlags;
    public uint OutputFlags;
    public uint ControlFlags;
    public uint LocalFlags;
    public byte Line;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] ControlCharacters;
    public uint InputSpeed;
    public uint OutputSpeed;
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct DarwinTermios
  {
    public ulong InputFlags;
    public ulong OutputFlags;
    public ulong ControlFlags;
    public ulong LocalFlags;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] ControlCharacters;
    public ulong InputSpeed;
    public ulong OutputSpeed;
  }
}
