dotnet publish --sc -r win-x64 -c Release -o ./publish/win-x64/
dotnet publish --sc -r win-x86 -c Release -o ./publish/win-x86/
dotnet publish --sc -r win-arm -c Release -o ./publish/win-arm/
dotnet publish --sc -r win-arm64 -c Release -o ./publish/win-arm64/

dotnet publish --sc -r linux-x64 -c Release -o ./publish/linux-x64/
dotnet publish --sc -r linux-musl-x64 -c Release -o ./publish/linux-musl-x64/
dotnet publish --sc -r linux-arm -c Release -o ./publish/linux-arm/
dotnet publish --sc -r linux-arm64 -c Release -o ./publish/linux-arm64/

dotnet publish --sc -r osx-x64 -c Release -o ./publish/osx-x64/