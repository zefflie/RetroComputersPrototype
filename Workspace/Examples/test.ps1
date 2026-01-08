msbuild ../../Tela.slnx

../../Nyassembler/bin/Debug/net10.0/Nyassembler.exe test.nya -o test.out
Start-Process powershell -ArgumentList '-NoExit','-Command', "../../Emulator/bin/Debug/net10.0/Emulator.exe -f test.out"