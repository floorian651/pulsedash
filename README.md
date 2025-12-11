# wave project

A scaffolded software project repository with MkDocs documentation, Material theme, and automated GitHub Pages deployment.

## 🚀 Quick Start

### Manual Setup

If you prefer to set up manually:

1. **Install dependencies:**
   ```bash
   pip install mkdocs-material
   ```

2. **Preview locally:**
   ```bash
   mkdocs serve
   ```
   Visit http://127.0.0.1:8000 in your browser

3. **Build the site:**
   ```bash
   mkdocs build
   ```

4. **Deploy to GitHub Pages:**
   ```bash
   mkdocs gh-deploy
   ```
   Or simply push to the main branch - GitHub Actions will deploy automatically!

## 📂 Project Structure

```
.
├── src/                        # Source code
├── docs/                       # Documentation
│   ├── assets/                 # Static assets (images, CSS, etc.)
│   ├── dev/                    # Development guides
│   │   └── conventions.md      # Gitflow & commit conventions
│   ├── arch/                   # Architecture documentation
│   └── index.md                # Homepage (dashboard)
├── .github/
│   └── workflows/
│       └── docs.yml            # GitHub Actions deployment workflow
├── mkdocs.yml                  # MkDocs configuration
└── README.md                   # This file
```

## 🔧 Configuration

The `mkdocs.yml` configuration includes:

- **Theme**: Material for MkDocs
- **Language**: French (fr)
- **Features**:
  - Navigation tabs (sticky)
  - Navigation sections
  - Navigation expand
  - Back to top button
  - TOC following
  - Search suggestions
  - Search highlighting

## 🚢 Deployment

### Automatic Deployment

Pushing to the `main` or `master` branch automatically triggers the GitHub Actions workflow that:
1. Checks out the code
2. Sets up Python
3. Installs MkDocs and dependencies
4. Builds the documentation
5. Deploys to the `gh-pages` branch

### Manual Deployment

```bash
mkdocs gh-deploy --force
```

## 📖 Documentation Pages

- **Home** (`docs/index.md`) - Dashboard-style overview with quick links
- **Development Conventions** (`docs/dev/conventions.md`) - Comprehensive guide covering:
  - Git Flow workflow
  - Conventional Commits specification
  - Branch naming conventions
  - Pull request guidelines
  - Code review best practices

## 🛠️ Customization

### Adding New Pages

1. Create a markdown file in the `docs/` directory
2. Add it to the `nav` section in `mkdocs.yml`
3. Build or serve to see changes

### Changing Theme Settings

Edit `mkdocs.yml` to customize:
- Colors and fonts
- Navigation features
- Markdown extensions
- Plugins

## 📝 License

This is a test project for MkDocs workflow testing.

