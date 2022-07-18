# Read only dependencies

# 1. Converting an embedded asset (a la Unity Asset Store) to a private package

Let's take an example; I have the Haon series under Haon-SD.
First we need to make a package.json.

Next manually create a repository; I choose gitlab for now.
I don't think github + unity is a combo for private repos. Gitlab still supports them.
So, let's be clear; this is a github issue, or a Unity issue but not a git issue.

Next we copy this directory to the locals, init a git repo and do an initial commit.

```
git init
git add --all
git commit -m "Initial commit"
git remote add origin https://gitlab.com/user/package.git
git push -u origin master
```

Large pushes tend to fail. But I managed to fix this (see "why can't I push")

## Why can't I push?

Okay so why can't I push on either git or gitlab? Gitlab has a troubleshooting section:

https://docs.gitlab.com/ee/topics/git/troubleshooting_git.html

The first idea for https is to increase the buffer size but uhm, this is for a clone? Maybe?

52,428,800  // 50 mb
100,000,000 // 100 mb

```
git config http.postBuffer 52428800
```

This is documented here: https://git-scm.com/docs/git-config
(page is a great reminder that *git moves files over the network*) however the doc itself appears to believe a large buffer usually won't fix issues. Well it fixed my issue.

They also encourage updating git. But I *seem* to be faring better with a larger buffer size.

Let's update Git anyway? I'm at 2.36 (who knows why!) whereas they recommend 2.9 ??? Well, maybe 9 < 36. Probably!
