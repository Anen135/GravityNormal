# Unity Framework Integration Guide

## Adding the Framework to Your Project

This framework must be added as a Git submodule. Do not use `git clone` or copy files manually, as this breaks version control and updates.

### Adding as Submodule

From your Unity project root:

```
git submodule add -b main https://github.com/your-org/your-framework.git Packages/com.yourname.framework
```

Commit the changes:

```
git add .gitmodules Packages/com.yourname.framework
git commit -m "Add framework as submodule"
```

## Cloning a Project with the Framework

```
git submodule update --init --recursive
```

Or clone with submodules:

```
git clone --recurse-submodules <repo-url>
```

## Updating the Framework

```
cd Packages/com.yourname.framework
git checkout main
git pull
cd ../..
git add Packages/com.yourname.framework
git commit -m "Update framework to latest version"
```

## Important Notes

- The framework is a separate Git repository; the project stores only a reference (commit).
- Always commit submodule updates.
- Avoid direct modifications unless necessary.

## Why Use Submodules?

- Ensures consistent framework version across developers.
- Simplifies updates via Git.
- Maintains clean separation of project and framework code.

## Troubleshooting

**Empty framework folder after cloning:**

```
git submodule update --init --recursive
```

**Detached HEAD in framework:**

```
cd Packages/com.yourname.framework
git checkout main
```

## Folder Structure

```
Packages/
  com.yourname.framework/   # Git submodule
```

## Team Workflow

1. Framework developed in its own repository.
2. Project references via submodule.
3. Pull updates and commit explicitly.

## What NOT to Do

- Do not `git clone` the framework into the project.
- Do not copy-paste framework files.
- Do not use local paths in shared repositories.

