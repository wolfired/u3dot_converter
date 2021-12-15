u3dot_converter
===============

## Settings for Pack

```xml
  <PropertyGroup>
    <PackageId>wolfired.u3dot_converter</PackageId>
    <Version>1.0.1</Version>
    <Authors>LinkWu</Authors>
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>u3dot_converter</ToolCommandName>
    <PackageOutputPath>./bin</PackageOutputPath>
  </PropertyGroup>
```

## New, Build, Run, Delete, Upload

```bash

# New
dotnet new console \
&& rm -rf ./*.cs \
&& dotnet add . package Mono.Options

dotnet new sln \
&& dotnet sln add .

# Build
dotnet build

# Run
dotnet run -- -h

# Delete
rm -rf ./{bin,obj,sample} ./*.{csproj,sln}

# Upload
dotnet pack -c Release

dotnet nuget push \
./bin/wolfired.u3dot_converter.1.0.2.nupkg \
-k APIKEY \
-s https://api.nuget.org/v3/index.json

```

## Example

```bash
dotnet build -o bin

rm -rf sample && dotnet new classlib --framework 'netstandard2.0' -o sample

./bin/u3dot_converter \
--cfsrc=./resource/unity_project_files/Assembly-CSharp.csproj \
--nssrc=http://schemas.microsoft.com/developer/msbuild/2003 \
--cfdst=./sample/sample.csproj
```
