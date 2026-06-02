#Requires -RunAsAdministrator

param(
    [string]$ServiceName = "Eudic Sync To MaiMemo Service"
)

$ErrorActionPreference = "Stop"
$exitCode = 0
$summaryMessage = "卸载未完成"
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
    Write-Host "开始卸载 Windows 服务..."
    Write-Host "服务名称: $ServiceName"

    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if (-not $service) {
        $summaryMessage = "服务不存在: $ServiceName"
        $summaryColor = [ConsoleColor]::Yellow
        Write-Host $summaryMessage -ForegroundColor Yellow
        return
    }

    Write-Host "当前状态: $($service.Status)"

    if ($service.Status -eq "Running") {
        Write-Host "正在停止服务..."
        Stop-Service -Name $ServiceName -Force
    }

    Write-Host "正在删除服务..."
    sc.exe delete $ServiceName | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "删除服务失败，退出码: $LASTEXITCODE"
    }

    $summaryMessage = "服务已卸载: $ServiceName"
    $summaryColor = [ConsoleColor]::Green
    Write-Host $summaryMessage -ForegroundColor Green
}
catch {
    $exitCode = 1
    $summaryMessage = "卸载失败: $($_.Exception.Message)"
    $summaryColor = [ConsoleColor]::Red
    Write-Host $summaryMessage -ForegroundColor Red
}
finally {
    Show-ScriptSummary -Message $summaryMessage -Color $summaryColor
    Wait-BeforeExit -Seconds 7
    exit $exitCode
}
