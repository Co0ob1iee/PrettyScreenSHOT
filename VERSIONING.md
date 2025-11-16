# Versioning Scheme

PrettyScreenSHOT uses a custom semantic versioning scheme optimized for continuous integration and deployment.

## Version Format

The version follows the format: **MAJOR.MINOR.PATCH**

However, we use specific values for each component to indicate the type of build:

### 🔹 Development Builds: `0.0.X`

**Format:** `0.0.<BUILD_NUMBER>`

- **Purpose:** Continuous integration builds from every commit
- **Build Number:** Automatically incremented by CI/CD (GitHub Actions run number)
- **Examples:** `0.0.1`, `0.0.2`, `0.0.42`, `0.0.150`
- **Trigger:** Automatic on every push to branches
- **Artifacts:** Available as CI build artifacts

**Usage:**
```bash
# These are created automatically by CI/CD
# No manual tagging required
```

---

### 🔸 Pre-release/Beta: `0.X.0`

**Format:** `0.<RELEASE_NUMBER>.0`

- **Purpose:** Beta releases and pre-release versions for testing
- **Release Number:** Manually incremented for each beta release
- **Examples:** `0.1.0`, `0.2.0`, `0.5.0`
- **Trigger:** Git tag with format `v0.X.0`
- **Artifacts:** GitHub Release (marked as pre-release)

**Usage:**
```bash
# Create a beta release
git tag v0.1.0
git push origin v0.1.0

# The workflow will:
# - Build the release
# - Create ZIP archives
# - Generate checksums
# - Create GitHub Release (pre-release)
```

---

### 🔶 Production Release: `X.0.0`

**Format:** `<MAJOR_VERSION>.0.0`

- **Purpose:** Stable production releases
- **Major Version:** Incremented for major releases (1.0.0, 2.0.0, 3.0.0, etc.)
- **Examples:** `1.0.0`, `2.0.0`, `3.0.0`
- **Trigger:** Git tag with format `vX.0.0`
- **Artifacts:** GitHub Release (full release)

**Usage:**
```bash
# Create a production release
git tag v1.0.0
git push origin v1.0.0

# The workflow will:
# - Build the release
# - Create ZIP archives
# - Generate checksums
# - Create GitHub Release (production)
```

---

## Version Determination

The CI/CD pipeline automatically determines the version type:

| Pattern | Type | Pre-release | Example |
|---------|------|-------------|---------|
| `X.0.0` (X ≥ 1) | Production | ❌ No | `1.0.0`, `2.0.0` |
| `0.X.0` (X ≥ 1) | Beta/Pre-release | ✅ Yes | `0.1.0`, `0.2.0` |
| `0.0.X` | Development | ✅ Yes | `0.0.42`, `0.0.100` |

---

## Implementation Details

### Project File (`.csproj`)

The base version in the project file is set to `0.0.0`:

```xml
<AssemblyVersion>0.0.0</AssemblyVersion>
<FileVersion>0.0.0</FileVersion>
<Version>0.0.0</Version>
```

This is overridden at build time by the CI/CD pipeline.

### GitHub Actions

#### Development Builds (`.github/workflows/dotnet.yml`)

```yaml
- name: Set development build version
  run: |
    $buildNumber = "${{ github.run_number }}"
    $version = "0.0.$buildNumber"
    echo "BUILD_VERSION=$version" >> $env:GITHUB_ENV

- name: Build solution
  run: dotnet build /p:Version=${{ env.BUILD_VERSION }}
```

#### Release Builds (`.github/workflows/release.yml`)

```yaml
- name: Extract and validate version
  run: |
    $version = "${{ github.ref_name }}".TrimStart('v')
    # Automatically determines if production, beta, or dev
```

---

## Examples

### Timeline Example

```
0.0.1  ← First development build
0.0.2  ← Second development build
...
0.0.42 ← 42nd development build
0.1.0  ← First beta release
0.0.43 ← Development continues...
0.0.50 ← More development
0.2.0  ← Second beta release
0.0.51 ← Development continues...
1.0.0  ← First production release! 🎉
1.0.1  ← Patch release (if needed)
0.0.52 ← Development for next version
2.0.0  ← Second major release
```

### Creating Releases

```bash
# Development builds - automatic, no action needed
# Every push triggers a 0.0.X build

# Beta release
git tag v0.1.0 -m "First beta release"
git push origin v0.1.0

# Production release
git tag v1.0.0 -m "First production release"
git push origin v1.0.0
```

---

## Rationale

This versioning scheme provides:

1. **Clear Build Tracking** - Every CI build has a unique version
2. **Automatic Versioning** - No manual intervention for dev builds
3. **Pre-release Identification** - Clear distinction between beta and production
4. **Semantic Meaning** - Version number indicates release type at a glance
5. **CI/CD Integration** - Works seamlessly with GitHub Actions

---

## Migration from Previous Versions

If upgrading from an older versioning scheme:

1. **Current version `0.0.1`** becomes the starting point
2. Next development build will be `0.0.<NEXT_BUILD_NUMBER>`
3. First beta release should be `0.1.0`
4. First production release should be `1.0.0`

---

## FAQs

### Q: Why not standard semantic versioning (1.2.3)?

**A:** Standard semver doesn't clearly distinguish between development builds, beta releases, and production releases. Our scheme makes this distinction explicit and automatic.

### Q: What if I need a patch release?

**A:** For production releases, use standard semver: `1.0.1`, `1.0.2`, etc. The pipeline will detect this as a custom version.

### Q: Can I use alpha or rc tags?

**A:** Yes! For tags like `v0.1.0-alpha` or `v1.0.0-rc1`, the pipeline will automatically mark them as pre-releases.

### Q: How do I know what version I'm running?

**A:** Check the executable properties or use `--version` flag (if implemented). The version is embedded during build.

---

## References

- [Semantic Versioning 2.0.0](https://semver.org/)
- [GitHub Actions - Expressions](https://docs.github.com/en/actions/learn-github-actions/expressions)
- [.NET Version Properties](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props)

---

**Last Updated:** 2025-11-16
**Scheme Version:** 1.0
**Status:** Active
