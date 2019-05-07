#!/bin/bash

if [[ "$TRAVIS_BRANCH" = "master" ]] && [[ "$TRAVIS_PULL_REQUEST" = "false" ]]; then

	pushd Inversion.Extensibility/bin/Debug
	  dotnet nuget push *.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json || exit 1
	popd

	pushd Inversion.Extensibility.Web/bin/Debug
	  dotnet nuget push *.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json || exit 1
	popd
fi

