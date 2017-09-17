How to contribute
=====================

Contributions are most welcome. Read the following guide to get to know what you need to know.

# Reporting bugs.

* Browse existing issues to make sure that it hasn't been reported yet.
* Create a new issue
  * Clearly describe the issue including steps to reproduce when it is a bug.
  * If it is a bug make sure you tell us what version you have encountered this bug on.

  
# Contributing

## Finding something do to.

Browse all issues (bugs and feature requests). Find something that you would like to help with. Comment the issue and say that you are working on it.

If you would like to contribute with something that doesn't exist, create a new issue and then comment on it.

## Making changes

* Create a feature branch from where you want to base your work.
  * From the develop branch since we never do any work off our master branch.
  * Only target release branches if you are certain your fix must be on that
      branch.
* Make commits of logical units (i.e. working features).
* Check for unnecessary whitespace with `git diff --check` before committing.
* Write meaningful commit messages.
* Make sure you have added the necessary tests for your changes (unit and integration tests).
* Make sure that you're "all green in reshaper" (or make sure that you have formatted your code reasonable well).

## Tests.

There are not that many unit tests yet, however our ultimate goal is to have complete coverage. Therefore all contributions must be covered by tests.

* Repositories should have integration tests where the queries and mappings are tested.
* Business logic should be covered by unit tests

### Guidelines

* Use Xunit, NSubtitute and FluentAssertions
* Follow the AAA pattern with one empty line as seperator
* Method names should explain the business rule being tested (i.e. "should_not_be_possible_to_login_into_a_locked_account")
* Aim at only one or mostly two asserts per test
* Factory methods should create objects for the working case, modify it after when you test the exceptional cases.

## Submitting Changes

* Push your changes to a feature branch in your fork of the repository.
* Submit a pull request to this repository
* Accept the CLA in your PR.

# Additional Resources

* [General GitHub documentation](http://help.github.com/)
* [GitHub pull request documentation](http://help.github.com/send-pull-requests/)
