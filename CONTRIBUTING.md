# Contribution Rules

These are the set of rules that should be followed whenever you wish to contribute to this project. These are very important as they give consistency and order to the project therefore allowing people to collaborate and work together better.

[Code Guidelines](#code-guidelines)

[Git Guidelines](#git-guidelines)
  * [Commits](#commits)
  * [Branch Structure](#branch-structure)
  * [Issues](#issues)
  * [Pull Requests](#pull-requests)
  * [Correcting Mistakes](#correcting-mistakes)

[Contact Us](#contact-us)

## Code Guidelines

Though different languages have different styles we like to keep a general set of rules for all languages. This list is not all encompassing and develops as we work on different projects and face differences.

* Global variables and constants should use ALL CAPS.
* Private members and local variables should use camelCase.
* ID's, tags and such should use kebab-case.
* Function and Class names are in PascalCase.
* Keep all lines of code within 100 characters.
* Use descriptive variable names even if they end up being a little long, but not to long.
* If you must use abbreviations use comments to explain what they stand for.

## Git Guidelines

This is the workflow that all people working on this project must maintain. This provides structure and order to the repository and allows for better collaboration.

The workflow to be used is based on the Github workflow called the [Github Flow](https://guides.github.com/introduction/flow/) and other guidelines obtained from across the internet.

#### Commits

Make sure that the commit that you are making represents an "atomic" piece of work. Do not work on one large feature for a period of time and only commit once, this defeats the point of having a history of changes that you can rollback to. There is no way to measure an "atomic" commit, use your brilliant mind to figure that out.

You can find more information [here](http://chris.beams.io/posts/git-commit/).

```
Summarize changes in around 50 characters or less

More detailed explanatory text, if necessary. Wrap it to about 72
characters or so. In some contexts, the first line is treated as the
subject of the commit and the rest of the text as the body. The
blank line separating the summary from the body is critical (unless
you omit the body entirely); various tools like `log`, `shortlog`
and `rebase` can get confused if you run the two together.

Explain the problem that this commit is solving. Focus on why you
are making this change as opposed to how (the code explains that).
Are there side effects or other unintuitive consequences of this
change? Here's the place to explain them.

Further paragraphs come after blank lines.

 - Bullet points are okay, too

 - Typically a hyphen or asterisk is used for the bullet, preceded
   by a single space, with blank lines in between, but conventions
   vary here

If you use an issue tracker, put references to them at the bottom,
like this:

Resolves: #123
See also: #456, #789
```

#### Branch Structure

The following rules apply to the online repository only. Locally, you may use whatever branch structure you wish.

The `master` branch should always have a project that builds and has no half features. This is what would be considered deployable.

Features are worked on in separate branches. Each branch name should have a short and concise name that represents the feature being worked on in the branch such as `side-bar-nav` or `facial-detection`.

#### Issues

The Github issue tracker is a great way to keep track of the current bugs or problems in the project and any suggested changes.

Any solved issues should be referenced in the commit message as mentioned above.

Bug fixes should end up as single commits in the `master` branch, however if an issue gives rise to a feature it should be made into it's own branch as mentioned above.

To know more about the capabilities of the Github issue tracker go [here](https://guides.github.com/features/issues/).

#### Pull Requests

Pull requests are a feature of Github which allow you to ask for help or code review on a commit. Feel free to use them if you need them. You can also direct them at specific people or teams using the `@` sign.

You can read more about them [here](https://help.github.com/articles/about-pull-requests/).

#### Correcting Mistakes

Git is a very flexible and powerful piece of software. (virtually) Anything you do can be fixed or changed to another state. You only need to know how.

However, better be safe than sorry, so keep the guidelines above in mind while developing and you will reduce the amount of issues you have to deal with.

If you need to fix your commit history, Git provides ways to rewrite history. Atlassian provides a tutorial [here](https://www.atlassian.com/git/tutorials/rewriting-history) and official Git documentation can be found [here](https://git-scm.com/book/en/v2/Git-Tools-Rewriting-History). Github also have a webcast [here](https://www.youtube.com/watch?v=W39CfI3-JFc).

## Contact Us

If you have any questions, concerns or suggestions, feel free to contact us (see README.md).
