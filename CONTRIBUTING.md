# Contributing to PrettyScreenSHOT

Thank you for your interest in contributing to PrettyScreenSHOT! 🎉

## How You Can Help

### 🐛 Reporting Bugs
If you found a bug:
1. Check if the bug hasn't been reported already in [Issues](https://github.com/Co0ob1iee/PrettyScreenSHOT/issues)
2. Create a new issue describing:
   - Step-by-step reproduction
   - Expected behavior
   - Actual behavior
   - OS version
   - Application version

### 💡 Feature Proposals
Have an idea for a new feature?
1. Check [ROADMAP.md](ROADMAP.md) - it might already be planned
2. Create an issue with "enhancement" label
3. Describe the feature and its usefulness in detail

### 🔧 Pull Requests

#### Process
1. **Fork** the repository
2. **Create a branch** for your feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** changes (`git commit -m 'Add some AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open a Pull Request**

#### Code Guidelines
- **Formatting**: Use default Visual Studio formatting or `.editorconfig` settings
- **Naming**:
  - Classes: PascalCase (`ScreenshotManager`)
  - Methods: PascalCase (`CaptureScreenshot`)
  - Variables: camelCase (`captureArea`)
  - Constants: UPPER_CASE (`MAX_SCROLLS`)
- **Comments**: Comment complex logic in English
- **Localization**: All UI texts must be in `.resx` files
- **Testing**: Test your feature before PR

#### Commit Structure
```
feat: Add video capture feature
fix: Fix Scroll Capture bug
docs: Update README
refactor: Refactor SecurityManager
style: Code formatting
perf: Optimize cache performance
```

### 🌍 Localization
If adding new UI texts:
1. Add key to all `.resx` files:
   - `Properties/Resources.resx` (English)
   - `Properties/Resources.pl.resx` (Polish)
   - `Properties/Resources.de.resx` (German)
   - `Properties/Resources.zh.resx` (Chinese)
   - `Properties/Resources.fr.resx` (French)
2. Use `LocalizationHelper.GetString("Key")` in code

### 🧪 Testing
Before submitting PR:
- [ ] Code compiles without errors
- [ ] Feature works correctly
- [ ] No regression in existing features
- [ ] Code follows guidelines
- [ ] All texts are localized

### 📝 Documentation
If adding a new feature:
- Update `README.md` if needed
- Add XML comments to public methods
- Update `ROADMAP.md` if feature was planned

## Questions?
If you have questions, open an issue with "question" label or use [Discussions](https://github.com/Co0ob1iee/PrettyScreenSHOT/discussions).

## License
By submitting a PR, you agree that your code will be licensed under GNU GPL v3.

---

**Thank you for contributing to PrettyScreenSHOT!** 🙏
