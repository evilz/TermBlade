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
      Assert.Equal(20, layout.ControlCharactersLength);
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
        var original = Snapshot(buffer, layout);

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
        Assert.Equal(original, Snapshot(buffer, layout));

        CliRenderer.SetRawModeUnix(enable: false, fd: slave);
        Assert.Equal(0, TcGetAttr(slave, buffer));
        Assert.Equal(original, Snapshot(buffer, layout));
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

    private static byte[] Snapshot(IntPtr buffer, UnixTermiosLayout layout)
    {
      int width = layout.FlagsWidth;
      var bytes = new byte[(width * 6) + layout.ControlCharactersLength];
      int destinationOffset = 0;

      for (int field = 0; field < 4; field++)
      {
        Marshal.Copy(
          IntPtr.Add(buffer, layout.InputFlagsOffset + (field * width)),
          bytes,
          destinationOffset,
          width);
        destinationOffset += width;
      }

      Marshal.Copy(
        IntPtr.Add(buffer, layout.ControlCharactersOffset),
        bytes,
        destinationOffset,
        layout.ControlCharactersLength);
      destinationOffset += layout.ControlCharactersLength;

      Marshal.Copy(IntPtr.Add(buffer, layout.InputSpeedOffset), bytes, destinationOffset, width);
      destinationOffset += width;
      Marshal.Copy(IntPtr.Add(buffer, layout.OutputSpeedOffset), bytes, destinationOffset, width);
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
