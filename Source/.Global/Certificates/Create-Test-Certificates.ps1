Import-Module "$($PSScriptRoot)\Certificate-Module";

$_maximumDate = [DateTime]::MaxValue;
$_minimumDate = [DateTime]::MinValue.AddYears(1899);

New-EncryptionCertificate `
	-Name "Key-Protection-Certificate" `
	-NotAfter $_maximumDate `
	-NotBefore $_minimumDate;