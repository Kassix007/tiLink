Write-Host "Starting Backend..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd backend; dotnet run"

Start-Sleep -Seconds 3

Write-Host "Starting UI..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd UI; npm run dev"

Write-Host ""
Write-Host "-----------------------------------"
Write-Host " UI:      http://localhost:5173/"
Write-Host " Backend: https://localhost:7125/"
Write-Host "-----------------------------------"
