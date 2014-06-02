@echo off
rundll32.exe dfshim.dll,ShArpMaintain VScan.application, Culture=neutral, PublicKeyToken=655b6746a6c17487, processorArchitecture=x86
msiexec /x {5E7F6921-33C9-4295-85AC-C4A1132BAB73}