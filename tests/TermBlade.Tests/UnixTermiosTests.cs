using System;
using System.Runtime.InteropServices;
using TermBlade.Core.Rendering;
using Xunit;

namespace TermBlade.Tests
{
  public class UnixTermiosTests
  {
    [Fact]
    public void LinuxTermiosLayoutMatchesAbi()
    {
      var layout = UnixTermiosLayout.Linux;

      Assert.Equal(60, layout.Size);
      Assert.Equal(0, layout.InputFlagsOffset);
      Assert.Equal(12, layout.LocalFlagsOffset);
      Assert.Equal(4, layout.FlagsWidth);
      Assert.Equal(17, layout.ControlCharactersOffset);
      Assert.Equal(52, layout.InputSpeedOffset);
      Assert.Equal(56, layout.OutputSpeedOffset);
      Assert.Equal(6, layout.VminIndex);
      Assert.Equal(5, layout.VtimeIndex);
    }

    [Fact]
    public void DarwinTermiosLayoutMatchesAbi()
    {
      var layout = UnixTermiosLayout.Darwin;

      Assert.Equal(72, layout.Size);
      Assert.Equal(0, layout.InputFlagsOffset);
      Assert.Equal(24, layout.LocalFlagsOffset);
      Assert.Equal(8, layout.FlagsWidth);
      Assert.Equal(32, layout.ControlCharactersOffset);
      Assert.Equal(56, layout.InputSpeedOffset);
      Assert.Equal(64, layout.OutputSpeedOffset);
      Assert.Equal(16, layout.VminIndex);
      Assert.Equal(17, layout.VtimeIndex);
    }

    [Fact]
    public void DarwinRawModeRestoresPseudoTerminalSettings()
    {
      if (!OperatingSystem.IsMacOS())
        return;

      int master = -1;
      int slave = -1;
      IntPtr buffer = IntPtr.Zero;
      try
      {
        Assert.Equal(0, OpenPty(out master, out slave, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero));
        var layout = UnixTermiosLayout.Darwin;
        buffer = Marshal.AllocHGlobal(layout.Size);

        Assert.Equal(0, TcGetAttr(slave, buffer));
        var original = Snapshot(buffer, layout.Size);

        try
        {
          CliRenderer.SetRawModeUnix(enable: true, fd: slave);

          Assert.Equal(0, TcGetAttr(slave, buffer));
          ulong localFlags = unchecked((ulong)Marshal.ReadInt64(buffer, layout.LocalFlagsOffset));
          ulong inputFlags = unchecked((ulong)Marshal.ReadInt64(buffer, layout.InputFlagsOffset));
          Assert.Equal(0UL, localFlags & (0x8UL | 0x100UL | 0x80UL | 0x400UL));
          Assert.Equal(0UL, inputFlags & (0x200UL | 0x100UL));
          Assert.Equal(1, Marshal.ReadByte(buffer, layout.ControlCharactersOffset + layout.VminIndex));
          Assert.Equal(0, Marshal.ReadByte(buffer, layout.ControlCharactersOffset + layout.VtimeIndex));
        }
        finally
        {
          CliRenderer.SetRawModeUnix(enable: false, fd: slave);
        }

        Assert.Equal(0, TcGetAttr(slave, buffer));
        Assert.Equal(original, Snapshot(buffer, layout.Size));

        CliRenderer.SetRawModeUnix(enable: false, fd: slave);
        Assert.Equal(0, TcGetAttr(slave, buffer));
        Assert.Equal(original, Snapshot(buffer, layout.Size));
      }
      finally
      {
        if (buffer != IntPtr.Zero)
          Marshal.FreeHGlobal(buffer);
        if (slave >= 0)
          Close(slave);
        if (master >= 0)
          Close(master);
      }
    }

    private static byte[] Snapshot(IntPtr buffer, int size)
    {
      var bytes = new byte[size];
      Marshal.Copy(buffer, bytes, 0, size);
      return bytes;
    }

    [DllImport("libutil.dylib", EntryPoint = "openpty", SetLastError = true)]
    private static extern int OpenPty(out int master, out int slave, IntPtr name, IntPtr termios, IntPtr winsize);

    [DllImport("libc", EntryPoint = "tcgetattr", SetLastError = true)]
    private static extern int TcGetAttr(int fd, IntPtr termios);

    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    private static extern int Close(int fd);
  }
}
