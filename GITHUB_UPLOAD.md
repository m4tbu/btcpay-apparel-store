# How to Upload This Project to GitHub

## Quick Method (Using GitHub Web Interface)

1. Go to GitHub.com and log in
2. Click the "+" icon in the top right, select "New repository"
3. Name it: `btcpay-apparel-store`
4. Description: "BTCPayServer plugin for selling dropshipped apparel with Bitcoin payments"
5. Choose "Public" (or Private if you prefer)
6. **DO NOT** initialize with README (we already have one)
7. Click "Create repository"
8. On the next page, click "uploading an existing file"
9. Drag and drop all the files from this project folder
10. Add commit message: "Initial commit: BTCPayServer Apparel Store plugin"
11. Click "Commit changes"

## Using Git Command Line

### Step 1: Install Git (if not installed)

**On Ubuntu/Debian:**
```bash
sudo apt-get update
sudo apt-get install git
```

**On macOS:**
```bash
brew install git
```

**On Windows:**
Download from https://git-scm.com/download/win

### Step 2: Configure Git (first time only)

```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

### Step 3: Initialize and Push

```bash
# Navigate to the project directory
cd btcpay-apparel-store

# Initialize git repository
git init

# Add all files
git add .

# Create initial commit
git commit -m "Initial commit: BTCPayServer Apparel Store plugin"

# Create repository on GitHub first (via web interface)
# Then add it as remote (replace YOUR-USERNAME):
git remote add origin https://github.com/YOUR-USERNAME/btcpay-apparel-store.git

# Push to GitHub
git branch -M main
git push -u origin main
```

When prompted for credentials:
- **Username:** Your GitHub username
- **Password:** Use a Personal Access Token (not your account password)

### Step 4: Create Personal Access Token (if needed)

1. Go to GitHub.com → Settings → Developer settings
2. Click "Personal access tokens" → "Tokens (classic)"
3. Click "Generate new token (classic)"
4. Give it a name: "BTCPay Apparel Store"
5. Select scope: `repo` (full control of private repositories)
6. Click "Generate token"
7. **Copy the token immediately** (you won't see it again)
8. Use this token as your password when pushing

## Using GitHub Desktop (Easiest for Beginners)

1. Download GitHub Desktop: https://desktop.github.com/
2. Install and log in to GitHub
3. Click "File" → "Add Local Repository"
4. Select the `btcpay-apparel-store` folder
5. Click "Create Repository" (if prompted)
6. Click "Publish repository" in the top right
7. Choose name: `btcpay-apparel-store`
8. Add description
9. Click "Publish Repository"

Done! Your project is now on GitHub.

## After Upload

Once your repository is on GitHub:

1. **Add Topics:** Click ⚙️ next to "About" on GitHub and add topics:
   - `btcpayserver`
   - `bitcoin`
   - `ecommerce`
   - `plugin`
   - `dropshipping`
   - `printful`

2. **Update Links:** Edit these files to add your GitHub repo URL:
   - README.md (line with GitHub URL)
   - btcpayserver-plugin.json (website field)

3. **Share:** Share your repository link with the BTCPayServer community!

## Troubleshooting

**Error: "refusing to merge unrelated histories"**
```bash
git pull origin main --allow-unrelated-histories
git push -u origin main
```

**Error: "remote origin already exists"**
```bash
git remote remove origin
git remote add origin https://github.com/YOUR-USERNAME/btcpay-apparel-store.git
```

**Need to change remote URL:**
```bash
git remote set-url origin https://github.com/YOUR-USERNAME/btcpay-apparel-store.git
```
