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
      Assert.Equal(16, layout.LineOffset);
      Assert.Equal(17, layout.ControlCharactersOffset);
      Assert.Equal(32, layout.ControlCharactersLength);
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
      Assert.Equal(-1, layout.LineOffset);
      Assert.Equal(32, layout.ControlCharactersOffset);
      Assert.Equal(20, layout.ControlCharactersLength);
      Assert.Equal(56, layout.InputSpeedOffset);
      Assert.Equal(64, layout.OutputSpeedOffset);
      Assert.Equal(16, layout.VminIndex);
      Assert.Equal(17, layout.VtimeIndex);
    }

    [Fact]
    public void LinuxRawModeRestoresPseudoTerminalSettings()
    {
      if (!OperatingSystem.IsLinux())
        return;

      int master = -1;
      int slave = -1;
      try
      {
        Assert.Equal(0, OpenPtyLinux(out master, out slave, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero));
        CliRenderer.SetRawModeUnix(enable: true, fd: -1);
        CliRenderer.SetRawModeUnix(enable: false, fd: -1);

        AssertRawModeRoundTrips(
          slave,
          UnixTermiosLayout.Linux,
          localFlagsToClear: 0x8UL | 0x2UL | 0x1UL | 0x8000UL,
          inputFlagsToClear: 0x400UL | 0x100UL);
      }
      finally
      {
        if (slave >= 0)
          Close(slave);
        if (master >= 0)
          Close(master);
      }
    }

    [Fact]
    public void DarwinRawModeRestoresPseudoTerminalSettings()
    {
      if (!OperatingSystem.IsMacOS())
        return;

      int master = -1;
      int slave = -1;
      try
      {
        Assert.Equal(0, OpenPtyDarwin(out master, out slave, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero));
        CliRenderer.SetRawModeUnix(enable: true, fd: -1);
        CliRenderer.SetRawModeUnix(enable: false, fd: -1);

        AssertRawModeRoundTrips(
          slave,
          UnixTermiosLayout.Darwin,
          localFlagsToClear: 0x8UL | 0x100UL | 0x80UL | 0x400UL,
          inputFlagsToClear: 0x200UL | 0x100UL);
      }
      finally
      {
        if (slave >= 0)
          Close(slave);
        if (master >= 0)
          Close(master);
      }
    }

    private static void AssertRawModeRoundTrips(
      int fd,
      UnixTermiosLayout layout,
      ulong localFlagsToClear,
      ulong inputFlagsToClear)
    {
      var buffer = Marshal.AllocHGlobal(layout.Size);
      try
      {
        Assert.Equal(0, TcGetAttr(fd, buffer));
        var original = Snapshot(buffer, layout);

        try
        {
          CliRenderer.SetRawModeUnix(enable: true, fd);
          Assert.Equal(0, TcGetAttr(fd, buffer));

          Assert.Equal(0UL, ReadFlags(buffer, layout.LocalFlagsOffset, layout.FlagsWidth) & localFlagsToClear);
          Assert.Equal(0UL, ReadFlags(buffer, layout.InputFlagsOffset, layout.FlagsWidth) & inputFlagsToClear);
          Assert.Equal(1, Marshal.ReadByte(buffer, layout.ControlCharactersOffset + layout.VminIndex));
          Assert.Equal(0, Marshal.ReadByte(buffer, layout.ControlCharactersOffset + layout.VtimeIndex));
        }
        finally
        {
          CliRenderer.SetRawModeUnix(enable: false, fd);
        }

        Assert.Equal(0, TcGetAttr(fd, buffer));
        Assert.Equal(original, Snapshot(buffer, layout));

        CliRenderer.SetRawModeUnix(enable: false, fd);
        Assert.Equal(0, TcGetAttr(fd, buffer));
        Assert.Equal(original, Snapshot(buffer, layout));
      }
      finally
      {
        Marshal.FreeHGlobal(buffer);
      }
    }

    private static ulong ReadFlags(IntPtr buffer, int offset, int width)
      => width == sizeof(ulong)
        ? unchecked((ulong)Marshal.ReadInt64(buffer, offset))
        : unchecked((uint)Marshal.ReadInt32(buffer, offset));

    private static byte[] Snapshot(IntPtr buffer, UnixTermiosLayout layout)
    {
      int width = layout.FlagsWidth;
      int lineLength = layout.LineOffset >= 0 ? 1 : 0;
      var bytes = new byte[(width * 6) + lineLength + layout.ControlCharactersLength];
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

      if (layout.LineOffset >= 0)
      {
        Marshal.Copy(IntPtr.Add(buffer, layout.LineOffset), bytes, destinationOffset, 1);
        destinationOffset++;
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

    [DllImport("libutil.so.1", EntryPoint = "openpty", SetLastError = true)]
    private static extern int OpenPtyLinux(out int master, out int slave, IntPtr name, IntPtr termios, IntPtr winsize);

    [DllImport("libutil.dylib", EntryPoint = "openpty", SetLastError = true)]
    private static extern int OpenPtyDarwin(out int master, out int slave, IntPtr name, IntPtr termios, IntPtr winsize);

    [DllImport("libc", EntryPoint = "tcgetattr", SetLastError = true)]
    private static extern int TcGetAttr(int fd, IntPtr termios);

    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    private static extern int Close(int fd);
  }
}
