# SerialComm Monitor v1 Source Code

**SerialComm Monitor** is a serial communication application based on `SerialPort` class, built in C# WPF with MVVM design pattern to send (write) or receive (read) data string to/from a serial communication physical interface connected via RS232 or USB to the computer (Windows OS).

![Screenshot](http://i.imgur.com/FSliKIX.png)

## Downloads

Go to [**releases page**](https://github.com/heiswayi/SerialComm/releases) for more details.

## Prerequisites

- [Visual Studio IDE](https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx) (2013 and above)
- [Microsoft .NET Framework 4.5 (x86 and x64)](https://www.microsoft.com/en-us/download/details.aspx?id=30653)

## Dependencies

- [NLog](http://nlog-project.org/)

You can [download and install](http://nlog-project.org/download) it or just add it through [NuGet](https://www.nuget.org/profiles/jkowalski) in Visual Studio.

## Features

- Available serial port settings
  - Selectable COM* ports
  - Selectable baud rates
  - Selectable parities
  - Selectable data bits
  - Selectable stop bits
  - Enable/disable Data Terminal Ready (DTR)
  - Enable/disable Request To Send (RTS)
- Send (write) data string to serial port
- Receive or monitor (read) data sent from serial port
- Line ending options
- Autoscroll output box
- Dark Metro-like application skin

## TODO

- [ ] Auto populate COM* ports on refresh button
- [ ] Ability to save output data into a text file
- [ ] Add more settings; Handshake, CTS and DSR
- [ ] Ability to send data within interval time set
- [ ] _Still thinking..._

## Contributions

If you have any question regarding this project, or found a bug in the source code, feel free to [report it here](https://github.com/heiswayi/SerialComm/issues). All you need to do is [create an issue](https://github.com/heiswayi/SerialComm/issues/new) there.

To contribute to the code, **fork** this repository (`master` branch), realize your ideas, and then **create a new pull request**.

## License

Develop by [Heiswayi Nrird](http://heiswayi.github.io) and released as an open source software under [MIT license](LICENSE.md).
