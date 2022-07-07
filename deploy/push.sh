#!/bin/sh

key=`cat ./nuget-key.secret.txt`

for package in ../out/*.nupkg; do
    dotnet nuget push $package -k $key --source https://api.nuget.org/v3/index.json --skip-duplicate
done
