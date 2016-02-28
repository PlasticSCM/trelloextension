# Plastic SCM Trello extension

This library enables [Trello](https://trello.com) tasks information to be displayed on [Plastic SCM](https://plasticscm.com), the distributed version control system. The implementation followed the steps detailed at the [Plastic SCM custom extensions guide](https://www.plasticscm.com/documentation/extensions/plastic-scm-version-control-task-and-issue-tracking-guide.shtml#WritingPlasticSCMcustomextensions).

## Building
The only current build method is using Visual Studio or MSBuild. To compile the solution just run the following command:
```
MSBuild.exe "C:\path\to\your\trelloextension\workspace\src\TrelloExtension.sln"
```
The Trello extension (`trelloextension.dll`) and all its required libraries will be found at `C:\path\to\your\trelloextension\workspace\bin`.

## Add to Plastic SCM
Edit the custom extensions file (found at `C:\Program Files\PlasticSCM5\client\customextensions.conf`) and add a line as follows:
```
Trello=C:\path\to\your\trelloextension\workspace\bin\trelloextension.dll
```
Once the Plastic SCM client is restarted you'll find a __Trello__ entry on the __Issue tracking__ preferences dialog tab.

## Configuration
First of all, start the Plastic SCM client and open the __Preferences__ window. Once it's displayed, open the __Issue tracking__ tab and configure your repository to use __Trello__ as issue tracker information provider. Take a look at the [Plastic SCM extensions guide](https://www.plasticscm.com/documentation/extensions/plastic-scm-version-control-task-and-issue-tracking-guide.shtml) for further details.

After that, you'll notice a textbox labeled _Login URL_. You'll need to copy that URL and open it in your browser. When the page is loaded, you'll be prompted with a Trello login screen. As soon as you're logged in, a plain page will be shown indicating an authorization token. Copy it and go back to the Plastic SCM issue tracker configuration screen.

Now, Plastic SCM is authorized to read and write your Trello cards. Paste the token you just got into the _API token_ textbox.

If you only need tasks to be read from a single board, open the board you want to use as reference and copy its URL as displayed by your browser. Copy that URL into the _Board URL_ textbox.

You're done! Click OK and reopen any branch explorer/branches/changesets view you might have displayed on your Plastic SCM client.

## Dependencies
This library uses [Trello.NET](https://github.com/chrisdw/Trello.NET/) to interact with the [Trello REST API](https://developers.trello.com/advanced-reference/). You might have noticed that I didn't link [the original Trello.Net GitHub repository](https://github.com/dillenmeister/Trello.NET); this is because the Trello model has increased the expected number of colors available to a tag. Take a look at issue [#70](https://github.com/dillenmeister/Trello.NET/issues/70).

For now I'll be using Trello.NET compiled from commit [`1dff677ef35aa95f30d251c200a65edc762b786d`](https://github.com/chrisdw/Trello.NET/commit/1dff677ef35aa95f30d251c200a65edc762b786d) at repo https://github.com/chrisdw/Trello.NET. User [chrisdw](https://github.com/chrisdw) opened a [pull request](https://github.com/dillenmeister/Trello.NET/pull/58) to fix issue [#70](https://github.com/dillenmeister/Trello.NET/issues/70), which was blocking to me.

Also, as required by Plastic SCM, two libraries are needed: `issuetrackerinterface.dll` and `log4net.dll`. You can found them at `C:\Program Files\PlasticSCM5\client` once you install Plastic SCM.

## Pending improvements
1. Add some tests!
2. Improve login/authorization process.
3. Link Trello.NET as Xlink/submodule as soon as the pull request gets merged.
4. Think of a better way to reference cards when no panel is selected
5. Use OAuth?