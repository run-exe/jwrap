#! /usr/bin/env bash
set -uvx
set -e
rm -rf bin obj
dotnet build -c Release jwrap.csproj
cp -rp bin/Release/net462/jwrap.exe $HOME/cmd/
dotnet build -c Release jwrapw.csproj
cp -rp bin/Release/net462/jwrapw.exe $HOME/cmd/
