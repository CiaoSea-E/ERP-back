$ERP_REPO = $PSScriptRoot
$APP_FILES_DIR = Join-Path $ERP_REPO "All\Application Files"
$ALL_ALL = Join-Path $ERP_REPO "All"

New-Item -ItemType Directory -Path $APP_FILES_DIR -Force | Out-Null
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function Get-Digest([string]$filePath) {
    $sha = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.IO.File]::ReadAllBytes($filePath)
    $hash = $sha.ComputeHash($bytes)
    return @{
        Digest = [Convert]::ToBase64String($hash)
        Size = $bytes.Length
    }
}

function Get-AsmInfo([string]$filePath) {
    try {
        $an = [System.Reflection.AssemblyName]::GetAssemblyName($filePath)
        $pktBytes = $an.GetPublicKeyToken()
        $pkt = if ($pktBytes -and $pktBytes.Length -gt 0) { 
            [System.BitConverter]::ToString($pktBytes).Replace("-","") 
        } else { 
            $null 
        }
        return @{
            Name = $an.Name
            Version = $an.Version.ToString()
            PKT = $pkt
        }
    } catch {
        $base = [System.IO.Path]::GetFileNameWithoutExtension($filePath)
        return @{
            Name = $base
            Version = "1.0.0.0"
            PKT = $null
        }
    }
}

