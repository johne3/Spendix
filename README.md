# Spendix

### Release and Deployment
We use Git tags to automatically create releases and deploy to Azure.

1. Make sure the dev branch is selected.
2. Navigate to the project folder with a command line tool.
3. Create the tag with the Git command line tool with version v1.0.xx, latest version can be found in GitHub Releases.
```
git tag v1.0.89
```
4. Push the tag to GitHub
```
git push origin --tags
```

This will trigger the Create Release and Deploy GitHub action workflow.
