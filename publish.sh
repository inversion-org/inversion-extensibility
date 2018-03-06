#!/bin/bash

pushd Inversion.Extensibility/bin/Debug
  dotnet nuget push *.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json
popd

pushd Inversion.Extensibility.Web/bin/Debug
  dotnet nuget push *.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json
popd
