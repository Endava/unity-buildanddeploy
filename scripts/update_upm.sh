#!/bin/bash

# set working directory to shell script
cd "$(dirname "$0")"

cd ..

# delete old upm branch 
git branch -d upm &> /dev/null || echo upm branch not found

# split repo and use package folder root as upm content
PKG_ROOT="./Packages/com.endava.buildanddeploy"
git subtree split -P "$PKG_ROOT" -b upm

# checkout upm branch
git checkout upm

# change upm branch by rename Samples folder
if [[ -d "Samples" ]]; then
    git mv Samples Samples~
    rm -f Samples.meta
fi

# push the brandnew upm step
git push -f -u origin upm