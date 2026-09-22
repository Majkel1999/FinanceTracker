#!/usr/bin/env bash
# Cloud Agent environment bootstrap for the FinanceTracker (.NET 5 WPF) solution.
#
# FinanceTracker is a Windows-only WPF desktop app (net5.0-windows, UseWPF).
# The GUI cannot run on the Linux Cloud Agent VM, but the whole solution can be
# COMPILED here (via EnableWindowsTargeting), and the non-WPF business logic can
# be EXECUTED on the installed .NET 5 runtime. This script is idempotent.
set -euo pipefail

DOTNET_DIR="$HOME/.dotnet"
DOTNET_INSTALL="/tmp/dotnet-install.sh"

echo ">>> Ensuring .NET install script is available"
curl -fsSL https://dot.net/v1/dotnet-install.sh -o "$DOTNET_INSTALL"
chmod +x "$DOTNET_INSTALL"

# 1. .NET 8 SDK — builds the net5.0-windows WPF solution on Linux.
if ! "$DOTNET_DIR/dotnet" --list-sdks 2>/dev/null | grep -q '^8\.'; then
  echo ">>> Installing .NET 8 SDK"
  "$DOTNET_INSTALL" --channel 8.0 --install-dir "$DOTNET_DIR"
else
  echo ">>> .NET 8 SDK already present"
fi

# 2. .NET 5 runtime — runs the (non-WPF) business logic / test harnesses.
if ! "$DOTNET_DIR/dotnet" --list-runtimes 2>/dev/null | grep -q 'Microsoft.NETCore.App 5\.'; then
  echo ">>> Installing .NET 5 runtime"
  "$DOTNET_INSTALL" --runtime dotnet --channel 5.0 --install-dir "$DOTNET_DIR"
else
  echo ">>> .NET 5 runtime already present"
fi

# 3. libssl1.1 — the .NET 5 runtime needs OpenSSL 1.1, absent on Ubuntu 24.04.
if ! ldconfig -p | grep -q 'libssl\.so\.1\.1'; then
  echo ">>> Installing libssl1.1 (required by the .NET 5 runtime)"
  tmp="$(mktemp -d)"
  curl -fsSL -o "$tmp/libssl1.1.deb" \
    http://security.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.1f-1ubuntu2.24_amd64.deb
  sudo dpkg -i "$tmp/libssl1.1.deb"
  rm -rf "$tmp"
else
  echo ">>> libssl1.1 already present"
fi

# 4. Make `dotnet` and sensible defaults available to every (login) shell.
echo ">>> Configuring PATH / environment"
sudo ln -sf "$DOTNET_DIR/dotnet" /usr/local/bin/dotnet
sudo tee /etc/profile.d/dotnet.sh >/dev/null <<'EOF'
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$PATH"
# WPF/WinForms projects target net5.0-windows; this lets them build on Linux.
export EnableWindowsTargeting=true
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
EOF

export DOTNET_ROOT="$DOTNET_DIR"
export PATH="$DOTNET_DIR:$PATH"
export EnableWindowsTargeting=true
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

# 5. Restore + compile the full solution (WPF app + tests) to prime caches.
echo ">>> Restoring and building FinanceTracker.sln"
dotnet restore FinanceTracker.sln
dotnet build FinanceTracker.sln --no-restore -c Debug

echo "<<< FinanceTracker environment ready"
