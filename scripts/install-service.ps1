#Requires -RunAsAdministrator

param(
    [string]$PublishDir = "$PSScriptRoot",
    [string]$ServiceName = "Eudic Sync To MaiMemo Service",
    [string]$DisplayName = "Eudic Sync To MaiMemo Service"
)

$ErrorActionPreference = "Stop"
$exitCode = 0
$summaryMessage = "安装未完成"
$summaryColor = [ConsoleColor]::Red

function Show-ScriptSummary {
    param(
        [string]$Message,
        [ConsoleColor]$Color = [ConsoleColor]::White
    )

    Write-Host ""
    Write-Host "----------------------------------------"
    Write-Host $Message -ForegroundColor $Color
    Write-Host "----------------------------------------"
    [Console]::Out.Flush()
    [Console]::Error.Flush()
}

function Wait-BeforeExit {
    param([int]$Seconds = 7)

    Write-Host ""
    $countdown = $Seconds
    while ($countdown -gt 0) {
        Write-Host -NoNewline ("`r将在 {0} 秒后自动退出（按回车立即退出）" -f $countdown).PadRight(56)
        try {
            if ([Console]::KeyAvailable) {
                $key = [Console]::ReadKey($true)
                if ($key.Key -eq [ConsoleKey]::Enter) {
                    Write-Host ""
                    return
                }
            }
        }
        catch {
            # 非交互环境下 KeyAvailable 可能不可用
        }

        Start-Sleep -Seconds 1
        $countdown--
    }

    Write-Host ""
}

try {
    Write-Host "开始安装 Windows 服务..."
    Write-Host "发布目录: $PublishDir"
    Write-Host "服务名称: $ServiceName"

    $resolvedPublishDir = Resolve-Path $PublishDir
    $exePath = Join-Path $resolvedPublishDir "EudicSyncToMaiMemo.exe"
    Write-Host "程序路径: $exePath"

    if (-not (Test-Path $exePath)) {
        throw "找不到 $exePath。请将 install-service.ps1 放在程序同级目录，或指定 -PublishDir"
    }

    $existing = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($existing) {
        Write-Host "检测到同名服务，准备覆盖安装"

        if ($existing.Status -eq "Running") {
            Write-Host "正在停止现有服务..."
            Stop-Service -Name $ServiceName -Force
        }

        Write-Host "正在删除现有服务..."
        sc.exe delete $ServiceName | Out-Null
        if ($LASTEXITCODE -ne 0) {
            throw "删除现有服务失败，退出码: $LASTEXITCODE"
        }

        Start-Sleep -Seconds 2
    }
    else {
        Write-Host "未检测到同名服务，执行首次安装"
    }

    Write-Host "正在创建服务..."
    New-Service -Name $ServiceName `
        -BinaryPathName "`"$exePath`"" `
        -DisplayName $DisplayName `
        -Description "将欧路词典收集的生词同步到墨墨背单词云词库" `
        -StartupType Automatic | Out-Null

    Write-Host "正在启动服务..."
    Start-Service -Name $ServiceName

    $summaryMessage = "服务已安装并启动: $ServiceName"
    $summaryColor = [ConsoleColor]::Green
    Write-Host $summaryMessage -ForegroundColor Green
}
catch {
    $exitCode = 1
    $summaryMessage = "安装失败: $($_.Exception.Message)"
    $summaryColor = [ConsoleColor]::Red
    Write-Host $summaryMessage -ForegroundColor Red
}
finally {
    Show-ScriptSummary -Message $summaryMessage -Color $summaryColor
    Wait-BeforeExit -Seconds 7
    exit $exitCode
}
