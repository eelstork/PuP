# PuP - Simple requirements for Unity projects

PuP lets you maintain a pip-styled requirements file in your Unity project. This allows sharing a common 'base' (requirements) while contributors also use their own, personal packages.

A typical use case is when team members maintain local dependencies.

## Installation

PuP requires Active Logic or Activ-LT:
https://assetstore.unity.com/packages/tools/ai/active-lt-behavior-trees-183959

## Setup

- Apply **Window > Activ > PuP > Freeze requirements**
- Remove *Packages/manifest.json* from version control.
- Add *Packages/Requirements.txt*

## Adding/removing packages

If you'd like team members to share a new package:

- Add the package to the requirements file and commit.
- Team members can apply requirements via **Window > Activ > PuP > Update requirements**

Team members may keep using the Unity Package Manager. Packages added via UPM will not be shared with team members (unless requirements are frozen and commited).

Note: currently version numbers are ignored.
