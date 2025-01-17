$scriptPath = $PSScriptRoot
cd $PSScriptRoot

foreach ($folder in `
    @(
    "DVBTTelevizor\DVBTTelevizor\bin",
    "DVBTTelevizor\DVBTTelevizor\obj",
    "DVBTTelevizor\DVBTTelevizor.Android\bin",
    "DVBTTelevizor\DVBTTelevizor.Android\obj",
		"LoggerService\bin",
		"LoggerService\obj",
		"MPEGTS\bin",
		"MPEGTS\obj",
		"MPEGTSAnalyzator\bin",
		"MPEGTSAnalyzator\obj",
    ".vs"
     ))
{
    $fullPath = [System.IO.Path]::Combine($scriptPath,$folder)
    if (-not $fullPath.EndsWith("\"))
    {
            $fullPath += "\"
    }

    if (Test-Path -Path $fullPath)
    {
	Remove-Item -Path $fullPath -Recurse -Force -Verbose		
    }
}
