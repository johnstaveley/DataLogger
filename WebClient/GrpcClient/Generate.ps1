npm install
# Add tools to path
if ($env:path -notmatch "grpc-tools") {
	Write-Host "*** Adding grpc-tools to path"
	$env:path = $env:path + ";./node_modules/.bin/;./node_modules/grpc-tools/bin/;./node_modules/protoc-gen-grpc-web/bin/;"
}
# Remove old proxy files
Remove-Item datalog_pb.js -force
Remove-Item datalog_grpc_web_pb.js -force
Write-Host "*** Generating new proxy files"
protoc.exe -I ..\..\ `
	--js_out=import_style=commonjs:.\ `
	--grpc-web_out=import_style=commonjs,mode=grpcwebtext:.\ `
	..\..\GrpcService\Protos\datalog.proto
# Move files to root of directory
$files = Get-ChildItem 'GrpcService/Protos/*'
Get-ChildItem $files | Move-Item -Destination { $_.Directory.Parent.Parent.FullName }
rmdir GrpcService -recurse -force
# Build proxy files in development mode (This will prompt you to install some packages on the first run)
Write-Host "*** Building proxy files"
npx webpack -o ../wwwroot/js/datalog/ ./send.js --mode development
Write-Host "*** Complete"