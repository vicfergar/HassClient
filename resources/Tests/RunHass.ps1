$version = $( Read-Host "Input Hass version, please" )

if ([string]::IsNullOrEmpty($version))
{
	$j = Invoke-WebRequest 'https://api.github.com/repos/AlexxIT/HassWP/releases/latest' | ConvertFrom-Json
}
else
{
	$j = Invoke-WebRequest "https://api.github.com/repos/AlexxIT/HassWP/releases/tags/$version" | ConvertFrom-Json
}

Write-Output $j.tag_name

$h = Invoke-WebRequest $j.assets_url | ConvertFrom-Json
$fileUrl = $h.browser_download_url
Write-Output $fileUrl

$workingDir = ".testEnv"
New-Item -Path $workingDir -ItemType Container -Force
$workingDir = (Get-Item $workingDir).FullName

$filename = $workingDir + "\" + [System.IO.Path]::GetFileName($h.browser_download_url)
$folderName = $workingDir + "\" + [System.IO.Path]::GetFileNameWithoutExtension($filename)

if(!(Test-Path $folderName -PathType Container))
{
	Write-Output "HassWP folder not found"
	if(!(Test-Path $filename))
	{
		# Download
		Write-Output "Downloading..."
		$tempFile = $workingDir + "\temp.zip"
		Remove-Item $tempFile -Force -ErrorAction Ignore
		
		Invoke-WebRequest $fileUrl -OutFile $tempFile
		Rename-Item -Path $tempFile -NewName $filename
	}
	
	# Unzip
	Write-Output "Unziping..."
	$tempFolder = $workingDir + "\temp"
	Remove-Item $tempFolder -Recurse -Force -ErrorAction Ignore
	
	Add-Type -AssemblyName System.IO.Compression.FileSystem
	[System.IO.Compression.ZipFile]::ExtractToDirectory($filename, $tempFolder)
	Rename-Item -Path $tempFolder -NewName $folderName
}

# Reset config
Start-Process -FilePath "$folderName\config\reset.cmd" -WorkingDirectory "$folderName\config\" -NoNewWindow -Wait

# Copy config
Copy-Item -Path "config\*" -Destination "$folderName\config\" -Recurse -Force

#Run
Start-Process -FilePath "$folderName\hass.cmd" -WorkingDirectory "$folderName"