function Package-ClickOnceApp([string]$appName, [string]$folderName, [string]$appVersion, [string]$srcDir, [string]$mainExe) {
    $targetDir = Join-Path $APP_FILES_DIR $folderName
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    Write-Host "Packaging $appName (v$appVersion) -> $folderName..."

    $items = @()

    # Copy files from srcDir
    $files = Get-ChildItem -Path $srcDir -File | Where-Object { $_.Extension -in @(".exe", ".dll", ".config") }
    foreach ($f in $files) {
        $destFile = $f.Name + ".deploy"
        $destPath = Join-Path $targetDir $destFile
        Copy-Item $f.FullName $destPath -Force
        $dig = Get-Digest $destPath
        $items += @{
            OrigName = $f.Name
            DestName = $destFile
            FullPath = $f.FullName
            Ext = $f.Extension.ToLower()
            Digest = $dig.Digest
            Size = $dig.Size
        }
    }

    # Resources folder if exists
    $resDir = Join-Path $srcDir "Resources"
    if (Test-Path $resDir) {
        $targetResDir = Join-Path $targetDir "Resources"
        New-Item -ItemType Directory -Path $targetResDir -Force | Out-Null
        $resFiles = Get-ChildItem -Path $resDir -File
        foreach ($rf in $resFiles) {
            $destFile = $rf.Name + ".deploy"
            $destPath = Join-Path $targetResDir $destFile
            Copy-Item $rf.FullName $destPath -Force
            $dig = Get-Digest $destPath
            $items += @{
                OrigName = "Resources\" + $rf.Name
                DestName = "Resources\" + $destFile
                FullPath = $rf.FullName
                Ext = $rf.Extension.ToLower()
                Digest = $dig.Digest
                Size = $dig.Size
            }
        }
    }

    # Main EXE asm info
    $mainExePath = Join-Path $srcDir $mainExe
    $mainAsmInfo = Get-AsmInfo $mainExePath

    # Build .exe.manifest
    $manifestName = "$appName.exe.manifest"
    $manifestPath = Join-Path $targetDir $manifestName

    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine('<?xml version="1.0" encoding="utf-8"?>')
    [void]$sb.AppendLine('<asmv1:assembly xsi:schemaLocation="urn:schemas-microsoft-com:asm.v1 assembly.adaptive.xsd" manifestVersion="1.0" xmlns:asmv1="urn:schemas-microsoft-com:asm.v1" xmlns="urn:schemas-microsoft-com:asm.v2" xmlns:asmv2="urn:schemas-microsoft-com:asm.v2" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:co.v1="urn:schemas-microsoft-com:clickonce.v1" xmlns:asmv3="urn:schemas-microsoft-com:asm.v3" xmlns:dsig="http://www.w3.org/2000/09/xmldsig#" xmlns:co.v2="urn:schemas-microsoft-com:clickonce.v2">')
    [void]$sb.AppendLine("  <asmv1:assemblyIdentity name=`"$appName.exe`" version=`"$appVersion`" publicKeyToken=`"0000000000000000`" language=`"neutral`" processorArchitecture=`"msil`" type=`"win32`" />")
    [void]$sb.AppendLine('  <application />')
    [void]$sb.AppendLine('  <entryPoint>')
    if ($mainAsmInfo.PKT) {
        [void]$sb.AppendLine("    <assemblyIdentity name=`"$($mainAsmInfo.Name)`" version=`"$($mainAsmInfo.Version)`" publicKeyToken=`"$($mainAsmInfo.PKT)`" language=`"neutral`" processorArchitecture=`"msil`" />")
    } else {
        [void]$sb.AppendLine("    <assemblyIdentity name=`"$($mainAsmInfo.Name)`" version=`"$($mainAsmInfo.Version)`" language=`"neutral`" processorArchitecture=`"msil`" />")
    }
    [void]$sb.AppendLine("    <commandLine file=`"$mainExe`" parameters=`"`" />")
    [void]$sb.AppendLine('  </entryPoint>')
    [void]$sb.AppendLine('  <trustInfo>')
    [void]$sb.AppendLine('    <security>')
    [void]$sb.AppendLine('      <applicationRequestMinimum>')
    [void]$sb.AppendLine('        <PermissionSet Unrestricted="true" ID="Custom" SameSite="site" />')
    [void]$sb.AppendLine('        <defaultAssemblyRequest permissionSetReference="Custom" />')
    [void]$sb.AppendLine('      </applicationRequestMinimum>')
    [void]$sb.AppendLine('      <requestedPrivileges xmlns="urn:schemas-microsoft-com:asm.v3">')
    [void]$sb.AppendLine('        <requestedExecutionLevel level="asInvoker" uiAccess="false" />')
    [void]$sb.AppendLine('      </requestedPrivileges>')
    [void]$sb.AppendLine('    </security>')
    [void]$sb.AppendLine('  </trustInfo>')
    [void]$sb.AppendLine('  <dependency>')
    [void]$sb.AppendLine('    <dependentOS>')
    [void]$sb.AppendLine('      <osVersionInfo>')
    [void]$sb.AppendLine('        <os majorVersion="5" minorVersion="1" buildNumber="2600" servicePackMajor="0" />')
    [void]$sb.AppendLine('      </osVersionInfo>')
    [void]$sb.AppendLine('    </dependentOS>')
    [void]$sb.AppendLine('  </dependency>')
    [void]$sb.AppendLine('  <dependency>')
    [void]$sb.AppendLine('    <dependentAssembly dependencyType="preRequisite" allowDelayedBinding="true">')
    [void]$sb.AppendLine('      <assemblyIdentity name="Microsoft.Windows.CommonLanguageRuntime" version="4.0.30319.0" />')
    [void]$sb.AppendLine('    </dependentAssembly>')
    [void]$sb.AppendLine('  </dependency>')

    $filesSection = @()
    foreach ($item in $items) {
        $orig = $item.OrigName
        $digest = $item.Digest
        $size = $item.Size
        $ext = $item.Ext
        $fullPath = $item.FullPath

        if ($ext -in @(".exe", ".dll")) {
            $asmInfo = Get-AsmInfo $fullPath
            [void]$sb.AppendLine('  <dependency>')
            [void]$sb.AppendLine("    <dependentAssembly dependencyType=`"install`" allowDelayedBinding=`"true`" codebase=`"$orig`" size=`"$size`">")
            if ($asmInfo.PKT) {
                [void]$sb.AppendLine("      <assemblyIdentity name=`"$($asmInfo.Name)`" version=`"$($asmInfo.Version)`" publicKeyToken=`"$($asmInfo.PKT)`" language=`"neutral`" processorArchitecture=`"msil`" />")
            } else {
                [void]$sb.AppendLine("      <assemblyIdentity name=`"$($asmInfo.Name)`" version=`"$($asmInfo.Version)`" language=`"neutral`" processorArchitecture=`"msil`" />")
            }
            [void]$sb.AppendLine('      <hash>')
            [void]$sb.AppendLine('        <dsig:Transforms>')
            [void]$sb.AppendLine('          <dsig:Transform Algorithm="urn:schemas-microsoft-com:HashTransforms.Identity" />')
            [void]$sb.AppendLine('        </dsig:Transforms>')
            [void]$sb.AppendLine('        <dsig:DigestMethod Algorithm="http://www.w3.org/2000/09/xmldsig#sha256" />')
            [void]$sb.AppendLine("        <dsig:DigestValue>$digest</dsig:DigestValue>")
            [void]$sb.AppendLine('      </hash>')
            [void]$sb.AppendLine('    </dependentAssembly>')
            [void]$sb.AppendLine('  </dependency>')
        } else {
            $filesSection += "  <file name=`"$orig`" size=`"$size`">`n    <hash>`n      <dsig:Transforms>`n        <dsig:Transform Algorithm=`"urn:schemas-microsoft-com:HashTransforms.Identity`" />`n      </dsig:Transforms>`n      <dsig:DigestMethod Algorithm=`"http://www.w3.org/2000/09/xmldsig#sha256`" />`n      <dsig:DigestValue>$digest</dsig:DigestValue>`n    </hash>`n  </file>"
        }
    }

    foreach ($fXml in $filesSection) {
        [void]$sb.AppendLine($fXml)
    }

    [void]$sb.AppendLine('</asmv1:assembly>')

    [System.IO.File]::WriteAllText($manifestPath, $sb.ToString(), $utf8NoBom)

    $manifestDig = Get-Digest $manifestPath

    # Build .application file
    $appSb = [System.Text.StringBuilder]::new()
    [void]$appSb.AppendLine('<?xml version="1.0" encoding="utf-8"?>')
    [void]$appSb.AppendLine('<asmv1:assembly xsi:schemaLocation="urn:schemas-microsoft-com:asm.v1 assembly.adaptive.xsd" manifestVersion="1.0" xmlns:asmv1="urn:schemas-microsoft-com:asm.v1" xmlns="urn:schemas-microsoft-com:asm.v2" xmlns:asmv2="urn:schemas-microsoft-com:asm.v2" xmlns:xrml="urn:mpeg:mpeg21:2003:01-REL-R-NS" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:asmv3="urn:schemas-microsoft-com:asm.v3" xmlns:dsig="http://www.w3.org/2000/09/xmldsig#" xmlns:co.v1="urn:schemas-microsoft-com:clickonce.v1" xmlns:co.v2="urn:schemas-microsoft-com:clickonce.v2">')
    [void]$appSb.AppendLine("  <assemblyIdentity name=`"$appName.application`" version=`"$appVersion`" publicKeyToken=`"0000000000000000`" language=`"neutral`" processorArchitecture=`"msil`" xmlns=`"urn:schemas-microsoft-com:asm.v1`" />")
    [void]$appSb.AppendLine("  <description asmv2:publisher=`"$appName`" asmv2:product=`"$appName`" xmlns=`"urn:schemas-microsoft-com:asm.v1`" />")
    [void]$appSb.AppendLine('  <deployment install="true" mapFileExtensions="true" />')
    [void]$appSb.AppendLine('  <compatibleFrameworks xmlns="urn:schemas-microsoft-com:clickonce.v2">')
    [void]$appSb.AppendLine('    <framework targetVersion="4.8" profile="Full" supportedRuntime="4.0.30319" />')
    [void]$appSb.AppendLine('  </compatibleFrameworks>')
    [void]$appSb.AppendLine('  <dependency>')
    [void]$appSb.AppendLine("    <dependentAssembly dependencyType=`"install`" codebase=`"Application Files\$folderName\$manifestName`" size=`"$($manifestDig.Size)`">")
    [void]$appSb.AppendLine("      <assemblyIdentity name=`"$appName.exe`" version=`"$appVersion`" publicKeyToken=`"0000000000000000`" language=`"neutral`" processorArchitecture=`"msil`" type=`"win32`" />")
    [void]$appSb.AppendLine('      <hash>')
    [void]$appSb.AppendLine('        <dsig:Transforms>')
    [void]$appSb.AppendLine('          <dsig:Transform Algorithm="urn:schemas-microsoft-com:HashTransforms.Identity" />')
    [void]$appSb.AppendLine('        </dsig:Transforms>')
    [void]$appSb.AppendLine('        <dsig:DigestMethod Algorithm="http://www.w3.org/2000/09/xmldsig#sha256" />')
    [void]$appSb.AppendLine("        <dsig:DigestValue>$($manifestDig.Digest)</dsig:DigestValue>")
    [void]$appSb.AppendLine('      </hash>')
    [void]$appSb.AppendLine('    </dependentAssembly>')
    [void]$appSb.AppendLine('  </dependency>')
    [void]$appSb.AppendLine('</asmv1:assembly>')

    $appContent = $appSb.ToString()

    # Write root .application
    $rootAppPath = Join-Path $ALL_ALL "$appName.application"
    [System.IO.File]::WriteAllText($rootAppPath, $appContent, $utf8NoBom)

    # Write inner .application
    $innerAppPath = Join-Path $targetDir "$appName.application"
    [System.IO.File]::WriteAllText($innerAppPath, $appContent, $utf8NoBom)

    Write-Host "  -> Done! Manifest: $($manifestDig.Size) bytes, Digest: $($manifestDig.Digest.Substring(0,10))..."
}

# 1. ERP_Khach (Portal with all 5 modules)
Package-ClickOnceApp -appName "ERP_Khach" -folderName "ERP_Khach_1_0_0_4" -appVersion "1.0.0.4" -srcDir (Join-Path $ERP_REPO "ERP_Code\ERP_BanHang\ERP_Khach\bin\Release") -mainExe "ERP_Khach.exe"

# 2. ERP_BanHang
Package-ClickOnceApp -appName "ERP_BanHang" -folderName "ERP_BanHang_1_0_0_9" -appVersion "1.0.0.9" -srcDir (Join-Path $ERP_REPO "ERP_Code\ERP_BanHang\ERP_BanHang\bin\Release") -mainExe "ERP_BanHang.exe"

# 3. ERPKho
Package-ClickOnceApp -appName "ERPKho" -folderName "ERPKho_1_0_0_1" -appVersion "1.0.0.1" -srcDir (Join-Path $ERP_REPO "ERP_Code\ERPKho\bin\Release") -mainExe "ERPKho.exe"

# 4. ERP_Logistics
Package-ClickOnceApp -appName "ERP_Logistics" -folderName "ERP_Logistics_1_0_0_0" -appVersion "1.0.0.0" -srcDir (Join-Path $ERP_REPO "ERP_Code\ERP_Logistics\bin\Release") -mainExe "ERP_Logistics.exe"

# 5. ERP_NhanSu
Package-ClickOnceApp -appName "ERP_NhanSu" -folderName "ERP_NhanSu_1_0_0_3" -appVersion "1.0.0.3" -srcDir (Join-Path $ERP_REPO "ERP_Code\ERP_NhanSu\bin\Release") -mainExe "ERP_NhanSu.exe"

# 6. KE_TOAN_TAI_CHINH
Package-ClickOnceApp -appName "KE_TOAN_TAI_CHINH" -folderName "KE_TOAN_TAI_CHINH_1_0_0_0" -appVersion "1.0.0.0" -srcDir (Join-Path $ERP_REPO "ERP_Code\KE_TOAN_TAI_CHINH\bin\Release\net48") -mainExe "KE_TOAN_TAI_CHINH.exe"

Write-Host "`nAll 6 ClickOnce packages successfully generated in D:\html\erp\All\All!"
